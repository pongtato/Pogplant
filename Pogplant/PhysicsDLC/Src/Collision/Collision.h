#ifndef COLLISION_H_
#define COLLISION_H_

#include "Colliders.h"

namespace PhysicsDLC
{	
	namespace Collision
	{
		struct CollisionResults
		{
			bool collided;
			float collisionTime;

			/**> Collision normal relative to shape 1*/
			vec3 collisionNormal;

			operator bool()
			{
				return collided;
			}
		};

		bool RaySphere(const Shapes::Ray& ray, const Shapes::Sphere& sphere, float& collisionTime);
		bool RaySphere(const vec3& rayStart, const vec3& rayDir, const vec3& spherePos, float sphereRadius, float& collisionTime);
		
		bool RayAABB(const Shapes::Ray& ray, const Shapes::AABB, float &collisionTime);
		bool RayAABB(const vec3& rayStart, const vec3& rayDir, const vec3& aabbMin, const vec3& aabbMax, float& collisionTime);

		bool StaticAABBAABB(const Shapes::AABB& aabb1, const Shapes::AABB& aabb2);
		float DynamicAABBAABB(const Shapes::AABB& aabb1, const vec3& vel1, const Shapes::AABB& aabb2, const vec3& vel2, dtType deltaTime);
		CollisionResults&& CRDynamicAABBAABB(const Shapes::AABB& aabb1, const vec3& vel1, const Shapes::AABB& aabb2, const vec3& vel2, dtType deltaTime);
		
		bool PointAABB(const vec3& point, const Shapes::AABB& aabb);
	}
}

#endif // !COLLISION_H_
