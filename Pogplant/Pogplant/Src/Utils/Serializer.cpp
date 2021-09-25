#include "Serializer.h"

#include "../../../Pogplant Driver/Src/ImguiHelper.h"
#include "../../../Pogplant Driver/Src/ECS/Components/Components.h"
#include <fstream>

using namespace PogplantDriver;
using namespace Components;
namespace Pogplant
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

	void Serializer::Load(const std::string& File)
	{
	}

	void Serializer::SaveObjects(const std::string& File)
	{
		std::ofstream ostream(File, std::ios::out);
		
		if (ostream.is_open())
		{
			Json::Value root;
			int  i = 0;
			
			auto entities = ImguiHelper::m_ecs->GetReg().view<Transform>();
			for (auto entity : entities)
			{
				Json::Value subroot = SaveComponents(entity);
				root[i] = subroot;
				++i;
			}

			//Append .json for the lazy peole


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
			Json::Value data(Json::arrayValue);

			// Save position
			glm::vec3 vec = transform_component->m_position;

			data.append(vec.x);
			data.append(vec.y);
			data.append(vec.z);

			classroot["Position"] = data;
			data.clear();

			// Save rotation
			vec = transform_component->m_rotation;

			data.append(vec.x);
			data.append(vec.y);
			data.append(vec.z);

			classroot["Rotation"] = data;
			data.clear();

			// Save scale
			vec = transform_component->m_scale;

			data.append(vec.x);
			data.append(vec.y);
			data.append(vec.z);

			classroot["Scale"] = data;
			data.clear();

			subroot["Transform"] = classroot;

		}


		return subroot;
	}
}

