#ifndef _MONO_OBJECT_RESOURCE_H
#define _MONO_OBJECT_RESOURCE_H

#include "entt_ecs/entt.hpp"
//#include "../entt_ecs/entt.hpp"
#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>

#include <unordered_map>
#include <vector>
#include <memory>

struct MonoObjectWithGC
{
	uint32_t m_GCHandle;
	MonoObject* m_MonoObject;
};

struct ScriptResource
{
	static void RemoveEntity(const entt::entity& entity);
	// Name of available scripts
	static std::vector<std::string> m_allScriptNames;
	// Map of MonoObjects
	//static std::unordered_map<std::string, std::unique_ptr<MonoObjectWithGC>> m_MonoObjects;
	// Lookup via entityID, second will be the name of scripts and the mono object.
	static std::unordered_map<entt::entity, std::unordered_map<std::string, std::unique_ptr<MonoObjectWithGC>>> m_MonoObjects;
};

#endif // _MONO_OBJECT_RESOURCE_H