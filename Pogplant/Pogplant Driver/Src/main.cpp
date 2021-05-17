#include "ImguiHelper.h"

#include <iostream>
#include <Pogplant.h>
#include <glm.hpp>
#include <gtc/matrix_transform.hpp>

#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#include <imgui.h>

struct MEMLEAK
{
	~MEMLEAK()
	{
		_CrtDumpMemoryLeaks();
	}
}MEMLEAK;

void ObjectTest()
{
	for (int i = -50; i < 50; i++)
	{
		for (int j = -50; j < 50; j++)
		{
			glm::mat4 Model = glm::mat4{ 1 };
			Model = glm::translate(Model, glm::vec3(i * 0.25f, j * 0.25f, -30.0f));
			Model = glm::scale(Model, glm::vec3(0.2f, 0.2f, 0.2f));
			PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{1}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });
		}
	}
}

void Init()
{
	PP::Window::InitWindow(1280, 720, "Pogplant Driver 6969");
	PP::MeshBuilder::InitMesh();
	PP::ShaderLinker::InitShader();
	PP::FrameBuffer::InitFrameBuffer();
	PP::CameraResource::InitBaseCameras(glm::vec3{ 0,0,5.0f }, glm::vec3{ 0,0,0.0f }, PP::CameraConfig{ -90.0f,0.0f,45.0f,2.0f,0.1f,100.0f });
	PPD::ImguiHelper::InitImgui();

	/// Test spawning of objects
	ObjectTest();
}

void Run()
{
	while (!PP::Window::ShouldCloseWindow())
	{
		PP::Window::CheckForceClose(); // Temp exit using Esc

		// Camera KB movement
		PP::CameraResource().UpdateActiveCamera(ImGui::GetIO().DeltaTime);

		PP::MeshBuilder::RebindQuad();

		// Editor
		PP::Renderer::StartEditorBuffer();
		PP::Renderer::ClearBuffer();
		PP::Renderer::Draw("EDITOR");
		PP::Renderer::EndBuffer();

		// Game
		PP::Renderer::StartGameBuffer();
		PP::Renderer::ClearBuffer();
		PP::Renderer::Draw("GAME");
		PP::Renderer::EndBuffer();

		// Post process
		PP::Renderer::PostProcess();
		PP::Renderer::ClearBuffer();
		PP::Renderer::DrawScreen();
		PP::Renderer::EndBuffer();

		PP::Renderer::ClearBuffer();
		PPD::ImguiHelper::DrawImgui();
		PP::Renderer::SwapBuffer();
	}
}

void Exit()
{
	PPD::ImguiHelper::CleanUpImgui();
	PP::CameraResource::CleanUpCameras();
	PP::FrameBuffer::CleanUpFrameBuffer();
	PP::MeshBuilder::CleanUpMesh();
	PP::Window::CleanUpWindow();
}

int main()
{
	Init();
	Run();
	Exit();
}