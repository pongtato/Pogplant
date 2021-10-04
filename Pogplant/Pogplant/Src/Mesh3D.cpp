#include "Mesh3D.h"
#include "MeshBuilder.h"
#include "ShaderLinker.h"

#include <glew.h>
#include <iostream>

namespace Pogplant
{
	Mesh3D::Mesh3D(std::vector<Vertex> _Vertices, std::vector<uint> _Indices, std::vector<Texture> _Textures, uint _PrimitiveType)
	{
		m_Vertices = _Vertices;
		m_Indices = _Indices;
		m_Textures = _Textures;
        m_PrimitiveType = _PrimitiveType;

        GenerateMesh3D();
	}

	void Mesh3D::Draw() const
	{
        // bind appropriate textures
        unsigned int diffuseNr = 0;
        unsigned int specularNr = 0;
        unsigned int normalNr = 0;
        unsigned int heightNr = 0;

        if (m_Textures.size() == 0)
        {
            ShaderLinker::SetUniform("noTex", 1);
        }
        else
        {
            ShaderLinker::SetUniform("noTex", 0);
            for (unsigned int i = 0; i < m_Textures.size(); i++)
            {
                //glActiveTexture(GL_TEXTURE0 + i); // active proper texture unit before binding
                // retrieve texture number (the N in diffuse_textureN)
                std::string number;
                std::string name = m_Textures[i].m_Type;
                if (name == "texture_diffuse")
                    number = std::to_string(diffuseNr++);
                else if (name == "texture_specular")
                    number = std::to_string(specularNr++); // transfer unsigned int to stream
                else if (name == "texture_normal")
                    number = std::to_string(normalNr++); // transfer unsigned int to stream
                else if (name == "texture_height")
                    number = std::to_string(heightNr++); // transfer unsigned int to stream

                //std::cout << ("material." + name + "[" + number + "]") << std::endl;
                ShaderLinker::SetUniform(("material." + name + "[" + number + "]").c_str(), static_cast<float>(i));
                //glBindTexture(GL_TEXTURE_2D, m_Textures[i].m_Id);
                glBindTextureUnit(static_cast<GLint>(i), m_Textures[i].m_Id);
            }
        }

		glBindVertexArray(m_VAO);
		glDrawElements(m_PrimitiveType, static_cast<int>(m_Indices.size()), GL_UNSIGNED_INT, 0);
		glBindVertexArray(0);

        // always good practice to set everything back to defaults once configured.
        glActiveTexture(GL_TEXTURE0);
	}

    void Mesh3D::GenerateMesh3D()
    {
        glCreateVertexArrays(1, &m_VAO);
        glCreateBuffers(1, &m_VBO);
        glCreateBuffers(1, &m_EBO);

        glBindVertexArray(m_VAO);
        glBindBuffer(GL_ARRAY_BUFFER, m_VBO);
        glBufferData(GL_ARRAY_BUFFER, m_Vertices.size() * sizeof(Vertex), &m_Vertices[0], GL_STATIC_DRAW);

        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, m_EBO);
        glBufferData(GL_ELEMENT_ARRAY_BUFFER, m_Indices.size() * sizeof(unsigned int), &m_Indices[0], GL_STATIC_DRAW);

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