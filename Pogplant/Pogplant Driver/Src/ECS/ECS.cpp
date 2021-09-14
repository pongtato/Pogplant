#include "ECS.h"
#include "Entity.h"

using namespace Components;

ECS::ECS()
{
}

ECS::~ECS()
{
}

Entity ECS::CreateEntity(std::string str)
{
	Entity entity = { m_registry.create(), this };
	entity.AddComponent<Tag>(str);

	return entity;
}

entt::registry& ECS::GetReg()
{
	return m_registry;
}