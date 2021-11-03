#include "ScriptResource.h"

std::unordered_map<std::string, std::unique_ptr<MonoObjectWithGC>> ScriptResource::m_MonoObjects;
std::vector<std::string> ScriptResource::m_scriptNames;