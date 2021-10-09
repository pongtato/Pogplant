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

	void Mesh::Draw() const
	{
		glBindVertexArray(this->m_VAO);
		glDrawElements(this->m_PrimitiveType, m_IndicesCount, GL_UNSIGNED_INT, 0);
		glBindVertexArray(0);
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

	float Mesh::GetHeight(glm::vec3 _Position)
	{
		/// You need to map the position to -0.5f to 0.5f, / by model component's scale

		if (m_Heightmap.size() == 0)
		{
			return 0;
		}

		if (_Position.x < -0.5f || _Position.x > 0.5f || _Position.z < -0.5f || _Position.z > 0.5f)
		{
			return 0;
		}

		float gridSquareSize = 1.f / m_HeightMapDim;

		size_t gridX = static_cast<size_t>((_Position.x + 0.5f) / gridSquareSize);
		size_t gridZ = static_cast<size_t>((_Position.z + 0.5f) / gridSquareSize);

		float xCoord = fmod((_Position.x + 0.5f), gridSquareSize) / gridSquareSize;
		float zCoord = fmod((_Position.z + 0.5f), gridSquareSize) / gridSquareSize;

		float HeightmapReturn = 0.f;

		size_t var1 = xCoord <= (1 - zCoord) ? 0 : 1;
		HeightmapReturn = Barycentric
		(
			glm::vec3(var1, m_Heightmap[gridZ * m_HeightMapDim + gridX + var1], 0),
			glm::vec3(1, m_Heightmap[(gridZ + var1) * m_HeightMapDim + gridX + 1], var1),
			glm::vec3(0, m_Heightmap[(gridZ + 1) * m_HeightMapDim + gridX], 1),
			glm::vec3(xCoord, 0, zCoord)
		);

		return HeightmapReturn / m_HeightMapDim;
	}

	float Mesh::Barycentric(glm::vec3 _V1, glm::vec3 _V2, glm::vec3 _V3, glm::vec3 _Position) const
	{
		float det = (_V2.z - _V3.z) * (_V1.x - _V3.x) + (_V3.x - _V2.x) * (_V1.z - _V3.z);

		float l1 = ((_V2.z - _V3.z) * (_Position.x - _V3.x) + (_V3.x - _V2.x) * (_Position.z - _V3.z)) / det;

		float l2 = ((_V3.z - _V1.z) * (_Position.x - _V3.x) + (_V1.x - _V3.x) * (_Position.z - _V3.z)) / det;

		float l3 = 1.0f - l1 - l2;

		return l1 * _V1.y + l2 * _V2.y + l3 * _V3.y;
	}
}