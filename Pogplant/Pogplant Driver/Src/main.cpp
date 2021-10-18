#pragma warning(disable  : 4005) // Winapi entry 

#include "ImguiHelper.h"

#include "AudioEngine.h"
#include "ECS/Entity.h"
#include "ECS/Systems/GeneralSystem.h"
#include "ECS/Systems/PhysicsSystem.h"
#include "ECS/Systems/ScriptSystem.h"

#include "Input/InputSystem.h"
#include "ResourceAllocator.hpp"
#include "AssetCompiler.h"
#include "FileHandler.h"

#include "Utils/ChronoTimer.h"

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

namespace PPD = PogplantDriver;
using namespace Components;
ECS ecs;
GeneralSystem generalSystem;
PhysicsSystem physicsSystem;
ScriptSystem scriptSystem;

void TempSceneObjects()
{
	/// Add to container
	std::string sphere, cube, ship, enemy, cubeDebug;
	sphere = PPC::AssetCompiler::GetFileName("Resources/KekFiles/sphere.kek");
	cube = PPC::AssetCompiler::GetFileName("Resources/KekFiles/Cube.kek");
	cubeDebug = PPC::AssetCompiler::GetFileName("Resources/KekFiles/CubeTest.kek");
	ship = PPC::AssetCompiler::GetFileName("Resources/KekFiles/Player_Ship.kek");
	enemy = PPC::AssetCompiler::GetFileName("Resources/KekFiles/Enemy_01.kek");

	PP::Model* sphereModel = PP::ModelResource::m_ModelPool[sphere];
	PP::Model* cubeModel = PP::ModelResource::m_ModelPool[cube];
	PP::Model* cubeDebugModel = PP::ModelResource::m_ModelPool[cubeDebug];
	PP::Model* shipModel = PP::ModelResource::m_ModelPool[ship];
	PP::Model* enemyModel = PP::ModelResource::m_ModelPool[enemy];
	PP::Mesh* floorMesh = PP::MeshResource::m_MeshPool[PP::MeshResource::MESH_TYPE::HEIGHTMAP];

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
	entity.AddComponent<Components::Renderer>(Renderer{ color, sphereModel });
	entity.AddComponent<Components::SphereCollider>(SphereCollider{ glm::vec3{ 0 }, 1.0f });
	entity.AddComponent<Components::HeightMapDebugger>(0.0f);
	entity.GetComponent<Components::Name>().m_name = "Height Map Debugger";
	//entity.AddComponent<Components::Name>(Name{ "Sphere Test" });

	//pos = { 0.0f, 0.0f, 0.0f };
	//rot = { 0.0f,0.0f,0.0f };
	//scale = { 2.0f,2.0f,2.0f };
	//entity = ecs.CreateEntity("", pos, rot, scale);
	//entity.AddComponent<Components::Renderer>(Renderer{ color, cubeDebugModel });
	//entity.GetComponent<Components::Name>().m_name = "DebugCube";

	pos = { 0.0f, -10.0f, 0.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 210.0f,30.0f,210.0f };
	entity = ecs.CreateEntity("", pos, rot, scale);
	//entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, cubeModel });
	entity.AddComponent<Components::PrimitiveRender>(PrimitiveRender
	(
		{ "MUD_DIFF", "GRASS_DIFF" },
		{ "MUD_BUMP", "GRASS_BUMP" },
		{ "MUD_NORM", "GRASS_NORM" },
		{ "MUD_SPEC", "GRASS_SPEC" },
		floorMesh,
		4.0f,
		true
	));
	entity.GetComponent<Components::Name>().m_name = "Floor";
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ {0.1f, 0.1f, 0.1f }, {0.f, 0.f, 0.f} });

	pos = { 15.0f, 15.0f, 10.f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 1.0f,1.0f,1.0f };

	entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Renderer>(Renderer{ color, shipModel });
	entity.AddComponent<Components::Rigidbody>(Rigidbody{});
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1, 1, 1}, glm::vec3{0, 0, 0} });
	std::unordered_map<std::string, bool> shipScripts;
	shipScripts["Player"] = false;
	entity.AddComponent<Components::Scriptable>(shipScripts);
	entity.GetComponent<Components::Name>().m_name = "Ship";

	pos = { 7.5f, 7.5f, 10.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 1.0f,1.0f,1.0f };

	entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Renderer>(Renderer{ color, enemyModel });
	entity.AddComponent<Components::Rigidbody>(Rigidbody{});
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1, 1, 1}, glm::vec3{0, 0, 0} });
	std::unordered_map<std::string, bool> enemyScripts;
	enemyScripts["Enemy"] = false;
	entity.AddComponent<Components::Scriptable>(enemyScripts);
	entity.GetComponent<Components::Name>().m_name = "Enemy";

	//auto entity = registry.create();
	//registry.emplace<Transform>(registry.create(), pos, rot, scale);
	//registry.emplace<Renderer>(entity,&cubeModel);

	/// Light
	pos = { 7.5f, 15.0f, 20.0f };
	glm::vec3 direction = -glm::normalize(pos);
	scale = { 1.0f,1.0f,1.0f }; // Affects light model and not the actual light size
	color = { 0.2f, 0.2f, 0.15f };
	float intensity = 13.0f;
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Directional_Light>(Directional_Light{ color, intensity, direction , 0.42f, 0.69f });
	entity.AddComponent<Components::Renderer>(Renderer{ color, sphereModel, false, true });
	entity.GetComponent<Components::Name>().m_name = "Directional Light";

	intensity = 1.0f;
	pos = { 15.0f, 22.0f, 19.0f };
	color = { 1.0f, 1.0f, 1.0f };
	const float linear = 0.00069f;
	const float quadratic = 0.0042f;
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	entity.AddComponent<Components::Renderer>(Renderer{ color, sphereModel, false, true });
	entity.GetComponent<Components::Name>().m_name = "White point light";

	intensity = 4.2f;
	pos = { 26.0f, 10.0f, -16.5f };
	color = { 0.0f, 0.0f, 1.0f };
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	entity.AddComponent<Components::Renderer>(Renderer{ color, sphereModel, false, true });
	entity.GetComponent<Components::Name>().m_name = "Blue light";

	pos = { 21.0f, 10.0f, 10.0f };
	color = { 1.0f, 0.0f, 0.0f };
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	entity.AddComponent<Components::Renderer>(Renderer{ color, sphereModel, false, true });
	entity.GetComponent<Components::Name>().m_name = "Red light";

	pos = { -12.5, 10.0f, -10.0f };
	color = { 0.0f, 1.0f, 0.0f };
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	entity.AddComponent<Components::Renderer>(Renderer{ color, sphereModel, false, true });
	entity.GetComponent<Components::Name>().m_name = "Green light";

	//Test Object with body
	pos = { 3.0f, 1.f, 0.0f };
	color = { 0.0f, 1.0f, 1.0f };
	scale = { 0.5f, 0.5f, 0.5f };
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Renderer>(Renderer{ color, sphereModel, false, false });
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1.f, 1.f, 1.f}, glm::vec3{0.f, 0.f, 0.f} });
	//entity.AddComponent<Components::SphereCollider>(SphereCollider{ glm::vec3{0.f}, 1.f });
	entity.AddComponent<Components::Rigidbody>(Rigidbody{ 1.f, 0.f, false, true });
	entity.GetComponent<Components::Name>().m_name = "Test Rigidbody";
	entity.AddComponent<Components::AudioSource>();
	entity.GetComponent<Components::AudioSource>().m_audioSources.push_back(
		Components::AudioSource::AudioClip{ "Resources/Audio/test2.ogg", 0.2f, true, true, true }
	);

	//Test movable controllable body
	pos = { -3.0f, 1.f, 0.0f };
	color = { 0.0f, 1.0f, 1.0f };
	scale = { 0.5f, 0.5f, 0.5f };
	entity = ecs.CreateEntity("", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Renderer>(Renderer{ color, sphereModel, false, false });
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1.f, 1.f, 1.f}, glm::vec3{0.f, 0.f, 0.f} });
	entity.AddComponent<Components::Rigidbody>(Rigidbody{ 1.f, 0.f, false, true });
	entity.AddComponent<Components::CharacterController>();
	entity.GetComponent<Components::Name>().m_name = "Test movableObject";
	entity.AddComponent<Components::AudioSource>();
	entity.GetComponent<Components::AudioSource>().m_audioSources.push_back(
		Components::AudioSource::AudioClip{ "Resources/Audio/test.ogg", 0.2f, true, true, true }
	);

	/// FONT
	pos = { 0.0f, 10.0, -10.0f };
	rot = { 0.0f, 0.0f,0.0f };
	scale = { 42.0f,42.0f,42.0f };
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
	pos = { 15.0f, 10.0f, 45.0f };
	color = { 0.9f, 0.5f, 0.2f };
	entity = ecs.CreateEntity("", pos, rot, scale);
	entity.AddComponent<Components::Camera>(Camera{ glm::mat4{1},glm::mat4{1}, glm::vec3{0}, glm::vec3{0}, glm::vec3{0}, -90.0f, 0.0, 45.0f, 0.1f, 200.0f, true });
	entity.AddComponent<Components::Renderer>(Renderer{ color, cubeModel, false, false });
	entity.GetComponent<Components::Name>().m_name = "Game Camera";

	std::cout << "PROGRAM STARTED, USE THE EDITOR'S DEBUGGER" << std::endl;

	generalSystem.Init(&ecs);
	physicsSystem.Init(&ecs);
	scriptSystem.Init(&ecs);
	PPI::InputSystem::Instance()->Init(PP::Window::GetWindow());
}

void Init()
{
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
	PPA::AudioEngine::Instance();

	/// Start pogplant lib
	PP::Entry::Init();
	PPD::ImguiHelper::InitImgui(&ecs);

	PPF::FileHandler& fh = fh.GetInstance();
	// DO THIS ONLY AFTER OPENGL HAS INIT
	// Model has to be before KekFiles
	fh.AddNewWatchPath("Resources/Models");
	fh.AddNewWatchPath("Resources/KekFiles");
	fh.AddNewWatchPath("Resources/Prefabs");

	/// To be moved to relative scenes
	TempSceneObjects();
}

void UpdateTransforms()
{
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
				PPA::AudioEngine::UpdateListenerPosition(transform.m_position, camera.m_Front, camera.m_Up, PhysicsDLC::Vector::Zero);
			}
		}
	}

	/// Height map example GAB refer to this hehe xd
	auto hmd_view = ecs.GetReg().view<Transform, HeightMapDebugger>();
	auto heightMap_view = ecs.GetReg().view<Transform, PrimitiveRender>();
	PP::Mesh* floorMesh = PP::MeshResource::m_MeshPool[PP::MeshResource::MESH_TYPE::HEIGHTMAP];
	for (auto hm : heightMap_view)
	{
		auto& hmT = heightMap_view.get<Transform>(hm);
		for (auto entity : hmd_view)
		{
			auto& debugObjectT = hmd_view.get<Transform>(entity);
			glm::vec3 mappedPos = glm::vec3(debugObjectT.m_position.x / hmT.m_scale.x, 0.0f, debugObjectT.m_position.z / hmT.m_scale.z);
			// Factor height * heightmap scale + heightmaps relative position then + the size of object
			debugObjectT.m_position.y = floorMesh->GetHeight(mappedPos) * hmT.m_scale.y + hmT.m_position.y + debugObjectT.m_scale.y;
		}
	}

	// Debug draws
	physicsSystem.DrawColliders();
	auto view = ecs.GetReg().view<Transform>();
	for (auto entity : view)
	{
		auto& transform = view.get<Transform>(entity);
		transform.updateModelMtx();
	}
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

	// Debug draws
	physicsSystem.DrawColliders();

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
	// Delta time
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

		PPA::AudioEngine::Update();

		generalSystem.Update();
		scriptSystem.Update();
		PPF::FileHandler& fh = fh.GetInstance();
		fh.UpdateModels();

		/// Most of this should be moved to other files when the engine is developed
		// Update the transform before drawing
		UpdateTransforms();
		// Things that appear in both editor & game
		DrawCommon();
		// Editor
		DrawEditor();
		// Game
		DrawGame();
		// Post process - Only used for run time, currently drawing to imgui
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
	PP::Entry::Cleanup();
	PPA::AudioEngine::Destroy();
	PPI::InputSystem::Destroy();
	PPF::FileHandler& fh = fh.GetInstance();
	fh.Stop();
}

int main()
{
	//testing ecs stuffs
	Init();
	Run();
	Exit();
}
