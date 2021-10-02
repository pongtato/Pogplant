#include "Serializer.h"


#include "../ECS/Components/Components.h"
#include "ModelResource.h"
#include <typeinfo>
#include <fstream>

using namespace Components;
namespace PogplantDriver
{
	Serializer::Serializer() : 
	m_child_counter()
	{
	}
	void Serializer::Save(const std::string& File)
	{
		//std::ofstream ostream(File, std::ios::out);
		//if (ostream.is_open())
		//{
		//	Json::Value root;
		//	Json::StreamWriterBuilder builder;
		//	Json::StreamWriter* writer = builder.newStreamWriter();

		//	writer->write(root, &ostream);

		//	delete writer;
		//	ostream.close();
		//}
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
			auto relationship_component = ImguiHelper::m_ecs->GetReg().try_get<Relationship>(id);
			if (relationship_component)
			{
				RecurSaveChild(root, id, ++i);
			}


			Json::StreamWriterBuilder builder;
			Json::StreamWriter* writer = builder.newStreamWriter();

			writer->write(root, &ostream);

			delete writer;
			ostream.close();
		}
	}

	void Serializer::LoadPrefab(const std::string& File)
	{
		LoadObjects(File);
	}

	void Serializer::SaveObjects(const std::string& File)
	{
		std::ofstream ostream(File, std::ios::out);
		
		if (ostream.is_open())
		{
			Json::Value root;
			int  i = 0;
			
			auto entities = ImguiHelper::m_ecs->GetReg().view<Transform>();
			for(auto entity = entities.rbegin(); entity != entities.rend() ; ++entity)
			{
				Json::Value subroot = SaveComponents(*entity);
				root[i] = subroot;
				++i;
			}
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
		auto transform_component = ImguiHelper::m_ecs->GetReg().try_get<Transform>(id);
		auto name_component = ImguiHelper::m_ecs->GetReg().try_get<Name>(id);
		auto position_component = ImguiHelper::m_ecs->GetReg().try_get<PositionList>(id);
		auto relationship_component = ImguiHelper::m_ecs->GetReg().try_get<Relationship>(id);
		auto render_component = ImguiHelper::m_ecs->GetReg().try_get<Renderer>(id);
		auto point_light_component = ImguiHelper::m_ecs->GetReg().try_get<Point_Light>(id);
		auto directional_light_component = ImguiHelper::m_ecs->GetReg().try_get<Directional_Light>(id);

		if (relationship_component)
		{
			subroot["Children"] = relationship_component->m_children.size();
		}

		if (transform_component)
		{
			Reflect_Serialization(subroot, transform_component);
		}

		if (name_component)
		{
			subroot["Name"] = name_component->m_name;
		}

		if (render_component)
		{
			Json::Value classroot;

			AddVec3To(classroot, "ColorTint", render_component->m_ColorTint);

			classroot["UseLight"] = render_component->m_UseLight;

			if (render_component->m_RenderModel)
			{
				Json::Value temp(Json::arrayValue);
				temp.append(render_component->m_RenderModel->m_Model_key);
				temp.append(render_component->m_RenderModel->m_Directory);
				classroot["RenderModel"] = temp;
			}


			subroot["Render"] = classroot;
		}

		if (point_light_component)
		{
			//Json::Value classroot;
			//AddVec3To(classroot, "Colour", point_light_component->m_Color);
			//classroot["Intensity"] = point_light_component->m_Intensity;
			//classroot["Linear"] = point_light_component->m_Linear;
			//classroot["Quadratic"] = point_light_component->m_Quadratic;


			//subroot["Point_Light"] = classroot;

			Reflect_Serialization(subroot, point_light_component);
		}

		if (directional_light_component)
		{
			//Json::Value classroot;
			//AddVec3To(classroot, "Colour", directional_light_component->m_Color);
			//classroot["Intensity"] = directional_light_component->m_Intensity;
			//AddVec3To(classroot, "Direction", directional_light_component->m_Direction);
			//classroot["Diffuse"] = directional_light_component->m_Diffuse;
			//classroot["Specular"] = directional_light_component->m_Specular;


			//subroot["Directional_Light"] = classroot;

			Reflect_Serialization(subroot, directional_light_component);
		}


		return subroot;
	}

	void Serializer::LoadObjects(const std::string& File)
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
				LoadComponents(subroot, ImguiHelper::m_ecs->GetReg().create());

				++iter;
			}

			istream.close();
		}
	}

	void Serializer::LoadComponents(const Json::Value& root, entt::entity id)
	{

		auto& transform = root["Transform"];
		auto& name = root["Name"];
		auto& render = root["Render"];
		auto& relationship = root["Children"];
		auto& direction_light = root["Directional_Light"];
		auto& point_light = root["Point_Light"];

		if (transform)
		{
			Transform _t;
			Reflect_Deserialization(_t, transform);
			ImguiHelper::m_ecs->GetReg().emplace<Transform>(id, _t);
		}

		if (direction_light)
		{
			Directional_Light d_light;
			Reflect_Deserialization(d_light, direction_light);
			ImguiHelper::m_ecs->GetReg().emplace<Directional_Light>(id, d_light);
		}

		if (point_light)
		{
			Point_Light p_light;
			Reflect_Deserialization(p_light, point_light);
			ImguiHelper::m_ecs->GetReg().emplace<Point_Light>(id, p_light);
		}


		if (name)
		{
			ImguiHelper::m_ecs->GetReg().emplace<Name>(id, name.asString());
		}

		if (relationship)
		{
			auto& new_relation = ImguiHelper::m_ecs->GetReg().emplace<Relationship>(id);
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
				auto relationship_component = ImguiHelper::m_ecs->GetReg().try_get<Relationship>(m_parent_id.top());
				relationship_component->m_children.insert(id);
				new_relation.m_parent = m_parent_id.top();

				--m_child_counter.top();
			}
			//Case the child is also of a parent of another child
			else 
			{
				auto relationship_component = ImguiHelper::m_ecs->GetReg().try_get<Relationship>(m_parent_id.top());
				relationship_component->m_children.insert(id);
				new_relation.m_parent = m_parent_id.top();
				--m_child_counter.top();
			
				m_child_counter.push(child);
				m_parent_id.push(id);
			}
			if (!m_child_counter.empty() && m_child_counter.top() <= 0)
			{
				m_child_counter.pop();
				m_parent_id.pop();
			}
		}

		if (render)
		{
			//glm::vec3 _colorTint = ;
			Pogplant::Model* m_Model = nullptr;

			if (render["RenderModel"] != Json::nullValue)
			{
				auto result = Pogplant::ModelResource::m_ModelPool.find(render["RenderModel"][0].asString());

				if (result != Pogplant::ModelResource::m_ModelPool.end())
				{
					m_Model = result->second;
				}
				else
				{
					std::cout	<< "Unable to find " << render["RenderModel"][1].asString()
								<< " (" << render["RenderModel"][0].asString() << ")" << std::endl;
				}
			}

			ImguiHelper::m_ecs->GetReg().emplace<Renderer>(
				id,
				glm::mat4{1},
				glm::vec3{ render["ColorTint"][0].asFloat(),render["ColorTint"][1].asFloat(),render["ColorTint"][2].asFloat() },
				m_Model,
				render["UseLight"].asInt()
				);
		}
	}

	int Serializer::RecurSaveChild(Json::Value& _classroot, entt::entity id,int counter)
	{
		auto relationship_component = ImguiHelper::m_ecs->GetReg().try_get<Relationship>(id);
		if (relationship_component)
		{
			for (auto& child : relationship_component->m_children)
			{
				_classroot[counter++] = SaveComponents(child);
				auto child_relationship_component = ImguiHelper::m_ecs->GetReg().try_get<Relationship>(child);
				if (child_relationship_component)
				{
					counter = RecurSaveChild(_classroot, child, counter);
				}
			}
		}
		return counter;
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

	Json::Value Serializer::Reflect_Serialization(Json::Value& _root, rttr::instance _obj)
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

			if (prop_value.is_type<glm::vec3>())
			{
				AddVec3To(root, name, prop_value.get_value<glm::vec3>());
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

		return _root[component_name] = root;
	}

	void Serializer::Reflect_Deserialization(rttr::instance _obj, const Json::Value& _data)
	{
		rttr::instance obj = _obj.get_type().get_raw_type().is_wrapper() ? _obj.get_wrapped_instance() : _obj;
		const auto component_name = obj.get_type().get_raw_type().get_name().to_string();

		auto prop_list = obj.get_derived_type().get_properties();

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
				if (prop_value.is_type<glm::vec3>())
				{
					prop.set_value(obj, CreateVec3(_data[name]));
				}
				else if (variable_type.is_arithmetic())
				{
					if (!Load_arithmetic(variable_type, prop, obj, _data[name]))
						std::cout << name << " is an unsupported arithmetic type" << std::endl;
				}
				else
				{
					std::cout << "type not supported" << std::endl;
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
}

