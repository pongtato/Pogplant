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

	//struct VTX_DATA_3D
	//{
	//	VTX_DATA_3D() {};
	//	VTX_DATA_3D(glm::vec3 _Position, glm::vec3 _Normal, glm::vec3 _Color, glm::vec2 _Tex_Coords)
	//		: m_Position{ _Position }
	//		, m_Normal{ _Normal }
	//		, m_Color{ _Color }
	//		, m_TexCoords{ _Tex_Coords }
	//	{}

	//	glm::vec3 m_Position;
	//	glm::vec3 m_Normal;
	//	glm::vec3 m_Color;
	//	glm::vec2 m_TexCoords;
	//};

	struct Mesh
	{
		Mesh();
		void Draw() const;
		void Draw(unsigned int _Texture) const;
		void DrawInstanced() const;

		std::vector<VTX_DATA> m_VertexData;
		//std::vector<VTX_DATA_3D> m_VertexData3D;
		std::vector<unsigned int> m_Indices;
		unsigned int m_VAO;
		unsigned int m_VBO;
		unsigned int m_EBO;
		unsigned int m_IBO;
		unsigned int m_IndicesCount;
		unsigned int m_PrimitiveType;
	};
}