#pragma once

#include <unordered_map>

namespace Pogplant
{
	struct Mesh3D;

	class MeshBuilder
	{
	public:
		static void InitMesh(size_t _PoolSize = 10000);
		static void CleanUpMesh();
		static void RebindQuad();
	private:
		static void GenerateQuad();
		static void GenerateScreen();
	};
}

