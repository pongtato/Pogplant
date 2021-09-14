#include "ImguiHelper.h"
#include "Pogplant.h"
#include "GameObjectContainer.h"

#include <imgui.h>
#include <imgui_internal.h>
#include <imgui_impl_glfw.h>
#include <imgui_impl_opengl3.h>
#include <ImGuiExtraStyles.h>
#include <ImGuizmo.h>
#include <gtc/type_ptr.hpp>

#include "ECS/Entity.h"

namespace PogplantDriver
{
	bool ImguiHelper::m_FirstRun = true;
	//int ImguiHelper::m_CurrentGOIdx = -1;
	entt::entity ImguiHelper::m_CurrentEntity = entt::null;
	ECS* ImguiHelper::m_ecs = nullptr;

	// Guizmo editor stuff
	static ImGuizmo::OPERATION m_EditMode(ImGuizmo::TRANSLATE);
	static bool m_UseSnap = false;
	static float m_SnapStep[]		= { 0.1f, 0.1f, 0.1f };
	static float m_BoundsPos[]		= { -0.5f, -0.5f, -0.5f, 0.5f, 0.5f, 0.5f };
	static float m_BoundsSnapStep[] = { 0.1f, 0.1f, 0.1f };
	static bool m_BoundSizing = false;
	static bool m_UseBoundsSnap = false;

	bool ImguiHelper::InitImgui(ECS* ecs)
	{
		IMGUI_CHECKVERSION();
		ImGui::CreateContext();
		ImGuiIO& io = ImGui::GetIO(); (void)io;
		io.Fonts->AddFontFromFileTTF("../Pogplant Driver/Fonts/Ruda.ttf", 14);
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
		if (ImGui::BeginMainMenuBar())
		{
			if (ImGui::BeginMenu("File"))
			{
				ImGui::MenuItem("(File menu)", NULL, false, false);
				if (ImGui::MenuItem("New")) {}
				ImGui::EndMenu();
			}
			if (ImGui::BeginMenu("Create"))
			{
				ImGui::MenuItem("(Create menu)", NULL, false, false);
				if (ImGui::MenuItem("Create")) {}
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

		/// MUST MATCH WINDOW NAMES ABOVE
		ImGui::Begin("Directory");
		{
			ImGui::Text("Directory");
		}
		ImGui::End();

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
			m_ecs->GetReg().each([](auto entity)
			{
				//auto e_Tag = m_ecs->GetReg().get<Components::Tag>(entity).m_tag;
					std::string name = "Object" + std::to_string((int)entity);

				if (ImGui::Selectable(name.c_str(), m_CurrentEntity == entity))
				{
					m_CurrentEntity = entity;
				}
			});


			//for (int i = 0; i < GO_Resource::m_GO_Container.size(); i++)
			//{
			//	std::string name = "Object" + std::to_string(i);
			//	if (ImGui::Selectable(name.c_str(), m_CurrentGOIdx == i))
			//	{
			//		m_CurrentGOIdx = i;
			//	}
			//}
		}
		ImGui::End();

		ImGui::Begin("Inspector");
		{
			if (m_CurrentEntity != entt::null)
			{
				//GameObject& currGO = GO_Resource::m_GO_Container[m_CurrentGOIdx];

				auto& transform = m_ecs->GetReg().get<Components::Transform>(m_CurrentEntity);

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
				ImGui::DragFloat3("", glm::value_ptr(transform.m_position));
				ImGui::PopID();
				ImGui::Text("Rotate");
				ImGui::PushID("Rt");
				//ImGui::DragFloat3("", currGO.m_Rotation);
				ImGui::DragFloat3("", glm::value_ptr(transform.m_rotation));
				ImGui::PopID();
				ImGui::Text("Scale");
				ImGui::PushID("Sc");
				//ImGui::DragFloat3("", currGO.m_Scale);
				ImGui::DragFloat3("", glm::value_ptr(transform.m_scale));
				ImGui::PopID();
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

		/// Picker
		Scene_GOPick(currCam, vMin, vMax);

		/// GUIZMO GO EDIT
		Scene_GOEdit(currCam, vMin, vMax);

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

	void ImguiHelper::Scene_GOPick(Pogplant::Camera* _CurrCam, ImVec2 _VMin, ImVec2 _VMax)
	{
		if (!ImGui::IsWindowFocused())
		{
			return;
		}

		if (ImGui::IsMouseClicked(ImGuiMouseButton_Left))
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
					auto& renderer = m_ecs->GetReg().get<Components::Renderer>(entity);
					// Naive approach
					float largestScale = std::numeric_limits<float>::min();

					for (int j = 0; j < 3; j++)
					{
						largestScale = std::max(largestScale, transform.m_scale[j]);
					}

					const float radius = renderer.render_object->m_RenderModel->m_Bounds.longest * 0.5f * largestScale;
					float currentTime = std::numeric_limits<float>::max();
					if (ray.CollideSphere(glm::make_vec3(transform.m_position), radius, currentTime))
					{
						if (currentTime < shortestTime)
						{
							chosenObject = entity;
							shortestTime = currentTime;
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
													transform.m_ModelMtx);
			ImGuizmo::Manipulate
			(
				glm::value_ptr(_CurrCam->GetView()),
				glm::value_ptr(_CurrCam->GetPerspective()),
				m_EditMode,
				ImGuizmo::LOCAL,
				transform.m_ModelMtx,
				NULL,
				m_UseSnap ? m_SnapStep : NULL,
				m_BoundSizing ? m_BoundsPos : NULL,
				m_UseBoundsSnap ? m_BoundsSnapStep : NULL
			);
			ImGuizmo::DecomposeMatrixToComponents(	transform.m_ModelMtx, 
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
}
