#include "ImguiHelper.h"
#include "ScriptSystem.h"

#include <iostream>
#include <Pogplant.h>
#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>
#define NOMINMAX
#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#include <imgui.h>

#include "ECS/Entity.h"
#include "ECS/Systems/imaginary_system.h"

#include "Input/InputSystem.h"
#include "ResourceAllocator.hpp"
#include "AssetCompiler.h"
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
Imaginary_system ImaginarySystem;

void Init()
{
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);

	PP::Window::InitWindow(
		1280,					// Width
		720,					// Height
		"Pogplant Driver 6969" // Window name
	);

	
	// Will change to automatic update when i figure it out
	AssetCompiler& acc = acc.GetInstance();
	if(!acc.Exists("Resources\\KekFiles\\Cube.kek"))
		acc.RunExecutable("CUBE", "Pogplant Compiler.exe", "Resources\\Models\\cube\\Cube.fbx");
	if (!acc.Exists("Resources\\KekFiles\\Sphere.kek"))
		acc.RunExecutable("SPHERE", "Pogplant Compiler.exe", "Resources\\Models\\sphere\\Sphere.fbx");
	if (!acc.Exists("Resources\\KekFiles\\Enemy_01.kek"))
		acc.RunExecutable("ENEMY", "Pogplant Compiler.exe", "Resources\\Models\\Enemy\\Enemy_01.fbx");
	if (!acc.Exists("Resources\\KekFiles\\Player_Ship.kek"))
		acc.RunExecutable("SHIP", "Pogplant Compiler.exe", "Resources\\Models\\Ship\\Player_Ship.fbx");
	if (!acc.Exists("Resources\\KekFiles\\backpack.kek"))
		acc.RunExecutable("BAG", "Pogplant Compiler.exe", "Resources\\Models\\backpack\\backpack.obj");
	acc.WaitForAllProcess();
	//acc.WaitForSingleProcess("BAG");

	PP::TextureResource::InitResource();
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
			200.0f,	// Far
			0.21f,	// Mouse look sens
			20.0f,	// Key input look sens
			0.1f	// Pan speed
		});
	PPD::ImguiHelper::InitImgui(&ecs);
	

	/// Add to container
	PP::Model* bagModel = PP::ModelResource::m_ModelPool["BAG"];
	PP::Model* sphereModel = PP::ModelResource::m_ModelPool["SPHERE"];
	PP::Model* floorModel = PP::ModelResource::m_ModelPool["CUBE"];
	PP::Model* shipModel = PP::ModelResource::m_ModelPool["SHIP"];
	PP::Model* enemyModel = PP::ModelResource::m_ModelPool["ENEMY"];
	
	/* CLARENCE DEBUGGING STUFF */
	//std::cout << "number of meshes: " << cubeModel->m_Meshes.size() << std::endl;
	//std::cout << "How many meshes: " << bagModel->m_Meshes.size() << std::endl;
	//std::cout << "How many textures loaded: " << bagModel->m_TexturesLoaded.size() << std::endl;

	//for (auto& meshes : bagModel->m_Meshes)
	//{
	//	
	//	for (auto& vert : meshes.m_Vertices)
	//	{
	//		std::cout
	//			// Position
	//			<< vert.m_Position.x << ' ' << vert.m_Position.y << ' ' << vert.m_Position.z << ' '
	//			// Normal
	//			<< vert.m_Normal.x << ' ' << vert.m_Normal.y << ' ' << vert.m_Normal.z << ' '
	//			// Tangent
	//			<< vert.m_Tangent.x << ' ' << vert.m_Tangent.y << ' ' << vert.m_Tangent.z << ' '
	//			// Bi-Tangent
	//			<< vert.m_BiTangent.x << ' ' << vert.m_BiTangent.y << ' ' << vert.m_BiTangent.z << ' '
	//			// Tex-Coords
	//			<< vert.m_TexCoords.x << ' ' << vert.m_TexCoords.y <<
	//		std::endl;
	//	}

	//	for (auto& idx : meshes.m_Indices)
	//	{
	//		std::cout << idx << std::endl;
	//	}

	//	for (auto& tex : meshes.m_Textures)
	//	{
	//		std::cout << tex.m_Path << ' ' << tex.m_Type << std::endl;
	//	}
	//	
	//	std::cout << "Number of vert: " << meshes.m_Vertices.size() << std::endl;
	//	std::cout << "Number of idx: " << meshes.m_Indices.size() << std::endl;
	//	std::cout << "Number of texs: " << meshes.m_Textures.size() << std::endl;
	//}

	//for (auto& texLoaded : bagModel->m_TexturesLoaded)
	//{
	//	std::cout << texLoaded.m_Path << ' ' << texLoaded.m_Type << std::endl;
	//}
	//std::cout << cubeModel->m_Directory << std::endl;
	/* END OF CLARENCE DEBUGGING STUFF*/

	// Assume 2 objects components
	//GO_Resource::m_Render_Container.push_back({ glm::mat4{1}, bagModel });
	//GO_Resource::m_Render_Container.push_back({ glm::mat4{1}, cubeModel });

	glm::vec3 pos = { 5.0f, 0.0f, -10.0f };
	glm::vec3 rot = { 0.0f,0.0f,0.0f };
	glm::vec3 scale = { 1.0f,1.0f,1.0f };
	glm::vec3 color = { 0.835f,0.921f,0.905f }; // In the event of no texture
	//GO_Resource::m_GO_Container.push_back(GameObject(pos, rot, scale, &GO_Resource::m_Render_Container[0]));
	
	auto entity = ecs.CreateEntity("", pos, rot, scale);
	//entity.AddComponent<Components::Transform>(pos, rot, scale);
	entity.AddComponent<Components::Render>(Render{ glm::mat4{1}, color, bagModel });
	//entity.AddComponent<Components::Name>(Name{ "Bag" });

	//ecs.AddComponent<Components::Renderer>(entity, &bagModel);
	//registry.emplace<Renderer>(entity, &bagModel);

	pos = { -5.0f, 0.0f, -10.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 2.0f,2.0f,2.0f };
	//GO_Resource::m_GO_Container.push_back(GameObject(pos, rot, scale, &GO_Resource::m_Render_Container[1]));
	
	entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Render>(Render{ glm::mat4{1}, color, sphereModel });
	entity.AddComponent<Imaginary_object>("gab_small_pepe");
	//entity.AddComponent<Components::Name>(Name{ "Sphere Test" });

	pos = { 0.0f, -55.0f, 0.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 100.0f,100.0f,100.0f };
	entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Render>(Render{ glm::mat4{1}, color, floorModel });
	//entity.AddComponent<Components::Name>(Name{ "Floor" });

	pos = { 5, -2.0f, 10.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 1.0f,1.0f,1.0f };

	entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Render>(Render{ glm::mat4{1}, color, shipModel });
	//entity.AddComponent<Components::Name>(Name{ "Ship" });

	pos = { -10.0f, -2.0f, 10.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 1.0f,1.0f,1.0f };

	entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Render>(Render{ glm::mat4{1}, color, enemyModel });
	//entity.AddComponent<Components::Name>(Name{ "Enemy" });

	//auto entity = registry.create();
	//registry.emplace<Transform>(registry.create(), pos, rot, scale);
	//registry.emplace<Renderer>(entity,&cubeModel);

	/// Light
	pos = { 0.0f, 1000.0f, 0.0f };
	glm::vec3 direction = {-0.5f,-0.5f,-0.5f};
	scale = { 1.0f,1.0f,1.0f }; // Affects light model and not the actual light size
	float intensity = 0.5f;
	entity = ecs.CreateEntity("", pos, glm::vec3{0}, scale);
	entity.AddComponent<Components::Directional_Light>(Directional_Light{ color, intensity, direction , 0.42f, 0.69f });
	entity.AddComponent<Components::Render>(Render{ glm::mat4{1}, color, sphereModel, false });
	//entity.AddComponent<Components::Name>(Name{ "Directional"});

	pos = { 0.0f, 10.0f, 0.0f };
	color = { 1.0f, 1.0f, 1.0f };
	const float linear = 0.00069f;
	const float quadratic = 0.0042f;
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	entity.AddComponent<Components::Render>(Render{ glm::mat4{1}, color, sphereModel, false });
	//entity.AddComponent<Components::Name>(Name{ "Light 1" });

	intensity = 1.0f;
	pos = { -10.0f, 10.0f, 10.0f };
	color = { 0.0f, 0.0f, 1.0f };
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	entity.AddComponent<Components::Render>(Render{ glm::mat4{1}, color, sphereModel, false });
	//entity.AddComponent<Components::Name>(Name{ "Light 2" });

	pos = { 10.0f, 10.0f, 10.0f };
	color = { 1.0f, 0.0f, 0.0f };
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	entity.AddComponent<Components::Render>(Render{ glm::mat4{1}, color, sphereModel, false });
	//entity.AddComponent<Components::Name>(Name{ "Light 3" });

	pos = { 0.0f, 10.0f, -10.0f };
	color = { 0.0f, 1.0f, 0.0f };
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	entity.AddComponent<Components::Render>(Render{ glm::mat4{1}, color, sphereModel, false });
	//entity.AddComponent<Components::Name>(Name{ "Light 4" });

	std::cout << "PROGRAM STARTED, USE THE EDITOR'S DEBUGGER" << std::endl;
	
	// Nazi?
	//ScriptSystem SS;
	//SS.testfuncwithparam('X');
	//SS.testfuncwithreturn();

	ImaginarySystem.Init(&ecs);
	Pogplant::Input::InputSystem::Instance()->Init(PP::Window::GetWindow());
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

//void DebugCubes(const GameObject& _GO)
//{
//	float largestScale = std::numeric_limits<float>::min();
//	for (int i = 0; i < 3; i++)
//	{
//		largestScale = std::max(largestScale, _GO.m_Scale[i]);
//	}
//
//	const float halfLen = _GO.m_RenderObject->m_RenderModel->m_Bounds.longest * largestScale * 0.5f;
//	glm::mat4 base = glm::mat4{ 1 };
//	glm::vec3 trans = glm::make_vec3(_GO.m_Position);
//	base = glm::translate( base, trans );
//	base = glm::scale( base, {1.0f,1.0f,1.0f } );
//
//	// Top 
//	glm::mat4 model = glm::translate(base, { 0,halfLen,0 });
//	SetCube(model);
//	// Left
//	model = glm::translate(base, { -halfLen,0,0 });
//	SetCube(model);
//	// Right
//	model = glm::translate(base, { halfLen,0,0 });
//	SetCube(model);
//	// Front
//	model = glm::translate(base, { 0,0,-halfLen });
//	SetCube(model);
//	// Back
//	model = glm::translate(base, { 0,0,halfLen });
//	SetCube(model);
//	// Bottom
//	model = glm::translate(base, { 0,-halfLen,0 });
//	SetCube(model);
//}

void DebugCubes(Transform& transform, Render& renderer)
{
	float largestScale = std::numeric_limits<float>::min();
	for (int i = 0; i < 3; i++)
	{
		largestScale = std::max(largestScale, transform.m_scale[i]);
	}

	const float halfLen = renderer.m_RenderModel->m_Bounds.longest * largestScale * 0.5f;
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
	//glm::mat4 Model = glm::mat4{ 1 };
	//Model = glm::translate(Model, glm::vec3(0.0f, 0.0f, -40.0f));
	//Model = glm::scale(Model, glm::vec3(20.0f, 20.0f, 20.0f));
	//PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{0.69f,0.69f,0.69f,1}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

	/// TEMP - Update transforms for render
	auto view = ecs.GetReg().view<Transform, Render>();
	
	for (auto entity : view)
	{
		auto& transform = view.get<Transform>(entity);
		auto& renderer = view.get<Render>(entity);
		
		renderer.m_Model = glm::make_mat4(transform.m_ModelMtx);
		//DebugCubes(transform, renderer);
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
	Render* renderOjbect = nullptr;

	//const int currIdx = PPD::ImguiHelper::m_CurrentGOIdx;
	entt::entity currIdx = PPD::ImguiHelper::m_CurrentEntity;

	//if (currIdx >= 0)
	if (currIdx != entt::null)
	{
		//renderOjbect = GO_Resource::m_GO_Container[currIdx].m_RenderObject;
		renderOjbect = ecs.GetReg().try_get<Render>(currIdx);
	}

	// Models for Gpass
	PP::Renderer::StartGBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::Draw("EDITOR", ecs.GetReg(), renderOjbect);
	PP::Renderer::EndBuffer();
	// Where to draw the gpass FB to
	PP::Renderer::StartEditorBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::GLightPass("EDITOR", ecs.GetReg());
	PP::Renderer::EndBuffer();
}

void DrawGame()
{
	auto results = ecs.GetReg().view<Render>();

	// Models for Gpass
	PP::Renderer::StartGBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::Draw("GAME", ecs.GetReg(), nullptr);
	PP::Renderer::EndBuffer();
	// Where to draw the gpass FB to
	PP::Renderer::StartGameBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::GLightPass("GAME", ecs.GetReg());
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


		ImaginarySystem.Update();

		/// Most of this should be moved to other files when the engine is developed
		// Things that appear in both editor & game
		DrawCommon();
		// Editor
		DrawEditor();
		// Game
		DrawGame();
		// Post process
		// DrawScreen();
		// ImGUI
		DrawImGUI();
		///

		Pogplant::Input::InputSystem::Instance()->pollEvents();

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

	Pogplant::Input::InputSystem::Destroy();
}

int main()
{
	//testing ecs stuffs

	Init();
	Run();
	Exit();
}
