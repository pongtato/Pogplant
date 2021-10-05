/*****************************************************************************/
/*!
\file	Model.h
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	Model class that contains all data for geometry of models

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

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
	std::string m_Type;
	std::string m_Path;
};

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

struct Mesh3D
{
	// 0x0004 = GL_TRIANGLES
	Mesh3D(std::vector<Vertex> _Vertices, std::vector<uint> _Indices, std::vector<Texture> _Textures);

	std::vector<Vertex> m_Vertices;
	std::vector<uint> m_Indices;
	std::vector<Texture> m_Textures;
};

class Model
{
public:
	// 0x0004 = GL_TRIANGLES
	Model(std::string _Path);

	std::vector<Texture> m_TexturesLoaded;
	std::vector<Mesh3D> m_Meshes;
	Bounds m_Bounds;
	std::string m_Directory;

private:
	void LoadModel(std::string _Path);
	void ProcessNode(aiNode* _Node, const aiScene* _Scene);
	Mesh3D ProcessMesh(aiMesh* _Mesh, const aiScene* _Scene);
	std::vector<Texture> LoadMaterialTextures(aiMaterial* _Material, aiTextureType _Type, std::string _TypeName);
};


#endif // _MODEL_H_
