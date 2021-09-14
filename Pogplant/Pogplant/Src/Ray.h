#pragma once
#include <glm.hpp>

namespace Pogplant
{
	struct RayConfig
	{
		glm::vec2 m_VP_Min;
		glm::vec2 m_VP_Max;
		glm::vec2 m_VP_Size;
		glm::vec2 m_CursorPos;
	};

	struct Ray
	{
		RayConfig m_RayConfig;
		glm::vec3 m_Origin;
		glm::vec3 m_Direction;

		bool CollideSphere(glm::vec3 _SpherePos, float _SphereRad, float& _Time) const;
	};
}