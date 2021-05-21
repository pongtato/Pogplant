#include "ImguiHelper.h"

#include "Pogplant.h"

#include <imgui.h>
#include <imgui_internal.h>
#include <imgui_impl_glfw.h>
#include <imgui_impl_opengl3.h>
#include <ImGuiExtraStyles.h>

namespace PogplantDriver
{
	bool ImguiHelper::m_FirstRun = true;

	bool ImguiHelper::InitImgui()
	{
		IMGUI_CHECKVERSION();
		ImGui::CreateContext();
		ImGuiIO& io = ImGui::GetIO(); (void)io;
		io.Fonts->AddFontFromFileTTF("../Pogplant Driver/Fonts/Ruda.ttf", 14);
		io.ConfigFlags |= ImGuiConfigFlags_DockingEnable; // Enable Docking
		imgui_extra_styles::Pogplant();
		ImGui_ImplGlfw_InitForOpenGL(PP::Window::GetWindow(), true);
		ImGui_ImplOpenGL3_Init();

		return true;
	}

	void ImguiHelper::DrawImgui()
	{
		/// IMGUI start
		ImGui_ImplOpenGL3_NewFrame();
		ImGui_ImplGlfw_NewFrame();
		ImGui::NewFrame();
		
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
				ImGui::TableSetupScrollFreeze(0, 1); // Make top row always visible
				ImGui::TableSetupColumn("Source", ImGuiTableColumnFlags_None, 0.2f);
				ImGui::TableSetupColumn("Description", ImGuiTableColumnFlags_None);
				ImGui::TableHeadersRow();

				for (auto& it : PP::Logger::m_Logs)
				{
					const char* typeText;
					switch (it.second.m_Type)
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
					ImGui::Text(it.second.m_Source.c_str());
					ImGui::TableSetColumnIndex(1);
					ImGui::Text(it.second.m_Description.c_str());
					ImGui::PopStyleColor();
				}

				ImGui::EndTable();
			}
		}
		ImGui::End();

		ImGui::Begin("Scene Hierarchy");
		{
			ImGui::Text("Scene Hierarchy");
		}
		ImGui::End();

		ImGui::Begin("Inspector");
		{
			ImGui::Text("Inspector");
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
		ImGui::PushStyleColor(0, ImVec4{ 0.55f,0.8f,0.2f,1 });
		ImGui::Text("Application average %.3f ms/frame (%.1f FPS)", 1000.0f / ImGui::GetIO().Framerate, ImGui::GetIO().Framerate);
		ImGui::PopStyleColor();

		ImVec2 currCursorPos = ImGui::GetCursorPos();
		ImGui::Image(PP::FBR::m_FrameBuffers[PP::BufferType::EDITOR_COLOR_BUFFER], ImGui::GetContentRegionAvail(), ImVec2(0, 1), ImVec2(1, 0));

		// Update the camera when resizing window
		ImVec2 currWindowSize = ImGui::GetWindowSize();
		PP::CameraResource::GetCamera("EDITOR")->UpdateProjection({ currWindowSize.x,currWindowSize.y });

		ImGui::SetCursorPos(ImVec2{ ImGui::GetWindowSize().x - 218.0f, currCursorPos.y - 40.0f });
		ImGui::Image(PP::FBR::m_FrameBuffers[PP::BufferType::GIZMO_COLOR_BUFFER], ImVec2{ 320.0f,180.0f }, ImVec2(0, 1), ImVec2(1, 0));

		const float radius = 10.0f;
		PP::CameraConfig eCameraConfig = PP::CameraResource::GetCamera("EDITOR")->mCameraConfig();
		float camX = cos(glm::radians(eCameraConfig.m_Yaw) + glm::radians(180.0f)) * cos(glm::radians(eCameraConfig.m_Pitch)) * radius;
		float camY = sin(-glm::radians(eCameraConfig.m_Pitch)) * radius;
		float camZ = sin(glm::radians(eCameraConfig.m_Yaw) + glm::radians(180.0f)) * cos(glm::radians(eCameraConfig.m_Pitch)) * radius;
		PP::CameraResource::GetCamera("GIZMO")->UpdateView(glm::vec3(camX, camY, camZ), glm::vec3(0.0, 0.0, 0.0), glm::vec3(0.0, 1.0, 0.0));
		
		// Make sure begin is being called before this function
		// This ensures the input for camera only works when the Scene window is focused
		if(ImGui::IsWindowFocused())
		{
			PP::CameraResource::SetActiveCam("EDITOR");
		}
		else
		{
			PP::CameraResource::DeselectCam();
		}
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
}