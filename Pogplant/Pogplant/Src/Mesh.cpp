#include "Mesh.h"
#include "MeshInstance.h"

#include <glew.h>

namespace Pogplant
{
	VTX_DATA::VTX_DATA(const glm::vec4& _Color, const glm::vec2& _Pos_Vtx, const glm::vec2& _Tex_Coords)
		: m_Color{ _Color }
		, m_PosVtx{ _Pos_Vtx }
		, m_TexCoords{ _Tex_Coords }
	{
	}

	Mesh::Mesh()
		: m_VAO{ 0 }
		, m_VBO{ 0 }
		, m_EBO{ 0 }
		, m_IBO{ 0 }
		, m_IndicesCount{ 0 }
		, m_PrimitiveType{ 0 }
	{
	}

	void Mesh::Draw(unsigned int _Texture) const
	{
		glBindTextureUnit(0, _Texture);
		glBindVertexArray(this->m_VAO);
		glDrawElements(this->m_PrimitiveType, m_IndicesCount, GL_UNSIGNED_INT, 0);
		glBindVertexArray(0);
	}

	void Mesh::DrawInstanced() const
	{
		glBindVertexArray(this->m_VAO);
		// Bind all textures in the pool here
		glDrawElementsInstanced(this->m_PrimitiveType, m_IndicesCount, GL_UNSIGNED_INT, 0, static_cast<GLsizei>(MeshInstance::GetInstanceCount()));
		glBindVertexArray(0);
	}
}