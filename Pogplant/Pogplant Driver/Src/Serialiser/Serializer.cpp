#include "Serializer.h"


#include "../ImguiHelper.h"
#include "../ECS/Components/Components.h"
#include "ModelResource.h"

#include <fstream>

using namespace Components;
namespace PogplantDriver
{
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

		if (ostream.is_open())
		{
			Json::Value root;
			Json::Value subroot = SaveComponents(id);
			root[0] = subroot;

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
		auto render_component = ImguiHelper::m_ecs->GetReg().try_get<Render>(id);
		auto point_light_component = ImguiHelper::m_ecs->GetReg().try_get<Point_Light>(id);
		auto directional_light_component = ImguiHelper::m_ecs->GetReg().try_get<Directional_Light>(id);

		if (transform_component)
		{
			//this works
			Reflect_This(subroot, transform_component);

			//Json::Value classroot;

			//AddVec3To(classroot, "Position", transform_component->m_position);
			//AddVec3To(classroot, "Rotation", transform_component->m_rotation);
			//AddVec3To(classroot, "Scale", transform_component->m_scale);

			//subroot["Transform"] = classroot;

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
			Json::Value classroot;
			AddVec3To(classroot, "Colour", point_light_component->m_Color);
			classroot["Intensity"] = point_light_component->m_Intensity;
			classroot["Linear"] = point_light_component->m_Linear;
			classroot["Quadratic"] = point_light_component->m_Quadratic;


			subroot["Point_Light"] = classroot;
		}

		if (directional_light_component)
		{
			Json::Value classroot;
			AddVec3To(classroot, "Colour", directional_light_component->m_Color);
			classroot["Intensity"] = directional_light_component->m_Intensity;
			AddVec3To(classroot, "Direction", directional_light_component->m_Direction);
			classroot["Diffuse"] = directional_light_component->m_Diffuse;
			classroot["Specular"] = directional_light_component->m_Specular;


			subroot["Directional_Light"] = classroot;
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
		auto& light = root["Light"];
		if (transform)
		{
			glm::vec3 pos = { transform["m_position"][0].asFloat(),transform["m_position"][1].asFloat(),transform["m_position"][2].asFloat() };
			glm::vec3 rot = { transform["m_rotation"][0].asFloat(),transform["m_rotation"][1].asFloat(),transform["m_rotation"][2].asFloat() };
			glm::vec3 sca = { transform["m_scale"][0].asFloat(),transform["m_scale"][1].asFloat(),transform["m_scale"][2].asFloat() };

			ImguiHelper::m_ecs->GetReg().emplace<Transform>(id, pos, rot, sca);
		}

		if (name)
		{
			ImguiHelper::m_ecs->GetReg().emplace<Name>(id, name.asString());
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

			ImguiHelper::m_ecs->GetReg().emplace<Render>(
				id,
				glm::mat4{1},
				glm::vec3{ render["ColorTint"][0].asFloat(),render["ColorTint"][1].asFloat(),render["ColorTint"][2].asFloat() },
				m_Model,
				render["UseLight"].asInt()
				);
		}
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

	Json::Value Serializer::Reflect_This(Json::Value& _root, rttr::instance _obj)
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

			if (prop_value.is_type<glm::vec3>())
			{
				AddVec3To(root, name, prop_value.get_value<glm::vec3>());
			}
			else
			{
				std::cout << "type not supported" << std::endl;
			}

		}

		return _root[component_name] = root;
	}
}

