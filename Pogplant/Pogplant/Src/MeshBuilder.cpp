#include "MeshBuilder.h"
#include "Mesh.h"
#include "Mesh3D.h"
#include "MeshResource.h"
#include "MeshInstance.h"
#include "Logger.h"
#include "ModelResource.h"
#include "TextureLoader.h"
#include "TextureResource.h"
#include "Skybox.h"

#include <glew.h>
#include <iostream>


namespace Pogplant
{
    using MT = MeshResource::MESH_TYPE;

    void MeshBuilder::InitMesh(size_t _PoolSize)
    {
        glewInit();
        MeshResource::InitResource();
        MeshInstance::InitMeshInstance(_PoolSize);
        GenerateQuad();
        GenerateTextQuad();
        GenerateScreen();
        GenerateSkybox();
        GenerateLines();
        GenerateHeightmap();
        //GenerateDebugQuad();
        ModelResource::InitResource();
    }

    void MeshBuilder::CleanUpMesh()
    {
        ModelResource::CleanUpResource();
        MeshResource::CleanUpResource();
    }

    void MeshBuilder::RebindQuad()
    {
        Mesh* mesh = MeshResource::m_MeshPool[MT::QUAD];

        /// Assert
        assert(mesh != nullptr && "Quad resource is NULL");
        //if (mesh == nullptr)
        //{
        //    Logger::Log({ "PP::MESH",LogEntry::LOGTYPE::ERROR,"Quad resource is NULL" });
        //    return;
        //}

        // Instanced data
        const size_t instDatSize = sizeof(InstanceData);
        const size_t totalSize = instDatSize * MeshInstance::GetInstanceCount();

        glBindVertexArray(mesh->m_VAO);
        glBindBuffer(GL_ARRAY_BUFFER, mesh->m_IBO);
        glBufferData(GL_ARRAY_BUFFER, totalSize, MeshResource::m_MeshInstances.data(), GL_DYNAMIC_DRAW);

        // m_Color
        glEnableVertexAttribArray(2);
        glVertexAttribPointer(2, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_Color));
        glVertexAttribDivisor(2, 1);

        // Model
        glEnableVertexAttribArray(3);
        glVertexAttribPointer(3, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_Model));
        glVertexAttribDivisor(3, 1);

        glEnableVertexAttribArray(4);
        glVertexAttribPointer(4, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)(sizeof(glm::vec4)));
        glVertexAttribDivisor(4, 1);

        glEnableVertexAttribArray(5);
        glVertexAttribPointer(5, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)(2 * sizeof(glm::vec4)));
        glVertexAttribDivisor(5, 1);

        glEnableVertexAttribArray(6);
        glVertexAttribPointer(6, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)(3 * sizeof(glm::vec4)));
        glVertexAttribDivisor(6, 1);

        // Tex ID
        glEnableVertexAttribArray(7);
        glVertexAttribIPointer(7, 1, GL_INT, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_TexID));
        glVertexAttribDivisor(7, 1);

        // Ortho
        glEnableVertexAttribArray(8);
        glVertexAttribIPointer(8, 1, GL_UNSIGNED_INT, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_Ortho));
        glVertexAttribDivisor(8, 1);

        // Ortho
        glEnableVertexAttribArray(9);
        glVertexAttribIPointer(9, 1, GL_UNSIGNED_INT, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_GameOnly));
        glVertexAttribDivisor(9, 1);

        // Unbind
        glBindVertexArray(0);
        glBindBuffer(GL_ARRAY_BUFFER, 0);
    }

    void MeshBuilder::RebindTextQuad(float _X, float _Y, float _Width, float _Height, float _UVx, float _UVy)
    {  
        Mesh* mesh = MeshResource::m_MeshPool[MT::TEXT_QUAD];
        assert(mesh != nullptr && "Text quad resource is NULL");

        float vertices[6][4] =
        {
           { _X         , _Y + _Height  , 0.0f, 0.0f },
           { _X         , _Y            , 0.0f, _UVy },
           { _X + _Width, _Y            , _UVx, _UVy },

           { _X         , _Y + _Height  , 0.0f, 0.0f },
           { _X + _Width, _Y            , _UVx, _UVy },
           { _X + _Width, _Y + _Height  , _UVx, 0.0f }
        };

        /*if (mesh == nullptr)
        {
            Logger::Log({ "PP::MESH",LogEntry::LOGTYPE::ERROR,"Screen resource is NULL" });
            return;
        }*/

        glBindVertexArray(mesh->m_VAO);
        glBindBuffer(GL_ARRAY_BUFFER, mesh->m_VBO);
        glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(vertices), vertices);
        glBindBuffer(GL_ARRAY_BUFFER, 0);
        glBindVertexArray(0);
    }

    void MeshBuilder::RebindLines(const std::vector<glm::vec3>& _Points)
    {
        Mesh* mesh = MeshResource::m_MeshPool[MT::LINE];

        /// Assert
        assert(mesh != nullptr && "Quad resource is NULL");
        //if (mesh == nullptr)
        //{
        //    Logger::Log({ "PP::MESH",LogEntry::LOGTYPE::ERROR,"Quad resource is NULL" });
        //    return;
        //}

        size_t poxVtxSize = sizeof(glm::vec3) * _Points.size();

        // Rebind m_VAO
        glBindVertexArray(mesh->m_VAO);

        // Rebind m_VBO
        glBindBuffer(GL_ARRAY_BUFFER, mesh->m_VBO);
        glBufferData(GL_ARRAY_BUFFER, poxVtxSize, _Points.data(), GL_DYNAMIC_DRAW);

        // m_VAO rebind for position
        glEnableVertexArrayAttrib(mesh->m_VAO, 0);
        glVertexArrayVertexBuffer(mesh->m_VAO, 0, mesh->m_VBO, 0, sizeof(glm::vec3));
        glVertexArrayAttribFormat(mesh->m_VAO, 0, 3, GL_FLOAT, GL_FALSE, 0);
        glVertexArrayAttribBinding(mesh->m_VAO, 0, 0);

        // Unbind
        glBindBuffer(0, mesh->m_VBO);
        glBindVertexArray(0);

        mesh->m_IndicesCount = static_cast<unsigned int>(_Points.size());
        mesh->m_PrimitiveType = GL_LINES;
    }

    void MeshBuilder::RebindHeightMap(const std::vector<float>& _Vertex, int _Size)
    {
        (void)_Size;
        (void)_Vertex;
    }

    void MeshBuilder::GenerateQuad()
    {
        Mesh* mesh = MeshResource::m_MeshPool[MT::QUAD];

        /// Assert
        assert(mesh != nullptr && "Quad resource is NULL");
        //if (mesh == nullptr)
        //{
        //    Logger::Log({ "PP::MESH",LogEntry::LOGTYPE::ERROR,"Quad resource is NULL" });
        //    return;
        //}

        mesh->m_VertexData =
        {
            // Vertex , UV
            VTX_DATA{ glm::vec4{1}, glm::vec2{ 0.5f, 0.5f}, glm::vec2{1,1}},
            VTX_DATA{ glm::vec4{1}, glm::vec2{ 0.5f,-0.5f}, glm::vec2{1,0}},
            VTX_DATA{ glm::vec4{1}, glm::vec2{-0.5f,-0.5f}, glm::vec2{0,0}},
            VTX_DATA{ glm::vec4{1}, glm::vec2{-0.5f, 0.5f}, glm::vec2{0,1}},
        };

        mesh->m_Indices =
        {
            2,1,3,0
        };

        glCreateVertexArrays(1, &mesh->m_VAO);
        glCreateBuffers(1, &mesh->m_VBO);
        glCreateBuffers(1, &mesh->m_EBO);

        // m_VBO
        glNamedBufferStorage(mesh->m_VBO, sizeof(VTX_DATA) * mesh->m_VertexData.size(), mesh->m_VertexData.data(), GL_DYNAMIC_STORAGE_BIT);

        // Update m_VAO
        glVertexArrayAttribBinding(mesh->m_VAO, 0, 0);
        glVertexArrayAttribFormat(mesh->m_VAO, 0, 2, GL_FLOAT, GL_FALSE, offsetof(VTX_DATA, VTX_DATA::m_PosVtx));
        glEnableVertexArrayAttrib(mesh->m_VAO, 0);

        glVertexArrayAttribBinding(mesh->m_VAO, 1, 0);
        glVertexArrayAttribFormat(mesh->m_VAO, 1, 2, GL_FLOAT, GL_FALSE, offsetof(VTX_DATA, VTX_DATA::m_TexCoords));
        glEnableVertexArrayAttrib(mesh->m_VAO, 1);

        glVertexArrayVertexBuffer(mesh->m_VAO, 0, mesh->m_VBO, 0, sizeof(VTX_DATA));

        // Bind the m_Indices
        glNamedBufferStorage(mesh->m_EBO, sizeof(unsigned int) * mesh->m_Indices.size(), mesh->m_Indices.data(), GL_DYNAMIC_STORAGE_BIT);
        glVertexArrayElementBuffer(mesh->m_VAO, mesh->m_EBO);

        // Instanced data
        glCreateBuffers(1, &mesh->m_IBO);
        glBufferData(GL_ARRAY_BUFFER, sizeof(InstanceData), MeshResource::m_MeshInstances.data(), GL_DYNAMIC_DRAW);

        glBindVertexArray(0);
        glBindBuffer(GL_ARRAY_BUFFER, 0);

        // Update type ands size
        mesh->m_IndicesCount = static_cast<unsigned int>(mesh->m_Indices.size());
        mesh->m_PrimitiveType = GL_TRIANGLE_STRIP;
    }

    void MeshBuilder::GenerateTextQuad()
    {
        Mesh* mesh = MeshResource::m_MeshPool[MT::TEXT_QUAD];

        assert(mesh != nullptr && "Text quad resource is NULL");
        //if (mesh == nullptr)
        //{
        //    Logger::Log({ "PP::MESH",LogEntry::LOGTYPE::ERROR,"Screen resource is NULL" });
        //    return;
        //}

        glGenVertexArrays(1, &mesh->m_VAO);
        glGenBuffers(1, &mesh->m_VBO);
        glBindVertexArray(mesh->m_VAO);
        glBindBuffer(GL_ARRAY_BUFFER, mesh->m_VBO);
        glBufferData(GL_ARRAY_BUFFER, sizeof(float) * 24, NULL, GL_DYNAMIC_DRAW);
        glEnableVertexAttribArray(0);
        glVertexAttribPointer(0, 4, GL_FLOAT, GL_FALSE, 4 * sizeof(float), (void*)0);
        glBindBuffer(GL_ARRAY_BUFFER, 0);
        glBindVertexArray(0);
    }

    void MeshBuilder::GenerateScreen()
    {
        Mesh* mesh = MeshResource::m_MeshPool[MT::SCREEN];

        /// Assert
        assert(mesh != nullptr && "Screen quad resource is NULL");
        //if (mesh == nullptr)
        //{
        //    Logger::Log({ "PP::MESH",LogEntry::LOGTYPE::ERROR,"Screen resource is NULL" });
        //    return;
        //}

        mesh->m_VertexData =
        {
            // Vertex , UV
            VTX_DATA{ glm::vec4{1}, glm::vec2{ 1.0f, 1.0f}, glm::vec2{1,1}},
            VTX_DATA{ glm::vec4{1}, glm::vec2{ 1.0f,-1.0f}, glm::vec2{1,0}},
            VTX_DATA{ glm::vec4{1}, glm::vec2{-1.0f,-1.0f}, glm::vec2{0,0}},
            VTX_DATA{ glm::vec4{1}, glm::vec2{-1.0f, 1.0f}, glm::vec2{0,1}},
        };

        mesh->m_Indices =
        {
            0,1,3,2
        };

        glCreateVertexArrays(1, &mesh->m_VAO);
        glCreateBuffers(1, &mesh->m_VBO);
        glCreateBuffers(1, &mesh->m_EBO);

        // m_VBO
        glNamedBufferStorage(mesh->m_VBO, sizeof(VTX_DATA) * mesh->m_VertexData.size(), mesh->m_VertexData.data(), GL_DYNAMIC_STORAGE_BIT);

        // Update m_VAO
        glVertexArrayAttribBinding(mesh->m_VAO, 0, 0);
        glVertexArrayAttribFormat(mesh->m_VAO, 0, 2, GL_FLOAT, GL_FALSE, offsetof(VTX_DATA, VTX_DATA::m_PosVtx));
        glEnableVertexArrayAttrib(mesh->m_VAO, 0);

        glVertexArrayAttribBinding(mesh->m_VAO, 1, 0);
        glVertexArrayAttribFormat(mesh->m_VAO, 1, 2, GL_FLOAT, GL_FALSE, offsetof(VTX_DATA, VTX_DATA::m_TexCoords));
        glEnableVertexArrayAttrib(mesh->m_VAO, 1);

        glVertexArrayVertexBuffer(mesh->m_VAO, 0, mesh->m_VBO, 0, sizeof(VTX_DATA));

        // Bind the m_Indices
        glNamedBufferStorage(mesh->m_EBO, sizeof(unsigned int) * mesh->m_Indices.size(), mesh->m_Indices.data(), GL_DYNAMIC_STORAGE_BIT);
        glVertexArrayElementBuffer(mesh->m_VAO, mesh->m_EBO);

        // Update type ands size
        mesh->m_IndicesCount = static_cast<unsigned int>(mesh->m_Indices.size());
        mesh->m_PrimitiveType = GL_TRIANGLE_STRIP;
    }

    void MeshBuilder::GenerateSkybox()
    {
        float vertices[] = 
        {
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f,  1.0f
        };

        glGenVertexArrays(1, &Skybox::m_VAO);
        glGenBuffers(1, &Skybox::m_VBO);
        glBindVertexArray(Skybox::m_VAO);
        glBindBuffer(GL_ARRAY_BUFFER, Skybox::m_VBO);
        glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), &vertices, GL_STATIC_DRAW);
        glEnableVertexAttribArray(0);
        glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);

        Skybox::m_IndicesCount = 36;
        Skybox::m_PrimitiveType = GL_TRIANGLES;

        glBindBuffer(GL_ARRAY_BUFFER, 0);
        glBindVertexArray(0);
    }

    void MeshBuilder::GenerateLines()
    {
        Mesh* mesh = MeshResource::m_MeshPool[MT::LINE];

        assert(mesh != nullptr && "Line resource is NULL");

        std::vector<glm::vec3> temp;
        temp.resize(10);
        size_t poxVtxSize = sizeof(glm::vec3) * temp.size();

        // Create m_VAO
        glCreateVertexArrays(1, &mesh->m_VAO);

        // Create m_VBO
        glCreateBuffers(1, &mesh->m_VBO);
        glBindBuffer(GL_ARRAY_BUFFER, mesh->m_VBO);
        glBufferData(GL_ARRAY_BUFFER, poxVtxSize, temp.data(), GL_DYNAMIC_DRAW);

        // m_VAO setup for position
        glEnableVertexArrayAttrib(mesh->m_VAO, 0);
        glVertexArrayVertexBuffer(mesh->m_VAO, 0, mesh->m_VBO, 0, sizeof(glm::vec3));
        glVertexArrayAttribFormat(mesh->m_VAO, 0, 3, GL_FLOAT, GL_FALSE, 0);
        glVertexArrayAttribBinding(mesh->m_VAO, 0, 0);

        // Unbind
        glBindBuffer(0, mesh->m_VBO);
        glBindVertexArray(0);

        mesh->m_IndicesCount = static_cast<unsigned int>(temp.size());
        mesh->m_PrimitiveType = GL_LINES;
    }

    void MeshBuilder::GenerateHeightmap()
    {
        // Get mesh
        Mesh* mesh = MeshResource::m_MeshPool[MT::HEIGHTMAP];

        assert(mesh != nullptr && "Heightmap resource is NULL");

        //if (mesh == nullptr)
        //{
        //    Logger::Log({ "PP::MESH",LogEntry::LOGTYPE::ERROR,"Heightmap resource is NULL" });
        //    return;
        //}

        // Get heightmap
        if (!TexLoader::LoadHeightMap("Heightmap.raw", "Resources/Textures/Heightmap", mesh->m_Heightmap, mesh->m_HeightMapDim))
        {
            Logger::Log({ "PP::MESH",LogEntry::LOGTYPE::ERROR,"Bad heightmap" });
            return;
        }

        // Construct
        struct TerrainVertex
        {
            glm::vec3 position;
            glm::vec3 normal;
            glm::vec3 color;
            glm::vec2 uv;
            glm::vec3 tangent;
            glm::vec3 bitangent;
        };

        std::vector<TerrainVertex> terrainVertex;
        TerrainVertex v;
        for (unsigned z = 0; z < mesh->m_HeightMapDim; ++z)
        {
            for (unsigned x = 0; x < mesh->m_HeightMapDim; ++x)
            {
                float scaledHeight = (float)mesh->m_Heightmap[z * mesh->m_HeightMapDim + x] / 256.0f;
                v.position = { static_cast<float>(x) / mesh->m_HeightMapDim - 0.5f, scaledHeight, static_cast<float>(z) / mesh->m_HeightMapDim - 0.5f };
                v.color = { 1.0f, 1.0f, 1.0f }; //for rendering height map without texture

				glm::vec3 origin = { x,scaledHeight, z };
                // Normal calc
                float hL = TexLoader::GetHeight(x - 1, z    , mesh->m_HeightMapDim, mesh->m_Heightmap);
                float hR = TexLoader::GetHeight(x + 1, z    , mesh->m_HeightMapDim, mesh->m_Heightmap);
                float hD = TexLoader::GetHeight(x    , z - 1, mesh->m_HeightMapDim, mesh->m_Heightmap);
                float hU = TexLoader::GetHeight(x    , z + 1, mesh->m_HeightMapDim, mesh->m_Heightmap);
                v.normal = glm::normalize(glm::vec3{ hL - hR, 2.0f, hD - hU });
                v.bitangent = glm::vec3{ 2.0f, hR - hL ,0.0f };
                v.tangent = glm::vec3(0.0f, hU - hD, 2.0f);
                v.uv = { (float)x / mesh->m_HeightMapDim, 1.f - (float)z / mesh->m_HeightMapDim };
                terrainVertex.push_back(v);
            }
        }
        for (size_t z = 0; z < mesh->m_HeightMapDim - 1; ++z)
        {
            for (size_t x = 0; x < mesh->m_HeightMapDim - 1; ++x)
            {
                mesh->m_Indices.push_back(static_cast<unsigned int>(mesh->m_HeightMapDim * z + x));      //Tri 1
                mesh->m_Indices.push_back(static_cast<unsigned int>(mesh->m_HeightMapDim * (z + 1) + x));
                mesh->m_Indices.push_back(static_cast<unsigned int>(mesh->m_HeightMapDim * z + x + 1));
                mesh->m_Indices.push_back(static_cast<unsigned int>(mesh->m_HeightMapDim * (z + 1) + x + 1)); //Tri 2
                mesh->m_Indices.push_back(static_cast<unsigned int>(mesh->m_HeightMapDim * z + x + 1));
                mesh->m_Indices.push_back(static_cast<unsigned int>(mesh->m_HeightMapDim * (z + 1) + x));
            }
        }

        glCreateVertexArrays(1, &mesh->m_VAO);
        glCreateBuffers(1, &mesh->m_VBO);
        glCreateBuffers(1, &mesh->m_IBO);

        // m_VBO
        glNamedBufferStorage(mesh->m_VBO, sizeof(TerrainVertex)* terrainVertex.size(), terrainVertex.data(), GL_DYNAMIC_STORAGE_BIT);

        // Update m_VAO
        glVertexArrayAttribBinding(mesh->m_VAO, 0, 0);
        glVertexArrayAttribFormat(mesh->m_VAO, 0, 3, GL_FLOAT, GL_FALSE, offsetof(TerrainVertex, TerrainVertex::position));
        glEnableVertexArrayAttrib(mesh->m_VAO, 0);

        glVertexArrayAttribBinding(mesh->m_VAO, 1, 0);
        glVertexArrayAttribFormat(mesh->m_VAO, 1, 3, GL_FLOAT, GL_FALSE, offsetof(TerrainVertex, TerrainVertex::normal));
        glEnableVertexArrayAttrib(mesh->m_VAO, 1);

        glVertexArrayAttribBinding(mesh->m_VAO, 2, 0);
        glVertexArrayAttribFormat(mesh->m_VAO, 2, 3, GL_FLOAT, GL_FALSE, offsetof(TerrainVertex, TerrainVertex::color));
        glEnableVertexArrayAttrib(mesh->m_VAO, 2);

        glVertexArrayAttribBinding(mesh->m_VAO, 3, 0);
        glVertexArrayAttribFormat(mesh->m_VAO, 3, 2, GL_FLOAT, GL_FALSE, offsetof(TerrainVertex, TerrainVertex::uv));
        glEnableVertexArrayAttrib(mesh->m_VAO, 3);

        glVertexArrayAttribBinding(mesh->m_VAO, 4, 0);
        glVertexArrayAttribFormat(mesh->m_VAO, 4, 3, GL_FLOAT, GL_FALSE, offsetof(TerrainVertex, TerrainVertex::tangent));
        glEnableVertexArrayAttrib(mesh->m_VAO, 4);

        glVertexArrayAttribBinding(mesh->m_VAO, 5, 0);
        glVertexArrayAttribFormat(mesh->m_VAO, 5, 3, GL_FLOAT, GL_FALSE, offsetof(TerrainVertex, TerrainVertex::bitangent));
        glEnableVertexArrayAttrib(mesh->m_VAO, 5);

        glVertexArrayVertexBuffer(mesh->m_VAO, 0, mesh->m_VBO, 0, sizeof(TerrainVertex));

        // Bind the m_Indices
        glNamedBufferStorage(mesh->m_IBO, sizeof(unsigned int)* mesh->m_Indices.size(), mesh->m_Indices.data(), GL_DYNAMIC_STORAGE_BIT);
        glVertexArrayElementBuffer(mesh->m_VAO, mesh->m_IBO);

        // Update type ands size
        mesh->m_IndicesCount = static_cast<unsigned int>(mesh->m_Indices.size());
        mesh->m_PrimitiveType = GL_TRIANGLES;
    }

  //  void MeshBuilder::GenerateDebugQuad()
  //  {
  //      // Get mesh
  //      Mesh* mesh = MeshResource::m_MeshPool[MT::DEBUG_QUAD];
  //      if (mesh == nullptr)
  //      {
  //          Logger::Log({ "PP::MESH",LogEntry::ERROR,"Debug quad resource is NULL" });
  //          return;
  //      }

  //      // Get heightmap
  //      std::vector<unsigned char> heightmap;
  //      size_t dim = 0;
  //      if (!TexLoader::LoadHeightMap("Heightmap.raw", "Resources/Textures/Heightmap", heightmap, dim))
  //      {
  //          Logger::Log({ "PP::MESH",LogEntry::ERROR,"Bad heightmap" });
  //          return;
  //      }

  //      // Construct
  //      struct TerrainVertex
  //      {
  //          glm::vec3 position;
  //          glm::vec3 normal;
  //          glm::vec3 color;
  //          glm::vec2 uv;
  //          glm::vec3 tangent;
  //          glm::vec3 bitangent;
  //      };

  //      // positions
  //      glm::vec3 pos1(-1.0f, 1.0f, 0.0f);
  //      glm::vec3 pos2(-1.0f, -1.0f, 0.0f);
  //      glm::vec3 pos3(1.0f, -1.0f, 0.0f);
  //      glm::vec3 pos4(1.0f, 1.0f, 0.0f);
  //      // texture coordinates
  //      glm::vec2 uv1(0.0f, 1.0f);
  //      glm::vec2 uv2(0.0f, 0.0f);
  //      glm::vec2 uv3(1.0f, 0.0f);
  //      glm::vec2 uv4(1.0f, 1.0f);
  //      // normal vector
  //      glm::vec3 nm(0.0f, 0.0f, 1.0f);

  //      // calculate tangent/bitangent vectors of both triangles
  //      glm::vec3 tangent1, bitangent1;
  //      glm::vec3 tangent2, bitangent2;
  //      // triangle 1
  //      // ----------
  //      glm::vec3 edge1 = pos2 - pos1;
  //      glm::vec3 edge2 = pos3 - pos1;
  //      glm::vec2 deltaUV1 = uv2 - uv1;
  //      glm::vec2 deltaUV2 = uv3 - uv1;

  //      float f = 1.0f / (deltaUV1.x * deltaUV2.y - deltaUV2.x * deltaUV1.y);

  //      tangent1.x = f * (deltaUV2.y * edge1.x - deltaUV1.y * edge2.x);
  //      tangent1.y = f * (deltaUV2.y * edge1.y - deltaUV1.y * edge2.y);
  //      tangent1.z = f * (deltaUV2.y * edge1.z - deltaUV1.y * edge2.z);
  //      tangent1 = glm::normalize(tangent1);

  //      bitangent1.x = f * (-deltaUV2.x * edge1.x + deltaUV1.x * edge2.x);
  //      bitangent1.y = f * (-deltaUV2.x * edge1.y + deltaUV1.x * edge2.y);
  //      bitangent1.z = f * (-deltaUV2.x * edge1.z + deltaUV1.x * edge2.z);
  //      bitangent1 = glm::normalize(bitangent1);

  //      // triangle 2
  //      // ----------
  //      edge1 = pos3 - pos1;
  //      edge2 = pos4 - pos1;
  //      deltaUV1 = uv3 - uv1;
  //      deltaUV2 = uv4 - uv1;

  //      f = 1.0f / (deltaUV1.x * deltaUV2.y - deltaUV2.x * deltaUV1.y);

  //      tangent2.x = f * (deltaUV2.y * edge1.x - deltaUV1.y * edge2.x);
  //      tangent2.y = f * (deltaUV2.y * edge1.y - deltaUV1.y * edge2.y);
  //      tangent2.z = f * (deltaUV2.y * edge1.z - deltaUV1.y * edge2.z);
  //      tangent2 = glm::normalize(tangent2);


  //      bitangent2.x = f * (-deltaUV2.x * edge1.x + deltaUV1.x * edge2.x);
  //      bitangent2.y = f * (-deltaUV2.x * edge1.y + deltaUV1.x * edge2.y);
  //      bitangent2.z = f * (-deltaUV2.x * edge1.z + deltaUV1.x * edge2.z);
  //      bitangent2 = glm::normalize(bitangent2);

		//std::vector<TerrainVertex> terrainVertex =
		//{
		//	{pos1,nm,{1,1,1},uv1,tangent1,bitangent1},
		//    {pos2,nm,{1,1,1},uv2,tangent1,bitangent1},
		//    {pos3,nm,{1,1,1},uv3,tangent1,bitangent1},

		//    {pos1,nm,{1,1,1},uv1,tangent2,bitangent2},
		//    {pos3,nm,{1,1,1},uv3,tangent2,bitangent2},
		//    {pos4,nm,{1,1,1},uv4,tangent2,bitangent2},
		//};

  //      mesh->m_Indices =
  //      {
  //          0,1,2,3,4,5
  //      };

  //      glCreateVertexArrays(1, &mesh->m_VAO);
  //      glCreateBuffers(1, &mesh->m_VBO);
  //      glCreateBuffers(1, &mesh->m_IBO);

  //      // m_VBO
  //      glNamedBufferStorage(mesh->m_VBO, sizeof(TerrainVertex) * terrainVertex.size(), terrainVertex.data(), GL_DYNAMIC_STORAGE_BIT);

  //      // Update m_VAO
  //      glVertexArrayAttribBinding(mesh->m_VAO, 0, 0);
  //      glVertexArrayAttribFormat(mesh->m_VAO, 0, 3, GL_FLOAT, GL_FALSE, offsetof(TerrainVertex, TerrainVertex::position));
  //      glEnableVertexArrayAttrib(mesh->m_VAO, 0);

  //      glVertexArrayAttribBinding(mesh->m_VAO, 1, 0);
  //      glVertexArrayAttribFormat(mesh->m_VAO, 1, 3, GL_FLOAT, GL_FALSE, offsetof(TerrainVertex, TerrainVertex::normal));
  //      glEnableVertexArrayAttrib(mesh->m_VAO, 1);

  //      glVertexArrayAttribBinding(mesh->m_VAO, 2, 0);
  //      glVertexArrayAttribFormat(mesh->m_VAO, 2, 3, GL_FLOAT, GL_FALSE, offsetof(TerrainVertex, TerrainVertex::color));
  //      glEnableVertexArrayAttrib(mesh->m_VAO, 2);

  //      glVertexArrayAttribBinding(mesh->m_VAO, 3, 0);
  //      glVertexArrayAttribFormat(mesh->m_VAO, 3, 2, GL_FLOAT, GL_FALSE, offsetof(TerrainVertex, TerrainVertex::uv));
  //      glEnableVertexArrayAttrib(mesh->m_VAO, 3);

  //      glVertexArrayAttribBinding(mesh->m_VAO, 4, 0);
  //      glVertexArrayAttribFormat(mesh->m_VAO, 4, 3, GL_FLOAT, GL_FALSE, offsetof(TerrainVertex, TerrainVertex::tangent));
  //      glEnableVertexArrayAttrib(mesh->m_VAO, 4);

  //      glVertexArrayAttribBinding(mesh->m_VAO, 5, 0);
  //      glVertexArrayAttribFormat(mesh->m_VAO, 5, 3, GL_FLOAT, GL_FALSE, offsetof(TerrainVertex, TerrainVertex::bitangent));
  //      glEnableVertexArrayAttrib(mesh->m_VAO, 5);

  //      glVertexArrayVertexBuffer(mesh->m_VAO, 0, mesh->m_VBO, 0, sizeof(TerrainVertex));

  //      // Bind the m_Indices
  //      glNamedBufferStorage(mesh->m_IBO, sizeof(unsigned int) * mesh->m_Indices.size(), mesh->m_Indices.data(), GL_DYNAMIC_STORAGE_BIT);
  //      glVertexArrayElementBuffer(mesh->m_VAO, mesh->m_IBO);

  //      // Update type ands size
  //      mesh->m_IndicesCount = static_cast<unsigned int>(mesh->m_Indices.size());
  //      mesh->m_PrimitiveType = GL_TRIANGLES;
  //  }
}