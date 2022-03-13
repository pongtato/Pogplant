#include "../Application.h"
#include "ECS.h"
#include "Entity.h"
#include "Systems/ScriptResource.h"
#include "../ECS/Components/DependantComponents.h"
#include <rpc.h>
#include <rpcdce.h>

using namespace Components;

ECS::ECS()
{
}

ECS::~ECS()
{
}

Entity ECS::CreateEntity(std::string _name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale, std::string _tag)
{
	Entity entity = { m_registry.create(), this };

	entity.AddComponent<Name>(_name);
	entity.AddComponent<Tag>(_tag);
	entity.AddComponent<Transform>(pos, rot, scale);
	return entity;
}

Entity ECS::CreateChild(entt::entity parent,
						std::string child_name,
						glm::vec3 pos,
						glm::vec3 rot,
						glm::vec3 scale,
						std::string child_tag)
{
	//auto _p_r = m_registry.try_get<Relationship>(parent);
	auto& transform = m_registry.get<Transform>(parent);


	//create child
	auto _child = CreateEntity(child_name, pos, rot, scale, child_tag);

	//child relationship component
	//auto& _c_r = m_registry.emplace<Relationship>(_child.GetID());
	//_c_r.m_parent = parent;
	auto& childTransform = m_registry.get<Transform>(_child.GetID());
	childTransform.m_parent = parent;

	transform.m_children.insert(_child.GetID());

	return _child;
}

entt::registry& ECS::GetReg()
{
	return m_registry;
}

void ECS::DestroyEntity(entt::entity entity)
{
	m_EntitiesToDelete.insert(entity);
}

void ECS::TrulyDestroyEntity(entt::entity entity)
{
	//auto _r = m_registry.try_get<Relationship>(entity);
	if (!m_registry.valid(entity))
		return;

	auto& _transform = m_registry.get<Transform>(entity);

	//if it's a prefab, remove from map
	//const auto& _prefab = m_registry.try_get<Prefab>(entity);
	//if (_prefab && m_prefab_map.contains(_prefab->file_path))
	//	m_prefab_map.extract(_prefab->file_path);

	//Destroy all children
	auto copy_set = _transform.m_children;
	for (auto child : copy_set)
	{
		TrulyDestroyEntity(child);
	}

	//remove parent connection
	if (_transform.m_parent != entt::null)
	{
		auto& _parentTrans = m_registry.get<Transform>(_transform.m_parent);
		
		if (_parentTrans.m_children.erase(entity) == 0)
		{
			//If this is hit means that parent linking is not synced properly somewhere
			assert(false);
		}
	}

	auto _s = m_registry.try_get<Scriptable>(entity);

	// Clear from the Script resource pool
	if (_s)
	{
		ScriptResource::RemoveEntity(entity);
	}
	
	PogplantDriver::Application::GetInstance().m_sPhysicsSystem.RemoveEntityFromTree(entity);

	//destroy itself
	//m_registry.destroy(entity, 0);
	m_registry.destroy(entity);
}

//returns the first entity with the name
entt::entity ECS::FindEntityWithName(std::string _name)
{
	auto view = m_registry.view<Name>(entt::exclude_t<Components::Prefab>());

	for (auto& entity : view)
	{
		auto& name = view.get<Name>(entity);
		if (name.m_name == _name)
			return entity;
	}

	return entt::null;
}

//returns the first entity with the name
entt::entity ECS::FindChildEntityWithName(entt::entity parentID, const std::string& _name)
{
	auto& transform = m_registry.get<Transform>(parentID);
	for (auto& entity : transform.m_children)
	{
		auto& name = m_registry.get<Name>(entity);
		if (name.m_name == _name)
			return entity;
		else
		{
			auto child = FindChildEntityWithName(entity, _name);
			if (child != entt::null)
				return child;
		}
	}

	return entt::null;
}

//returns the immediate parent of entity_id
entt::entity ECS::GetParent(entt::entity entity_id)
{
	return m_registry.get<Transform>(entity_id).m_parent;
}

entt::entity ECS::GetNull()
{
	return entt::null;
}

//returns the first entity with the tag
entt::entity ECS::FindEntityWithTag(std::string _tag)
{
	auto view = m_registry.view<Tag>(entt::exclude_t<Components::Prefab>());

	for (auto& entity : view)
	{
		auto tag = view.get<Tag>(entity);
		if (tag.m_tag == _tag)
			return entity;
	}
	return entt::null;
}

entt::entity ECS::CopyEntity(entt::entity _target, entt::entity _override)
{
	//override is used to overwrite the transform, name and any other information that should be
	//unique to an entity
	const auto& transform = m_registry.get<Transform>(_target);
	//const auto& override_transform = _override == entt::null ? m_registry.get<Transform>(_target) : m_registry.get<Transform>(_override);
	//const auto& tag = _override == entt::null ? m_registry.get<Tag>(_target) : m_registry.get<Tag>(_override);
	//const auto& name = _override == entt::null ? m_registry.get<Name>(_target) : m_registry.get<Name>(_override);

	entt::entity new_entity = m_registry.create();

	auto& new_transform = m_registry.emplace_or_replace<Transform>(new_entity, transform.m_position, transform.m_rotation, transform.m_scale);

	//if (transform.m_parent != entt::null)
	//{
	//	//set new parent
	//	new_transform.m_parent = transform.m_parent;

	//	//set children in parent
	//	m_registry.get<Transform>(transform.m_parent).m_children.insert(new_entity);
	//}


	if (_override != entt::null)
	{
		const auto override_transform = m_registry.get<Transform>(_override);
		new_transform.m_position = override_transform.m_position;
		new_transform.m_rotation = override_transform.m_rotation;
		new_transform.m_scale = override_transform.m_scale;
	}
	//m_registry.emplace_or_replace<Tag>(new_entity, tag.m_tag);
	//m_registry.emplace_or_replace<Name>(new_entity, name.m_name);

	for (const auto& ent : transform.m_children)
	{
		//creates a new entity first
		entt::entity new_child = ECS::CopyEntity(ent);
		//set up the parent child relationship
		new_transform.m_children.insert(new_child);
		//set child's parent
		m_registry.get<Transform>(new_child).m_parent = new_entity;
	}

	Try_Copy<Tag>(new_entity, _override == entt::null ? _target : _override);
	Try_Copy<Name>(new_entity, _override == entt::null ? _target : _override);
	Try_Copy<PositionList>(new_entity, _target);
	Try_Copy<Renderer>(new_entity, _target);
	Try_Copy<PrimitiveRender>(new_entity, _target);
	Try_Copy<DebugRender>(new_entity, _target);
	Try_Copy<Point_Light>(new_entity, _target);
	Try_Copy<Directional_Light>(new_entity, _target);
	Try_Copy<Text>(new_entity, _target);
	Try_Copy<Camera>(new_entity, _target);
	Try_Copy<ParticleSystem>(new_entity, _target);
	Try_Copy<Canvas>(new_entity, _target);
	//Try_Copy<GUID>(new_entity, _target);	//GUID should not be copied
	Try_Copy<PrefabInstance>(new_entity, _target);

	//DependantComponents
	Try_Copy<BoxCollider>(new_entity, _target);
	Try_Copy<SphereCollider>(new_entity, _target);
	Try_Copy<AudioSource>(new_entity, _target);
	Try_Copy<Rigidbody>(new_entity, _target);

	Try_Copy<Projectile>(new_entity, _target);
	Try_Copy<Laser>(new_entity, _target);

	Try_Copy<Scriptable>(new_entity, _target);
	Try_Copy<PauseScriptable>(new_entity, _target);
	Try_Copy<ScriptVariables>(new_entity, _target);
	//Try_Copy<>(new_entity, _target);
	//Try_Copy<Prefab>(new_entity, _target);

	Try_Copy<OBBBoxCollider>(new_entity, _target);
	Try_Copy<MeshCollider>(new_entity, _target);

	return new_entity;
}

std::string ECS::GenerateGUID()
{
	//while (true)
	//{
		UUID uuid;
		UuidCreate(&uuid);
		char* str;
		UuidToStringA(&uuid, (RPC_CSTR*)&str);
		std::string ret_str{ str };
		RpcStringFreeA((RPC_CSTR*)&str);


	//}

	return ret_str;
}

void ECS::Clear()
{
	m_registry.clear();
	m_prefab_map.clear();
	m_EntitiesToDelete.clear();
	m_EntitiesToDisable.clear();
	m_EntitiesToEnable.clear();

	PogplantDriver::Application::GetInstance().m_sPhysicsSystem.Clear();
}

void ECS::SetParent(entt::entity _parent, entt::entity _child)
{
	auto& _child_transform = m_registry.get<Transform>(_child);

	if (_child_transform.m_parent != entt::null)
	{
		RemoveParentFrom(_child);
	}

	auto _translate = _child_transform.GetGlobalPosition();
	auto _rotation = _child_transform.GetGlobalRotation();
	auto _scale = _child_transform.GetGlobalScale();

	_child_transform.m_parent = _parent;

	auto& _parent_transform = m_registry.get<Transform>(_parent);
	_child_transform.updateModelMtx(_parent_transform);

	_child_transform.SetGlobalPosition(_translate);
	_child_transform.SetGlobalRotation(_rotation);
	_child_transform.SetGlobalScale(_scale);

	_parent_transform.m_children.insert(_child);

}

entt::entity ECS::RemoveParentFrom(entt::entity _id)
{
	auto& _TargetTransform = m_registry.get<Components::Transform>(_id);
	entt::entity _parent_id = entt::null;
	if (_TargetTransform.m_parent != entt::null)
	{
		//add parent transform to child transform or something to fix the local transform issue
		auto _translate = _TargetTransform.GetGlobalPosition();
		auto _rotation = _TargetTransform.GetGlobalRotation();
		auto _scale = _TargetTransform.GetGlobalScale();

		RemoveChildFrom(_TargetTransform.m_parent, _id);
		_parent_id = _TargetTransform.m_parent;
		_TargetTransform.m_parent = entt::null;

		_TargetTransform.SetGlobalPosition(_translate);
		_TargetTransform.SetGlobalRotation(_rotation);
		_TargetTransform.SetGlobalScale(_scale);
	}
	return _parent_id;
}

void ECS::SetChild(entt::entity _parent, entt::entity _child)
{
	auto& _parent_transform = m_registry.get<Transform>(_parent);
	_parent_transform.m_children.insert(_child);
}

void ECS::RemoveChildFrom(entt::entity _parent, entt::entity _child)
{
	auto& _parentTransform = m_registry.get<Components::Transform>(_parent);
	_parentTransform.m_children.erase(_child);
}

bool ECS::IsChildOf(entt::entity _parent, entt::entity _child)
{
	const auto& _parentTransform = m_registry.get<Components::Transform>(_parent);

	return _parentTransform.m_children.contains(_child);
}

void ECS::DisableEntity(entt::entity _entity)
{
	m_EntitiesToDisable.insert(_entity);
}

void ECS::EnableEntity(entt::entity _entity)
{
	m_EntitiesToEnable.insert(_entity);
}

void ECS::ToggleEntity(entt::entity _entity)
{
	if (m_registry.try_get<Disabled>(_entity))
		m_EntitiesToEnable.insert(_entity);
	else
		m_EntitiesToDisable.insert(_entity);
}

void ECS::RemovePrefabInstance(entt::entity _entity)
{
	auto prefab_instance = m_registry.try_get<PrefabInstance>(_entity);
	if (prefab_instance)
		m_registry.remove<PrefabInstance>(_entity);
}

void ECS::TrulyDisableEntity(entt::entity _entity)
{
	//assert(m_registry.try_get<Disabled>(_entity) == nullptr);
	if (m_registry.try_get<Disabled>(_entity))
		return;

	auto& _transform = m_registry.get<Transform>(_entity);
	//disable all child
	for (auto e : _transform.m_children)
		TrulyDisableEntity(e);

	PogplantDriver::Application::GetInstance().m_sPhysicsSystem.RemoveEntityFromTree(_entity);

	m_registry.emplace<Disabled>(_entity);
	m_registry.get<Name>(_entity).status = false;
}

void ECS::TrulyEnableEntity(entt::entity _entity)
{
	if (!m_registry.valid(_entity))
		return;

	//assert(m_registry.try_get<Disabled>(_entity) != nullptr);
	if (m_registry.try_get<Disabled>(_entity) == nullptr)
		return;

	auto& _transform = m_registry.get<Transform>(_entity);
	//enable all child
	for (auto e : _transform.m_children)
		TrulyEnableEntity(e);

	m_registry.remove<Disabled>(_entity);
	m_registry.get<Name>(_entity).status = true;
}