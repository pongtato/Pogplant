#include "SceneHierarchy.h"
#include "Logger.h"

#include <IconsMaterialDesign.h>
#include <imgui.h>

#include "../../Pogplant/Src/Utils/FileDialogs.h"
#include "../../Pogplant/Src/ModelResource.h"
#include "ECS/Components/Components.h"
#include "ECS/Components/DependantComponents.h"
#include "Serialiser/Serializer.h"

namespace PogplantDriver
{
	bool SceneHierarchy::m_Loading = false;

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

#ifdef SHOW_PREFAB
			auto results = m_ECS->view_SHOW_PREFAB<Components::Transform>();
#else

			auto results = m_ECS->view<Components::Transform>();
#endif // SHOW_PREFAB

			if (ImGui::IsMouseDown(0) && ImGui::IsWindowHovered())
				m_CurrentEntity = entt::null;

			std::for_each(results.rbegin(), results.rend(), [&results,this](auto entity)
				{
					DrawEntityNode(entity);
				}); 

			ImGui::Text("");
			ImGui::Text("");

			if (m_CurrentEntity != entt::null)
			{
				if (ImGui::BeginPopupContextWindow("EntityPopup", ImGuiPopupFlags_MouseButtonRight))
				{
					if (ImGui::MenuItem("Create Child"))
					{
						m_ECS->CreateChild(m_CurrentEntity);
					}

					if (ImGui::MenuItem("Create Child Prefab"))
					{
						Entity child = m_ECS->CreateChild(m_CurrentEntity);
						child.GetID();
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

					if (ImGui::MenuItem("Duplicate Entity"))
						m_ECS->CopyEntity(m_CurrentEntity);

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
				if (ImGui::MenuItem("Load Model"))
					m_Loading = true;

				//testing stuffs for new prefab load
				if (ImGui::MenuItem("Load Prefab 2(test)"))
					LoadPrefab2();


				ImGui::EndPopup();
			}

			if (ImGui::GetScrollY() >= ImGui::GetScrollMaxY())
				ImGui::SetScrollHereY(1.0f);

			// This has to be done this way due to how imgui handles IDs, it will have been popped in the context window above
			LoadModel();
		}
		current_entity = m_CurrentEntity;
		ImGui::End();
	}


	bool SceneHierarchy::DrawEntityNode(entt::entity entity, bool draw_childen)
	{
		auto& _transform = m_ECS->GetReg().get<Components::Transform>(entity);

		if (!draw_childen && _transform.m_parent != entt::null)
			return false;

		/*auto _r = m_ECS->GetReg().try_get<Components::Relationship>(entity);

		if (!draw_childen && _r && _r->m_parent != entt::null)
			return false;//*/


		auto name = m_ECS->GetReg().get<Components::Name>(entity);
		std::string obj_name = name.m_name;

		auto _p = m_ECS->GetReg().try_get<Components::Prefab>(entity);
		if (_p)
			obj_name.append(" (Prefab)");

		ImGuiTreeNodeFlags flags = (m_CurrentEntity == entity) ? ImGuiTreeNodeFlags_Selected : 0;
		
		if(!_transform.m_children.empty())
			flags |= ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_OpenOnDoubleClick | ImGuiTreeNodeFlags_DefaultOpen | ImGuiTreeNodeFlags_SpanAvailWidth;
		else
			flags |= ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_OpenOnDoubleClick | ImGuiTreeNodeFlags_Leaf | ImGuiTreeNodeFlags_SpanAvailWidth;

		/*if (_r && _r->m_children.size() != 0)
			flags |= ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_OpenOnDoubleClick | ImGuiTreeNodeFlags_DefaultOpen | ImGuiTreeNodeFlags_SpanAvailWidth;
		else
			flags |= ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_OpenOnDoubleClick | ImGuiTreeNodeFlags_Leaf | ImGuiTreeNodeFlags_SpanAvailWidth;
		*/

		bool is_opened = ImGui::TreeNodeEx((void*)(uint64_t)entity, flags, obj_name.c_str());

		if (ImGui::IsItemClicked() || ImGui::IsMouseClicked(1) && ImGui::IsItemHovered(ImGuiHoveredFlags_RectOnly))
			m_CurrentEntity = entity;

		bool is_deleted = false;

		if (is_opened)
		{
			if (!_transform.m_children.empty())
			{
				for (const auto& ent : _transform.m_children)
				{
					DrawEntityNode(ent, true);
				}
			}

			ImGui::TreePop();

		}

		return is_deleted;
	}



	void SceneHierarchy::SavePrefab(entt::entity _object)
	{
		std::string filepath = Pogplant::FileDialogs::SaveFile("Prefab Files(*.prefab)\0*.prefab\0");
		//Append .prefab
		if (filepath.find(".prefab") == std::string::npos)
		{
			filepath.append(".prefab");
		}
		if (!filepath.empty())
		{
			Serializer serialiser{ *m_ECS };
			serialiser.SavePrefab(filepath, _object);
		}
	}

	void SceneHierarchy::LoadPrefab()
	{
		std::string filepath = Pogplant::FileDialogs::OpenFile("Prefab Files(*.prefab)\0*.prefab\0");
		if (!filepath.empty())
		{
			if (filepath.find(".prefab") == std::string::npos)
			{
				std::cout << "Failed to find.prefab file" << std::endl;
				Pogplant::Logger::Log({ "SceneHiearchy::LoadPreFab",Pogplant::LogEntry::LOGTYPE::ERROR, "Failed to find .prefab file"});
				return;
			}
			Serializer serialiser{ *m_ECS };
			serialiser.LoadPrefab(filepath);
		}
	}

	void SceneHierarchy::LoadPrefabChild()
	{

	}

	void SceneHierarchy::LoadPrefab2()
	{
		std::string filepath = Pogplant::FileDialogs::OpenFile("Prefab Files(*.prefab)\0*.prefab\0");
		if (!filepath.empty())
		{
			if (filepath.find(".prefab") == std::string::npos)
			{
				std::cout << "Failed to find.prefab file" << std::endl;
				Pogplant::Logger::Log({ "SceneHiearchy::LoadPreFab",Pogplant::LogEntry::LOGTYPE::ERROR, "Failed to find .prefab file" });
				return;
			}
			Serializer serialiser{ *m_ECS };
			serialiser.LoadPrefab(filepath, true);
		}
	}

	/****************************TEMPORARY STUFF TO MOVE***************************/
	// Prolly have to be moved again when prefab
	// This has to be moved to a model loading or smth
	void SceneHierarchy::ConstructModel
	(
		Entity& _Entity,
		Pogplant::Model* _Model,
		Pogplant::Mesh3D* _Mesh3D,
		const glm::vec3& _Color,
		const glm::vec3& _Emissive,
		bool _UseLight,
		bool _EditorOnly,
		bool _FirstIt
	)
	{
		if (!_FirstIt)
		{
			auto child = m_ECS->CreateChild(_Entity.GetID(), _Mesh3D->m_Name);
			child.AddComponent<Components::Renderer>(Components::Renderer{ _Color, _Emissive, _Model, _Mesh3D, _UseLight, _EditorOnly });
			auto& transform = child.GetComponent<Components::Transform>();
			transform.m_position = _Mesh3D->m_Translate;
			transform.m_rotation = _Mesh3D->m_Rotate * 90.0f;
			transform.m_scale = _Mesh3D->m_Scale;
			for (auto it : _Mesh3D->m_SubMeshIDs)
			{
				ConstructModel(child, _Model, &_Model->m_Meshes[it], _Color, _Emissive, _UseLight, _EditorOnly, false);
			}
		}
		else
		{
			_Entity.AddComponent<Components::Renderer>(Components::Renderer{ _Color, _Emissive, _Model, _Mesh3D, _UseLight, _EditorOnly });
			for (auto it : _Mesh3D->m_SubMeshIDs)
			{
				ConstructModel(_Entity, _Model, &_Model->m_Meshes[it], _Color, _Emissive, _UseLight, _EditorOnly, false);
			}
		}
	}

	/****************************END OF TEMPORARY STUFF TO MOVE***************************/

	void SceneHierarchy::LoadModel()
	{
		const char* label = "Load Model";
		if (m_Loading)
		{
			ImGui::OpenPopup(label);
		}

		// Always center this window when appearing
		ImVec2 center = ImGui::GetMainViewport()->GetCenter();
		ImGui::SetNextWindowPos(center, ImGuiCond_Appearing, ImVec2(0.5f, 0.5f));

		if (ImGui::BeginPopupModal(label, NULL, ImGuiWindowFlags_AlwaysAutoResize))
		{
			for (auto& it : Pogplant::ModelResource::m_ModelPool)
			{
				// Center object
				//ImGui::SetCursorPosX(ImGui::GetWindowContentRegionMax().x * 0.5f - 146.0f);
				if(ImGui::Button(it.first.c_str(), ImVec2(300,00)))
				{
					auto entity = m_ECS->CreateEntity(it.first);
					ConstructModel(entity, it.second, &it.second->m_Meshes.begin()->second, glm::vec4 { 1 });
					// Done loading
					m_Loading = false;
					ImGui::CloseCurrentPopup();
				}
			}

			ImGui::SetItemDefaultFocus();
			// Center object
			ImGui::SetCursorPosX(ImGui::GetWindowContentRegionMax().x * 0.5f - 56.0f);
			if (ImGui::Button("Cancel", ImVec2(120, 0))) 
			{
				m_Loading = false;
				ImGui::CloseCurrentPopup(); 
			}
			ImGui::EndPopup();
		}
	}
}