#ifndef _SCRIPTSYSTEM_HELPER_
#define _SCRIPTSYSTEM_HELPER_

#include "../Components/Components.h"
#include "../Components/DependantComponents.h"
#include "../../Events/Events.h"

#include "ScriptResource.h"
#include "../../Serialiser/CustomSaver.h"

#include "ScriptSystem.h"

namespace SSH
{
	// ECS for C# side
	std::uint32_t CreateEntity(MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale, MonoString* tag);
	void DestroyEntity(std::uint32_t entityID);
	void UpdateDestroyedEntities();
	std::uint32_t CreateChild(std::uint32_t parentID, MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale, MonoString* tag);
	std::uint32_t FindEntityWithName(MonoString* name);
	std::uint32_t FindChildEntityWithName(std::uint32_t parentID, MonoString* name);
	std::uint32_t GetParent(std::uint32_t m_id);
	std::uint32_t GetNull();
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
	void RigidbodyAddImpulseForce(std::uint32_t entityID, glm::vec3 direction);
	void RigidbodySetGravity(std::uint32_t entityID, bool boolean);
	void GetCamera(std::uint32_t entityID, float& yaw, float& pitch, float& roll);
	void SetCamera(std::uint32_t entityID, float yaw, float pitch, float roll);
	void SetPosition(std::uint32_t entityID, glm::vec3 pos);
	void SetRotation(std::uint32_t entityID, glm::vec3 rot);
	void SetScale(std::uint32_t entityID, glm::vec3 sca);
	void SetFOV(std::uint32_t entityID, float fov);
	float GetFOV(std::uint32_t entityID);
	void SetParticlePause(std::uint32_t entityID, bool isPaused);
	void SetParticlePlay(std::uint32_t entityID, bool setPlay);
	void SetLaserStart(std::uint32_t entityID, bool isActivated);
	bool IsLaserComplete(std::uint32_t entityID);
	
	void ResetLaser(std::uint32_t entityID);
	//Canvas Component
	void SetFrames(std::uint32_t entityID, int frameValue);
	void SetColorTint(std::uint32_t entityID, glm::vec3& color); //Does not get the alpha
	void SetDiffuseTint(std::uint32_t entityID, glm::vec3& color); //Does not get the alpha
	void SetEmissiveTint(std::uint32_t entityID, glm::vec3& color); //Does not get the alpha
	glm::vec3 GetColorTint(std::uint32_t entityID, glm::vec3& color); //Does not get the alpha

	// Text Component
	void SetSubtitles(std::uint32_t entityID, MonoString* levelID, int indexMin, int indexMax);
	void PlaySubtitles(std::uint32_t entityID);
	void PauseSubtitles(std::uint32_t entityID);

	void ChangeSkybox(MonoString* skyboxName);

	// True is enabled, False is disabled
	void SetActive(std::uint32_t entityID, bool isEnabled);
	void ToggleEntity(std::uint32_t entityID);

	std::uint32_t GetTransformParent(std::uint32_t childID);
	void GetBoxColliderECS(std::uint32_t entityID, MonoString* collisionLayer, bool& isTrigger, glm::vec3& centre, glm::vec3& extends);
	MonoString* GetTagECS(std::uint32_t entityID);
	void LookAt(std::uint32_t self_entityID, glm::vec3& target);
	void LookAtClamped(std::uint32_t self_entityID, glm::vec3& target);
	void LookAtDirectionalVector(std::uint32_t self_entityID, glm::vec3& directionalVector);
	glm::vec3 GetUpVector(std::uint32_t self_entityID);
	void PlayAudio(std::uint32_t entity, std::uint32_t index, MonoString* channelGroupName = nullptr);
	void CreateAudioChannelGroup(MonoString* channelGroupName);
	void PauseAudioChannelGroup(MonoString* channelGroupName);
	void ResumeAudioChannelGroup(MonoString* channelGroupName);
	float GetAudioChannelGroupVolume(MonoString* channelGroupName);
	void SetAudioChannelGroupVolume(MonoString* channelGroupName, float volume);
	void SetAudioChannelGroupPitch(MonoString* channelGroupName, float pitch);

	void LogToEditor(MonoString* scriptName, MonoString* debugLog);

	bool CheckValidEntity(std::uint32_t entityID); // returns true if entity is valid

	glm::vec3 GetGlobalPosition(std::uint32_t entityID);
	glm::vec3 GetGlobalRotation(std::uint32_t entityID);
	glm::vec3 GetRotation(std::uint32_t entityID);
	glm::vec3 GetGlobalScale(std::uint32_t entityID);
	glm::vec3 GetScale(std::uint32_t entityID);
	void SetGlobalPosition(std::uint32_t entityID, glm::vec3 pos);
	void SetGlobalRotation(std::uint32_t entityID, glm::vec3 rot);
	void SetGlobalScale(std::uint32_t entityID, glm::vec3 scale);
	glm::vec3 GetForwardVector(std::uint32_t entityID);
	bool RayCastEntity(glm::vec3 rayOrigin, glm::vec3 rayDir, std::uint32_t entityIDToCast);
	bool SphereCastEntity(glm::vec3 rayOrigin, glm::vec3 rayDir, float radius, std::uint32_t entityIDToCast);
	glm::vec4 GetMovementBounds(std::uint32_t entityID);

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

	void PauseScene();
	void ResumeScene();
	MonoString* GetSceneName();

	template <typename T>
	inline static T CustomSaverGetValueMono(MonoString* monoKey, T defaultValue, bool loadFromDocuments)
	{
		const char* key = mono_string_to_utf8(monoKey);
		return PPU::CustomSaver::template GetValue<T>(key, defaultValue, loadFromDocuments);
	}

	inline static MonoString* CustomSaverGetValueMonoString(MonoString* monoKey, MonoString* defaultValue, bool loadFromDocuments)
	{
		const char* key = mono_string_to_utf8(monoKey);
		const char* defaultStrValue = mono_string_to_utf8(defaultValue);

		return mono_string_new(mono_domain_get(), PPU::CustomSaver::GetValue<std::string>(key, defaultStrValue, loadFromDocuments).c_str());
	}

	template <typename T>
	inline static void CustomSaverSetValueMono(MonoString* monoKey, T value, bool saveToDocuments)
	{
		const char* key = mono_string_to_utf8(monoKey);
		PPU::CustomSaver::template Append<T>(key, value, saveToDocuments);
	}

	inline static void CustomSaverSetValueMonoString(MonoString* monoKey, MonoString* value, bool saveToDocuments)
	{
		const char* key = mono_string_to_utf8(monoKey);
		const char* strValue = mono_string_to_utf8(value);
		PPU::CustomSaver::Append<std::string>(key, strValue, saveToDocuments);
	}

	MonoMethod* FindMethod(MonoClass* klass, const std::string& methodName, int params);

	//void SetTransformECS(std::uint32_t entityID, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale);
	template <typename T>
	inline static T ScriptVariableGet(std::uint32_t entityID, T defaultValue, MonoString* monoName)
	{
		auto scriptVarCom = ScriptSystem::GetECS()->GetReg().try_get<Components::ScriptVariables>(static_cast<entt::entity>(entityID));

		const char* key = mono_string_to_utf8(monoName);

		if (scriptVarCom)
			return scriptVarCom->GetValue<T>(key, defaultValue);

		return defaultValue;
	}

	template <typename T>
	inline static void ScriptVariableSet(std::uint32_t entityID, T value, MonoString* monoName)
	{
		auto scriptVarCom = ScriptSystem::GetECS()->GetReg().try_get<Components::ScriptVariables>(static_cast<entt::entity>(entityID));
		const char* key = mono_string_to_utf8(monoName);

		if (scriptVarCom)
		{
			if (scriptVarCom->m_variables.find(key) != scriptVarCom->m_variables.end())
			{
				scriptVarCom->SetValue<T>(key, value);
			}
		}
	}

	float GetGamma();
	void SetGamma(float newGamma);

	MonoString* ScriptVariableGetString(std::uint32_t entityID, MonoString* defaultValue, MonoString* monoName);
}

#include "ScriptSystemHelper.hpp"

#endif // _SCRIPTSYSTEM_HELPER_