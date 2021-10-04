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
		namespace fs = std::filesystem;
		LoadModel(fs::path("Resources/KekFiles/Cube.kek").stem().string(), "Resources/KekFiles/Cube.kek", GL_TRIANGLES);
		LoadModel(fs::path("Resources/KekFiles/Sphere.kek").stem().string() , "Resources/KekFiles/Sphere.kek", GL_TRIANGLES);
		LoadModel(fs::path("Resources/KekFiles/Player_Ship.kek").stem().string(), "Resources/KekFiles/Player_Ship.kek", GL_TRIANGLES);
		LoadModel(fs::path("Resources/KekFiles/Enemy_01.kek").stem().string(), "Resources/KekFiles/Enemy_01.kek", GL_TRIANGLES);
		LoadModel(fs::path("Resources/KekFiles/backpack.kek").stem().string(), "Resources/KekFiles/backpack.kek", GL_TRIANGLES);
	}

	void ModelResource::CleanUpResource()
	{
		for (auto& it : m_ModelPool)
		{
			delete it.second;
		}
	}

	void ModelResource::LoadModel(std::string _ModelID, std::string _Path, unsigned int _PrimitiveType)
	{
		// 0 to use assimp, 1 to use asset compiler
		m_ModelPool[_ModelID] = new Model(_Path, _PrimitiveType, _ModelID);
	}

	void ModelResource::UpdateModel(std::string _ModelID, std::string _Path, unsigned int _PrimitiveType)
	{
		Model temp(_Path, _PrimitiveType, _ModelID);

		m_ModelPool[_ModelID]->m_Bounds = temp.m_Bounds;
		m_ModelPool[_ModelID]->m_Directory = temp.m_Directory;
		m_ModelPool[_ModelID]->m_Meshes = temp.m_Meshes;
		m_ModelPool[_ModelID]->m_Model_key = temp.m_Model_key;
		m_ModelPool[_ModelID]->m_TexturesLoaded = temp.m_TexturesLoaded;
	}

	void ModelResource::LoadModel(std::unordered_map<std::string, Model*>& _ModelPool, std::string _ModelID, std::string _Path, unsigned int _PrimitiveType)
	{
		_ModelPool[_ModelID] = new Model(_Path, _PrimitiveType, _ModelID);
	}

	void ModelResource::UpdateModel(std::unordered_map<std::string, Model*>& _ModelPool, std::string _ModelID, std::string _Path, unsigned int _PrimitiveType)
	{
		Model temp(_Path, _PrimitiveType, _ModelID);

		_ModelPool[_ModelID]->m_Bounds = temp.m_Bounds;
		_ModelPool[_ModelID]->m_Directory = temp.m_Directory;
		_ModelPool[_ModelID]->m_Meshes = temp.m_Meshes;
		_ModelPool[_ModelID]->m_Model_key = temp.m_Model_key;
		_ModelPool[_ModelID]->m_TexturesLoaded = temp.m_TexturesLoaded;
	}
}