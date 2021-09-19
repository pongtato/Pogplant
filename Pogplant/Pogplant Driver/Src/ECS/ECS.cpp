#include "ECS.h"
#include "Entity.h"

using namespace Components;

ECS::ECS()
{
}

ECS::~ECS()
{
}

Entity ECS::CreateEntity(std::string str, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale)
{
	Entity entity = { m_registry.create(), this };
	entity.AddComponent<Tag>(str);
	entity.AddComponent<Transform>(pos, rot, scale);
	entity.AddComponent<Name>("letsgoooo");

	return entity;
}

Entity ECS::CreateChild(entt::entity parent,
						std::string str,
						glm::vec3 pos,
						glm::vec3 rot,
						glm::vec3 scale)
{
	auto _p_r = m_registry.try_get<Relationship>(parent);

	//create child
	auto _child = CreateEntity();

	//child relationship component
	auto& _c_r = m_registry.emplace<Relationship>(_child.GetID());
	_c_r.m_parent = parent;

	if (_p_r)
		_p_r->m_children.insert(_child.GetID());
	else
		m_registry.emplace<Relationship>(parent).m_children.insert(_child.GetID());

	return _child;
}

entt::registry& ECS::GetReg()
{
	return m_registry;
}

void ECS::DestroyEntity(entt::entity entity)
{
	auto _r = m_registry.try_get<Relationship>(entity);

	//destory childrens first
	if (_r)
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
	}

	//destroy itself
	m_registry.destroy(entity, 0);

}