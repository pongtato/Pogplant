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
		ScriptResource::m_EntitiesToDelete.push_back(static_cast<entt::entity>(entityID));
		//ScriptSystem::GetECS()->DestroyEntity(static_cast<entt::entity>(entityID));
	}

	void UpdateDestroyedEntities()
	{
		for (auto entityID : ScriptResource::m_EntitiesToDelete)
		{
			ScriptSystem::GetECS()->DestroyEntity(static_cast<entt::entity>(entityID));
		}

		ScriptResource::m_EntitiesToDelete.clear();
	}

	std::uint32_t CreateChild(std::uint32_t parentID, MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale, MonoString* tag)
	{
		std::string _name = mono_string_to_utf8(name);
		std::string _tag = mono_string_to_utf8(tag);

		return static_cast<std::uint32_t>(ScriptSystem::GetECS()->CreateChild(static_cast<entt::entity>(parentID), _name, pos, rot, scale, _tag).GetID());
	}

	std::uint32_t FindEntityWithName(MonoString* name)
	{
		std::string _name = mono_string_to_utf8(name);
		return static_cast<std::uint32_t>(ScriptSystem::GetECS()->FindEntityWithName(_name));
	}

	std::uint32_t FindChildEntityWithName(std::uint32_t parentID, MonoString* name)
	{
		std::string _name = mono_string_to_utf8(name);
		return static_cast<std::uint32_t>(ScriptSystem::GetECS()->FindChildEntityWithName(static_cast<entt::entity>(parentID), _name));
	}

	void LookAt(std::uint32_t self_entityID, std::uint32_t other_entityID)
	{
		const auto& self_trans = ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(self_entityID));
		const auto& other_trans = ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(other_entityID));
		self_trans->LookAt(other_trans->GetGlobalPosition());
	}

	void GetTransformECS(std::uint32_t entityID, glm::vec3& pos, glm::vec3& rot, glm::vec3& scale)
	{
		const auto& trans = ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(entityID));
		pos = trans->m_position;
		rot = trans->m_rotation;
		scale = trans->m_scale;
	}

	void SetTransformECS(std::uint32_t entityID, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale)
	{
		auto trans = ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(entityID));
		trans->m_position = pos;
		trans->m_rotation = rot;
		trans->m_scale = scale;
	}

	void SetTransformParent(std::uint32_t childID, std::uint32_t parentID)
	{
		ScriptSystem::GetECS()->SetParent(static_cast<entt::entity>(parentID), static_cast<entt::entity>(childID));
	}

	int GetChildCount(std::uint32_t parentID)
	{
		auto trans = ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(parentID));
		int count = 0;
		if (trans)
			count =  static_cast<int>(trans->m_children.size());
		return count;
	}

	void SetColliderBox(std::uint32_t entityID, bool& isTrigger, glm::vec3& centre, glm::vec3& extends)
	{
		auto boxCollider = ScriptSystem::GetECS()->GetReg().try_get<Components::BoxCollider>(static_cast<entt::entity>(entityID));
		if (boxCollider)
		{
			boxCollider->centre = centre;
			boxCollider->extends = extends;
			boxCollider->isTrigger = isTrigger;
		}
	}

	void RemoveParentFrom(std::uint32_t childID)
	{
		ScriptSystem::GetECS()->RemoveParentFrom(static_cast<entt::entity>(childID));
	}

	std::uint32_t GetTransformParent(std::uint32_t childID)
	{
		return static_cast<std::uint32_t>(ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(childID))->m_parent);
	}

	void GetBoxColliderECS(std::uint32_t entityID, MonoString* collisionLayer, bool& isTrigger, glm::vec3& centre, glm::vec3& extends)
	{
		const auto& boxCollider = ScriptSystem::GetECS()->GetReg().try_get<Components::BoxCollider>(static_cast<entt::entity>(entityID));

		if (boxCollider)
		{
			collisionLayer = mono_string_new(mono_domain_get(), boxCollider->collisionLayer.c_str());
			isTrigger = boxCollider->isTrigger;
			centre = boxCollider->centre;
			extends = boxCollider->extends;
		}
	}

	MonoString* GetTagECS(std::uint32_t entityID)
	{
		const auto& tag = ScriptSystem::GetECS()->GetReg().try_get<Components::Tag>(static_cast<entt::entity>(entityID));
		MonoString* ptr = nullptr;
		if (tag)
		{
			ptr = mono_string_new(mono_domain_get(), tag->m_tag.c_str());
		}
		return ptr;
	}

	glm::vec3 GetGlobalPosition(std::uint32_t entityID)
	{
		return ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(entityID))->GetGlobalPosition();
	}

	glm::vec3 GetGlobalRotation(std::uint32_t entityID)
	{
		return ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(entityID))->GetGlobalRotation();
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

	void AddScript(unsigned int id, MonoString* _scriptName)
	{
		std::string scriptName = mono_string_to_utf8(_scriptName);
		const auto& scriptComp = ScriptSystem::GetECS()->GetReg().try_get<Components::Scriptable>(static_cast<entt::entity>(id));
		if (scriptComp)
		{
			scriptComp->m_ScriptTypes[scriptName] = false;
		}
		else
		{
			std::unordered_map <std::string, bool> scripts;
			scripts[scriptName] = false;
			ScriptSystem::GetECS()->GetReg().emplace_or_replace<Components::Scriptable>(static_cast<entt::entity>(id), Components::Scriptable{ scripts });
		}
	}

	void PlayAudio(std::uint32_t entity, std::uint32_t index)
	{
		const auto& audio_comp = ScriptSystem::GetECS()->GetReg().try_get<Components::AudioSource>(static_cast<entt::entity>(entity));
		audio_comp->PlayAudio(index);
	}

	void AddComponentBoxCollider(unsigned int id, bool isTrigger, MonoString* collisionLayer, glm::vec3 extends, glm::vec3 centre)
	{
		(void)isTrigger;
		(void)collisionLayer;
		ScriptSystem::GetECS()->GetReg().emplace_or_replace<Components::BoxCollider>(static_cast<entt::entity>(id), Components::BoxCollider{ extends, centre });
	}

	void AddComponentSphereCollider(unsigned int id, bool isTrigger, MonoString* collisionLayer, float radius, glm::vec3 centre)
	{
		(void)isTrigger;
		(void)collisionLayer;
		ScriptSystem::GetECS()->GetReg().emplace_or_replace<Components::SphereCollider>(static_cast<entt::entity>(id), Components::SphereCollider{ centre, radius });
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
				InvokeFunction(scripts.first, "OnTriggerEnter", onTriggerEnterEvent.get()->m_entity1, onTriggerEnterEvent.get()->m_entity2);
			}
		}

		if (script2)
		{
			for (auto& scripts : script2->m_ScriptTypes)
			{
				InvokeFunction(scripts.first, "OnTriggerEnter", onTriggerEnterEvent.get()->m_entity2, onTriggerEnterEvent.get()->m_entity1);
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
				InvokeFunction(scripts.first, "OnTriggerExit", onTriggerExitEvent.get()->m_entity1, onTriggerExitEvent.get()->m_entity2);
			}
		}

		if (script2)
		{
			for (auto& scripts : script2->m_ScriptTypes)
			{
				InvokeFunction(scripts.first, "OnTriggerExit", onTriggerExitEvent.get()->m_entity2, onTriggerExitEvent.get()->m_entity1);
			}
		}
	}
}
