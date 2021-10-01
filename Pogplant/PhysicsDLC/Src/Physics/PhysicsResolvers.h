#ifndef PHYSICSRESOLVERS_H_
#define PHYSICSRESOLVERS_H_

#include "..\Collision\Collision.h"

namespace PhysicsDLC
{
	namespace Physics
	{
		struct Rigidbody
		{
			bool isKinematic;
			bool useGravity;
			float mass = 1.f;

			vec3 velocity;
			vec3 acceleration;
			vec3 newPosition;

			void AddForce(glm::vec3 direction);
		};

		void ResolveAABBDynamic(
			vec3& _1pos,
			Rigidbody& _1rigidbody,
			Rigidbody* _2rigidbody,
			Collision::Shapes::AABB& _1aabb,
			Collision::Shapes::AABB& _2aabb,
			float collisionTime,
			float dt);
	}
}

#endif // !PHYSICSRESOLVERS_H_
