#include "MeshInstance.h"
#include "MeshResource.h"

namespace Pogplant
{
	size_t MeshInstance::m_InstanceCount = 0;

	void MeshInstance::InitMeshInstance(size_t _PoolSize)
	{
		MeshResource::m_MeshInstances.resize(_PoolSize);
	}

	void MeshInstance::SetInstance(InstanceData _Instance_Data)
	{
		MeshResource::m_MeshInstances[m_InstanceCount] = _Instance_Data;
		m_InstanceCount++;
	}

	void MeshInstance::ResetCount()
	{
		m_InstanceCount = 0;
	}

	size_t MeshInstance::GetInstanceCount()
	{
		return m_InstanceCount;
	}
}