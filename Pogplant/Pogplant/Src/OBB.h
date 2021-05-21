#pragma once
#include "Ray.h"

#include <glm.hpp>

namespace Pogplant
{
	struct OBB
	{
		glm::vec3 m_Min;
		glm::vec3 m_Max;
		glm::mat4 m_Model;

		bool RayObb(const Ray& _Ray);
	};
}