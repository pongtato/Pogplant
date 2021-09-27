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
		LoadModel("CUBE", "Resources/KekFiles/cube.kek", GL_TRIANGLES, 1);
		LoadModel("SPHERE", "Resources/KekFiles/sphere.kek", GL_TRIANGLES, 1);
		LoadModel("SHIP", "Resources/KekFiles/Player_Ship.kek", GL_TRIANGLES, 1);
		LoadModel("ENEMY", "Resources/KekFiles/Enemy_01.kek", GL_TRIANGLES, 1);
		LoadModel("BAG", "Resources/KekFiles/backpack.kek", GL_TRIANGLES, 1);
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
}