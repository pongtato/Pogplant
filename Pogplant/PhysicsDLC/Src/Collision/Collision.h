#ifndef COLLISION_H_
#define COLLISION_H_

#include "Colliders.h"

namespace PhysicsDLC
{	
	namespace Collision
	{
		bool RaySphere(const Shapes::Ray& ray, const Shapes::Sphere& sphere, float& collisionTime);
		bool RaySphere(const vec3& rayStart, const vec3& rayDir, const vec3& spherePos, float sphereRadius, float& collisionTime);
		
		bool RayAABB(const Shapes::Ray& ray, const Shapes::AABB, float &collisionTime);
		bool RayAABB(const vec3& rayStart, const vec3& rayDir, const vec3& aabbMin, const vec3& aabbMax, float& collisionTime);

		bool StaticAABBAABB(const Shapes::AABB& aabb1, const Shapes::AABB& aabb2);
		bool DynamicAABBAABB(const Shapes::AABB& aabb1, const vec3& vel1, const Shapes::AABB& aabb2, const vec3& vel2, dtType deltaTime);
	}
}

#endif // !COLLISION_H_
