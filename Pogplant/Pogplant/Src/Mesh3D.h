#pragma once

#include <glm.hpp>
#include <string>
#include <vector>

namespace Pogplant
{
	typedef unsigned int uint;

	struct Vertex
	{
		glm::vec3 m_Position;
		glm::vec3 m_Normal;
		glm::vec3 m_Tangent;
		glm::vec3 m_BiTangent;
		glm::vec2 m_TexCoords;
	};

	struct Texture
	{
		unsigned int m_Id;
		std::string m_Type;
		std::string m_Path;
	};

	struct Mesh3D
	{
		Mesh3D() = default;
		// 0x0004 = GL_TRIANGLES
		Mesh3D
		(
			std::vector<Vertex> _Vertices,
			std::vector<uint> _Indices,
			std::vector<Texture> _Textures,
			std::vector<std::string> _SubMeshIDs,
			glm::vec3 _Translate,
			glm::vec4 _Rotate,
			glm::vec3 _Scale,
			std::string _Name,
			uint _PrimitiveType = 0x0004
		);

		void Draw(bool _BindTex) const;

		std::vector<Vertex> m_Vertices;
		std::vector<uint> m_Indices;
		std::vector<Texture> m_Textures;
		std::vector<std::string> m_SubMeshIDs;
		glm::vec3 m_Translate;
		glm::vec4 m_Rotate;
		glm::vec3 m_Scale;
		std::string m_Name;
		uint m_VAO;
		uint m_VBO;
		uint m_EBO;
		uint m_PrimitiveType;

	private:
		void GenerateMesh3D();
	};
}