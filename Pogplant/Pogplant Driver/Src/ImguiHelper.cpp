﻿#include "ImguiHelper.h"
#include "Pogplant.h"

#include <imgui.h>
#include <imgui_internal.h>
#include <imgui_impl_glfw.h>
#include <imgui_impl_opengl3.h>
#include <ImGuiExtraStyles.h>
#include <ImGuizmo.h>
#include <imgui_curve.hpp>
#include <gtc/type_ptr.hpp>
#include <IconsFontAwesome5.h>
#include <IconsKenney.h>

#include "ECS/Entity.h"
#include "../../Pogplant/Src/Utils/FileDialogs.h"
#include "ECS/Components/Components.h"
#include "ECS/Components/DependantComponents.h"
#include "Serialiser/Serializer.h"
#include "SceneHierarchy.h"
#include "Debugger.h"
#include "Panels/InputPanel.h"
#include "Panels/CollisionLayerPanel.h"
#include "ECS/Systems/ScriptResource.h"
#include <algorithm>
#include <execution>

#include "Application.h"
#include <IconsMaterialDesign.h>

namespace PogplantDriver
{
	//Adds blank text as seperator text to make things look nicer
	void ImguiBlankSeperator(int value)
	{
		for (int i = 0; i < value; ++i)
		{
			ImGui::Text("");
		}
	}

	void CurrentPopupComponents()
	{

		bool adding_enabled = false;

		if (PPD::ImguiHelper::m_CurrentEntity != entt::null)
		{
			adding_enabled = true;
		}
		if (ImGui::MenuItem("Tag", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::Tag>(PPD::ImguiHelper::m_CurrentEntity);
		}

		if (ImGui::BeginMenu("3D Render"))
		{
			glm::vec3 color = { 0.835f,0.921f,0.905f };
			glm::vec3 emissiveTint = glm::vec3{ 1.0f };
			for (const auto& model : PP::ModelResource::m_ModelPool)
			{
				if (ImGui::MenuItem(model.first.c_str(), NULL, false, adding_enabled))
				{
					(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::Renderer>(PPD::ImguiHelper::m_CurrentEntity,
						color,
						emissiveTint,
						model.second,
						&model.second->m_Meshes.begin()->second);
				}
			}
			ImGui::EndMenu();
		}
		if (ImGui::MenuItem("Point_Light", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::Point_Light>(PPD::ImguiHelper::m_CurrentEntity);
		}
		if (ImGui::MenuItem("Directional_Light", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::Directional_Light>(PPD::ImguiHelper::m_CurrentEntity);
		}
		if (ImGui::MenuItem("Font", NULL, false, adding_enabled)) //There is another font somewhere might crash (But should not as id stack is cleared)
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::Text>(PPD::ImguiHelper::m_CurrentEntity, glm::vec3{ 0.835f,0.921f,0.905f }, "Ruda", "Lorem ipsum dolor sit amet");
		}
		if (ImGui::MenuItem("RigidBody", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::Rigidbody>(PPD::ImguiHelper::m_CurrentEntity);
		}
		if (ImGui::MenuItem("Box Collider", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::BoxCollider>(PPD::ImguiHelper::m_CurrentEntity);
		}
		if (ImGui::MenuItem("Sphere Collider", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::SphereCollider>(PPD::ImguiHelper::m_CurrentEntity);
		}
		if (ImGui::MenuItem("OBB Box Collider", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::OBBBoxCollider>(PPD::ImguiHelper::m_CurrentEntity);
		}
		/*if (ImGui::MenuItem("Mesh Collider", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::MeshCollider>(PPD::ImguiHelper::m_CurrentEntity);
		}//*/
		if (ImGui::MenuItem(ICON_FA_CAMERA "  Camera", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::Camera>(PPD::ImguiHelper::m_CurrentEntity);
		}
		if (ImGui::MenuItem("Laserrrr", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::Laser>(PPD::ImguiHelper::m_CurrentEntity);
		}
		// Scriptable Component
		if (ImGui::MenuItem("Scriptable", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::Scriptable>(PPD::ImguiHelper::m_CurrentEntity);
			auto name_com = PPD::ImguiHelper::m_ecs->GetReg().try_get<Components::Name>(PPD::ImguiHelper::m_CurrentEntity);
			if (name_com)
			{
				std::cout << "Entity [" << name_com->m_name << "] has added script component" << std::endl;
			}
			
		}
		if (ImGui::MenuItem("PauseScriptable", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::PauseScriptable>(PPD::ImguiHelper::m_CurrentEntity);
			auto name_com = PPD::ImguiHelper::m_ecs->GetReg().try_get<Components::Name>(PPD::ImguiHelper::m_CurrentEntity);
			if (name_com)
			{
				std::cout << "Entity [" << name_com->m_name << "] has added pause script component" << std::endl;
			}

		}

		if (ImGui::MenuItem("Script Variable Container", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::ScriptVariables>(PPD::ImguiHelper::m_CurrentEntity);
		}

		if (ImGui::MenuItem("Audio Source", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::AudioSource>(PPD::ImguiHelper::m_CurrentEntity);
		}
		if (ImGui::MenuItem("Canvas", NULL, false, adding_enabled))
		{
			glm::vec4 color { 1.f,1.f,1.f,1.f };
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::Canvas>(PPD::ImguiHelper::m_CurrentEntity, color, "NO_TEX.dds", true, false);
		}

		if (ImGui::MenuItem("Particle System", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::ParticleSystem>(PPD::ImguiHelper::m_CurrentEntity);
		}

		if (ImGui::MenuItem("MovementBounds", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::MovementBounds>(PPD::ImguiHelper::m_CurrentEntity);
		}

		if (ImGui::MenuItem("Transform Debugger", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::TransformDebugger>(PPD::ImguiHelper::m_CurrentEntity);
		}
	}

	bool ImguiHelper::m_FirstRun = true;
	//int ImguiHelper::m_CurrentGOIdx = -1;
	entt::entity ImguiHelper::m_CurrentEntity = entt::null;
	ECS* ImguiHelper::m_ecs = nullptr;
	Directory ImguiHelper::m_Directory;
	std::vector<std::unique_ptr<Panels>> ImguiHelper::m_Panels;

	// Guizmo editor stuff
	static ImGuizmo::OPERATION m_EditMode(ImGuizmo::TRANSLATE);
	static bool m_UseSnap = false;
	static float m_SnapStep[]		= { 0.1f, 0.1f, 0.1f };
	static float m_BoundsPos[]		= { -0.5f, -0.5f, -0.5f, 0.5f, 0.5f, 0.5f };
	static float m_BoundsSnapStep[] = { 0.1f, 0.1f, 0.1f };
	static bool m_BoundSizing = false;
	static bool m_UseBoundsSnap = false;
	static bool m_GoEditing = false;

	bool ImguiHelper::InitImgui(ECS* ecs)
	{
		IMGUI_CHECKVERSION();
		ImGui::CreateContext();
		ImGuiIO& io = ImGui::GetIO(); 
		io.Fonts->AddFontFromFileTTF("Resources/Fonts/Ruda.ttf", 14);
		static const ImWchar icons_ranges[] = { ICON_MIN_FA, ICON_MAX_FA, 0 };
		ImFontConfig icons_config; icons_config.MergeMode = true; icons_config.PixelSnapH = true;
		io.Fonts->AddFontFromFileTTF("Resources/Fonts/fa-solid-900.ttf", 14.0f, &icons_config, icons_ranges);

		static const ImWchar wicons_ranges[] = { ICON_MIN_FA, ICON_MAX_FA, 0 };
		ImFontConfig wicons_config; wicons_config.MergeMode = true; wicons_config.PixelSnapH = true;
		io.Fonts->AddFontFromFileTTF("Resources/Fonts/MaterialIcons-Regular.ttf", 58.0f, &wicons_config, wicons_ranges);


		io.ConfigFlags |= ImGuiConfigFlags_DockingEnable; // Enable Docking
		imgui_extra_styles::Pogplant();
		ImGui_ImplGlfw_InitForOpenGL(PP::Window::GetWindow(), true);
		ImGui_ImplOpenGL3_Init();

		m_ecs = ecs;
		
		m_Panels.push_back(std::make_unique<PPP::InputPanel>());
		m_Panels.push_back(std::make_unique<PPP::CollisionLayerPanel>());
		m_Panels.push_back(std::make_unique<Debugger>());
		m_Panels.push_back(std::make_unique<SceneHierarchy>());
		for (auto& Panel : m_Panels)
		{
			Panel.get()->Init(m_ecs, m_CurrentEntity);
		}
		return true;
	}

	void ImguiHelper::RelinkECS(ECS* ecs)
	{
		m_ecs = ecs;
		for (auto& Panel : m_Panels)
		{
			Panel.get()->Init(m_ecs, m_CurrentEntity);
		}
	}

	void ImguiHelper::DrawImgui()
	{
		/// IMGUI start
		ImGui_ImplOpenGL3_NewFrame();
		ImGui_ImplGlfw_NewFrame();
		ImGui::NewFrame();
		ImGuizmo::SetOrthographic(false);
		ImGuizmo::BeginFrame();

		// Window
		ImVec2 winSize = ImVec2(static_cast<float>(PP::Window::m_Width), static_cast<float>(PP::Window::m_Height));
		ImGui::SetNextWindowSize(winSize);
		ImGui::SetNextWindowPos(ImVec2(0, 0));

		// Draw the entire UI
		DrawDock();

		//ImGui::ShowDemoWindow();

		// Render dear imgui into screen
		ImGui::Render();
		ImGui_ImplOpenGL3_RenderDrawData(ImGui::GetDrawData());
	}

	void ImguiHelper::DrawDock()
	{
		std::unordered_map<const char*, ImGuiID> m_DockIDs;

		ImGuiWindowFlags window_flags = ImGuiWindowFlags_NoDocking;
		window_flags |= ImGuiWindowFlags_NoTitleBar | ImGuiWindowFlags_NoCollapse;
		window_flags |= ImGuiWindowFlags_NoResize | ImGuiWindowFlags_NoMove;
		window_flags |= ImGuiWindowFlags_NoBringToFrontOnFocus | ImGuiWindowFlags_NoNavFocus;
		//window_flags |= ImGuiWindowFlags_NoBackground;

		// Set DockSpace to fill viewport
		ImGuiViewport* viewport = ImGui::GetMainViewport();
		ImGui::SetNextWindowPos(viewport->GetWorkPos());
		ImGui::SetNextWindowSize(viewport->GetWorkSize());
		ImGui::SetNextWindowViewport(viewport->ID);

		// Dock window
		ImGui::PushStyleVar(ImGuiStyleVar_WindowPadding, ImVec2(0, 0));
		ImGui::PushStyleVar(ImGuiStyleVar_WindowRounding, 0);
		ImGui::Begin("Dockspace", 0, window_flags);
		{
			m_DockIDs["DOCKSPACE"] = ImGui::GetID("dockSpace");
			if (!ImGui::DockBuilderGetNode(m_DockIDs["DOCKSPACE"]))
			{
				// For the Docking Style
				const auto stylingFlags = static_cast<unsigned>(ImGuiDockNodeFlags_NoCloseButton | ImGuiDockNodeFlags_NoWindowMenuButton);

				ImGui::DockBuilderRemoveNode(m_DockIDs["DOCKSPACE"]);
				ImGui::DockBuilderAddNode(m_DockIDs["DOCKSPACE"], stylingFlags);

				ImGuiID sceneID = m_DockIDs["DOCKSPACE"];

				// Create the 4 sides
				// Inspector/Debugger
				m_DockIDs["RIGHT_DOCK"] = ImGui::DockBuilderSplitNode(sceneID, ImGuiDir_Right, 0.25f, nullptr, &sceneID);
				// Split bottom dock for file explorer & prefabs/sound
				m_DockIDs["BOT_DOCK"] = ImGui::DockBuilderSplitNode(sceneID, ImGuiDir_Down, 0.3f, nullptr, &sceneID);
				m_DockIDs["SUB_BOT_DOCK"] = ImGui::DockBuilderSplitNode(m_DockIDs["BOT_DOCK"], ImGuiDir_Left, 0.2f, &m_DockIDs["DIRECTORY"], &m_DockIDs["EXPLORER"]);
				// Entirely for Scene Hierarchy
				m_DockIDs["LEFT_DOCK"] = ImGui::DockBuilderSplitNode(sceneID, ImGuiDir_Left, 0.2f, nullptr, &sceneID);
				// Used entirey for game render
				m_DockIDs["TOP_DOCK"] = ImGui::DockBuilderSplitNode(sceneID, ImGuiDir_Up, 0.6f, nullptr, &sceneID);

				// The windows below need to match the window names here
				ImGui::DockBuilderDockWindow("Directory", m_DockIDs["DIRECTORY"]);
				ImGui::DockBuilderDockWindow("Prefab_Loaded", m_DockIDs["EXPLORER"]);
				ImGui::DockBuilderDockWindow("Debug", m_DockIDs["EXPLORER"]);
				ImGui::DockBuilderDockWindow("Scene Hierarchy", m_DockIDs["LEFT_DOCK"]);
				ImGui::DockBuilderDockWindow("Input Editor", m_DockIDs["RIGHT_DOCK"]);
				ImGui::DockBuilderDockWindow("Collision Layer Editor", m_DockIDs["RIGHT_DOCK"]);
				ImGui::DockBuilderDockWindow("Inspector", m_DockIDs["RIGHT_DOCK"]);
				ImGui::DockBuilderDockWindow("Scene", m_DockIDs["TOP_DOCK"]);
				ImGui::DockBuilderDockWindow("Game", m_DockIDs["TOP_DOCK"]);
				ImGui::DockBuilderDockWindow("Menu", m_DockIDs["TOP_DOCK"]);

				// Apple Style to all Nodes
				ImGuiDockNode* node = nullptr;
				node = ImGui::DockBuilderGetNode(m_DockIDs["LEFT_DOCK"]);
				node->LocalFlags |= stylingFlags;
				node = ImGui::DockBuilderGetNode(m_DockIDs["RIGHT_DOCK"]);
				node->LocalFlags |= stylingFlags;
				node = ImGui::DockBuilderGetNode(m_DockIDs["TOP_DOCK"]);
				node->LocalFlags |= stylingFlags;
				node = ImGui::DockBuilderGetNode(m_DockIDs["DIRECTORY"]);
				node->LocalFlags |= stylingFlags;
				node = ImGui::DockBuilderGetNode(m_DockIDs["EXPLORER"]);
				node->LocalFlags |= stylingFlags;

				ImGui::DockBuilderFinish(m_DockIDs["DOCKSPACE"]);
			}
		}

		ImGui::DockSpace(m_DockIDs["DOCKSPACE"], ImVec2(0.0f, 0.0f), ImGuiDockNodeFlags_None); //called every frame
		ImGui::End();
		ImGui::PopStyleVar(2);

		// Menu Bar
		bool exiting = false;
		if (ImGui::BeginMainMenuBar())
		{
			if (ImGui::BeginMenu("File"))
			{
				Application::GetInstance().TransitionApplicationState(Application::APPLICATIONSTATE::EDITOR);
				Application::GetInstance().SetPlayState(Application::PLAYSTATE::PLAY);
				// Save scene
				if (ImGui::MenuItem(ICON_FA_FILE "  New"))
					NewScene();
				if (ImGui::MenuItem(ICON_FA_FOLDER_OPEN "  Open..."))
					OpenScene();
				if (ImGui::MenuItem(ICON_FA_SAVE "  Save"))
					SaveSceneAs();
				if (ImGui::MenuItem(ICON_FA_TIMES "  Exit"))
					exiting = true;
				ImGui::EndMenu();
			}

			if (ImGui::BeginMenu("Components"))
			{
				CurrentPopupComponents();
				ImGui::EndMenu();
			}

			ImGui::EndMainMenuBar();
		}

		if(exiting)
			ImGui::OpenPopup("Exiting");

		if (ImGui::BeginPopupModal("Exiting", NULL, ImGuiWindowFlags_AlwaysAutoResize))
		{
			ImGui::Text("All those beautiful files will be deleted.\nThis operation cannot be undone!\n\n");
			ImGui::Separator();

			if (ImGui::Button("OK", ImVec2(120, 0)))
			{
				PP::Window::CloseWindow();
			}
			ImGui::SetItemDefaultFocus();
			ImGui::SameLine();
			if (ImGui::Button("Cancel", ImVec2(120, 0)))
			{
				ImGui::CloseCurrentPopup();
			}
			ImGui::EndPopup();
		}

		//Play button
		ImGui::PushStyleVar(ImGuiStyleVar_FramePadding, { 0,6.f });
		if (ImGui_BeginMainStatusBar())
		{
			ImGui::PopStyleVar();
			ImGui::SetCursorPosY(ImGui::GetFrameHeight() * 0.16f);

			switch (Application::GetInstance().GetState())
			{
				case Application::APPLICATIONSTATE::EDITOR:
				{
					if (ImGui::Button(ICON_FA_PLAY_CIRCLE " Play"))
					{
						Application::GetInstance().TransitionApplicationState(Application::APPLICATIONSTATE::PLAY);
						PP::Logger::m_Logs.clear();
					}
				}
				break;
				case Application::APPLICATIONSTATE::PLAY:
				{
					if (ImGui::Button(ICON_FA_STOP_CIRCLE " Stop"))
					{
						Application::GetInstance().TransitionApplicationState(Application::APPLICATIONSTATE::EDITOR);
					}
				}
				break;
				case Application::APPLICATIONSTATE::PREFAB_EDITOR:
				{
					if (ImGui::Button(ICON_FA_STEP_BACKWARD " Exit Prefab Editor"))
					{
						Application::GetInstance().ExitPrefabEditing();
					}
				}
				break;
				default:
					assert(false);
			}

			ImGui_EndMainStatusBar();
		}

		//Pause button
		ImGui::PushStyleVar(ImGuiStyleVar_FramePadding, { 0,6.f });
		if (ImGui_BeginMainStatusBar())
		{
			ImGui::PopStyleVar();
			ImGui::SetCursorPosY(ImGui::GetFrameHeight() * 0.16f);

			switch (Application::GetInstance().GetPlayState())
			{
			case Application::PLAYSTATE::PLAY:
				if (ImGui::Button(ICON_FA_PAUSE_CIRCLE " Pause"))
					Application::GetInstance().SetPlayState(Application::PLAYSTATE::PAUSE);
				break;
			default:
				if (ImGui::Button(ICON_FA_PAUSE " Paused"))
					Application::GetInstance().SetPlayState(Application::PLAYSTATE::PLAY);
			}

			if (ImGui::Button(ICON_FA_ARROW_CIRCLE_RIGHT " Step"))
			{
				Application::GetInstance().SetPlayState(Application::PLAYSTATE::STEPNEXT);
			}

			ImGui_EndMainStatusBar();
		}

		//Secondary bar
		ImGui::PushStyleVar(ImGuiStyleVar_FramePadding, { 0,6.f });
		if (ImGui_BeginMainStatusBar())
		{
			ImGui::PopStyleVar();
			ImGui::SetCursorPosY(ImGui::GetFrameHeight() * 0.16f);
			if (ImGui::Button(ICON_FA_ARROWS_ALT " Move"))
			{
				m_EditMode = ImGuizmo::TRANSLATE;
			}

			if (ImGui::Button(ICON_FA_SYNC " Rotate"))
			{
				m_EditMode = ImGuizmo::ROTATE;
			}

			if (ImGui::Button(ICON_FA_EXPAND_ALT " Scale"))
			{
				m_EditMode = ImGuizmo::SCALE;
			}

			//ImGui::SameLine(ImGui::GetContentRegionAvail().x * 0.62f);
			// Snap when editing transform
			ImGui::Checkbox("Snap Transform", &m_UseSnap);
			//Bounds edit
			ImGui::SameLine();
			ImGui::Checkbox("Edit Bounds", &m_BoundSizing);
			//Snap when editing
			ImGui::SameLine();
			ImGui::Checkbox("Snap Bounds", &m_UseBoundsSnap);
			//If to draw grid
			ImGui::SameLine();
			ImGui::Checkbox("Draw Grid", &PP::Renderer::m_RenderGrid);

			ImGui_EndMainStatusBar();
		}

		// Directory Render
		m_Directory.RenderDirectory();
		for (auto& Panel : m_Panels)
		{
			Panel.get()->Render(m_CurrentEntity);
		}

		if (ImGui::Begin("Prefab_Loaded"))
		{
			int col_count = (int)(ImGui::GetContentRegionAvail().x / 80.f);

			if (col_count < 1)
				col_count = 1;

			auto _view = m_ecs->view_SHOW_PREFAB<Components::Prefab, Components::Name>();


			if (ImGui::BeginTable("PrefabWindow", col_count))
			{
				if (ImGui::IsMouseDown(0) && ImGui::IsWindowHovered())
					m_CurrentEntity = entt::null;

				for (auto& ent : _view)
				{
					auto _transform = m_ecs->GetReg().get<Components::Transform>(ent);
					if (_transform.m_parent != entt::null)
						continue;

					ImGui::TableNextColumn();
					ImGui::PushStyleVar(ImGuiStyleVar_ButtonTextAlign, { 0.f ,1.f });
					ImGui::PushStyleColor(ImGuiCol_Button, ImVec4(0, 0, 0, 0));
					ImGui::PushID((int)ent);

					ImGui::Button(ICON_MD_DESCRIPTION, { 64.f, 64.f });

					ImGui::PopID();
					ImGui::PopStyleColor();
					ImGui::PopStyleVar();

					if (ImGui::IsItemClicked() || ImGui::IsMouseClicked(1) && ImGui::IsItemHovered())
						m_CurrentEntity = ent;

					const auto& _name = _view.get<Components::Name>(ent);

					ImGui::TextWrapped(_name.m_name.c_str());
				}

				ImGui::EndTable();
			}

			if (m_CurrentEntity != entt::null)
			{
				if (ImGui::BeginPopupContextWindow("PrefabWindowPopup", ImGuiPopupFlags_MouseButtonRight))
				{
					if (ImGui::MenuItem("Add to scene"))
					{
						auto _guid = m_ecs->GetReg().get<Components::Guid>(m_CurrentEntity);
						auto _prefab = m_ecs->GetReg().get<Components::Prefab>(m_CurrentEntity);
						m_ecs->GetReg().emplace_or_replace<Components::PrefabInstance>(m_ecs->CopyEntity(m_CurrentEntity), _guid.m_guid, _prefab.file_path);
					}
					if (ImGui::MenuItem("Edit prefab"))
					{
					const auto& prefab = _view.get<Components::Prefab>(m_CurrentEntity);
					Application::GetInstance().StartPrefabEditing(prefab.file_path);
					}

					if (ImGui::MenuItem("Save prefab"))
					{
						Application::GetInstance().ExitPrefabEditing();
					}

					if (ImGui::MenuItem("Unload prefab"))
					{
						m_ecs->DestroyEntity(m_CurrentEntity);
						m_CurrentEntity = entt::null;
					}

					ImGui::EndPopup();
				}
			}
		}
		ImGui::End();


		ImGui::Begin("Inspector");
		{
			if (m_CurrentEntity != entt::null)
			{
				//debug stuffs for ECS data
#ifdef ECS_DEBUG
				if (ImGui::CollapsingHeader(ICON_FA_FILE_SIGNATURE"  ECS_debug_info", ImGuiTreeNodeFlags_DefaultOpen))
				{
					ImGui::Text("ID: %d", m_CurrentEntity);

					auto transform = m_ecs->GetReg().try_get<Components::Transform>(m_CurrentEntity);
					if (transform)
					{
						if (transform->m_parent == entt::null)
							ImGui::Text("Parent: None");
						else
							ImGui::Text("Parent: %d", transform->m_parent);

						auto m_children = transform->m_children;

						if (!m_children.empty())
						{
							if (ImGui::CollapsingHeader(ICON_FA_FILE_SIGNATURE"  Children", ImGuiTreeNodeFlags_DefaultOpen))
								for (auto child : m_children)
									ImGui::Text("ID: %d", child);
						}
					}
					auto _guid = m_ecs->GetReg().try_get<Components::Guid>(m_CurrentEntity);
					if (_guid)
					{
						std::string str{ "GUID: " };
						str.append(_guid->m_guid);
						ImGui::Text(str.c_str());
					}

					auto _prefabinstance = m_ecs->GetReg().try_get<Components::PrefabInstance>(m_CurrentEntity);
					if (_prefabinstance)
					{
						std::string str{ "Instance of: " };
						str.append(_prefabinstance->prefab_GUID);
						ImGui::Text(str.c_str());
					}
					ImguiBlankSeperator(1);
					ImGui::Separator();
				}
#endif

				auto naming = m_ecs->GetReg().try_get<Components::Name>(m_CurrentEntity);
				if (naming && ImGui::CollapsingHeader(ICON_FA_FILE_SIGNATURE"  Name", ImGuiTreeNodeFlags_DefaultOpen))
				{
					bool _prev_status = naming->status;
					ImGui::Checkbox("Enable entity", &naming->status);
					Reflect_ImGui(naming);

					if (_prev_status != naming->status)
					{
						if (naming->status)
							m_ecs->EnableEntity(m_CurrentEntity);
						else
							m_ecs->DisableEntity(m_CurrentEntity);
					}
				}

				auto tagging = m_ecs->GetReg().try_get<Components::Tag>(m_CurrentEntity);

				if (tagging)
				{
					bool enable_tagging = true;
					if (ImGui::CollapsingHeader(ICON_FA_TAGS "  Tag", &enable_tagging, ImGuiTreeNodeFlags_DefaultOpen))
					{
						Reflect_ImGui(tagging);
					}
					if (!enable_tagging)
						m_ecs->GetReg().remove<Components::Tag>(m_CurrentEntity);
				}

				auto transform = m_ecs->GetReg().try_get<Components::Transform>(m_CurrentEntity);
				if (transform && ImGui::CollapsingHeader(ICON_FA_SLIDERS_H "  Transform", ImGuiTreeNodeFlags_DefaultOpen))
				{
					Reflect_ImGui(transform);
				}

				auto transformDebugger = m_ecs->GetReg().try_get<Components::TransformDebugger>(m_CurrentEntity);
				if (transformDebugger && transform)
				{
					bool enable_transformdebugger = true;
					if (ImGui::CollapsingHeader("  Transform Debugger", &enable_transformdebugger, ImGuiTreeNodeFlags_DefaultOpen))
					{
						ImGui::PushID("##TDebugger");
						auto tmpPos = transform->GetGlobalPosition();
						auto tmpRot = transform->GetGlobalRotation();
						auto tmpScale = transform->GetGlobalScale();

						CreateDragFloat3("Global Position", glm::value_ptr(tmpPos));
						CreateDragFloat3("Global Rotation", glm::value_ptr(tmpRot));
						CreateDragFloat3("Global Scale", glm::value_ptr(tmpScale));

						transform->SetGlobalPosition(tmpPos);
						transform->SetGlobalRotation(tmpRot);
						transform->SetGlobalScale(tmpScale);

						CreateDragFloat3("Local Position", glm::value_ptr(transform->m_position));
						CreateDragFloat3("Local Rotation", glm::value_ptr(transform->m_rotation));
						CreateDragFloat3("Local Scale", glm::value_ptr(transform->m_scale));

						ImguiBlankSeperator(1);

						ImGui::Checkbox("Draw Forward Vector", &transformDebugger->m_drawForwardVector);

						if (transformDebugger->m_drawForwardVector)
						{
							ImGui::DragFloat("Vector Length", &transformDebugger->m_forwardVectorLength, 0.2f, 0.1f, 10.f);

							Pogplant::DebugDraw::DebugLine(tmpPos, tmpPos + transform->GetForwardVector() * transformDebugger->m_forwardVectorLength);
						}
						
						if (ImGui::Button("Set LookAt to (0, 0, 0)"))
						{
							transform->LookAt(glm::vec3{ 0.f,0.f,0.f });
						}

						ImGui::PopID();
						if (!enable_transformdebugger)
							m_ecs->GetReg().remove<Components::TransformDebugger>(m_CurrentEntity);
					}
				}

				RendererComponentHelper();
				PRendererComponentHelper();
				ParticlesHelper();
				CanvasHelper();

				auto point_light = m_ecs->GetReg().try_get<Components::Point_Light>(m_CurrentEntity);
				if (point_light)
				{
					bool enable_pointlight = true;
					if (ImGui::CollapsingHeader(ICON_FA_LIGHTBULB "  Point Lighting", &enable_pointlight, ImGuiTreeNodeFlags_DefaultOpen))
					{
						ImGui::Text("Light Color Editor");
						ImGui::ColorEdit3("###PLight", glm::value_ptr(point_light->m_Color));

						ImGui::Text("Light Intensity");
						ImGui::InputFloat("###PLI", &point_light->m_Intensity, 0.1f, 1.0f, "%.3f");

						ImGui::Text("Linear Attenuation");
						ImGui::InputFloat("###PLA", &point_light->m_Linear, 0.001f, 1.0f, "%.3f");

						ImGui::Text("Quadratic Attenuation");
						ImGui::InputFloat("###PQA", &point_light->m_Quadratic, 0.001f, 1.0f, "%.3f");

						ImguiBlankSeperator(1);
						ImGui::Separator();
					}
					if (!enable_pointlight)
					{
						m_ecs->GetReg().remove<Components::Point_Light>(m_CurrentEntity);
					}

				}

				auto direction_light = m_ecs->GetReg().try_get<Components::Directional_Light>(m_CurrentEntity);
				if (direction_light)
				{
					bool enable_directionlight = true;
					if (ImGui::CollapsingHeader(ICON_FA_CLOUD_SUN "  Directional Lighting", &enable_directionlight, ImGuiTreeNodeFlags_DefaultOpen))
					{
						ImGui::Text("Direction");
						ImGui::DragFloat3("###Ddir", glm::value_ptr(direction_light->m_Direction));

						ImGui::Text("Light Color Editor");
						ImGui::ColorEdit3("###DLight", glm::value_ptr(direction_light->m_Color));

						ImGui::Text("Light Intensity");
						ImGui::InputFloat("###DLI", &direction_light->m_Intensity, 0.1f, 1.0f, "%.3f");

						ImGui::Text("Diffusion");
						ImGui::InputFloat("###Ddif", &direction_light->m_Diffuse, 0.01f, 1.0f, "%.3f");

						ImGui::Text("Specular");
						ImGui::InputFloat("###Dspec", &direction_light->m_Specular, 0.01f, 1.0f, "%.3f");

						ImguiBlankSeperator(1);
						ImGui::Separator();
					}

					if (!enable_directionlight)
					{
						m_ecs->GetReg().remove<Components::Directional_Light>(m_CurrentEntity);
					}

				}

				auto text = m_ecs->GetReg().try_get<Components::Text>(m_CurrentEntity);
				if (text)
				{
					bool enable_text = true;
					if (ImGui::CollapsingHeader(ICON_FA_TEXT_HEIGHT "  Text Editor", &enable_text, ImGuiTreeNodeFlags_DefaultOpen))
					{
						ImGui::Text("Color");
						ImGui::ColorEdit3("Tcol", glm::value_ptr(text->m_Color));
						PP::FontResource::m_FontPool;


						ImGuiComboFlags flag = 0;
						flag |= ImGuiComboFlags_PopupAlignLeft;
						auto font_itr = PP::FontResource::m_FontPool.find(text->m_FontID);
						ImGui::Text("Fonts");
						if (ImGui::BeginCombo("Fnt", font_itr->first.c_str(), flag))
						{
							for (auto it = PP::FontResource::m_FontPool.begin(); it != PP::FontResource::m_FontPool.end(); ++it)
							{
								const bool  is_selected = (font_itr == it);
								if (ImGui::Selectable(it->first.c_str(), is_selected))
								{
									font_itr = it;
									text->m_FontID = font_itr->first;
								}

								if (is_selected)
									ImGui::SetItemDefaultFocus();

							}
							ImGui::EndCombo();
						}

						ImGui::Text("UI font");
						static char font_stuff[256] = "";
						sprintf_s(font_stuff, IM_ARRAYSIZE(font_stuff), text->m_Text.c_str());
						ImGui::InputText("TextInput", font_stuff, IM_ARRAYSIZE(font_stuff));
						text->m_Text = font_stuff;
						ImguiBlankSeperator(1);
						ImGui::Checkbox("Use Ortho", &text->m_Ortho);
						ImguiBlankSeperator(1);
						ImGui::Checkbox("Center text", &text->m_Center);
						ImguiBlankSeperator(1);
						ImGui::Text("Play Speed");
						ImGui::DragFloat("###PlaySpeedText", &text->m_PlaySpeed);
						ImGui::Text("Current Index %d, Min. Index %d, Max. Index %d", text->m_CurrentIndex, text->m_IndexMin, text->m_IndexMax);
						ImGui::Text("Timer %f, Delay %f", text->m_Timer, text->m_Delay);
						ImguiBlankSeperator(1);
						if (text->m_Play)
						{
							if (ImGui::Button("Pause"))
							{
								text->m_Play = false;
							}
						}
						else
						{
							if (ImGui::Button("Play"))
							{
								text->m_Play = true;
							}
						}

						ImguiBlankSeperator(2);
					}
					if (!enable_text)
					{
						m_ecs->GetReg().remove<Components::Text>(m_CurrentEntity);
					}
					ImguiBlankSeperator(1);
					ImGui::Separator();
				}

				auto rigid = m_ecs->GetReg().try_get<Components::Rigidbody>(m_CurrentEntity);
				if (rigid)
				{
					bool enable_rigid = true;

					if (ImGui::CollapsingHeader(ICON_FA_SKIING  "RigidBody", &enable_rigid, ImGuiTreeNodeFlags_DefaultOpen))
					{
						ImGui::Text("Mass");
						ImGui::InputFloat("###Mass", &rigid->mass , 0.1f, 1.0f, "%.3f");
						ImGui::Checkbox("Kinematic", &rigid->isKinematic);
						ImGui::SameLine();
						ImGui::Checkbox("Gravity", &rigid->useGravity);

						ImguiBlankSeperator(1);
						ImGui::Separator();
					}
					if (!enable_rigid)
					{
						m_ecs->GetReg().remove<Components::Rigidbody>(m_CurrentEntity);
					}

				}


				auto box_collider = m_ecs->GetReg().try_get<Components::BoxCollider>(m_CurrentEntity);
				if (box_collider)
				{
					bool enable_box_collider = true;

					if (ImGui::CollapsingHeader(ICON_FA_BOXES "  Collider Box", &enable_box_collider, ImGuiTreeNodeFlags_DefaultOpen))
					{
						ImGuiComboFlags flag = 0;
						flag |= ImGuiComboFlags_PopupAlignLeft;

						ImGui::Text("Extend");
						ImGui::DragFloat3("###CExt", glm::value_ptr(box_collider->extends));

						ImGui::Text("Centre");
						ImGui::DragFloat3("###CCen", glm::value_ptr(box_collider->centre));

						ImGui::Checkbox("Is Trigger?", &box_collider->isTrigger);
						ImGui::Checkbox("Is Static?", &box_collider->isStatic);

						auto identifier = m_ecs->GetReg().try_get<Components::ColliderIdentifier>(m_CurrentEntity);
						if (identifier)
							identifier->isTrigger = box_collider->isTrigger;

						/**Collision Layer editor**/
						ImguiBlankSeperator(1);

						auto& collisionLayers = Application::GetInstance().m_sPhysicsSystem.m_collisionLayers;

						ImGui::Text(ICON_FA_LAYER_GROUP " Collision Layer");
						if (ImGui::BeginCombo("###AxisKeys", box_collider->collisionLayer.c_str(), ImGuiComboFlags_PopupAlignLeft))
						{
							for (auto itr = collisionLayers.begin(); itr != collisionLayers.end(); ++itr)
							{
								const bool isSelected = (itr->first == box_collider->collisionLayer);

								if (ImGui::Selectable(itr->first.c_str(), isSelected))
								{
									box_collider->collisionLayer = itr->first;

									if (identifier)
										identifier->collisionLayer = itr->second;
								}

								if (isSelected)
									ImGui::SetItemDefaultFocus();
							}

							ImGui::EndCombo();
						}
						/****/

						ImguiBlankSeperator(1);
						ImGui::Separator();
					}
					if (!enable_box_collider)
					{
						m_ecs->GetReg().remove<Components::BoxCollider>(m_CurrentEntity);
						
						if (m_ecs->GetReg().try_get<Components::ColliderIdentifier>(m_CurrentEntity))
							m_ecs->GetReg().remove<Components::ColliderIdentifier>(m_CurrentEntity);
					}
				}

				auto movementBound = m_ecs->GetReg().try_get<Components::MovementBounds>(m_CurrentEntity);
				if (movementBound)
				{
					bool enable_move_bound = true;
					if (ImGui::CollapsingHeader(ICON_FA_BOXES "  Movement Bounds", &enable_move_bound, ImGuiTreeNodeFlags_DefaultOpen))
					{
						//Reflect_ImGui(movementBound);//didn't work

						ImGui::DragFloat("minX", &movementBound->minX, 1.f , 0.1f);
						ImGui::DragFloat("maxX", &movementBound->maxX, 1.f , 0.1f);
						ImGui::DragFloat("minY", &movementBound->minY, 1.f , 0.1f);
						ImGui::DragFloat("maxY", &movementBound->maxY, 1.f , 0.1f);
					}
					
					if (!enable_move_bound)
						m_ecs->GetReg().remove<Components::MovementBounds>(m_CurrentEntity);
				}

				auto box_colliderOBB = m_ecs->GetReg().try_get <Components::OBBBoxCollider> (m_CurrentEntity);
				if (box_colliderOBB)
				{
					bool enable_box_collider = true;

					if (ImGui::CollapsingHeader(ICON_FA_BOXES "  Collider Box OBB", &enable_box_collider, ImGuiTreeNodeFlags_DefaultOpen))
					{
						ImGuiComboFlags flag = 0;
						flag |= ImGuiComboFlags_PopupAlignLeft;

						ImGui::Text("Extend");
						ImGui::DragFloat3("###CExt", glm::value_ptr(box_colliderOBB->extends));

						ImGui::Text("Centre");
						ImGui::DragFloat3("###CCen", glm::value_ptr(box_colliderOBB->centre));

						ImGui::Checkbox("Is Trigger?", &box_colliderOBB->isTrigger);
						ImGui::Checkbox("Is Static?", &box_colliderOBB->isStatic);

						auto identifier = m_ecs->GetReg().try_get<Components::ColliderIdentifier>(m_CurrentEntity);
						if (identifier)
							identifier->isTrigger = box_colliderOBB->isTrigger;

						/**Collision Layer editor**/
						ImguiBlankSeperator(1);

						auto& collisionLayers = Application::GetInstance().m_sPhysicsSystem.m_collisionLayers;

						ImGui::Text(ICON_FA_LAYER_GROUP " Collision Layer");
						if (ImGui::BeginCombo("###AxisKeys", box_colliderOBB->collisionLayer.c_str(), ImGuiComboFlags_PopupAlignLeft))
						{
							for (auto itr = collisionLayers.begin(); itr != collisionLayers.end(); ++itr)
							{
								const bool isSelected = (itr->first == box_colliderOBB->collisionLayer);

								if (ImGui::Selectable(itr->first.c_str(), isSelected))
								{
									box_colliderOBB->collisionLayer = itr->first;

									if (identifier)
										identifier->collisionLayer = itr->second;
								}

								if (isSelected)
									ImGui::SetItemDefaultFocus();
							}

							ImGui::EndCombo();
						}
						/****/

						ImguiBlankSeperator(1);
						ImGui::Separator();
					}

					if (!enable_box_collider)
					{
						m_ecs->GetReg().remove<Components::OBBBoxCollider>(m_CurrentEntity);

						if (m_ecs->GetReg().try_get<Components::ColliderIdentifier>(m_CurrentEntity))
							m_ecs->GetReg().remove<Components::ColliderIdentifier>(m_CurrentEntity);
					}
				}

				auto sphere_collider = m_ecs->GetReg().try_get<Components::SphereCollider>(m_CurrentEntity);
				if (sphere_collider)
				{
					bool enable_sphere_collider = true;

					if (ImGui::CollapsingHeader(ICON_FA_USER_CIRCLE "  Collider Sphere", &enable_sphere_collider, ImGuiTreeNodeFlags_DefaultOpen))
					{
						ImGui::Text("Centre");
						ImGui::DragFloat3("###CCen", glm::value_ptr(sphere_collider->centre));

						ImGui::Text("Radius");
						ImGui::InputFloat("###CRad", &sphere_collider->radius, 0.01f, 1.0f, "%.3f");

						ImGui::Checkbox("Is Trigger?", &sphere_collider->isTrigger);
						ImGui::Checkbox("Is Static?", &sphere_collider->isStatic);

						auto identifier = m_ecs->GetReg().try_get<Components::ColliderIdentifier>(m_CurrentEntity);
						if (identifier)
							identifier->isTrigger = sphere_collider->isTrigger;

						/**Collision Layer editor**/
						ImguiBlankSeperator(1);

						auto& collisionLayers = Application::GetInstance().m_sPhysicsSystem.m_collisionLayers;

						ImGui::Text(ICON_FA_LAYER_GROUP " Collision Layer");
						if (ImGui::BeginCombo("###AxisKeys", sphere_collider->collisionLayer.c_str(), ImGuiComboFlags_PopupAlignLeft))
						{
							for (auto itr = collisionLayers.begin(); itr != collisionLayers.end(); ++itr)
							{
								const bool isSelected = (itr->first == sphere_collider->collisionLayer);

								if (ImGui::Selectable(itr->first.c_str(), isSelected))
								{
									sphere_collider->collisionLayer = itr->first;

									if (identifier)
										identifier->collisionLayer = itr->second;
								}

								if (isSelected)
									ImGui::SetItemDefaultFocus();
							}

							ImGui::EndCombo();
						}
						/****/


						ImguiBlankSeperator(1);
						ImGui::Separator();
					}
					if (!enable_sphere_collider)
					{
						m_ecs->GetReg().remove<Components::SphereCollider>(m_CurrentEntity);

						if(m_ecs->GetReg().try_get<Components::ColliderIdentifier>(m_CurrentEntity))
							m_ecs->GetReg().remove<Components::ColliderIdentifier>(m_CurrentEntity);
					}
				}

				auto meshCollider = m_ecs->GetReg().try_get <Components::MeshCollider>(m_CurrentEntity);
				if (meshCollider)
				{
					bool enableMeshCollider = true;

					if (ImGui::CollapsingHeader(ICON_FA_DRAW_POLYGON "  Mesh Collider", &enableMeshCollider, ImGuiTreeNodeFlags_DefaultOpen))
					{
						ImGuiComboFlags flag = 0;
						flag |= ImGuiComboFlags_PopupAlignLeft;

						ImGui::Checkbox("Is Trigger?", &meshCollider->isTrigger);
						ImGui::Checkbox("Is Static?", &meshCollider->isStatic);

						auto identifier = m_ecs->GetReg().try_get<Components::ColliderIdentifier>(m_CurrentEntity);
						if (identifier)
							identifier->isTrigger = meshCollider->isTrigger;

						/**Collision Layer editor**/
						ImguiBlankSeperator(1);

						auto& collisionLayers = Application::GetInstance().m_sPhysicsSystem.m_collisionLayers;

						ImGui::Text(ICON_FA_LAYER_GROUP " Collision Layer");
						if (ImGui::BeginCombo("###AxisKeys", meshCollider->collisionLayer.c_str(), ImGuiComboFlags_PopupAlignLeft))
						{
							for (auto itr = collisionLayers.begin(); itr != collisionLayers.end(); ++itr)
							{
								const bool isSelected = (itr->first == meshCollider->collisionLayer);

								if (ImGui::Selectable(itr->first.c_str(), isSelected))
								{
									meshCollider->collisionLayer = itr->first;

									if (identifier)
										identifier->collisionLayer = itr->second;
								}

								if (isSelected)
									ImGui::SetItemDefaultFocus();
							}

							ImGui::EndCombo();
						}
						/****/

						ImguiBlankSeperator(1);
						ImGui::Separator();
					}

					if (!enableMeshCollider)
					{
						m_ecs->GetReg().remove<Components::MeshCollider>(m_CurrentEntity);

						if (m_ecs->GetReg().try_get<Components::ColliderIdentifier>(m_CurrentEntity))
							m_ecs->GetReg().remove<Components::ColliderIdentifier>(m_CurrentEntity);
					}
				}

				auto camera_com = m_ecs->GetReg().try_get<Components::Camera>(m_CurrentEntity);
				if (camera_com)
				{
					bool enable_camera_com = true;

					if (ImGui::CollapsingHeader(ICON_FA_CAMERA_RETRO "  Camera", &enable_camera_com, ImGuiTreeNodeFlags_DefaultOpen))
					{

						ImGui::Text("Active Cam");
						ImGui::Checkbox("Active?", &camera_com->m_Active);

						ImGui::Text("Yaw");
						ImGui::DragFloat("###yaw", &camera_com->m_Yaw, 1.0f);
						ImGui::Text("Pitch");
						ImGui::DragFloat("###pitch", &camera_com->m_Pitch, 1.0f);
						ImGui::Text("Roll");
						ImGui::DragFloat("###roll", &camera_com->m_Roll, 1.0f);

						ImGui::Text("Near");
						ImGui::DragFloat("###near", &camera_com->m_Near, 1.0f);
						ImGui::Text("Far");
						ImGui::DragFloat("###far", &camera_com->m_Far, 1.0f);
						ImGui::Text("Fov");
						ImGui::DragFloat("###fov", &camera_com->m_Fov, 1.0f);

						ImguiBlankSeperator(1);
						ImGui::Separator();
					}
					if (!enable_camera_com)
					{
						m_ecs->GetReg().remove<Components::Camera>(m_CurrentEntity);
					}
				}

				//Laser Component
				auto laser_com = m_ecs->GetReg().try_get<Components::Laser>(m_CurrentEntity);
				if (laser_com)
				{
					bool enable_laser_com = true;

					if (ImGui::CollapsingHeader(ICON_FA_CAMERA_RETRO "  Laser", &enable_laser_com, ImGuiTreeNodeFlags_DefaultOpen))
					{

						ImGui::Text("ActivateLaser");
						ImGui::Checkbox("###ActivateLaser", &laser_com->m_ActivateLaser);
						
						ImGui::Text("Laser Activation time");
						ImGui::DragFloat("###laser_activate", &laser_com->m_Spawntime, 1.0f);

						ImGui::Text("ActiveTime");
						ImGui::DragFloat("###ActiveTime", &laser_com->m_Activetime, 1.0f);

						ImGui::Text("InactiveTime");
						ImGui::DragFloat("###InactiveTime", &laser_com->m_Inactivetime, 1.0f);

						ImGui::Text("Lerp Time");
						ImGui::DragFloat("###Lerp Time", &laser_com->m_LaserLerptime, 1.0f);

						ImguiBlankSeperator(1);
						ImGui::Separator();
					}
					if (!enable_laser_com)
					{
						m_ecs->GetReg().remove<Components::Laser>(m_CurrentEntity);
					}
				}

				//Audio component
				auto audioSourceComponent = m_ecs->GetReg().try_get<Components::AudioSource>(m_CurrentEntity);
				if (audioSourceComponent)
				{
					bool enable_audio_com = true;

					if (ImGui::CollapsingHeader(ICON_FA_MUSIC "  AudioSource", &enable_audio_com, ImGuiTreeNodeFlags_DefaultOpen))
					{
						ImGui::PushID("AudioSourcesID");

						for (size_t i = 0; i < audioSourceComponent->m_audioSources.size(); i++)
						{
							ImGui::PushID((int)i);
							
							auto audioClipIt = PPA::AudioResource::AudioPool().find(audioSourceComponent->m_audioSources[i].m_fileDir);

							if (audioClipIt == PPA::AudioResource::AudioPool().end())
							{
								audioClipIt = PPA::AudioResource::AudioPool().begin();
								audioSourceComponent->m_audioSources[i].m_fileDir = audioClipIt->first;
								audioSourceComponent->m_audioSources[i].m_audioClip = &audioClipIt->second;
							}

							std::stringstream ssT;
							ssT << "AudioSource " << i;

							ImGui::Text(ssT.str().c_str());
							if (ImGui::BeginCombo("###Sound", audioClipIt->first.c_str(), ImGuiComboFlags_PopupAlignLeft))
							{
								for (auto it = PPA::AudioResource::AudioPool().begin(); it != PPA::AudioResource::AudioPool().end(); ++it)
								{
									const bool is_selected = (audioClipIt == it);
									if (ImGui::Selectable(it->first.c_str(), is_selected))
									{
										audioClipIt = it;
										
										audioSourceComponent->m_audioSources[i].m_fileDir = audioClipIt->first;
										audioSourceComponent->m_audioSources[i].m_audioClip = &audioClipIt->second;
									}

									if (is_selected)
										ImGui::SetItemDefaultFocus();
								}

								ImGui::EndCombo();
							}

							bool hasChanged = false;
							hasChanged |= ImGui::Checkbox("Audio follows object", &audioSourceComponent->m_audioSources[i].m_update3DPosition);
							hasChanged |= ImGui::SliderFloat("Volume", &audioSourceComponent->m_audioSources[i].m_volume, 0.f, 1.f);

							//
							ImGui::Text("AudioClip - Below settings are bound to audio object");

							if (audioSourceComponent->m_audioSources[i].m_audioClip)
							{
								hasChanged |= ImGui::Checkbox("Is 3D", &audioSourceComponent->m_audioSources[i].m_audioClip->m_is3D);
								hasChanged |= ImGui::Checkbox("Loop", &audioSourceComponent->m_audioSources[i].m_audioClip->m_isLooping);
								hasChanged |= ImGui::Checkbox("Stream audio", &audioSourceComponent->m_audioSources[i].m_audioClip->m_isStreamed);
								hasChanged |= ImGui::Checkbox("Enable doppler", &audioSourceComponent->m_audioSources[i].m_audioClip->m_enableDopplerEffect);
							}

							if (hasChanged)
								audioSourceComponent->UpdateAudioSettings(i);

							if (ImGui::Button("Play"))
								audioSourceComponent->PlayAudio(i);

							ImGui::SameLine();

							if (ImGui::Button("Stop"))
								audioSourceComponent->StopAudio(i);

							if (ImGui::Button("Remove"))
							{
								audioSourceComponent->m_audioSources.erase(audioSourceComponent->m_audioSources.begin() + i);
								ImGui::PopID();
								break;
							}

							ImguiBlankSeperator(1);
							ImGui::PopID();
						}
						

						if (ImGui::Button("Add audio Object"))
						{
							audioSourceComponent->m_audioSources.push_back(Components::AudioSource::AudioObject());
						}

						ImGui::PopID();

						ImguiBlankSeperator(1);
						ImGui::Separator();
					}
					if (!enable_audio_com)
					{
						m_ecs->GetReg().remove<Components::AudioSource>(m_CurrentEntity);
					}
				}

				// Scriptable Component
				auto scripts_com = m_ecs->GetReg().try_get<Components::Scriptable>(m_CurrentEntity);
				auto name_com = m_ecs->GetReg().try_get<Components::Name>(m_CurrentEntity);
				if (scripts_com && name_com)
				{
					bool enable_scripts_com = true;

					if (ImGui::CollapsingHeader(ICON_FA_SCROLL "  Mono Scripts", &enable_scripts_com, ImGuiTreeNodeFlags_DefaultOpen))
					{	
						for (auto& scriptName : ScriptResource::m_MonoScriptNames)
						{
							bool hasScript = scripts_com->m_ScriptTypes.contains(scriptName);
							bool setScript = hasScript;
							ImGui::Checkbox(scriptName.c_str(), &setScript);
							if (setScript != hasScript)
							{
								if (setScript == false)
								{
									scripts_com->m_ScriptTypes.erase(scriptName);
									std::cout << "Entity [" << name_com->m_name << "] has removed script [" << scriptName << "]" << std::endl;
								}
								else
								{
									scripts_com->m_ScriptTypes[scriptName] = false;
									std::cout << "Entity [" << name_com->m_name << "] has added script [" << scriptName << "]" << std::endl;
								}
							}
						}
					}

					if (!enable_scripts_com)
					{
						m_ecs->GetReg().remove<Components::Scriptable>(m_CurrentEntity);
						std::cout << "Entity [" << name_com->m_name << "] has removed mono script component" << std::endl;
					}
				}

				auto pausescripts_com = m_ecs->GetReg().try_get<Components::PauseScriptable>(m_CurrentEntity);
				if (pausescripts_com && name_com)
				{
					bool enable_scripts_com = true;

					if (ImGui::CollapsingHeader(ICON_FA_SCROLL "  Pause Scripts", &enable_scripts_com, ImGuiTreeNodeFlags_DefaultOpen))
					{
						for (auto& scriptName : ScriptResource::m_PauseScriptNames)
						{
							bool hasScript = pausescripts_com->m_ScriptTypes.contains(scriptName);
							bool setScript = hasScript;
							ImGui::Checkbox(scriptName.c_str(), &setScript);
							if (setScript != hasScript)
							{
								if (setScript == false)
								{
									pausescripts_com->m_ScriptTypes.erase(scriptName);
									std::cout << "Entity [" << name_com->m_name << "] has removed script [" << scriptName << "]" << std::endl;
								}
								else
								{
									pausescripts_com->m_ScriptTypes[scriptName] = false;
									std::cout << "Entity [" << name_com->m_name << "] has added script [" << scriptName << "]" << std::endl;
								}
							}
						}
					}

					if (!enable_scripts_com)
					{
						m_ecs->GetReg().remove<Components::PauseScriptable>(m_CurrentEntity);
						std::cout << "Entity [" << name_com->m_name << "] has removed pause script component" << std::endl;
					}
				}

				auto scriptVariableCom = m_ecs->GetReg().try_get<Components::ScriptVariables>(m_CurrentEntity);
				if (scriptVariableCom)
				{
					bool enable_scripts_com = true;

					if (ImGui::CollapsingHeader(ICON_FA_SD_CARD "  Script Variables", &enable_scripts_com, ImGuiTreeNodeFlags_DefaultOpen))
					{
						for (auto itr = scriptVariableCom->m_variables.begin(); itr != scriptVariableCom->m_variables.end(); ++itr)
						{
							if (ImGui::TreeNode(itr->first.c_str()))
							{
								ImGui::Text("Type");
								ImGui::SameLine();

								Components::ScriptVariables::Variable::Type selectedType = itr->second.m_type;

								if (ImGui::BeginCombo("###ValueType", scriptVariableCom->GetTypeName(selectedType).c_str(), ImGuiComboFlags_PopupAlignLeft))
								{
									for (int i = 0; i < (int)Components::ScriptVariables::Variable::Type::TOTAL; ++i)
									{
										const bool isSelected = (i == (int)selectedType);

										if (ImGui::Selectable(scriptVariableCom->GetTypeName((Components::ScriptVariables::Variable::Type)i).c_str(), isSelected))
										{
											itr->second.UpdateType((Components::ScriptVariables::Variable::Type)i);
										}

										if (isSelected)
											ImGui::SetItemDefaultFocus();
									}

									ImGui::EndCombo();
								}

								switch (itr->second.m_type)
								{
								case Components::ScriptVariables::Variable::Type::FLOAT:
								{
									float value = itr->second.GetValue<float>();

									if (ImGui::InputFloat("Float", &value))
										itr->second.SetValue(value);
									break;
								}
								case Components::ScriptVariables::Variable::Type::INT:
								{
									int value = itr->second.GetValue<int>();
									if (ImGui::InputInt("Int", &value))
										itr->second.SetValue(value);
									break;
								}
								case Components::ScriptVariables::Variable::Type::BOOL:
								{
									bool value = itr->second.GetValue<bool>();
									if (ImGui::Checkbox("Bool", &value))
										itr->second.SetValue(value);
									break;
								}
								case Components::ScriptVariables::Variable::Type::STRING:
								{
									std::string _str = CreateStringInputField("String", itr->second.GetValue<std::string>());
									itr->second.SetValue(_str);
									break;
								}
								case Components::ScriptVariables::Variable::Type::VECTOR3:
								{
									glm::vec3 value = itr->second.GetValue<glm::vec3>();

									CreateDragFloat3("Vec3", glm::value_ptr(value));

									itr->second.SetValue<glm::vec3>(value);
									break;
								}
								default:
									assert(false && "Something exploded");
									break;
								}

								ImGui::Spacing();

								//Remove key from map
								if (ImGui::Button(ICON_FA_MINUS_CIRCLE " Remove"))
								{
									scriptVariableCom->m_variables.erase(itr);
									ImGui::TreePop();
									break;
								}

								ImGui::TreePop();
							}
						}

						//Add new key
						static char name_stuff[256] = "";
						ImGui::InputText("Variable name", name_stuff, IM_ARRAYSIZE(name_stuff));

						if (ImGui::Button(ICON_FA_PLUS_CIRCLE " Add variable"))
						{
							scriptVariableCom->m_variables.insert(
								{ std::string(name_stuff),
								Components::ScriptVariables::Variable{} });
						}
					}

					if (!enable_scripts_com)
					{
						m_ecs->GetReg().remove<Components::ScriptVariables>(m_CurrentEntity);
					}
				}

				ImGui::Separator();
				ImguiBlankSeperator(2);

				float lineHeight = GImGui->Font->FontSize + GImGui->Style.FramePadding.y * 2.0f;
				float lineWidth = lineHeight + 100.f;
				ImGui::SameLine(ImGui::GetContentRegionAvail().x * 0.5f - lineWidth + lineWidth * 0.5f);
				ImVec2 buttonSize = { lineWidth , lineHeight };
				if (ImGui::Button("Add Component", buttonSize))
				{
					ImGui::OpenPopup("AddComPop");
				}
				ImGui::Unindent();
				ImguiBlankSeperator(2);
				if (ImGui::BeginPopup("AddComPop"))
				{
					CurrentPopupComponents();
					ImGui::EndPopup();
				}
			}
		}
		ImGui::End();

		ImGui::Begin("Globals");
		{
			Pogplant::Camera4D* currQuatCam = PP::CameraResource::GetCamera("EDITOR");
			ImGui::PushItemWidth(69.0f);
			ImGui::Checkbox("Shadows", &PP::Renderer::m_EnableShadows);
			ImGui::DragFloat("Bloom Dampening", &PP::Renderer::m_BloomDamp, 0.05f);
			ImGui::DragFloat("Exposure", &PP::Renderer::m_Exposure, 0.05f);
			ImGui::DragFloat("Gamma", &PP::Renderer::m_Gamma, 0.05f);
			ImGui::DragInt("Depth Layer Debug", &PP::Renderer::m_Layer);
			//auto* camera = &PP::CameraResource::m_QuatCam;
			ImGui::DragFloat("Editor Cam Near", &currQuatCam->m_Near);
			ImGui::DragFloat("Editor Cam Far", &currQuatCam->m_Far);
			ImGui::Dummy({ 0,2.0f });

			ImGui::Text("Camera config");
			ImGui::PushItemWidth(207.0f);
			ImGui::DragFloat3("Camera Pos", &currQuatCam->m_Position.x);
			ImGui::PopItemWidth();
			ImGui::DragFloat("Camera Speed", &currQuatCam->m_Speed);
			ImGui::Dummy({ 0,2.0f });

		/*	ImGui::Text("Light Shaft");
			ImGui::PushItemWidth(207.0f);
			ImGui::DragFloat3("Position", &PP::Renderer::m_LightShaftPos.x);
			ImGui::PopItemWidth();
			ImGui::DragFloat("Scale", &PP::Renderer::m_LightShaftScale);
			ImGui::DragFloat("Decay", &PP::Renderer::m_LightShaftDecay);
			ImGui::DragFloat("Exposure", &PP::Renderer::m_LightShaftExposure);
			ImGui::DragFloat("Density", &PP::Renderer::m_LightShaftDensity);
			ImGui::DragFloat("Weight", &PP::Renderer::m_LightShaftWeight);
			ImGui::PopItemWidth();*/

			ImGui::Text("AO Config");
			ImGui::DragFloat("Radius", &PP::Renderer::m_AO_Radius);
			ImGui::DragFloat("Bias ", &PP::Renderer::m_AO_Bias);
			ImGui::Dummy({ 0,2.0f });

			ImGui::PushItemWidth(207.0f);
			ImGui::Text("Quat Pos");
			ImGui::DragFloat3("###Quat Pos", &PP::Renderer::m_QuatTestPos .x);
			ImGui::Text("Quat Rot");
			ImGui::DragFloat3("###Quat Rot", &PP::Renderer::m_QuatTestRot.x);
			ImGui::Text("Quat Scale");
			ImGui::DragFloat3("###Quat Scale", &PP::Renderer::m_QuatTestScale.x);
			ImGui::PopItemWidth();



		}
		ImGui::End();

		ImGui::Begin("Scene");
		{
			if (ImGui::IsWindowFocused())
			{
				if (ImGui::IsKeyPressed('2'))
					m_EditMode = ImGuizmo::TRANSLATE;
				if (ImGui::IsKeyPressed('3'))
					m_EditMode = ImGuizmo::ROTATE;
				if (ImGui::IsKeyPressed('4'))
					m_EditMode = ImGuizmo::SCALE;
				if (ImGui::IsKeyPressed(GLFW_KEY_DELETE))
				{
					if (m_CurrentEntity != entt::null)
					{
						m_ecs->DestroyEntity(m_CurrentEntity);
						m_CurrentEntity = entt::null;
					}
				}

				Pogplant::Camera4D* currQuatCam = PP::CameraResource::GetCamera("EDITOR");
				if (currQuatCam && PPI::InputSystem::onKeyHeld(GLFW_KEY_X))
				{
					if (m_CurrentEntity != entt::null)
					{
						auto& transform = m_ecs->GetReg().get<Components::Transform>(m_CurrentEntity);
						currQuatCam->m_Position = transform.GetGlobalPosition() + glm::vec3{ 0.f, 0.f, 5.f };
						currQuatCam->m_Yaw = 0.f;
						currQuatCam->m_Pitch = 0.f;
						currQuatCam->m_Roll = 0.f;
						currQuatCam->UpdateVectors();
					}
				}
			}

			SceneWindow();
		}
		ImGui::End();

		ImGui::Begin("Game");
		{
			GameWindow();

			if (ImGui::IsItemClicked() && ImGui::IsWindowFocused() && !m_FirstRun)
			{
				PP::Window::HideCursor();
			}
			/*if (ImGui::IsKeyPressed(GLFW_KEY_Z))
			{
				Application::GetInstance().SetPlayState(Application::PLAYSTATE::PAUSE);
			}
			if (ImGui::IsKeyPressed(GLFW_KEY_X))
			{
				Application::GetInstance().SetPlayState(Application::PLAYSTATE::PLAY);
			}
			if (ImGui::IsKeyPressed(GLFW_KEY_C))
			{
				Application::GetInstance().SetPlayState(Application::PLAYSTATE::STEPNEXT);
			}*/
		}
		ImGui::End();

		// Focus select windows on first run
		if (m_FirstRun)
		{
			//ImGui::SetWindowFocus("Prefab");
			ImGui::SetWindowFocus("Scene");
			ImGui::SetWindowFocus("Inspector");
			m_FirstRun = false;
		}
	}

	void ImguiHelper::CleanUpImgui()
	{
		ImGui_ImplOpenGL3_Shutdown();
		ImGui_ImplGlfw_Shutdown();
		ImGui::DestroyContext();
	}

	void ImguiHelper::SceneWindow()
	{
		// Debug info
		ImGui::PushStyleColor(0, ImVec4{ 0.55f,0.8f,0.2f,1 });
		ImGui::Text("Application average %.3f ms/frame (%.1f FPS)", 1000.0f / ImGui::GetIO().Framerate, ImGui::GetIO().Framerate);
		ImGui::PopStyleColor();

		/// Unused, temp debugger, have to restructure for extra draw calls just so this will work properly
		//ImGui::PushItemWidth(200.0f);
		//const std::vector<std::string> comboStr = { "EDITOR", "POSITION", "NORMAL", "ALBEDO", "AO" };
		//if (ImGui::BeginCombo("###debugcombo", comboStr[PP::Renderer::m_DebugRenderMode].c_str()))
		//{
		//	for (int i = 0; i < PP::Renderer::RenderMode::RENDER_MODE_COUNT; i++)
		//	{
		//		bool selected = (PP::Renderer::m_DebugRenderMode == i);
		//		if (ImGui::Selectable(comboStr[i].c_str()))
		//		{
		//			PP::Renderer::m_DebugRenderMode = static_cast<PP::Renderer::RenderMode>(i);
		//		}

		//		if (selected)
		//		{
		//			ImGui::SetItemDefaultFocus();
		//		}

		//	}
		//	ImGui::EndCombo();
		//}
		//ImGui::PopItemWidth();

		// Draw the actual editor scene
		ImGui::Image(PP::FBR::m_FrameBuffers[PP::BufferType::EDITOR_COLOR_BUFFER], ImGui::GetContentRegionAvail(), ImVec2(0, 1), ImVec2(1, 0));

		// Config the draw zone for guizmo
		ImVec2 vMin = ImGui::GetWindowContentRegionMin();
		ImVec2 vMax = ImGui::GetWindowContentRegionMax();

		// Aspect ratio update
		Pogplant::Camera4D* currQuatCam = PP::CameraResource::GetCamera("EDITOR");
		//currCam->UpdateProjection({ vMax.x, vMax.y });
		//PP::Camera4D& currQuatCam = PP::CameraResource::m_QuatCam;
		//currQuatCam->UpdateProjection({ vMax.x, vMax.y });
		//currQuatCam->UpdateOrthographic({ vMax.x, vMax.y }, currQuatCam->m_Far, currQuatCam->m_Orthographic);

		// Account for position of window
		vMin.x += ImGui::GetWindowPos().x;
		vMin.y += ImGui::GetWindowPos().y + 20; // + 20 to account for the text line kekw
		vMax.x += ImGui::GetWindowPos().x;
		vMax.y += ImGui::GetWindowPos().y;

		if (!ImGui::IsWindowFocused())
		{
			return;
		}

		/// GUIZMO GO EDIT
		Scene_GOEdit(currQuatCam, vMin, vMax);

		/// Picker
		Scene_GOPick(currQuatCam, vMin, vMax);

		/// GUIZMO VIEW EDIT
		Scene_ViewEdit(currQuatCam, vMin, vMax);
	}

	void ImguiHelper::GameWindow()
	{
		ImGui::PushStyleColor(0, ImVec4{ 0.55f,0.8f,0.2f,1 });
		ImGui::Text("Application average %.3f ms/frame (%.1f FPS)", 1000.0f / ImGui::GetIO().Framerate, ImGui::GetIO().Framerate);
		ImGui::PopStyleColor();

		ImGui::SameLine(ImGui::GetWindowWidth() - 208.0f);
		// Unused, temp debugger, have to restructure for extra draw calls just so this will work properly
		ImGui::PushItemWidth(200.0f);
		const std::vector<std::string> comboStr = { "GAME CAMERA", "EDITOR CAMERA"};
		if (ImGui::BeginCombo("###debugcombo", comboStr[PP::Renderer::m_EditorCamDebug].c_str()))
		{
			for (int i = 0; i < comboStr.size(); i++)
			{
				bool selected = (PP::Renderer::m_EditorCamDebug == static_cast<bool>(i));
				if (ImGui::Selectable(comboStr[i].c_str()))
				{
					PP::Renderer::m_EditorCamDebug = static_cast<bool>(i);
				}

				if (selected)
				{
					ImGui::SetItemDefaultFocus();
				}

			}
			ImGui::EndCombo();
		}
		ImGui::PopItemWidth();

		ImGui::Image(PP::FBR::m_FrameBuffers[PP::BufferType::GAME_COLOR_BUFFER], ImGui::GetContentRegionAvail(), ImVec2(0, 1), ImVec2(1, 0));
		ImVec2 vMax = ImGui::GetWindowContentRegionMax();

		if (!ImGui::IsWindowFocused())
		{
			return;
		}

		if (!ImGui::IsItemHovered())
		{
			PP::CameraResource::DeselectCam();
		}
		else if(PP::Renderer::m_EditorCamDebug)
		{
			PP::CameraResource::SetActiveCam("EDITOR");
		}
	}

	void ImguiHelper::SaveSceneAs()
	{
		std::string filepath = Pogplant::FileDialogs::SaveFile("Json Files(*.json)\0*.json\0");
		//Append .json 
		if (filepath.find(".json") == std::string::npos)
		{
			filepath.append(".json");
		}
		if (!filepath.empty())
		{
			Serializer serialiser{ (*m_ecs) };
			serialiser.Save(filepath);
		}
	}

	void ImguiHelper::NewScene()
	{
		m_CurrentEntity = entt::null;
		m_ecs->Clear();

		/// Reupdate the textures
		PP::TextureResource::m_Updated = false;
	}

	void ImguiHelper::OpenScene()
	{
		std::string filepath = Pogplant::FileDialogs::OpenFile("Json Files(*.json)\0*.json\0");
		if (!filepath.empty())
		{
			OpenScene(filepath);
			Application::GetInstance().SetSceneNameFromFilePath(filepath);
		}

		/// Reupdate the textures
		PP::TextureResource::m_Updated = false;
	}


	bool PogplantDriver::ImguiHelper::ImGui_BeginMainStatusBar()
	{
		ImGuiContext& g = *GImGui;
		ImGuiViewportP* viewport = g.Viewports[0];
		ImGuiWindow* menu_bar_window = ImGui::FindWindowByName("##MainStatusBar"); // CHANGED HERE

		// For the main menu bar, which cannot be moved, we honor g.Style.DisplaySafeAreaPadding to ensure text can be visible on a TV set.
		g.NextWindowData.MenuBarOffsetMinVal = ImVec2(g.Style.DisplaySafeAreaPadding.x, ImMax(g.Style.DisplaySafeAreaPadding.y - g.Style.FramePadding.y, 0.0f));

		// Get our rectangle at the top of the work area
		//__debugbreak();
		if (menu_bar_window == NULL || menu_bar_window->BeginCount == 0)
		{
			// Set window position
			// We don't attempt to calculate our height ahead, as it depends on the per-viewport font size. However menu-bar will affect the minimum window size so we'll get the right height.
			ImVec2 menu_bar_pos = viewport->Pos + viewport->CurrWorkOffsetMin;
			ImVec2 menu_bar_size = ImVec2(viewport->Size.x - viewport->CurrWorkOffsetMin.x + viewport->CurrWorkOffsetMax.x, 1.0f);
			ImGui::SetNextWindowPos(menu_bar_pos);
			ImGui::SetNextWindowSize(menu_bar_size);
		}

		// Create window
		ImGui::SetNextWindowViewport(viewport->ID); // Enforce viewport so we don't create our own viewport when ImGuiConfigFlags_ViewportsNoMerge is set.
		ImGui::PushStyleVar(ImGuiStyleVar_WindowRounding, 0.0f);
		ImGui::PushStyleVar(ImGuiStyleVar_WindowMinSize, ImVec2(0, 0));    // Lift normal size constraint, however the presence of a menu-bar will give us the minimum height we want.
		ImGuiWindowFlags window_flags = ImGuiWindowFlags_NoDocking | ImGuiWindowFlags_NoTitleBar | ImGuiWindowFlags_NoResize | ImGuiWindowFlags_NoMove | ImGuiWindowFlags_NoScrollbar | ImGuiWindowFlags_NoSavedSettings | ImGuiWindowFlags_MenuBar;
		bool is_open = ImGui::Begin("##MainStatusBar", NULL, window_flags) && ImGui::BeginMenuBar();
		ImGui::PopStyleVar(2);

		// Report our size into work area (for next frame) using actual window size
		menu_bar_window = ImGui::GetCurrentWindow();
		if (menu_bar_window->BeginCount == 1)
			viewport->CurrWorkOffsetMin.y += menu_bar_window->Size.y;

		g.NextWindowData.MenuBarOffsetMinVal = ImVec2(0.0f, 0.0f);
		if (!is_open)
		{
			ImGui::End();
			return false;
		}
		return true; //-V1020
	}

	void PogplantDriver::ImguiHelper::ImGui_EndMainStatusBar()
	{
		ImGui::EndMenuBar();

		// When the user has left the menu layer (typically: closed menus through activation of an item), we restore focus to the previous window
		// FIXME: With this strategy we won't be able to restore a NULL focus.
		ImGuiContext& g = *GImGui;
		if (g.CurrentWindow == g.NavWindow && g.NavLayer == ImGuiNavLayer_Main && !g.NavAnyRequest)
			ImGui::FocusTopMostWindowUnderOne(g.NavWindow, NULL);

		ImGui::End();
	}

	void ImguiHelper::Scene_GOPick(Pogplant::Camera4D* _CurrCam, ImVec2 _VMin, ImVec2 _VMax)
	{
		if (!ImGui::IsWindowFocused() || !ImGui::IsItemHovered())
			return;

		if (ImGui::IsMouseClicked(ImGuiMouseButton_Left) && !m_GoEditing)
		{
			glm::vec2 min = { _VMin.x,_VMin.y };
			glm::vec2 max = { _VMax.x,_VMax.y };
			glm::vec2 cursor = { ImGui::GetMousePos().x,ImGui::GetMousePos().y };

			_CurrCam->UpdateRayConfig({ min,max,max - min,cursor });
			_CurrCam->RayCast();

			// Ray for picking
			PhysicsDLC::Collision::Shapes::Ray ray{ _CurrCam->m_Ray.m_Origin, _CurrCam->m_Ray.m_Direction };
			float shortestTime = std::numeric_limits<float>::max();
			entt::entity chosenObject = entt::null;

			m_ecs->GetReg().each([&ray, &shortestTime, &chosenObject](auto entity)
			{
				//GameObject& currGO = GO_Resource::m_GO_Container[i];
					
				auto& transform = m_ecs->GetReg().get<Components::Transform>(entity);
				auto renderer = m_ecs->GetReg().try_get<Components::Renderer>(entity);
				auto boxCollider = m_ecs->GetReg().try_get<Components::BoxCollider>(entity);
				auto sphereCollider = m_ecs->GetReg().try_get<Components::SphereCollider>(entity);
				//auto debugRenderer = m_ecs->GetReg().try_get<Components::DebugRender>(entity);

				if (boxCollider != nullptr)
				{
					float currentTime = std::numeric_limits<float>::max();

					if (PhysicsDLC::Collision::RayAABB(ray, boxCollider->aabb, currentTime))
					{
						if (currentTime < shortestTime)
						{
							chosenObject = entity;
							shortestTime = currentTime;
						}
					}
				}
				else if (sphereCollider != nullptr)
				{
					float currentTime = std::numeric_limits<float>::max();

					if (PhysicsDLC::Collision::RaySphere(ray, sphereCollider->sphere, currentTime))
					{
						if (currentTime < shortestTime)
						{
							chosenObject = entity;
							shortestTime = currentTime;
						}
					}
				}
				else if (renderer != nullptr)
				{
					// Naive approach
					float largestScale = std::numeric_limits<float>::min();

					for (int j = 0; j < 3; j++)
					{
						largestScale = std::max(largestScale, transform.m_scale[j]);
					}

					if (renderer->m_RenderModel)
					{
						const float radius = renderer->m_RenderModel->m_Bounds.longest * 0.5f * largestScale;
						float currentTime = std::numeric_limits<float>::max();

						//Largest sphere approach
						if (PhysicsDLC::Collision::RaySphere(ray, PhysicsDLC::Collision::Shapes::Sphere{ transform.m_position, radius }, currentTime))
						{
							if (currentTime < shortestTime)
							{
								chosenObject = entity;
								shortestTime = currentTime;
							}
						}

						//aabb approach (not good for rotation)
						/*PhysicsDLC::Collision::Shapes::AABB aabb{
							{ renderer->m_RenderModel->m_Bounds.minX,
							renderer->m_RenderModel->m_Bounds.minY,
							renderer->m_RenderModel->m_Bounds.minZ },
							{ renderer->m_RenderModel->m_Bounds.maxX,
							renderer->m_RenderModel->m_Bounds.maxY,
							renderer->m_RenderModel->m_Bounds.maxZ } };

						aabb.m_min = aabb.m_min * transform.m_scale + transform.m_position;
						aabb.m_max = aabb.m_max * transform.m_scale + transform.m_position;

						if (PhysicsDLC::Collision::RayAABB(ray, aabb, currentTime))
						{
							if (currentTime < shortestTime)
							{
								chosenObject = entity;
								shortestTime = currentTime;
							}
						}//*/
					}
				}
			});

			// Update object picked
			if (chosenObject != entt::null)
			{
				m_CurrentEntity = chosenObject;
			}
			else
			{
				m_CurrentEntity = entt::null;
			}
		}
	}

	glm::mat4 ImguiHelper::get_parent_transform(entt::entity _id)
	{
		auto& transform = m_ecs->GetReg().get<Components::Transform>(_id);
		return transform.m_ModelMtx;
	}


	void ImguiHelper::Scene_GOEdit(Pogplant::Camera4D* _CurrCam, ImVec2 _VMin, ImVec2 _VMax)
	{
		// Draw view manipulate only in editor scene
		ImGuizmo::SetDrawlist();

		// Bounds for guizmo
		ImGuizmo::SetRect(_VMin.x, _VMin.y, _VMax.x, _VMax.y);

		if (m_CurrentEntity != entt::null)
		{
			//GameObject& currGO = GO_Resource::m_GO_Container[m_CurrentGOIdx];

			auto& transform = m_ecs->GetReg().get<Components::Transform>(m_CurrentEntity);

			//glm::mat4& pos = transform.m_ModelMtx;
			glm::mat4 parent_matrix;

			glm::mat4 xxTTVMatrix69x;//A temporary matrix used for child/parent transformation
			bool hasParent = false;

			if (transform.m_parent != entt::null)
			{
				hasParent = true;
				parent_matrix = get_parent_transform(transform.m_parent);
			}

			ImGuizmo::RecomposeMatrixFromComponents(
				glm::value_ptr(transform.m_position),
				glm::value_ptr(transform.m_rotation),
				glm::value_ptr(transform.m_scale),
				glm::value_ptr(xxTTVMatrix69x));

			//At this point, xxTTVMatrix69x is a matrix that is purely local space

			if (hasParent)
			{
				//This computes the local to world matrix
				xxTTVMatrix69x = parent_matrix * xxTTVMatrix69x;
			}//*/

			ImGuizmo::Manipulate
			(
				m_GoEditing,
				glm::value_ptr(_CurrCam->GetView()),
				glm::value_ptr(_CurrCam->m_Projection),
				m_EditMode,
				ImGuizmo::LOCAL,
				glm::value_ptr(xxTTVMatrix69x),
				NULL,
				m_UseSnap ? m_SnapStep : NULL,
				m_BoundSizing ? m_BoundsPos : NULL,
				m_UseBoundsSnap ? m_BoundsSnapStep : NULL
			);

			if (hasParent)
			{
				//This translates from local relative to world, to local relative to parent
				xxTTVMatrix69x = inverse(parent_matrix) * xxTTVMatrix69x;
			}//*/

			ImGuizmo::DecomposeMatrixToComponents(glm::value_ptr(xxTTVMatrix69x),
				glm::value_ptr(transform.m_position),
				glm::value_ptr(transform.m_rotation),
				glm::value_ptr(transform.m_scale));
		}
	}

	void ImguiHelper::Scene_ViewEdit(Pogplant::Camera4D* _CurrCam, ImVec2 _VMin, ImVec2 _VMax)
	{
		float view[16] = { 0 };
		float projection[16] = { 0 };
		float front[3] = { 0 };
		const glm::mat4& cView = _CurrCam->GetView();
		memcpy(view, glm::value_ptr(cView), sizeof(cView));
		memcpy(projection, glm::value_ptr(_CurrCam->m_Projection), sizeof(_CurrCam->m_Projection));
		// After clickng on gizmo update yaw pitch accordingly
		if (ImGuizmo::ViewManipulate(view, 1.0f, ImVec2(_VMax.x - 128, _VMin.y), ImVec2(128, 128), 0x0, front))
		{
			_CurrCam->UpdateFront(front);
		}

		//// Updated view from gizmo
		//_CurrCam->View() = glm::make_mat4(view);

		// Make sure begin is being called before this function
		// This ensures the input for camera only works when the Scene window is focused
		if (!ImGui::IsItemHovered())
		{
			PP::CameraResource::DeselectCam();
		}
		else
		{
			PP::CameraResource::SetActiveCam("EDITOR");
		}
	}

	void ImguiHelper::OpenScene(const std::filesystem::path& path)
	{
		//if (path.extension().string() != ".json")
		//{
		//	//ASSET HERE
		//	//("Could not load {0} - not a scene file", path.filename().string());
		//	return;
		//}
		NewScene();
		Serializer serialiser{ (*m_ecs) };
		if(!serialiser.Load(path.string()))
		{
			//ASSET HERE
			//Something failed to load?
		}
	}

	std::string ImguiHelper::CreateStringInputField(const std::string& _label, std::string _target)
	{
		ImGui::Text(_label.c_str());
		static char name_stuff[256] = "";
		sprintf_s(name_stuff, IM_ARRAYSIZE(name_stuff), _target.c_str());

		std::string aaa{ "###" };
		aaa.append(_label);

		ImGui::InputText(aaa.c_str(), name_stuff, IM_ARRAYSIZE(name_stuff));

		return std::string{ name_stuff };
	}

	void ImguiHelper::CreateDragFloat3(const std::string& _label, float* _value, float increment_speed, float min_val, float max_val)
	{
		ImGui::Text(_label.c_str());
		ImGui::PushID(_label.c_str());
		ImGui::DragFloat3("", _value, increment_speed, min_val, max_val);
		ImGui::PopID();
	}

	void ImguiHelper::Reflect_ImGui(rttr::instance _obj)
	{
		rttr::instance obj = _obj.get_type().get_raw_type().is_wrapper() ? _obj.get_wrapped_instance() : _obj;
		const auto component_name = obj.get_type().get_raw_type().get_name().to_string();

		auto prop_list = obj.get_derived_type().get_properties();

		for (auto& prop : prop_list)
		{
			rttr::variant prop_value = prop.get_value(obj);

			if (!prop_value)
				continue; // cannot serialize, because we cannot retrieve the value

			auto name = prop.get_name().to_string();

			if (prop_value.is_type<std::string>())
			{
				std::string _str = CreateStringInputField(name, prop_value.to_string());
				prop.set_value(obj, _str);
			}
			else if (prop_value.is_type<glm::vec3>()) 
			{
				glm::vec3& test = prop_value.get_value<glm::vec3>();
				CreateDragFloat3(name, glm::value_ptr(test));
				prop.set_value(obj, test);
			}
		}


		ImguiBlankSeperator(1);
		ImGui::Separator();
	}

	void ImguiHelper::ToolTipHelper(const char* _Label, bool _SameLine)
	{
		if (_SameLine)
		{
			ImGui::SameLine();
		}

		ImGui::TextDisabled("(?)");
		if (ImGui::IsItemHovered())
		{
			ImGui::BeginTooltip();
			ImGui::PushTextWrapPos(ImGui::GetFontSize() * 35.0f);
			ImGui::TextUnformatted(_Label);
			ImGui::PopTextWrapPos();
			ImGui::EndTooltip();
		}
	}

	void ImguiHelper::TextureSelectHelper(const char* _Label, Pogplant::Texture* _CurrTex)
	{
		// Always center this window when appearing
		ImVec2 center = ImGui::GetMainViewport()->GetCenter();
		ImGui::SetNextWindowSizeConstraints({ 1,1 }, { 1000,900 });
		ImGui::SetNextWindowPos(center, ImGuiCond_Appearing, ImVec2(0.5f, 0.5f));
		static std::string currentPath = "";
		static bool hovered = false;

		/// Texture select pop up
		if (ImGui::BeginPopupModal(_Label, NULL, ImGuiWindowFlags_AlwaysAutoResize))
		{
			assert(_CurrTex != nullptr);
			ImGui::Text(currentPath.c_str());
			ImGui::Dummy(ImVec2(0.0f, 1.0f));

			// Clear hover flag after printing to detect hover stop
			hovered = false;
			currentPath = "";

			size_t it_count = 0;
			for (const auto& it : PP::TextureResource::m_TexturePool)
			{
				if (ImGui::ImageButton(it.second, ImVec2(64, 64), { 0,0 }, { 1,1 }, -1, ImVec4{ 0,0,0,0 }, ImVec4{ 1,1,1,1 }))
				{
					_CurrTex->m_Id = it.second;
					_CurrTex->m_Path = it.first;
					ImGui::CloseCurrentPopup();
				}
				if (ImGui::IsItemHovered())
				{
					currentPath = it.first;
					hovered = true;
				}

				// Formatting
				if ((it_count + 1) % 10 != 0)
				{
					ImGui::SameLine();
				}

				it_count++;
			}

			// So wont be on the same line as prev
			ImGui::Dummy(ImVec2(0.0f, 0.0f));
			// Center object
			ImGui::SetCursorPosX(ImGui::GetWindowContentRegionMax().x * 0.5f - 56.0f);
			if (ImGui::Button("Cancel", ImVec2(120, 0)))
			{
				ImGui::CloseCurrentPopup();
			}
			ImGui::SetItemDefaultFocus();
			ImGui::SameLine();
			ImGui::EndPopup();
		}
	}

	void ImguiHelper::TextureSelectHelper(const char* _Label, std::string* _Path)
	{
		// Always center this window when appearing
		ImVec2 center = ImGui::GetMainViewport()->GetCenter();
		ImGui::SetNextWindowSizeConstraints({ 1,1 }, { 1000,900 });
		ImGui::SetNextWindowPos(center, ImGuiCond_Appearing, ImVec2(0.5f, 0.5f));
		static std::string currentPath = "";
		static bool hovered = false;

		/// Texture select pop up
		if (ImGui::BeginPopupModal(_Label, NULL, ImGuiWindowFlags_AlwaysAutoResize))
		{
			assert(_Path != nullptr);
			ImGui::Text(currentPath.c_str());
			ImGui::Dummy(ImVec2(0.0f, 1.0f));

			// Clear hover flag after printing to detect hover stop
			hovered = false;
			currentPath = "";
			
			size_t it_count = 0;
			for (const auto& it : PP::TextureResource::m_TexturePool)
			{
				if (ImGui::ImageButton(it.second, ImVec2(64, 64), { 0,0 }, { 1,1 }, -1, ImVec4{ 0,0,0,0 }, ImVec4{ 1,1,1,1 }))
				{
					*_Path = it.first;
					ImGui::CloseCurrentPopup();
				}
				if (ImGui::IsItemHovered())
				{
					currentPath = it.first;
					hovered = true;
				}

				// Formatting
				if ((it_count + 1) % 10 != 0)
				{
					ImGui::SameLine();
				}

				it_count++;
			}

			// So wont be on the same line as prev
			ImGui::Dummy(ImVec2(0.0f, 0.0f));
			// Center object
			ImGui::SetCursorPosX(ImGui::GetWindowContentRegionMax().x * 0.5f - 56.0f);
			if (ImGui::Button("Cancel", ImVec2(120, 0)))
			{
				ImGui::CloseCurrentPopup();
			}
			ImGui::SetItemDefaultFocus();
			ImGui::SameLine();
			ImGui::EndPopup();
		}
	}

	void ImguiHelper::TextureSelectHelper(const char* _Label, std::string& _Path, int& _TexID)
	{
		// Always center this window when appearing
		ImVec2 center = ImGui::GetMainViewport()->GetCenter();
		ImGui::SetNextWindowSizeConstraints({ 1,1 }, { 1000,900 });
		ImGui::SetNextWindowPos(center, ImGuiCond_Appearing, ImVec2(0.5f, 0.5f));
		static std::string currentPath = "";
		static bool hovered = false;

		/// Texture select pop up
		if (ImGui::BeginPopupModal(_Label, NULL, ImGuiWindowFlags_AlwaysAutoResize))
		{
			ImGui::Text(currentPath.c_str());
			ImGui::Dummy(ImVec2(0.0f, 1.0f));

			// Clear hover flag after printing to detect hover stop
			hovered = false;
			currentPath = "";

			size_t it_count = 0;
			for (const auto& it : PP::TextureResource::m_TexturePool)
			{
				if (ImGui::ImageButton(it.second, ImVec2(64, 64), { 0,0 }, { 1,1 }, -1, ImVec4{ 0,0,0,0 }, ImVec4{ 1,1,1,1 }))
				{
					_Path = it.first;
					//_TexID = PP::TextureResource::m_UsedTextures[it.second];
					PP::TextureResource::m_Updated = false;
					ImGui::CloseCurrentPopup();
				}
				if (ImGui::IsItemHovered())
				{
					currentPath = it.first;
					hovered = true;
				}

				// Formatting
				if ((it_count + 1) % 10 != 0)
				{
					ImGui::SameLine();
				}

				it_count++;
			}

			// So wont be on the same line as prev
			ImGui::Dummy(ImVec2(0.0f, 0.0f));
			// Center object
			ImGui::SetCursorPosX(ImGui::GetWindowContentRegionMax().x * 0.5f - 56.0f);
			if (ImGui::Button("Cancel", ImVec2(120, 0)))
			{
				ImGui::CloseCurrentPopup();
			}
			ImGui::SetItemDefaultFocus();
			ImGui::SameLine();
			ImGui::EndPopup();
		}
	}

	void ImguiHelper::RendererComponentHelper()
	{
		assert(m_CurrentEntity != entt::null);
		auto renderer = m_ecs->GetReg().try_get<Components::Renderer>(m_CurrentEntity);
		if (renderer)
		{
			bool enable_render = true;

			if (ImGui::CollapsingHeader(ICON_FA_TH "  Renderer", &enable_render, ImGuiTreeNodeFlags_DefaultOpen))
			{
				// Model
				ImGuiComboFlags flag = 0;
				flag |= ImGuiComboFlags_PopupAlignLeft;
				auto model_itr = PP::ModelResource::m_ModelPool.begin();
				for (auto it = PP::ModelResource::m_ModelPool.begin(); it != PP::ModelResource::m_ModelPool.end(); ++it)
				{
					if (it->second == renderer->m_RenderModel)
						model_itr = it;
				}

				ImGui::Text("Model");
				if (ImGui::BeginCombo("###Mdl", model_itr->first.c_str(), flag))
				{
					for (auto it = PP::ModelResource::m_ModelPool.begin(); it != PP::ModelResource::m_ModelPool.end(); ++it)
					{
						const bool is_selected = (model_itr == it);
						if (ImGui::Selectable(it->first.c_str(), is_selected))
						{
							model_itr = it;
							renderer->m_RenderModel = model_itr->second;
							// Update mesh as well
							renderer->m_Mesh = &model_itr->second->m_Meshes.begin()->second;
						}

						if (is_selected)
							ImGui::SetItemDefaultFocus();

					}
					ImGui::EndCombo();
				}

				// Mesh
				std::string select = "NULL";
				if (renderer->m_Mesh != nullptr)
				{
					select = renderer->m_Mesh->m_Name;
				}
				ImGui::Text("Mesh");
				if (ImGui::BeginCombo("###Msh", select.c_str(), flag))
				{
					for (auto it = model_itr->second->m_Meshes.begin(); it != model_itr->second->m_Meshes.end(); ++it)
					{
						const bool is_selected = (select == it->first.c_str());
						if (ImGui::Selectable(it->first.c_str(), is_selected))
						{
							select = it->first;
							renderer->m_Mesh = &model_itr->second->m_Meshes[select];
						}

						if (is_selected)
							ImGui::SetItemDefaultFocus();

					}
					ImGui::EndCombo();
				}

				// Textures
				ImGui::NewLine();
				if (ImGui::TreeNode("Textures"))
				{
					static PP::Texture* selectedTex = nullptr;
					const char* popuplabel = "Texture Selection";

					// Diffuse
					if (ImGui::TreeNode("Diffuse"))
					{
						// Diffuse color picker
						ImGui::Dummy(ImVec2(0.0f, 2.0f));
						ImGui::PushID("REND");
						ImGui::Text("Color Tint");
						ImGui::ColorEdit3("###Diffuse Color", glm::value_ptr(renderer->m_ColorTint));
						ImGui::PopID();
						ImGui::Dummy(ImVec2(0.0f, 1.0f));

						for (size_t i = 0; i < renderer->m_Mesh->m_Textures.size(); i++)
						{
							PP::Texture& currTexture = renderer->m_Mesh->m_Textures[i];
							if (currTexture.m_Type != "texture_diffuse")
							{
								continue;
							}
							ImGui::Text(currTexture.m_Path.c_str());
							ImGui::SameLine();
							ImGui::Text(std::to_string(currTexture.m_Id).c_str());

							// Texture picker
							if (ImGui::ImageButton(currTexture.m_Id, ImVec2(32, 32), { 0,0 }, { 1,1 }, -1, ImVec4{ 0,0,0,0 }, ImVec4{ 1,1,1,1 }))
							{
								selectedTex = &renderer->m_Mesh->m_Textures[i];
								ImGui::OpenPopup(popuplabel);
							}
							ImGui::Dummy(ImVec2(0.0f, 0.5f));
						}
						TextureSelectHelper(popuplabel, selectedTex);
						ImGui::Dummy(ImVec2(0.0f, 1.0f));

						if (ImGui::Button("Add Debug Texture"))
						{
							AddDebugTexture(renderer, "texture_diffuse");
						}
						ToolTipHelper("This is just to check UV's see if texture would have worked, setting this is a local change and it will NOT be saved", true);

						ImGui::TreePop();
						ImGui::Dummy(ImVec2(0.0f, 1.0f));
					}

					// Spec
					if (ImGui::TreeNode("Specular"))
					{
						ImGui::Dummy(ImVec2(0.0f, 2.0f));
						for (size_t i = 0; i < renderer->m_Mesh->m_Textures.size(); i++)
						{
							PP::Texture& currTexture = renderer->m_Mesh->m_Textures[i];
							if (currTexture.m_Type != "texture_specular")
							{
								continue;
							}
							ImGui::Text(currTexture.m_Path.c_str());
							ImGui::SameLine();
							ImGui::Text(std::to_string(currTexture.m_Id).c_str());
							// Texture picker
							if (ImGui::ImageButton(currTexture.m_Id, ImVec2(32, 32), { 0,0 }, { 1,1 }, -1, ImVec4{ 0,0,0,0 }, ImVec4{ 1,1,1,1 }))
							{
								selectedTex = &renderer->m_Mesh->m_Textures[i];
								ImGui::OpenPopup(popuplabel);
							}
							ImGui::Dummy(ImVec2(0.0f, 0.5f));
						}
						TextureSelectHelper(popuplabel, selectedTex);
						if (ImGui::Button("Add Debug Texture"))
						{
							AddDebugTexture(renderer, "texture_specular");
						}
						ToolTipHelper("This is just to check UV's see if texture would have worked, setting this is a local change and it will NOT be saved", true);

						ImGui::TreePop();
						ImGui::Dummy(ImVec2(0.0f, 1.0f));
					}

					// Normal
					if (ImGui::TreeNode("Normal"))
					{
						ImGui::Dummy(ImVec2(0.0f, 2.0f));
						for (size_t i = 0; i < renderer->m_Mesh->m_Textures.size(); i++)
						{
							PP::Texture& currTexture = renderer->m_Mesh->m_Textures[i];
							if (currTexture.m_Type != "texture_normal")
							{
								continue;
							}
							ImGui::Text(currTexture.m_Path.c_str());
							ImGui::SameLine();
							ImGui::Text(std::to_string(currTexture.m_Id).c_str());
							// Texture picker
							if (ImGui::ImageButton(currTexture.m_Id, ImVec2(32, 32), { 0,0 }, { 1,1 }, -1, ImVec4{ 0,0,0,0 }, ImVec4{ 1,1,1,1 }))
							{
								selectedTex = &renderer->m_Mesh->m_Textures[i];
								ImGui::OpenPopup(popuplabel);
							}
							ImGui::Dummy(ImVec2(0.0f, 0.5f));
						}
						TextureSelectHelper(popuplabel, selectedTex);
						if (ImGui::Button("Add Debug Texture"))
						{
							AddDebugTexture(renderer, "texture_normal");
						}
						ToolTipHelper("This is just to check UV's see if texture would have worked, setting this is a local change and it will NOT be saved", true);

						ImGui::TreePop();
						ImGui::Dummy(ImVec2(0.0f, 1.0f));
					}

					// Emi
					if (ImGui::TreeNode("Emissive"))
					{
						// Emission color picker
						ImGui::Dummy(ImVec2(0.0f, 2.0f));
						ImGui::PushID("REND");
						ImGui::Text("Emission Tint");
						ImGui::ColorEdit3("###Emi Color", glm::value_ptr(renderer->m_EmissiveTint));
						ImGui::PopID();
						ImGui::Dummy(ImVec2(0.0f, 1.0f));

						for (size_t i = 0; i < renderer->m_Mesh->m_Textures.size(); i++)
						{
							PP::Texture& currTexture = renderer->m_Mesh->m_Textures[i];
							if (currTexture.m_Type != "texture_emissive")
							{
								continue;
							}
							ImGui::Text(currTexture.m_Path.c_str());
							ImGui::SameLine();
							ImGui::Text(std::to_string(currTexture.m_Id).c_str());

							// Texture picker
							if (ImGui::ImageButton(currTexture.m_Id, ImVec2(32, 32), { 0,0 }, { 1,1 }, -1, ImVec4{ 0,0,0,0 }, ImVec4{ 1,1,1,1 }))
							{
								selectedTex = &renderer->m_Mesh->m_Textures[i];
								ImGui::OpenPopup(popuplabel);
							}
							ImGui::Dummy(ImVec2(0.0f, 0.5f));
						}
						TextureSelectHelper(popuplabel, selectedTex);
						if (ImGui::Button("Add Debug Texture"))
						{
							AddDebugTexture(renderer, "texture_emissive");
						}
						ToolTipHelper("This is just to check UV's see if texture would have worked, setting this is a local change and it will NOT be saved", true);

						ImGui::TreePop();
					}
					ImGui::TreePop();
				}
				ImGui::NewLine();

				bool temp_light = renderer->m_UseLight;
				ImGui::Checkbox("Use Light", &temp_light);
				if (temp_light != static_cast<bool>(renderer->m_UseLight))
					renderer->m_UseLight = temp_light;

				ImGui::Dummy(ImVec2(0.0f, 1.0f));

				bool temp_renderMode = renderer->m_EditorDrawOnly;
				ImGui::Checkbox("Editor Draw Only", &temp_renderMode);
				if (temp_renderMode != static_cast<bool>(renderer->m_EditorDrawOnly))
					renderer->m_EditorDrawOnly = temp_renderMode;

				ImguiBlankSeperator(1);
				ImGui::Separator();
			}

			if (!enable_render)
			{
				m_ecs->GetReg().remove<Components::Renderer>(m_CurrentEntity);
			}
		}
	}

	void ImguiHelper::AddDebugTexture(Components::Renderer* _CRenderer, std::string _Type)
	{
		// Only 1 of this type
		for (auto it : _CRenderer->m_Mesh->m_Textures)
		{
			if (it.m_Type == _Type)
			{
				return;
			}
		}

		// This is only for adding textures to model manually for debugging, it will not be saved 
		_CRenderer->m_Mesh->m_Textures.push_back(PP::Texture{ 0,_Type, "NULL"});
	}

	void ImguiHelper::SpriteAnimationHelper()
	{
		ImGui::Dummy(ImVec2(0.0f, 5.0f));
		if (ImGui::TreeNode("Sprite Animation"))
		{
			auto spriteAnim = &m_ecs->GetReg().try_get<Components::Canvas>(m_CurrentEntity)->m_SpriteAnimation;

			ImGui::Dummy(ImVec2(0.0f, 2.0f));
			ImGui::Text("Current Frame: %d", static_cast<int>(spriteAnim->m_FrameCounter));
			ImGui::Text("UV: %f, %f", spriteAnim->m_UV_Offset.x, spriteAnim->m_UV_Offset.y);
			ImGui::Text("Tiling: %f, %f", spriteAnim->m_Tiling.x, spriteAnim->m_Tiling.y);

			ImGui::Dummy(ImVec2(0.0f, 2.0f));

			// Buttons
			if (spriteAnim->m_Play)
			{
				if (spriteAnim->m_Pause)
				{
					if (ImGui::Button("Resume"))
					{
						spriteAnim->m_Pause = false;
					}
				}
				else
				{
					if (ImGui::Button("Pause"))
					{
						spriteAnim->m_Pause = true;
					}
				}
			}
			else
			{
				if (ImGui::Button("Play"))
				{
					spriteAnim->m_Play = true;
				}
			}
			ImGui::SameLine();
			if (ImGui::Button("Stop"))
			{
				spriteAnim->m_Play = false;
				spriteAnim->m_Pause = false;
			}
			ImGui::Dummy(ImVec2(0.0f, 2.0f));

			ImGui::Text("Frame Controls");
			if (ImGui::Button("Prev Frame"))
			{
				spriteAnim->PrevFrame();
			}
			ImGui::SameLine();
			if (ImGui::Button("Next Frame"))
			{
				spriteAnim->NextFrame();
			}

			ImGui::Dummy(ImVec2(0.0f, 2.0f));
			ImGui::Text("Set Frame");
			int frameSetter = static_cast<int>(spriteAnim->m_FrameCounter);
			if (ImGui::InputInt("###SetFrame", &frameSetter))
			{
				spriteAnim->SetFrame(frameSetter);
			}

			ImGui::Dummy(ImVec2(0.0f, 8.0f));
			ImGui::Text("Repeat");
			ImGui::SameLine();
			ImGui::Checkbox("###SARepeat", &spriteAnim->m_Repeat);
			ImGui::Dummy(ImVec2(0.0f, 4.0f));

			// Config
			ImGui::Text("Rows");
			if (ImGui::DragInt("###Rows", &spriteAnim->m_Rows, 1.0f, 0, 100))
			{
				spriteAnim->UpdateTiling();
			}
			ImGui::Dummy(ImVec2(0.0f, 1.0f));

			ImGui::Text("Columns");
			if (ImGui::DragInt("###Columns", &spriteAnim->m_Columns, 1.0f, 0, 100))
			{
				spriteAnim->UpdateTiling();
			}
			ImGui::Dummy(ImVec2(0.0f, 1.0f));

			ImGui::Text("Max Frames");
			ImGui::DragInt("###MaxFrames", &spriteAnim->m_MaxFrames, 1.0f, 0, spriteAnim->m_Rows * spriteAnim->m_Columns);
			ImGui::Dummy(ImVec2(0.0f, 1.0f));

			ImGui::Text("Play Speed");
			ImGui::DragFloat("###PlaySpeed", &spriteAnim->m_PlaySpeed, 1.0f);
			ImGui::Dummy(ImVec2(0.0f, 1.0f));

			ImGui::TreePop();
		}
	}

	void ImguiHelper::PRendererComponentHelper()
	{
		assert(m_CurrentEntity != entt::null);
		auto prenderer = m_ecs->GetReg().try_get<Components::PrimitiveRender>(m_CurrentEntity);
		if (prenderer)
		{
			bool enableRenderer = true;
			if (ImGui::CollapsingHeader(ICON_FA_TH "  Primitive Renderer", &enableRenderer, ImGuiTreeNodeFlags_DefaultOpen))
			{
				// Textures
				if (ImGui::TreeNode("Textures"))
				{
					static std::string* currPath = nullptr;
					const char* popuplabel = "Texture Selection";

					// Diffuse
					if (ImGui::TreeNode("Diffuse"))
					{
						// Diffuse color picker
						ImGui::Dummy(ImVec2(0.0f, 2.0f));
						ImGui::PushID("PREND");
						ImGui::Text("Color Tint");
						ImGui::ColorEdit3("###Color Tint", glm::value_ptr(prenderer->m_ColorTint));
						ImGui::PopID();
						ImGui::Dummy(ImVec2(0.0f, 1.0f));

						// Show current textures
						for (size_t i = 0; i < prenderer->m_DiffTex.size(); i++)
						{
							std::string* currTexID = &prenderer->m_DiffTex[i];
							ImGui::Text(currTexID->c_str());
							ImGui::SameLine();
							auto rawID = PP::TextureResource::m_TexturePool[*currTexID];
							ImGui::Text(std::to_string(rawID).c_str());

							// Texture picker
							if (ImGui::ImageButton(rawID, ImVec2(32, 32), { 0,0 }, { 1,1 }, -1, ImVec4{ 0,0,0,0 }, ImVec4{ 1,1,1,1 }))
							{
								currPath = currTexID;
								ImGui::OpenPopup(popuplabel);
							}
							ImGui::Dummy(ImVec2(0.0f, 0.5f));
						}
						// Pop up window
						TextureSelectHelper(popuplabel, currPath);
						ImGui::TreePop();
						ImGui::Dummy(ImVec2(0.0f, 1.0f));
					}

					// Spec
					if (ImGui::TreeNode("Specular"))
					{
						ImGui::Dummy(ImVec2(0.0f, 2.0f));
						for (size_t i = 0; i < prenderer->m_SpecTex.size(); i++)
						{
							std::string* currTexID = &prenderer->m_SpecTex[i];
							ImGui::Text(currTexID->c_str());
							ImGui::SameLine();
							auto rawID = PP::TextureResource::m_TexturePool[*currTexID];
							ImGui::Text(std::to_string(rawID).c_str());
							
							// Texture picker
							if (ImGui::ImageButton(rawID, ImVec2(32, 32), { 0,0 }, { 1,1 }, -1, ImVec4{ 0,0,0,0 }, ImVec4{ 1,1,1,1 }))
							{
								currPath = currTexID;
								ImGui::OpenPopup(popuplabel);
							}
							ImGui::Dummy(ImVec2(0.0f, 0.5f));
						}
						TextureSelectHelper(popuplabel, currPath);
						ImGui::TreePop();
						ImGui::Dummy(ImVec2(0.0f, 1.0f));
					}

					// Normal
					if (ImGui::TreeNode("Normal"))
					{
						ImGui::Dummy(ImVec2(0.0f, 2.0f));
						for (size_t i = 0; i < prenderer->m_NormTex.size(); i++)
						{
							std::string* currTexID = &prenderer->m_NormTex[i];
							ImGui::Text(currTexID->c_str());
							ImGui::SameLine();
							auto rawID = PP::TextureResource::m_TexturePool[*currTexID];
							ImGui::Text(std::to_string(rawID).c_str());

							// Texture picker
							if (ImGui::ImageButton(rawID, ImVec2(32, 32), { 0,0 }, { 1,1 }, -1, ImVec4{ 0,0,0,0 }, ImVec4{ 1,1,1,1 }))
							{
								currPath = currTexID;
								ImGui::OpenPopup(popuplabel);
							}
							ImGui::Dummy(ImVec2(0.0f, 0.5f));
						}
						TextureSelectHelper(popuplabel, currPath);
						ImGui::TreePop();
						ImGui::Dummy(ImVec2(0.0f, 1.0f));
					}

					// Emi
					if (ImGui::TreeNode("Bump"))
					{
						ImGui::Dummy(ImVec2(0.0f, 2.0f));
						for (size_t i = 0; i < prenderer->m_BumpTex.size(); i++)
						{
							std::string* currTexID = &prenderer->m_BumpTex[i];
							ImGui::Text(currTexID->c_str());
							ImGui::SameLine();
							auto rawID = PP::TextureResource::m_TexturePool[*currTexID];
							ImGui::Text(std::to_string(rawID).c_str());

							// Texture picker
							if (ImGui::ImageButton(rawID, ImVec2(32, 32), { 0,0 }, { 1,1 }, -1, ImVec4{ 0,0,0,0 }, ImVec4{ 1,1,1,1 }))
							{
								currPath = currTexID;
								ImGui::OpenPopup(popuplabel);
							}
							ImGui::Dummy(ImVec2(0.0f, 0.5f));
						}
						TextureSelectHelper(popuplabel, currPath);
						ImGui::TreePop();
					}
					ImGui::TreePop();
				}
				ImGui::NewLine();
			}
			if(!enableRenderer)
				m_ecs->GetReg().remove<Components::PrimitiveRender>(m_CurrentEntity);
		}
	}

	void ImguiHelper::ParticlesHelper()
	{
		assert(m_CurrentEntity != entt::null);
		auto pSystem = m_ecs->GetReg().try_get<Components::ParticleSystem>(m_CurrentEntity);
		if (pSystem)
		{
			bool enablePSystem = true;
			if (ImGui::CollapsingHeader(ICON_FA_VIRUS "  Particle System", &enablePSystem, ImGuiTreeNodeFlags_DefaultOpen))
			{	
				// To be used throughout this inspector component
				std::string toolTip = "";
				const char* popuplabel = "Particles Texture Selection";

				// Buttons
				ImGui::Dummy(ImVec2(0.0f, 2.0f));
				if (pSystem->m_Play)
				{
					if (pSystem->m_Pause)
					{
						if (ImGui::Button("Resume"))
						{
							pSystem->m_Pause = false;
							pSystem->m_SpriteAnimation.m_Pause = false;

							// Resume sprite animations
							for (auto& it : pSystem->m_ParticlePool)
							{
								it.m_SpriteAnimation.m_Pause = false;
							}
						}
					}
					else
					{
						if(ImGui::Button("Pause"))
						{
							pSystem->m_Pause = true;
							pSystem->m_SpriteAnimation.m_Pause = true;

							// Pause sprite animations
							for (auto& it : pSystem->m_ParticlePool)
							{
								it.m_SpriteAnimation.m_Pause = true;
							}
						}
					}
				}
				else
				{
					if (ImGui::Button("Play"))
					{
						pSystem->m_Play = true;
						pSystem->m_Done = false;
						pSystem->m_SpriteAnimation.m_Play = true;

						// Play sprite animations
						for (auto& it : pSystem->m_ParticlePool)
						{
							it.m_SpriteAnimation.m_Play = true;
						}
					}
				}
				ImGui::SameLine();
				if (ImGui::Button("Stop"))
				{
					pSystem->m_Play = false;
					pSystem->m_Pause = false;
					pSystem->Clear();
					pSystem->m_Done = false;
				}

				// Color
				ImGui::Dummy(ImVec2(0.0f, 1.0f));
				ImGui::PushID("PHelper");
				ImGui::Text("Color Tint");
				ImGui::ColorEdit3("###Color Tint", glm::value_ptr(pSystem->m_Color));
				ImGui::PopID();
				ImGui::Dummy(ImVec2(0.0f, 1.0f));

				// Texture
				ImGui::Text("Texture");
				ImGui::Text("Current Texture: %s", pSystem->m_TexName.c_str());
				//auto mappedID = PP::TextureResource::m_UsedTextures[pSystem->m_TexID];
				if (ImGui::ImageButton(PP::TextureResource::m_TexturePool[pSystem->m_TexName], ImVec2(32, 32), { 0,0 }, { 1,1 }, -1, ImVec4{ 0,0,0,0 }, ImVec4{ 1,1,1,1 }))
				{
					ImGui::OpenPopup(popuplabel);
				}
				ImGui::Dummy(ImVec2(0.0f, 0.5f));
				TextureSelectHelper(popuplabel, pSystem->m_TexName, pSystem->m_TexID);

				// Emitter type
				ImGui::Dummy(ImVec2(0.0f, 1.0f));
				constexpr int emitterTypeCount = static_cast<int>(Components::ParticleSystem::EMITTER_TYPE::COUNT);
				const std::string emitterChoices[emitterTypeCount] = { "General", "Burst", "Cone" };
				ImGui::Text("Emitter Type");
				const int currID = static_cast<int>(pSystem->m_EmitterType);
				std::string combo_label = emitterChoices[currID];  // Label to preview before opening the combo (technically it could be anything)
				if (ImGui::BeginCombo("###Emitter selection", combo_label.c_str()))
				{
					for (int n = 0; n < IM_ARRAYSIZE(emitterChoices); n++)
					{
						const bool is_selected = (currID == n);
						if (ImGui::Selectable(emitterChoices[n].c_str(), is_selected))
						{
							pSystem->m_EmitterType = static_cast<Components::ParticleSystem::EMITTER_TYPE>(n);
						}

						// Set the initial focus when opening the combo (scrolling + keyboard navigation focus)
						if (is_selected)
						{
							ImGui::SetItemDefaultFocus();
						}
					}
					ImGui::EndCombo();
				}
				ImGui::NewLine();

				// Billboard config
				ImGui::Text("Billboard Axis");
				toolTip = "Which axis will the particle rotate to face the camera";
				ToolTipHelper(toolTip.c_str(), true);
				ImGui::DragFloat3("###BillboardAxis", &pSystem->m_BillboardAxis.x, 1.0f, 0.0f, 1.0f);
				ImGui::NewLine();

				// Life
				ImGui::Text("Life");
				toolTip = "Actual life duration will be a random value between Min. and Max.";
				ToolTipHelper(toolTip.c_str(), true);

				ImGui::Dummy(ImVec2(0.0f, 1.0f));
				ImGui::Text("Min. Life");
				ImGui::DragFloat("###Min. Life", &pSystem->m_MinLife, 0.01f);
				ImGui::Text("Max. Life");
				ImGui::DragFloat("###Max. Life", &pSystem->m_MaxLife, 0.01f);
				ImGui::NewLine();

				// Force
				ImGui::Text("Force");
				toolTip = "Force applied over time";
				ToolTipHelper(toolTip.c_str(), true);
				ImGui::DragFloat3("###Force Direction", &pSystem->m_Force.x, 0.01f);
				ImGui::NewLine();

				// Delay
				ImGui::Text("Delay");
				toolTip = "Delay inbetween spawns (Only applies if burst is not on)";
				ToolTipHelper(toolTip.c_str(), true);
				ImGui::DragFloat("###SetDelay", &pSystem->m_Delay, 0.01f, 0.0f);
				ImGui::NewLine();

				switch (pSystem->m_EmitterType)
				{
				case Components::ParticleSystem::EMITTER_TYPE::GENERAL:
					// Spawn radius
					ImGui::Text("Spawn Radius");
					toolTip = "Random spawn within sphere of specified radius";
					ToolTipHelper(toolTip.c_str(), true);
					ImGui::DragFloat("###Spawn Radius", &pSystem->m_SpawnRadius, 0.01f);
					ImGui::NewLine();

					// Different direction config
					ImGui::Checkbox("Move To Target Pos.", &pSystem->m_MoveToTarget);
					ImGui::NewLine();

					if (pSystem->m_MoveToTarget)
					{
						// Direction
						ImGui::Text("Target Position");
						toolTip = "Target Position that particles will try to move to";
						ToolTipHelper(toolTip.c_str(), true);
						ImGui::DragFloat3("###Target Position", &pSystem->m_TargetPos.x, 0.01f);
						ImGui::NewLine();

						// Direction
						ImGui::Text("Direction");
						ImGui::DragFloat3("###Spawn Direction", &pSystem->m_SpawnDirection.x, 0.01f);
					}
					else
					{
						// Direction
						ImGui::Text("Direction");
						toolTip = "Spawn direction. that particles will move in";
						ToolTipHelper(toolTip.c_str(), true);
						ImGui::DragFloat3("###Spawn Direction", &pSystem->m_SpawnDirection.x, 0.01f);
					}
					break;
				case Components::ParticleSystem::EMITTER_TYPE::BURST:
					// Spawn radius
					ImGui::Text("Spawn Radius");
					toolTip = "Random spawn within sphere of specified radius";
					ToolTipHelper(toolTip.c_str(), true);
					ImGui::DragFloat("###Spawn Radius", &pSystem->m_SpawnRadius, 0.01f);
					ImGui::NewLine();

					// Burst amount
					ImGui::Text("Burst Spawn Count");
					ImGui::DragInt("###Burst Count", &pSystem->m_SpawnCount);
					ImGui::Dummy(ImVec2(0.0f, 1.0f));

					// Loop
					ImGui::Text("Loop");
					ImGui::SameLine();
					ImGui::Checkbox("###UseLoop", &pSystem->m_Loop);
					break;
				case Components::ParticleSystem::EMITTER_TYPE::CONE:
					// Loop
					ImGui::Text("Spawn Radius Min.");
					toolTip = "Random spawn within circle of specified radius";
					ToolTipHelper(toolTip.c_str(), true);
					ImGui::DragFloat("###ConeRadiusMin", &pSystem->m_ConeRadiusMin);
					ImGui::Text("Spawn Radius Max.");
					ImGui::DragFloat("###ConeRadiusMax", &pSystem->m_ConeRadiusMax);

					// Target cone size
					ImGui::Text("Target Cone Scale");
					ImGui::DragFloat("###MinAngle", &pSystem->m_TargetConeScale, 0.01f);

					// Direction
					ImGui::Text("Direction");
					toolTip = "Spawn direction. that particles will move in";
					ToolTipHelper(toolTip.c_str(), true);
					ImGui::DragFloat3("###Spawn Direction", &pSystem->m_SpawnDirection.x, 0.01f);

					// Burst amount
					ImGui::Text("Spawn Count");
					ImGui::DragInt("###Cone Spawn Count", &pSystem->m_SpawnCount);

					// Loop
					ImGui::Text("Trail");
					ImGui::SameLine();
					ImGui::Checkbox("###UseTrail", &pSystem->m_Trail);

					// Burst amount
					ImGui::Text("Trail Spawn Count");
					ImGui::DragInt("###Trail Spawn Count", &pSystem->m_SubSpawnCount);

					// Burst amount
					ImGui::Text("Trail Delay");
					ImGui::DragFloat("###Trail Spawn Delay", &pSystem->m_SubDelay);
					break;
				default:
					break;
				};
				ImGui::NewLine();

				ImGui::Dummy(ImVec2(0.0f, 1.0f));
				// Random rotate to make it look diff
				ImGui::Text("Randomly Rotate");
				ImGui::SameLine();
				ImGui::Checkbox("###UseRand", &pSystem->m_RandomRotate);

				// Follow parent?
				ImGui::Text("Parented");
				ImGui::SameLine();
				ImGui::Checkbox("###UseParent", &pSystem->m_FollowParent);
				ImGui::NewLine();

				// Speed
				toolTip = "Use curve to have the speed vary between Min.(0) to Max.(1)";
				if (ImGui::TreeNode("Speed Config"))
				{
					ToolTipHelper(toolTip.c_str(), true);

					ImGui::Dummy(ImVec2(0.0f, 1.0f));
					ImGui::Text("Min. Speed");
					ImGui::InputFloat("###Min. Speed", &pSystem->m_Speed.m_CurveMin, 0.01f);
					ImGui::Text("Max. Speed");
					ImGui::InputFloat("###Max. Speed", &pSystem->m_Speed.m_CurveMax, 0.01f);
					ImGui::Text("Min. Speed Multiplier");
					ImGui::DragFloat("###Min. Speed Multiplier", &pSystem->m_Speed.m_MultiplierMin, 0.01f);
					ImGui::Text("Max. Speed Multiplier");
					ImGui::DragFloat("###Max. Speed Multiplier", &pSystem->m_Speed.m_MultiplierMax, 0.01f);
					ImGui::Text("Speed Curve");
					ImGui::Dummy(ImVec2(0.0f, 1.0f));
					static int speedItem = 0;
					ImGui::Curve
					(
						"Speed",
						{ ImGui::GetContentRegionMax().x * 0.85f,256 },
						static_cast<int>(pSystem->m_Speed.m_MaxPoints - 1),
						pSystem->m_Speed.m_CurvePoints.data(),
						pSystem->m_Speed.m_CurveData, 
						speedItem
					);
					ImGui::TreePop();
					ImGui::NewLine();
				}
				else
				{
					ToolTipHelper(toolTip.c_str(), true);
				}

				// Scale
				toolTip = "Use curve to have the scale vary between Min.(0) to Max.(1)";
				if (ImGui::TreeNode("Scale Config."))
				{
					ToolTipHelper(toolTip.c_str(), true);

					ImGui::Dummy(ImVec2(0.0f, 1.0f));
					ImGui::Text("Min. Scale");
					ImGui::DragFloat("###Min. Scale", &pSystem->m_Scale.m_CurveMin, 0.01f);
					ImGui::Text("Max. Scale");
					ImGui::DragFloat("###Max. Scale", &pSystem->m_Scale.m_CurveMax, 0.01f);
					ImGui::Text("Min. Scale Multiplier");
					ImGui::DragFloat("###Min. Scale Multiplier", &pSystem->m_Scale.m_MultiplierMin, 0.01f);
					ImGui::Text("Max. Scale Multiplier");
					ImGui::DragFloat("###Max. Scale Multiplier", &pSystem->m_Scale.m_MultiplierMax, 0.01f);
					ImGui::Text("Scale Curve");
					ImGui::Dummy(ImVec2(0.0f, 1.0f));
					static int scaleItem = 0;
					ImGui::Curve
					(
						"Scale",
						{ ImGui::GetContentRegionMax().x * 0.85f,256 },
						static_cast<int>(pSystem->m_Scale.m_MaxPoints - 1),
						pSystem->m_Scale.m_CurvePoints.data(),
						pSystem->m_Scale.m_CurveData,
						scaleItem
					);
					ImGui::TreePop();
				}
				else
				{
					ToolTipHelper(toolTip.c_str(), true);
				}

				ParticleSpriteAnimationHelper();

				if(!enablePSystem)
					m_ecs->GetReg().remove<Components::ParticleSystem>(m_CurrentEntity);

				ImguiBlankSeperator(1);
				ImGui::Separator();
			}
		}
	}

	void ImguiHelper::ParticleSpriteAnimationHelper()
	{
		ImGui::Dummy(ImVec2(0.0f, 5.0f));
		if (ImGui::TreeNode("Sprite Animation"))
		{
			auto spriteAnim = &m_ecs->GetReg().try_get<Components::ParticleSystem>(m_CurrentEntity)->m_SpriteAnimation;

			ImGui::Dummy(ImVec2(0.0f, 2.0f));
			ImGui::Text("Current Frame: %d", static_cast<int>(spriteAnim->m_FrameCounter));
			ImGui::Text("UV: %f, %f", spriteAnim->m_UV_Offset.x, spriteAnim->m_UV_Offset.y);
			ImGui::Text("Tiling: %f, %f", spriteAnim->m_Tiling.x, spriteAnim->m_Tiling.y);

			ImGui::Dummy(ImVec2(0.0f, 2.0f));
			ImGui::Text("Set Frame");
			int frameSetter = static_cast<int>(spriteAnim->m_FrameCounter);
			if (ImGui::InputInt("###SetFrame", &frameSetter))
			{
				spriteAnim->SetFrame(frameSetter);
			}

			ImGui::Dummy(ImVec2(0.0f, 2.0f));
			ImGui::Text("Repeat");
			ImGui::SameLine();
			ImGui::Checkbox("###PSARepeat", &spriteAnim->m_Repeat);
			ImGui::Dummy(ImVec2(0.0f, 4.0f));

			// Config
			ImGui::Text("Rows");
			if (ImGui::DragInt("###PRows", &spriteAnim->m_Rows, 1.0f, 0, 100))
			{
				spriteAnim->UpdateTiling();
			}
			ImGui::Dummy(ImVec2(0.0f, 1.0f));

			ImGui::Text("Columns");
			if (ImGui::DragInt("###PColumns", &spriteAnim->m_Columns, 1.0f, 0, 100))
			{
				spriteAnim->UpdateTiling();
			}
			ImGui::Dummy(ImVec2(0.0f, 1.0f));

			ImGui::Text("Max Frames");
			ImGui::DragInt("###PMaxFrames", &spriteAnim->m_MaxFrames, 1.0f, 0, spriteAnim->m_Rows * spriteAnim->m_Columns);
			ImGui::Dummy(ImVec2(0.0f, 1.0f));

			ImGui::Text("Play Speed");
			ImGui::DragFloat("###PPlaySpeed", &spriteAnim->m_PlaySpeed, 1.0f);
			ImGui::Dummy(ImVec2(0.0f, 1.0f));

			ImGui::TreePop();
		}
	}

	void ImguiHelper::CanvasHelper()
	{
		assert(m_CurrentEntity != entt::null);
		auto canvas = m_ecs->GetReg().try_get<Components::Canvas>(m_CurrentEntity);
		if (canvas)
		{
			bool enableCanvas = true;
			if (ImGui::CollapsingHeader(ICON_FA_VIRUS "  Canvas 2D", &enableCanvas, ImGuiTreeNodeFlags_DefaultOpen))
			{
				const char* popuplabel = "Texture Selection";

				// World space or screen space?
				ImGui::Dummy(ImVec2(0.0f, 2.0f));
				ImGui::Text("Ortho");
				ImGui::SameLine();
				ImGui::Checkbox("###UseOrtho", &canvas->m_Ortho);
				ImGui::Dummy(ImVec2(0.0f, 1.0f));

				// Force alpha?
				ImGui::Dummy(ImVec2(0.0f, 2.0f));
				ImGui::Text("Force Alpha");
				ImGui::SameLine();
				ImGui::Checkbox("###UseFalpha", &canvas->m_ForceAlpha);
				ImGui::Dummy(ImVec2(0.0f, 1.0f));

				// Diffuse color picker
				ImGui::PushID("CANVAS");
				ImGui::Text("Color Tint");
				ImGui::ColorEdit4("###Color Tint", glm::value_ptr(canvas->m_Color));
				ImGui::PopID();
				ImGui::Dummy(ImVec2(0.0f, 1.0f));

				// Texture
				ImGui::Text("Texture");
				ImGui::Text("Current Texture: %s", canvas->m_TexName.c_str());
				if (ImGui::ImageButton(PP::TextureResource::m_TexturePool[canvas->m_TexName], ImVec2(32, 32), { 0,0 }, { 1,1 }, -1, ImVec4{ 0,0,0,0 }, ImVec4{ 1,1,1,1 }))
				{
					ImGui::OpenPopup(popuplabel);
				}
				ImGui::Dummy(ImVec2(0.0f, 0.5f));
				TextureSelectHelper(popuplabel, canvas->m_TexName, canvas->m_TexID);

				ImGui::Dummy(ImVec2(0.0f, 0.1f));
				SpriteAnimationHelper();

				if (!enableCanvas)
					m_ecs->GetReg().remove<Components::Canvas>(m_CurrentEntity);

				ImguiBlankSeperator(1);
				ImGui::Separator();
			}
		}
	}

}