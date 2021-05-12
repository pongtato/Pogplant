#include "Mesh.h"
#include "MeshInstance.h"

#include <glew.h>


namespace Pogplant
{
	VTX_DATA::VTX_DATA(const glm::vec2& _Pos_Vtx, const glm::vec2& _Tex_Coords)
		: m_PosVtx{ _Pos_Vtx }
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

	void Mesh::Draw()
	{
		glBindVertexArray(this->m_VAO);
		glDrawElementsInstanced(this->m_PrimitiveType, m_IndicesCount, GL_UNSIGNED_INT, 0, static_cast<GLsizei>(MeshInstance::GetInstanceCount()));
		glBindVertexArray(0);
	}
}