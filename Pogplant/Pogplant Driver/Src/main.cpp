#include "ImguiHelper.h"
#include "GameObjectContainer.h"
#include "ScriptSystem.h"

#include <iostream>
#include <Pogplant.h>
#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>

#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#include <imgui.h>

#include "ECS/Entity.h"
//struct MEMLEAK
//{
//	~MEMLEAK()
//	{
//		_CrtDumpMemoryLeaks();
//	}
//}MEMLEAK;


namespace PPD = PogplantDriver;

using namespace Components;
ECS ecs;

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
	PPD::ImguiHelper::InitImgui(&ecs);

	/// Add to container
	PP::Model* bagModel = PP::ModelResource::m_ModelPool["BAG"];
	PP::Model* cubeModel = PP::ModelResource::m_ModelPool["SPHERE"];
	// Assume 2 objects components
	
	GO_Resource::m_Render_Container.push_back({ glm::mat4{1}, bagModel });
	GO_Resource::m_Render_Container.push_back({ glm::mat4{1}, cubeModel });

	glm::vec3 pos = { 5.0f, 0.0f, -10.0f };
	glm::vec3 rot = { 0.0f,0.0f,0.0f };
	glm::vec3 scale = { 1.0f,1.0f,1.0f };
	//GO_Resource::m_GO_Container.push_back(GameObject(pos, rot, scale, &GO_Resource::m_Render_Container[0]));
	
	auto entity = ecs.CreateEntity();
	entity.AddComponent<Components::Transform>(pos, rot, scale);
	entity.AddComponent<Components::Renderer>(&GO_Resource::m_Render_Container[0]);

	//ecs.AddComponent<Components::Renderer>(entity, &bagModel);
	//registry.emplace<Renderer>(entity, &bagModel);

	pos = { -5.0f, 0.0f, -10.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 2.0f,2.0f,2.0f };
	//GO_Resource::m_GO_Container.push_back(GameObject(pos, rot, scale, &GO_Resource::m_Render_Container[1]));
	
	entity = ecs.CreateEntity();
	entity.AddComponent<Components::Transform>(pos, rot, scale);
	entity.AddComponent<Components::Renderer>(&GO_Resource::m_Render_Container[1]);


	//auto entity = registry.create();
	//registry.emplace<Transform>(registry.create(), pos, rot, scale);
	//registry.emplace<Renderer>(entity,&cubeModel);

	std::cout << "PROGRAM STARTED, USE THE EDITOR'S DEBUGGER" << std::endl;
	
	// Nazi?
	ScriptSystem SS;
	SS.testfuncwithparam('X');
	SS.testfuncwithreturn();
}

void SetCube(glm::mat4 _BasePos)
{
	// Front
	glm::mat4 model = glm::translate(_BasePos, glm::vec3(0.0f, 0.0f, 0.5f));
	PP::MeshInstance::SetInstance(PP::InstanceData{ model, glm::vec4{0.0f,0.0f,1.0f,1.0f}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

	// Right
	model = glm::translate(_BasePos, glm::vec3(0.5f, 0, 0.0f));
	model = glm::rotate(model, glm::radians(90.0f), { 0, 1, 0 });
	PP::MeshInstance::SetInstance(PP::InstanceData{ model, glm::vec4{1.0f,0.0f,0.0f,1.0f}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

	// Left
	model = glm::translate(_BasePos, glm::vec3(-0.5f, 0, 0.0f));
	model = glm::rotate(model, glm::radians(90.0f), { 0, 1, 0 });
	PP::MeshInstance::SetInstance(PP::InstanceData{ model, glm::vec4{1.0f,0.2f,0.6f,1.0f}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

	// Back
	model = glm::translate(_BasePos, glm::vec3(0.0f, 0.0f, -0.5f));
	PP::MeshInstance::SetInstance(PP::InstanceData{ model, glm::vec4{0.2f,0.6f,1.0f,1.0f}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

	// Top
	model = glm::translate(_BasePos, glm::vec3(0.0f, 0.5f, 0.0f));
	model = glm::rotate(model, glm::radians(90.0f), { 1, 0, 0 });
	PP::MeshInstance::SetInstance(PP::InstanceData{ model, glm::vec4{0.0f,1.0f,0.0f,1.0f}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

	// Bottom
	model = glm::translate(_BasePos, glm::vec3(0.0f, -0.5f, 0.0f));
	model = glm::rotate(model, glm::radians(90.0f), { 1, 0, 0 });
	PP::MeshInstance::SetInstance(PP::InstanceData{ model, glm::vec4{0.6f,1.0f,0.2f,1.0f}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });
}

void DebugCubes(const GameObject& _GO)
{
	float largestScale = std::numeric_limits<float>::min();
	for (int i = 0; i < 3; i++)
	{
		largestScale = std::max(largestScale, _GO.m_Scale[i]);
	}

	const float halfLen = _GO.m_RenderObject->m_RenderModel->m_Bounds.longest * largestScale * 0.5f;
	glm::mat4 base = glm::mat4{ 1 };
	glm::vec3 trans = glm::make_vec3(_GO.m_Position);
	base = glm::translate( base, trans );
	base = glm::scale( base, {1.0f,1.0f,1.0f } );

	// Top 
	glm::mat4 model = glm::translate(base, { 0,halfLen,0 });
	SetCube(model);
	// Left
	model = glm::translate(base, { -halfLen,0,0 });
	SetCube(model);
	// Right
	model = glm::translate(base, { halfLen,0,0 });
	SetCube(model);
	// Front
	model = glm::translate(base, { 0,0,-halfLen });
	SetCube(model);
	// Back
	model = glm::translate(base, { 0,0,halfLen });
	SetCube(model);
	// Bottom
	model = glm::translate(base, { 0,-halfLen,0 });
	SetCube(model);
}

void DebugCubes(Transform& transform, Renderer& renderer)
{
	float largestScale = std::numeric_limits<float>::min();

	for (int i = 0; i < 3; i++)
	{
		largestScale = std::max(largestScale, transform.m_scale[i]);
	}
	
	const float halfLen = renderer.render_object->m_RenderModel->m_Bounds.longest * largestScale * 0.5f;
	glm::mat4 base = glm::mat4{ 1 };
	glm::vec3 trans = glm::make_vec3(transform.m_position);
	base = glm::translate(base, trans);
	base = glm::scale(base, { 1.0f,1.0f,1.0f });

	// Top 
	glm::mat4 model = glm::translate(base, { 0,halfLen,0 });
	SetCube(model);
	// Left
	model = glm::translate(base, { -halfLen,0,0 });
	SetCube(model);
	// Right
	model = glm::translate(base, { halfLen,0,0 });
	SetCube(model);
	// Front
	model = glm::translate(base, { 0,0,-halfLen });
	SetCube(model);
	// Back
	model = glm::translate(base, { 0,0,halfLen });
	SetCube(model);
	// Bottom
	model = glm::translate(base, { 0,-halfLen,0 });
	SetCube(model);
}

void DrawCommon()
{
	PP::MeshInstance::ResetCount();
	// 3D object background to see orientation
	glm::mat4 Model = glm::mat4{ 1 };
	Model = glm::translate(Model, glm::vec3(0.0f, 0.0f, -40.0f));
	Model = glm::scale(Model, glm::vec3(20.0f, 20.0f, 20.0f));
	PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{0.69f,0.69f,0.69f,1}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

	/// TEMP - Update transforms for render
	auto view = ecs.GetReg().view<Transform, Renderer>();
	
	for (auto entity : view)
	{
		auto& transform = view.get<Transform>(entity);
		auto& renderer = view.get<Renderer>(entity);
		
		renderer.render_object->m_Model = glm::make_mat4(transform.m_ModelMtx);
		DebugCubes(transform, renderer);
	}




	//for (size_t i = 0; i < GO_Resource::m_GO_Container.size(); i++)
	//{
	//	const auto& go = GO_Resource::m_GO_Container[i];
	//	go.m_RenderObject->m_Model = glm::make_mat4(go.m_ModelMtx);
	//	DebugCubes(go);
	//}

	PP::MeshBuilder::RebindQuad();
}

void DrawEditor()
{
	// If something is selected choose it to be highlighted
	PP::RenderObject* renderOjbect = nullptr;

	//const int currIdx = PPD::ImguiHelper::m_CurrentGOIdx;
	entt::entity currIdx = PPD::ImguiHelper::m_CurrentEntity;

	//if (currIdx >= 0)
	if (currIdx != entt::null)
	{
		//renderOjbect = GO_Resource::m_GO_Container[currIdx].m_RenderObject;
		renderOjbect = ecs.GetReg().get<Renderer>(currIdx).render_object;
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
	//testing ecs stuffs

	Init();
	Run();
	Exit();
}
