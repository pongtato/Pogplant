#include "ScriptSystemHelper.h"
namespace SSH
{
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

		MonoMethod* method = mono_class_get_method_from_name(klass, funcName.c_str(), -1);
		//MonoMethod* method = FindMethod(klass, funcName, -1);
		if (method)
		{
			void* argss[] = { &args... };
			mono_runtime_invoke(method, monoObj, argss, nullptr);
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

		MonoMethod* method = mono_class_get_method_from_name(klass, funcName.c_str(), -1);
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
}
