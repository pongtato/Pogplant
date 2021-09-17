#pragma once

namespace Pogplant
{
	struct Skybox
	{
		static unsigned int m_VAO;
		static unsigned int m_VBO;
		static unsigned int m_IndicesCount;
		static unsigned int m_PrimitiveType;
		static void Draw(unsigned int _CubeTexture);
	};
}