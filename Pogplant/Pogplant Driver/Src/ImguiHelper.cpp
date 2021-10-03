#include "ImguiHelper.h"
#include "Pogplant.h"

#include <imgui.h>
#include <imgui_internal.h>
#include <imgui_impl_glfw.h>
#include <imgui_impl_opengl3.h>
#include <ImGuiExtraStyles.h>
#include <ImGuizmo.h>
#include <gtc/type_ptr.hpp>

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
		ImGuiIO& io = ImGui::GetIO(); (void)io;
		io.Fonts->AddFontFromFileTTF("Resources/Fonts/Ruda.ttf", 14);
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
				if (ImGui::MenuItem("New"))
					NewScene();
				if (ImGui::MenuItem("Open..."))
					OpenScene();
				if (ImGui::MenuItem("Save"))
					SaveSceneAs();
				if (ImGui::MenuItem("Exit"))
					exiting = true;
				ImGui::EndMenu();
			}

			if (ImGui::BeginMenu("Components"))
			{
					bool adding_enabled = false;
					if (m_CurrentEntity != entt::null)
						adding_enabled = true;
					//if(ImGui::MenuItem("Transform", NULL, false, adding_enabled))
					//{
					//	auto& trans_component = m_ecs->GetReg().get_or_emplace<Components::Transform>(m_CurrentEntity);
					//}
					if (ImGui::BeginMenu("3D Render"))
					{
						glm::vec3 color = { 0.835f,0.921f,0.905f };
						if (ImGui::MenuItem("Sphere"))
						{
							auto& renderer = m_ecs->GetReg().get_or_emplace<Components::Renderer>(m_CurrentEntity,
								glm::mat4{1},
								color, 
								PP::ModelResource::m_ModelPool["Sphere"]);
						}
						ImGui::EndMenu();
					}
					if (ImGui::MenuItem("Point_Light", NULL, false, adding_enabled))
					{
						auto& lightp_component = m_ecs->GetReg().get_or_emplace<Components::Point_Light>(m_CurrentEntity);
					}
					if (ImGui::MenuItem("Directional_Light", NULL, false, adding_enabled))
					{
						auto& lightd_component = m_ecs->GetReg().get_or_emplace<Components::Directional_Light>(m_CurrentEntity);
					}
					if (ImGui::MenuItem("Font", NULL, false, adding_enabled)) //There is another font somewhere might crash (But should not as id stack is cleared)
					{
						auto& new_font = m_ecs->GetReg().get_or_emplace<Components::Text>(m_CurrentEntity, glm::vec3 {0.835f,0.921f,0.905f },"Ruda", "Lorem ipsum dolor sit amet");
					}
					if (ImGui::MenuItem("RigidBody", NULL, false, adding_enabled)) 
					{
						auto& new_body = m_ecs->GetReg().get_or_emplace<Components::Rigidbody>(m_CurrentEntity);
					}
					if (ImGui::MenuItem("BoxCollider", NULL, false, adding_enabled))
					{
						auto& new_bcollider = m_ecs->GetReg().get_or_emplace<Components::BoxCollider>(m_CurrentEntity);
					}
					ImGui::EndMenu();
				}

			if (ImGui::BeginMenu("Tools"))
			{
				ImGui::MenuItem("(Tools menu)", NULL, false, false);
				if (ImGui::MenuItem("Tool?")) {}
				ImGui::EndMenu();
			}
			ImGui::EndMainMenuBar();
		}
		if(exiting)
			ImGui::OpenPopup("Exiting");
		//ImVec2 center = ImGui::GetMainViewport()->GetCenter();
		//ImGui::SetNextWindowPos(center, ImGuiCond_Appearing, ImVec2(0.5f, 0.5f));
		if (ImGui::BeginPopupModal("Exiting", NULL, ImGuiWindowFlags_AlwaysAutoResize))
		{
			ImGui::Text("All those beautiful files will be deleted.\nThis operation cannot be undone!\n\n");
			ImGui::Separator();

			//static int unused_i = 0;
			//ImGui::Combo("Combo", &unused_i, "Delete\0Delete harder\0");

			//static bool dont_ask_me_next_time = false;
			//ImGui::PushStyleVar(ImGuiStyleVar_FramePadding, ImVec2(0, 0));
			//ImGui::Checkbox("Don't ask me next time", &dont_ask_me_next_time);
			//ImGui::PopStyleVar();

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


		// Directory Render
		m_Directory.RenderDirectory();



		ImGui::Begin("Prefab");
		{
			ImGui::Text("Prefab");
		}
		ImGui::End();

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
					//ImGui::SetScrollHere(1.0f);
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

			//printf("id: %d\n", m_CurrentEntity);


			//right click hovered item
			//if (ImGui::IsItemHovered())
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

				//if (ImGui::MenuItem("Create From Prefab"))
				//{
				//	//CreateEntityFromPrefab(m_current_scene, ecs_handler);
				//}

				ImGui::EndPopup();
			}
		}
		ImGui::End();

		ImGui::Begin("Inspector");
		{
			if (m_CurrentEntity != entt::null)
			{
				auto naming = m_ecs->GetReg().try_get<Components::Name>(m_CurrentEntity);
				if (naming && ImGui::CollapsingHeader("Name", ImGuiTreeNodeFlags_DefaultOpen))
				{
					ImGui::Text("Name");
					static char name_stuff[256] = "";
					sprintf_s(name_stuff, IM_ARRAYSIZE(name_stuff), naming->m_name.c_str());
					ImGui::InputText("###TI", name_stuff, IM_ARRAYSIZE(name_stuff));
					naming->m_name = name_stuff;
					ImguiBlankSeperator(1);
				}


				//GameObject& currGO = GO_Resource::m_GO_Container[m_CurrentGOIdx];
				auto transform = m_ecs->GetReg().try_get<Components::Transform>(m_CurrentEntity);
				if (transform && ImGui::CollapsingHeader("Transform", ImGuiTreeNodeFlags_DefaultOpen))
				{
					// Mode switch
					if (ImGui::RadioButton("Translate", m_EditMode == ImGuizmo::TRANSLATE))
					{
						m_EditMode = ImGuizmo::TRANSLATE;
					}
					ImGui::SameLine();
					if (ImGui::RadioButton("Rotate", m_EditMode == ImGuizmo::ROTATE))
					{
						m_EditMode = ImGuizmo::ROTATE;
					}
					ImGui::SameLine();
					if (ImGui::RadioButton("Scale", m_EditMode == ImGuizmo::SCALE))
					{
						m_EditMode = ImGuizmo::SCALE;
					}

					// Snap when editing transform
					ImGui::Checkbox("Snap Transform", &m_UseSnap);
					//Bounds edit
					ImGui::Checkbox("Edit Bounds", &m_BoundSizing);
					//Snap when editing
					ImGui::Checkbox("Snap Bounds", &m_UseBoundsSnap);

					// Usual stuff
					ImGui::Text("Translate");
					ImGui::PushID("Tr");
					//ImGui::DragFloat3("", currGO.m_Position);
					ImGui::DragFloat3("", glm::value_ptr(transform->m_position));
					ImGui::PopID();
					ImGui::Text("Rotate");
					ImGui::PushID("Rt");
					//ImGui::DragFloat3("", currGO.m_Rotation);
					ImGui::DragFloat3("", glm::value_ptr(transform->m_rotation));
					ImGui::PopID();
					ImGui::Text("Scale");
					ImGui::PushID("Sc");
					//ImGui::DragFloat3("", currGO.m_Scale);
					ImGui::DragFloat3("", glm::value_ptr(transform->m_scale));
					ImGui::PopID();
				}

				auto renderer = m_ecs->GetReg().try_get<Components::Renderer>(m_CurrentEntity);
				if (renderer)
				{
					bool enable_render = true;

					if (ImGui::CollapsingHeader("Renderer", &enable_render, ImGuiTreeNodeFlags_DefaultOpen))
					{
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
								const bool  is_selected = (model_itr == it);
								if (ImGui::Selectable(it->first.c_str(), is_selected))
								{
									model_itr = it;
									renderer->m_RenderModel = model_itr->second;
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
						if (temp_light != renderer->m_UseLight)
							renderer->m_UseLight = temp_light;

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
					if (ImGui::CollapsingHeader("Point Lighting", &enable_pointlight, ImGuiTreeNodeFlags_DefaultOpen))
					{
						ImGui::Text("Light Color Editor");
						ImGui::ColorEdit3("###PLight", glm::value_ptr(point_light->m_Color));

						ImGui::Text("Light Intensity");
						ImGui::InputFloat("###PLI", &point_light->m_Intensity, 0.1f, 1.0f, "%.3f");

						ImGui::Text("Linear Attenuation");
						ImGui::InputFloat("###PLA", &point_light->m_Linear, 0.001f, 1.0f, "%.3f");

						ImGui::Text("Quadratic Attenuation");
						ImGui::InputFloat("###PQA", &point_light->m_Quadratic, 0.001f, 1.0f, "%.3f");

					}
					if (!enable_pointlight)
					{
						m_ecs->GetReg().remove<Components::Point_Light>(m_CurrentEntity);
					}
				}

				auto direction_light = m_ecs->GetReg().try_get<Components::Directional_Light>(m_CurrentEntity);
				if (direction_light)
				{
					m_EditMode = ImGuizmo::SCALE;
					bool enable_directionlight = true;
					if (ImGui::CollapsingHeader("Directional Lighting", &enable_directionlight, ImGuiTreeNodeFlags_DefaultOpen))
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
					if (ImGui::CollapsingHeader("Text Editor", &enable_text, ImGuiTreeNodeFlags_DefaultOpen))
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
				}

				auto rigid = m_ecs->GetReg().try_get<Components::Rigidbody>(m_CurrentEntity);
				if (rigid)
				{
					bool enable_rigid = true;

					if (ImGui::CollapsingHeader("RigidBody", &enable_rigid, ImGuiTreeNodeFlags_DefaultOpen))
					{
						ImGui::Text("Mass");
						ImGui::InputFloat("###", &rigid->mass , 0.1f, 1.0f, "%.3f");
						ImGui::Checkbox("Kinematic", &rigid->isKinematic);
						ImGui::SameLine();
						ImGui::Checkbox("Gravity", &rigid->useGravity);

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

					if (ImGui::CollapsingHeader("Collider", &enable_box_collider, ImGuiTreeNodeFlags_DefaultOpen))
					{
						ImGuiComboFlags flag = 0;
						flag |= ImGuiComboFlags_PopupAlignLeft;
						std::vector<std::string> collision_rule_str = { "Collide", "Event", "Collide & Event", "Ignore" };
						std::vector<int> collision_rule = { Components::BoxCollider::COLLISION_RULE::CR_COLLIDE,
																								Components::BoxCollider::COLLISION_RULE::CR_EVENT,
																								Components::BoxCollider::COLLISION_RULE::CR_COLLIDE_EVENT,
																								Components::BoxCollider::COLLISION_RULE::CR_IGNORE };
						std::vector<std::string> collision_type_str = { "Box", "Sphere", "GFK"};
						std::vector<int> collider_type = { Components::BoxCollider::CT_BOX,
																							 Components::BoxCollider::CT_SPHERE,
																							 Components::BoxCollider::CT_GJK };
						auto box_rule = box_collider->collisionLayer;
						ImGui::Text("Collision Rule");
						if (ImGui::BeginCombo("Crule", collision_rule_str[box_rule].c_str(), flag))
						{
							for (auto& it : collision_rule)
							{
								const bool  is_selected = (box_rule == it);
								if (ImGui::Selectable(collision_rule_str[it].c_str(), is_selected))
								{
									box_rule = it;
									box_collider->collisionLayer = collision_rule[box_rule];
								}

								if (is_selected)
									ImGui::SetItemDefaultFocus();
							}
							ImGui::EndCombo();
						}

						auto box_type = box_collider->colliderType;
						ImGui::Text("Collision Type");
						if (ImGui::BeginCombo("CType", collision_type_str[box_type].c_str(), flag))
						{
							for (auto& it : collider_type)
							{
								const bool  is_selected = (box_type == it);
								if (ImGui::Selectable(collision_type_str[it].c_str(), is_selected))
								{
									box_type = it;
									box_collider->colliderType = collider_type[box_type];
								}

								if (is_selected)
									ImGui::SetItemDefaultFocus();
							}
							ImGui::EndCombo();
						}


						ImGui::Text("Extend");
						ImGui::DragFloat3("###CExt", glm::value_ptr(box_collider->extends));

						ImGui::Text("Centre");
						ImGui::DragFloat3("###CCen", glm::value_ptr(box_collider->centre));

						ImGui::Text("Trigger?");
						ImGui::Checkbox("Trig?", &box_collider->isTrigger);

					}
					if (!enable_box_collider)
					{
						m_ecs->GetReg().remove<Components::Rigidbody>(m_CurrentEntity);
					}
				}

	
				ImGui::Separator();
				ImguiBlankSeperator(2);
				ImGui::Indent(ImGui::GetContentRegionAvail().x * 0.2f);
				float lineHeight = GImGui->Font->FontSize + GImGui->Style.FramePadding.y * 2.0f;
				ImVec2 buttonSize = { lineHeight + 100.0f, lineHeight };
				if (ImGui::Button("Add Component", buttonSize))
				{

				}
				ImGui::Unindent();
				ImguiBlankSeperator(2);
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

		// Update the camera when resizing window
		ImVec2 currWindowSize = ImGui::GetWindowSize();
		PP::CameraResource::GetCamera("GAME")->UpdateProjection({ currWindowSize.x,currWindowSize.y });
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
				auto debugRenderer = m_ecs->GetReg().try_get<Components::DebugRender>(entity);

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

	void ImguiHelper::Scene_GOEdit(Pogplant::Camera* _CurrCam, ImVec2 _VMin, ImVec2 _VMax)
	{
		// Draw view manipulate only in editor scene
		ImGuizmo::SetDrawlist();

		// Debug draw
		//ImGui::GetForegroundDrawList()->AddRect(_VMin, _VMax, IM_COL32(255, 255, 0, 255));

		// Bounds for guizmo
		ImGuizmo::SetRect(_VMin.x, _VMin.y, _VMax.x, _VMax.y);

		/// GUIZMO GO EDIT
		//if (m_CurrentGOIdx >= 0)
		//{
		//	GameObject& currGO = GO_Resource::m_GO_Container[m_CurrentGOIdx];
		//	// Gizmo transform, matrix to components & back
		//	ImGuizmo::RecomposeMatrixFromComponents(currGO.m_Position, currGO.m_Rotation, currGO.m_Scale, currGO.m_ModelMtx);
		//	ImGuizmo::Manipulate
		//	(
		//		glm::value_ptr(currCam->GetView()),
		//		glm::value_ptr(currCam->GetPerspective()),
		//		m_EditMode,
		//		ImGuizmo::LOCAL,
		//		currGO.m_ModelMtx,
		//		NULL,
		//		m_UseSnap ? m_SnapStep : NULL,
		//		m_BoundSizing ? m_BoundsPos : NULL,
		//		m_UseBoundsSnap ? m_BoundsSnapStep : NULL
		//	);
		//	ImGuizmo::DecomposeMatrixToComponents(currGO.m_ModelMtx, currGO.m_Position, currGO.m_Rotation, currGO.m_Scale);
		//}

		if (m_CurrentEntity != entt::null)
		{
			//GameObject& currGO = GO_Resource::m_GO_Container[m_CurrentGOIdx];

			auto& transform = m_ecs->GetReg().get<Components::Transform>(m_CurrentEntity);

			// Gizmo transform, matrix to components & back
			ImGuizmo::RecomposeMatrixFromComponents(glm::value_ptr(transform.m_position), 
													glm::value_ptr(transform.m_rotation), 
													glm::value_ptr(transform.m_scale), 
													glm::value_ptr(transform.m_ModelMtx));

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

			ImGuizmo::DecomposeMatrixToComponents(glm::value_ptr(transform.m_ModelMtx),
													glm::value_ptr(transform.m_position),
													glm::value_ptr(transform.m_rotation),
													glm::value_ptr(transform.m_scale));
		}
	}

	void ImguiHelper::Scene_ViewEdit(Pogplant::Camera* _CurrCam, ImVec2 _VMin, ImVec2 _VMax)
	{
		float view[16] = { 0 };
		float front[3] = { 0 };
		memcpy(view, glm::value_ptr(_CurrCam->View()), sizeof(_CurrCam->View()));
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

		//if (!draw_childen && ecs_handler.IsChildren(entity))
		if (!draw_childen && _r && _r->m_parent != entt::null)
			return false;

		//printf("id: %d\n", entity);

		//auto name = ecs_handler.GetComponent<ECS::ObjectName>(entity).name.c_str();



		auto name = m_ecs->GetReg().get<Components::Name>(entity);
		std::string obj_name = name.m_name;
		ImGuiTreeNodeFlags flags = (m_CurrentEntity == entity) ? ImGuiTreeNodeFlags_Selected : 0;
		flags |= ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_OpenOnDoubleClick;

		bool is_opened = ImGui::TreeNodeEx((void*)(uint64_t)entity, flags, obj_name.c_str());

		if (ImGui::IsItemClicked() || ImGui::IsMouseClicked(1) && ImGui::IsItemHovered(ImGuiHoveredFlags_RectOnly))
			m_CurrentEntity = entity;

		bool is_deleted = false;

		//if (m_CurrentEntity != entt::null && ImGui::BeginPopupContextItem())
		//{

		//	if (ImGui::MenuItem("Delete Entity"))
		//		is_deleted = true;

		//	//if (ImGui::MenuItem("Save As Prefab"))
		//	//{
		//	//	auto path = WindowsFileDialogue::SaveFileDialogue("Prefab (*.prefab)\0*.prefab\0", "untitled.prefab");
		//	//	if (path.has_value())
		//	//	{
		//	//		GameObjectFactory::SavePrefab(ecs_handler, path.value(), m_selectedEntity);
		//	//	}
		//	//}

		//	ImGui::EndPopup();
		//}

		if (is_opened)
		{
			std::string c_name = "<no children>";
			ImGuiTreeNodeFlags flags2 = ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_SpanAvailWidth;
			
			(void)flags2;

			//auto _r = m_ecs->GetReg().try_get<Components::Relationship>(entity);

			if (_r)
			{
				std::set<entt::entity> s = _r->m_children;
				//int i = 0;
				for (const auto& ent : s)
				{
					//std::string new_c_name = ecs_handler.GetComponent<ECS::ObjectName>(ent).name.c_str();
					//bool active = ImGui::TreeNodeEx((void*)(uint64_t)ent, flags, new_c_name.c_str(), i++);
					DrawEntityNode(ent, true);
					//if (ImGui::IsItemClicked())
					//	m_selectedEntity = ent;
					//if (active)
					//	ImGui::TreePop();
				}
			}
			else
			{
				bool is_opened2 = ImGui::TreeNodeEx((void*)9817239, flags2, c_name.c_str());
				if (is_opened2)
					ImGui::TreePop();
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


}