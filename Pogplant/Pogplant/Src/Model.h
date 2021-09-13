#pragma once

#include "Mesh3D.h"

#include <Importer.hpp>
#include <scene.h>
#include <postprocess.h>

namespace Pogplant
{
	struct Bounds
	{
		float minX;
		float maxX;
		float minY;
		float maxY;
		float minZ;
		float maxZ;
		float longest;
	};

	class Model
	{
	public:
		// 0x0004 = GL_TRIANGLES
		Model(std::string _Path, uint _PrimitiveType = 0x0004);
		void Draw() const;

		std::vector<Texture> m_TexturesLoaded;
		std::vector<Mesh3D> m_Meshes;
		std::string m_Directory;
		Bounds m_Bounds;

	private:
		void LoadModel(std::string _Path, uint _PrimitiveType);
		void ProcessNode(aiNode* _Node, const aiScene* _Scene, uint _PrimitiveType);
		Mesh3D ProcessMesh(aiMesh* _Mesh, const aiScene* _Scene, uint _PrimitiveType);
		std::vector<Texture> LoadMaterialTextures(aiMaterial* _Material, aiTextureType _Type, std::string _TypeName);
	};
}