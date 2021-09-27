#include "ModelResource.h"
#include "Model.h"
#include <glew.h>

namespace Pogplant
{
	std::unordered_map<std::string, Model*> ModelResource::m_ModelPool;

	void ModelResource::InitResource()
	{
		// Driver edit json, this reads from the json or smth in the future idk kekw
		//LoadModel("BAG", "Resources/Models/backpack/backpack.obj", GL_TRIANGLES, 0);
		//LoadModel("CUBE", "Resources/Models/cube/cube.fbx", GL_TRIANGLES, 0);
		//LoadModel("SPHERE", "Resources/Models/sphere/sphere.fbx", GL_TRIANGLES, 0);
		//LoadModel("SHIP", "Resources/Models/Ship/Player_Ship.fbx", GL_TRIANGLES, 0);
		//LoadModel("ENEMY", "Resources/Models/Enemy/Enemy_01.fbx", GL_TRIANGLES, 0);

		// Load from .kek files
		LoadModel(GetFileName("Resources/KekFiles/Cube.kek"), "Resources/KekFiles/Cube.kek", GL_TRIANGLES, 1);
		LoadModel(GetFileName("Resources/KekFiles/Sphere.kek"), "Resources/KekFiles/Sphere.kek", GL_TRIANGLES, 1);
		LoadModel(GetFileName("Resources/KekFiles/Player_Ship.kek"), "Resources/KekFiles/Player_Ship.kek", GL_TRIANGLES, 1);
		LoadModel(GetFileName("Resources/KekFiles/Enemy_01.kek"), "Resources/KekFiles/Enemy_01.kek", GL_TRIANGLES, 1);
		LoadModel(GetFileName("Resources/KekFiles/backpack.kek"), "Resources/KekFiles/backpack.kek", GL_TRIANGLES, 1);
	}

	void ModelResource::CleanUpResource()
	{
		for (auto& it : m_ModelPool)
		{
			delete it.second;
		}
	}

	void ModelResource::LoadModel(std::string _ModelID, std::string _Path, unsigned int _PrimitiveType, int _Mode)
	{
		// 0 to use assimp, 1 to use asset compiler
		m_ModelPool[_ModelID] = new Model(_Path, _PrimitiveType, _ModelID, _Mode);
	}

	std::string ModelResource::GetFileName(const std::string& fullpath)
	{
		std::string filename;
		size_t found = fullpath.find_last_of('/');
		filename = fullpath.substr(found + 1, fullpath.find_last_of('.') - found - 1);
		return filename;
	}
}