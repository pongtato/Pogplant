#include "Skybox.h"

#include <glew.h>

namespace Pogplant
{
	unsigned int Skybox::m_VAO;
	unsigned int Skybox::m_VBO;
	unsigned int Skybox::m_IndicesCount;
	unsigned int Skybox::m_PrimitiveType;

	void Skybox::Draw(unsigned int _CubeTexture)
	{
		glBindVertexArray(m_VAO);
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_CUBE_MAP, _CubeTexture);
		glDrawArrays(m_PrimitiveType, 0, m_IndicesCount);
		glBindVertexArray(0);
	}
}
