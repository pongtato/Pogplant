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
				PhysicsDLC::Physics::ResolveAABBAABBDynamic(c_1transform.GetGlobalPosition(), c_1rigidbody, c_2rigidbody, _1collider->aabb, _2collider->aabb, 0.f, c_dt);
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
					PhysicsDLC::Physics::ResolveAABBAABBDynamic(c_1transform.GetGlobalPosition(), c_1rigidbody, c_2rigidbody, _1collider->aabb, _2collider->aabb, collisionResult.collisionTime, c_dt);
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

				auto trans1Pos = c_1transform.GetGlobalPosition();

				collisionResult.collisionNormal = glm::normalize(c_2transform.GetGlobalPosition() - trans1Pos);
				collisionResult.collisionTime = 0.f;

				PhysicsDLC::Physics::ResolveSphereSphereDynamic(
					trans1Pos,
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
						c_1transform.GetGlobalPosition(),
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
	glm::vec3 camPos = PP::CameraResource::GetCamera("EDITOR")->m_Position;
	//glm::vec3 camPos = PP::CameraResource::m_QuatCam.m_Position;

	auto boxColliders = m_registry->view<Components::BoxCollider>();
	for (auto collidable : boxColliders)
	{
		auto& boxCollider = boxColliders.get<Components::BoxCollider>(collidable);
		PP::DebugDraw::DebugCube(boxCollider.aabb.m_min, boxCollider.aabb.m_max);
	}

	auto sphereColliders = m_registry->view<Components::SphereCollider>();
	for (auto collidable : sphereColliders)
	{
		auto& sphereCollider = sphereColliders.get<Components::SphereCollider>(collidable);
		PP::DebugDraw::DebugSphere(sphereCollider.sphere.m_pos, (sphereCollider.sphere.m_pos - camPos), sphereCollider.sphere.m_radius);
	}

	auto obbBoxColliders = m_registry->view<Components::OBBBoxCollider>();
	for (auto collidable : obbBoxColliders)
	{
		auto& boxCollider = obbBoxColliders.get<Components::OBBBoxCollider>(collidable);

		const glm::vec3 v0 = boxCollider.obb.m_pos;
		const glm::vec3 v1 = boxCollider.obb.m_pos + boxCollider.obb.m_extendX;
		const glm::vec3 v2 = boxCollider.obb.m_pos + boxCollider.obb.m_extendZ;
		const glm::vec3 v3 = boxCollider.obb.m_pos + boxCollider.obb.m_extendX + boxCollider.obb.m_extendZ;

		const glm::vec3 v4 = boxCollider.obb.m_pos + boxCollider.obb.m_extendY;
		const glm::vec3 v5 = boxCollider.obb.m_pos + boxCollider.obb.m_extendX + boxCollider.obb.m_extendY;
		const glm::vec3 v6 = boxCollider.obb.m_pos + boxCollider.obb.m_extendZ + boxCollider.obb.m_extendY;
		const glm::vec3 v7 = boxCollider.obb.m_pos + boxCollider.obb.m_extendX + boxCollider.obb.m_extendZ + boxCollider.obb.m_extendY;


		//const glm::vec3 v5 = boxCollider.obb.m_pos + boxCollider.obb.m_extendY;
		
		//Bottom
		PP::DebugDraw::DebugLine(v0, v1);
		PP::DebugDraw::DebugLine(v0, v2);
		PP::DebugDraw::DebugLine(v2, v3);
		PP::DebugDraw::DebugLine(v1, v3);

		//Top
		PP::DebugDraw::DebugLine(v4, v5);
		PP::DebugDraw::DebugLine(v4, v6);
		PP::DebugDraw::DebugLine(v6, v7);
		PP::DebugDraw::DebugLine(v5, v7);

		//Sides
		PP::DebugDraw::DebugLine(v0, v4);
		PP::DebugDraw::DebugLine(v1, v5);
		PP::DebugDraw::DebugLine(v2, v6);
		PP::DebugDraw::DebugLine(v3, v7);
	}

	//Draw dynamic aabbtree
	auto boxes = m_broadphase.GetAABBTreeBoxes();
	for (auto& box : boxes)
	{
		PP::DebugDraw::DebugCube(box.m_min, box.m_max);
	}

	// haha xd
	auto cameras = m_registry->view<Components::Camera, Components::Transform>();
	const float aspect = static_cast<float>(PP::Window::m_Height) / PP::Window::m_Width;
	for (auto it : cameras)
	{
		auto& cam_trans = cameras.get<Components::Transform>(it);
		auto& cam = cameras.get<Components::Camera>(it);

		PP::DebugDraw::DebugFrustum
		(
			cam_trans.m_position,
			glm::radians(cam.m_Fov * 0.5f),
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

	m_collisionMatrix[collisionLayer1][collisionLayer2] = collisionRule;
}

Components::Collider::COLLISION_RULE PhysicsSystem::GetCollisionRule(int collisionLayer1, int collisionLayer2)
{
	if (collisionLayer1 > collisionLayer2)
		std::swap(collisionLayer1, collisionLayer2);

	auto itr1 = m_collisionMatrix.find(collisionLayer1);

	if (itr1 == m_collisionMatrix.end())
		return Components::Collider::COLLISION_RULE::CR_COLLIDE;

	auto itr2 = itr1->second.find(collisionLayer2);
	if (itr2 == itr1->second.end())
		return Components::Collider::COLLISION_RULE::CR_COLLIDE;

	return static_cast<Components::Collider::COLLISION_RULE>((*itr2).second);
}

int PhysicsSystem::CreateCollisionLayer(const std::string& name)
{
	if (m_collisionLayers.find(name) != m_collisionLayers.end())
		return m_collisionLayers[name];

	int ID = 0;

	for (auto itr = m_collisionLayers.begin(); itr != m_collisionLayers.end(); ++itr)
	{
		if (itr->second >= ID)
			ID = itr->second + 1;
	}

	m_collisionLayers[name] = ID;

	return ID;
}

void PhysicsSystem::DestroyCollisionLayer(const std::string& name)
{
	auto itr = m_collisionLayers.find(name);

	if (itr != m_collisionLayers.end())
		m_collisionLayers.erase(itr);
}

int PhysicsSystem::GetCollisionLayer(const std::string& layerName)
{
	auto itr = m_collisionLayers.find(layerName);

	if (itr != m_collisionLayers.end())
	{
		return itr->second;
	}

	return CreateCollisionLayer(layerName);
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
	m_mTriggerQueueMutex.lock();

	//Add to trigger queue
	m_triggerQueue.push_back(QueuedTriggerAction{ c_triggerEntity, c_triggeringEntity, true });

	m_mTriggerQueueMutex.unlock();
}

bool PhysicsSystem::SetUntrigger(entt::entity c_triggerEntity, entt::entity c_triggeringEntity)
{
	auto objects = m_triggerList.equal_range(c_triggerEntity);

	for (auto it = objects.first; it != objects.second; ++it)
	{
		if ((*it).second == c_triggeringEntity)
		{
			m_mTriggerQueueMutex.lock();

			//Add to trigger queue
			m_triggerQueue.push_back(QueuedTriggerAction{ c_triggerEntity, c_triggeringEntity, false });

			//m_triggerList.erase(it);

			m_mTriggerQueueMutex.unlock();
			return true;
		}
	}
	return false;
}
