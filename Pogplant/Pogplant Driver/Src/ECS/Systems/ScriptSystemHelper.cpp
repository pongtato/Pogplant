#include "ScriptSystem.h"
#include "ScriptSystemHelper.h"

#include "../Entity.h"
#include <Pogplant.h>

namespace SSH
{
	MonoMethod* FindMethod(MonoClass* klass, const std::string& methodName, int params = -1)
	{
		MonoMethod* method = mono_class_get_method_from_name(klass, methodName.c_str(), params);
		if (!method)
		{
			return nullptr;
		}
		return method;
	}

	std::uint32_t CreateEntity(MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale, MonoString* tag)
	{
		std::string _name = mono_string_to_utf8(name);
		std::string _tag = mono_string_to_utf8(tag);

		return static_cast<std::uint32_t>(ScriptSystem::GetECS()->CreateEntity(_name, pos, rot, scale, _tag).GetID());
	}

	void DestroyEntity(std::uint32_t entityID)
	{
		ScriptSystem::GetECS()->DestroyEntity(static_cast<entt::entity>(entityID));
	}

	std::uint32_t CreateChild(std::uint32_t parentID, MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale, MonoString* tag)
	{
		std::string _name = mono_string_to_utf8(name);
		std::string _tag = mono_string_to_utf8(tag);

		return static_cast<std::uint32_t>(ScriptSystem::GetECS()->CreateChild(static_cast<entt::entity>(parentID), _name, pos, rot, scale, _tag));
	}

	std::uint32_t FindEntityWithName(MonoString* name)
	{
		std::string _name = mono_string_to_utf8(name);
		return static_cast<std::uint32_t>(ScriptSystem::GetECS()->FindEntityWithName(_name));
	}

	void AddComponentTransform(unsigned int id, Components::Transform transform)
	{
		ScriptSystem::GetECS()->GetReg().emplace_or_replace<Components::Transform>(static_cast<entt::entity>(id), transform);
	}

	void AddComponentRigidbody(unsigned int id, Components::Rigidbody rigidbody)
	{
		ScriptSystem::GetECS()->GetReg().emplace_or_replace<Components::Rigidbody>(static_cast<entt::entity>(id), rigidbody);
	}

	void AddComponentRenderer(unsigned int id, glm::vec3 colorTint, glm::vec3 emissiveTint, int useLight, bool editorDrawOnly, MonoString* name)
	{
		std::string modelName = mono_string_to_utf8(name);
		Pogplant::Model* model = Pogplant::ModelResource::m_ModelPool[modelName];
		Pogplant::Mesh3D* mesh3d = &model->m_Meshes.begin()->second;

		ScriptSystem::GetECS()->GetReg().emplace_or_replace<Components::Renderer>(static_cast<entt::entity>(id), Components::Renderer{ colorTint, emissiveTint, model, mesh3d, useLight, editorDrawOnly });
	}

	MonoArray* GetModelKeysIC()
	{
		MonoArray* data = mono_array_new(mono_domain_get(), mono_get_string_class(), Pogplant::ModelResource::m_ModelPool.size());
		int i = 0;
		for (auto it = Pogplant::ModelResource::m_ModelPool.begin(); it != Pogplant::ModelResource::m_ModelPool.end(); ++it)
		{
			mono_array_set(data, MonoString*, i, mono_string_new(mono_domain_get(), it->first.c_str()));
			++i;
		}
		return data;
	}

	void InvokeFunction(const std::string& scriptName, const std::string& funcName, const entt::entity& entity)
	{
		MonoObject* monoObj = ScriptResource::m_MonoObjects[scriptName]->m_MonoObject;
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
			mono_runtime_invoke(method, monoObj, nullptr, nullptr);
		}
	}

	void OnTriggerEnterEvent(std::shared_ptr<PPE::OnTriggerEnterEvent> onTriggerEnterEvent)
	{
		auto script1 = ScriptSystem::GetECS()->GetReg().try_get<Components::Scriptable>(onTriggerEnterEvent.get()->m_entity1);
		auto script2 = ScriptSystem::GetECS()->GetReg().try_get<Components::Scriptable>(onTriggerEnterEvent.get()->m_entity2);

		if (script1)
		{
			for (auto& scripts : script1->m_ScriptTypes)
			{
				InvokeFunction(scripts.first, "OnTriggerEnter", onTriggerEnterEvent.get()->m_entity1);
				//MonoObject* monoObj = m_MonoObjects[scripts.first]->m_MonoObject;
				//MonoObject* monoObj = ScriptResource::m_MonoObjects[scripts.first]->m_MonoObject;
				//if (!monoObj)
				//{
				//	// Maybe log something here
				//	std::cout << "Script: " << scripts.first << " not found" << std::endl;
				//	continue;
				//}

				//MonoClass* klass = mono_object_get_class(monoObj);
				//if (!klass)
				//{
				//	// Maybe log something here
				//	std::cout << "MonoClass not found" << std::endl;
				//}

				//MonoMethod* startMethod = FindMethod(klass, "OnTriggerEnter");
				//if (startMethod)
				//{
				//	mono_runtime_invoke(startMethod, monoObj, nullptr, nullptr);
				//}
			}
		}

		if (script2)
		{
			for (auto& scripts : script2->m_ScriptTypes)
			{
				InvokeFunction(scripts.first, "OnTriggerEnter", onTriggerEnterEvent.get()->m_entity2);
				//MonoObject* monoObj = m_MonoObjects[scripts.first]->m_MonoObject;
				//MonoObject* monoObj = ScriptResource::m_MonoObjects[scripts.first]->m_MonoObject;
				//if (!monoObj)
				//{
				//	// Maybe log something here
				//	std::cout << "Script: " << scripts.first << " not found" << std::endl;
				//	continue;
				//}

				//MonoClass* klass = mono_object_get_class(monoObj);
				//if (!klass)
				//{
				//	// Maybe log something here
				//	std::cout << "MonoClass not found" << std::endl;
				//}

				//MonoMethod* startMethod = FindMethod(klass, "OnTriggerEnter");
				//if (startMethod)
				//{
				//	mono_runtime_invoke(startMethod, monoObj, nullptr, nullptr);
				//}
			}
		}
	}

	void OnTriggerExitEvent(std::shared_ptr<PPE::OnTriggerExitEvent> onTriggerExitEvent)
	{
		auto script1 = ScriptSystem::GetECS()->GetReg().try_get<Components::Scriptable>(onTriggerExitEvent.get()->m_entity1);
		auto script2 = ScriptSystem::GetECS()->GetReg().try_get<Components::Scriptable>(onTriggerExitEvent.get()->m_entity2);

		if (script1)
		{
			for (auto& scripts : script1->m_ScriptTypes)
			{
				InvokeFunction(scripts.first, "OnTriggerExit", onTriggerExitEvent.get()->m_entity1);
				//MonoObject* monoObj = m_MonoObjects[scripts.first]->m_MonoObject;
				//MonoObject* monoObj = ScriptResource::m_MonoObjects[scripts.first]->m_MonoObject;
				//if (!monoObj)
				//{
				//	// Maybe log something here
				//	std::cout << "Script: " << scripts.first << " not found" << std::endl;
				//	continue;
				//}

				//MonoClass* klass = mono_object_get_class(monoObj);
				//if (!klass)
				//{
				//	// Maybe log something here
				//	std::cout << "MonoClass not found" << std::endl;
				//}

				//MonoMethod* startMethod = FindMethod(klass, "OnTriggerExit");
				//if (startMethod)
				//{
				//	mono_runtime_invoke(startMethod, monoObj, nullptr, nullptr);
				//}
			}
		}

		if (script2)
		{
			for (auto& scripts : script2->m_ScriptTypes)
			{
				InvokeFunction(scripts.first, "OnTriggerExit", onTriggerExitEvent.get()->m_entity2);
				//MonoObject* monoObj = m_MonoObjects[scripts.first]->m_MonoObject;
				//MonoObject* monoObj = ScriptResource::m_MonoObjects[scripts.first]->m_MonoObject;
				//if (!monoObj)
				//{
				//	// Maybe log something here
				//	std::cout << "Script: " << scripts.first << " not found" << std::endl;
				//	continue;
				//}

				//MonoClass* klass = mono_object_get_class(monoObj);
				//if (!klass)
				//{
				//	// Maybe log something here
				//	std::cout << "MonoClass not found" << std::endl;
				//}

				//MonoMethod* startMethod = FindMethod(klass, "OnTriggerExit");
				//if (startMethod)
				//{
				//	mono_runtime_invoke(startMethod, monoObj, nullptr, nullptr);
				//}
			}
		}
	}
}