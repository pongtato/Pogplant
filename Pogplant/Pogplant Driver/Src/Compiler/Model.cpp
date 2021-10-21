/*****************************************************************************/
/*!
\file	Model.cpp
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
    Model class that contains all data for geometry of models

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
            or disclosure of this file or its contents without the prior
            written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

#include "Model.h"
#include "Logger.h"

#include <limits>
#include <iostream>

Mesh3D::Mesh3D
(
    std::vector<Vertex> _Vertices,
    std::vector<uint> _Indices,
    std::vector<Texture> _Textures,
    std::vector<std::string> _SubMeshIDs,
    const glm::vec3& _Translate,
    const glm::vec4& _Rotate,
    const glm::vec3& _Scale,
    const std::string& _Name
)
{
    m_Vertices = _Vertices;
    m_Indices = _Indices;
    m_Textures = _Textures;
    m_SubMeshIDs = _SubMeshIDs;
    m_Translate = _Translate;
    m_Rotate = _Rotate;
    m_Scale = _Scale;
    m_Name = _Name;
}

Model::Model(std::string _Path)
{
    LoadModel(_Path);
}

void Model::LoadModel(std::string _Path)
{
    Assimp::Importer importer;
    const aiScene* scene = importer.ReadFile(_Path, aiProcess_Triangulate | aiProcess_GenSmoothNormals | aiProcess_FlipUVs | aiProcess_CalcTangentSpace);
    size_t found = _Path.find("Resources");
    size_t found1 = _Path.find_last_of("/\\") - found;
    std::string front = _Path.substr(found, found1);
    m_Directory = front;
    // check for errors
    if (!scene || scene->mFlags & AI_SCENE_FLAGS_INCOMPLETE || !scene->mRootNode) // if is Not Zero
    {
        Logger::Log({ "PP::ASSIMP",LogEntry::ERROR, importer.GetErrorString() });
        return;
    }

    ProcessNode(scene->mRootNode, scene, nullptr);
}

void Model::ProcessNode(aiNode* _Node, const aiScene* _Scene, Mesh3D* _Parent)
{
    Mesh3D* ref = _Parent;

    // process each mesh located at the current node
    for (unsigned int i = 0; i < _Node->mNumMeshes; i++)
    {
        // the node object only contains indices to index the actual objects in the scene. 
        // the scene contains all the data, node is just to keep stuff organized (like relations between nodes).
        aiMesh* mesh = _Scene->mMeshes[_Node->mMeshes[i]];
        ProcessMesh(mesh, _Scene, _Node, _Parent);
        ref = &m_Meshes[mesh->mName.C_Str()];
    }

    // after we've processed all of the meshes (if any) we then recursively process each of the children nodes
    for (unsigned int i = 0; i < _Node->mNumChildren; i++)
    {
        ProcessNode(_Node->mChildren[i], _Scene, ref);
    }
}

void Model::ProcessMesh(aiMesh* _Mesh, const aiScene* _Scene, aiNode* _Node, Mesh3D* _Parent)
{
    // data to fill
    std::vector<Vertex> vertices;
    std::vector<unsigned int> indices;
    std::vector<Texture> textures;

    // walk through each of the mesh's vertices
    for (unsigned int i = 0; i < _Mesh->mNumVertices; i++)
    {
        Vertex vertex;
        // we declare a placeholder vector since assimp uses its own vector class that doesn't directly
        // convert to glm's vec3 class so we transfer the data to this placeholder glm::vec3 first.
        glm::vec3 vector; 
        // positions
        vector.x = _Mesh->mVertices[i].x;
        vector.y = _Mesh->mVertices[i].y;
        vector.z = _Mesh->mVertices[i].z;

        vertex.m_Position = vector;

        m_Bounds.minX = std::min(m_Bounds.minX, vector.x);
        m_Bounds.minY = std::min(m_Bounds.minY, vector.y);
        m_Bounds.minZ = std::min(m_Bounds.minZ, vector.z);

        m_Bounds.maxX = std::max(m_Bounds.maxX, vector.x);
        m_Bounds.maxY = std::max(m_Bounds.maxY, vector.y);
        m_Bounds.maxZ = std::max(m_Bounds.maxZ, vector.z);

        // normals
        if (_Mesh->HasNormals())
        {
            vector.x = _Mesh->mNormals[i].x;
            vector.y = _Mesh->mNormals[i].y;
            vector.z = _Mesh->mNormals[i].z;
            vertex.m_Normal = vector;
        }
        // texture coordinates
        if (_Mesh->mTextureCoords[0]) // does the mesh contain texture coordinates?
        {
            glm::vec2 vec;
            // a vertex can contain up to 8 different texture coordinates. We thus make the assumption that we won't 
            // use models where a vertex can have multiple texture coordinates so we always take the first set (0).
            vec.x = _Mesh->mTextureCoords[0][i].x;
            vec.y = _Mesh->mTextureCoords[0][i].y;
            vertex.m_TexCoords = vec;
            // tangent
            vector.x = _Mesh->mTangents[i].x;
            vector.y = _Mesh->mTangents[i].y;
            vector.z = _Mesh->mTangents[i].z;
            vertex.m_Tangent = vector;
            // bitangent
            vector.x = _Mesh->mBitangents[i].x;
            vector.y = _Mesh->mBitangents[i].y;
            vector.z = _Mesh->mBitangents[i].z;
            vertex.m_BiTangent = vector;
        }
        else
            vertex.m_TexCoords = glm::vec2(0.0f, 0.0f);

        vertices.push_back(vertex);
    }
    // now wak through each of the mesh's faces (a face is a mesh its triangle) and retrieve the corresponding vertex indices.
    for (unsigned int i = 0; i < _Mesh->mNumFaces; i++)
    {
        aiFace face = _Mesh->mFaces[i];
        // retrieve all indices of the face and store them in the indices vector
        for (unsigned int j = 0; j < face.mNumIndices; j++)
            indices.push_back(face.mIndices[j]);
    }
    // process materials
    aiMaterial* material = _Scene->mMaterials[_Mesh->mMaterialIndex];
    // we assume a convention for sampler names in the shaders. Each diffuse texture should be named
    // as 'texture_diffuseN' where N is a sequential number ranging from 1 to MAX_SAMPLER_NUMBER. 
    // Same applies to other texture as the following list summarizes:
    // diffuse: texture_diffuseN
    // specular: texture_specularN
    // normal: texture_normalN

    // 1. diffuse maps
    std::vector<Texture> diffuseMaps = LoadMaterialTextures(material, aiTextureType_DIFFUSE, "texture_diffuse");
    textures.insert(textures.end(), diffuseMaps.begin(), diffuseMaps.end());
    // 2. specular maps
    std::vector<Texture> specularMaps = LoadMaterialTextures(material, aiTextureType_SPECULAR, "texture_specular");
    textures.insert(textures.end(), specularMaps.begin(), specularMaps.end());
    // 3. normal maps
    std::vector<Texture> normalMaps = LoadMaterialTextures(material, aiTextureType_HEIGHT, "texture_normal");
    textures.insert(textures.end(), normalMaps.begin(), normalMaps.end());
    // 4. height maps
    std::vector<Texture> heightMaps = LoadMaterialTextures(material, aiTextureType_AMBIENT, "texture_height");
    textures.insert(textures.end(), heightMaps.begin(), heightMaps.end());

    //textures.push_back({ "diffuse", "sample.dds"});

    // Find longest edge - General usage no ritter's
    float lenX = std::fabsf(m_Bounds.maxX) + std::fabsf(m_Bounds.minX);
    float lenY = std::fabsf(m_Bounds.maxY) + std::fabsf(m_Bounds.minY);
    float lenZ = std::fabsf(m_Bounds.maxZ) + std::fabsf(m_Bounds.minZ);
    m_Bounds.longest = std::max(lenX, lenY);
    m_Bounds.longest = std::max(m_Bounds.longest, lenZ);

    // Get the variables required for parenting/transformation
    aiVector3D pos = {};
    aiVector3D scale = {};
    aiVector3D rotAxis = {};
    ai_real rotAmnt = {};
    _Node->mTransformation.Decompose(scale, rotAxis, rotAmnt, pos);

    /// DEBUG INFO
    //std::cout << "TRANSFORM DATA" << std::endl;
    //std::cout << "T: " << pos.x << "|" << pos.y << "|" <<  pos.z << std::endl;
    //std::cout << "S: " << scale.x << "|" << scale.y << "|" <<scale.z << std::endl;
    //std::cout << "R: " << rotAxis.x << "|" << rotAxis.y << "|" << rotAxis.z << "|" << rotAmnt << std::endl;
    //std::cout << "Parent: " << _ParentName << std::endl;
    //std::cout << "Current: " << _Mesh->mName.C_Str() << std::endl;
    //std::cout << std::endl;

    // Update container
    m_Meshes[_Mesh->mName.C_Str()] = Mesh3D(vertices, indices, textures, std::vector<std::string>{}, { pos.x,pos.y,pos.z }, { rotAxis.x,rotAxis.y,rotAxis.z,rotAmnt }, { scale.x,scale.y,scale.z }, _Mesh->mName.C_Str());

    // Update parent
    if (_Parent)
    {
        _Parent->m_SubMeshIDs.push_back(_Mesh->mName.C_Str());
    }
}

std::vector<Texture> Model::LoadMaterialTextures(aiMaterial* _Material, aiTextureType _Type, std::string _TypeName)
{
    std::vector<Texture> textures;
    for (unsigned int i = 0; i < _Material->GetTextureCount(_Type); i++)
    {
        aiString str;
        _Material->GetTexture(_Type, i, &str);
        // check if texture was loaded before and if so, continue to next iteration: skip loading a new texture
        bool skip = false;
        for (unsigned int j = 0; j < m_TexturesLoaded.size(); j++)
        {
            if (std::strcmp(m_TexturesLoaded[j].m_Path.data(), str.C_Str()) == 0)
            {
                textures.push_back(m_TexturesLoaded[j]);
                skip = true; // a texture with the same filepath has already been loaded, continue to next one. (optimization)
                break;
            }
        }
        if (!skip)
        {   // if texture hasn't been loaded already, load it
            Texture texture;
            //texture.m_Id = Textures::LoadTexture(str.C_Str(), this->m_Directory);
            texture.m_Type = _TypeName;
            texture.m_Path = str.C_Str();
            textures.push_back(texture);
            m_TexturesLoaded.push_back(texture);  // store it as texture loaded for entire model, to ensure we won't unnecesery load duplicate textures.
        }
    }
    return textures;
}