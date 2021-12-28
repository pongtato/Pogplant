#ifndef _SCRIPTSYSTEM_HELPER_
#define _SCRIPTSYSTEM_HELPER_

#include "../Components/Components.h"
#include "../Components/DependantComponents.h"
#include "../../Events/Events.h"

#include "ScriptResource.h"


namespace SSH
{
	// ECS for C# side
	std::uint32_t CreateEntity(MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale, MonoString* tag);
	void DestroyEntity(std::uint32_t entityID);
	void UpdateDestroyedEntities();
	std::uint32_t CreateChild(std::uint32_t parentID, MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale, MonoString* tag);
	std::uint32_t FindEntityWithName(MonoString* name);
	std::uint32_t FindChildEntityWithName(std::uint32_t parentID, MonoString* name);
	void GetTransformECS(std::uint32_t entityID, glm::vec3& pos, glm::vec3& rot, glm::vec3& scale);
	void SetTransformECS(std::uint32_t entityID, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale);
	void SetTransformParent(std::uint32_t childID, std::uint32_t parentID);
	int GetChildCount(std::uint32_t parentID);
	void SetColliderBox(std::uint32_t entityID, bool& isTrigger, glm::vec3& centre, glm::vec3& extends);
	void RemoveParentFrom(std::uint32_t _id);
	MonoArray* GetChildren(std::uint32_t entityID);

	glm::vec3 GetVelocity(std::uint32_t entityID);
	void SetVelocity(std::uint32_t entityID, glm::vec3 velocity);
	void RigidbodyAddForce(std::uint32_t entityID, glm::vec3 direction);
	void GetCamera(std::uint32_t entityID, float& yaw, float& pitch, float& roll);
	void SetCamera(std::uint32_t entityID, float yaw, float pitch, float roll);
	void SetPosition(std::uint32_t entityID, glm::vec3 pos);
	void SetRotation(std::uint32_t entityID, glm::vec3 rot);
	void SetScale(std::uint32_t entityID, glm::vec3 sca);

	// True is enabled, False is disabled
	void SetActive(std::uint32_t entityID, bool isEnabled);

	std::uint32_t GetTransformParent(std::uint32_t childID);
	void GetBoxColliderECS(std::uint32_t entityID, MonoString* collisionLayer, bool& isTrigger, glm::vec3& centre, glm::vec3& extends);
	MonoString* GetTagECS(std::uint32_t entityID);
	void LookAt(std::uint32_t self_entityID, glm::vec3& target);
	void LookAtClamped(std::uint32_t self_entityID, glm::vec3& target);
	glm::vec3 GetUpVector(std::uint32_t self_entityID);
	void PlayAudio(std::uint32_t entity, std::uint32_t index);
	void LogToEditor(MonoString* scriptName, MonoString* debugLog);

	bool CheckValidEntity(std::uint32_t entityID); // returns true if entity is valid

	glm::vec3 GetGlobalPosition(std::uint32_t entityID);
	glm::vec3 GetGlobalRotation(std::uint32_t entityID);
	void SetGlobalPosition(std::uint32_t entityID, glm::vec3 pos);
	void SetGlobalRotation(std::uint32_t entityID, glm::vec3 rot);
  glm::vec3 GetForwardVector(std::uint32_t entityID);


	// Components for GambObject
	void AddComponentTransform(unsigned int id, Components::Transform transform);
	void AddComponentRigidbody(unsigned int id, Components::Rigidbody rigidbody);
	void AddComponentRenderer(unsigned int id, glm::vec3 colorTint, glm::vec3 emissiveTint, int useLight, bool editorDrawOnly, MonoString* name);
	void AddScript(unsigned int id, MonoString* _scriptName);
	void AddComponentBoxCollider(unsigned int id, bool isTrigger, MonoString* collisionLayer, glm::vec3 extends, glm::vec3 centre);
	//void AddComponentBoxCollider(unsigned int id, Components::BoxCollider);
	void AddComponentSphereCollider(unsigned int id, bool isTrigger, MonoString* collisionLayer, float radius, glm::vec3 centre);
	//void AddComponentSphereCollider(unsigned int id, Components::SphereCollider);
	MonoArray* GetModelKeysIC();

	void InvokeFunction(const std::string& scriptName, const std::string& funcName, const entt::entity& entity);

	template <typename T>
	T InvokeFunctionWithReturn(const std::string& scriptName, const std::string& funcName, const entt::entity& entity);

	// Please check if the entity exists with scriptable component before calling this function
	template<typename ...Args>
	inline void InvokeFunction(const std::string& scriptName, const std::string& funcName, const entt::entity& entity, Args&&... args);

	template<typename T, typename ...Args>
	inline T InvokeFunctionWithReturn(const std::string& scriptName, const std::string& funcName, const entt::entity& entity, Args&&... args);

	void OnTriggerEnterEvent(std::shared_ptr<PPE::OnTriggerEnterEvent> onTriggerEnterEvent);
	void OnTriggerExitEvent(std::shared_ptr<PPE::OnTriggerExitEvent> onTriggerExitEvent);

	// For scene loading through scripts
	void LoadScene(MonoString* sceneFileName);
	void ExitScene();

	MonoMethod* FindMethod(MonoClass* klass, const std::string& methodName, int params);
}

#include "ScriptSystemHelper.hpp"

#endif // _SCRIPTSYSTEM_HELPER_