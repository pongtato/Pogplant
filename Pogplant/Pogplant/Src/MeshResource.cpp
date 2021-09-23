#include "MeshResource.h"
#include "Mesh.h"
#include "Mesh3D.h"
#include "MeshInstance.h"

namespace Pogplant
{
	std::unordered_map<MeshResource::MESH_TYPE, Mesh*> MeshResource::m_MeshPool;
	std::vector<InstanceData> MeshResource::m_MeshInstances;

	void MeshResource::InitResource()
	{
		for (size_t i = 0; i < COUNT_2D; ++i)
		{
			m_MeshPool[static_cast<MESH_TYPE>(i)] = new Mesh();
		}
	}

	void MeshResource::CleanUpResource()
	{
		// Clear the meshes in the pools
		for (auto& it : m_MeshPool)
		{
			delete it.second;
		}
	}

	void MeshResource::Draw(MESH_TYPE _Mesh_Type)
	{
		m_MeshPool[_Mesh_Type]->Draw();
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
