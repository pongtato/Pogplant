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

		bool RaySphere(const vec3& rayStart, const vec3& rayDir, const vec3& spherePos, float sphereRadius, float& collisionTime)
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

			vec3 v0 = rayStart - spherePos;

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

		bool RayAABB(const Shapes::Ray& ray, const Shapes::AABB aabb, float& collisionTime)
		{
			return RayAABB(ray.m_start, ray.m_dir, aabb.m_min, aabb.m_max, collisionTime);
		}

		bool RayAABB(const vec3& rayStart, const vec3& rayDir, const vec3& aabbMin, const vec3& aabbMax, float& collisionTime)
		{
			float tMin = 0.f;
			float tMax = std::numeric_limits<float>::max();

			for (size_t i = 0; i < 3; ++i)
			{
				//If Ray is parallel
				if (fabs(rayDir[i]) < std::numeric_limits<float>::epsilon())
				{
					if (rayStart[i] < aabbMin[i] || rayStart[i] > aabbMax[i])
						return false;
				}
				else
				{
					float t1 = (aabbMin[i] - rayStart[i]) / rayDir[i];
					float t2 = (aabbMax[i] - rayStart[i]) / rayDir[i];

					if (t1 > t2)
						std::swap(t1, t2);

					if (t1 > tMin)
						tMin = t1;

					if (t2 < tMax)
						tMax = t2;

					if (tMin > tMax)
						return false;

					collisionTime = tMin;
				}
			}

			return (collisionTime >= 0.f);
		}

		bool StaticAABBAABB(const Shapes::AABB& aabb1, const Shapes::AABB& aabb2)
		{
			return (aabb1.m_min.x <= aabb2.m_max.x && aabb2.m_min.x <= aabb1.m_max.x
				&& aabb1.m_max.y >= aabb2.m_min.y && aabb2.m_max.y >= aabb1.m_min.y
				&& aabb1.m_max.z >= aabb2.m_min.z && aabb2.m_max.z >= aabb1.m_min.z);
		}

		bool DynamicAABBAABB(const Shapes::AABB& aabb1, const vec3& vel1, const Shapes::AABB& aabb2, const vec3& vel2, dtType deltaTime)
		{
			//calculate relative velocity
			vec3 Vrel = vel2 - vel1;
			
			//set tFirst and tLast values to default
			float tFirst = 0.f;
			float tLast = static_cast<float>(deltaTime);

			for (size_t i = 0; i < 3; i++)
			{
				if (aabb1.m_max[i] > aabb2.m_min[i] && aabb2.m_max[i] > aabb1.m_min[i])
					tFirst = std::max(0.f, tFirst);
				else
				{
					if (Vrel[i] > 0)
					{
						if (aabb1.m_max[i] < aabb2.m_min[i])
							return -1.f;

						if (aabb1.m_min[i] > aabb2.m_max[i])
							tFirst = std::max((aabb1.m_min[i] - aabb2.m_max[i]) / Vrel[i], tFirst);

						if (aabb1.m_max[i] > aabb2.m_min[i])
							tLast = std::min((aabb1.m_max[i] - aabb2.m_min[i]) / Vrel[i], tLast);
					}
					else if (Vrel[i] < 0)
					{
						if (aabb1.m_min[i] > aabb2.m_max[i])
							return -1.f;

						if (aabb1.m_max[i] < aabb2.m_min[i])
							tFirst = std::max((aabb1.m_max[i] - aabb2.m_min[i]) / Vrel[i], tFirst);
						if (aabb1.m_min[i] < aabb2.m_max[i])
							tLast = std::min((aabb1.m_min[i] - aabb2.m_max[i]) / Vrel[i], tLast);
					}
					else
					{
						if (aabb2.m_max[i] < aabb1.m_min[i])
							return -1.f;
						else if (aabb2.m_min[i] > aabb1.m_max[i])
							return -1.f;
					}
				}

				if (tFirst > tLast)
					return -1.f;
			}

			/*if (aabb1.m_min.x < aabb2.m_max.x && aabb2.m_min.x < aabb1.m_max.x)
				tFirst = 0.f;
			else
			{
				if (Vrel.x > 0)
				{
					if (aabb1.m_max.x < aabb2.m_min.x)
						return -1.f;

					if (aabb1.m_min.x > aabb2.m_max.x)
						tFirst = std::max((aabb1.m_min.x - aabb2.m_max.x) / Vrel.x, tFirst);

					if (aabb1.m_max.x > aabb2.m_min.x)
						tLast = std::min((aabb1.m_max.x - aabb2.m_min.x) / Vrel.x, tLast);
				}
				else if (Vrel.x < 0)
				{
					if (aabb1.m_min.x > aabb2.m_max.x)
						return -1.f;

					if (aabb1.m_max.x < aabb2.m_min.x)
						tFirst = std::max((aabb1.m_max.x - aabb2.m_min.x) / Vrel.x, tFirst);
					if (aabb1.m_min.x < aabb2.m_max.x)
						tLast = std::min((aabb1.m_min.x - aabb2.m_max.x) / Vrel.x, tLast);
				}
				else
				{
					if (aabb2.m_max.x < aabb1.m_min.x)
						return false;
					else if (aabb2.m_min.x > aabb1.m_max.x)
						return false;
				}
			}


			if (aabb1.m_max.y > aabb2.m_min.y && aabb2.m_max.y > aabb1.m_min.y)
				tFirst = std::max(0.f, tFirst);
			else
			{
				if (Vrel.y > 0)
				{
					if (aabb1.m_max.y < aabb2.m_min.y)
						return -1.f;

					if (aabb1.m_min.y > aabb2.m_max.y)
						tFirst = std::max((aabb1.m_min.y - aabb2.m_max.y) / Vrel.y, tFirst);

					if (aabb1.m_max.y > aabb2.m_min.y)
						tLast = std::min((aabb1.m_max.y - aabb2.m_min.y) / Vrel.y, tLast);
				}
				else if (Vrel.y < 0)
				{
					if (aabb1.m_min.y > aabb2.m_max.y)
						return -1.f;

					if (aabb1.m_max.y < aabb2.m_min.y)
						tFirst = std::max((aabb1.m_max.y - aabb2.m_min.y) / Vrel.y, tFirst);
					if (aabb1.m_min.y < aabb2.m_max.y)
						tLast = std::min((aabb1.m_min.y - aabb2.m_max.y) / Vrel.y, tLast);
				}
				else
				{
					if (aabb2.m_max.y < aabb1.m_min.y)
						return false;
					else if (aabb2.m_min.y > aabb1.m_max.y)
						return false;
				}
			}//*/

			//Objects collide
			return tFirst;
		}
	}
}