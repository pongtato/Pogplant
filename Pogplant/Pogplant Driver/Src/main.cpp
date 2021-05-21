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
	/*for (int i = -50; i < 50; i++)
	{
		for (int j = -50; j < 50; j++)
		{
			glm::mat4 Model = glm::mat4{ 1 };
			Model = glm::translate(Model, glm::vec3(i * 0.25f, j * 0.25f, -30.0f));
			Model = glm::scale(Model, glm::vec3(0.2f, 0.2f, 0.2f));
			PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{1}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });
		}
	}*/

	PP::MeshInstance::ResetCount();

	// 3D object background to see orientation
	glm::mat4 Model = glm::mat4{ 1 };
	Model = glm::translate(Model, glm::vec3(0.0f, 0.0f, -40.0f));
	Model = glm::scale(Model, glm::vec3(20.0f, 20.0f, 20.0f));
	PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{0.69f,0.69f,0.69f,1}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

	/*Model = glm::mat4{ 1 };
	Model = glm::translate(Model, glm::vec3(800.0f, 600.0f, -5.0f));
	Model = glm::scale(Model, glm::vec3(50.0f, 50.0f, 1.0f));
	PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{0.0f,1.0f,0.0f,1}, glm::vec2{1}, glm::vec2{0}, -1, 1, 0 });*/
}

void GizmoTest()
{
	PP::MeshInstance::ResetCount();

	glm::mat4 Parent = glm::mat4{ 1 };
	Parent = glm::translate(Parent, glm::vec3(0.09f, 0.0f, -1.0f));
	Parent = glm::scale(Parent, glm::vec3(0.5f, 0.5f, 0.5f));

	/// To form a cube
	// Front
	glm::mat4 Model = glm::translate(Parent, glm::vec3(0.0f, 0.0f, 0.4f));
	PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{0,0,1,1}, glm::vec2{1}, glm::vec2{0}, -1, 1, 0 });

	// Right
	Model = glm::translate(Parent, glm::vec3(0.4f, 0, 0.0f));
	Model = glm::rotate(Model, glm::radians(90.0f), { 0, 1, 0 });
	PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{1,0,0,1}, glm::vec2{1}, glm::vec2{0}, -1, 1, 0 });

	// Left
	Model = glm::translate(Parent, glm::vec3(-0.6f, 0, 0.0f));
	Model = glm::rotate(Model, glm::radians(90.0f), { 0, 1, 0 });
	PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{1}, glm::vec2{1}, glm::vec2{0}, -1, 1, 0 });

	// Back
	Model = glm::translate(Parent, glm::vec3(0.0f, 0.0f, -0.6f));
	PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{1}, glm::vec2{1}, glm::vec2{0}, -1, 1, 0 });

	// Top
	Model = glm::translate(Parent, glm::vec3(0.0f, 0.6f, 0.0f));
	Model = glm::rotate(Model, glm::radians(90.0f), { 1, 0, 0 });
	PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{0,1,0,1}, glm::vec2{1}, glm::vec2{0}, -1, 1, 0 });

	// Bottom
	Model = glm::translate(Parent, glm::vec3(0.0f, -0.4f, 0.0f));
	Model = glm::rotate(Model, glm::radians(90.0f), { 1, 0, 0 });
	PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{1}, glm::vec2{1}, glm::vec2{0}, -1, 1, 0 });
}

void Init()
{
	PP::Window::InitWindow(
		1280,					// Width
		720,					// Height
		 "Pogplant Driver 6969" // Window name
		);
	PP::MeshBuilder::InitMesh();
	PP::ShaderLinker::InitShader();
	PP::FrameBuffer::InitFrameBuffer();
	PP::CameraResource::InitBaseCameras(
		glm::vec3{ 0,0,5.0f }, // Editor cam pos
		glm::vec3{ 0,0,0.0f }, // Game cam pos
		PP::CameraConfig{ 
			-90.0f, // Yaw
			0.0f,	// Pitch
			45.0f,	// Zoom 
			6.9f,	// Speed 
			0.1f,	// Near
			100.0f,	// Far
			0.21f,	// Mouse look sens
			20.0f,	// Key input look sens
			0.1f	// Pan speed
		});
	PPD::ImguiHelper::InitImgui();

	/// Test spawning of objects
	//ObjectTest();
}

void Run()
{
	while (!PP::Window::ShouldCloseWindow())
	{
		PP::Window::CheckForceClose(); // Temp exit using Esc

	
		ObjectTest();
		PP::MeshBuilder::RebindQuad();

		// Camera KB movement
		PP::CameraResource().UpdateActiveCamera(ImGui::GetIO().DeltaTime);

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

		GizmoTest();
		PP::MeshBuilder::RebindQuad();

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