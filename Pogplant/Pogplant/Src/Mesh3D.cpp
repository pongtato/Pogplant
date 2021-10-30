#include "Mesh3D.h"
#include "MeshBuilder.h"
#include "ShaderLinker.h"
#include "Logger.h"

#include <glew.h>
#include <iostream>

namespace Pogplant
{
	Mesh3D::Mesh3D(
        std::vector<Vertex> _Vertices,
        std::vector<uint> _Indices,
        std::vector<Texture> _Textures,
        std::vector<std::string> _SubMeshIDs,
        glm::vec3 _Translate,
        glm::vec4 _Rotate,
        glm::vec3 _Scale,
        std::string _Name,
        uint _PrimitiveType)
	{
        m_Instances = {};
		m_Vertices = _Vertices;
		m_Indices = _Indices;
		m_Textures = _Textures;
        m_SubMeshIDs = _SubMeshIDs;
        m_Translate = _Translate;
        m_Rotate = _Rotate;
        m_Scale = _Scale;
        m_Name = _Name;
        m_PrimitiveType = _PrimitiveType;

        GenerateMesh3D();
	}

	void Mesh3D::Draw(bool _BindTex) const
	{
        // bind appropriate textures
        int diffuseNr = 0;
        int specularNr = 0;
        int normalNr = 0;
        int emissiveNr = 0;
        int heightNr = 0;

        if (m_Textures.size() > 0 && _BindTex)
        {
            for (unsigned int i = 0; i < m_Textures.size(); i++)
            {
                //glActiveTexture(GL_TEXTURE0 + i); // active proper texture unit before binding
                // retrieve texture number (the N in diffuse_textureN)
                std::string number;
                std::string name = m_Textures[i].m_Type;
                if (name == "texture_diffuse")
                {
                    number = std::to_string(diffuseNr++);
                }
                else if (name == "texture_specular")
                {
                    number = std::to_string(specularNr++);
                }
                else if (name == "texture_normal")
                {
                    number = std::to_string(normalNr++);
                }
                else if (name == "texture_emissive")
                {
                    number = std::to_string(emissiveNr++);
                    ShaderLinker::SetUniform("material.emissive_count", emissiveNr);
                }
                else
                {
                    Logger::Log({ "PP::ERROR::MESH3D",LogEntry::LOGTYPE::ERROR , "Unsupported texture type" + name });
                }

                //std::cout << ("material." + name + "[" + number + "]") << std::endl;
                ShaderLinker::SetUniform(("material." + name + "[" + number + "]").c_str(), static_cast<float>(i));
                //glBindTexture(GL_TEXTURE_2D, m_Textures[i].m_Id);
                glBindTextureUnit(static_cast<GLint>(i), m_Textures[i].m_Id);
            }
        }

		glBindVertexArray(m_VAO);
		glDrawElements(m_PrimitiveType, static_cast<int>(m_Indices.size()), GL_UNSIGNED_INT, 0);
		glBindVertexArray(0);
        glActiveTexture(GL_TEXTURE0);
	}

    void Mesh3D::DrawInstanced(bool _BindTex) const
    {
        // bind appropriate textures
        int diffuseNr = 0;
        int specularNr = 0;
        int normalNr = 0;
        int emissiveNr = 0;

        if (m_Textures.size() > 0 && _BindTex)
        {
            for (unsigned int i = 0; i < m_Textures.size(); i++)
            {
                //glActiveTexture(GL_TEXTURE0 + i); // active proper texture unit before binding
                // retrieve texture number (the N in diffuse_textureN)
                std::string number;
                std::string name = m_Textures[i].m_Type;
                if (name == "texture_diffuse")
                {
                    number = std::to_string(diffuseNr++);
                }
                else if (name == "texture_specular")
                {
                    number = std::to_string(specularNr++);
                }
                else if (name == "texture_normal")
                {
                    number = std::to_string(normalNr++);
                }
                else if (name == "texture_emissive")
                {
                    number = std::to_string(emissiveNr++);
                    ShaderLinker::SetUniform("material.emissive_count", emissiveNr);
                }
                else
                {
                    Logger::Log({ "PP::ERROR::MESH3D",LogEntry::LOGTYPE::ERROR , "Unsupported texture type" + name });
                }

                //std::cout << ("material." + name + "[" + number + "]") << std::endl;
                ShaderLinker::SetUniform(("material." + name + "[" + number + "]").c_str(), static_cast<GLint>(i));
                //glBindTexture(GL_TEXTURE_2D, m_Textures[i].m_Id);
                glBindTextureUnit(static_cast<GLint>(i), m_Textures[i].m_Id);
            }
        }

        glBindVertexArray(m_VAO);
        glDrawElementsInstanced(m_PrimitiveType, static_cast<int>(m_Indices.size()), GL_UNSIGNED_INT, 0, static_cast<int>(m_Instances.size()));
        glBindVertexArray(0);
        glActiveTexture(GL_TEXTURE0);
    }

    void Mesh3D::RebindMesh3D()
    {
        glBindVertexArray(m_VAO);
        glBindBuffer(GL_ARRAY_BUFFER, m_IBO);
        glBufferData(GL_ARRAY_BUFFER, sizeof(InstanceData3D) * m_Instances.size(), m_Instances.data(), GL_DYNAMIC_DRAW);

        // m_Color
        glEnableVertexAttribArray(5);
        glVertexAttribPointer(5, 4, GL_FLOAT, GL_FALSE, sizeof(InstanceData3D), (void*)offsetof(InstanceData3D, InstanceData3D::m_Model));
        glVertexAttribDivisor(5, 1);

        // Model
        glEnableVertexAttribArray(6);
        glVertexAttribPointer(6, 4, GL_FLOAT, GL_FALSE, sizeof(InstanceData3D), (void*)(sizeof(glm::vec4)));
        glVertexAttribDivisor(6, 1);

        glEnableVertexAttribArray(7);
        glVertexAttribPointer(7, 4, GL_FLOAT, GL_FALSE, sizeof(InstanceData3D), (void*)(2 * sizeof(glm::vec4)));
        glVertexAttribDivisor(7, 1);

        glEnableVertexAttribArray(8);
        glVertexAttribPointer(8, 4, GL_FLOAT, GL_FALSE, sizeof(InstanceData3D), (void*)(3 * sizeof(glm::vec4)));
        glVertexAttribDivisor(8, 1);

        // Color tint
        glEnableVertexAttribArray(9);
        glVertexAttribPointer(9, 3, GL_FLOAT, GL_FALSE, sizeof(InstanceData3D), (void*)offsetof(InstanceData3D, InstanceData3D::m_Color));
        glVertexAttribDivisor(9, 1);

        // Affect by light?
        glEnableVertexAttribArray(10);
        glVertexAttribIPointer(10, 1, GL_INT, sizeof(InstanceData3D), (void*)offsetof(InstanceData3D, InstanceData3D::m_UseLight));
        glVertexAttribDivisor(10, 1);

        // Use texture or not
        glEnableVertexAttribArray(11);
        glVertexAttribIPointer(11, 1, GL_INT, sizeof(InstanceData3D), (void*)offsetof(InstanceData3D, InstanceData3D::m_UseTexture));
        glVertexAttribDivisor(11, 1);

        // Only to be drawn in editor
        glEnableVertexAttribArray(12);
        glVertexAttribIPointer(12, 1, GL_INT, sizeof(InstanceData3D), (void*)offsetof(InstanceData3D, InstanceData3D::m_EditorOnly));
        glVertexAttribDivisor(12, 1);

        // Unbind
        glBindVertexArray(0);
        glBindBuffer(GL_ARRAY_BUFFER, 0);
    }

    void Mesh3D::GenerateMesh3D()
    {
        glCreateVertexArrays(1, &m_VAO);
        glCreateBuffers(1, &m_VBO);
        glCreateBuffers(1, &m_IBO);
        glCreateBuffers(1, &m_EBO);

        glBindVertexArray(m_VAO);
        glBindBuffer(GL_ARRAY_BUFFER, m_VBO);
        glBufferData(GL_ARRAY_BUFFER, m_Vertices.size() * sizeof(Vertex), m_Vertices.data(), GL_STATIC_DRAW);

        //glBindBuffer(GL_ARRAY_BUFFER, m_IBO);
        //glBufferData(GL_ARRAY_BUFFER, m_Instances.size() * sizeof(InstanceData3D), m_Instances.data(), GL_DYNAMIC_DRAW);

        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, m_EBO);
        glBufferData(GL_ELEMENT_ARRAY_BUFFER, m_Indices.size() * sizeof(unsigned int), m_Indices.data(), GL_STATIC_DRAW);

        glEnableVertexAttribArray(0);
        glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)0);

        glEnableVertexAttribArray(1);
        glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, m_Normal));

        glEnableVertexAttribArray(2);
        glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, m_Tangent));

        glEnableVertexAttribArray(3);
        glVertexAttribPointer(3, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, m_BiTangent));

        glEnableVertexAttribArray(4);
        glVertexAttribPointer(4, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, m_TexCoords));

        glBindVertexArray(0);

        // Clear vertices & indices buffer after done copying to opengl
        m_Vertices.clear();
        m_Vertices.shrink_to_fit();
        //m_Indices.clear();
        //m_Indices.shrink_to_fit();
    }
}