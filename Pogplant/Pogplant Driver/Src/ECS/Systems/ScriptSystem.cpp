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

ScriptSystem::ScriptSystem()
{
	mono_set_dirs("..\\Libs\\Mono\\lib", "..\\Libs\\Mono\\etc");

	// Create the mono domain
	m_ptrMonoDomain = mono_jit_init("ScriptSystem");
	if (m_ptrMonoDomain)
	{
		// Load the compiled dll script (c#)
		m_ptrGameAssembly = mono_domain_assembly_open(m_ptrMonoDomain, "..\\Libs\\ScriptLib\\Scripting.dll");

		if (m_ptrGameAssembly)
		{
			m_ptrGameAssemblyImage = mono_assembly_get_image(m_ptrGameAssembly);
			if (m_ptrGameAssemblyImage)
			{
				MonoClass* m_MonoClass = mono_class_from_name(m_ptrGameAssemblyImage, "Scripting", "Scripting");
				MonoClass* m_MonoPlayerClass = mono_class_from_name(m_ptrGameAssemblyImage, "Scripting", "PlayerScript");
				MonoClass* m_MonoEnemyClass = mono_class_from_name(m_ptrGameAssemblyImage, "Scripting", "EnemyScript");

				if (m_MonoPlayerClass && m_MonoClass)
				{
					// Main method describe
					MonoMethodDesc* ptrMainMethodDesc = mono_method_desc_new(".Scripting:Player()", false);

					if (ptrMainMethodDesc)
					{
						// Find the main in mainclass
						MonoMethod* ptrMainMethod = mono_method_desc_search_in_class(ptrMainMethodDesc, m_MonoClass);
						if (ptrMainMethod)
						{
							MonoObject* ptrExObject = nullptr;
							MonoObject* m_ptrGameObject = mono_runtime_invoke(ptrMainMethod, nullptr, nullptr, &ptrExObject);

							if (m_ptrGameObject)
							{
								// Garbage Collection Handle for the game object
								uint32_t m_gameObjectGCHandle = mono_gchandle_new(m_ptrGameObject, false);
								// Add to the map
								m_MonoObjects["Player"] = new MonoObjectWithGC{ m_gameObjectGCHandle, m_ptrGameObject };

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

				if (m_MonoEnemyClass && m_MonoClass)
				{
					// Main method describe
					MonoMethodDesc* ptrMainMethodDesc = mono_method_desc_new(".Scripting:Enemy()", false);

					if (ptrMainMethodDesc)
					{
						// Find the main in mainclass
						MonoMethod* ptrMainMethod = mono_method_desc_search_in_class(ptrMainMethodDesc, m_MonoClass);
						if (ptrMainMethod)
						{
							MonoObject* ptrExObject = nullptr;
							MonoObject* m_ptrGameObject = mono_runtime_invoke(ptrMainMethod, nullptr, nullptr, &ptrExObject);

							if (m_ptrGameObject)
							{
								// Garbage Collection Handle for the game object
								uint32_t m_gameObjectGCHandle = mono_gchandle_new(m_ptrGameObject, false);
								// Add to the map
								m_MonoObjects["Enemy"] = new MonoObjectWithGC{ m_gameObjectGCHandle, m_ptrGameObject };

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

ScriptSystem::~ScriptSystem()
{
	for (auto& monoObj : m_MonoObjects)
	{
		mono_gchandle_free(monoObj.second->m_GCHandle);
		free(monoObj.second);
	}

	if (m_ptrMonoDomain)
	{
		mono_jit_cleanup(m_ptrMonoDomain);
	}
}

void ScriptSystem::Init(ECS* ecs)
{
	m_registry = ecs;
}

void ScriptSystem::Update()
{
	auto entities = m_registry->GetReg().view<Components::Scriptable, Components::Rigidbody, Components::Transform, Components::Name>();

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
			void* args[] = {&transform, &rigidbody };
			mono_runtime_invoke(updateMethod, monoObj, args, nullptr);
		}
	}
}

MonoMethod* ScriptSystem::FindMethod(MonoClass* klass, std::string methodName, int params)
{
	MonoMethod* method = mono_class_get_method_from_name(klass, methodName.c_str(), params);
	if (!method)
	{
		return NULL;
	}
	return method;
}