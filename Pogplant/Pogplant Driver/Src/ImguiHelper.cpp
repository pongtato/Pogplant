#include "ImguiHelper.h"
#include "Pogplant.h"

#include <imgui.h>
#include <imgui_internal.h>
#include <imgui_impl_glfw.h>
#include <imgui_impl_opengl3.h>
#include <ImGuiExtraStyles.h>
#include <ImGuizmo.h>
#include <gtc/type_ptr.hpp>
#include <IconsFontAwesome5.h>
#include <IconsKenney.h>

#include "ECS/Entity.h"
#include "../../Pogplant/Src/Utils/FileDialogs.h"
#include "ECS/Systems/PhysicsSystem.h" // Should not be here?
#include "Serialiser/Serializer.h"
#include <algorithm>
#include <execution>

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
			adding_enabled = true;

		if (ImGui::BeginMenu("3D Render"))
		{
			glm::vec3 color = { 0.835f,0.921f,0.905f };
			for (const auto& model : PP::ModelResource::m_ModelPool)
			{
				if (ImGui::MenuItem(model.first.c_str(), NULL, false, adding_enabled))
				{
					(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::Renderer>(PPD::ImguiHelper::m_CurrentEntity,
						color,
						model.second,
						&model.second->m_Meshes[0]);
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
		if (ImGui::MenuItem("BoxCollider", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::BoxCollider>(PPD::ImguiHelper::m_CurrentEntity);
		}
		if (ImGui::MenuItem("SphereCollider", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::SphereCollider>(PPD::ImguiHelper::m_CurrentEntity);
		}
		if (ImGui::MenuItem(ICON_FA_CAMERA "  Camera", NULL, false, adding_enabled))
		{
			(void)PPD::ImguiHelper::m_ecs->GetReg().get_or_emplace<Components::Camera>(PPD::ImguiHelper::m_CurrentEntity);
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
	}

	bool ImguiHelper::m_FirstRun = true;
	//int ImguiHelper::m_CurrentGOIdx = -1;
	entt::entity ImguiHelper::m_CurrentEntity = entt::null;
	ECS* ImguiHelper::m_ecs = nullptr;
	Directory ImguiHelper::m_Directory;

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

		return true;
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
				ImGui::DockBuilderDockWindow("Prefab", m_DockIDs["EXPLORER"]);
				ImGui::DockBuilderDockWindow("Debug", m_DockIDs["EXPLORER"]);
				ImGui::DockBuilderDockWindow("Scene Hierarchy", m_DockIDs["LEFT_DOCK"]);
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

			ImGui_EndMainStatusBar();
		}

		// Directory Render
		m_Directory.RenderDirectory();


		ImGui::Begin("Debug");
		{
			const float TEXT_BASE_HEIGHT = ImGui::GetTextLineHeightWithSpacing();

			static ImGuiTableFlags flags = ImGuiTableFlags_ScrollY | ImGuiTableFlags_RowBg | ImGuiTableFlags_BordersOuter | ImGuiTableFlags_BordersV | ImGuiTableFlags_Resizable | ImGuiTableFlags_Reorderable | ImGuiTableFlags_Hideable;

			ImGuiStyle& style = ImGui::GetStyle();
			ImGui::PushStyleVar(ImGuiStyleVar_FramePadding, ImVec2(style.FramePadding.x, (float)(int)(style.FramePadding.y * 0.60f)));
			ImGui::PushStyleVar(ImGuiStyleVar_ItemSpacing, ImVec2(style.ItemSpacing.x, (float)(int)(style.ItemSpacing.y * 0.60f)));
			ImGui::PopStyleVar(2);

			if (ImGui::BeginTable("##table", 2, flags))
			{
				//ImGui::TableSetupScrollFreeze(0, 0); // Make top row always visible
				ImGui::TableSetupColumn("Source", ImGuiTableColumnFlags_None, 0.2f);
				ImGui::TableSetupColumn("Description", ImGuiTableColumnFlags_None);
				ImGui::TableHeadersRow();

				for (auto& it : PP::Logger::m_Logs)
				{
					const char* typeText;
					switch (it.m_Type)
					{
					case PP::LogEntry::ERROR:
						ImGui::PushStyleColor(0, ImVec4{ 0.75f,0.0f,0.0f,1.0f });
						typeText = "ERROR";
						break;
					case PP::LogEntry::SUCCESS:
						ImGui::PushStyleColor(0, ImVec4{ 0.55f,0.8f,0.2f,1.0f });
						typeText = "SUCCESS";
						break;
					case PP::LogEntry::WARNING:
						ImGui::PushStyleColor(0, ImVec4{ 1.0f,1.0f,0.0f,1.0f });
						typeText = "WARNING";
						break;
					case PP::LogEntry::DEBUG_TEXT:
						ImGui::PushStyleColor(0, ImVec4{ 1.0f,1.0f,1.0f,1.0f });
						typeText = "LOG";
						break;
					default:
						ImGui::PushStyleColor(0, ImVec4{ 0.2f,1.0f,1.0f,1.0f });
						typeText = "Undefined Type";
						break;
					}

					ImGui::TableNextRow();
					ImGui::TableSetColumnIndex(0);
					ImGui::Text(it.m_Source.c_str());
					ImGui::TableSetColumnIndex(1);
					ImGui::Text(it.m_Description.c_str());
					ImGui::PopStyleColor();
					// Scroll to bottom
				}

				ImGui::EndTable();
			}
		}
		ImGui::End();

		ImGui::Begin("Scene Hierarchy");
		{
			auto results = m_ecs->GetReg().view<Components::Transform>();

			if (ImGui::IsMouseDown(0) && ImGui::IsWindowHovered())
				m_CurrentEntity = entt::null;

			std::for_each(results.rbegin(), results.rend(), [&results](auto entity) 
				{
					DrawEntityNode(entity);
				});


			if (m_CurrentEntity != entt::null)
			{
				if (ImGui::BeginPopupContextWindow("EntityPopup", ImGuiPopupFlags_MouseButtonRight))
				{
					if (ImGui::MenuItem("Create Child"))
					{
						m_ecs->CreateChild(m_CurrentEntity);
					}

					if (ImGui::MenuItem("Delete Entity"))
					{
						m_ecs->DestroyEntity(m_CurrentEntity);
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
					m_ecs->CreateEntity();
				if (ImGui::MenuItem("Load Prefab"))
					LoadPrefab();

				ImGui::EndPopup();
			}
		}
		ImGui::End();

		ImGui::Begin("Inspector");
		{
			if (m_CurrentEntity != entt::null)
			{
				auto naming = m_ecs->GetReg().try_get<Components::Name>(m_CurrentEntity);
				if (naming && ImGui::CollapsingHeader(ICON_FA_FILE_SIGNATURE"  Name", ImGuiTreeNodeFlags_DefaultOpen))
				{
					Reflect_ImGui(naming);
				}

				auto transform = m_ecs->GetReg().try_get<Components::Transform>(m_CurrentEntity);
				if (transform && ImGui::CollapsingHeader(ICON_FA_SLIDERS_H "  Transform", ImGuiTreeNodeFlags_DefaultOpen))
				{
					Reflect_ImGui(transform);
				}

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
						std::string select = renderer->m_Mesh->m_Name;
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

						ImGui::Text("Color Editor");
						ImGui::ColorEdit3("###RenderColor", glm::value_ptr(renderer->m_ColorTint));

						ImGui::Text("RLighting");
						bool temp_light = renderer->m_UseLight;
						ImGui::Checkbox("Use Light", &temp_light);
						if (temp_light != static_cast<bool>(renderer->m_UseLight))
							renderer->m_UseLight = temp_light;

						ImguiBlankSeperator(1);
						ImGui::Separator();
					}
					if (!enable_render)
					{
						m_ecs->GetReg().remove<Components::Renderer>(m_CurrentEntity);
					}
				}

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
						ImGui::InputFloat("###", &rigid->mass , 0.1f, 1.0f, "%.3f");
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

						auto identifier = m_ecs->GetReg().try_get<Components::ColliderIdentifier>(m_CurrentEntity);
						if (identifier)
							identifier->isTrigger = box_collider->isTrigger;

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

						auto identifier = m_ecs->GetReg().try_get<Components::ColliderIdentifier>(m_CurrentEntity);
						if (identifier)
							identifier->isTrigger = sphere_collider->isTrigger;

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

				auto camera_com = m_ecs->GetReg().try_get<Components::Camera>(m_CurrentEntity);
				if (camera_com)
				{
					bool enable_camera_com = true;

					if (ImGui::CollapsingHeader(ICON_FA_CAMERA_RETRO "  Camera", &enable_camera_com, ImGuiTreeNodeFlags_DefaultOpen))
					{

						ImGui::Text("Active Cam");
						ImGui::Checkbox("Active?", &camera_com->m_Active);

						ImGui::Text("Yaw");
						ImGui::InputFloat("###yaw", &camera_com->m_Yaw, 0.1f, 1.0f, "%.3f");

						ImGui::Text("Pitch");
						ImGui::InputFloat("###pitch", &camera_com->m_Pitch, 0.1f, 1.0f, "%.3f");

						ImGui::Text("Near");
						ImGui::InputFloat("###near", &camera_com->m_Near, 0.1f, 1.0f, "%.3f");

						ImGui::Text("Far");
						ImGui::InputFloat("###far", &camera_com->m_Far, 1.f, 1.0f, "%.3f");

						ImGui::Text("Zoom");
						ImGui::InputFloat("###zoom", &camera_com->m_Zoom, 1.f, 1.0f, "%.3f");


						ImguiBlankSeperator(1);
						ImGui::Separator();
					}
					if (!enable_camera_com)
					{
						m_ecs->GetReg().remove<Components::Camera>(m_CurrentEntity);
					}
				}

				//TODO
				auto audioSourceComponent = m_ecs->GetReg().try_get<Components::AudioSource>(m_CurrentEntity);
				if (audioSourceComponent)
				{
					bool enable_audio_com = true;

					if (ImGui::CollapsingHeader(ICON_FA_MUSIC "  AudioSource", &enable_audio_com, ImGuiTreeNodeFlags_DefaultOpen))
					{
						for (size_t i = 0; i < audioSourceComponent->m_audioSources.size(); i++)
						{
							ImGui::Text(audioSourceComponent->m_audioSources[i].m_fileDir.c_str());

							bool hasChanged = false;

							hasChanged |= ImGui::Checkbox("Is 3D", &audioSourceComponent->m_audioSources[i].m_is3D);
							hasChanged |= ImGui::Checkbox("Loop", &audioSourceComponent->m_audioSources[i].m_isLooping);
							hasChanged |= ImGui::Checkbox("Stream audio", &audioSourceComponent->m_audioSources[i].m_isStreamed);
							hasChanged |= ImGui::Checkbox("Enable doppler", &audioSourceComponent->m_audioSources[i].m_enableDopplerEffect);
							hasChanged |= ImGui::SliderFloat("Volume", &audioSourceComponent->m_audioSources[i].m_volume, 0.f, 1.f);

							if (hasChanged)
								audioSourceComponent->UpdateAudioSettings(i);

							if (ImGui::Button("Play"))
								audioSourceComponent->PlayAudio(i);

							ImGui::SameLine();

							if (ImGui::Button("Stop"))
								audioSourceComponent->StopAudio(i);

							ImguiBlankSeperator(1);
						}

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

					if (ImGui::CollapsingHeader(ICON_FA_SCROLL "  Active Scripts", &enable_scripts_com, ImGuiTreeNodeFlags_DefaultOpen))
					{
						std::vector<std::string> totalScripts;
						// I have to hard code the script names for now until I figure out a better solution.
						totalScripts.push_back("Player");
						totalScripts.push_back("Enemy");

						for (auto& scriptName : totalScripts)
						{
							bool hasScript = scripts_com->m_ScriptTypes.contains(scriptName);
							bool setScript = hasScript;
							ImGui::Checkbox(scriptName.c_str(), &setScript);
							if (setScript != hasScript)
							{
								if (setScript == false)
								{
									scripts_com->m_ScriptTypes.erase(scriptName);
									std::cout << "Entity [" << name_com->m_name << "] has stopped script [" << scriptName << "]" << std::endl;
								}
								else
								{
									scripts_com->m_ScriptTypes[scriptName] = false;
								}
							}
						}
					}

					if (!enable_scripts_com)
					{
						m_ecs->GetReg().remove<Components::Scriptable>(m_CurrentEntity);
						std::cout << "Entity [" << name_com->m_name << "] has removed script component" << std::endl;
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

		ImGui::Begin("Scene");
		{
			SceneWindow();
		}
		ImGui::End();

		ImGui::Begin("Game");
		{
			GameWindow();
		}
		ImGui::End();

		// Focus select windows on first run
		if (m_FirstRun)
		{
			ImGui::SetWindowFocus("Prefab");
			ImGui::SetWindowFocus("Scene");
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

		// Draw the actual editor scene
		ImGui::Image(PP::FBR::m_FrameBuffers[PP::BufferType::EDITOR_COLOR_BUFFER], ImGui::GetContentRegionAvail(), ImVec2(0, 1), ImVec2(1, 0));

		// Config the draw zone for guizmo
		ImVec2 vMin = ImGui::GetWindowContentRegionMin();
		ImVec2 vMax = ImGui::GetWindowContentRegionMax();

		// Aspect ratio update
		Pogplant::Camera* currCam = PP::CameraResource::GetCamera("EDITOR");
		currCam->UpdateProjection({ vMax.x, vMax.y });

		// Account for position of window
		vMin.x += ImGui::GetWindowPos().x;
		vMin.y += ImGui::GetWindowPos().y + 20; // + 20 to account for the text line kekw
		vMax.x += ImGui::GetWindowPos().x;
		vMax.y += ImGui::GetWindowPos().y;

		/// GUIZMO GO EDIT
		Scene_GOEdit(currCam, vMin, vMax);

		/// Picker
		Scene_GOPick(currCam, vMin, vMax);

		/// GUIZMO VIEW EDIT
		Scene_ViewEdit(currCam, vMin, vMax);
	}

	void ImguiHelper::GameWindow()
	{
		ImGui::PushStyleColor(0, ImVec4{ 0.55f,0.8f,0.2f,1 });
		ImGui::Text("Application average %.3f ms/frame (%.1f FPS)", 1000.0f / ImGui::GetIO().Framerate, ImGui::GetIO().Framerate);
		ImGui::PopStyleColor();
		ImGui::Image(PP::FBR::m_FrameBuffers[PP::BufferType::GAME_COLOR_BUFFER], ImGui::GetContentRegionAvail(), ImVec2(0, 1), ImVec2(1, 0));

		ImVec2 vMax = ImGui::GetWindowContentRegionMax();
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
			Serializer serialiser;
			serialiser.Save(filepath);
		}
	}

	void ImguiHelper::NewScene()
	{
		m_CurrentEntity = entt::null;
		m_ecs->GetReg().clear();
	}

	void ImguiHelper::OpenScene()
	{
		std::string filepath = Pogplant::FileDialogs::OpenFile("Json Files(*.json)\0*.json\0");
		if (!filepath.empty())
			OpenScene(filepath);
	}

	void PogplantDriver::ImguiHelper::SavePrefab(entt::entity _object)
	{
		std::string filepath = Pogplant::FileDialogs::SaveFile("Json Files(*.json)\0*.json\0");
		//Append .json 
		if (filepath.find(".json") == std::string::npos)
		{
			filepath.append(".json");
		}
		if (!filepath.empty())
		{
			Serializer serialiser;
			serialiser.SavePrefab(filepath, _object);
		}
	}

	void PogplantDriver::ImguiHelper::LoadPrefab()
	{
		std::string filepath = Pogplant::FileDialogs::OpenFile("Json Files(*.json)\0*.json\0");
		if (!filepath.empty())
		{
			Serializer serialiser;
			serialiser.LoadPrefab(filepath);
		}
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

	void ImguiHelper::Scene_GOPick(Pogplant::Camera* _CurrCam, ImVec2 _VMin, ImVec2 _VMax)
	{
		if (!ImGui::IsWindowFocused())
		{
			return;
		}

		if (ImGui::IsMouseClicked(ImGuiMouseButton_Left) && !m_GoEditing)
		{
			glm::vec2 min = { _VMin.x,_VMin.y };
			glm::vec2 max = { _VMax.x,_VMax.y };
			glm::vec2 cursor = { ImGui::GetMousePos().x,ImGui::GetMousePos().y };
			_CurrCam->UpdateRayConfig({ min,max,max - min,cursor });
			_CurrCam->RayCast();

			// Ray for picking
			const PP::Ray ray = _CurrCam->GetRay();

			float shortestTime = std::numeric_limits<float>::max();
			entt::entity chosenObject = entt::null;

			m_ecs->GetReg().each([&ray, &shortestTime, &chosenObject](auto entity)
			{
				//GameObject& currGO = GO_Resource::m_GO_Container[i];
					
				auto& transform = m_ecs->GetReg().get<Components::Transform>(entity);
				auto renderer = m_ecs->GetReg().try_get<Components::Renderer>(entity);
				//auto debugRenderer = m_ecs->GetReg().try_get<Components::DebugRender>(entity);

				if (renderer != nullptr)
				{
					// Naive approach
					float largestScale = std::numeric_limits<float>::min();

					for (int j = 0; j < 3; j++)
					{
						largestScale = std::max(largestScale, transform.m_scale[j]);
					}

					const float radius = renderer->m_RenderModel->m_Bounds.longest * 0.5f * largestScale;
					float currentTime = std::numeric_limits<float>::max();
					if (ray.CollideSphere(glm::make_vec3(transform.m_position), radius, currentTime))
					{
						if (currentTime < shortestTime)
						{
							chosenObject = entity;
							shortestTime = currentTime;
						}
					}
				}
			});

			//for (size_t i = 0; i < GO_Resource::m_GO_Container.size(); i++)
			//{
			//	GameObject& currGO = GO_Resource::m_GO_Container[i];
			//	// Naive approach
			//	float largestScale = std::numeric_limits<float>::min();
			//	for (int j = 0; j < 3; j++)
			//	{
			//		largestScale = std::max(largestScale, currGO.m_Scale[i]);
			//	}
			//	const float radius = currGO.m_RenderObject->m_RenderModel->m_Bounds.longest * 0.5f * largestScale;
			//	float currentTime = std::numeric_limits<float>::max();
			//	if (ray.CollideSphere(glm::make_vec3(currGO.m_Position), radius, currentTime))
			//	{
			//		if (currentTime < shortestTime)
			//		{
			//			chosenObject = static_cast<int>(i);
			//			shortestTime = currentTime;
			//		}
			//	}
			//}

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
		auto relationship = m_ecs->GetReg().try_get<Components::Relationship>(_id);

		if (relationship && relationship->m_parent != entt::null)
		{
			auto transform = m_ecs->GetReg().get<Components::Transform>(relationship->m_parent);

			return transform.m_ModelMtx * glm::inverse(get_parent_transform(relationship->m_parent));
		}
		else
		{
			auto transform = m_ecs->GetReg().get<Components::Transform>(_id);
			return transform.m_ModelMtx;
		}
	}

	void ImguiHelper::Scene_GOEdit(Pogplant::Camera* _CurrCam, ImVec2 _VMin, ImVec2 _VMax)
	{
		// Draw view manipulate only in editor scene
		ImGuizmo::SetDrawlist();

		// Bounds for guizmo
		ImGuizmo::SetRect(_VMin.x, _VMin.y, _VMax.x, _VMax.y);

		if (m_CurrentEntity != entt::null)
		{
			//GameObject& currGO = GO_Resource::m_GO_Container[m_CurrentGOIdx];

			auto& transform = m_ecs->GetReg().get<Components::Transform>(m_CurrentEntity);

			////glm::mat4& pos = transform.m_ModelMtx;
			//glm::mat4 parent_pos;
			//bool parent = false;
			//
			//auto relationship = m_ecs->GetReg().try_get<Components::Relationship>(m_CurrentEntity);

			//if (relationship && relationship->m_parent != entt::null)
			//{
			//	parent = true;
			//	parent_pos = get_parent_transform(relationship->m_parent);
			//	////pos += parent_pos;
			//	//pos = parent_pos * pos;
			//}


			 //Gizmo transform, matrix to components & back
			ImGuizmo::RecomposeMatrixFromComponents(glm::value_ptr(transform.m_position),
													glm::value_ptr(transform.m_rotation),
													glm::value_ptr(transform.m_scale),
													glm::value_ptr(transform.m_ModelMtx));

			//if (parent)
			//{
			//	auto iasd = glm::inverse(parent_pos);
			//	transform.m_ModelMtx = transform.m_ModelMtx * iasd;
			//}
			ImGuizmo::Manipulate
			(
				m_GoEditing,
				glm::value_ptr(_CurrCam->GetView()),
				glm::value_ptr(_CurrCam->GetPerspective()),
				m_EditMode,
				ImGuizmo::LOCAL,
				glm::value_ptr(transform.m_ModelMtx),
				NULL,
				m_UseSnap ? m_SnapStep : NULL,
				m_BoundSizing ? m_BoundsPos : NULL,
				m_UseBoundsSnap ? m_BoundsSnapStep : NULL
			);

			//if (parent)
			//{
			//	auto iasd = glm::inverse(parent_pos);
			//	transform.m_ModelMtx = transform.m_ModelMtx * iasd;
			//}
			////else
			{

				ImGuizmo::DecomposeMatrixToComponents(glm::value_ptr(transform.m_ModelMtx),
														glm::value_ptr(transform.m_position),
														glm::value_ptr(transform.m_rotation),
														glm::value_ptr(transform.m_scale));
			}





			//transform.m_position = pos;

		}
	}

	void ImguiHelper::Scene_ViewEdit(Pogplant::Camera* _CurrCam, ImVec2 _VMin, ImVec2 _VMax)
	{
		float view[16] = { 0 };
		float projection[16] = { 0 };
		float front[3] = { 0 };
		memcpy(view, glm::value_ptr(_CurrCam->View()), sizeof(_CurrCam->View()));
		memcpy(projection, glm::value_ptr(_CurrCam->Perspective()), sizeof(_CurrCam->Perspective()));
		// After clickng on gizmo update yaw pitch accordingly
		if (ImGuizmo::ViewManipulate(view, 1.0f, ImVec2(_VMax.x - 128, _VMin.y), ImVec2(128, 128), 0x0, front))
		{
			_CurrCam->UpdateFront(front);
		}
		// Updated view from gizmo
		_CurrCam->View() = glm::make_mat4(view);

		// Make sure begin is being called before this function
		// This ensures the input for camera only works when the Scene window is focused
		if (ImGui::IsWindowFocused())
		{
			PP::CameraResource::SetActiveCam("EDITOR");
		}
		else
		{
			PP::CameraResource::DeselectCam();
		}
	}

	bool ImguiHelper::DrawEntityNode(entt::entity entity, bool draw_childen)
	{

		auto _r = m_ecs->GetReg().try_get<Components::Relationship>(entity);

		if (!draw_childen && _r && _r->m_parent != entt::null)
			return false;

		auto name = m_ecs->GetReg().get<Components::Name>(entity);
		std::string obj_name = name.m_name;
		ImGuiTreeNodeFlags flags = (m_CurrentEntity == entity) ? ImGuiTreeNodeFlags_Selected : 0;
		if(_r && _r->m_children.size() != 0)
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

	void ImguiHelper::OpenScene(const std::filesystem::path& path)
	{
		if (path.extension().string() != ".json")
		{
			//ASSET HERE
			//("Could not load {0} - not a scene file", path.filename().string());
			return;
		}
		NewScene();
		Serializer serialiser;
		if(!serialiser.Load(path.string()))
		{
			//ASSET HERE
			//Something failed to load?
		}
	}

	std::string ImguiHelper::CreateStringInputField(std::string& _label, std::string _target)
	{
		ImGui::Text(_label.c_str());
		static char name_stuff[256] = "";
		sprintf_s(name_stuff, IM_ARRAYSIZE(name_stuff), _target.c_str());

		std::string aaa{ "###" };
		aaa.append(_label);

		ImGui::InputText(aaa.c_str(), name_stuff, IM_ARRAYSIZE(name_stuff));

		return std::string{ name_stuff };
	}

	void ImguiHelper::CreateDragFloat3(std::string& _label, float* _value, float increment_speed, float min_val, float max_val)
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

}