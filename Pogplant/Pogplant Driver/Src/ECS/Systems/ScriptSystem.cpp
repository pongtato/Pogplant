/*****************************************************************************/
/*!
\file	ScriptSystem.cpp
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	Scripting system that utilizes Mono for c# scripting

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

#include "ScriptSystem.h"
#include "ScriptSystemHelper.h"
#include "ScriptResource.h"

#include "../../Input/InputSystem.h"
#include "../../Input/GLFWInput.h"
#include "../../Pogplant/Src/DebugDraw.h"
#include "../../GameScript.h"
#include "../Components/GenericComponents.h"


bool ScriptSystem::isReload = false;
ECS* ScriptSystem::m_ecs = nullptr;

// Helper to read binary data
std::vector<char> ReadRawBin(const std::string& filePath)
{
	// Check if file exists & read to buffer
	std::ifstream inBuffer(filePath, std::ios::binary);

	if (inBuffer.fail())
	{
		return std::vector<char>{};
	}
	size_t size = std::filesystem::file_size(filePath);
	std::vector<char> data(size);

	inBuffer.seekg(0, std::ios::beg);
	inBuffer.read(&data[0], size);
	inBuffer.close();

	return data;
}

ScriptSystem::ScriptSystem()
{
	mono_set_dirs("Mono\\lib", "Mono\\etc");
	// Root domain
	m_ptrMonoDomain = mono_jit_init("ScriptSystem");

	// Add Script names here
	//ScriptResource::m_allScriptNames.push_back("PlayerScript");
	//ScriptResource::m_allScriptNames.push_back("FollowSpline");
	//ScriptResource::m_allScriptNames.push_back("EnemyManager");
	//ScriptResource::m_allScriptNames.push_back("EncounterManager");
	//ScriptResource::m_allScriptNames.push_back("BaseTurret");
	//ScriptResource::m_allScriptNames.push_back("BaseEnemy");
	//ScriptResource::m_allScriptNames.push_back("MoveStuff");
	// Load dll into memory
	//LoadMemory();
}

ScriptSystem::~ScriptSystem()
{
	Cleanup();
}

void ScriptSystem::InitEditor(ECS* ecs)
{
	m_ecs = ecs;

	ScriptResource::m_allScriptNames.clear();
	ScriptResource::m_allScriptNames.shrink_to_fit();
	InitLoad();
}

void ScriptSystem::InitPlayState(ECS* ecs)
{
	m_ecs = ecs;

	LoadMemory();
}

void ScriptSystem::Update(float dt)
{
	auto entities = m_ecs->view<Components::Scriptable, Components::Rigidbody, Components::Transform, Components::Name>();

	for (auto entity : entities)
	{
		auto& scriptable = entities.get<Components::Scriptable>(entity);
		auto& rigidbody = entities.get<Components::Rigidbody>(entity);
		auto& transform = entities.get<Components::Transform>(entity);
		auto& name = entities.get<Components::Name>(entity);

		for (auto& scripts : scriptable.m_ScriptTypes)
		{
			if (scripts.second == false)
			{
				AddScriptToEntity(entity);
				SSH::InvokeFunction(scripts.first, "Init", entity, static_cast<std::uint32_t>(entity));
				SSH::InvokeFunction(scripts.first, "Start", entity);
				scripts.second = true;
				std::cout << "Entity [" << name.m_name << "] has started script [" << scripts.first << "]" << std::endl;
			}

			SSH::InvokeFunction(scripts.first, "Update", entity, transform, rigidbody, dt);
		}
	}

	SSH::UpdateDestroyedEntities();
}

void ScriptSystem::LateUpdate(float dt)
{
	auto entities = m_ecs->view<Components::Scriptable, Components::Rigidbody, Components::Transform, Components::Name>();

	for (auto& entity : entities)
	{
		auto& scriptable = entities.get<Components::Scriptable>(entity);
		auto& rigidbody = entities.get<Components::Rigidbody>(entity);
		auto& transform = entities.get<Components::Transform>(entity);
		auto& name = entities.get<Components::Name>(entity);

		for (auto& scripts : scriptable.m_ScriptTypes)
		{
			if (scripts.second == false)
			{
				AddScriptToEntity(entity);
				SSH::InvokeFunction(scripts.first, "Init", entity, static_cast<std::uint32_t>(entity));
				SSH::InvokeFunction(scripts.first, "Start", entity);
				scripts.second = true;
				std::cout << "Entity [" << name.m_name << "] has started script [" << scripts.first << "]" << std::endl;
			}

			SSH::InvokeFunction(scripts.first, "LateUpdate", entity, transform, rigidbody, dt);
		}
	}

	SSH::UpdateDestroyedEntities();
}

void ScriptSystem::SetReload(bool _isReload)
{
	isReload = _isReload;
}

MonoMethod* ScriptSystem::FindMethod(MonoClass* klass, const std::string& methodName, int params)
{
	MonoMethod* method = mono_class_get_method_from_name(klass, methodName.c_str(), params);
	if (!method)
	{
		return nullptr;
	}
	return method;
}

void ScriptSystem::LoadMemory()
{
	// My saved domain will be replaced by new domain
	MonoDomain* newDomain = mono_domain_create_appdomain(const_cast<char*>("Scripting"), NULL);
	mono_domain_set(newDomain, false);
	std::cout << "Loading Scripts" << std::endl;
	// Create the mono domain
	if (newDomain)
	{
		// Used to check if loaded properly
		MonoImageOpenStatus status;

		// Have to manually load the system assemblies before loading the scripts
		std::string sysPath = "Mono/lib/mono/4.5/System.dll";
		std::string sysCorePath = "Mono/lib/mono/4.5/System.Core.dll";

		std::vector<char> sysData = ReadRawBin(sysPath);
		std::vector<char> sysCoreData = ReadRawBin(sysCorePath);

		MonoImage* sysImg = mono_image_open_from_data_with_name(sysData.data(), static_cast<uint32_t>(sysData.size()), true, &status, false, sysPath.c_str());
		assert(status == MONO_IMAGE_OK);
		MonoAssembly* sysAssem = mono_assembly_load_from_full(sysImg, sysPath.c_str(), &status, false);
		assert(status == MONO_IMAGE_OK);
		(void)sysAssem;

		MonoImage* sysCoreImg = mono_image_open_from_data_with_name(sysCoreData.data(), static_cast<uint32_t>(sysCoreData.size()), true, &status, false, sysCorePath.c_str());
		assert(status == MONO_IMAGE_OK);
		MonoAssembly* sysCoreAssem = mono_assembly_load_from_full(sysCoreImg, sysCorePath.c_str(), &status, false);
		assert(status == MONO_IMAGE_OK);
		(void)sysCoreAssem;

		// Load from memory
		std::string dllPath = "Resources/DLL/Scripting.dll";
		std::vector<char> asmData = ReadRawBin(dllPath);
		m_ptrGameAssemblyImage = mono_image_open_from_data_with_name(asmData.data(), static_cast<uint32_t>(asmData.size()), true, &status, false, dllPath.c_str());

		if (m_ptrGameAssemblyImage && status == MONO_IMAGE_OK)
		{
			m_ptrGameAssembly = mono_assembly_load_from_full(m_ptrGameAssemblyImage, dllPath.c_str(), &status, false);
			if (m_ptrGameAssembly && status == MONO_IMAGE_OK)
			{
				m_ptrMainEntryClass = mono_class_from_name(m_ptrGameAssemblyImage, m_namespace.c_str(), "Scripting");

				assert(m_ptrMainEntryClass && "MonoClass* monoMainClass does not exist, this means that the main function is bogged in c#.");

				BindFunctions();
				if (m_ptrMainEntryClass)
				{
					auto entities = m_ecs->view<Components::Scriptable, Components::Transform, Components::Name>();
					for (auto& entity : entities)
					{
						AddScriptToEntity(entity);
					}
				}
			}
		}
	}
}

void ScriptSystem::InitLoad()
{
	// My saved domain will be replaced by new domain
	MonoDomain* newDomain = mono_domain_create_appdomain(const_cast<char*>("Scripting"), NULL);
	mono_domain_set(newDomain, false);
	std::cout << "Editor Init Scripts" << std::endl;
	// Create the mono domain
	if (newDomain)
	{
		// Load from memory
		MonoImageOpenStatus status;
		std::string dllPath = "Resources/DLL/Scripting.dll";
		std::vector<char> asmData = ReadRawBin(dllPath);
		m_ptrGameAssemblyImage = mono_image_open_from_data_with_name(asmData.data(), static_cast<uint32_t>(asmData.size()), true, &status, false, dllPath.c_str());

		if (m_ptrGameAssemblyImage && status == MONO_IMAGE_OK)
		{
			m_ptrGameAssembly = mono_assembly_load_from_full(m_ptrGameAssemblyImage, dllPath.c_str(), &status, false);
			if (m_ptrGameAssembly && status == MONO_IMAGE_OK)
			{
				m_ptrMainEntryClass = mono_class_from_name(m_ptrGameAssemblyImage, m_namespace.c_str(), "Scripting");

				assert(m_ptrMainEntryClass && "MonoClass* monoMainClass does not exist, this means that the main function is bogged in c#.");

				BindFunctions();
				if (m_ptrMainEntryClass)
				{
					std::string scriptName{"ScriptsContainer"};
					MonoClass* monoclass = mono_class_from_name(m_ptrGameAssemblyImage, m_namespace.c_str(), scriptName.c_str());
					std::string excep = "MonoClass* monoclass does not exist, this means that function " + scriptName + " is bogged in c#.";
					assert(monoclass && excep.c_str());
					(void)monoclass;

					// Main method describe
					std::string fullname = '.' + m_namespace + ':' + scriptName.c_str() + "()";
					MonoMethodDesc* ptrMainMethodDesc = mono_method_desc_new(fullname.c_str(), false);
					excep = "MonoMethodDesc* ptrMainMethodDesc does not exist, " + fullname + " is bogged in c#.";
					assert(ptrMainMethodDesc && excep.c_str());

					// Find the main in mainclass
					MonoMethod* ptrMainMethod = mono_method_desc_search_in_class(ptrMainMethodDesc, m_ptrMainEntryClass);
					assert(ptrMainMethod && "MainMonoMethod* ptrMainMethod does not exist, probably missing main.cs functions in c#");

					MonoObject* ptrExObject = nullptr;
					// this is the scriptcontainer class
					MonoObject* ptrGameObject = mono_runtime_invoke(ptrMainMethod, nullptr, nullptr, &ptrExObject);
					excep = "MonoObject* ptrGameObject does not exist, the method " + fullname + " to call the script in c# is invalid.";
					assert(ptrGameObject && excep.c_str());

					// Call the getscript
					MonoClass* klass = mono_object_get_class(ptrGameObject);
					MonoMethod* method = mono_class_get_method_from_name(klass, "GetScriptNamesSize", -1);

					MonoObject* retContainerSize = mono_runtime_invoke(method, ptrGameObject, nullptr, nullptr);
					int len = *(int*)mono_object_unbox(retContainerSize);
					//ScriptResource::m_allScriptNames.resize(len);

					for (int i = 0; i < len; ++i)
					{
						void* args[] = { &i };

						MonoMethod* getNameMethod = mono_class_get_method_from_name(klass, "GetScriptNameElement", -1);
						MonoObject* retStringName = mono_runtime_invoke(getNameMethod, ptrGameObject, args, nullptr);
						MonoString* monoStr = mono_object_to_string(retStringName, nullptr);
						std::string scrName{ mono_string_to_utf8(monoStr) };
						ScriptResource::m_allScriptNames.push_back(scrName);
					}

					// Garbage Collection Handle for the game object
					uint32_t m_gameObjectGCHandle = mono_gchandle_new(ptrGameObject, false);

					//remove warning, idk dude
					(void)m_gameObjectGCHandle;

					// Exception hit
					if (ptrExObject)
					{
						MonoString* exString = mono_object_to_string(ptrExObject, nullptr);
						const char* exCString = mono_string_to_utf8(exString);
						std::cout << exCString << std::endl;
					}

					// Free desc
					mono_method_desc_free(ptrMainMethodDesc);
				}
			}
		}
	}
}

void ScriptSystem::Cleanup()
{
	if (m_ptrMonoDomain)
	{
		mono_jit_cleanup(m_ptrMonoDomain);
	}
}

void ScriptSystem::Unload()
{
	// Current domain will be unloaded
	if (m_ptrGameAssemblyImage != nullptr)
	{
		std::cout << "Unloading Scripts" << std::endl;
		mono_image_close(m_ptrGameAssemblyImage);
		MonoDomain* domainToUnload = mono_domain_get();
		if (domainToUnload && domainToUnload != mono_get_root_domain())
		{
			mono_domain_set(mono_get_root_domain(), false);
			mono_domain_unload(domainToUnload);
		}
	}
}

void ScriptSystem::BindFunctions()
{
	// GLFW calls
	mono_add_internal_call("Scripting.InputUtility::onKeyTriggered(int)", &PPI::GLFWInputManager::onKeyTriggeredMono);
	mono_add_internal_call("Scripting.InputUtility::onKeyReleased(int)", &PPI::GLFWInputManager::onKeyReleasedMono);
	mono_add_internal_call("Scripting.InputUtility::onKeyHeld(int)", &PPI::GLFWInputManager::onKeyHeldMono);

	// Input System calls
	mono_add_internal_call("Scripting.InputUtility::onKeyTriggered(string)", &PPI::InputSystem::onKeyTriggeredMono);
	mono_add_internal_call("Scripting.InputUtility::onKeyReleased(string)", &PPI::InputSystem::onKeyReleasedMono);
	mono_add_internal_call("Scripting.InputUtility::onKeyHeld(string)", &PPI::InputSystem::onKeyHeldMono);
	mono_add_internal_call("Scripting.InputUtility::GetAxis(string)", &PPI::InputSystem::GetAxisMono);

	// Debug Draw
	mono_add_internal_call("Scripting.DebugDraw::DebugSphere", &Pogplant::DebugDraw::DebugSphere);
	
	// Debug Text
	mono_add_internal_call("Scripting.DebugUtilities::LogToEditor", &SSH::LogToEditor);

	//Game Utils
	mono_add_internal_call("Scripting.GameUtilities::CheckBounds", &Scripting::CheckBounds);
	mono_add_internal_call("Scripting.GameUtilities::Instantiate", &Scripting::Instantiate);
	mono_add_internal_call("Scripting.GameUtilities::FollowPlayerCam", &Scripting::FollowPlayerCam);
	mono_add_internal_call("Scripting.GameUtilities::FirePlayerBullet", &Scripting::FirePlayerBullet);
	mono_add_internal_call("Scripting.GameUtilities::FireEnemyBullet", &Scripting::FireEnemyBullet);
	mono_add_internal_call("Scripting.GameUtilities::SpawnStaticExplosion", &Scripting::SpawnStaticExplosion);
	mono_add_internal_call("Scripting.GameUtilities::UpdatePlayerHealth_UI", &Scripting::UpdatePlayerHealth_UI);
	mono_add_internal_call("Scripting.GameUtilities::GetAlive", &Scripting::GetAlive);
	mono_add_internal_call("Scripting.GameUtilities::GetMousePos", &Scripting::GetMousePos);
	mono_add_internal_call("Scripting.GameUtilities::PlayerTakeDamage", &Scripting::PlayerTakeDamage);
	mono_add_internal_call("Scripting.GameUtilities::PlayEnemyDeathAnimation", &Scripting::PlayEnemyDeathAnimation);
	mono_add_internal_call("Scripting.GameUtilities::StartMissile", &Scripting::StartMissile);
	mono_add_internal_call("Scripting.GameUtilities::SetTurretFire", &Scripting::SetTurretFire);

	// ECS & Component st
	mono_add_internal_call("Scripting.ECS::CreateEntity", SSH::CreateEntity);
	mono_add_internal_call("Scripting.ECS::DestroyEntity", SSH::DestroyEntity);
	mono_add_internal_call("Scripting.ECS::CreateChild", SSH::CreateChild);
	mono_add_internal_call("Scripting.ECS::FindEntityWithName", SSH::FindEntityWithName);
	mono_add_internal_call("Scripting.ECS::FindChildEntityWithName", SSH::FindChildEntityWithName);
	mono_add_internal_call("Scripting.ECS::GetTransformECS", SSH::GetTransformECS);
	mono_add_internal_call("Scripting.ECS::SetTransformECS", SSH::SetTransformECS);
	mono_add_internal_call("Scripting.ECS::SetTransformParent", SSH::SetTransformParent);
	mono_add_internal_call("Scripting.ECS::GetTransformParent", SSH::GetTransformParent);
	mono_add_internal_call("Scripting.ECS::RemoveParentFrom", SSH::RemoveParentFrom);
	mono_add_internal_call("Scripting.ECS::GetGlobalPosition", SSH::GetGlobalPosition);
	mono_add_internal_call("Scripting.ECS::GetGlobalRotation", SSH::GetGlobalRotation);
	mono_add_internal_call("Scripting.ECS::PlayAudio", SSH::PlayAudio);
	mono_add_internal_call("Scripting.ECS::GetBoxColliderECS", SSH::GetBoxColliderECS);
	mono_add_internal_call("Scripting.ECS::GetTagECS", SSH::GetTagECS);
	mono_add_internal_call("Scripting.ECS::SetColliderBox", SSH::SetColliderBox);
	mono_add_internal_call("Scripting.ECS::GetChildCount", SSH::GetChildCount);
	mono_add_internal_call("Scripting.ECS::SetActive", SSH::SetActive);

	mono_add_internal_call("Scripting.GameObject::AddComponentTransform", SSH::AddComponentTransform);
	mono_add_internal_call("Scripting.GameObject::AddComponentRigidbody", SSH::AddComponentRigidbody);
	mono_add_internal_call("Scripting.GameObject::AddComponentRenderer", SSH::AddComponentRenderer);
	mono_add_internal_call("Scripting.GameObject::AddComponentBoxCollider", SSH::AddComponentBoxCollider);
	mono_add_internal_call("Scripting.GameObject::AddComponentSphereCollider", SSH::AddComponentSphereCollider);
	
	mono_add_internal_call("Scripting.GameObject::AddScript", SSH::AddScript);

	// For Components
	mono_add_internal_call("Scripting.ComponentHelper::GetModelKeysIC", SSH::GetModelKeysIC);
	mono_add_internal_call("Scripting.Transform::GetForwardVector", Scripting::GetForwardVector);
	mono_add_internal_call("Scripting.Transform::GetUpVector", SSH::GetUpVector);
	mono_add_internal_call("Scripting.Transform::LookAt", SSH::LookAt);
	mono_add_internal_call("Scripting.Transform::LookAtClamped", SSH::LookAtClamped);
	
	// Changing scenes
	mono_add_internal_call("Scripting.GameUtilities::LoadScene", SSH::LoadScene);
	mono_add_internal_call("Scripting.GameUtilities::ExitScene", SSH::ExitScene);
}

void ScriptSystem::AddScriptToEntity(const entt::entity& entity)
{
	auto& scriptable = m_ecs->GetReg().get<Components::Scriptable>(entity);

	for (auto& scripts : scriptable.m_ScriptTypes)
	{
		MonoClass* monoclass = mono_class_from_name(m_ptrGameAssemblyImage, m_namespace.c_str(), scripts.first.c_str());
		std::string excep = "MonoClass* monoclass does not exist, this means that function " + scripts.first + " is bogged in c#.";
		assert(monoclass && excep.c_str());
		(void)monoclass;

		// Main method describe
		std::string fullname = '.' + m_namespace + ':' + scripts.first.c_str() + "()";
		MonoMethodDesc* ptrMainMethodDesc = mono_method_desc_new(fullname.c_str(), false);
		excep = "MonoMethodDesc* ptrMainMethodDesc does not exist, " + fullname + " is bogged in c#.";
		assert(ptrMainMethodDesc && excep.c_str());

		// Find the main in mainclass
		MonoMethod* ptrMainMethod = mono_method_desc_search_in_class(ptrMainMethodDesc, m_ptrMainEntryClass);
		assert(ptrMainMethod && "MainMonoMethod* ptrMainMethod does not exist, probably missing main.cs functions in c#");

		MonoObject* ptrExObject = nullptr;
		MonoObject* ptrGameObject = mono_runtime_invoke(ptrMainMethod, nullptr, nullptr, &ptrExObject);
		excep = "MonoObject* ptrGameObject does not exist, the method " + fullname + " to call the script in c# is invalid.";
		assert(ptrGameObject && excep.c_str());

		// Garbage Collection Handle for the game object
		uint32_t m_gameObjectGCHandle = mono_gchandle_new(ptrGameObject, false);

		// Exception hit
		if (ptrExObject)
		{
			MonoString* exString = mono_object_to_string(ptrExObject, nullptr);
			const char* exCString = mono_string_to_utf8(exString);
			std::cout << exCString << std::endl;
		}

		// Entity ID then scriptName with monoObject. Lookup with entityID
		ScriptResource::m_MonoObjects[entity][scripts.first] = std::make_unique<MonoObjectWithGC>(m_gameObjectGCHandle, ptrGameObject);

		// Free desc
		mono_method_desc_free(ptrMainMethodDesc);
	}
}

void ScriptSystem::Reload()
{
	Unload();
	LoadMemory();
}
