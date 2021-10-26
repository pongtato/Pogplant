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

#include "../Components/Components.h"
#include "../Components/DependantComponents.h"
#include "ScriptSystem.h"
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
	// Load dll into memory
	LoadMemory();
}

ScriptSystem::~ScriptSystem()
{
	Cleanup();
}

void ScriptSystem::Init(ECS* ecs)
{
	m_ecs = ecs;

	// Ghetto until I figure out CBs
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
			MonoObject* monoObj = m_MonoObjects[scripts.first]->m_MonoObject;
			if (!monoObj)
			{
				// Maybe log something here
				std::cout << "Script: " << scripts.first << " not found" << std::endl;
				continue;
			}

			MonoClass* klass = mono_object_get_class(monoObj);
			if (!klass)
			{
				// Maybe log something here
				std::cout << "MonoClass not found" << std::endl;
			}

			if (scripts.second == false)
			{
				MonoMethod* startMethod = FindMethod(klass, "Start");
				mono_runtime_invoke(startMethod, monoObj, nullptr, nullptr);
				scripts.second = true;
				std::cout << "Entity [" << name.m_name << "] has started script [" << scripts.first << "]" << std::endl;
			}

			MonoMethod* updateMethod = FindMethod(klass, "Update");
			void* args[] = { &transform, &rigidbody, &dt };
			mono_runtime_invoke(updateMethod, monoObj, args, nullptr);
		}
	}
}

void ScriptSystem::LateUpdate()
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
			MonoObject* monoObj = m_MonoObjects[scripts.first]->m_MonoObject;
			if (!monoObj)
			{
				// Maybe log something here
				std::cout << "Script: " << scripts.first << " not found" << std::endl;
				continue;
			}

			MonoClass* klass = mono_object_get_class(monoObj);
			if (!klass)
			{
				// Maybe log something here
				std::cout << "MonoClass not found" << std::endl;
			}

			if (scripts.second == false)
			{
				MonoMethod* startMethod = FindMethod(klass, "Start");
				mono_runtime_invoke(startMethod, monoObj, nullptr, nullptr);
				scripts.second = true;
				std::cout << "Entity [" << name.m_name << "] has started script [" << scripts.first << "]" << std::endl;
			}

			MonoMethod* updateMethod = FindMethod(klass, "LateUpdate");
			void* args[] = { &transform, &rigidbody };
			mono_runtime_invoke(updateMethod, monoObj, args, nullptr);
		}
	}
}

void ScriptSystem::SetReload(bool _isReload)
{
	isReload = _isReload;
}

void ScriptSystem::OnTriggerEnterEvent(std::shared_ptr<PPE::OnTriggerEnterEvent> onTriggerEnterEvent)
{
	auto script1 = m_ecs->GetReg().try_get<Components::Scriptable>(onTriggerEnterEvent.get()->m_entity1);
	auto script2 = m_ecs->GetReg().try_get<Components::Scriptable>(onTriggerEnterEvent.get()->m_entity2);

	if (script1)
	{
		for (auto& scripts : script1->m_ScriptTypes)
		{
			MonoObject* monoObj = m_MonoObjects[scripts.first]->m_MonoObject;
			if (!monoObj)
			{
				// Maybe log something here
				std::cout << "Script: " << scripts.first << " not found" << std::endl;
				continue;
			}

			MonoClass* klass = mono_object_get_class(monoObj);
			if (!klass)
			{
				// Maybe log something here
				std::cout << "MonoClass not found" << std::endl;
			}

			MonoMethod* startMethod = FindMethod(klass, "OnTriggerEnter");
			if (startMethod)
			{
				mono_runtime_invoke(startMethod, monoObj, nullptr, nullptr);
			}
		}
	}

	if (script2)
	{
		for (auto& scripts : script2->m_ScriptTypes)
		{
			MonoObject* monoObj = m_MonoObjects[scripts.first]->m_MonoObject;
			if (!monoObj)
			{
				// Maybe log something here
				std::cout << "Script: " << scripts.first << " not found" << std::endl;
				continue;
			}

			MonoClass* klass = mono_object_get_class(monoObj);
			if (!klass)
			{
				// Maybe log something here
				std::cout << "MonoClass not found" << std::endl;
			}

			MonoMethod* startMethod = FindMethod(klass, "OnTriggerEnter");
			if (startMethod)
			{
				mono_runtime_invoke(startMethod, monoObj, nullptr, nullptr);
			}
		}
	}
}

void ScriptSystem::OnTriggerExitEvent(std::shared_ptr<PPE::OnTriggerExitEvent> onTriggerExitEvent)
{
	auto script1 = m_ecs->GetReg().try_get<Components::Scriptable>(onTriggerExitEvent.get()->m_entity1);
	auto script2 = m_ecs->GetReg().try_get<Components::Scriptable>(onTriggerExitEvent.get()->m_entity2);

	if (script1)
	{
		for (auto& scripts : script1->m_ScriptTypes)
		{
			MonoObject* monoObj = m_MonoObjects[scripts.first]->m_MonoObject;
			if (!monoObj)
			{
				// Maybe log something here
				std::cout << "Script: " << scripts.first << " not found" << std::endl;
				continue;
			}

			MonoClass* klass = mono_object_get_class(monoObj);
			if (!klass)
			{
				// Maybe log something here
				std::cout << "MonoClass not found" << std::endl;
			}

			MonoMethod* startMethod = FindMethod(klass, "OnTriggerExit");
			if (startMethod)
			{
				mono_runtime_invoke(startMethod, monoObj, nullptr, nullptr);
			}
		}
	}

	if (script2)
	{
		for (auto& scripts : script2->m_ScriptTypes)
		{
			MonoObject* monoObj = m_MonoObjects[scripts.first]->m_MonoObject;
			if (!monoObj)
			{
				// Maybe log something here
				std::cout << "Script: " << scripts.first << " not found" << std::endl;
				continue;
			}

			MonoClass* klass = mono_object_get_class(monoObj);
			if (!klass)
			{
				// Maybe log something here
				std::cout << "MonoClass not found" << std::endl;
			}

			MonoMethod* startMethod = FindMethod(klass, "OnTriggerExit");
			if (startMethod)
			{
				mono_runtime_invoke(startMethod, monoObj, nullptr, nullptr);
			}
		}
	}
}

MonoMethod* ScriptSystem::FindMethod(MonoClass* klass,const std::string& methodName, int params)
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
				
				MonoClass* monoMainClass = mono_class_from_name(m_ptrGameAssemblyImage, m_namespace.c_str(), "Scripting");

				if (monoMainClass)
				{
					m_scriptNames.push_back("PlayerScript");
					m_scriptNames.push_back("EnemyScript");
					m_scriptNames.push_back("FollowSpline");

					for (auto& scriptName : m_scriptNames)
					{
						MonoClass* monoclass = mono_class_from_name(m_ptrGameAssemblyImage, m_namespace.c_str(), scriptName.c_str());

						if (monoclass)
						{
							// Main method describe
							std::string fullname = '.' + m_namespace + ':' + scriptName.c_str() + "()";
							MonoMethodDesc* ptrMainMethodDesc = mono_method_desc_new(fullname.c_str(), false);

							if (ptrMainMethodDesc)
							{
								// Find the main in mainclass
								MonoMethod* ptrMainMethod = mono_method_desc_search_in_class(ptrMainMethodDesc, monoMainClass);
								if (ptrMainMethod)
								{
									MonoObject* ptrExObject = nullptr;
									MonoObject* m_ptrGameObject = mono_runtime_invoke(ptrMainMethod, nullptr, nullptr, &ptrExObject);

									if (m_ptrGameObject)
									{
										// Garbage Collection Handle for the game object
										uint32_t m_gameObjectGCHandle = mono_gchandle_new(m_ptrGameObject, false);
										// Add to the map
										m_MonoObjects[scriptName.c_str()] = std::make_unique<MonoObjectWithGC>(m_gameObjectGCHandle, m_ptrGameObject);

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
				}
			}
		}
	}
}

void ScriptSystem::Cleanup()
{
	for (auto& monoObj : m_MonoObjects)
	{
		mono_gchandle_free(monoObj.second->m_GCHandle);
	}

	if (m_ptrMonoDomain)
	{
		mono_jit_cleanup(m_ptrMonoDomain);
	}
}

void ScriptSystem::Unload()
{
	// Current domain will be unloaded
	mono_image_close(m_ptrGameAssemblyImage);
	MonoDomain* domainToUnload = mono_domain_get();
	if (domainToUnload && domainToUnload != mono_get_root_domain())
	{
		mono_domain_set(mono_get_root_domain(), false);
		mono_domain_unload(domainToUnload);
	}
	//mono_domain_unload(m_ptrMonoDomain);
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

	//CreateEntityPtr CEPtr = &ScriptSystem::CreateEntity;
	//mono_add_internal_call("Scripting.ECS::CreateEntity", &CEPtr);
	mono_add_internal_call("Scripting.ECS::CreateEntity", &this->CreateEntity);
	mono_add_internal_call("Scripting.ECS::DestroyEntity", &this->DestroyEntity);
	mono_add_internal_call("Scripting.ECS::CreateChild", &this->CreateChild);
	mono_add_internal_call("Scripting.ECS::FindEntityWithTag", &this->FindEntityWithTag);

}

void ScriptSystem::Reload()
{
	std::cout << "Reloading Scripts" << std::endl;
	m_scriptNames.clear();
	Unload();
	LoadMemory();
}