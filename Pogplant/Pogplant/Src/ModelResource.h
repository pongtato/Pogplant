#pragma once

#include <string>
#include <unordered_map>

namespace Pogplant
{
	class Model;
	struct Mesh3D;

	struct ModelResource
	{
		static void InitResource();
		static void CleanUpResource();
		// 0x0004 = GL_TRIANGLES
		// _Mode = 0 to use assimp, 1 to use asset compiler
		static void LoadModel(std::string _ModelID, std::string _Path, unsigned int _PrimitiveType = 0x0004);
		static void UpdateModel(std::string _ModelID, std::string _Path, unsigned int _PrimitiveType = 0x0004);
		static void LoadModel(std::unordered_map<std::string, Model*>& _ModelPool, std::string _ModelID, std::string _Path,unsigned int _PrimitiveType = 0x0004);
		static void UpdateModel(std::unordered_map<std::string, Model*>& _ModelPool, std::string _ModelID, std::string _Path,unsigned int _PrimitiveType = 0x0004);
		static std::unordered_map<std::string, Model*> m_ModelPool;
		//static std::unordered_map<std::string, Mesh3D*> m_UsedMeshes;
	};
}