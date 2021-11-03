namespace SSH
{
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
		if (method)
		{
			void* argss[] = { &args... };
			mono_runtime_invoke(method, monoObj, argss, nullptr);
		}
	}
}
