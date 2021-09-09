#pragma once

#include <string>
#include <unordered_map>

namespace Pogplant
{
	class Model;

	struct ModelResource
	{
		static void InitResource();
		static void CleanUpResource();
		// 0x0004 = GL_TRIANGLES
		static void LoadModel(std::string _ModelID, std::string _Path, unsigned int _PrimitiveType = 0x0004);
		static std::unordered_map<std::string, Model*> m_ModelPool;
	};
}