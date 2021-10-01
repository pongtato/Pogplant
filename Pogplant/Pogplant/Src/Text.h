#pragma once
#include "FontResource.h"
#include <glm.hpp>

namespace Pogplant
{
	struct Text
	{
		// Transform in the future
		glm::vec3 m_Position;
		glm::vec3 m_Rotation;
		glm::vec3 m_Scale;
		std::string m_Text;
		FontResource::FONTS m_FontID;
	};
}