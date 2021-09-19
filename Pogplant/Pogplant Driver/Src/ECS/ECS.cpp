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

entt::registry& ECS::GetReg()
{
	return m_registry;
}