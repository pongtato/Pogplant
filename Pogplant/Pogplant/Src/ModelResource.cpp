#include "ModelResource.h"
#include "Model.h"
#include <glew.h>

namespace Pogplant
{
	std::unordered_map<std::string, Model*> ModelResource::m_ModelPool;
	//std::unordered_map<std::string, Mesh3D*> ModelResource::m_UsedMeshes;

	void ModelResource::InitResource()
	{
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