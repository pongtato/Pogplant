#include "ImguiHelper.h"

#include "Pogplant.h"

#include <imgui.h>
#include <imgui_internal.h>
#include <imgui_impl_glfw.h>
#include <imgui_impl_opengl3.h>
#include <ImGuiExtraStyles.h>

namespace PogplantDriver
{
	bool ImguiHelper::InitImgui()
	{
		IMGUI_CHECKVERSION();
		ImGui::CreateContext();
		ImGuiIO& io = ImGui::GetIO(); (void)io;
		io.Fonts->AddFontFromFileTTF("../Pogplant Driver/Fonts/Ruda.ttf", 14);
		io.ConfigFlags |= ImGuiConfigFlags_DockingEnable; // Enable Docking
		//ImGui::StyleColorsDark();
		imgui_extra_styles::Eggplant();
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

		// render your GUI
		ImGuiWindowFlags window_flags = ImGuiWindowFlags_NoDecoration | ImGuiWindowFlags_NoBringToFrontOnFocus;

		// Window
		ImVec2 winSize = ImVec2(static_cast<float>(PP::Window::m_Width), static_cast<float>(PP::Window::m_Height));
		ImGui::SetNextWindowSize(winSize);
		ImGui::SetNextWindowPos(ImVec2(0, 0));
		ImGui::Begin("Scene", 0, window_flags);
		{
			ImGui::Text("Application average %.3f ms/frame (%.1f FPS)", 1000.0f / ImGui::GetIO().Framerate, ImGui::GetIO().Framerate);
			if (ImGui::BeginTabBar("Tabs"))
			{
				if (ImGui::BeginTabItem("Editor"))
				{
					ImGui::Image(PP::FBR::m_FrameBuffers[PP::BufferType::EDITOR_COLOR_BUFFER], ImGui::GetContentRegionAvail(), ImVec2(0, 1), ImVec2(1, 0));
					ImGui::EndTabItem();
				}

				if (ImGui::BeginTabItem("Game"))
				{
					ImGui::Image(PP::FBR::m_FrameBuffers[PP::BufferType::GAME_COLOR_BUFFER], ImGui::GetContentRegionAvail(), ImVec2(0, 1), ImVec2(1, 0));
					ImGui::EndTabItem();
				}

				if (ImGui::BeginTabItem("PP_Normal"))
				{
					ImGui::Image(PP::FBR::m_FrameBuffers[PP::BufferType::PP_COLOR_BUFFER_NORMAL], ImGui::GetContentRegionAvail(), ImVec2(0, 1), ImVec2(1, 0));
					ImGui::EndTabItem();
				}

				if (ImGui::BeginTabItem("PP_Bright"))
				{
					ImGui::Image(PP::FBR::m_FrameBuffers[PP::BufferType::PP_COLOR_BUFFER_BRIGHT], ImGui::GetContentRegionAvail(), ImVec2(0, 1), ImVec2(1, 0));
					ImGui::EndTabItem();
				}

				if (ImGui::BeginTabItem("Blur_0"))
				{
					ImGui::Image(PP::FBR::m_FrameBuffers[PP::BufferType::BLUR_COLOR_BUFFER_0], ImGui::GetContentRegionAvail(), ImVec2(0, 1), ImVec2(1, 0));
					ImGui::EndTabItem();
				}

				if (ImGui::BeginTabItem("Blur_1"))
				{
					ImGui::Image(PP::FBR::m_FrameBuffers[PP::BufferType::BLUR_COLOR_BUFFER_1], ImGui::GetContentRegionAvail(), ImVec2(0, 1), ImVec2(1, 0));
					ImGui::EndTabItem();
				}
				ImGui::EndTabBar();
			}
		}
		ImGui::End();

		ImGui::ShowDemoWindow();

		//// Render dear imgui into screen
		ImGui::Render();
		ImGui_ImplOpenGL3_RenderDrawData(ImGui::GetDrawData());
	}

	void ImguiHelper::CleanUpImgui()
	{
		ImGui_ImplOpenGL3_Shutdown();
		ImGui_ImplGlfw_Shutdown();
		ImGui::DestroyContext();
	}
}