#pragma once

#include <glm.hpp>
#include <vector>
#include <unordered_map>

namespace Pogplant
{
	struct Mesh3D;
	struct Skybox;

	class MeshBuilder
	{
	public:
		static void InitMesh(size_t _PoolSize = 10000);
		static void CleanUpMesh();
		static void RebindQuad();
		static void RebindTextQuad(float _X, float _Y, float _Width, float _Height, float _UVx, float _UVy);
		static void RebindLines(const std::vector<glm::vec3>& _Points);
	private:
		static void GenerateQuad();
		static void GenerateTextQuad();
		static void GenerateScreen();
		static void GenerateSkybox();
		static void GenerateLines();
	};
}

