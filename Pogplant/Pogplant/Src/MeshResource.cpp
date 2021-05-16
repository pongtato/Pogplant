#include "MeshResource.h"
#include "Mesh.h"
#include "MeshInstance.h"

namespace Pogplant
{
	std::unordered_map<MeshResource::MESH_TYPE, Mesh*> MeshResource::m_MeshPool;
	std::vector<InstanceData> MeshResource::m_MeshInstances;

	void MeshResource::InitResource()
	{
		for (size_t i = 0; i < COUNT; ++i)
		{
			m_MeshPool[static_cast<MESH_TYPE>(i)] = new Mesh();
		}
	}

	void MeshResource::CleanUpResource()
	{
		for (size_t i = 0; i < COUNT; ++i)
		{
			delete m_MeshPool[static_cast<MESH_TYPE>(i)];
		}
	}

	void MeshResource::Draw(MESH_TYPE _Mesh_Type, unsigned int _Texture)
	{
		m_MeshPool[_Mesh_Type]->Draw(_Texture);
	}

	void MeshResource::DrawInstanced(MESH_TYPE _Mesh_Type)
	{
		m_MeshPool[_Mesh_Type]->DrawInstanced();
	}
}
