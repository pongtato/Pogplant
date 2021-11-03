#ifndef _MONO_OBJECT_RESOURCE_H
#define _MONO_OBJECT_RESOURCE_H

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
	// Name of available scripts
	static std::vector<std::string> m_scriptNames;
	// Map of MonoObjects
	static std::unordered_map<std::string, std::unique_ptr<MonoObjectWithGC>> m_MonoObjects;
};

#endif // _MONO_OBJECT_RESOURCE_H