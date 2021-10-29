#include "Application.h"

#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>
#include <gtc/random.hpp>
#include <imgui.h>

#include <iostream>
#include <Pogplant.h>
#include "GameScript.h"

namespace PPD = PogplantDriver;
using namespace Components;
using namespace PogplantDriver;

std::unique_ptr<Application> Application::m_instance = nullptr;
std::once_flag Application::m_onceFlag;

/****************************TEMPORARY STUFF TO MOVE***************************/
// This has to be moved to a model loading or smth
void Application::ConstructModel(Entity& _Entity, PP::Model* _Model, PP::Mesh3D* _Mesh3D, const glm::vec3& _Color, bool _UseLight, bool _EditorOnly, bool _FirstIt)
{
	if (!_FirstIt)
	{
		auto child = m_activeECS->CreateChild(_Entity.GetID(), _Mesh3D->m_Name);
		child.AddComponent<Components::Renderer>(Renderer{ _Color, _Model, _Mesh3D, _UseLight, _EditorOnly });
		auto& transform = child.GetComponent<Components::Transform>();
		transform.m_position = _Mesh3D->m_Translate;
		transform.m_rotation = _Mesh3D->m_Rotate;
		transform.m_scale = _Mesh3D->m_Scale;
		for (auto it : _Mesh3D->m_SubMeshIDs)
		{
			ConstructModel(child, _Model, &_Model->m_Meshes[it], _Color, _UseLight, _EditorOnly, false);
		}
	}
	else
	{
		_Entity.AddComponent<Components::Renderer>(Renderer{ _Color, _Model, _Mesh3D, _UseLight, _EditorOnly });
		for (auto it : _Mesh3D->m_SubMeshIDs)
		{
			ConstructModel(_Entity, _Model, &_Model->m_Meshes[it], _Color, _UseLight, _EditorOnly, false);
		}
	}
}

/****************************END OF TEMPORARY STUFF TO MOVE***************************/


/******************************************************************************/
/*!
\brief
	Generic initialise to initialise the engine
*/
/******************************************************************************/
void Application::Init()
{
	//Create audio engine
	PPA::AudioEngine::Instance();

	//Initialise event bus
	m_eventBus = std::make_shared<PPE::EventBus>();

	/// Start pogplant lib
	PP::Entry::Init();
	PPD::ImguiHelper::InitImgui(&m_editorECS);

	PPF::FileHandler& fh = fh.GetInstance();
	// DO THIS ONLY AFTER OPENGL HAS INIT
	// Model has to be before KekFiles
	fh.AddNewWatchPath("Resources/Models");
	fh.AddNewWatchPath("Resources/KekFiles");
	fh.AddNewWatchPath("Resources/Prefabs");
	fh.AddNewWatchPath("Resources/Audio");

	std::cout << "PROGRAM STARTED, USE THE EDITOR'S DEBUGGER" << std::endl;

	PPI::InputSystem::Init(PP::Window::GetWindow());

#ifdef PPD_EDITOR_BUILD

#ifdef PPD_DEBUG_OBJECTS
	m_activeECS = &m_editorECS;
	InitialiseDebugObjects();
#endif

	m_appState = Application::APPLICATIONSTATE::EDITOR;
	m_nextAppState = Application::APPLICATIONSTATE::EDITOR;
	EnterEditorState();

#else
	m_appState = Application::APPLICATIONSTATE::PLAY;
	m_nextAppState = Application::APPLICATIONSTATE::PLAY;
	EnterPlayState();
#endif // PPD_EDITOR_BUILD

	BindEvents();
}


/******************************************************************************/
/*!
\brief
	Initialise test objects we are using for debugging
*/
/******************************************************************************/
#ifdef PPD_DEBUG_OBJECTS
void Application::InitialiseDebugObjects()
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
	//PP::Model* cubeDebugModel = PP::ModelResource::m_ModelPool[cubeDebug];
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

	auto entity = m_activeECS->CreateEntity("Height Map Debugger", pos, rot, scale);
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color);
	entity.AddComponent<Components::SphereCollider>(SphereCollider{ glm::vec3{ 0 }, 1.0f });
	entity.AddComponent<Components::HeightMapDebugger>(0.0f);

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
	entity = m_activeECS->CreateEntity("Floor", pos, rot, scale);
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
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ {0.1f, 0.1f, 0.1f }, {0.f, 0.f, 0.f} });

	pos = { 15.0f, 15.0f, 10.f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 1.0f,1.0f,1.0f };
	entity = m_activeECS->CreateEntity("Ship", pos, rot, scale);
	ConstructModel(entity, shipModel, &shipModel->m_Meshes.begin()->second);
	entity.AddComponent<Components::Rigidbody>(Rigidbody{});
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1, 1, 1}, glm::vec3{0, 0, 0} });
	std::unordered_map<std::string, bool> shipScripts;
	shipScripts["PlayerScript"] = false;
	entity.AddComponent<Components::Scriptable>(shipScripts);
	entity.AddComponent<Components::ParticleSystem>
		(
			ParticleSystem
			(
				glm::vec4{ 1,0,1,1 },
				glm::vec3{ 0,0,0 },
				0.005f, // Delay
				0.69f,	// Min Life
				1.00f,	// Max Life
				0.42f,	// Min Scale
				0.69f,	// Max Scale
				21.0f,	// Min Speed
				42.0f,	// Max Speed
				420,	// Spawn Count
				true,	// Loop
				false,	// Gravity
				true,	// Burst
				true	// Lerp speed
			)
			);

	pos = { 7.5f, 7.5f, 10.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 1.0f,1.0f,1.0f };

	entity = m_activeECS->CreateEntity("Enemy", pos, rot, scale);
	ConstructModel(entity, enemyModel, &enemyModel->m_Meshes.begin()->second);
	entity.AddComponent<Components::Rigidbody>(Rigidbody{});
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1, 1, 1}, glm::vec3{0, 0, 0} });
	std::unordered_map<std::string, bool> enemyScripts;
	enemyScripts["EnemyScript"] = false;
	entity.AddComponent<Components::Scriptable>(enemyScripts);

	/// Light
	pos = { 7.5f, 15.0f, 20.0f };
	glm::vec3 direction = -glm::normalize(pos);
	scale = { 1.0f,1.0f,1.0f }; // Affects light model and not the actual light size
	color = { 0.2f, 0.2f, 0.15f };
	float intensity = 13.0f;
	entity = m_activeECS->CreateEntity("Directional Light", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Directional_Light>(Directional_Light{ color, intensity, direction , 0.42f, 0.69f });
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color, false, true);

	intensity = 1.0f;
	pos = { 15.0f, 22.0f, 19.0f };
	color = { 1.0f, 1.0f, 1.0f };
	const float linear = 0.00069f;
	const float quadratic = 0.0042f;
	entity = m_activeECS->CreateEntity("White point light", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color, false, true);

	intensity = 4.2f;
	pos = { 26.0f, 10.0f, -16.5f };
	color = { 0.0f, 0.0f, 1.0f };
	entity = m_activeECS->CreateEntity("Blue light", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color, false, true);

	pos = { 21.0f, 10.0f, 10.0f };
	color = { 1.0f, 0.0f, 0.0f };
	entity = m_activeECS->CreateEntity("Red light", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color, false, true);

	pos = { -12.5, 10.0f, -10.0f };
	color = { 0.0f, 1.0f, 0.0f };
	entity = m_activeECS->CreateEntity("Green light", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color, false, true);

	//Test Object with body
	pos = { 3.0f, 1.f, 0.0f };
	color = { 0.0f, 1.0f, 1.0f };
	scale = { 0.5f, 0.5f, 0.5f };
	entity = m_activeECS->CreateEntity("Test Rigidbody", pos, glm::vec3{ 0 }, scale);
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color);
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1.f, 1.f, 1.f}, glm::vec3{0.f, 0.f, 0.f} });
	//entity.AddComponent<Components::SphereCollider>(SphereCollider{ glm::vec3{0.f}, 1.f });
	entity.AddComponent<Components::Rigidbody>(Rigidbody{ 1.f, 0.f, false, true });
	entity.AddComponent<Components::AudioSource>();
	entity.GetComponent<Components::AudioSource>().m_audioSources.push_back(
		Components::AudioSource::AudioObject{ "Resources/Audio\\test2.ogg", 0.2f, true }
	);

	//Test movable controllable body
	pos = { -3.0f, 1.f, 0.0f };
	color = { 0.0f, 1.0f, 1.0f };
	scale = { 0.5f, 0.5f, 0.5f };
	entity = m_activeECS->CreateEntity("Test Rigidbody2", pos, glm::vec3{ 0 }, scale);
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color);
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1.f, 1.f, 1.f}, glm::vec3{0.f, 0.f, 0.f} });
	entity.AddComponent<Components::Rigidbody>(Rigidbody{ 1.f, 0.f, false, true });
	entity.AddComponent<Components::AudioSource>();
	entity.GetComponent<Components::AudioSource>().m_audioSources.push_back(
		Components::AudioSource::AudioObject{ "Resources/Audio\\test.ogg", 0.2f, true }
	);

	/// FONT
	pos = { 0.0f, 10.0, -10.0f };
	rot = { 0.0f, 0.0f, 0.0f };
	scale = { 42.0f, 42.0f, 42.0f };
	entity = m_activeECS->CreateEntity("World font", pos, rot, scale);
	entity.AddComponent<Components::Text>(Text{ {1.0f, 0.0f, 0.0f}, "Ruda", "This is a very big text", false });

	pos = { -1.0f, 0.85f, 0.0f };
	rot = { 0.0f, 0.0f, 0.0f };
	scale = { 1.0f, 1.0f, 1.0f };
	entity = m_activeECS->CreateEntity("Screen font", pos, rot, scale);
	entity.AddComponent<Components::Text>(Text{ {1.0f, 0.0f, 1.0f}, "Ruda", "Screen Font", true });

	/// Camera
	pos = { 15.0f, 10.0f, 45.0f };
	color = { 0.9f, 0.5f, 0.2f };
	entity = m_activeECS->CreateEntity("Game Camera", pos, rot, scale);
	entity.AddComponent<Components::Camera>(Camera{ glm::mat4{1},glm::mat4{1}, glm::vec3{0}, glm::vec3{0}, glm::vec3{0}, -90.0f, 0.0, 45.0f, 0.1f, 200.0f, true });
	ConstructModel(entity, cubeModel, &cubeModel->m_Meshes.begin()->second, color, false, true);

	/// Canvas test
	pos = { 0.0f, 0.0f, -1.0f };
	scale = { 1.0f, 1.0f, 1.0f };
	entity = m_activeECS->CreateEntity("Canvas", pos, rot, scale);

	pos = { -0.55f, 0.3f, 0.0f };
	color = { 1.0f, 1.0f, 1.0f };
	scale = { 0.1f, 0.1f, 0.1f };
	auto child = m_activeECS->CreateChild(entity.GetID(), "Canvas Image 1", pos, rot, scale);
	// Simulate inspector set texture
	PP::TextureResource::UseTexture("TEST_TEX");
	child.GetComponent<Components::Transform>() = { pos,rot,scale };
	child.AddComponent<Components::Canvas>(Canvas{ {color, 1.0f}, PP::TextureResource::GetUsedTextureID("TEST_TEX")});

	pos = { -0.55f, 0.2f, 0.0f };
	color = { 1.0f, 1.0f, 1.0f };
	scale = { 0.1f, 0.1f, 0.1f };
	child = m_activeECS->CreateChild(entity.GetID(), "Canvas Image 2", pos, rot, scale);
	// Simulate inspector set texture
	PP::TextureResource::UseTexture("TEST_TEX2");
	child.GetComponent<Components::Transform>() = { pos,rot,scale };
	child.AddComponent<Components::Canvas>(Canvas{ {color, 1.0f}, PP::TextureResource::GetUsedTextureID("TEST_TEX2") });

	/// Instancing test
	//pos = { 0.0f, 0.0f, 0.0f };
	//rot = { 0.0f,0.0f,0.0f };
	//scale = { 1.0f,1.0f,1.0f };
	//entity = m_activeECS->CreateEntity("Instancing Test", pos, rot, scale);

	//int counter = 1;
	//for (int i = -12; i < 12; i ++)
	//{
	//	for (int j = -12; j < 12; j ++)
	//	{
	//		const std::string name = "Child " + std::to_string(counter);
	//		child = m_activeECS->CreateChild(entity.GetID(), name, pos, rot, scale);
	//		glm::vec3 cPos = { i * 8, 100.0f, j * 8 };
	//		child.GetComponent<Components::Transform>() = { cPos,rot,scale };
	//		ConstructModel(child, shipModel, &shipModel->m_Meshes.begin()->second);
	//		counter++;
	//	}
	//}
}
#endif

void Application::BindEvents()
{
	m_eventBus->listen(&m_sScriptSystem, &ScriptSystem::OnTriggerEnterEvent);
	m_eventBus->listen(&m_sScriptSystem, &ScriptSystem::OnTriggerExitEvent);
}

//recursion call
void Application::UpdateTransform(entt::entity _id, Transform& parent_transform)
{
	//update myself
	auto& transform = m_activeECS->GetReg().get<Transform>(_id);

	transform.updateModelMtx(parent_transform);

	//update children
	auto relationship = m_activeECS->GetReg().try_get<Relationship>(_id);
	if (relationship)
	{
		for (auto& entity : relationship->m_children)
			UpdateTransform(entity, transform);
	}
}

void Application::UpdateTransforms(float _Dt)
{
	//auto lol_id = m_activeECS->FindEntityWithName("Green light");

	/// Camera tranforms
	auto camView = m_activeECS->GetReg().view<Transform, Camera>();
	{
		for (auto& entity : camView)
		{
			auto& camera = m_activeECS->GetReg().get<Camera>(entity);
			auto& transform = m_activeECS->GetReg().get<Transform>(entity);

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

	/// Height map transform
	auto hmd_view = m_activeECS->GetReg().view<Transform, HeightMapDebugger>();
	auto heightMap_view = m_activeECS->GetReg().view<Transform, PrimitiveRender>();
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

	//Update transform matrix of all gameobject
	auto view = m_activeECS->GetReg().view<Transform>();
	for (auto entity : view)
	{
		auto& transform = view.get<Transform>(entity);

		auto relationship = m_activeECS->GetReg().try_get<Relationship>(entity);
		if (relationship && relationship->m_parent == entt::null)
		{
			transform.updateModelMtx();
			for (auto& ent : relationship->m_children)
				UpdateTransform(ent, transform);
		}
		else if (relationship == nullptr)
		{
			transform.updateModelMtx();
		}
	}

	/// 3D instance transforms
	// Clear old instance data
	for (auto& model : PP::ModelResource::m_ModelPool)
	{
		for (auto& mesh : model.second->m_Meshes)
		{
			mesh.second.m_Instances.clear();
		}
	}
	// Set instance data
	auto instanceView = m_activeECS->GetReg().view<Transform, Renderer>();
	for (auto& entity : instanceView)
	{
		auto& renderer = m_activeECS->GetReg().get<Renderer>(entity);
		auto& transform = m_activeECS->GetReg().get<Transform>(entity);
		auto* mesh = renderer.m_Mesh;
		int useTex = mesh->m_Textures.size() == 0 ? 0 : 1;
		mesh->m_Instances.push_back({ transform.m_ModelMtx, renderer.m_ColorTint, renderer.m_UseLight, useTex, renderer.m_EditorDrawOnly });
	}
	// Update meshes
	for (auto& model : PP::ModelResource::m_ModelPool)
	{
		for (auto& mesh : model.second->m_Meshes)
		{
			mesh.second.RebindMesh3D();
		}
	}

	/// Particle transforms
	auto particleView = m_activeECS->GetReg().view<Transform, ParticleSystem>();
	{
		for (auto& entity : particleView)
		{
			auto& transform = m_activeECS->GetReg().get<Transform>(entity);
			auto& pSys = m_activeECS->GetReg().get<ParticleSystem>(entity);

			// Burst vs constant spawn
			if (!pSys.m_Burst)
			{
				// Spawn delay
				pSys.m_Timer += _Dt;
				if (pSys.m_Timer >= pSys.m_Delay)
				{
					pSys.m_Timer = 0.0f;
					pSys.Spawn(transform.m_position, pSys.m_SpawnDirection);
				}
			}
			// Burst
			else if (!pSys.m_Done)
			{
				// Only spawn when everything has despawned
				if (pSys.m_ActiveCount == 0)
				{
					for (int i = 0; i < pSys.m_SpawnCount; i++)
					{
						pSys.Spawn(transform.m_position, glm::sphericalRand(1.0f));
					}
				}

				// To loop or not
				if (!pSys.m_Loop)
				{
					pSys.m_Done = true;
				}
			}

			// Update particles
			for (int i = 0; i < pSys.m_ActiveCount; i++)
			{
				Particle& it = pSys.m_ParticlePool[i];
				if (it.m_Life > 0.0f)
				{
					// Decrease life
					it.m_Life -= _Dt;

					if (it.m_Life <= 0.0f)
					{
						pSys.m_ActiveCount--;
						std::swap(it, pSys.m_ParticlePool[pSys.m_ActiveCount]);
						continue;
					}

					float t = it.m_Life / it.m_BaseLife;
					// Update position
					if (it.m_Gravity)
					{
						it.m_Velocity.y -= _Dt * 9.81f;
					}

					glm::vec3 currVel = it.m_Velocity;
					// Update lerp speed
					if (it.m_LerpSpeed)
					{
						// Ease in
						t = sinf(t * 3.14f * 0.25f);
						currVel = it.m_MinVelocity * (1 - t) + it.m_Velocity * t;
					}

					it.m_Position += currVel * _Dt;

					glm::mat4 model = glm::mat4{ 1 };
					model = glm::translate(model, it.m_Position);
					model = glm::scale(model, it.m_Scale);
					PP::MeshInstance::SetInstance(PP::InstanceData{ model, it.m_Color, it.m_TexID, false });
				}
			}
		}
	}

	/// Canvas
	auto canvasView = m_activeECS->GetReg().view<Transform, Canvas>();
	for (auto it : canvasView)
	{
		auto& transform = canvasView.get<Transform>(it);
		auto& canvas = canvasView.get<Canvas>(it);
		PP::MeshInstance::SetInstance(PP::InstanceData{ transform.m_ModelMtx, canvas.m_Color, canvas.m_TexID, true });
	}
}

void Application::DrawCommon()
{
	/// For all draws
	// If something is selected choose it to be highlighted
	Renderer* renderOjbect = nullptr;

	if (!m_activeECS->GetReg().valid(PPD::ImguiHelper::m_CurrentEntity))
		PPD::ImguiHelper::m_CurrentEntity = entt::null;

	//const int currIdx = PPD::ImguiHelper::m_CurrentGOIdx;
	entt::entity currIdx = PPD::ImguiHelper::m_CurrentEntity;

	//if (currIdx >= 0)
	if (currIdx != entt::null)
	{
		//renderOjbect = GO_Resource::m_GO_Container[currIdx].m_RenderObject;
		renderOjbect = m_activeECS->GetReg().try_get<Renderer>(currIdx);
	}

	// Common, since directional
	PP::Renderer::ShadowPass(m_activeECS->GetReg());
}

void Application::DrawEditor()
{
	// Models for Gpass
	PP::Renderer::StartGBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::Draw(m_activeECS->GetReg(), nullptr, true);
	PP::Renderer::EndBuffer();

	// Where to draw the gpass FB to
	PP::Renderer::PostProcess();
	PP::Renderer::ClearBuffer();
	PP::Renderer::GLightPass(m_activeECS->GetReg(), true);
	PP::Renderer::EndBuffer();

	//PP::Renderer::DebugPass(m_activeECS->GetReg());
	//PP::Renderer::BlurPass();
	PP::Renderer::StartEditorBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::HDRPass(false);
	PP::Renderer::EndBuffer();
}

void Application::DrawGame()
{
	auto results = m_activeECS->GetReg().view<Renderer>();

	// Models for Gpass
	PP::Renderer::StartGBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::Draw(m_activeECS->GetReg(), nullptr, false);
	PP::Renderer::EndBuffer();

	// Where to draw the gpass FB to
	PP::Renderer::PostProcess();
	PP::Renderer::ClearBuffer();
	PP::Renderer::GLightPass(m_activeECS->GetReg(), false);
	PP::Renderer::EndBuffer();

	PP::Renderer::BlurPass();
	PP::Renderer::StartGameBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::HDRPass(true);
	PP::Renderer::EndBuffer();
}

void Application::DrawScreen()
{
	PP::Renderer::PostProcess();
	PP::Renderer::ClearBuffer();
	PP::Renderer::DrawScreen();
	PP::Renderer::EndBuffer();
}

void Application::DrawImGUI()
{
	PP::Renderer::ClearBuffer();
	PPD::ImguiHelper::DrawImgui();
	m_sPhysicsSystem.DrawImGUI();
}

/******************************************************************************/
/*!
\brief
	Runs the application
*/
/******************************************************************************/
void Application::Run()
{
	// Delta time
	PPU::ChronoTimer<float> c_dtTimer;
	float c_deltaTime = 0.f;

	while (!PP::Window::ShouldCloseWindow())
	{
		c_dtTimer.startTimer();
		PP::Window::CheckForceClose(); // Temp exit using Esc

		// Camera KB movement
		PP::CameraResource().UpdateActiveCamera(ImGui::GetIO().DeltaTime);

#ifdef PPD_EDITOR_BUILD
		switch (m_appState)
		{
		case PogplantDriver::Application::APPLICATIONSTATE::EDITOR:
		{
			UpdateEditorState(c_deltaTime);
			RenderEditorState();
		}
		break;
		case PogplantDriver::Application::APPLICATIONSTATE::PLAY:
		{
			UpdatePlayState(c_deltaTime);
			RenderPlayState();
		}
		break;
		default:
			assert(false);
		}
#else
		UpdatePlayState(c_deltaTime);
		RenderPlayState();
#endif // PPD_EDITOR_BUILD


		PPA::AudioEngine::Update();
		PPI::InputSystem::PollEvents();
		PP::Renderer::SwapBuffer();

		if (m_nextAppState != m_appState)
		{
			switch (m_appState)
			{
			case PogplantDriver::Application::APPLICATIONSTATE::EDITOR:
				LeaveEditorState();
				EnterPlayState();
				break;
			case PogplantDriver::Application::APPLICATIONSTATE::PLAY:
				LeavePlayState();
				EnterEditorState();
				break;
			default:
				assert(false);
			}

			m_appState = m_nextAppState;
		}

		c_deltaTime = c_dtTimer.getElapsedTimePrecise();
	}
}

/******************************************************************************/
/*!
\brief
	Sets the application to transition to a new state
*/
/******************************************************************************/
void Application::TransitionApplicationState(APPLICATIONSTATE c_newState)
{
	m_nextAppState = c_newState;
}

/******************************************************************************/
/*!
\brief
	Cleans up the application
*/
/******************************************************************************/
void Application::Exit()
{
	switch (m_appState)
	{
	case PogplantDriver::Application::APPLICATIONSTATE::EDITOR:
		LeaveEditorState();
		break;
	case PogplantDriver::Application::APPLICATIONSTATE::PLAY:
		LeavePlayState();
		break;
	default:
		assert(false);
	}

	PPD::ImguiHelper::CleanUpImgui();
	PP::Entry::Cleanup();

	PPF::FileHandler& fh = fh.GetInstance();
	fh.Stop();
}

/******************************************************************************/
/*!
\brief
	Gets the singleton instance
\return
	Returns a reference to the Application instance
*/
/******************************************************************************/
Application& Application::GetInstance()
{
	std::call_once(m_onceFlag, [] {
		m_instance.reset(new Application);
		});

	return *m_instance.get();
}
