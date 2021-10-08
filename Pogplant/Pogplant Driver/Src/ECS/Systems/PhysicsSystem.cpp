/******************************************************************************/
/*!
\file	PhysicsSystem.cpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details

	System to handle components with colliders and rigidbodies

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#include "../Components/Components.h"
#include "../Components/DependantComponents.h"

#include "PhysicsSystem.h"
#include "../Entity.h"

#include <PhysicsDLC.h>
#include <Pogplant.h>

#include <imgui.h>

#include <glm.hpp>
#include <gtc/matrix_transform.hpp>

PhysicsSystem::PhysicsSystem()
	:
	m_registry{ nullptr },
	m_hasJob{ 0 },
	t_EXIT_THREADS{ false }
{
	m_threads.push_back(std::thread{ &PhysicsSystem::TriggerUpdate, std::ref(*this) });
}

PhysicsSystem::~PhysicsSystem()
{
	CleanUp();
}

void PhysicsSystem::CleanUp()
{
	t_EXIT_THREADS = true;
	m_hasJob.release();

	for (auto& thread : m_threads)
	{
		thread.join();
	}
}

void PhysicsSystem::Init(ECS* ecs)
{
	m_registry = ecs;
}

void PhysicsSystem::InitPlayState()
{
	//Update all colliders
	auto boxColliders = m_registry->GetReg().view<Components::Transform, Components::BoxCollider>();
	auto sphereColliders = m_registry->GetReg().view<Components::Transform, Components::SphereCollider>();

	/*auto combinedView = boxColliders | sphereColliders;

	for (auto collidable : combinedView)
	{
		auto& transform = combinedView.get<Components::Transform>(collidable);

		transform.m_position;
	}//*/

	for (auto collidable : boxColliders)
	{
		auto& transform = boxColliders.get<Components::Transform>(collidable);
		auto& boxCollider = boxColliders.get<Components::BoxCollider>(collidable);

		boxCollider.colliderType = Components::Collider::COLLIDER_TYPE::CT_BOX;
		boxCollider.aabb.CalculateAABBFromExtends(transform.m_position + boxCollider.centre, boxCollider.extends * transform.m_scale);
	}

	for (auto collidable : sphereColliders)
	{
		auto& transform = sphereColliders.get<Components::Transform>(collidable);
		auto& sphereCollider = sphereColliders.get<Components::SphereCollider>(collidable);

		sphereCollider.colliderType = Components::Collider::COLLIDER_TYPE::CT_SPHERE;
		sphereCollider.sphere.m_pos = transform.m_position + sphereCollider.centre;
		sphereCollider.sphere.m_radius = sphereCollider.radius * std::max({ transform.m_scale.x, transform.m_scale.y, transform.m_scale.z });
	}
}

/******************************************************************************/
/*!
\brief
	Updates trigger behaviour on a separate thread to try to improve
	performance
*/
/******************************************************************************/
void PhysicsSystem::TriggerUpdate()
{
	while (!t_EXIT_THREADS)
	{
		//perform busy wait LOL
		m_hasJob.acquire();

		if (!t_EXIT_THREADS)
		{
			auto collidableEntities = m_registry->GetReg().view<Components::Transform, Components::BoxCollider>();
			auto movableEntities = m_registry->GetReg().view<Components::Transform, Components::Rigidbody, Components::BoxCollider>();

			for (auto _1entity : collidableEntities)
			{
				auto& _1collider = collidableEntities.get<Components::BoxCollider>(_1entity);

				if (!_1collider.isTrigger)
					continue;

				//Get list of entities triggered with this
				auto objects = m_triggerList.equal_range(_1entity);

				for (auto _2entity : movableEntities)
				{
					if (_1entity == _2entity)
						continue;

					auto& _2collider = movableEntities.get<Components::BoxCollider>(_2entity);

					if (PhysicsDLC::Collision::StaticAABBAABB(_1collider.aabb, _2collider.aabb))
					{
						//If not in the list trigger
						if (objects.first == objects.second)
						{
							SetTrigger(_1entity, _2entity);
						}
						else
						{
							bool shouldCall = true;
							//Find through
							for (auto it = objects.first; it != objects.second; ++it)
							{
								if ((*it).second == _2entity)
								{
									shouldCall = false;
									break;
								}
							}

							//Can't find so trigger
							if (shouldCall)
								SetTrigger(_1entity, _2entity);
						}
					}
					else if (objects.first != objects.second)
					{
						if(SetUntrigger(_1entity, _2entity))
							objects = m_triggerList.equal_range(_1entity);
					}
				}
			}
		}
	}
}

/******************************************************************************/
/*!
\brief
	Main physics update loop
\param c_dt
	Delta time for this frame
*/
/******************************************************************************/
void PhysicsSystem::Update(float c_dt)
{
	//Temporary since we have no play state
	InitPlayState();

	auto rigidBodyEntities = m_registry->GetReg().view<Components::Transform, Components::Rigidbody, Components::BoxCollider>();
	auto collidableEntities = m_registry->GetReg().view<Components::Transform, Components::BoxCollider>();

	auto movedColliders = m_registry->GetReg().view<Components::Transform, Components::BoxCollider>();

	//Update colliders that move
	for (auto movableColliders : rigidBodyEntities)
	{
		auto& rigidbody = rigidBodyEntities.get<Components::Rigidbody>(movableColliders);

		if (rigidbody.isKinematic)
			continue;

		auto& transform = rigidBodyEntities.get<Components::Transform>(movableColliders);
		auto& collider = rigidBodyEntities.get<Components::BoxCollider>(movableColliders);

		collider.aabb.m_min = transform.m_position + collider.centre - collider.extends * transform.m_scale;
		collider.aabb.m_max = transform.m_position + collider.centre + collider.extends * transform.m_scale;
	}

	//Set the 2nd thread to do trigger list
	m_hasJob.release();


	//O(n^2) probably not ideal, but will do for now
	for (auto _1entity : rigidBodyEntities)
	{
		auto& _1transform = rigidBodyEntities.get<Components::Transform>(_1entity);
		auto& _1rigidbody = rigidBodyEntities.get<Components::Rigidbody>(_1entity);
		auto& _1collider = rigidBodyEntities.get<Components::BoxCollider>(_1entity);

		if (_1rigidbody.isKinematic)
			continue;

		_1collider.aabb.CalculateAABBFromExtends(_1transform.m_position + _1collider.centre, _1collider.extends * _1transform.m_scale);

		if (_1rigidbody.useGravity)
			_1rigidbody.acceleration.y += m_gravityAcc;

		//temp for now
		PP::DebugDraw::DebugLine(_1rigidbody.newPosition, _1rigidbody.newPosition + _1rigidbody.acceleration * _1rigidbody.mass * 0.5f);
		PP::DebugDraw::DebugLine(_1rigidbody.newPosition, _1rigidbody.newPosition + _1rigidbody.impulseAcceleration * _1rigidbody.mass * 0.5f);

		_1rigidbody.velocity += _1rigidbody.acceleration * c_dt + _1rigidbody.impulseAcceleration;
		_1rigidbody.acceleration = PhysicsDLC::Vector::Zero;
		_1rigidbody.impulseAcceleration = PhysicsDLC::Vector::Zero;
		_1rigidbody.newPosition = _1transform.m_position + _1rigidbody.velocity * c_dt;

		if (!_1collider.isTrigger)
		{
			for (auto _2entity : collidableEntities)
			{
				if (_2entity == _1entity)
					continue;

				auto& _2collider = collidableEntities.get<Components::BoxCollider>(_2entity);

				auto collisionRule = GetCollisionRule(_1collider.collisionLayer, _2collider.collisionLayer);

				if (collisionRule == Components::Collider::COLLISION_RULE::CR_IGNORE
					|| _2collider.isTrigger)
					continue;

				auto _2rigidbody = m_registry->GetReg().try_get<Components::Rigidbody>(_2entity);
				
				//auto& _2transform = collidableEntities.get<Components::Transform>(_2entity);
				if (PhysicsDLC::Collision::StaticAABBAABB(_1collider.aabb, _2collider.aabb))
				{
					PhysicsDLC::Physics::ResolveAABBDynamic(_1transform.m_position, _1rigidbody, _2rigidbody, _1collider.aabb, _2collider.aabb, 0.f, c_dt);
				}
				else
				{
					float collisionTime = -1.f;
					if(_2rigidbody)
						collisionTime = PhysicsDLC::Collision::DynamicAABBAABB(_1collider.aabb, _1rigidbody.velocity, _2collider.aabb, _2rigidbody->velocity, 10.f);
					else
						collisionTime = PhysicsDLC::Collision::DynamicAABBAABB(_1collider.aabb, _1rigidbody.velocity, _2collider.aabb, PhysicsDLC::Vector::Zero, 10.f);
					
					if (collisionTime > 0.f && collisionTime < c_dt)
					{
						//std::cout << "COLLIDE" << std::endl;
						PhysicsDLC::Physics::ResolveAABBDynamic(_1transform.m_position, _1rigidbody, _2rigidbody, _1collider.aabb, _2collider.aabb, collisionTime, c_dt);
					}
				}
			}
		}

		_1transform.m_position = _1rigidbody.newPosition;
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

		const auto near = cam.m_Near;
		const auto far = cam.m_Far;
		const glm::vec3 right = cam.m_Right;
		const glm::vec3 up = cam.m_Up;
		const glm::vec3 front = cam.m_Front;
		const float zoom = cam.m_Zoom * 0.5f;

		PP::DebugDraw::DebugFrustum(cam_trans.m_position, glm::radians(zoom), aspect, near, far, right, up, front);
	}
}

void PhysicsSystem::DrawImGUI()
{

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
