#include "Serializer.h"

#include "Logger.h"
#include "../ECS/Components/Components.h"
#include "ModelResource.h"
#include <typeinfo>
#include <filesystem>

#include "../ECS/Components/Reflection_for_components.h"

using namespace Components;
namespace PogplantDriver
{
	Serializer::Serializer(ECS& ecs) :
	m_ecs{ ecs },
	m_child_counter()
	{
	}
	void Serializer::Save(const std::string& File)
	{
		SaveObjects(File);
	}

	bool Serializer::Load(const std::string& File)
	{
		LoadObjects(File);
		return true;
	}

	void Serializer::SavePrefab(const std::string& File, entt::entity id)
	{
		std::ofstream ostream(File, std::ios::out);

		int i = 0;
		if (ostream.is_open())
		{
			Json::Value root;
			Json::Value subroot = SaveComponents(id);
			root[i] = subroot;

			auto& transform = m_ecs.GetReg().get<Transform>(id);

			if (!transform.m_children.empty())
				RecurSaveChild(root, id, ++i);

			m_saved.clear();
			Json::StreamWriterBuilder builder;
			Json::StreamWriter* writer = builder.newStreamWriter();

			writer->write(root, &ostream);

			delete writer;
			ostream.close();
		}
	}

	void Serializer::LoadPrefab(const std::string& File, bool IsPrefab)
	{
		LoadObjects(File, IsPrefab);
	}

	entt::entity Serializer::Instantiate(const std::string& Filename, glm::vec3 _Position, glm::vec3 _Rotation)
	{
		std::filesystem::path path = std::filesystem::current_path();
		path += "\\Resources\\Prefabs\\" + Filename + ".prefab";
		std::ifstream istream(path, std::ios::in);
		entt::entity first_entity = entt::null;
		if (istream.is_open())
		{
			Json::Value root;
			istream >> root;
			first_entity = m_ecs.GetReg().create();

			Json::ValueIterator iter = root.begin();
			bool isfirst = true;

			while (iter != root.end())
			{
				Json::Value subroot = root[iter.index()];

				if (isfirst)
				{
					LoadComponents(subroot, first_entity);
					isfirst = false;
				}
				else
				{
					LoadComponents(subroot, m_ecs.GetReg().create());
				}
				// Load components
				++iter;
			}
			istream.close();

			//Instantiate given position, rotation
			auto transform = m_ecs.GetReg().try_get<Transform>(first_entity);
			transform->m_position = _Position;
			transform->m_rotation = _Rotation;
		}
		else
		{
			Pogplant::Logger::Log({ "Serialiser::Instantiate",Pogplant::LogEntry::LOGTYPE::ERROR, "Failed to Instantiate prefab" });
			assert(first_entity == entt::null && "Failed to Instantiate prefab");
		}
		return first_entity;
	}

	void Serializer::SaveObjects(const std::string& File)
	{
		std::ofstream ostream(File, std::ios::out);
		
		if (ostream.is_open())
		{
			Json::Value root;
			int  i = 0;
			entt::entity parent = entt::null;
			entt::entity prev = entt::null;

			Components::Transform* parentPtr;
			auto entities = m_ecs.view<Transform>();
			for (auto entity = entities.rbegin(); entity != entities.rend(); ++entity)
			{
				if (m_saved.contains(*entity))
					continue;

				auto& transform = m_ecs.GetReg().get<Transform>(*entity);
				if (transform.m_parent != entt::null)
				{
					parent = transform.m_parent;
					parentPtr = &transform;

					while (parentPtr && parentPtr->m_parent != entt::null)
					{
						prev = parent;
						parentPtr = &m_ecs.GetReg().get<Transform>(parent);
						parent = parentPtr->m_parent;
					}

					Json::Value subroot = SaveComponents(prev);
					root[i] = subroot;
					m_saved.insert(prev);
					i = RecurSaveChild(root, prev, ++i);
					continue;
				}
				else if (!transform.m_children.empty())
				{
					Json::Value subroot = SaveComponents(*entity);
					root[i] = subroot;
					i = RecurSaveChild(root, *entity, ++i);
					continue;
				}
				
				//Standard no relationship
				Json::Value subroot = SaveComponents(*entity);
				root[i] = subroot;
				++i;
			}

			m_saved.clear();
			Json::StreamWriterBuilder builder;
			Json::StreamWriter* writer = builder.newStreamWriter();

			writer->write(root, &ostream);

			delete writer;
			ostream.close();
		}

	}

	Json::Value Serializer::SaveComponents(entt::entity id)
	{
		Json::Value subroot;

		//Get all pointers to all components

		auto& transform_component = m_ecs.GetReg().get<Transform>(id);
		auto render_component = m_ecs.GetReg().try_get<Renderer>(id);
		auto script_component = m_ecs.GetReg().try_get<Scriptable>(id);
		auto audio_component = m_ecs.GetReg().try_get<AudioSource>(id);

		Try_Save_Component<Transform>(subroot, id);
		Try_Save_Component<Name>(subroot, id);
		Try_Save_Component<Point_Light>(subroot, id);
		Try_Save_Component<Directional_Light>(subroot, id);
		Try_Save_Component<Text>(subroot, id);
		Try_Save_Component<SphereCollider>(subroot, id);
		Try_Save_Component<BoxCollider>(subroot, id);
		Try_Save_Component<Camera>(subroot, id);
		Try_Save_Component<Rigidbody>(subroot, id);
		Try_Save_Component<ParticleSystem>(subroot, id);

		if (!transform_component.m_children.empty() || transform_component.m_parent != entt::null)
		{
			subroot["Children"] = transform_component.m_children.size();
		}

		if (render_component)
		{
			Json::Value classroot;

			AddVec3To(classroot, "ColorTint", render_component->m_ColorTint);
			AddVec3To(classroot, "EmissiveTint", render_component->m_EmissiveTint);

			classroot["UseLight"] = render_component->m_UseLight;

			if (render_component->m_Mesh)
			{
				Json::Value temp(Json::arrayValue);
				temp.append(render_component->m_RenderModel->m_Model_key);
				temp.append(render_component->m_RenderModel->m_Directory);
				temp.append(render_component->m_Mesh->m_Name);
				classroot["RenderModel"] = temp;
			}

			subroot["Render"] = classroot;
		}

		if (script_component)
		{
			Json::Value classroot;
			for (auto& script_name : script_component->m_ScriptTypes)
			{
				classroot[script_name.first] = script_name.second;
			}
			subroot["Scripting"] = classroot;
		}

		if (audio_component)
		{
			Json::Value classRoot;

			for (size_t i = 0; i < audio_component->m_audioSources.size(); i++)
			{
				Json::Value clipRoot;
				
				clipRoot["UpdatePos"] = audio_component->m_audioSources[0].m_update3DPosition;
				clipRoot["Volume"] = audio_component->m_audioSources[0].m_volume;
				clipRoot["Dir"] = audio_component->m_audioSources[0].m_fileDir;

				std::stringstream ss;
				ss << "Clip" << i;
				classRoot[ss.str()] = clipRoot;
			}

			subroot["AudioSource"] = classRoot;
		}

		return subroot;
	}

	void Serializer::LoadObjects(const std::string& File, bool IsPrefab)
	{
		std::ifstream istream(File, std::ios::in);

		if (istream.is_open())
		{
			Json::Value root;
			istream >> root;

			Json::ValueIterator iter = root.begin();

			while (iter != root.end())
			{
				Json::Value subroot = root[iter.index()];

				// Load components
				LoadComponents(subroot, m_ecs.GetReg().create(), IsPrefab);

				++iter;
			}

			istream.close();
		}
	}

	void Serializer::LoadComponents(const Json::Value& root, entt::entity id, bool IsPrefab)
	{

		auto& render = root["Render"];
		auto& relationship = root["Children"];
		auto& scripting = root["Scripting"];
		auto& audioSource = root["AudioSource"];

		if (IsPrefab)
		{
			m_ecs.GetReg().emplace<Prefab>(id);
		}

		Try_Load_Component<ParticleSystem>(root, "ParticleSystem", id);

		Try_Load_Component<Transform>(root, "Transform", id);

		Try_Load_Component<Directional_Light>(root, "Directional_Light", id);
		Try_Load_Component<Point_Light>(root, "Point_Light", id);
		Try_Load_Component<Name>(root, "Name", id);
		Try_Load_Component<Text>(root, "Text", id);
		Try_Load_Component<BoxCollider>(root, "BoxCollider", id);
		Try_Load_Component<SphereCollider>(root, "SphereCollider", id);
		Try_Load_Component<Camera>(root, "Camera", id);
		Try_Load_Component<Rigidbody>(root, "Rigidbody", id);

		if (relationship)
		{
			bool is_another_parent = false;
			auto& transform = m_ecs.GetReg().get<Transform>(id);
			int child = relationship.asInt();

			//Starting case
			if (child != 0 && m_parent_id.empty())
			{
				m_child_counter.push(child);
				m_parent_id.push(id);
			}
			//Base Child only case, Seek parent
			else if (child == 0 && !m_parent_id.empty())
			{
				auto& relationshipTrans = m_ecs.GetReg().get<Transform>(m_parent_id.top());
				relationshipTrans.m_children.insert(id);
				transform.m_parent = m_parent_id.top();

				--m_child_counter.top();
			}
			else
			{
				auto& relationshipTrans = m_ecs.GetReg().get<Transform>(m_parent_id.top());
				relationshipTrans.m_children.insert(id);
				transform.m_parent = m_parent_id.top();

				--m_child_counter.top();

				is_another_parent = true;
			}
			if (!m_child_counter.empty() && m_child_counter.top() <= 0)
			{
				m_child_counter.pop();
				m_parent_id.pop();
			}
			if (is_another_parent)
			{
				m_child_counter.push(child);
				m_parent_id.push(id);
			}
		}

		if (render)
		{
			//glm::vec3 _colorTint = ;
			Pogplant::Model* model = nullptr;
			Pogplant::Mesh3D* mesh = nullptr;

			if (render["RenderModel"] != Json::nullValue)
			{
				auto result = Pogplant::ModelResource::m_ModelPool.find(render["RenderModel"][0].asString());

				if (result != Pogplant::ModelResource::m_ModelPool.end())
				{
					model = result->second;

					// Find mesh
					for (auto& it : model->m_Meshes)
					{
						if (it.second.m_Name == render["RenderModel"][2].asString())
						{
							mesh = &it.second;
						}
					}

					assert(mesh != nullptr);
				}
				else
				{
					std::cout	<< "Unable to find " << render["RenderModel"][1].asString()
								<< " (" << render["RenderModel"][0].asString() << ")" << std::endl;
				}
			}

			m_ecs.GetReg().emplace<Renderer>(
				id,
				glm::vec3{ render["ColorTint"][0].asFloat(),render["ColorTint"][1].asFloat(),render["ColorTint"][2].asFloat() },
				glm::vec3{ render["EmissiveTint"][0].asFloat(),render["EmissiveTint"][1].asFloat(),render["EmissiveTint"][2].asFloat() },
				model,
				mesh,
				render["UseLight"].asInt()
				);
		}

		if (scripting)
		{
			auto members = scripting.getMemberNames();
			std::unordered_map<std::string, bool> temp_ScriptTypes;
			for (auto it = members.begin(); it != members.end(); ++it)
			{
				temp_ScriptTypes.emplace(it->c_str(), scripting[*it].asBool());
			}
			m_ecs.GetReg().emplace<Scriptable>(id, temp_ScriptTypes);
		}

		if (audioSource)
		{
			auto& audioComponent = m_ecs.GetReg().emplace<AudioSource>(id);
			
			std::stringstream ss;
			
			size_t i = 0;
			ss << "Clip" << i;

			do
			{
				
				if (audioSource[ss.str()] != Json::nullValue)
				{
					AudioSource::AudioObject audioObject;
					audioObject.m_update3DPosition = audioSource[ss.str()]["UpdatePos"].asBool();
					audioObject.m_volume = audioSource[ss.str()]["Volume"].asFloat();
					audioObject.m_fileDir = audioSource[ss.str()]["Dir"].asString();

					if(audioObject.Init())
						audioComponent.m_audioSources.push_back(audioObject);
				}

				++i;
				ss.str("");
				ss.clear();
				ss << "Clip" << i;
			} while (audioSource[ss.str()] != Json::nullValue);
		}
	}

	int Serializer::RecurSaveChild(Json::Value& _classroot, entt::entity id,int counter)
	{
		auto& transform = m_ecs.GetReg().get<Transform>(id);
		if (!transform.m_children.empty())
		{
			for (auto& child : transform.m_children)
			{
				_classroot[counter++] = SaveComponents(child);
				m_saved.insert(child);

				auto& childTransform = m_ecs.GetReg().get<Transform>(child);

				if(!childTransform.m_children.empty())
					counter = RecurSaveChild(_classroot, child, counter);
			}
		}
		return counter;
	}

	void Serializer::AddVec2To(Json::Value& _classroot, std::string _string, glm::vec2& _vec2)
	{
		static Json::Value data(Json::arrayValue);

		data.append(_vec2.x);
		data.append(_vec2.y);

		_classroot[_string] = data;

		data.clear();
	}

	glm::vec2 Serializer::CreateVec2(const Json::Value& _data)
	{
		if (_data[0] && _data[1])
			return glm::vec2{ _data[0].asFloat(), _data[1].asFloat()};

		std::cout << _data.asCString() << " is not a vec 2" << std::endl;

		return glm::vec2{};

	}

	ImVec2 Serializer::CreateImVec2(const Json::Value& _data)
	{
		if (_data[0] && _data[1])
			return ImVec2{ _data[0].asFloat(), _data[1].asFloat() };

		std::cout << _data.asCString() << " is not a vec 2" << std::endl;

		return ImVec2{};

	}

	void Serializer::AddVec3To(Json::Value& _classroot, std::string _string, glm::vec3& _vec3)
	{
		static Json::Value data(Json::arrayValue);

		data.append(_vec3.x);
		data.append(_vec3.y);
		data.append(_vec3.z);

		_classroot[_string] = data;

		data.clear();
	}

	glm::vec3 Serializer::CreateVec3(const Json::Value& _data)
	{
		if (_data[0] && _data[1] && _data[2])
			return glm::vec3{ _data[0].asFloat(), _data[1].asFloat(), _data[2].asFloat() };

		std::cout << _data.asCString() << " is not a vec 3" << std::endl;

		return glm::vec3{};

	}

	void Serializer::AddVec4To(Json::Value& _classroot, std::string _string, glm::vec4& _vec4)
	{
		static Json::Value data(Json::arrayValue);

		data.append(_vec4.x);
		data.append(_vec4.y);
		data.append(_vec4.z);
		data.append(_vec4.w);

		_classroot[_string] = data;

		data.clear();
	}

	glm::vec4 Serializer::CreateVec4(const Json::Value& _data)
	{
		if (_data[0] && _data[1] && _data[2] && _data[3])
			return glm::vec4{ _data[0].asFloat(), _data[1].asFloat(), _data[2].asFloat(), _data[3].asFloat() };

		std::cout << _data.asCString() << " is not a vec 3" << std::endl;

		return glm::vec4{};
	}

	void Serializer::AddMat4To(Json::Value& _classroot, std::string _string, glm::mat4& _mat4)
	{
		static Json::Value mat4_data(Json::arrayValue);

		for (auto i = 0; i < 4; i++)
			for (auto j = 0; j < 4; j++)
				mat4_data.append(_mat4[i][j]);

		_classroot[_string] = mat4_data;

		mat4_data.clear();
	}

	glm::mat4 Serializer::CreateMat4(const Json::Value& _data)
	{
		for (int i = 0; i < 16; ++i)
		{
			if (!_data[i])
				return glm::mat4{};
		}
		return glm::mat4{ _data[0].asFloat(),	_data[1].asFloat(), _data[2].asFloat(), _data[3].asFloat(),
												_data[4].asFloat(), _data[5].asFloat(), _data[6].asFloat(), _data[7].asFloat(),
												_data[8].asFloat(), _data[9].asFloat(), _data[10].asFloat(), _data[11].asFloat(),
												_data[12].asFloat(), _data[13].asFloat(), _data[14].asFloat(), _data[15].asFloat() };
	}

	Json::Value Serializer::Reflect_Serialization(Json::Value& _root, rttr::instance _obj, bool save_comp_name)
	{
		rttr::instance obj = _obj.get_type().get_raw_type().is_wrapper() ? _obj.get_wrapped_instance() : _obj;
		const auto component_name = obj.get_type().get_raw_type().get_name().to_string();

		auto prop_list = obj.get_derived_type().get_properties();
		
		Json::Value root;

		for (auto prop : prop_list)
		{
			if (prop.get_metadata("NO_SERIALIZE"))
				continue;

			rttr::variant prop_value = prop.get_value(obj);

			if (!prop_value)
				continue; // cannot serialize, because we cannot retrieve the value

			const auto name = prop.get_name().to_string();
			auto variable_type = prop_value.get_type();

			if (prop_value.is_sequential_container())
			{
				WriteSeqContainer(prop_value.create_sequential_view(), root[name]);
			}
			else if (prop_value.is_associative_container())
			{
				std::cout << "no support for associative container yet" << std::endl;
			}
			else if (prop_value.is_type<ParticleSystem::CurveVariable>())
			{
				Reflect_Serialization(root[name], prop_value.get_value<ParticleSystem::CurveVariable>(), false);
			}
			else if (prop_value.is_type<glm::vec2>())
			{
				AddVec2To(root, name, prop_value.get_value<glm::vec2>());
			}
			else if (prop_value.is_type<glm::vec3>())
			{
				AddVec3To(root, name, prop_value.get_value<glm::vec3>());
			}
			else if (prop_value.is_type<glm::vec4>())
			{
				AddVec4To(root, name, prop_value.get_value<glm::vec4>());
			}
			else if (prop_value.is_type<glm::mat4>())
			{
				AddMat4To(root, name, prop_value.get_value<glm::mat4>());
			}
			else if (prop_value.is_type<std::string>())
			{
				root[name] = prop_value.get_value<std::string>();
			}
			else if (variable_type.is_arithmetic())
			{
				if(!Save_arithmetic(root, name, variable_type, prop_value))
					std::cout << name << " is an unsupported arithmetic type" << std::endl;
			}
			else
			{
				std::cout << name << " is somehow not supported" << std::endl;
			}
		}

		if(save_comp_name)
			return _root[component_name] = root;
		else
			return _root = root;
	}

	void Serializer::Reflect_Deserialization(rttr::instance _obj, const Json::Value& _data)
	{
		rttr::instance obj = _obj.get_type().get_raw_type().is_wrapper() ? _obj.get_wrapped_instance() : _obj;
		const auto component_name = obj.get_type().get_raw_type().get_name().to_string();

		auto prop_list = obj.get_derived_type().get_properties();


		//if (component_name == "ParticleSystem")
		//	int k = 0;


		for (auto prop : prop_list)
		{
			if (prop.get_metadata("NO_SERIALIZE"))
				continue;

			rttr::variant prop_value = prop.get_value(obj);

			if (!prop_value)
				continue; // cannot serialize, because we cannot retrieve the value

			const auto name = prop.get_name().to_string();
			auto variable_type = prop_value.get_type();


			if (_data[name])
			{
				if (prop_value.is_type<ParticleSystem::CurveVariable>())
				{
					auto temp_obj = prop_value.get_value<ParticleSystem::CurveVariable>();
					Reflect_Deserialization(temp_obj, _data[name]);
					prop.set_value(obj, temp_obj);
				}
				else if (prop_value.is_type<glm::vec2>())
				{
					prop.set_value(obj, CreateVec2(_data[name]));
				}
				else if (prop_value.is_type<glm::vec3>())
				{
					prop.set_value(obj, CreateVec3(_data[name]));
				}
				else if (prop_value.is_type<glm::vec4>())
				{
					prop.set_value(obj, CreateVec4(_data[name]));
				}
				else if (prop_value.is_type<std::string>())
				{
					prop.set_value(obj, _data[name].asString());
				}
				else if (variable_type.is_arithmetic())
				{
					if (!Load_arithmetic(variable_type, prop, obj, _data[name]))
						std::cout << name << " is an unsupported arithmetic type" << std::endl;
				}
				else if (prop_value.is_sequential_container())
				{
					rttr::variant var = prop.get_value(obj);
					auto view = var.create_sequential_view();

					CreateSeqContainer(view, _data[name]);
					//for (const auto& item : view)
					//{
					//	std::cout << "data: " << item.convert<ImVec2>().x << ", " << item.convert<ImVec2>().y << std::endl;
					//}
					prop.set_value(obj, var);

				}
				else if (prop_value.is_associative_container())
				{
					//int k = 0;
				}
				else
				{
					std::cout << name << "'s type is not supported" << std::endl;
				}
			}
		}
		_obj.get_derived_type().get_method("init").invoke(obj);
	}

	//basically checking all the basic types lol
	bool Serializer::Save_arithmetic(Json::Value& _root, const std::string& _name, rttr::type _type, rttr::variant& _value)
	{
		if (_type == rttr::type::get<bool>())
			_root[_name] = _value.to_bool();
		else if (_type == rttr::type::get<char>())
			_root[_name] = _value.to_bool();

		else if (_type == rttr::type::get<int8_t>())
			_root[_name] = _value.to_int8();
		else if (_type == rttr::type::get<int16_t>())
			_root[_name] = _value.to_int16();
		else if (_type == rttr::type::get<int32_t>())
			_root[_name] = _value.to_int32();
		else if (_type == rttr::type::get<int64_t>())
			_root[_name] = _value.to_int64();

		else if (_type == rttr::type::get<uint8_t>())
			_root[_name] = _value.to_uint8();
		else if (_type == rttr::type::get<uint16_t>())
			_root[_name] = _value.to_uint16();
		else if (_type == rttr::type::get<uint32_t>())
			_root[_name] = _value.to_uint32();
		else if (_type == rttr::type::get<uint64_t>())
			_root[_name] = _value.to_uint64();

		else if (_type == rttr::type::get<float>())
			_root[_name] = _value.to_float();
		else if (_type == rttr::type::get<double>())
			_root[_name] = _value.to_double();
		else
			return false;

		return true;
	}

	//basically checking all the basic types but for array
	bool Serializer::Save_arithmetic_array(Json::Value& _root, const std::string& _name, rttr::type _type, rttr::variant& _value)
	{
		if (_type == rttr::type::get<bool>())
			_root[_name].append(_value.to_bool());
		else if (_type == rttr::type::get<char>())
			_root[_name].append(_value.to_bool());

		else if (_type == rttr::type::get<int8_t>())
			_root[_name].append(_value.to_int8());
		else if (_type == rttr::type::get<int16_t>())
			_root[_name].append(_value.to_int16());
		else if (_type == rttr::type::get<int32_t>())
			_root[_name].append(_value.to_int32());
		else if (_type == rttr::type::get<int64_t>())
			_root[_name].append(_value.to_int64());

		else if (_type == rttr::type::get<uint8_t>())
			_root[_name].append(_value.to_uint8());
		else if (_type == rttr::type::get<uint16_t>())
			_root[_name].append(_value.to_uint16());
		else if (_type == rttr::type::get<uint32_t>())
			_root[_name].append(_value.to_uint32());
		else if (_type == rttr::type::get<uint64_t>())
			_root[_name].append(_value.to_uint64());

		else if (_type == rttr::type::get<float>())
			_root[_name].append(_value.to_float());
		else if (_type == rttr::type::get<double>())
			_root[_name].append(_value.to_double());
		else
			return false;

		return true;
	}

	bool Serializer::Load_arithmetic(rttr::type _type, rttr::property& _prop, rttr::instance& _obj, const Json::Value& _data)
	{
		if (_type == rttr::type::get<bool>())
			_prop.set_value(_obj, _data.asBool());
		else if (_type == rttr::type::get<char>())
			_prop.set_value(_obj, _data.asBool());

		else if (_type == rttr::type::get<int8_t>() ||
				_type == rttr::type::get<int16_t>() ||
				_type == rttr::type::get<int32_t>())
			_prop.set_value(_obj, _data.asInt());
		else if (_type == rttr::type::get<int64_t>())
			_prop.set_value(_obj, _data.asInt64());

		else if (_type == rttr::type::get<uint8_t>() ||
			_type == rttr::type::get<uint16_t>() ||
			_type == rttr::type::get<uint32_t>())
			_prop.set_value(_obj, _data.asUInt());
		else if (_type == rttr::type::get<uint64_t>())
			_prop.set_value(_obj, _data.asUInt64());

		else if (_type == rttr::type::get<float>())
			_prop.set_value(_obj, _data.asFloat());
		else if (_type == rttr::type::get<double>())
			_prop.set_value(_obj, _data.asDouble());
		else
			return false;

		return true;
	}

	Json::Value Serializer::WriteSeqContainer(const rttr::variant_sequential_view& _view, Json::Value& _root)
	{
		for (const auto& item : _view)
		{
			if (item.is_sequential_container())
			{
				WriteSeqContainer(item.create_sequential_view(), _root);
			}
			else
			{
				Json::Value data(Json::arrayValue);


				rttr::variant wrapped_var = item.extract_wrapped_value();
				rttr::type value_type = wrapped_var.get_type();
				auto name = value_type.get_name().to_string();
				if (value_type == rttr::type::get<std::string>())
				{
					_root[name].append(wrapped_var.to_string());
				}
				else if (value_type == rttr::type::get<glm::vec2>() || value_type == rttr::type::get<ImVec2>())
				{
					//AddVec2To(_root, name, wrapped_var.get_value<glm::vec2>(), true);
					auto _vec2 = wrapped_var.get_value<glm::vec2>();
					data.append(_vec2.x);
					data.append(_vec2.y);
					
					_root.append(data);

				}
				else if (value_type == rttr::type::get<glm::vec3>())
				{
					auto _vec3 = wrapped_var.get_value<glm::vec3>();
					data.append(_vec3.x);
					data.append(_vec3.y);
					data.append(_vec3.y);

					_root.append(data);
				}
				else if (value_type.is_arithmetic())
				{
					if (!Save_arithmetic_array(_root, name, value_type, wrapped_var))
						std::cout << name << " is an unsupported arithmetic type" << std::endl;

				}
			}
		}
		return _root;
	}


	rttr::variant extract_basic_types(const Json::Value& json_value)
	{
		switch (json_value.type())
		{
		case Json::stringValue:
		{
			return std::string(json_value.asString());
			break;
		}
		case Json::nullValue:     break;
		case Json::booleanValue:
		{
			return json_value.asBool();
			break;
		}
		case Json::intValue:
		{
			return json_value.asInt();
			break;
		}
		case Json::uintValue:
		{
			return json_value.asUInt();
			break;
		}
		case Json::realValue:
		{
			return json_value.asDouble();
			break;
		}
		// we handle only the basic types here
		case Json::objectValue:
		case Json::arrayValue: 
			return rttr::variant();
		}
		return rttr::variant();

	}

	void Serializer::CreateSeqContainer(rttr::variant_sequential_view& view, const Json::Value& json_array_value)
	{
		Json::ArrayIndex max = json_array_value.size();

		view.set_size(max);
		const rttr::type array_value_type = view.get_rank_type(1);
		const rttr::type array_value_type2 = view.get_rank_type(0);
		for (Json::ArrayIndex i = 0; i < max; i++)
		{
			auto& json_index_value = json_array_value[i];
			if (json_index_value.isArray())
			{
				auto test = view.get_value(i);

				if (test.can_convert<ImVec2>())
					view.set_value(i, CreateImVec2(json_index_value));

				auto sub_array_view = test.create_sequential_view();

			}
			else if (json_index_value.isObject())
			{
				std::cout << "CreateSeqContainer does not support isObject" << std::endl;
				//rttr::variant var_tmp = view.get_value(i);
				//rttr::variant wrapped_var = var_tmp.extract_wrapped_value();
				//fromjson_recursively(wrapped_var, json_index_value);
				//view.set_value(i, wrapped_var);
			}
			else
			{
				rttr::variant extracted_value = extract_basic_types(json_index_value);
				//auto asd = json_index_value.type();
				//if (extracted_value.convert(array_value_type))
				if (!view.set_value(i, extracted_value))
					int k = 0;
			}
		}

		//int k = 0;
	}
}

