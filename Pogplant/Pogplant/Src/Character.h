#pragma once

#include <glm.hpp>

namespace Pogplant
{
	struct Character
	{
		glm::vec2		m_TexCoords;
		glm::vec2		m_Size;
		glm::vec2		m_Offsets;
		float			m_Advance;
		unsigned int	m_ID;
		unsigned int	m_Index;
		unsigned int	m_Chnl;
		unsigned char	m_Char;
		//unsigned int  m_Page, texture atlas should just be 1 page
	};
}