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

bool ScriptSystem::isReload = false;

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
			void* args[] = { &transform, &rigidbody };
			mono_runtime_invoke(updateMethod, monoObj, args, nullptr);
		}
	}

	// Ghetto until I figure out CBs
	if (isReload == true)
	{
		Reload();
		isReload = false;
	}

}

void ScriptSystem::SetReload(bool _isReload)
{
	isReload = _isReload;
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

// Not used atm, probably 4ever.
void ScriptSystem::Load()
{
	// My saved domain will be replaced by new domain
	MonoDomain* newDomain = mono_domain_create_appdomain(const_cast<char*>("Scripting"), NULL);
	mono_domain_set(newDomain, false);

	// Create the mono domain
	if (newDomain)
	{
		// Load the compiled dll script (c#)
		m_ptrGameAssembly = mono_domain_assembly_open(newDomain, "Resources\\DLL\\Scripting.dll");

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
								m_MonoObjects["Player"] = std::make_unique<MonoObjectWithGC>(m_gameObjectGCHandle, m_ptrGameObject);

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
								m_MonoObjects["Enemy"] = std::make_unique<MonoObjectWithGC>(m_gameObjectGCHandle, m_ptrGameObject);

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

		//size_t len = 0;
		//char* myData = Read(dllPath, &len);
		//m_ptrGameAssemblyImage = mono_image_open_from_data_with_name(myData, len, true, &status, false, dllPath.c_str());
		//delete myData;

		if (m_ptrGameAssemblyImage && status == MONO_IMAGE_OK)
		{
			//MonoImage* m_ptrGameAssemblyImage = mono_assembly_get_image(m_ptrGameAssembly);
			//m_ptrGameAssembly = mono_assembly_load_from(m_ptrGameAssemblyImage, "", &status);
			m_ptrGameAssembly = mono_assembly_load_from_full(m_ptrGameAssemblyImage, dllPath.c_str(), &status, false);
			if (m_ptrGameAssembly && status == MONO_IMAGE_OK)
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
								m_MonoObjects["Player"] = std::make_unique<MonoObjectWithGC>(m_gameObjectGCHandle, m_ptrGameObject);

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
								m_MonoObjects["Enemy"] = std::make_unique<MonoObjectWithGC>(m_gameObjectGCHandle, m_ptrGameObject);

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

void ScriptSystem::Reload()
{
	std::cout << "Reloading Scripts" << std::endl;
	Unload();
	LoadMemory();
}