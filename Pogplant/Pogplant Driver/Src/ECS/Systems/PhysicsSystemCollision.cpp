/******************************************************************************/
/*!
\file	PhysicsSystemCollision.cpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details

	This file contains the collision handling functions

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#include "PhysicsSystem.h"

#include <Pogplant.h>
#include <iostream>

void PhysicsSystem::HandleCollision(const entt::entity& c_1entity,
	const entt::entity& c_2entity,
	Components::Transform& c_1transform,
	Components::Transform& c_2transform,
	Components::Rigidbody& c_1rigidbody,
	Components::Rigidbody* c_2rigidbody,
	const Components::ColliderIdentifier& c_1colliderIdentifier,
	const Components::ColliderIdentifier& c_2colliderIdentifier,
	float c_dt)
{
	auto collisionRule = GetCollisionRule(c_1colliderIdentifier.collisionLayer, c_2colliderIdentifier.collisionLayer);

	if (collisionRule == Components::Collider::COLLISION_RULE::CR_IGNORE)
		return;

	switch (c_1colliderIdentifier.colliderType)
	{
	case Components::ColliderIdentifier::COLLIDER_TYPE::CT_BOX:
	{
		/***************************************************/
		switch (c_2colliderIdentifier.colliderType)
		{
		case Components::ColliderIdentifier::COLLIDER_TYPE::CT_BOX:
		{
			auto _1collider = m_registry->GetReg().try_get<Components::BoxCollider>(c_1entity);
			auto _2collider = m_registry->GetReg().try_get<Components::BoxCollider>(c_2entity);

			//auto& _2transform = collidableEntities.get<Components::Transform>(_2entity);
			if (PhysicsDLC::Collision::StaticAABBAABB(_1collider->aabb, _2collider->aabb))
			{
				PhysicsDLC::Physics::ResolveAABBAABBDynamic(c_1transform.m_position, c_1rigidbody, c_2rigidbody, _1collider->aabb, _2collider->aabb, 0.f, c_dt);
			}
			else
			{
				static PhysicsDLC::Collision::CollisionResults collisionResult;

				if (c_2rigidbody)
					collisionResult = PhysicsDLC::Collision::CRDynamicAABBAABB(_1collider->aabb, c_1rigidbody.velocity, _2collider->aabb, c_2rigidbody->velocity, c_dt);
				else
					collisionResult = PhysicsDLC::Collision::CRDynamicAABBAABB(_1collider->aabb, c_1rigidbody.velocity, _2collider->aabb, PhysicsDLC::Vector::Zero, c_dt);

				if (collisionResult)
				{
					//std::cout << "COLLIDE: " << c_dt << std::endl;
					PhysicsDLC::Physics::ResolveAABBAABBDynamic(c_1transform.m_position, c_1rigidbody, c_2rigidbody, _1collider->aabb, _2collider->aabb, collisionResult.collisionTime, c_dt);
				}
			}

			break;
		}
		case Components::ColliderIdentifier::COLLIDER_TYPE::CT_SPHERE:
		{
			break;
		}
		case Components::ColliderIdentifier::COLLIDER_TYPE::CT_HEIGHTMAP:
		{
			break;
		}
		default:
			std::cerr << "PhysicsSystem::HandleCollision, unhandled collider update check box 2" << std::endl;
			break;
		}
		/***************************************************/

		break;
	}
	case Components::ColliderIdentifier::COLLIDER_TYPE::CT_SPHERE:
	{
		/***************************************************/
		switch (c_2colliderIdentifier.colliderType)
		{
		case Components::ColliderIdentifier::COLLIDER_TYPE::CT_BOX:
		{
			break;
		}
		case Components::ColliderIdentifier::COLLIDER_TYPE::CT_SPHERE:
		{
			auto _1collider = m_registry->GetReg().try_get<Components::SphereCollider>(c_1entity);
			auto _2collider = m_registry->GetReg().try_get<Components::SphereCollider>(c_2entity);

			//auto& _2transform = collidableEntities.get<Components::Transform>(_2entity);
			if (PhysicsDLC::Collision::StaticSphereSphere(_1collider->sphere, _2collider->sphere))
			{
				PhysicsDLC::Collision::CollisionResults collisionResult;
				collisionResult.collisionNormal = glm::normalize(c_2transform.m_position - c_1transform.m_position);
				collisionResult.collisionTime = 0.f;

				PhysicsDLC::Physics::ResolveSphereSphereDynamic(
					c_1transform.m_position,
					c_1rigidbody,
					c_2rigidbody,
					_1collider->sphere,
					_2collider->sphere,
					collisionResult,
					c_dt);
			}
			else
			{
				static PhysicsDLC::Collision::CollisionResults collisionResult;

				if (c_2rigidbody)
					collisionResult = PhysicsDLC::Collision::CRDynamicSphereSphere(_1collider->sphere, c_1rigidbody.velocity, _2collider->sphere, c_2rigidbody->velocity, c_dt);
				else
					collisionResult = PhysicsDLC::Collision::CRDynamicSphereSphere(_1collider->sphere, c_1rigidbody.velocity, _2collider->sphere, PhysicsDLC::Vector::Zero, c_dt);

				if (collisionResult)
				{
					std::cout << "COLLIDE: " << c_dt << std::endl;
					PhysicsDLC::Physics::ResolveSphereSphereDynamic(
						c_1transform.m_position,
						c_1rigidbody,
						c_2rigidbody,
						_1collider->sphere,
						_2collider->sphere,
						collisionResult,
						c_dt);
				//	PhysicsDLC::Physics::ResolveAABBAABBDynamic(c_1transform.m_position, c_1rigidbody, c_2rigidbody, _1collider->aabb, _2collider->aabb, collisionResult.collisionTime, c_dt);
				}
			}

			break;
		}
		case Components::ColliderIdentifier::COLLIDER_TYPE::CT_HEIGHTMAP:
		{
			break;
		}
		default:
			std::cerr << "PhysicsSystem::HandleCollision, unhandled collider update check sphere 2" << std::endl;
			break;
		}
		/***************************************************/
		break;
	}
	default:
		std::cerr << "PhysicsSystem::HandleCollision, unhandled collider update check 1" << std::endl;
		break;
	}
}

/******************************************************************************/
/*!
\brief
	Draws all the colliders in the world
*/
/******************************************************************************/
void PhysicsSystem::DrawColliders()
{
	glm::vec3 camPos = PP::CameraResource::GetCamera("EDITOR")->GetPosition();

	auto boxColliders = m_registry->GetReg().view<Components::BoxCollider>();
	for (auto collidable : boxColliders)
	{
		auto& boxCollider = boxColliders.get<Components::BoxCollider>(collidable);
		PP::DebugDraw::DebugCube(boxCollider.aabb.m_min, boxCollider.aabb.m_max);
	}

	auto sphereColliders = m_registry->GetReg().view<Components::SphereCollider>();
	for (auto collidable : sphereColliders)
	{
		auto& sphereCollider = sphereColliders.get<Components::SphereCollider>(collidable);
		PP::DebugDraw::DebugSphere(sphereCollider.sphere.m_pos, (sphereCollider.sphere.m_pos - camPos), sphereCollider.sphere.m_radius);
	}

	// haha xd
	auto cameras = m_registry->GetReg().view<Components::Camera, Components::Transform>();
	const float aspect = static_cast<float>(PP::Window::m_Height) / PP::Window::m_Width;
	for (auto it : cameras)
	{
		auto& cam_trans = cameras.get<Components::Transform>(it);
		auto& cam = cameras.get<Components::Camera>(it);

		PP::DebugDraw::DebugFrustum
		(
			cam_trans.m_position,
			glm::radians(cam.m_Zoom * 0.5f),
			aspect,
			cam.m_Near,
			cam.m_Far,
			cam.m_Right,
			cam.m_Up,
			cam.m_Front
		);
	}
}

void PhysicsSystem::SetCollisionRule(int collisionLayer1, int collisionLayer2, Components::Collider::COLLISION_RULE collisionRule)
{
	if (collisionLayer1 > collisionLayer2)
		std::swap(collisionLayer1, collisionLayer2);

	m_collisionMatrix[std::make_pair(collisionLayer1, collisionLayer2)] = collisionRule;
}

Components::Collider::COLLISION_RULE PhysicsSystem::GetCollisionRule(int collisionLayer1, int collisionLayer2)
{
	if (collisionLayer1 > collisionLayer2)
		std::swap(collisionLayer1, collisionLayer2);

	auto itr = m_collisionMatrix.find(std::make_pair(collisionLayer1, collisionLayer2));

	if (itr == m_collisionMatrix.end())
		return Components::Collider::COLLISION_RULE::CR_COLLIDE;

	return static_cast<Components::Collider::COLLISION_RULE>((*itr).second);
}

decltype(auto) PhysicsSystem::GetTriggered(entt::entity c_triggerEntity, entt::entity c_triggeringEntity)
{
	auto objects = m_triggerList.equal_range(c_triggerEntity);

	for (auto it = objects.first; it != objects.second; ++it)
	{
		if ((*it).second == c_triggeringEntity)
		{
			return it;
		}
	}

	return objects.second;
}

void PhysicsSystem::SetTrigger(entt::entity c_triggerEntity, entt::entity c_triggeringEntity)
{
	//Call ontriggerenter function here
	std::stringstream ss;
	ss << "OnTriggerEnter: "
		<< (uint32_t)c_triggerEntity << " "
		<< (uint32_t)c_triggeringEntity;

	PP::Logger::Log(
		PP::LogEntry{ "PhysicsSystem::TriggerUpdate", PP::LogEntry::TYPE::DEBUG_TEXT, ss.str() }, true);

	m_triggerList.insert(std::make_pair(c_triggerEntity, c_triggeringEntity));
}

bool PhysicsSystem::SetUntrigger(entt::entity c_triggerEntity, entt::entity c_triggeringEntity)
{
	auto objects = m_triggerList.equal_range(c_triggerEntity);

	for (auto it = objects.first; it != objects.second; ++it)
	{
		if ((*it).second == c_triggeringEntity)
		{
			//Call ontriggerexit function here
			std::stringstream ss;
			ss << "OnTriggerExit: "
				<< (uint32_t)c_triggerEntity << " "
				<< (uint32_t)c_triggeringEntity;

			PP::Logger::Log(
				PP::LogEntry{ "PhysicsSystem::TriggerUpdate", PP::LogEntry::TYPE::DEBUG_TEXT, ss.str() }, true);

			m_triggerList.erase(it);
			return true;
		}
	}
	return false;
}
