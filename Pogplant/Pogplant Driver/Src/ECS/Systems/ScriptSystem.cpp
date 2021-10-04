#include "../Components/Components.h"
#include "ScriptSystem.h"

ScriptSystem::ScriptSystem()
{
}

ScriptSystem::~ScriptSystem()
{
	for (auto& monoObj : m_MonoObjects)
	{
		mono_gchandle_free(monoObj.first);
	}

	if (m_ptrMonoDomain)
	{
		mono_jit_cleanup(m_ptrMonoDomain);
	}
}

void ScriptSystem::Init(ECS* ecs)
{
	m_registry = ecs;

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
				// Add the internal calls
				mono_add_internal_call("Scripting.ObjectScript::getRandomWord()", &ScriptBinder::CS_getRandomWord);
				// The IScript Class
				//MonoClass* ptrIScriptClass = mono_class_from_name(m_ptrGameAssemblyImage, "Scripting", "IScripts");
				m_VirtualMonoClass = mono_class_from_name(m_ptrGameAssemblyImage, "Scripting", "IScripts");
				// The Main Class
				//MonoClass* ptrMainClass = mono_class_from_name(m_ptrGameAssemblyImage, "Scripting", "Scripting");
				m_MonoClass = mono_class_from_name(m_ptrGameAssemblyImage, "Scripting", "Scripting");

				// If both classes exist
				if (m_VirtualMonoClass && m_MonoClass)
				{
					// Main method describe
					MonoMethodDesc* ptrMainMethodDesc = mono_method_desc_new(".Scripting:main()", false);

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
								// Add to the pool
								m_MonoObjects[m_gameObjectGCHandle] = m_ptrGameObject;

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

	Start();
}

void ScriptSystem::Start()
{
	auto entities = m_registry->GetReg().view<Components::Scriptable>();

	for (auto& obj : m_MonoObjects)
	{
		auto klass = mono_object_get_class(obj.second);
		if (klass)
		{
			for (auto& entity : entities)
			{
				auto& scriptable = entities.get<Components::Scriptable>(entity);
				MonoMethod* method = FindMethod(klass, "Start");
				mono_runtime_invoke(method, obj.second, nullptr, nullptr);
				//for (auto& methodName : scriptable.m_Functions)
				//{
				//	MonoMethod* method = FindMethod(klass, methodName);
				//	if (method)
				//	{
				//		mono_runtime_invoke(method, obj.second, nullptr, nullptr);
				//	}
				//}
			}
		}
	}
}

void ScriptSystem::Update()
{
	auto entities = m_registry->GetReg().view<Components::Scriptable>();

	for (auto& obj : m_MonoObjects)
	{
		auto klass = mono_object_get_class(obj.second);
		if (klass)
		{
			for (auto& entity : entities)
			{
				auto& scriptable = entities.get<Components::Scriptable>(entity);
				MonoMethod* method = FindMethod(klass, "Update");
				mono_runtime_invoke(method, obj.second, nullptr, nullptr);
				//for (auto& methodName : scriptable.m_Functions)
				//{
				//	MonoMethod* method = FindMethod(klass, methodName);
				//	if (method)
				//	{
				//		mono_runtime_invoke(method, obj.second, nullptr, nullptr);
				//	}
				//}
			}
		}
	}
}

MonoMethod* ScriptSystem::FindMethod(MonoClass* klass, std::string methodName, int params)
{
	// I think that takes searches all methods with any number of params.
	MonoMethod* method = mono_class_get_method_from_name(klass, methodName.c_str(), params);
	if (!method)
	{
		return NULL;
	}
	return method;
}