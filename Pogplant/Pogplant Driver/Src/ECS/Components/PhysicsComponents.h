#ifndef PHYSICSCOMPONENTS_H_
#define PHYSICSCOMPONENTS_H_

#include <PhysicsDLC.h>
#include <rttr/registration>

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

		bool isTrigger = false;

		int collisionLayer = 0;
		int colliderType = 0;
	};

	struct BoxCollider : public Collider
	{
		BoxCollider() = default;
		inline BoxCollider(glm::vec3 c_extends, glm::vec3 c_centre)
			: extends{ c_extends }, centre{ c_centre }
		{}

		glm::vec3 extends;
		glm::vec3 centre;

		PhysicsDLC::Collision::Shapes::AABB aabb;
	};

	struct SphereCollider : public Collider
	{
		SphereCollider() = default;
		inline SphereCollider(glm::vec3 c_centre, float c_radius)
			: centre{ c_centre }, radius{ c_radius }
		{}

		float radius;
		glm::vec3 centre;

		PhysicsDLC::Collision::Shapes::Sphere sphere;
	};
}

#endif // !PHYSICSCOMPONENTS_H_
