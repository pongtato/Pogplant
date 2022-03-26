#include "ScriptSystemHelper.h"
namespace SSH
{
#pragma optimize( "", off )
	template<typename T>
	T InvokeFunctionWithReturn(const std::string& scriptName, const std::string& funcName, const entt::entity& entity)
	{
		T result = NULL;
		MonoObject* monoObj = ScriptResource::m_MonoObjects[entity][scriptName]->m_MonoObject;
		if (!monoObj)
		{
			// Maybe log something here
			std::cout << "Script: " << scriptName << " not found" << std::endl;
		}

		MonoClass* klass = mono_object_get_class(monoObj);
		if (!klass)
		{
			// Maybe log something here
			std::cout << "MonoClass not found" << std::endl;
		}

		MonoMethod* method = mono_class_get_method_from_name(klass, funcName.c_str(), -1);
		MonoObject* ptrReturnObj = nullptr;

		if (method)
		{
			ptrReturnObj = mono_runtime_invoke(method, monoObj, nullptr, nullptr);
		}

		if (ptrReturnObj)
		{
			result = *(T*)mono_object_unbox(ptrReturnObj);
		}

		return result;
	}

	template<typename ...Args>
	inline void InvokeFunction(const std::string& scriptName, const std::string& funcName, const entt::entity& entity, Args&&... args)
	{
		MonoObject* monoObj = ScriptResource::m_MonoObjects[entity][scriptName]->m_MonoObject;
		if (!monoObj)
		{
			// Maybe log something here
			std::cout << "Script: " << scriptName << " not found" << std::endl;
		}

		MonoClass* klass = mono_object_get_class(monoObj);
		if (!klass)
		{
			// Maybe log something here
			std::cout << "MonoClass not found" << std::endl;
		}

		volatile std::string test = scriptName;
		test;

		volatile std::string test2 = funcName;
		test2;

		try
		{
			MonoMethod* method = mono_class_get_method_from_name(klass, funcName.c_str(), (int)sizeof...(args));
			//MonoMethod* method = FindMethod(klass, funcName, -1);
			if (method)
			{
				void* argss[] = { &args... };
				mono_runtime_invoke(method, monoObj, argss, nullptr);
			}
		}
		catch (...)
		{
			Pogplant::Logger::Log(
				Pogplant::LogEntry{ "ScriptSystemHelper::InvokeFunction", Pogplant::LogEntry::LOGTYPE::ERROR, "Unknown error occured in invoke function" }, true);
		}

	}

	template<typename T, typename ...Args>
	inline T InvokeFunctionWithReturn(const std::string& scriptName, const std::string& funcName, const entt::entity& entity, Args&&... args)
	{
		T result = NULL;
		MonoObject* monoObj = ScriptResource::m_MonoObjects[entity][scriptName]->m_MonoObject;
		if (!monoObj)
		{
			// Maybe log something here
			std::cout << "Script: " << scriptName << " not found" << std::endl;
		}

		MonoClass* klass = mono_object_get_class(monoObj);
		if (!klass)
		{
			// Maybe log something here
			std::cout << "MonoClass not found" << std::endl;
		}

		MonoMethod* method = mono_class_get_method_from_name(klass, funcName.c_str(), (int)sizeof...(args));
		MonoObject* ptrReturnObj = nullptr;

		if (method)
		{
			void* argss[] = { &args... };
			ptrReturnObj = mono_runtime_invoke(method, monoObj, argss, nullptr);
		}

		if (ptrReturnObj)
		{
			result = *(T*)mono_object_unbox(ptrReturnObj);
		}

		return result;
	}
#pragma optimize( "", on )
}
