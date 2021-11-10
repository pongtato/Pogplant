#include "ECS.h"
#include "Entity.h"
#include "Systems/ScriptResource.h"

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
	//auto _r = m_registry.try_get<Relationship>(entity);
	if (!m_registry.valid(entity))
		return;

	auto& _transform = m_registry.get<Transform>(entity);

	//Destroy all children
	auto copy_set = _transform.m_children;
	for (auto child : copy_set)
	{
		DestroyEntity(child);
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

	//destory childrens first
	/*if (_r)
	{
		//if it's a parent, delete all children
		auto copy_set = _r->m_children;
		for (auto child : copy_set)
		{
			DestroyEntity(child);
		}

		//remove parent's connection
		if (_r->m_parent != entt::null)
		{
			auto _p_r = m_registry.try_get<Relationship>(_r->m_parent);

			if (_p_r)
			{
				if (_p_r->m_children.erase(entity) == 0)
					printf("Tried to delete a child that didn't exist\n");
			}
		}
	}*/

	auto _s = m_registry.try_get<Scriptable>(entity);

	// Clear from the Script resource pool
	if (_s)
	{
		ScriptResource::RemoveEntity(entity);
	}
	
	//destroy itself
	//m_registry.destroy(entity, 0);
	m_registry.destroy(entity);

}

//returns the first entity with the name
entt::entity ECS::FindEntityWithName(std::string _name)
{
	auto view = m_registry.view<Name>();

	for (auto& entity : view)
	{
		auto name = view.get<Name>(entity);
		if (name.m_name == _name)
			return entity;
	}

	return entt::null;
}

//returns the first entity with the name
entt::entity ECS::FindChildEntityWithName(entt::entity parentID, std::string _name)
{
	auto& transform = m_registry.get<Transform>(parentID);
	for (auto& entity : transform.m_children)
	{
		auto name = m_registry.get<Name>(entity);
		if (name.m_name == _name)
			return entity;
	}

	return entt::null;
}


//returns the first entity with the tag
entt::entity ECS::FindEntityWithTag(std::string _tag)
{
	auto view = m_registry.view<Tag>();

	for (auto& entity : view)
	{
		auto tag = view.get<Tag>(entity);
		if (tag.m_tag == _tag)
			return entity;
	}


	auto results = m_registry.view<Transform, Renderer>();
	return entt::null;
}


