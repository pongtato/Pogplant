#include "SceneHierarchy.h"
#include "Logger.h"

#include <IconsMaterialDesign.h>
#include <imgui.h>

#include "../../Pogplant/Src/Utils/FileDialogs.h"
#include "../../Pogplant/Src/ModelResource.h"
#include "../../Pogplant/Src/Window.h"
#include "ECS/Components/Components.h"
#include "ECS/Components/DependantComponents.h"
#include "Serialiser/Serializer.h"

namespace PogplantDriver
{
	bool SceneHierarchy::m_Loading = false;
	bool SceneHierarchy::m_SH_LeftClickHeld = false;
	entt::entity SceneHierarchy::m_ClickedOn = entt::null;
	entt::entity SceneHierarchy::m_ReleasedOn = entt::null;

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
#ifdef SHOW_IMGUIDEMOWINDOW
			ImGui::ShowDemoWindow();
#endif

#ifdef SHOW_PREFAB
			auto results = m_ECS->view_SHOW_PREFAB<Components::Transform>();
#else

			auto results = m_ECS->view_SHOW_PREFAB<Components::Transform>();
#endif // SHOW_PREFAB

			if (ImGui::IsMouseDown(0) && ImGui::IsWindowHovered())
			{
				m_CurrentEntity = entt::null;
			}

			std::for_each(results.rbegin(), results.rend(), [&results,this](auto entity)
				{
					DrawEntityNode(entity);
				}); 

			ImGui::Text("");
			ImGui::Text("");

			CheckReparenting();

			if (m_CurrentEntity != entt::null)
			{
				if (ImGui::BeginPopupContextWindow("EntityPopup", ImGuiPopupFlags_MouseButtonRight))
				{
					if (ImGui::MenuItem("Create Child"))
					{
						m_ECS->CreateChild(m_CurrentEntity);
					}

					if (ImGui::MenuItem("Duplicate Entity"))
						m_ECS->CopyEntity(m_CurrentEntity);


					if (ImGui::MenuItem("Save Prefab"))
					{
						SavePrefab(m_CurrentEntity);
					}

					auto prefab_instance = m_ECS->GetReg().try_get<Components::PrefabInstance>(m_CurrentEntity);
					if (prefab_instance)
					{
						if (ImGui::MenuItem("Detach"))
						{
							m_ECS->RemovePrefabInstance(m_CurrentEntity);
						}
					}

					//let this always be last
					//because it sets m_CurrentEntity to null
					if (ImGui::MenuItem("Delete Entity"))
					{
						m_ECS->DestroyEntity(m_CurrentEntity);
						m_CurrentEntity = entt::null;
					}

					ImGui::EndPopup();
				}
			}

			// Right-click on blank space
			if (ImGui::BeginPopupContextWindow("NoEntityPopup", 1, false))
			{
				if (ImGui::MenuItem("Create Empty Entity"))
					m_ECS->CreateEntity();
				//if (ImGui::MenuItem("Load Prefab"))
				//	LoadPrefab2();
				if (ImGui::MenuItem("Load Model"))
					m_Loading = true;

				////testing stuffs for new prefab load
				//if (ImGui::MenuItem("Load Prefab 2(test)"))
				//	LoadPrefab2();

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

		auto name = m_ECS->GetReg().get<Components::Name>(entity);
		std::string obj_name = name.m_name;


		auto _p = m_ECS->GetReg().try_get<Components::Prefab>(entity);
		if (_p)
#ifdef SHOW_PREFAB
			obj_name.append(" (Prefab)");
#else
			return false;
#endif
		auto _pi = m_ECS->GetReg().try_get<Components::PrefabInstance>(entity);
		if (_pi)
			obj_name.append(" (Instance)");

		ImGuiTreeNodeFlags flags = (m_CurrentEntity == entity) ? ImGuiTreeNodeFlags_Selected : 0;
		
		if(!_transform.m_children.empty())
			flags |= ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_OpenOnDoubleClick | ImGuiTreeNodeFlags_SpanAvailWidth;
		else
			flags |= ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_OpenOnDoubleClick | ImGuiTreeNodeFlags_Leaf | ImGuiTreeNodeFlags_SpanAvailWidth;

		bool is_opened = ImGui::TreeNodeEx((void*)(uint64_t)entity, flags, obj_name.c_str());

		if (ImGui::IsItemClicked() || ImGui::IsMouseClicked(1) && ImGui::IsItemHovered(ImGuiHoveredFlags_RectOnly))
			m_CurrentEntity = entity;
		else if (ImGui::IsMouseDown(0) && ImGui::IsItemHovered(ImGuiHoveredFlags_RectOnly) &&
			ImGui::IsItemFocused())
		{
			if (!m_SH_LeftClickHeld)
			{
				m_SH_LeftClickHeld = true;
				m_ClickedOn = entity;
				m_ReleasedOn = entt::null;
			}
		}
		else if (ImGui::IsMouseReleased(0) && ImGui::IsItemHovered(ImGuiHoveredFlags_RectOnly))
		{
			m_ReleasedOn = entity;
		}

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

			//creates a copy of the entity and save as a prefab type
			auto copy_of_entity = m_ECS->CopyEntity(_object);

			serialiser.SavePrefab(filepath, copy_of_entity);

			//current copy becomes a prefab instnace
			m_ECS->GetReg().emplace<Components::PrefabInstance>(_object, 
																m_ECS->GetReg().get<Components::Guid>(copy_of_entity).m_guid, 
																m_ECS->GetReg().get<Components::Prefab>(copy_of_entity).file_path);
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
		ImGui::SetNextWindowSizeConstraints({ 1,1 }, {1000,900});
		ImGui::SetNextWindowPos(center, ImGuiCond_Appearing, ImVec2(0.5f, 0.5f));

		if (ImGui::BeginPopupModal(label, NULL, ImGuiWindowFlags_AlwaysAutoResize))
		{
			size_t it_count = 0;
			for (auto& it : Pogplant::ModelResource::m_ModelPool)
			{
				// Center object
				//ImGui::SetCursorPosX(ImGui::GetWindowContentRegionMax().x * 0.5f - 146.0f);
				if(ImGui::Button(it.first.c_str(), ImVec2(300,69)))
				{
					auto entity = m_ECS->CreateEntity(it.first);
					ConstructModel(entity, it.second, &it.second->m_Meshes.begin()->second, glm::vec4 { 1 });
					// Done loading
					m_Loading = false;
					ImGui::CloseCurrentPopup();
				}

				// Formatting
				if ((it_count + 1) % 3 != 0)
				{
					ImGui::SameLine();
				}

				it_count++;
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


	void SceneHierarchy::CheckReparenting()
	{
		//process reparenting
		if (m_ClickedOn != entt::null && m_SH_LeftClickHeld)
		{
			if (!ImGui::IsMouseDragging(0, 0.f))
			{
				//std::cout << "Clicked on " << (uint32_t)m_ClickedOn << std::endl;
				//std::cout << "Released on " << (uint32_t)m_ReleasedOn << std::endl;

				//case 1
				//clicked on entity and release on nothing
				if (m_ClickedOn != entt::null && m_ReleasedOn == entt::null)
				{
					m_ECS->RemoveParentFrom(m_ClickedOn);
				}
				else if (m_ClickedOn != entt::null && m_ReleasedOn != entt::null && m_ClickedOn != m_ReleasedOn)
				{
					//ignore all immediate hierarchy(1 level)
					//P
					//- C
					//  - D
					//should ignore swapping for pair PC and CD.
					if (!m_ECS->IsChildOf(m_ClickedOn, m_ReleasedOn) && !m_ECS->IsChildOf(m_ReleasedOn, m_ClickedOn))
					{
						//std::cout << "valid swap" << std::endl;
						m_ECS->SetParent(m_ReleasedOn, m_ClickedOn);
						m_ECS->SetChild(m_ReleasedOn, m_ClickedOn);
					}

				}

				m_SH_LeftClickHeld = false;

			}
		}
	}
}
