#include "MeshResource.h"
#include "Mesh.h"
#include "MeshInstance.h"

namespace Pogplant
{
	std::unordered_map<MeshResource::MESH_TYPE, Mesh*> MeshResource::m_MeshPool;
	std::vector<InstanceData> MeshResource::m_MeshInstances;

	void MeshResource::InitResource()
	{
		m_MeshPool[QUAD] = new Mesh();
	}

	void MeshResource::CleanUpResource()
	{
		delete m_MeshPool[QUAD];
	}

	void MeshResource::DrawPrimitive(MESH_TYPE _Mesh_Type)
	{
		m_MeshPool[_Mesh_Type]->Draw();
	}
}
