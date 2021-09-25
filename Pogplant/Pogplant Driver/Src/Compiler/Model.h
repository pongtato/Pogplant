#ifndef _MODEL_H_
#define _MODEL_H_

#include <Importer.hpp>
#include <scene.h>
#include <postprocess.h>
#include <glm.hpp>
#include <string>
#include <vector>

typedef unsigned int uint;

struct Vertex
{
	glm::vec3 m_Position;
	glm::vec3 m_Normal;
	glm::vec3 m_Tangent;
	glm::vec3 m_BiTangent;
	glm::vec2 m_TexCoords;
};

struct Texture
{
	unsigned int m_Id;
	std::string m_Type;
	std::string m_Path;
};

struct Mesh3D
{
	// 0x0004 = GL_TRIANGLES
	Mesh3D(std::vector<Vertex> _Vertices, std::vector<uint> _Indices, std::vector<Texture> _Textures, uint _PrimitiveType = 0x0004);

	std::vector<Vertex> m_Vertices;
	std::vector<uint> m_Indices;
	std::vector<Texture> m_Textures;
	uint m_PrimitiveType;
};

class Model
{
public:
	// 0x0004 = GL_TRIANGLES
	Model(std::string _Path, uint _PrimitiveType = 0x0004);

	std::vector<Texture> m_TexturesLoaded;
	std::vector<Mesh3D> m_Meshes;
	std::string m_Directory;

private:
	void LoadModel(std::string _Path, uint _PrimitiveType);
	void ProcessNode(aiNode* _Node, const aiScene* _Scene, uint _PrimitiveType);
	Mesh3D ProcessMesh(aiMesh* _Mesh, const aiScene* _Scene, uint _PrimitiveType);
	std::vector<Texture> LoadMaterialTextures(aiMaterial* _Material, aiTextureType _Type, std::string _TypeName);
};


#endif // _MODEL_H_#pragma once
