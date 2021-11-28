/******************************************************************************/
/*!
\file	Utility.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	Header defining the utility functions for the PhysicsDLC

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef PHYSUTILITY_H_
#define PHYSUTILITY_H_

namespace PhysicsDLC::Utility
{
	inline bool BarycentricCoordsLine(const glm::vec3& pt, const glm::vec3& a, const glm::vec3& b,
		float& u, float& v)
	{
		glm::vec3 delta = a - b;
		float lengthSqr = glm::dot(delta, delta);

		if (lengthSqr < std::numeric_limits<float>::epsilon())
			return false;

		u = glm::dot(pt - b, delta) / lengthSqr;
		v = 1 - u;

		return (u <= 1.f && u >= 0.f) && (v <= 1.f && v >= 0.f);
	}

	inline bool BarycentricCoordsTriangle(
		const glm::vec3& pt, const glm::vec3& a, const glm::vec3& b, const glm::vec3& c,
		float& u, float& v, float& w)
	{
		glm::vec3 v0 = pt - c;
		glm::vec3 v1 = a - c;
		glm::vec3 v2 = b - c;

		float E = glm::dot(v0, v1);
		float A = glm::dot(v1, v1);
		float F = glm::dot(v0, v2);
		float C = glm::dot(v1, v2);
		float D = glm::dot(v2, v2);

		float denom = A * D - C * C;

		if (std::abs(denom) <= std::numeric_limits<float>::epsilon())
		{
			return false;
		}

		u = ((E * D) - (F * C)) / denom;
		v = ((A * F) - (C * E)) / denom;
		w = 1.f - u - v;

		return (u <= 1.f && u >= 0.f) && (v <= 1.f && v >= 0.f) && (w <= 1.f && w >= 0.f);
	}
}

#endif // !PHYSUTILITY_H_
