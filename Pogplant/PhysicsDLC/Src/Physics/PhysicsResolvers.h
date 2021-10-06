/******************************************************************************/
/*!
\file	PhysicsReolvers.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details

	Header file to define physics handling functions

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef PHYSICSRESOLVERS_H_
#define PHYSICSRESOLVERS_H_

#include "..\Collision\Collision.h"

namespace PhysicsDLC
{
	namespace Physics
	{
		struct Rigidbody
		{
			Rigidbody(
				float c_mass = 10.f,
				float c_drag = 0.f,
				bool c_isKinematic = false,
				bool c_useGravity = false);

			bool isKinematic = false;
			bool useGravity = false;
			float mass = 10.f;
			float drag = 0.f;

			vec3 velocity;
			vec3 acceleration;
			vec3 impulseAcceleration;
			vec3 newPosition;

			void AddForce(glm::vec3 direction);
			void AddImpulseForce(glm::vec3 direction);
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
