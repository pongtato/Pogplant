#include "MeshBuilder.h"
#include "Mesh.h"
#include "MeshResource.h"
#include "MeshInstance.h"

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
    }

    void MeshBuilder::CleanUpMesh()
    {
        MeshResource::CleanUpResource();
    }

    void MeshBuilder::RebindQuad()
    {
        Mesh* mesh = MeshResource::m_MeshPool[MT::QUAD];

        /// Assert
        if (mesh == nullptr)
        {
            std::cout << " [ASSERT] Quad resource is null" << std::endl;
            return;
        }

        // Instanced data
        const size_t instDatSize = sizeof(InstanceData);
        const size_t totalSize = instDatSize * MeshInstance::GetInstanceCount();

        glBindVertexArray(mesh->m_VAO);
        glBindBuffer(GL_ARRAY_BUFFER, mesh->m_IBO);
        glBufferData(GL_ARRAY_BUFFER, totalSize, MeshResource::m_MeshInstances.data(), GL_DYNAMIC_DRAW);

        // m_Color
        glEnableVertexAttribArray(2);
        glVertexAttribPointer(2, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_Color));

        // Model
        glEnableVertexAttribArray(3);
        glVertexAttribPointer(3, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_Model));
        glEnableVertexAttribArray(4);
        glVertexAttribPointer(4, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)(sizeof(glm::vec4)));
        glEnableVertexAttribArray(5);
        glVertexAttribPointer(5, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)(2 * sizeof(glm::vec4)));
        glEnableVertexAttribArray(6);
        glVertexAttribPointer(6, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)(3 * sizeof(glm::vec4)));

        // UV m_Tiling
        glEnableVertexAttribArray(7);
        glVertexAttribPointer(7, 2, GL_FLOAT, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_Tiling));

        // UV Offset
        glEnableVertexAttribArray(8);
        glVertexAttribPointer(8, 2, GL_FLOAT, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_UVOffset));

        // Tex ID
        glEnableVertexAttribArray(9);
        glVertexAttribPointer(9, 1, GL_UNSIGNED_INT, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_TexID));

        // Ortho
        glEnableVertexAttribArray(10);
        glVertexAttribPointer(10, 1, GL_BOOL, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_Ortho));

        glVertexAttribDivisor(2, 1);
        glVertexAttribDivisor(3, 1);
        glVertexAttribDivisor(4, 1);
        glVertexAttribDivisor(5, 1);
        glVertexAttribDivisor(6, 1);
        glVertexAttribDivisor(7, 1);
        glVertexAttribDivisor(8, 1);
        glVertexAttribDivisor(9, 1);
        glVertexAttribDivisor(10, 1);

        // Unbind
        glBindVertexArray(0);
        glBindBuffer(0, mesh->m_IBO);
    }

    void MeshBuilder::GenerateQuad()
    {
        Mesh* mesh = MeshResource::m_MeshPool[MT::QUAD];

        /// Assert
        if (mesh == nullptr)
        {
            std::cout << " [ASSERT] Quad resource is null" << std::endl;
            return;
        }

        mesh->m_VertexData =
        {
            // Vertex , UV
            VTX_DATA{ glm::vec2{ 0.5f, 0.5f}, glm::vec2{1,1}},
            VTX_DATA{ glm::vec2{ 0.5f,-0.5f}, glm::vec2{1,0}},
            VTX_DATA{ glm::vec2{-0.5f,-0.5f}, glm::vec2{0,0}},
            VTX_DATA{ glm::vec2{-0.5f, 0.5f}, glm::vec2{0,1}},
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

        // Instanced data
        const size_t instDatSize = sizeof(InstanceData);

        glCreateBuffers(1, &mesh->m_IBO);
        glBufferData(GL_ARRAY_BUFFER, instDatSize, MeshResource::m_MeshInstances.data(), GL_DYNAMIC_DRAW);

        // m_Color
        glEnableVertexAttribArray(2);
        glVertexAttribPointer(2, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_Color));

        // Model
        glEnableVertexAttribArray(3);
        glVertexAttribPointer(3, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_Model));
        glEnableVertexAttribArray(4);
        glVertexAttribPointer(4, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)(sizeof(glm::vec4)));
        glEnableVertexAttribArray(5);
        glVertexAttribPointer(5, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)(2 * sizeof(glm::vec4)));
        glEnableVertexAttribArray(6);
        glVertexAttribPointer(6, 4, GL_FLOAT, GL_FALSE, instDatSize, (void*)(3 * sizeof(glm::vec4)));
        // UV m_Tiling
        glEnableVertexAttribArray(7);
        glVertexAttribPointer(7, 2, GL_FLOAT, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_Tiling));
        // UV Offset
        glEnableVertexAttribArray(8);
        glVertexAttribPointer(8, 2, GL_FLOAT, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_UVOffset));
        // Tex ID
        glEnableVertexAttribArray(9);
        glVertexAttribPointer(9, 1, GL_UNSIGNED_INT, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_TexID));
        // Ortho
        glEnableVertexAttribArray(10);
        glVertexAttribPointer(10, 1, GL_BOOL, GL_FALSE, instDatSize, (void*)offsetof(InstanceData, InstanceData::m_Ortho));

        glVertexAttribDivisor(2, 1);
        glVertexAttribDivisor(3, 1);
        glVertexAttribDivisor(4, 1);
        glVertexAttribDivisor(5, 1);
        glVertexAttribDivisor(6, 1);
        glVertexAttribDivisor(7, 1);
        glVertexAttribDivisor(8, 1);
        glVertexAttribDivisor(9, 1);
        glVertexAttribDivisor(10, 1);

        glBindVertexArray(0);
        glBindBuffer(0, mesh->m_IBO);
        glBindBuffer(0, mesh->m_EBO);
        glBindBuffer(0, mesh->m_VBO);

        // Update type ands size
        mesh->m_IndicesCount = static_cast<unsigned int>(mesh->m_Indices.size());
        mesh->m_PrimitiveType = GL_TRIANGLE_STRIP;
    }
}