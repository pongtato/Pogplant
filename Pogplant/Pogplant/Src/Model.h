#pragma once

#include "Mesh3D.h"
#include <filesystem>
#include <iostream>
#include <string>
#include <sstream>
#include <fstream>

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
		Model() {};
		Model(std::string _Path, uint _PrimitiveType = 0x0004, std::string _Key = "Key");
		void Draw(bool _BindTex) const;
		bool LoadFromFile(std::string filePath);

		Bounds m_Bounds;
		std::vector<Texture> m_TexturesLoaded;
		std::vector<Mesh3D> m_Meshes;
		std::string m_Directory;
		std::string m_Model_key;

	private:
		//void LoadModel(std::string _Path, uint _PrimitiveType);
		//void ProcessNode(aiNode* _Node, const aiScene* _Scene, uint _PrimitiveType);
		//Mesh3D ProcessMesh(aiMesh* _Mesh, const aiScene* _Scene, uint _PrimitiveType);
		std::vector<Texture> LoadMaterialTextures(aiMaterial* _Material, aiTextureType _Type, std::string _TypeName);
		std::vector<Texture> LoadMaterialTextures(std::string& _Material, std::string _TypeName);
	};
}