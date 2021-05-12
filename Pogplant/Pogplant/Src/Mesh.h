#pragma once

#include <glm.hpp>
#include <vector>

namespace Pogplant
{
	struct VTX_DATA
	{
		VTX_DATA(const glm::vec2& _Pos_Vtx, const glm::vec2& _Tex_Coords);

		glm::vec2 m_PosVtx;
		glm::vec2 m_TexCoords;
	};

	struct Mesh
	{
		Mesh();
		void Draw();

		std::vector<VTX_DATA> m_VertexData;
		std::vector<unsigned int> m_Indices;
		unsigned int m_VAO;
		unsigned int m_VBO;
		unsigned int m_EBO;
		unsigned int m_IBO;
		unsigned int m_IndicesCount;
		unsigned int m_PrimitiveType;
	};
}