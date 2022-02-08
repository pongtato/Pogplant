#include "Application.h"

#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>
#include <gtc/random.hpp>
#include <imgui.h>

#include <iostream>
#include <Pogplant.h>
#include "GameScript.h"
#include "ECS/Systems/ScriptSystemHelper.h"

#include "Serialiser/CustomSaver.h"

namespace PPD = PogplantDriver;
using namespace Components;
using namespace PogplantDriver;

std::unique_ptr<Application> Application::m_instance = nullptr;
std::once_flag Application::m_onceFlag;

/****************************TEMPORARY STUFF TO MOVE***************************/
// Remove the one in application when model loading from json works
void Application::ConstructModel(Entity& _Entity, PP::Model* _Model, PP::Mesh3D* _Mesh3D, const glm::vec3& _Color, const glm::vec3& _Emissive, bool _UseLight, bool _EditorOnly, bool _FirstIt)
{
	if (!_FirstIt)
	{
		auto child = m_activeECS->CreateChild(_Entity.GetID(), _Mesh3D->m_Name);
		child.AddComponent<Components::Renderer>(Renderer{ _Color, _Emissive, _Model, _Mesh3D, _UseLight, _EditorOnly });
		auto& transform = child.GetComponent<Components::Transform>();
		transform.m_position = _Mesh3D->m_Translate;
		transform.m_rotation = _Mesh3D->m_Rotate * 90.0f;
		transform.m_scale = _Mesh3D->m_Scale;
		for (auto it : _Mesh3D->m_SubMeshIDs)
		{
			ConstructModel(child, _Model, &_Model->m_Meshes[it], _Color, _Emissive, _UseLight, _EditorOnly, false);
		}
	}
	else
	{
		_Entity.AddComponent<Components::Renderer>(Renderer{ _Color, _Emissive, _Model, _Mesh3D, _UseLight, _EditorOnly });
		for (auto it : _Mesh3D->m_SubMeshIDs)
		{
			ConstructModel(_Entity, _Model, &_Model->m_Meshes[it], _Color, _Emissive, _UseLight, _EditorOnly, false);
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

	//Load default audio Settings
	PPA::AudioEngine::CreateChannelGroup("BGM");
	PPA::AudioEngine::CreateChannelGroup("SFX");
	PPA::AudioEngine::CreateChannelGroup("VO");
	PPA::AudioEngine::SetChannelGroupVolume("BGM", PPU::CustomSaver::GetValue<float>("AudioBGM", 1.f));
	PPA::AudioEngine::SetChannelGroupVolume("SFX", PPU::CustomSaver::GetValue<float>("AudioSFX", 1.f));
	PPA::AudioEngine::SetChannelGroupVolume("VO", PPU::CustomSaver::GetValue<float>("AudioVO", 1.f));

	Pogplant::Renderer::m_Gamma = PPU::CustomSaver::GetValue<float>("Gamma", 2.2f);

#ifdef PPD_EDITOR_BUILD

#ifdef PPD_DEBUG_OBJECTS
	m_activeECS = &m_editorECS;
	InitialiseDebugObjects();
#endif

	m_appState = Application::APPLICATIONSTATE::EDITOR;
	m_nextAppState = Application::APPLICATIONSTATE::EDITOR;
	EnterEditorState();

#else
	PP::Window::HideCursor();
	m_appState = Application::APPLICATIONSTATE::PLAY;
	m_nextAppState = Application::APPLICATIONSTATE::PLAY;
	EnterPlayState("Resources/Scenes/MainMenu.json");
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
	//PP::Mesh* floorMesh = PP::MeshResource::m_MeshPool[PP::MeshResource::MESH_TYPE::HEIGHTMAP];

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

	//pos = { 0.0f, -10.0f, 0.0f };
	//rot = { 0.0f,0.0f,0.0f };
	//scale = { 210.0f,30.0f,210.0f };
	//entity = m_activeECS->CreateEntity("Floor", pos, rot, scale);
	////entity.AddComponent<Components::Renderer>(Renderer{ glm::mat4{1}, color, cubeModel });
	//entity.AddComponent<Components::PrimitiveRender>(PrimitiveRender
	//(
	//	{ "mud_diff.dds", "grass_diff.dds" },
	//	{ "mud_bump.dds", "grass_bump.dds" },
	//	{ "mud_norm.dds", "grass_norm.dds" },
	//	{ "mud_rough.dds", "grass_rough.dds" },
	//	floorMesh,
	//	4.0f,
	//	true
	//));
	//entity.AddComponent<Components::BoxCollider>(BoxCollider{ {0.1f, 0.1f, 0.1f }, {0.f, 0.f, 0.f} });

	const size_t second_last = Components::ParticleSystem::CurveVariable::m_MaxPoints - 2;
	const float increment = 1.0f / second_last;
	std::vector<ImVec2> curvePoints = {};
	// Init curve vertices
	for (size_t i = 0; i < second_last; i++)
	{
		float currX = i * increment;
		curvePoints.push_back({ currX, 1.0f });
	}
	curvePoints.push_back({ 1.0f, 1.0f });

	pos = { 15.0f, 15.0f, 10.f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 1.0f,1.0f,1.0f };
	entity = m_activeECS->CreateEntity("Ship", pos, rot, scale);
	ConstructModel(entity, shipModel, &shipModel->m_Meshes.begin()->second);
	entity.AddComponent<Components::Rigidbody>(Rigidbody{});
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1, 1, 1}, glm::vec3{0, 0, 0} });
	entity.AddComponent<Components::ParticleSystem>
		(
			ParticleSystem
			(
				glm::vec4{ 1,1,1,1 },		// Color
				glm::vec3{ 0,1,0 },			// Spawn dir
				glm::vec3{ 0,0.0f,0 },		// Force
				glm::vec3{ 1,1,1 },			// Billboard axis
				1.0f,	// SpawnRadius
				3.5f,	// Cone radius min
				4.2f,	// Cone radius max
				1.05f,	// Cone target scale
				0.42f,	// Delay
				0.01f,	// SubDelay
				1.00f,	// Min Life
				1.00f,	// Max Life
				{
					curvePoints,	// Curve points
					10.0f,			// Min Speed
					10.0f,			// Max Speed
					1.0f,			// Min Speed Mult
					1.0f,			// Max Speed Mult
				},
				{
					curvePoints,	// Curve points
					0.2f,			// Min Scale
					0.2f,			// Max Scale
					1.0f,			// Min Scale Mult
					1.0f,			// Max Scale Mult
				},
				SpriteAnimation(),
				"mud_rough.dds",		// TexID
				1,	// Spawn Count
				20, // Sub spawn count
				static_cast<int>(ParticleSystem::EMITTER_TYPE::CONE), // Emitter type, this was made an int for easy serialization idk kekw
				true,	// Loop
				false,	// Randomly rotate particles?
				true,   // Follow parent's position?
				true	// Leave "trails"
				)
			);

	pos = { 7.5f, 7.5f, 10.0f };
	rot = { 0.0f,0.0f,0.0f };
	scale = { 1.0f,1.0f,1.0f };

	entity = m_activeECS->CreateEntity("Enemy", pos, rot, scale);
	ConstructModel(entity, enemyModel, &enemyModel->m_Meshes.begin()->second);
	entity.AddComponent<Components::Rigidbody>(Rigidbody{});
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1, 1, 1}, glm::vec3{0, 0, 0} });

	/// Light
	pos = { 7.5f, 15.0f, 20.0f };
	glm::vec3 direction = -glm::normalize(pos);
	scale = { 1.0f,1.0f,1.0f }; // Affects light model and not the actual light size
	color = { 0.2f, 0.2f, 0.15f };
	float intensity = 13.0f;
	entity = m_activeECS->CreateEntity("Directional Light", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Directional_Light>(Directional_Light{ color, intensity, direction , 0.42f, 0.69f });
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color, glm::vec3(1.0f), false, true);

	intensity = 1.0f;
	pos = { 15.0f, 22.0f, 19.0f };
	color = { 1.0f, 1.0f, 1.0f };
	const float linear = 0.00069f;
	const float quadratic = 0.0042f;
	entity = m_activeECS->CreateEntity("White point light", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color, glm::vec3(1.0f), false, true);

	intensity = 4.2f;
	pos = { 26.0f, 10.0f, -16.5f };
	color = { 0.0f, 0.0f, 1.0f };
	entity = m_activeECS->CreateEntity("Blue light", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color, glm::vec3(1.0f), false, true);

	pos = { 21.0f, 10.0f, 10.0f };
	color = { 1.0f, 0.0f, 0.0f };
	entity = m_activeECS->CreateEntity("Red light", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color, glm::vec3(1.0f), false, true);

	pos = { -12.5, 10.0f, -10.0f };
	color = { 0.0f, 1.0f, 0.0f };
	entity = m_activeECS->CreateEntity("Green light", pos, glm::vec3{ 0 }, scale);
	entity.AddComponent<Components::Point_Light>(Point_Light{ color, intensity, linear, quadratic });
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color, glm::vec3(1.0f), false, true);

	//Test Object with body
	pos = { 3.0f, 1.f, 0.0f };
	color = { 0.0f, 1.0f, 1.0f };
	scale = { 0.5f, 0.5f, 0.5f };
	entity = m_activeECS->CreateEntity("Test Rigidbody", pos, glm::vec3{ 0 }, scale);
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color);
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1.f, 1.f, 1.f}, glm::vec3{0.f, 0.f, 0.f} });
	//entity.AddComponent<Components::SphereCollider>(SphereCollider{ glm::vec3{0.f}, 1.f });
	entity.AddComponent<Components::Rigidbody>(Rigidbody{ 1.f, 0.f, false, true });

	//Test movable controllable body
	pos = { -3.0f, 1.f, 0.0f };
	color = { 0.0f, 1.0f, 1.0f };
	scale = { 0.5f, 0.5f, 0.5f };
	entity = m_activeECS->CreateEntity("Test Rigidbody2", pos, glm::vec3{ 0 }, scale);
	ConstructModel(entity, sphereModel, &sphereModel->m_Meshes.begin()->second, color);
	entity.AddComponent<Components::BoxCollider>(BoxCollider{ glm::vec3{1.f, 1.f, 1.f}, glm::vec3{0.f, 0.f, 0.f} });
	entity.AddComponent<Components::Rigidbody>(Rigidbody{ 1.f, 0.f, false, true });

	/// FONT
	pos = { 0.0f, 10.0, -10.0f };
	rot = { 0.0f, 0.0f, 0.0f };
	scale = { 42.0f, 42.0f, 42.0f };
	entity = m_activeECS->CreateEntity("World font", pos, rot, scale);
	entity.AddComponent<Components::Text>(Text{ {1.0f, 0.0f, 0.0f}, "Ruda", "This is a very big text", false });

	pos = { -1.0f, 0.8f, 0.0f };
	rot = { 0.0f, 0.0f, 0.0f };
	scale = { 1.0f, 1.0f, 1.0f };
	entity = m_activeECS->CreateEntity("Screen font", pos, rot, scale);
	entity.AddComponent<Components::Text>(Text{ {1.0f, 0.0f, 1.0f}, "Ruda", "Screen Font", true });

	/// Camera
	pos = { 15.0f, 10.0f, 45.0f };
	color = { 0.9f, 0.5f, 0.2f };
	entity = m_activeECS->CreateEntity("Game Camera", pos, rot, scale);
	entity.AddComponent<Components::Camera>(true);
	ConstructModel(entity, cubeModel, &cubeModel->m_Meshes.begin()->second, color, glm::vec3(1.0f), false, true);

	/// Canvas test
	pos = { 0.0f, 0.0f, -1.0f };
	scale = { 1.0f, 1.0f, 1.0f };
	entity = m_activeECS->CreateEntity("Canvas", pos, rot, scale);

	pos = { -0.8, 0.5f, 0.0f };
	color = { 1.0f, 1.0f, 1.0f };
	scale = { 0.1f, 0.1f, 0.1f };
	auto child = m_activeECS->CreateChild(entity.GetID(), "Canvas Image 1", pos, rot, scale);
	// Simulate inspector set texture
	//PP::TextureResource::UseTexture("rocks_diff.dds");
	//child.AddComponent<Components::Canvas>(Canvas{ {color, 1.0f}, PP::TextureResource::GetUsedTextureID("rocks_diff.dds") });
	child.AddComponent<Components::Canvas>(Canvas{ {color, 1.0f}, "ParticleTest.dds", true });

	pos = { 0.0f, 0.0f, 0.0f };
	color = { 1.0f, 1.0f, 1.0f };
	scale = { 1.0f, 1.0f, 1.0f };
	child = m_activeECS->CreateChild(entity.GetID(), "Canvas Image 2", pos, rot, scale);
	// Simulate inspector set texture
	//PP::TextureResource::UseTexture("snow_diff.dds");
	//child.AddComponent<Components::Canvas>(Canvas{ {color, 1.0f}, PP::TextureResource::GetUsedTextureID("snow_diff.dds") });
	child.AddComponent<Components::Canvas>(Canvas{ {color, 1.0f}, "ParticleTest.dds", true });

	//Vinceen testing code
	//auto _ra = m_activeECS->view<Transform>(entt::exclude_t<Renderer>());
	//for (auto ent : _ra)
	//{
	//	std::cout << (entt::id_type)ent << std::endl;
	//}

	//int k = 0;
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
	//m_eventBus->listen(&m_sScriptSystem, &ScriptSystem::OnTriggerEnterEvent);
	//m_eventBus->listen(&m_sScriptSystem, &ScriptSystem::OnTriggerExitEvent);

	m_eventBus->listen(&SSH::OnTriggerEnterEvent);
	m_eventBus->listen(&SSH::OnTriggerExitEvent);
	m_eventBus->listen(&Scripting::GameScript::OnTriggerEnterEvent);
}

void Application::UpdateTransforms(float _Dt)
{
	//auto lol_id = m_activeECS->FindEntityWithName("Green light");

	// Debug with editor cam
	//gameCamPos = PP::CameraResource::GetCamera("EDITOR")->m_Position;

	/// Height map transform
	auto hmd_view = m_activeECS->view<Transform, HeightMapDebugger>();
	auto heightMap_view = m_activeECS->view<Transform, PrimitiveRender>();
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

	///// TEMP FIX FOR CANVAS ID
	//auto b_results = m_activeECS->view<Components::Canvas, Components::Transform>();
	//for (auto& e : b_results)
	//{
	//	auto& canvas = b_results.get<Components::Canvas>(e);
	//	auto rawID = PP::TextureResource::m_TexturePool[canvas.m_TexName];
	//	canvas.m_TexID = static_cast<int>(PP::TextureResource::m_UsedTextures[rawID]);
	//}

	//Update transform matrix of all gameobject
	m_sGeneralSystem.UpdateTransforms();

	/// Camera tranforms
	glm::vec3 gameCamPos = glm::vec3{ 0.0f };
	auto camView = m_activeECS->view<Transform, Camera>();
	{
		for (auto& entity : camView)
		{
			auto& camera = m_activeECS->GetReg().get<Camera>(entity);
			auto& transform = m_activeECS->GetReg().get<Transform>(entity);

			// If active update its projection & view;
			if (camera.m_Active)
			{
				gameCamPos = transform.GetGlobalPosition();
				const glm::vec2 windowSize = { PP::Window::m_Width, PP::Window::m_Height };
				//PP::Camera::GetUpdatedVec(camera.m_Yaw, camera.m_Pitch, camera.m_Up, camera.m_Right, camera.m_Front);
				//PP::Camera::GetUpdatedProjection(windowSize, camera.m_Zoom, camera.m_Near, camera.m_Far, camera.m_Projection);
				//PP::Camera::GetUpdatedView(gameCamPos, gameCamPos + camera.m_Front, camera.m_Up, camera.m_View);
				PP::Camera4D::UpdateVectors(camera.m_Yaw, camera.m_Pitch, camera.m_Roll, camera.m_Front, camera.m_Right, camera.m_Up, camera.m_Orientation);
				PP::Camera4D::UpdateProjection(windowSize, camera.m_Near, camera.m_Far, camera.m_Fov, camera.m_Projection);
				PP::Camera4D::UpdateOrthographic(windowSize, camera.m_Far, camera.m_Orthographic);
				PP::Camera4D::GetView(gameCamPos, camera.m_Orientation, camera.m_View);
				PPA::AudioEngine::UpdateListenerPosition(gameCamPos, camera.m_Front, camera.m_Up, PhysicsDLC::Vector::Zero);
			}
		}
	}

	/// 3D instance transforms
	// Clear old instance data
	PP::MeshInstance::ResetCount();
	for (auto& model : PP::ModelResource::m_ModelPool)
	{
		for (auto& mesh : model.second->m_Meshes)
		{
			mesh.second.m_Instances.clear();
		}
	}
	// Set instance data
	auto instanceView = m_activeECS->view<Transform, Renderer>();
	for (auto& entity : instanceView)
	{
		auto& renderer = m_activeECS->GetReg().get<Renderer>(entity);
		auto& transform = m_activeECS->GetReg().get<Transform>(entity);
		if (renderer.m_Mesh != nullptr)
		{
			auto* mesh = renderer.m_Mesh;
			int useTex = mesh->m_Textures.size() == 0 ? 0 : 1;
			mesh->m_Instances.push_back({ transform.m_ModelMtx, renderer.m_ColorTint, renderer.m_EmissiveTint, renderer.m_UseLight, useTex, renderer.m_EditorDrawOnly });
		}
	}
	// Update meshes
	for (auto& model : PP::ModelResource::m_ModelPool)
	{
		for (auto& mesh : model.second->m_Meshes)
		{
			mesh.second.RebindMesh3D();
		}
	}

	/// Start of instanced objects
	// See if texture list has to be updated
	if (!PP::TextureResource::m_Updated)
	{
		//printf("Clear\n");
		PP::TextureResource::m_UsedTextures.clear();
		PP::TextureResource::m_UsedTexCount = 0;
	}

	/// Particle transforms
	auto particleView = m_activeECS->view<Transform, ParticleSystem>();
	{
		for (auto& entity : particleView)
		{
			auto& transform = m_activeECS->GetReg().get<Transform>(entity);
			auto& pSys = m_activeECS->GetReg().get<ParticleSystem>(entity);

			//If to be updated
			auto& usedTex = PP::TextureResource::m_UsedTextures;
			if (!PP::TextureResource::m_Updated || !usedTex[pSys.m_TexName].m_Used)
			{
				if (!usedTex[pSys.m_TexName].m_Used)
				{
					pSys.Clear();
					//If not in used yet toggle it to be in use
					usedTex[pSys.m_TexName].m_ID = static_cast<int>(PP::TextureResource::m_TexturePool[pSys.m_TexName]);
					usedTex[pSys.m_TexName].m_MappedID = PP::TextureResource::m_UsedTexCount;
					usedTex[pSys.m_TexName].m_Used = true;
					PP::TextureResource::m_UsedTexCount++;
					//printf("Mapped ID: %d|%s| Generated ID: %d\n", usedTex[pSys.m_TexName].m_MappedID, pSys.m_TexName.c_str(), usedTex[pSys.m_TexName].m_ID);
					

					//printf("%s | %d | %d \n", pSys.m_TexName.c_str(), usedTex[pSys.m_TexName].m_ID), usedTex[pSys.m_TexName].m_MappedID;
				}
			}
			pSys.m_TexID = usedTex[pSys.m_TexName].m_MappedID;
			pSys.Update(_Dt, transform, gameCamPos);
		}
	}

	/// Canvas
	auto canvasView = m_activeECS->view<Transform, Canvas>();
	for (auto it : canvasView)
	{
		auto& transform = canvasView.get<Transform>(it);
		auto& canvas = canvasView.get<Canvas>(it);
		canvas.m_SpriteAnimation.Update(_Dt);

		// If to be updated or if not in used yet toggle it to be in use
		auto& usedTex = PP::TextureResource::m_UsedTextures;
		if (!PP::TextureResource::m_Updated || !usedTex[canvas.m_TexName].m_Used)
		{
			if (!usedTex[canvas.m_TexName].m_Used)
			{
				usedTex[canvas.m_TexName].m_ID = static_cast<int>(PP::TextureResource::m_TexturePool[canvas.m_TexName]);
				usedTex[canvas.m_TexName].m_MappedID = PP::TextureResource::m_UsedTexCount;
				usedTex[canvas.m_TexName].m_Used = true;
				PP::TextureResource::m_UsedTexCount++;
				//printf("Mapped ID: %d|%s| Generated ID: %d\n", usedTex[canvas.m_TexName].m_MappedID, canvas.m_TexName.c_str(), usedTex[canvas.m_TexName].m_ID);
				// Update canvas ID
				
				//printf("%s | %d | %d \n", canvas.m_TexName.c_str(), usedTex[canvas.m_TexName].m_ID, usedTex[canvas.m_TexName].m_MappedID);
			}
		}

		canvas.m_TexID = usedTex[canvas.m_TexName].m_MappedID;

		/// Scale with window size
		glm::mat4 model = glm::mat4{ 1 };
		if (canvas.m_Ortho)
		{
			glm::vec3 pos = {};
			glm::vec3 rot = {};
			glm::vec3 scale = {};

			// To maintain parent transform
			ImGuizmo::DecomposeMatrixToComponents
			(
				glm::value_ptr(transform.m_ModelMtx),
				glm::value_ptr(pos),
				glm::value_ptr(rot),
				glm::value_ptr(scale)
			);

			// Decompose to apply mapping
			model = glm::translate(model, { pos.x * PP::Window::m_Width, pos.y * PP::Window::m_Height, pos.z });
			model = glm::rotate(model, glm::radians(rot.x), { 1,0,0 });
			model = glm::rotate(model, glm::radians(rot.y), { 0,1,0 });
			model = glm::rotate(model, glm::radians(rot.z), { 0,0,1 });
			// Disregard aspect ratio so width for x and y
			model = glm::scale(model, { scale.x * PP::Window::m_Width, scale.y * PP::Window::m_Height * PP::Window::m_TargetAspect, 1.0f });
		}
		else
		{
			model = transform.m_ModelMtx;
		}

		PP::MeshInstance::SetInstance
		(
			PP::InstanceData
			{ 
				model,
				canvas.m_Color,
				canvas.m_SpriteAnimation.m_Tiling,
				canvas.m_SpriteAnimation.m_UV_Offset,
				canvas.m_TexID,
				canvas.m_Ortho,
				canvas.m_Ortho, // If ortho, show in game only since it blocks the screen, else can show it in the world
			}
		);

		// Move it to the front so its rendered first
		if (canvas.m_ForceAlpha)
		{
			PP::MeshInstance::ForceAlpha();
		}
	}

	// Send the forced alpha object to be rendered last
	PP::MeshInstance::AlphaSwap();

	// Would always be set to true
	PP::TextureResource::m_Updated = true;

	//delete entity in the delete set
	m_sGeneralSystem.DisableEntities();
	m_sGeneralSystem.EnableEntities();
	m_sGeneralSystem.DeleteEntities();
}

void Application::UpdateModelRef(std::vector<std::string>& _EditedModels)
{
	auto results = m_activeECS->view<Components::Renderer>();

	// Over all edited objects
	for (auto it : _EditedModels)
	{
		for (const auto& e : results)
		{
			// Clear previous meshes to null, since new model might not have that many meshes
			auto& it_Rend = results.get<Components::Renderer>(e);
			if (it_Rend.m_RenderModel->m_Model_key == it)
			{
				it_Rend.m_Mesh = nullptr;
			}
		}
	}
	// When done
	_EditedModels.clear();
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

	// AO
	PP::Renderer::AOPass(m_activeECS->GetReg(), true);
	PP::Renderer::EndBuffer();
	PP::Renderer::AOBlurPass();
	PP::Renderer::EndBuffer();

	// Where to draw the gpass FB to
	PP::Renderer::PostProcess();
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
	auto results = m_activeECS->view<Renderer>();

	// Models for Gpass
	PP::Renderer::StartGBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::Draw(m_activeECS->GetReg(), nullptr, PP::Renderer::m_EditorCamDebug);
	PP::Renderer::EndBuffer();

	// AO
	PP::Renderer::AOPass(m_activeECS->GetReg(), PP::Renderer::m_EditorCamDebug);
	PP::Renderer::EndBuffer();
	PP::Renderer::AOBlurPass();
	PP::Renderer::EndBuffer();

	// Where to draw the gpass FB to
	PP::Renderer::PostProcess();
	PP::Renderer::ClearBuffer();
	PP::Renderer::GLightPass(m_activeECS->GetReg(), PP::Renderer::m_EditorCamDebug);
	PP::Renderer::EndBuffer();

	PP::Renderer::BlurPass();
	PP::Renderer::StartGameBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::HDRPass(true);
	PP::Renderer::EndBuffer();
}

void Application::DrawScreen()
{
	PP::Renderer::EndBuffer();
	PP::Renderer::ClearBuffer();
	PP::Renderer::DrawScreen();
}

void Application::DrawImGUI()
{
	PP::Renderer::ClearBuffer();
	PPD::ImguiHelper::DrawImgui();
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
		case PogplantDriver::Application::APPLICATIONSTATE::PREFAB_EDITOR:
		{
			UpdatePrefabState(c_deltaTime);
			RenderEditorState();
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
		//PPI::InputSystem::PollEvents();
		PP::Renderer::SwapBuffer();

		if (m_nextAppState != m_appState)
		{
			switch (m_appState)
			{
			case APPLICATIONSTATE::EDITOR:
				LeaveEditorState();
				break;
			case APPLICATIONSTATE::PLAY:
				LeavePlayState();
				break;
			case APPLICATIONSTATE::PREFAB_EDITOR:
				LeavePrefabState();
				break;
			default:
				assert(false);
			}

			switch (m_nextAppState)
			{
			case APPLICATIONSTATE::EDITOR:
				EnterEditorState();
				break;
			case APPLICATIONSTATE::PLAY:
				if (m_appState == APPLICATIONSTATE::EDITOR)
					EnterPlayState("Resources/tmp");
				break;
			case APPLICATIONSTATE::PREFAB_EDITOR:
				EnterPrefabState();
				break;
			case APPLICATIONSTATE::NEWSCENETRANSITION:
				EnterPlayState(m_genericFilePath);

				m_nextAppState = APPLICATIONSTATE::PLAY;
				break;
			default:
				assert(false);
			}

			m_appState = m_nextAppState;
		}

		c_deltaTime = c_dtTimer.getElapsedTimePrecise();
	}
}

void PogplantDriver::Application::LoadScene(const std::string& newScene)
{
	m_nextAppState = APPLICATIONSTATE::NEWSCENETRANSITION;
	m_genericFilePath = newScene;
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
	case PogplantDriver::Application::APPLICATIONSTATE::PREFAB_EDITOR:
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
