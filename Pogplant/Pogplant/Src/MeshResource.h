#pragma once

#include <unordered_map>
#include <vector>

namespace Pogplant
{
	struct Mesh;
	struct InstanceData;

	struct MeshResource
	{
		enum MESH_TYPE
		{
			QUAD,
			COUNT,
		};

		static void InitResource();
		static void CleanUpResource();
		static void DrawPrimitive(MESH_TYPE _Mesh_Type);

		static std::unordered_map<MESH_TYPE, Mesh*> m_MeshPool;
		static std::vector<InstanceData> m_MeshInstances;
	};
}

