#include "Serializer.h"


#include "../ImguiHelper.h"
#include "../ECS/Components/Components.h"
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
		auto mesh_component = ImguiHelper::m_ecs->GetReg().try_get<RenderObject>(id);

		if (transform_component)
		{
			Json::Value classroot;

			AddVec3To(classroot, "Position", transform_component->m_position);
			AddVec3To(classroot, "Rotation", transform_component->m_rotation);
			AddVec3To(classroot, "Scale", transform_component->m_scale);

			subroot["Transform"] = classroot;

		}

		if (name_component)
		{
			subroot["Name"] = name_component->m_name;
		}

		if (mesh_component)
		{
			Json::Value classroot;

			AddVec3To(classroot, "ColorTint", mesh_component->m_ColorTint);

			classroot["UseLight"] = mesh_component->m_UseLight;

			if (mesh_component->m_RenderModel)
			{
				Json::Value temp(Json::arrayValue);
				temp.append(mesh_component->m_RenderModel->m_Model_key);
				temp.append(mesh_component->m_RenderModel->m_Directory);
				classroot["RenderModel"] = temp;
			}


			subroot["Mesh"] = classroot;
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

		if (transform)
		{
			glm::vec3 pos = { transform["Position"][0].asFloat(),transform["Position"][1].asFloat(),transform["Position"][2].asFloat() };
			glm::vec3 rot = { transform["Rotation"][0].asFloat(),transform["Rotation"][1].asFloat(),transform["Rotation"][2].asFloat() };
			glm::vec3 sca = { transform["Scale"][0].asFloat(),transform["Scale"][1].asFloat(),transform["Scale"][2].asFloat() };
			ImguiHelper::m_ecs->GetReg().emplace<Transform>(id,pos,rot,sca);
		}
		if (name)
		{
			ImguiHelper::m_ecs->GetReg().emplace<Name>(id, name.asString());
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
}

