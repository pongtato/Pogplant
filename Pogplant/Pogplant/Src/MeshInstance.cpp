#include "MeshInstance.h"
#include "MeshResource.h"

namespace Pogplant
{
	size_t MeshInstance::m_InstanceCount = 0;
	size_t MeshInstance::m_AlphaIndex = 0;

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
		m_AlphaIndex = 0;
	}

	void MeshInstance::ForceAlpha()
	{
		// This should not < 0 since this is iterated within the loop for canvas
		m_AlphaIndex = m_InstanceCount - 1;
	}

	void MeshInstance::AlphaSwap()
	{
		if (m_InstanceCount > 0)
		{
			std::swap(MeshResource::m_MeshInstances[m_AlphaIndex], MeshResource::m_MeshInstances[m_InstanceCount - 1]);
		}
	}

	size_t MeshInstance::GetInstanceCount()
	{
		return m_InstanceCount;
	}
}