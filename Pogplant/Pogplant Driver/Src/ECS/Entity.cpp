#include "Entity.h"

Entity::Entity(entt::entity id, ECS* ecs)
	: m_id(id), m_ecs(ecs)
{
}

Entity::~Entity()
{
}