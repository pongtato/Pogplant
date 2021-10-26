#include "ScriptSystem.h"
#include "../Entity.h"

std::uint32_t ScriptSystem::CreateEntity(MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale)
{
	std::string _name = mono_string_to_utf8(name);
	return static_cast<std::uint32_t>(m_ecs->CreateEntity(_name, pos, rot, scale).GetID());
}

void ScriptSystem::DestroyEntity(std::uint32_t entityID)
{
	m_ecs->DestroyEntity(static_cast<entt::entity>(entityID));
}

std::uint32_t ScriptSystem::CreateChild(std::uint32_t parentID, MonoString* name, glm::vec3 pos, glm::vec3 rot, glm::vec3 scale)
{
	std::string _name = mono_string_to_utf8(name);
	return static_cast<std::uint32_t>(m_ecs->CreateChild(static_cast<entt::entity>(parentID), _name, pos, rot, scale));
}

std::uint32_t ScriptSystem::FindEntityWithTag(MonoString* name)
{
	std::string _name = mono_string_to_utf8(name);
	return static_cast<std::uint32_t>(m_ecs->FindEntityWithTag(_name));
}