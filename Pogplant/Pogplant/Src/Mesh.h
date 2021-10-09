#pragma once

#include <glm.hpp>
#include <vector>

namespace Pogplant
{
	struct VTX_DATA
	{
		VTX_DATA(const glm::vec4& _Color, const glm::vec2& _Pos_Vtx, const glm::vec2& _Tex_Coords);

		glm::vec4 m_Color;
		glm::vec2 m_PosVtx;
		glm::vec2 m_TexCoords;
	};

	struct Mesh
	{
		Mesh();
		void Draw() const;
		void Draw(unsigned int _Texture) const;
		void DrawInstanced() const;
		float GetHeight(glm::vec3 _Position);

		std::vector<VTX_DATA> m_VertexData;
		std::vector<unsigned int> m_Indices;
		std::vector<unsigned char> m_Heightmap;
		unsigned int m_VAO;
		unsigned int m_VBO;
		unsigned int m_EBO;
		unsigned int m_IBO;
		unsigned int m_IndicesCount;
		unsigned int m_PrimitiveType;
		size_t m_HeightMapDim;

	private:
		float Barycentric(glm::vec3 _V1, glm::vec3 _V2, glm::vec3 _V3, glm::vec3 _Position) const;
	};
}