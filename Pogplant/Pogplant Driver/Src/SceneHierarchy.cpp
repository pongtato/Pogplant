#include "SceneHierarchy.h"

#include <IconsMaterialDesign.h>
#include <imgui.h>

#include "../../Pogplant/Src/Utils/FileDialogs.h"
#include "ECS/Components/Components.h"
#include "ECS/Components/DependantComponents.h"
#include "Serialiser/Serializer.h"

namespace PogplantDriver
{
	void SceneHierarchy::Init(ECS* ecs, entt::entity& current_entity)
	{
		m_ECS = ecs;
		m_CurrentEntity = current_entity;
	}
	void SceneHierarchy::Render(entt::entity& current_entity)
	{
		m_CurrentEntity = current_entity;
		ImGui::Begin("Scene Hierarchy");
		{
			auto results = m_ECS->GetReg().view<Components::Transform>();

			if (ImGui::IsMouseDown(0) && ImGui::IsWindowHovered())
				m_CurrentEntity = entt::null;

			std::for_each(results.rbegin(), results.rend(), [&results,this](auto entity)
				{
					DrawEntityNode(entity);
				});


			if (m_CurrentEntity != entt::null)
			{
				if (ImGui::BeginPopupContextWindow("EntityPopup", ImGuiPopupFlags_MouseButtonRight))
				{
					if (ImGui::MenuItem("Create Child"))
					{
						m_ECS->CreateChild(m_CurrentEntity);
					}

					if (ImGui::MenuItem("Delete Entity"))
					{
						m_ECS->DestroyEntity(m_CurrentEntity);
						m_CurrentEntity = entt::null;

					}
					if (ImGui::MenuItem("Save Prefab"))
					{
						SavePrefab(m_CurrentEntity);
					}


					ImGui::EndPopup();
				}
			}

			// Right-click on blank space
			if (ImGui::BeginPopupContextWindow("NoEntityPopup", 1, false))
			{
				if (ImGui::MenuItem("Create Empty Entity"))
					m_ECS->CreateEntity();
				if (ImGui::MenuItem("Load Prefab"))
					LoadPrefab();

				ImGui::EndPopup();
			}
		}
		current_entity = m_CurrentEntity;
		ImGui::End();
	}


	bool SceneHierarchy::DrawEntityNode(entt::entity entity, bool draw_childen)
	{

		auto _r = m_ECS->GetReg().try_get<Components::Relationship>(entity);

		if (!draw_childen && _r && _r->m_parent != entt::null)
			return false;

		auto name = m_ECS->GetReg().get<Components::Name>(entity);
		std::string obj_name = name.m_name;
		ImGuiTreeNodeFlags flags = (m_CurrentEntity == entity) ? ImGuiTreeNodeFlags_Selected : 0;
		if (_r && _r->m_children.size() != 0)
			flags |= ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_OpenOnDoubleClick | ImGuiTreeNodeFlags_DefaultOpen | ImGuiTreeNodeFlags_SpanAvailWidth;
		else
			flags |= ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_OpenOnDoubleClick | ImGuiTreeNodeFlags_Leaf | ImGuiTreeNodeFlags_SpanAvailWidth;

		bool is_opened = ImGui::TreeNodeEx((void*)(uint64_t)entity, flags, obj_name.c_str());

		if (ImGui::IsItemClicked() || ImGui::IsMouseClicked(1) && ImGui::IsItemHovered(ImGuiHoveredFlags_RectOnly))
			m_CurrentEntity = entity;

		bool is_deleted = false;

		if (is_opened)
		{
			std::string c_name = "<no children>";

			if (_r)
			{
				std::set<entt::entity> s = _r->m_children;
				//int i = 0;
				for (const auto& ent : s)
				{
					DrawEntityNode(ent, true);
				}
			}
			else
			{

			}
			ImGui::TreePop();

		}

		return is_deleted;
	}



	void SceneHierarchy::SavePrefab(entt::entity _object)
	{
		std::string filepath = Pogplant::FileDialogs::SaveFile("Json Files(*.json)\0*.json\0");
		//Append .json 
		if (filepath.find(".json") == std::string::npos)
		{
			filepath.append(".json");
		}
		if (!filepath.empty())
		{
			Serializer serialiser{ *m_ECS };
			serialiser.SavePrefab(filepath, _object);
		}
	}

	void SceneHierarchy::LoadPrefab()
	{
		std::string filepath = Pogplant::FileDialogs::OpenFile("Json Files(*.json)\0*.json\0");
		if (!filepath.empty())
		{
			Serializer serialiser{ *m_ECS };
			serialiser.LoadPrefab(filepath);
		}
	}
}