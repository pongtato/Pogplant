#ifndef COLLISION_H_
#define COLLISION_H_

#include "Colliders.h"

namespace PhysicsDLC
{	
	namespace Collision
	{
		bool RaySphere(const Shapes::Ray& ray, const Shapes::Sphere& sphere, float& collisionTime);
		bool RaySphere(const vec3& rayPos, const vec3& rayDir, const vec3& spherePos, float sphereRadius, float& collisionTime);
		
		bool RayAABB(const Shapes::Ray& ray, const Shapes::AABB, float &collisionTime);
		bool RayAABB(const vec3& rayPos, const vec3& rayDir, const vec3& aabbMin, const vec3& aabbMax, float& collisionTime);
	}
}

#endif // !COLLISION_H_
