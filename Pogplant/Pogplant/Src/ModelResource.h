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
		// _Mode = 0 to use assimp, 1 to use asset compiler
		static void LoadModel(std::string _ModelID, std::string _Path, unsigned int _PrimitiveType = 0x0004, int _Mode = 1);
		static std::string GetFileName(const std::string& fullpath);
		static std::unordered_map<std::string, Model*> m_ModelPool;
	};
}