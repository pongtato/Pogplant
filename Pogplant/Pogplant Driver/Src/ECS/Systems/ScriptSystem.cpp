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
	mono_set_dirs("..\\Libs\\Mono\\lib", "..\\Libs\\Mono\\etc");
	// Root domain
	m_ptrMonoDomain = mono_jit_init("ScriptSystem");

	// Add Script names here
	ScriptResource::m_allScriptNames.push_back("PlayerScript");
	ScriptResource::m_allScriptNames.push_back("FollowSpline");
	ScriptResource::m_allScriptNames.push_back("EnemyManager");
	ScriptResource::m_allScriptNames.push_back("EncounterManager");
	ScriptResource::m_allScriptNames.push_back("BaseTurret");
	ScriptResource::m_allScriptNames.push_back("BaseEnemy");
	// Load dll into memory
	//LoadMemory();
}

ScriptSystem::~ScriptSystem()
{
	Cleanup();
}

void ScriptSystem::Init(ECS* ecs)
{
	m_ecs = ecs;

	if (isReload == true)
	{
		Reload();
		isReload = false;
	}
}

void ScriptSystem::Update(float dt)
{
	auto entities = m_ecs->GetReg().view<Components::Scriptable, Components::Rigidbody, Components::Transform, Components::Name>();

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

			SSH::InvokeFunction(scripts.first, "Update", entity, transform, rigidbody, dt);
		}
	}
}

void ScriptSystem::LateUpdate(float dt)
{
	auto entities = m_ecs->GetReg().view<Components::Scriptable, Components::Rigidbody, Components::Transform, Components::Name>();

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
				BindFunctions();

				m_ptrMainEntryClass = mono_class_from_name(m_ptrGameAssemblyImage, m_namespace.c_str(), "Scripting");

				assert(m_ptrMainEntryClass && "MonoClass* monoMainClass does not exist, this means that the main function is bogged in c#.");

				if (m_ptrMainEntryClass)
				{
					auto entities = m_ecs->GetReg().view<Components::Scriptable, Components::Transform, Components::Name>();
					for (auto& entity : entities)
					{
						AddScriptToEntity(entity);
					}
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

	// Debug Draw
	mono_add_internal_call("Scripting.DebugDraw::DebugSphere", &Pogplant::DebugDraw::DebugSphere);

	//Game Utils
	mono_add_internal_call("Scripting.GameUtilities::CheckBounds", &Scripting::CheckBounds);
	mono_add_internal_call("Scripting.GameUtilities::FollowPlayerCam", &Scripting::FollowPlayerCam);
	mono_add_internal_call("Scripting.GameUtilities::FirePlayerBullet", &Scripting::FirePlayerBullet);
	mono_add_internal_call("Scripting.GameUtilities::FireEnemyBullet", &Scripting::FireEnemyBullet);

	// ECS & Component st
	mono_add_internal_call("Scripting.ECS::CreateEntity", SSH::CreateEntity);
	mono_add_internal_call("Scripting.ECS::DestroyEntity", SSH::DestroyEntity);
	mono_add_internal_call("Scripting.ECS::CreateChild", SSH::CreateChild);
	mono_add_internal_call("Scripting.ECS::FindEntityWithName", SSH::FindEntityWithName);

	mono_add_internal_call("Scripting.GameObject::AddComponentTransform", SSH::AddComponentTransform);
	mono_add_internal_call("Scripting.GameObject::AddComponentRigidbody", SSH::AddComponentRigidbody);
	mono_add_internal_call("Scripting.GameObject::AddComponentRenderer", SSH::AddComponentRenderer);
	mono_add_internal_call("Scripting.GameObject::AddComponentBoxCollider", SSH::AddComponentBoxCollider);
	mono_add_internal_call("Scripting.GameObject::AddComponentSphereCollider", SSH::AddComponentSphereCollider);
	mono_add_internal_call("Scripting.GameObject::AddScript", SSH::AddScript);

	mono_add_internal_call("Scripting.ComponentHelper::GetModelKeysIC", SSH::GetModelKeysIC);
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
		//assert(ptrMainMethod, "MainMonoMethod* ptrMainMethod does not exist, probably missing main.cs in c#");
		assert(ptrMainMethod && "MainMonoMethod* ptrMainMethod does not exist, probably missing main.cs functions in c#");

		MonoObject* ptrExObject = nullptr;
		MonoObject* ptrGameObject = mono_runtime_invoke(ptrMainMethod, nullptr, nullptr, &ptrExObject);
		excep = "MonoObject* ptrGameObject does not exist, the method " + fullname + " to call the script in c# is invalid.";
		assert(ptrGameObject && excep.c_str());

		// Garbage Collection Handle for the game object
		uint32_t m_gameObjectGCHandle = mono_gchandle_new(ptrGameObject, false);
		// Add to the map
		//m_MonoObjects[scriptName.c_str()] = std::make_unique<MonoObjectWithGC>(m_gameObjectGCHandle, ptrGameObject);
		//ScriptResource::m_MonoObjects[entity] = std::make_unique<MonoObjectWithGC>(m_gameObjectGCHandle, ptrGameObject);
		// 

		// Exception hit
		if (ptrExObject)
		{
			MonoString* exString = mono_object_to_string(ptrExObject, nullptr);
			const char* exCString = mono_string_to_utf8(exString);
			std::cout << exCString << std::endl;
		}

		// Entity ID then scriptName with monoObject. Lookup with entityID
		ScriptResource::m_MonoObjects[entity][scripts.first] = std::make_unique<MonoObjectWithGC>(m_gameObjectGCHandle, ptrGameObject);
		//scriptable.m_MonoObjects[scripts.first] = std::make_unique<MonoObjectWithGC>(m_gameObjectGCHandle, ptrGameObject);

		// Free desc
		mono_method_desc_free(ptrMainMethodDesc);
	}
}

void ScriptSystem::Reload()
{
	Unload();
	LoadMemory();
}