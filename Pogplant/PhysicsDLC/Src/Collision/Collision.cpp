#include "Collision.h"
#include <gtx/norm.hpp>

namespace PhysicsDLC
{
	namespace Collision
	{
		bool RaySphere(const Shapes::Ray& ray, const Shapes::Sphere& sphere, float& collisionTime)
		{
			return RaySphere(ray.m_start, ray.m_dir, sphere.m_pos, sphere.m_radius, collisionTime);
		}

		bool RaySphere(const vec3& rayPos, const vec3& rayDir, const vec3& spherePos, float sphereRadius, float& collisionTime)
		{
			/*vec3 v0 = rayPos - spherePos;

			float a = glm::length2(rayDir);
			float b = 2.f * glm::dot(rayDir, v0);
			float m = glm::length2(v0);

			float c = m - sphereRadius * sphereRadius;

			if (c < 0.f)//Ray in sphere
			{
				collisionTime = 0.f;
				return true;
			}
			else if (b > 0.f)
			{
				return false;
			}

			float discriminant = b * b - 4.f * a * c;

			if (discriminant >= 0.f)
			{
				collisionTime = (-b - sqrtf(discriminant)) / (2.f * a);
				return true;
			}

			return false;//*/

			vec3 v0 = rayPos - spherePos;

			float rayProjection = glm::dot(rayDir, v0);

			float rayPosProj = glm::length2(v0) - sphereRadius * sphereRadius;

			if (rayPosProj < 0.f)//Ray in sphere
			{
				collisionTime = 0.f;
				return true;
			}
			else if (rayProjection > 0.f)
			{
				return false;
			}

			float discriminant = rayProjection * rayProjection - rayPosProj;

			if (discriminant >= 0.f)
			{
				collisionTime = -rayProjection - sqrtf(discriminant);
				return true;
			}

			return false;
		}
	}
}