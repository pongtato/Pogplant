#include "ImguiHelper.h"
#include "GameObjectContainer.h"

#include <iostream>
#include <Pogplant.h>
#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>

#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#include <imgui.h>

//struct MEMLEAK
//{
//	~MEMLEAK()
//	{
//		_CrtDumpMemoryLeaks();
//	}
//}MEMLEAK;

namespace PPD = PogplantDriver;

void Init()
{
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);

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
			16.9f,	// Speed 
			0.1f,	// Near
			100.0f,	// Far
			0.21f,	// Mouse look sens
			20.0f,	// Key input look sens
			0.1f	// Pan speed
		});
	PPD::ImguiHelper::InitImgui();

	/// Test spawning of objects
	//ObjectTest();

	/// Add to container
	PP::Model* currModel = PP::ModelResource::m_ModelPool["BAG"];
	// Assume 2 objects components
	GO_Resource::m_Render_Container.push_back({ glm::mat4{1}, currModel });
	GO_Resource::m_Render_Container.push_back({ glm::mat4{1}, currModel });

	glm::vec3 pos = { 5.0f, 0.0f, -10.0f };
	glm::vec3 rot = { 0.0f,0.0f,0.0f };
	glm::vec3 scale = { 1.0f,1.0f,1.0f };
	GO_Resource::m_GO_Container.push_back(GameObject(pos, rot, scale, &GO_Resource::m_Render_Container[0]));

	pos = { -5.0f, 0.0f, -10.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 2.0f,2.0f,2.0f };
	GO_Resource::m_GO_Container.push_back(GameObject(pos, rot, scale, &GO_Resource::m_Render_Container[1]));

	std::cout << "PROGRAM STARTED, USE THE EDITOR'S DEBUGGER" << std::endl;
}

void TestCube()
{
	for (const auto& go : GO_Resource::m_GO_Container)
	{
		glm::mat4 Parent = glm::mat4{ 1 };
		Parent = glm::make_mat4(go.m_ModelMtx);

		// Front
		glm::mat4 Model = glm::translate(Parent, glm::vec3(0.0f, 0.0f, 0.5f));
		PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{0.0f,0.0f,1.0f,1.0f}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

		// Right
		Model = glm::translate(Parent, glm::vec3(0.5f, 0, 0.0f));
		Model = glm::rotate(Model, glm::radians(90.0f), { 0, 1, 0 });
		PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{1.0f,0.0f,0.0f,1.0f}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

		// Left
		Model = glm::translate(Parent, glm::vec3(-0.5f, 0, 0.0f));
		Model = glm::rotate(Model, glm::radians(90.0f), { 0, 1, 0 });
		PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{1.0f,0.2f,0.6f,1.0f}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

		// Back
		Model = glm::translate(Parent, glm::vec3(0.0f, 0.0f, -0.5f));
		PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{0.2f,0.6f,1.0f,1.0f}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

		// Top
		Model = glm::translate(Parent, glm::vec3(0.0f, 0.5f, 0.0f));
		Model = glm::rotate(Model, glm::radians(90.0f), { 1, 0, 0 });
		PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{0.0f,1.0f,0.0f,1.0f}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

		// Bottom
		Model = glm::translate(Parent, glm::vec3(0.0f, -0.5f, 0.0f));
		Model = glm::rotate(Model, glm::radians(90.0f), { 1, 0, 0 });
		PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{0.6f,1.0f,0.2f,1.0f}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });
	}
}

void DrawCommon()
{
	PP::MeshInstance::ResetCount();
	// 3D object background to see orientation
	glm::mat4 Model = glm::mat4{ 1 };
	Model = glm::translate(Model, glm::vec3(0.0f, 0.0f, -40.0f));
	Model = glm::scale(Model, glm::vec3(20.0f, 20.0f, 20.0f));
	PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{0.69f,0.69f,0.69f,1}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

	//TestCube();

	/// TEMP - Update transforms for render
	for (size_t i = 0; i < GO_Resource::m_GO_Container.size(); i++)
	{
		const auto& go = GO_Resource::m_GO_Container[i];
		GO_Resource::m_Render_Container[i].m_Model = glm::make_mat4(go.m_ModelMtx);
	}

	PP::MeshBuilder::RebindQuad();
}

void DrawEditor()
{
	// If something is selected choose it to be highlighted
	PP::RenderObject* renderOjbect = nullptr;
	const int currIdx = PPD::ImguiHelper::m_CurrentGOIdx;
	if (currIdx >= 0)
	{
		renderOjbect = GO_Resource::m_GO_Container[currIdx].m_RenderObject;
	}
	PP::Renderer::StartEditorBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::Draw("EDITOR", GO_Resource::m_Render_Container, renderOjbect);
	PP::Renderer::EndBuffer();
}

void DrawGame()
{
	PP::Renderer::StartGameBuffer();
	PP::Renderer::ClearBuffer();
	// Dont highlight in game scene so leave 3rd param as nullptr
	PP::Renderer::Draw("GAME", GO_Resource::m_Render_Container, nullptr);
	PP::Renderer::EndBuffer();
}

void DrawScreen()
{
	PP::Renderer::PostProcess();
	PP::Renderer::ClearBuffer();
	PP::Renderer::DrawScreen();
	PP::Renderer::EndBuffer();
}

void DrawImGUI()
{
	PP::Renderer::ClearBuffer();
	PPD::ImguiHelper::DrawImgui();
}

void Run()
{
	while (!PP::Window::ShouldCloseWindow())
	{
		PP::Window::CheckForceClose(); // Temp exit using Esc

		// Camera KB movement
		PP::CameraResource().UpdateActiveCamera(ImGui::GetIO().DeltaTime);

		/// Most of this should be moved to other files when the engine is developed
		// Things that appear in both editor & game
		DrawCommon();
		// Editor
		DrawEditor();
		// Game
		DrawGame();
		// Post process
		DrawScreen();
		// ImGUI
		DrawImGUI();
		///

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