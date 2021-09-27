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
				// Add the internal calls
				mono_add_internal_call("Scripting.ObjectScript::getRandomWord()", &ScriptBinder::CS_getRandomWord);
				// The IScript Class
				MonoClass* ptrIScriptClass = mono_class_from_name(m_ptrGameAssemblyImage, "Scripting", "IScripts");
				// The Main Class
				MonoClass* ptrMainClass = mono_class_from_name(m_ptrGameAssemblyImage, "Scripting", "Scripting");

				// If both classes exist
				if (ptrIScriptClass && ptrMainClass)
				{
					// Main method describe
					MonoMethodDesc* ptrMainMethodDesc = mono_method_desc_new(".Scripting:main()", false);

					if (ptrMainMethodDesc)
					{
						// Find the main in mainclass
						MonoMethod* ptrMainMethod = mono_method_desc_search_in_class(ptrMainMethodDesc, ptrMainClass);
						if (ptrMainMethod)
						{
							MonoObject* ptrExObject = nullptr;
							m_ptrGameObject = mono_runtime_invoke(ptrMainMethod, nullptr, nullptr, &ptrExObject);

							if (m_ptrGameObject)
							{
								// Garbage Collection Handle for the game object
								m_gameObjectGCHandle = mono_gchandle_new(m_ptrGameObject, false);

								// Get example function with return (Function #1)
								MonoMethodDesc* ptrTickMethodDesc = mono_method_desc_new(".IScript:testfuncwithreturn()", false);
								if (ptrTickMethodDesc)
								{
									// Get the real function
									MonoMethod* virtualMethod = mono_method_desc_search_in_class(ptrTickMethodDesc, ptrIScriptClass);
									if (virtualMethod)
									{
										m_ptrTickMethod = mono_object_get_virtual_method(m_ptrGameObject, virtualMethod);
									}

									// Free desc
									mono_method_desc_free(ptrTickMethodDesc);
								}

								// Get example function with param (Function #2)
								MonoMethodDesc* ptrKeyMethodDesc = mono_method_desc_new(".IScript:testfuncwithparam(char)", false);
								if (ptrKeyMethodDesc)
								{
									// Get the real function
									MonoMethod* virtualMethod = mono_method_desc_search_in_class(ptrKeyMethodDesc, ptrIScriptClass);
									if (virtualMethod)
									{
										m_ptrKeyEventMethod = mono_object_get_virtual_method(m_ptrGameObject, virtualMethod);
									}

									// Free desc
									mono_method_desc_free(ptrKeyMethodDesc);
								}
							}

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

ScriptSystem::~ScriptSystem()
{
	// Cleanup
	if (m_gameObjectGCHandle)
	{
		mono_gchandle_free(m_gameObjectGCHandle);
	}

	if (m_ptrMonoDomain)
	{
		mono_jit_cleanup(m_ptrMonoDomain);
	}
}

bool ScriptSystem::testfuncwithreturn()
{
	bool result = false;

	if (m_ptrTickMethod)
	{
		// Invoke
		MonoObject* ptrExObject = nullptr;
		MonoObject* ptrReturnObject = mono_runtime_invoke(m_ptrTickMethod, m_ptrGameObject, nullptr, &ptrExObject);

		// Check for exceptions
		if (ptrExObject)
		{
			MonoString* exString = mono_object_to_string(ptrExObject, nullptr);
			const char* exCString = mono_string_to_utf8(exString);
			std::cout << exCString << std::endl;
		}

		// Extract the result from the object 
		if (ptrReturnObject)
		{
			result = *(bool*)mono_object_unbox(ptrReturnObject);
		}
	}

	return result;
}

void ScriptSystem::testfuncwithparam(char key)
{
	if (m_ptrKeyEventMethod)
	{
		// Invoke
		MonoObject* ptrExObject = nullptr;

		void* args[] = { &key };
		mono_runtime_invoke(m_ptrKeyEventMethod, m_ptrGameObject, args, &ptrExObject);

		// Check for exceptions
		if (ptrExObject)
		{
			MonoString* exString = mono_object_to_string(ptrExObject, nullptr);
			const char* exCString = mono_string_to_utf8(exString);
			std::cout << exCString << std::endl;
		}
	}
}
