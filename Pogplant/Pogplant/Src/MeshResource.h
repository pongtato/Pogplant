#pragma once

#include <unordered_map>
#include <vector>

namespace Pogplant
{
	struct Mesh;
	struct Mesh3D;
	struct InstanceData;

	struct MeshResource
	{
		enum MESH_TYPE
		{
			QUAD,
			SCREEN,
			COUNT_2D,
		};

		static void InitResource();
		static void CleanUpResource();
		static void Draw(MESH_TYPE _Mesh_Type, unsigned int _Texture);
		static void DrawInstanced(MESH_TYPE _Mesh_Type);

		static std::unordered_map<MESH_TYPE, Mesh*> m_MeshPool;
		static std::vector<InstanceData> m_MeshInstances;
	};
}

