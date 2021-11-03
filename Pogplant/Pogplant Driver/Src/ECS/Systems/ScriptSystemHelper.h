#ifndef _SCRIPTSYSTEM_HELPER_
#define _SCRIPTSYSTEM_HELPER_

#include "../Components/Components.h"
#include "../Components/DependantComponents.h"
#include "../../Events/Events.h"

#include "ScriptResource.h"

namespace SSH
{
	std::uint32_t CreateEntity(MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale, MonoString* tag);
	void DestroyEntity(std::uint32_t entityID);
	std::uint32_t CreateChild(std::uint32_t parentID, MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale, MonoString* tag);
	std::uint32_t FindEntityWithName(MonoString* name);
	void AddComponentTransform(unsigned int id, Components::Transform transform);
	void AddComponentRigidbody(unsigned int id, Components::Rigidbody rigidbody);
	void AddComponentRenderer(unsigned int id, glm::vec3 colorTint, glm::vec3 emissiveTint, int useLight, bool editorDrawOnly, MonoString* name);
	MonoArray* GetModelKeysIC();

	void InvokeFunction(const std::string& scriptName, const std::string& funcName, const entt::entity& entity);

	// Please check if the entity exists with scriptable component before calling this function
	template<typename ...Args>
	inline void InvokeFunction(const std::string& scriptName, const std::string& funcName, const entt::entity& entity, Args&&... args)
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
			void* argss[] = { &args... };
			mono_runtime_invoke(method, monoObj, argss, nullptr);
		}
	}

	void OnTriggerEnterEvent(std::shared_ptr<PPE::OnTriggerEnterEvent> onTriggerEnterEvent);
	void OnTriggerExitEvent(std::shared_ptr<PPE::OnTriggerExitEvent> onTriggerExitEvent);
}

#endif // _SCRIPTSYSTEM_HELPER_