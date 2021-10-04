﻿#include "ImguiHelper.h"

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
#include "ECS/Systems/PhysicsSystem.h"
#include "ECS/Systems/ScriptSystem.h"

#include "Input/InputSystem.h"
#include "ResourceAllocator.hpp"
#include "AssetCompiler.h"
#include "FileHandler.h"

#include "Utils/ChronoTimer.h"
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
PhysicsSystem physicsSystem;
ScriptSystem scriptSystem;

void Init()
{
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);

	PP::Window::InitWindow(
		1280,					// Width
		720,					// Height
		"BogosBinted"			// Window name
	);

	// Will change to automatic update when i figure it out
	AssetCompiler& acc = acc.GetInstance();
	//if(!acc.Exists("Resources/KekFiles/Cube.kek"))
	acc.RunExecutable("Pogplant Compiler.exe", "Resources/Models/cube/Cube.fbx");
	//if (!acc.Exists("Resources/KekFiles/Sphere.kek"))
	acc.RunExecutable("Pogplant Compiler.exe", "Resources/Models/sphere/Sphere.fbx");
	//if (!acc.Exists("Resources/KekFiles/Enemy_01.kek"))
	acc.RunExecutable("Pogplant Compiler.exe", "Resources/Models/Enemy/Enemy_01.fbx");
	//if (!acc.Exists("Resources/KekFiles/Player_Ship.kek"))
	acc.RunExecutable("Pogplant Compiler.exe", "Resources/Models/Ship/Player_Ship.fbx");
	//if (!acc.Exists("Resources/KekFiles/backpack.kek"))
	//acc.RunExecutable( "Pogplant Compiler.exe", "Resources/Models/backpack/backpack.obj");
	acc.WaitForAllProcess();
	//acc.WaitForSingleProcess("BAG");

	FileHandler& fileHandler = fileHandler.GetInstance();
	fileHandler.AddNewWatchPath("Resources/KekFiles");
	fileHandler.AddNewWatchPath("Resources/Models");
	fileHandler.AddNewWatchPath("Resources/Prefabs");

	PP::TextureResource::InitResource();
	PP::MeshBuilder::InitMesh();
	PP::FontResource::InitResource();
	PP::ShaderLinker::InitShader();
	PP::FrameBuffer::InitFrameBuffer();
	PP::CameraResource::InitBaseCameras(
		glm::vec3{ 0,0,5.0f }, // Editor cam pos
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
	std::string sphere, cube, ship, enemy;
	sphere = AssetCompiler::GetFileName("Resources/KekFiles/Sphere.kek");
	cube = AssetCompiler::GetFileName("Resources/KekFiles/Cube.kek");
	ship = AssetCompiler::GetFileName("Resources/KekFiles/Player_Ship.kek");
	enemy = AssetCompiler::GetFileName("Resources/KekFiles/Enemy_01.kek");
	PP::Model* sphereModel = PP::ModelResource::m_ModelPool[sphere];
	PP::Model* cubeModel = PP::ModelResource::m_ModelPool[cube];
	PP::Model* shipModel = PP::ModelResource::m_ModelPool[ship];
	PP::Model* enemyModel = PP::ModelResource::m_ModelPool[enemy];

	/* CLARENCE DEBUGGING STUFF */
	//std::cout << "number of meshes: " << cubeModel->m_Meshes.size() << std::endl;
	//std::cout << "How many meshes: " << bagModel->m_Meshes.size() << std::endl;
	//std::cout << "How many textures loaded: " << bagModel->m_TexturesLoaded.size() << std::endl;

	//for (auto& meshes : floorModel->m_Meshes)
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

	//ecs.AddComponent<Components::Renderer>(entity, &bagModel);
	//registry.emplace<Renderer>(entity, &bagModel);

	pos = { -5.0f, 0.0f, -10.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 2.0f,2.0f,2.0f };
	//GO_Resource::m_GO_Container.push_back(GameObject(pos, rot, scale, &GO_Resource::m_Render_Container[1]));

	auto entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, sphereModel });
	entity.AddComponent<Components::SphereCollider>(SphereCollider{ glm::vec3{ 0 }, 1.0f });
	entity.AddComponent<Imaginary_object>("gab_small_pepe");
	entity.GetComponent<Components::Name>().m_name = "Sphere";
	//entity.AddComponent<Components::Name>(Name{ "Sphere Test" });

	pos = { 0.0f, -55.0f, 0.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 100.0f,100.0f,100.0f };
	entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, cubeModel });
	entity.GetComponent<Components::Name>().m_name = "Floor";
	//scriptsok.push_back("Start");
	//scriptsok.push_back("Update");
	//scriptsok.push_back("Move");
	//entity.AddComponent<Components::Scriptable>(Scriptable{ scriptsok });
	//entity.AddComponent<Components::Name>(Name{ "Floor" });

	pos = { 5, -2.0f, 10.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 1.0f,1.0f,1.0f };

	entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, shipModel });
	entity.AddComponent<Components::Rigidbody>(Rigidbody{});
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1, 1, 1}, glm::vec3{0, 0, 0} });
	std::vector<std::string> scriptsok;
	entity.AddComponent<Components::Scriptable>(Scriptable{ scriptsok });
	entity.GetComponent<Components::Name>().m_name = "Ship";

	pos = { -10.0f, -2.0f, 10.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 1.0f,1.0f,1.0f };

	entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, enemyModel });
	entity.GetComponent<Components::Name>().m_name = "Enemy";

	//auto entity = registry.create();
	//registry.emplace<Transform>(registry.create(), pos, rot, scale);
	//registry.emplace<Renderer>(entity,&cubeModel);

	/// Light
	pos = { 7.5f, 15.0f, 20.0f };
	glm::vec3 direction = -glm::normalize(pos);
	scale = { 1.0f,1.0f,1.0f }; // Affects light model and not the actual light size
	float intensity = 0.69f;
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Directional_Light>(Directional_Light{ color, intensity, direction , 0.42f, 0.69f });
	entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, sphereModel, false, true });
	entity.GetComponent<Components::Name>().m_name = "Directional Light";

	pos = { 0.0f, 10.0f, 0.0f };
	color = { 1.0f, 1.0f, 1.0f };
	const float linear = 0.00069f;
	const float quadratic = 0.0042f;
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, sphereModel, false, true });
	entity.GetComponent<Components::Name>().m_name = "White point light";

	intensity = 1.0f;
	pos = { -10.0f, 10.0f, 10.0f };
	color = { 0.0f, 0.0f, 1.0f };
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, sphereModel, false, true });
	entity.GetComponent<Components::Name>().m_name = "Blue light";

	pos = { 10.0f, 10.0f, 10.0f };
	color = { 1.0f, 0.0f, 0.0f };
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, sphereModel, false, true });
	entity.GetComponent<Components::Name>().m_name = "Red light";

	pos = { 0.0f, 10.0f, -10.0f };
	color = { 0.0f, 1.0f, 0.0f };
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, sphereModel, false, true });
	entity.GetComponent<Components::Name>().m_name = "Green light";

	//Test Object with body
	pos = { 10.0f, 1.f, -10.0f };
	color = { 0.0f, 1.0f, 1.0f };
	scale = { 0.5f, 0.5f, 0.5f };
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, sphereModel, false, false });
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1.f, 1.f, 1.f}, glm::vec3{0.f, 0.f, 0.f} });
	//entity.AddComponent<Components::SphereCollider>(SphereCollider{ glm::vec3{0.f}, 1.f });
	entity.AddComponent<Components::Rigidbody>(Rigidbody{ 1.f });
	entity.GetComponent<Components::Name>().m_name = "Test Rigidbody";

	pos = { 10.0f, 1.f, -15.0f };
	color = { 0.0f, 1.0f, 1.0f };
	scale = { 0.5f, 0.5f, 0.5f };
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, sphereModel, false, false });
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1.f, 1.f, 1.f}, glm::vec3{0.f, 0.f, 0.f} });
	entity.AddComponent<Components::Rigidbody>(Rigidbody{ 1.f });
	entity.AddComponent<Components::CharacterController>();
	entity.GetComponent<Components::Name>().m_name = "Test movableObject";

	/// FONT
	pos = { 0.0f, 0.0f, -5.0f };
	rot = { 0.0f, 0.0f,0.0f };
	scale = { 30.0f,30.0f,30.0f };
	entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Text>(Text{ {1.0f,0.0f,0.0f}, "Ruda", "This is a very big text", false });
	entity.GetComponent<Components::Name>().m_name = "World font";

	pos = { -1.0f, 0.85f, 0.0f };
	rot = { 0.0f, 0.0f,0.0f };
	scale = { 1.0f,1.0f,1.0f };
	entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Text>(Text{ {1.0f,0.0f,1.0f}, "Ruda", "Screen Font", true });
	entity.GetComponent<Components::Name>().m_name = "Screen font";

	/// Camera
	pos = { 0.0f, 0.0f, 30.0f };
	color = { 0.9f, 0.5f, 0.2f };
	entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Camera>(Camera{ glm::mat4{1},glm::mat4{1}, glm::vec3{0}, glm::vec3{0}, glm::vec3{0}, -90.0f, 0.0, 45.0f, 0.1f, 200.0f, true });
	entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, cubeModel, false, false });
	entity.GetComponent<Components::Name>().m_name = "Game Camera";

	std::cout << "PROGRAM STARTED, USE THE EDITOR'S DEBUGGER" << std::endl;

	ImaginarySystem.Init(&ecs);
	physicsSystem.Init(&ecs);
	scriptSystem.Init(&ecs);
	PPI::InputSystem::Instance()->Init(PP::Window::GetWindow());
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

void DebugCubes(Transform& transform, Renderer& renderer)
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
	/// TEMP - Old reference code for instancing

	//PP::MeshInstance::ResetCount();
	//// 3D object background to see orientation
	////glm::mat4 Model = glm::mat4{ 1 };
	////Model = glm::translate(Model, glm::vec3(0.0f, 0.0f, -40.0f));
	////Model = glm::scale(Model, glm::vec3(20.0f, 20.0f, 20.0f));
	////PP::MeshInstance::SetInstance(PP::InstanceData{ Model, glm::vec4{0.69f,0.69f,0.69f,1}, glm::vec2{1}, glm::vec2{0}, -1, 0, 0 });

	auto camView = ecs.GetReg().view<Transform, Camera>();
	{
		for (auto entity : camView)
		{
			auto& camera = ecs.GetReg().get<Camera>(entity);
			auto& transform = ecs.GetReg().get<Transform>(entity);

			// If active update its projection & view;
			if (camera.m_Active)
			{
				const glm::vec2 windowSize = { PP::Window::m_Width, PP::Window::m_Height };
				PP::Camera::GetUpdatedVec(camera.m_Yaw, camera.m_Pitch, camera.m_Up, camera.m_Right, camera.m_Front);
				PP::Camera::GetUpdatedProjection(windowSize, camera.m_Zoom, camera.m_Near, camera.m_Far, camera.m_Projection);
				PP::Camera::GetUpdatedView(transform.m_position, transform.m_position + camera.m_Front, camera.m_Up, camera.m_View);
			}
		}
	}


	auto view = ecs.GetReg().view<Transform, Renderer>();
	auto debugView = ecs.GetReg().view<Transform, DebugRender>();
	glm::vec3 camPos = PP::CameraResource::GetCamera("EDITOR")->GetPosition();
	for (auto entity : view)
	{
		auto& transform = view.get<Transform>(entity);
		auto& renderer = view.get<Renderer>(entity);

		auto boxCollider = ecs.GetReg().try_get<BoxCollider>(entity);
		auto sphereCollider = ecs.GetReg().try_get<SphereCollider>(entity);

		transform.updateModelMtx();
		renderer.m_Model = transform.m_ModelMtx;

		/// Debug draw update
		if (boxCollider)
		{
			PP::DebugDraw::DebugCube(boxCollider->aabb.m_min, boxCollider->aabb.m_max);
		}
		else if (sphereCollider)
		{
			const glm::vec3 camDir = sphereCollider->sphere.m_pos - camPos;
			PP::DebugDraw::DebugSphere(sphereCollider->sphere.m_pos, camDir, sphereCollider->sphere.m_radius);
		}
	}

	for (auto entity : debugView)
	{
		auto& transform = debugView.get<Transform>(entity);
		auto& renderer = debugView.get<DebugRender>(entity);

		renderer.m_Model = transform.m_ModelMtx;
		//DebugCubes(transform, renderer);
	}

	////for (size_t i = 0; i < GO_Resource::m_GO_Container.size(); i++)
	////{
	////	const auto& go = GO_Resource::m_GO_Container[i];
	////	go.m_RenderObject->m_Model = glm::make_mat4(go.m_ModelMtx);
	////	DebugCubes(go);
	////}

	//PP::MeshBuilder::RebindQuad();

	/// For all draws
	// If something is selected choose it to be highlighted
	Renderer* renderOjbect = nullptr;

	//const int currIdx = PPD::ImguiHelper::m_CurrentGOIdx;
	entt::entity currIdx = PPD::ImguiHelper::m_CurrentEntity;

	//if (currIdx >= 0)
	if (currIdx != entt::null)
	{
		//renderOjbect = GO_Resource::m_GO_Container[currIdx].m_RenderObject;
		renderOjbect = ecs.GetReg().try_get<Renderer>(currIdx);
	}

	// Common, since directional
	PP::Renderer::ShadowPass(ecs.GetReg());
}

void DrawEditor()
{
	// Models for Gpass
	PP::Renderer::StartGBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::Draw(ecs.GetReg(), nullptr, true);
	PP::Renderer::EndBuffer();

	// Where to draw the gpass FB to
	PP::Renderer::PostProcess();
	PP::Renderer::ClearBuffer();
	PP::Renderer::GLightPass(ecs.GetReg(), true);
	PP::Renderer::EndBuffer();

	PP::Renderer::DebugPass(ecs.GetReg());
	//PP::Renderer::BlurPass();
	PP::Renderer::StartEditorBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::HDRPass(false);
	PP::Renderer::EndBuffer();
}

void DrawGame()
{
	auto results = ecs.GetReg().view<Renderer>();

	// Models for Gpass
	PP::Renderer::StartGBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::Draw(ecs.GetReg(), nullptr, false);
	PP::Renderer::EndBuffer();

	// Where to draw the gpass FB to
	PP::Renderer::PostProcess();
	PP::Renderer::ClearBuffer();
	PP::Renderer::GLightPass(ecs.GetReg(), false);
	PP::Renderer::EndBuffer();

	PP::Renderer::BlurPass();
	PP::Renderer::StartGameBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::HDRPass(true);
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
	physicsSystem.DrawImGUI();
}

void Run()
{
	PPU::ChronoTimer<float> c_dtTimer;
	float c_deltaTime = 0.f;
	float c_accumulatedFixedTime = 0.f;
	static constexpr float c_minFixedUpdateTime = 1 / 30.f;

	while (!PP::Window::ShouldCloseWindow())
	{
		c_dtTimer.startTimer();

		PP::Window::CheckForceClose(); // Temp exit using Esc

		// Camera KB movement
		PP::CameraResource().UpdateActiveCamera(ImGui::GetIO().DeltaTime);

		//Should move this to game state when it's available
		//Physics dynamic update until fps drops below 30fps
		c_accumulatedFixedTime += c_deltaTime;

		if (c_accumulatedFixedTime < c_minFixedUpdateTime)
		{
			physicsSystem.Update(c_accumulatedFixedTime);
			c_accumulatedFixedTime = 0.f;
		}
		else
		{
			while (c_accumulatedFixedTime > c_minFixedUpdateTime)
			{
				physicsSystem.Update(c_minFixedUpdateTime);
				c_accumulatedFixedTime -= c_minFixedUpdateTime;
			}
		}

		physicsSystem.Update(ImGui::GetIO().DeltaTime);
		ImaginarySystem.Update();
		scriptSystem.Update();
		FileHandler& fh = fh.GetInstance();
		fh.UpdateModels();

		/// Most of this should be moved to other files when the engine is developed
		// Things that appear in both editor & game
		DrawCommon();
		// Editor
		DrawEditor();
		// Game
		DrawGame();
		// Post process
		//DrawScreen();
		// ImGUI
		DrawImGUI();
		///

		PPI::InputSystem::Instance()->pollEvents();

		PP::Renderer::SwapBuffer();

		c_deltaTime = c_dtTimer.getElapsedTimePrecise();
	}
}

void Exit()
{
	PPD::ImguiHelper::CleanUpImgui();
	PP::CameraResource::CleanUpCameras();
	PP::FrameBuffer::CleanUpFrameBuffer();
	PP::FontResource::CleanUpResource();
	PP::MeshBuilder::CleanUpMesh();
	PP::Window::CleanUpWindow();

	PPI::InputSystem::Destroy();
	FileHandler& fh = fh.GetInstance();
	fh.Stop();
}

int main()
{
	//testing ecs stuffs

	Init();
	Run();
	Exit();
}
