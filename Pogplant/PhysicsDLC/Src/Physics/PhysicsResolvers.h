#ifndef PHYSICSRESOLVERS_H_
#define PHYSICSRESOLVERS_H_

#include "..\Collision\Collision.h"

namespace PhysicsDLC
{
	namespace Physics
	{
		void test()
		{
			Collision::Shapes::AABB aabb;
			glm::vec3 test;
			float dt = 0.5f;
			Collision::DynamicAABBAABB(aabb, test, aabb, test, dt);
		}
	}
}

#endif // !PHYSICSRESOLVERS_H_
