#include "Model.h"
#include "Logger.h"
#include "TextureLoader.h"

#include <limits>

#include <iostream>

namespace Pogplant
{
	Model::Model(std::string _Path, uint _PrimitiveType, std::string _Key)
	{
        (void)_PrimitiveType;

        m_Model_key = _Key;

        // For comparison later
        m_Bounds.minX = std::numeric_limits<float>::max();
        m_Bounds.minY = std::numeric_limits<float>::max();
        m_Bounds.minZ = std::numeric_limits<float>::max();

        m_Bounds.maxX = std::numeric_limits<float>::min();
        m_Bounds.maxY = std::numeric_limits<float>::min();
        m_Bounds.maxZ = std::numeric_limits<float>::min();

        LoadFromFile(_Path);
	}

	void Model::Draw() const
	{
		for (const auto& it : m_Meshes)
		{
			it.Draw();
		}
	}

    bool Model::LoadFromFile(std::string filePath)
    {
        // Check if file exists & read to buffer
        std::ifstream inFile;
        inFile.open(filePath, std::ios::binary);
        if (!inFile.good())
        {
            std::cout << "Unable to read file " << filePath << std::endl;
            return false;
        }

        //TexLoader::SetTextureFlip(true);

        std::getline(inFile, m_Directory);
        inFile.read(reinterpret_cast<char*>(&m_Bounds), sizeof(Bounds));

        for (std::string line; std::getline(inFile, line);)
        {
            std::vector<Vertex> vertices;
            std::vector<uint> indices;
            std::vector<Texture> textures;
            size_t vertSize, idxSize, texSize;
            std::stringstream ss;
            ss << line;
            ss >> vertSize >> idxSize >> texSize;
            vertices.resize(vertSize);
            indices.resize(idxSize);
            //textures.resize(texSize);
            inFile.read(reinterpret_cast<char*>(&vertices[0]), vertSize * sizeof(Vertex));
            inFile.read(reinterpret_cast<char*>(&indices[0]), idxSize * sizeof(uint));

            for (size_t i = 0; i < texSize; ++i)
            {
                size_t len;

                inFile.read(reinterpret_cast<char*>(&len), sizeof(size_t));
                std::string type(len, '\0');
                inFile.read(&type[0], len);

                inFile.read(reinterpret_cast<char*>(&len), sizeof(size_t));
                std::string path(len, '\0');
                inFile.read(&path[0], len);

                std::vector<Texture> texture = LoadMaterialTextures(path, type);
                textures.insert(textures.end(), texture.begin(), texture.end());
            }
            m_Meshes.push_back(Mesh3D(vertices, indices, textures));
        }

        // Find longest edge - General usage no ritter's
        float lenX = std::fabsf(m_Bounds.maxX) + std::fabsf(m_Bounds.minX);
        float lenY = std::fabsf(m_Bounds.maxY) + std::fabsf(m_Bounds.minY);
        float lenZ = std::fabsf(m_Bounds.maxZ) + std::fabsf(m_Bounds.minZ);
        m_Bounds.longest = std::max(lenX, lenY);
        m_Bounds.longest = std::max(m_Bounds.longest, lenZ);
        return true;
    }

	//void Model::LoadModel(std::string _Path, uint _PrimitiveType)
	//{
	//	Assimp::Importer importer;
	//	const aiScene* scene = importer.ReadFile(_Path, aiProcess_Triangulate | aiProcess_GenSmoothNormals | aiProcess_FlipUVs | aiProcess_CalcTangentSpace);
	//	// check for errors
	//	if (!scene || scene->mFlags & AI_SCENE_FLAGS_INCOMPLETE || !scene->mRootNode) // if is Not Zero
	//	{
	//		Logger::Log({ "PP::ERROR::ASSIMP",LogEntry::ERROR, importer.GetErrorString() });
	//		return;
	//	}
	//	m_Directory = _Path.substr(0, _Path.find_last_of('/'));
	//	ProcessNode(scene->mRootNode, scene, _PrimitiveType);

 //       // Find longest edge - General usage no ritter's
 //       float lenX = std::fabsf(m_Bounds.maxX) + std::fabsf(m_Bounds.minX);
 //       float lenY = std::fabsf(m_Bounds.maxY) + std::fabsf(m_Bounds.minY);
 //       float lenZ = std::fabsf(m_Bounds.maxZ) + std::fabsf(m_Bounds.minZ);
 //       m_Bounds.longest = std::max(lenX, lenY);
 //       m_Bounds.longest = std::max(m_Bounds.longest, lenZ);
	//}

	//void Model::ProcessNode(aiNode* _Node, const aiScene* _Scene, uint _PrimitiveType)
	//{
	//	// process each mesh located at the current node
	//	for (unsigned int i = 0; i < _Node->mNumMeshes; i++)
	//	{
	//		// the node object only contains indices to index the actual objects in the scene. 
	//		// the scene contains all the data, node is just to keep stuff organized (like relations between nodes).
	//		aiMesh* mesh = _Scene->mMeshes[_Node->mMeshes[i]];
	//		m_Meshes.push_back(ProcessMesh(mesh, _Scene, _PrimitiveType));
	//	}
	//	// after we've processed all of the meshes (if any) we then recursively process each of the children nodes
	//	for (unsigned int i = 0; i < _Node->mNumChildren; i++)
	//	{
	//		ProcessNode(_Node->mChildren[i], _Scene, _PrimitiveType);
	//	}
	//}

	//Mesh3D Model::ProcessMesh(aiMesh* _Mesh, const aiScene* _Scene, uint _PrimitiveType)
	//{
 //       // data to fill
 //       std::vector<Vertex> vertices;
 //       std::vector<unsigned int> indices;
 //       std::vector<Texture> textures;

 //       // walk through each of the mesh's vertices
 //       for (unsigned int i = 0; i < _Mesh->mNumVertices; i++)
 //       {
 //           Vertex vertex;
 //           glm::vec3 vector; // we declare a placeholder vector since assimp uses its own vector class that doesn't directly convert to glm's vec3 class so we transfer the data to this placeholder glm::vec3 first.
 //           // positions
 //           vector.x = _Mesh->mVertices[i].x;
 //           vector.y = _Mesh->mVertices[i].y;
 //           vector.z = _Mesh->mVertices[i].z;

 //           /// Form bounds
 //           m_Bounds.minX = std::min(m_Bounds.minX, vector.x);
 //           m_Bounds.minY = std::min(m_Bounds.minY, vector.y);
 //           m_Bounds.minZ = std::min(m_Bounds.minZ, vector.z);

 //           m_Bounds.maxX = std::max(m_Bounds.maxX, vector.x);
 //           m_Bounds.maxY = std::max(m_Bounds.maxY, vector.y);
 //           m_Bounds.maxZ = std::max(m_Bounds.maxZ, vector.z);

 //           vertex.m_Position = vector;
 //           // normals
 //           if (_Mesh->HasNormals())
 //           {
 //               vector.x = _Mesh->mNormals[i].x;
 //               vector.y = _Mesh->mNormals[i].y;
 //               vector.z = _Mesh->mNormals[i].z;
 //               vertex.m_Normal = vector;
 //           }
 //           // texture coordinates
 //           if (_Mesh->mTextureCoords[0]) // does the mesh contain texture coordinates?
 //           {
 //               glm::vec2 vec;
 //               // a vertex can contain up to 8 different texture coordinates. We thus make the assumption that we won't 
 //               // use models where a vertex can have multiple texture coordinates so we always take the first set (0).
 //               vec.x = _Mesh->mTextureCoords[0][i].x;
 //               vec.y = _Mesh->mTextureCoords[0][i].y;
 //               vertex.m_TexCoords = vec;
 //               // tangent
 //               vector.x = _Mesh->mTangents[i].x;
 //               vector.y = _Mesh->mTangents[i].y;
 //               vector.z = _Mesh->mTangents[i].z;
 //               vertex.m_Tangent = vector;
 //               // bitangent
 //               vector.x = _Mesh->mBitangents[i].x;
 //               vector.y = _Mesh->mBitangents[i].y;
 //               vector.z = _Mesh->mBitangents[i].z;
 //               vertex.m_BiTangent = vector;
 //           }
 //           else
 //               vertex.m_TexCoords = glm::vec2(0.0f, 0.0f);

 //           vertices.push_back(vertex);
 //       }
 //       // now wak through each of the mesh's faces (a face is a mesh its triangle) and retrieve the corresponding vertex indices.
 //       for (unsigned int i = 0; i < _Mesh->mNumFaces; i++)
 //       {
 //           aiFace face = _Mesh->mFaces[i];
 //           // retrieve all indices of the face and store them in the indices vector
 //           for (unsigned int j = 0; j < face.mNumIndices; j++)
 //               indices.push_back(face.mIndices[j]);
 //       }
 //       // process materials
 //       aiMaterial* material = _Scene->mMaterials[_Mesh->mMaterialIndex];
 //       // we assume a convention for sampler names in the shaders. Each diffuse texture should be named
 //       // as 'texture_diffuseN' where N is a sequential number ranging from 1 to MAX_SAMPLER_NUMBER. 
 //       // Same applies to other texture as the following list summarizes:
 //       // diffuse: texture_diffuseN
 //       // specular: texture_specularN
 //       // normal: texture_normalN

 //       // 1. diffuse maps
 //       std::vector<Texture> diffuseMaps = LoadMaterialTextures(material, aiTextureType_DIFFUSE, "texture_diffuse");
 //       textures.insert(textures.end(), diffuseMaps.begin(), diffuseMaps.end());
 //       // 2. specular maps
 //       std::vector<Texture> specularMaps = LoadMaterialTextures(material, aiTextureType_SPECULAR, "texture_specular");
 //       textures.insert(textures.end(), specularMaps.begin(), specularMaps.end());
 //       // 3. normal maps
 //       std::vector<Texture> normalMaps = LoadMaterialTextures(material, aiTextureType_HEIGHT, "texture_normal");
 //       textures.insert(textures.end(), normalMaps.begin(), normalMaps.end());
 //       // 4. height maps
 //       std::vector<Texture> heightMaps = LoadMaterialTextures(material, aiTextureType_AMBIENT, "texture_height");
 //       textures.insert(textures.end(), heightMaps.begin(), heightMaps.end());

 //       // return a mesh object created from the extracted mesh data
 //       return Mesh3D(vertices, indices, textures, _PrimitiveType);
	//}

	//std::vector<Texture> Model::LoadMaterialTextures(aiMaterial* _Material, aiTextureType _Type, std::string _TypeName)
	//{
 //       std::vector<Texture> textures;
 //       for (unsigned int i = 0; i < _Material->GetTextureCount(_Type); i++)
 //       {
 //           aiString str;
 //           _Material->GetTexture(_Type, i, &str);
 //           // check if texture was loaded before and if so, continue to next iteration: skip loading a new texture
 //           bool skip = false;
 //           for (unsigned int j = 0; j < m_TexturesLoaded.size(); j++)
 //           {
 //               if (std::strcmp(m_TexturesLoaded[j].m_Path.data(), str.C_Str()) == 0)
 //               {
 //                   textures.push_back(m_TexturesLoaded[j]);
 //                   skip = true; // a texture with the same filepath has already been loaded, continue to next one. (optimization)
 //                   break;
 //               }
 //           }
 //           if (!skip)
 //           {   // if texture hasn't been loaded already, load it
 //               Texture texture;
 //               texture.m_Id = TexLoader::LoadTexture(str.C_Str(), this->m_Directory);
 //               texture.m_Type = _TypeName;
 //               texture.m_Path = str.C_Str();
 //               textures.push_back(texture);
 //               m_TexturesLoaded.push_back(texture);  // store it as texture loaded for entire model, to ensure we won't unnecesery load duplicate textures.
 //           }
 //       }
 //       return textures;
	//}

    std::vector<Texture> Model::LoadMaterialTextures(std::string& _Material, std::string _TypeName)
    {
        std::vector<Texture> textures;
        //for (unsigned int i = 0; i < _Material->GetTextureCount(_Type); i++)
        //{
            // check if texture was loaded before and if so, continue to next iteration: skip loading a new texture
            bool skip = false;
            for (unsigned int j = 0; j < m_TexturesLoaded.size(); j++)
            {
                if (std::strcmp(m_TexturesLoaded[j].m_Path.data(), _Material.c_str()) == 0)
                {
                    textures.push_back(m_TexturesLoaded[j]);
                    skip = true; // a texture with the same filepath has already been loaded, continue to next one. (optimization)
                    break;
                }
            }
            if (!skip)
            {   // if texture hasn't been loaded already, load it
                Texture texture;
                texture.m_Id = TexLoader::LoadTexture(_Material, this->m_Directory);
                texture.m_Type = _TypeName;
                texture.m_Path = _Material;
                textures.push_back(texture);
                m_TexturesLoaded.push_back(texture);  // store it as texture loaded for entire model, to ensure we won't unnecesery load duplicate textures.
            }
        //}
        return textures;
    }
}