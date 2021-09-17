#include "Ray.h"

bool Pogplant::Ray::CollideSphere(glm::vec3 _SpherePos, float _SphereRad, float& _Time) const
{
	const float a = glm::dot(m_Direction, m_Direction);
	const glm::vec3 dir = m_Origin - _SpherePos;
	const float b = 2.0f * glm::dot(dir, m_Direction);
	const float c = glm::dot(dir,dir) - (_SphereRad * _SphereRad);
	const float disc = b * b - 4.0f * a * c;

	// Trivial rejection
	if (disc < 0.0f)
	{
		_Time = 0.0f;
		return false;
	}

	float t1 = (-b - sqrtf(disc)) / 2 * a;
	float t2 = (-b + sqrtf(disc)) / 2 * a;

	// Both miss
	if (t1 < 0.0f && t2 < 0.0f)
	{
		return false;
	}

	// Else shortest time is the one we want
	_Time = glm::min(t1, t2);

	// Clamp
	_Time = glm::max(_Time, 0.0f);

	return true;
}
