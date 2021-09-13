#include "ModelResource.h"
#include "Model.h"
#include <glew.h>

namespace Pogplant
{
	std::unordered_map<std::string, Model*> ModelResource::m_ModelPool;

	void ModelResource::InitResource()
	{
		// Driver edit json, this reads from the json or smth in the future idk kekw
		LoadModel("BAG", "Models/backpack/backpack.obj", GL_TRIANGLES);
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
		m_ModelPool[_ModelID] = new Model(_Path, _PrimitiveType);
	}
}