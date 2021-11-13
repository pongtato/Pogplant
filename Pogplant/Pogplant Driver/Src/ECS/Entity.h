#ifndef ENTITY_H
#define ENTITY_H

#include "ECS.h"

#include "Components/Components.h"

//essentially a wrapper around the ECS functions
class Entity
{
public:
	Entity(entt::entity id, ECS* ecs);
	~Entity();

	template<typename Type>
	Type* HasComponent()
	{
		return m_ecs->m_registry.try_get<Type>(m_id);
	}

	template<typename Type, typename... Args>
	Type& AddComponent(Args&&... args)
	{
		//assert here, welp

		return m_ecs->m_registry.emplace<Type>(m_id, std::forward<Args>(args)...);
	}

	template<typename Type>
	Type& GetComponent()
	{
		return m_ecs->m_registry.get<Type>(m_id);
	}

	template<typename Type>
	Type& RemoveComponent()
	{
		return m_ecs->m_registry.remove<Type>(m_id);
	}

	inline entt::entity GetID() { return m_id; }

	operator bool() const { return m_id != entt::null; }

private:
	entt::entity m_id = entt::null;
	ECS* m_ecs = nullptr;
};

#endif ENTITY_H