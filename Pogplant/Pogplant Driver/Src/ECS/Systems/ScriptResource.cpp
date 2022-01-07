#include "ScriptResource.h"

std::unordered_map<entt::entity, std::unordered_map<std::string, std::unique_ptr<MonoObjectWithGC>>> ScriptResource::m_MonoObjects;
//std::unordered_multimap<entt::entity, std::unique_ptr<MonoObjectWithGC>> ScriptResource::m_MonoObjects;
std::vector<std::string> ScriptResource::m_MonoScriptNames;
std::vector<std::string> ScriptResource::m_PauseScriptNames;
std::vector<entt::entity> ScriptResource::m_EntitiesToDelete;

void ScriptResource::RemoveEntity(const entt::entity& entity)
{
	m_MonoObjects.erase(entity);
}
