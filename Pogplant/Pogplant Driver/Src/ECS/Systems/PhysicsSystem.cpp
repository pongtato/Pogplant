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
	m_shouldContinue{ 0 },
	t_EXIT_THREADS{ false }
{
	m_collisionLayers["DEFAULT"] = 0;

	//Temporary
	CreateCollisionLayer("PLAYER");
	CreateCollisionLayer("ENEMY");
	CreateCollisionLayer("PLAYER PROJECTILES");
	CreateCollisionLayer("ENEMY PROJECTILES");
	CreateCollisionLayer("TRIGGERS");

	SetCollisionRule(GetCollisionLayer("PLAYER"), GetCollisionLayer("PLAYER"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("ENEMY"), GetCollisionLayer("PLAYER"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("ENEMY"), GetCollisionLayer("ENEMY"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("PLAYER"), GetCollisionLayer("PLAYER PROJECTILES"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("ENEMY"), GetCollisionLayer("ENEMY PROJECTILES"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("ENEMY PROJECTILES"), GetCollisionLayer("ENEMY PROJECTILES"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("PLAYER PROJECTILES"), GetCollisionLayer("PLAYER PROJECTILES"), Components::Collider::COLLISION_RULE::CR_IGNORE);

	SetCollisionRule(GetCollisionLayer("TRIGGERS"), GetCollisionLayer("TRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("PLAYER PROJECTILES"), GetCollisionLayer("TRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("ENEMY PROJECTILES"), GetCollisionLayer("TRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("ENEMY"), GetCollisionLayer("TRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);

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

void PhysicsSystem::Init(ECS* ecs, std::shared_ptr<PPE::EventBus>& eventBus)
{
	m_registry = ecs;
	m_eventBus = eventBus;
}

void PhysicsSystem::InitPlayState()
{
	//Update all colliders
	auto boxColliders = m_registry->view<Components::Transform, Components::BoxCollider>();
	auto sphereColliders = m_registry->view<Components::Transform, Components::SphereCollider>();

	/*auto combinedView = boxColliders | sphereColliders;

	for (auto collidable : combinedView)
	{
		auto& transform = combinedView.get<Components::Transform>(collidable);

		transform.m_position;
	}//*/

	for (auto& collidable : boxColliders)
	{
		auto& transform = boxColliders.get<Components::Transform>(collidable);
		auto& boxCollider = boxColliders.get<Components::BoxCollider>(collidable);

		auto colliderIdentifier = m_registry->GetReg().try_get<Components::ColliderIdentifier>(collidable);
		if (!colliderIdentifier)
		{
			m_registry->GetReg().emplace<Components::ColliderIdentifier>(
				collidable,
				Components::ColliderIdentifier::COLLIDER_TYPE::CT_BOX,
				boxCollider.isTrigger,
				GetCollisionLayer(boxCollider.collisionLayer));
		}

		boxCollider.aabb.CalculateAABBFromExtends(transform.GetGlobalPosition() + boxCollider.centre, boxCollider.extends * transform.GetGlobalScale());
	}

	for (auto& collidable : sphereColliders)
	{
		auto& transform = sphereColliders.get<Components::Transform>(collidable);
		auto& sphereCollider = sphereColliders.get<Components::SphereCollider>(collidable);

		auto colliderIdentifier = m_registry->GetReg().try_get<Components::ColliderIdentifier>(collidable);
		if (!colliderIdentifier)
		{
			m_registry->GetReg().emplace<Components::ColliderIdentifier>(
				collidable,
				Components::ColliderIdentifier::COLLIDER_TYPE::CT_SPHERE,
				sphereCollider.isTrigger,
				GetCollisionLayer(sphereCollider.collisionLayer));
		}

		auto tmpScale = transform.GetGlobalScale();
		sphereCollider.sphere.m_pos = transform.GetGlobalPosition() + sphereCollider.centre;
		sphereCollider.sphere.m_radius = sphereCollider.radius * std::max({ tmpScale.x, tmpScale.y, tmpScale.z });
	}

	/// Height map example GAB refer to this hehe xd
	/*auto hmd_view = ecs.view<Transform, HeightMapDebugger>();
	auto heightMap_view = ecs.view<Transform, PrimitiveRender>();
	PP::Mesh* floorMesh = PP::MeshResource::m_MeshPool[PP::MeshResource::MESH_TYPE::HEIGHTMAP];
	for (auto hm : heightMap_view)
	{
		floorMesh->m_Heightmap;
		auto& hmT = heightMap_view.get<Transform>(hm);
		for (auto entity : hmd_view)
		{
			auto& debugObjectT = hmd_view.get<Transform>(entity);
			glm::vec3 mappedPos = glm::vec3(debugObjectT.m_position.x / hmT.m_scale.x, 0.0f, debugObjectT.m_position.z / hmT.m_scale.z);
			// Factor height * heightmap scale + heightmaps relative position then + the size of object
			debugObjectT.m_position.y = floorMesh->GetHeight(mappedPos) * hmT.m_scale.y + hmT.m_position.y + debugObjectT.m_scale.y;
		}
	}//*/

	/*auto heightMap_view = m_registry->view<Components::Transform, Components::PrimitiveRender, Components::HeightMapCollider>();
	PP::Mesh* floorMesh = PP::MeshResource::m_MeshPool[PP::MeshResource::MESH_TYPE::HEIGHTMAP];
	for (auto& heightMap : heightMap_view)
	{
		auto& heightMapTransform = heightMap_view.get<Components::Transform>(heightMap);
		auto& heightMapCollider = heightMap_view.get<Components::HeightMapCollider>(heightMap);

		heightMapCollider.heightMap.m_Heightmap = floorMesh->m_Heightmap;
		heightMapCollider.heightMap.m_HeightMapDim = floorMesh->m_HeightMapDim;
	}//*/
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
		if (!t_EXIT_THREADS /*&& m_hasJob.try_acquire()*/)
		{
			auto collidableEntities = m_registry->view<Components::Transform, Components::BoxCollider>();
			auto movableEntities = m_registry->view<Components::Transform, Components::Rigidbody, Components::BoxCollider>();

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

					auto collisionRule = GetCollisionRule(m_collisionLayers[_1collider.collisionLayer], m_collisionLayers[_2collider.collisionLayer]);
					if (collisionRule == Components::Collider::COLLISION_RULE::CR_IGNORE)
						continue;

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
						if (SetUntrigger(_1entity, _2entity))
							objects = m_triggerList.equal_range(_1entity);
					}
				}
			}

			m_shouldContinue.release();
		}
	}
}

/******************************************************************************/
/*!
\brief
	Main physics update loop for the editor
\param c_dt
	Delta time for this frame
*/
/******************************************************************************/
void PhysicsSystem::UpdateEditor()
{
	//Update all colliders
	auto boxColliders = m_registry->view<Components::Transform, Components::BoxCollider>();
	auto sphereColliders = m_registry->view<Components::Transform, Components::SphereCollider>();

	for (auto& collidable : boxColliders)
	{
		auto& transform = boxColliders.get<Components::Transform>(collidable);
		auto& boxCollider = boxColliders.get<Components::BoxCollider>(collidable);

		auto colliderIdentifier = m_registry->GetReg().try_get<Components::ColliderIdentifier>(collidable);
		if (!colliderIdentifier)
		{
			m_registry->GetReg().emplace<Components::ColliderIdentifier>(
				collidable,
				Components::ColliderIdentifier::COLLIDER_TYPE::CT_BOX,
				boxCollider.isTrigger,
				GetCollisionLayer(boxCollider.collisionLayer));
		}
		else
		{
			colliderIdentifier->isTrigger = boxCollider.isTrigger;
			colliderIdentifier->collisionLayer = GetCollisionLayer(boxCollider.collisionLayer);
		}

		boxCollider.aabb.CalculateAABBFromExtends(transform.GetGlobalPosition() + boxCollider.centre, boxCollider.extends * transform.GetGlobalScale());
	}

	for (auto& collidable : sphereColliders)
	{
		auto& transform = sphereColliders.get<Components::Transform>(collidable);
		auto& sphereCollider = sphereColliders.get<Components::SphereCollider>(collidable);

		auto colliderIdentifier = m_registry->GetReg().try_get<Components::ColliderIdentifier>(collidable);
		if (!colliderIdentifier)
		{
			m_registry->GetReg().emplace<Components::ColliderIdentifier>(
				collidable,
				Components::ColliderIdentifier::COLLIDER_TYPE::CT_SPHERE,
				sphereCollider.isTrigger,
				GetCollisionLayer(sphereCollider.collisionLayer));
		}
		else
		{
			colliderIdentifier->isTrigger = sphereCollider.isTrigger;
			colliderIdentifier->collisionLayer = GetCollisionLayer(sphereCollider.collisionLayer);
		}

		auto tmpScale = transform.GetGlobalScale();
		sphereCollider.sphere.m_pos = transform.GetGlobalPosition() + sphereCollider.centre;
		sphereCollider.sphere.m_radius = sphereCollider.radius * std::max({ tmpScale.x, tmpScale.y, tmpScale.z });
	}
}

/******************************************************************************/
/*!
\brief
	Main physics update loop, meant for playstate
\param c_dt
	Delta time for this frame
*/
/******************************************************************************/
void PhysicsSystem::Update(float c_dt)
{
	auto rigidBodyEntities = m_registry->view<Components::Transform, Components::Rigidbody, Components::ColliderIdentifier>();
	auto collidableEntities = m_registry->view<Components::Transform, Components::ColliderIdentifier>();

	//Update colliders that move
	for (auto& movableColliders : rigidBodyEntities)
	{
		auto& rigidbody = rigidBodyEntities.get<Components::Rigidbody>(movableColliders);

		//if (rigidbody.isKinematic)
		//	continue;

		auto& transform = rigidBodyEntities.get<Components::Transform>(movableColliders);
		auto& colliderIdentifier = rigidBodyEntities.get<Components::ColliderIdentifier>(movableColliders);

		switch (colliderIdentifier.colliderType)
		{
		case Components::ColliderIdentifier::COLLIDER_TYPE::CT_BOX:
		{
			auto boxCollider = m_registry->GetReg().try_get<Components::BoxCollider>(movableColliders);
			boxCollider->aabb.CalculateAABBFromExtends(transform.GetGlobalPosition() + boxCollider->centre, boxCollider->extends * transform.GetGlobalScale());
			break;
		}
		case Components::ColliderIdentifier::COLLIDER_TYPE::CT_SPHERE:
		{
			auto sphereCollider = m_registry->GetReg().try_get<Components::SphereCollider>(movableColliders);
			sphereCollider->sphere.m_pos = transform.GetGlobalPosition() + sphereCollider->centre;
			break;
		}
		default:
			std::cerr << "PhysicsSystem::Update, unhandled collider update" << std::endl;
			assert(false);
		}
	}

	//Set the 2nd thread to update trigger behavior
	m_hasJob.release();

	//O(n^2) probably not ideal, but will do for now
	for (auto& _1entity : rigidBodyEntities)
	{
		auto& _1rigidbody = rigidBodyEntities.get<Components::Rigidbody>(_1entity);

		if (_1rigidbody.isKinematic)
			continue;

		auto& _1transform = rigidBodyEntities.get<Components::Transform>(_1entity);
		auto& _1colliderIdentifier = rigidBodyEntities.get<Components::ColliderIdentifier>(_1entity);

		if (_1rigidbody.useGravity)
			_1rigidbody.acceleration.y += m_gravityAcc;

		//Temp debug draw to show resultant forces, will change location after playstate if needed
		PP::DebugDraw::DebugLine(_1rigidbody.newPosition, _1rigidbody.newPosition + _1rigidbody.acceleration * _1rigidbody.mass * 0.5f);
		PP::DebugDraw::DebugLine(_1rigidbody.newPosition, _1rigidbody.newPosition + _1rigidbody.impulseAcceleration * _1rigidbody.mass * 0.5f);

		_1rigidbody.velocity += _1rigidbody.acceleration * c_dt + _1rigidbody.impulseAcceleration;
		_1rigidbody.acceleration = PhysicsDLC::Vector::Zero;
		_1rigidbody.impulseAcceleration = PhysicsDLC::Vector::Zero;
		_1rigidbody.newPosition = _1transform.GetGlobalPosition() + _1rigidbody.velocity * c_dt;

		if (!_1colliderIdentifier.isTrigger)
		{
			for (auto _2entity : collidableEntities)
			{
				if (_2entity == _1entity)
					continue;

				auto& _2colliderIdentifier = collidableEntities.get<Components::ColliderIdentifier>(_2entity);

				if (_2colliderIdentifier.isTrigger)
					continue;

				auto& _2transform = collidableEntities.get<Components::Transform>(_2entity);

				auto _2rigidbody = m_registry->GetReg().try_get<Components::Rigidbody>(_2entity);
				HandleCollision(_1entity, _2entity, _1transform, _2transform, _1rigidbody, _2rigidbody, _1colliderIdentifier, _2colliderIdentifier, c_dt);
			}
		}

		_1transform.SetGlobalPosition(_1rigidbody.newPosition);
	}

	while (!m_shouldContinue.try_acquire())
	{
	}

	if (!m_triggerQueue.empty())
	{
		m_mTriggerQueueMutex.lock();

		while (!m_triggerQueue.empty())
		{
			auto& queuedAction = m_triggerQueue.back();

			if (std::get<2>(queuedAction))
			{
				auto& entity1 = std::get<0>(queuedAction);
				auto& entity2 = std::get<1>(queuedAction);

				if (m_registry->GetReg().valid(entity1) && m_registry->GetReg().valid(entity2))
					m_eventBus->emit(
						std::make_shared<PPE::OnTriggerEnterEvent>(
							entity1,
							entity2
							)
					);

				//std::cout << "Triggered" << (uint16_t)std::get<0>(queuedAction) << " " << (uint16_t)std::get<1>(queuedAction) << std::endl;
			}
			else
			{
				auto& entity1 = std::get<0>(queuedAction);
				auto& entity2 = std::get<1>(queuedAction);

				if (m_registry->GetReg().valid(entity1) && m_registry->GetReg().valid(entity2))

				m_eventBus->emit(
					std::make_shared<PPE::OnTriggerExitEvent>(
						entity1,
						entity2
						)
				);

				//std::cout << "UnTriggered" << (uint16_t)std::get<0>(queuedAction) << " " << (uint16_t)std::get<1>(queuedAction) << std::endl;
			}

			m_triggerQueue.pop_back();
		}

		m_mTriggerQueueMutex.unlock();
	}
}
