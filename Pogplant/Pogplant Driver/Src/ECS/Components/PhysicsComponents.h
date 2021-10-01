#ifndef PHYSICSCOMPONENTS_H_
#define PHYSICSCOMPONENTS_H_

#include <PhysicsDLC.h>

namespace Components
{
	using Rigidbody = PhysicsDLC::Physics::Rigidbody;

	struct Collider
	{
		enum COLLISION_RULE
		{
			CR_COLLIDE = 0,/**<Just perform collision response*/
			CR_EVENT,/**<Just send an event*/
			CR_COLLIDE_EVENT,/**<Perform both response and sending event*/
			CR_IGNORE/**<Just ignore the collision*/
		};

		enum COLLIDER_TYPE
		{
			CT_BOX,
			CT_SPHERE,
			CT_GJK
		};

		bool isTrigger;

		int collisionLayer = 0;
		int colliderType = 0;
	};

	struct BoxCollider : public Collider
	{
		PhysicsDLC::Collision::Shapes::AABB aabb;
		glm::vec3 extends;
		glm::vec3 centre;
	};

	struct SphereCollider : public Collider
	{
		PhysicsDLC::Collision::Shapes::Sphere sphere;
		float radius;
		glm::vec3 centre;
	};
}

#endif // !PHYSICSCOMPONENTS_H_
