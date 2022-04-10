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
#include <ImGuizmo.h>

#include "../../Application.h"

#include "../../Serialiser/CustomSaver.h"

#ifdef TRACY_ENABLE
#include "../../Tools/Tracy/Tracy.hpp"
#endif

PhysicsSystem::PhysicsSystem()
	:
	m_registry{ nullptr },
	t_EXIT_THREADS{ false }
{
	m_collisionLayers["DEFAULT"] = 0;

	LoadLayers();

	/*SetCollisionRule(GetCollisionLayer("ENEMY"), GetCollisionLayer("PLAYER"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("ENEMY"), GetCollisionLayer("ENEMY"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("PLAYER"), GetCollisionLayer("PLAYER PROJECTILES"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("ENEMY"), GetCollisionLayer("ENEMY PROJECTILES"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("ENEMY PROJECTILES"), GetCollisionLayer("ENEMY PROJECTILES"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("PLAYER PROJECTILES"), GetCollisionLayer("PLAYER PROJECTILES"), Components::Collider::COLLISION_RULE::CR_IGNORE);

	SetCollisionRule(GetCollisionLayer("TRIGGERS"), GetCollisionLayer("TRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("PLAYER PROJECTILES"), GetCollisionLayer("TRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("ENEMY PROJECTILES"), GetCollisionLayer("TRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("ENEMY"), GetCollisionLayer("TRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("OBSTACLES"), GetCollisionLayer("TRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);

	SetCollisionRule(GetCollisionLayer("WAVETRIGGERS"), GetCollisionLayer("WAVETRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("PLAYER PROJECTILES"), GetCollisionLayer("WAVETRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("ENEMY PROJECTILES"), GetCollisionLayer("WAVETRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("ENEMY"), GetCollisionLayer("WAVETRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);
	SetCollisionRule(GetCollisionLayer("OBSTACLES"), GetCollisionLayer("WAVETRIGGERS"), Components::Collider::COLLISION_RULE::CR_IGNORE);

	SetCollisionRule(GetCollisionLayer("OBSTACLES"), GetCollisionLayer("OBSTACLES"), Components::Collider::COLLISION_RULE::CR_IGNORE);


	for (auto itr = m_collisionLayers.begin(); itr != m_collisionLayers.end(); itr++)
	{
		SetCollisionRule(GetCollisionLayer("PLAYERBOX"), itr->second, Components::Collider::COLLISION_RULE::CR_IGNORE);
	}

	SetCollisionRule(GetCollisionLayer("PLAYERBOX"), GetCollisionLayer("PLAYER"), Components::Collider::COLLISION_RULE::CR_COLLIDE);
	//*/

	//Create threads
	for (int i = 0; i < NUM_TRIGGER_THREADS; i++)
	{
		m_hasTriggerJob[i] = std::make_unique<std::binary_semaphore>(0);
		m_shouldContinue[i] = std::make_unique<std::binary_semaphore>(0);

		m_threads.push_back(std::thread{ &PhysicsSystem::TriggerUpdate, std::ref(*this), i });
	}
}

PhysicsSystem::~PhysicsSystem()
{
	CleanUp();
}

void PhysicsSystem::CleanUp()
{
	Clear();

	t_EXIT_THREADS = true;

	for (size_t i = 0; i < NUM_TRIGGER_THREADS; i++)
	{
		m_hasTriggerJob[i]->release();
	}

	for (auto& thread : m_threads)
	{
		thread.join();
	}
}

void PhysicsSystem::Init(ECS* ecs, std::shared_ptr<PPE::EventBus>& eventBus)
{
	m_registry = ecs;
	m_eventBus = eventBus;

	m_mTriggerQueueMutex.lock();
	m_triggerList.clear();
	m_triggerQueue.clear();
	m_mTriggerQueueMutex.unlock();


	m_broadphase.SetNullObject(entt::null);
}

/******************************************************************************/
/*!
\brief
	Initialises the play state
*/
/******************************************************************************/
void PhysicsSystem::InitPlayState()
{
	UpdateEditor();
}

/******************************************************************************/
/*!
\brief
	Updates only moving objects in the scene
*/
/******************************************************************************/
void PhysicsSystem::UpdateMovingObjects()
{
	//Update all colliders
	auto boxColliders = m_registry->view<Components::Transform, Components::BoxCollider>();
	for (auto& collidable : boxColliders)
	{
		auto& boxCollider = boxColliders.get<Components::BoxCollider>(collidable);

		if (boxCollider.isStatic)
			continue;

		auto& transform = boxColliders.get<Components::Transform>(collidable);

		boxCollider.aabb.CalculateAABBFromExtends(
			transform.GetGlobalPosition() + boxCollider.centre,
			boxCollider.extends * transform.GetGlobalScale());

		//Update broadphase and identifiers
		auto colliderIdentifier = m_registry->GetReg().try_get<Components::ColliderIdentifier>(collidable);
		if (!colliderIdentifier)
		{
			auto& identifier = m_registry->GetReg().emplace<Components::ColliderIdentifier>(
				collidable,
				Components::ColliderIdentifier::COLLIDER_TYPE::CT_BOX,
				boxCollider.isTrigger,
				GetCollisionLayer(boxCollider.collisionLayer));

			m_broadphase.InsertData(&identifier.broadPhaseKey, collidable, boxCollider.aabb);
		}
		else
		{
			colliderIdentifier->isTrigger = boxCollider.isTrigger;
			colliderIdentifier->collisionLayer = GetCollisionLayer(boxCollider.collisionLayer);

			if (colliderIdentifier->broadPhaseKey)
			{
				auto rigidbody = m_registry->GetReg().try_get<Components::Rigidbody>(collidable);

				if (rigidbody && !rigidbody->isKinematic)
				{
					static PhysicsDLC::Collision::Shapes::AABB dynamicAABB;

					dynamicAABB.CalculateAABBFromExtends(
						transform.GetGlobalPosition() + boxCollider.centre,
						boxCollider.extends * transform.GetGlobalScale() * (1.f + (float)rigidbody->velocity.length()));

					m_broadphase.UpdateData(&colliderIdentifier->broadPhaseKey, collidable, dynamicAABB);
				}
				else
				{
					m_broadphase.UpdateData(&colliderIdentifier->broadPhaseKey, collidable, boxCollider.aabb);
				}
			}
			else
				m_broadphase.InsertData(&colliderIdentifier->broadPhaseKey, collidable, boxCollider.aabb);
		}
	}

	auto meshColliders = m_registry->view<Components::Transform, Components::Renderer, Components::MeshCollider>();
	for (auto& collidable : meshColliders)
	{
		auto& meshCollider = meshColliders.get<Components::MeshCollider>(collidable);

		if (meshCollider.isStatic)
			continue;

		auto& transform = meshColliders.get<Components::Transform>(collidable);
		auto& renderer = meshColliders.get<Components::Renderer>(collidable);

		meshCollider.m_id = collidable;

		float maxScale = std::max({ transform.m_scale.x, transform.m_scale.y, transform.m_scale.z });

		meshCollider.aabb.CalculateAABBFromExtends(
			transform.GetGlobalPosition(),
			glm::vec3{
			renderer.m_RenderModel->m_Bounds.longest,
			renderer.m_RenderModel->m_Bounds.longest,
			renderer.m_RenderModel->m_Bounds.longest
			} *maxScale
		);

		//Update broadphase and identifiers
		auto colliderIdentifier = m_registry->GetReg().try_get<Components::ColliderIdentifier>(collidable);
		if (!colliderIdentifier)
		{
			auto& identifier = m_registry->GetReg().emplace<Components::ColliderIdentifier>(
				collidable,
				Components::ColliderIdentifier::COLLIDER_TYPE::CT_GJKMESH,
				meshCollider.isTrigger,
				GetCollisionLayer(meshCollider.collisionLayer));

			m_broadphase.InsertData(&identifier.broadPhaseKey, collidable, meshCollider.aabb);
		}
		else
		{
			colliderIdentifier->isTrigger = meshCollider.isTrigger;
			colliderIdentifier->collisionLayer = GetCollisionLayer(meshCollider.collisionLayer);

			if (colliderIdentifier->broadPhaseKey)
				m_broadphase.UpdateData(&colliderIdentifier->broadPhaseKey, collidable, meshCollider.aabb);
			else
				m_broadphase.InsertData(&colliderIdentifier->broadPhaseKey, collidable, meshCollider.aabb);
		}
	}

	auto sphereColliders = m_registry->view<Components::Transform, Components::SphereCollider>();
	for (auto& collidable : sphereColliders)
	{
		auto& sphereCollider = sphereColliders.get<Components::SphereCollider>(collidable);

		if (sphereCollider.isStatic)
			continue;

		auto& transform = sphereColliders.get<Components::Transform>(collidable);

		auto tmpScale = transform.GetGlobalScale();
		sphereCollider.sphere.m_pos = transform.GetGlobalPosition() + sphereCollider.centre;
		sphereCollider.sphere.m_radius = sphereCollider.radius * std::max({ tmpScale.x, tmpScale.y, tmpScale.z });

		sphereCollider.aabb.CalculateAABBFromExtends(
			sphereCollider.sphere.m_pos,
			glm::vec3{
				sphereCollider.sphere.m_radius,
				sphereCollider.sphere.m_radius,
				sphereCollider.sphere.m_radius
			} *1.1f
		);

		auto colliderIdentifier = m_registry->GetReg().try_get<Components::ColliderIdentifier>(collidable);
		if (!colliderIdentifier)
		{
			auto& identifier = m_registry->GetReg().emplace<Components::ColliderIdentifier>(
				collidable,
				Components::ColliderIdentifier::COLLIDER_TYPE::CT_SPHERE,
				sphereCollider.isTrigger,
				GetCollisionLayer(sphereCollider.collisionLayer));

			m_broadphase.InsertData(&identifier.broadPhaseKey, collidable, sphereCollider.aabb);
		}
		else
		{
			colliderIdentifier->isTrigger = sphereCollider.isTrigger;
			colliderIdentifier->collisionLayer = GetCollisionLayer(sphereCollider.collisionLayer);

			if (colliderIdentifier->broadPhaseKey)
				m_broadphase.UpdateData(&colliderIdentifier->broadPhaseKey, collidable, sphereCollider.aabb);
			else
				m_broadphase.InsertData(&colliderIdentifier->broadPhaseKey, collidable, sphereCollider.aabb);
		}
	}

	auto obbBoxColliders = m_registry->view<Components::Transform, Components::OBBBoxCollider>();
	for (auto& collidable : obbBoxColliders)
	{
		auto& obbBoxCollider = obbBoxColliders.get<Components::OBBBoxCollider>(collidable);

		if (obbBoxCollider.isStatic)
			continue;

		auto& transform = obbBoxColliders.get<Components::Transform>(collidable);

		glm::vec3 scale = (obbBoxCollider.extends * transform.GetGlobalScale());

		float maxLength = std::max({ scale.x, scale.y, scale.z }) * 2.f;

		obbBoxCollider.aabb.CalculateAABBFromExtends(transform.GetGlobalPosition() + obbBoxCollider.centre, glm::vec3{ maxLength, maxLength, maxLength });
		obbBoxCollider.m_id = collidable;

		auto colliderIdentifier = m_registry->GetReg().try_get<Components::ColliderIdentifier>(collidable);
		if (!colliderIdentifier)
		{
			auto& identifier = m_registry->GetReg().emplace<Components::ColliderIdentifier>(
				collidable,
				Components::ColliderIdentifier::COLLIDER_TYPE::CT_OBBBOX,
				obbBoxCollider.isTrigger,
				GetCollisionLayer(obbBoxCollider.collisionLayer));

			m_broadphase.InsertData(&identifier.broadPhaseKey, collidable, obbBoxCollider.aabb);
		}
		else
		{
			colliderIdentifier->isTrigger = obbBoxCollider.isTrigger;
			colliderIdentifier->collisionLayer = GetCollisionLayer(obbBoxCollider.collisionLayer);

			if (colliderIdentifier->broadPhaseKey)
				m_broadphase.UpdateData(&colliderIdentifier->broadPhaseKey, collidable, obbBoxCollider.aabb);
			else
				m_broadphase.InsertData(&colliderIdentifier->broadPhaseKey, collidable, obbBoxCollider.aabb);
		}
	}
}

/******************************************************************************/
/*!
\brief
	Updates trigger behaviour on a separate thread to try to improve
	performance
*/
/******************************************************************************/
void PhysicsSystem::TriggerUpdate(int threadID)
{
	//int entityIndex = 0;
	while (!t_EXIT_THREADS)
	{
		//perform busy wait LOL
		m_hasTriggerJob[threadID]->acquire();

		if (!t_EXIT_THREADS)
		{
			for (size_t i = threadID; i < m_collisionQuery.m_query.size(); i += NUM_TRIGGER_THREADS)
			{
				//if ((threadID + i) % NUM_TRIGGER_THREADS != 0)
				//	continue;

				//std::cout << i << " of " << m_collisionQuery.m_query.size() << std::endl;

				auto query = m_collisionQuery.m_query[i];

				if (query.m_ID1 < query.m_ID2)
					std::swap(query.m_ID1, query.m_ID2);

				auto& _1colliderIdentifier = m_registry->GetReg().get<Components::ColliderIdentifier>(query.m_ID1);
				auto& _2colliderIdentifier = m_registry->GetReg().get<Components::ColliderIdentifier>(query.m_ID2);

				//If no triggers ignore
				if (_1colliderIdentifier.isTrigger || _2colliderIdentifier.isTrigger)
				{
					auto rigidbodyCheck = m_registry->GetReg().try_get<Components::Rigidbody>(query.m_ID1);

					if (!rigidbodyCheck)
						rigidbodyCheck = m_registry->GetReg().try_get<Components::Rigidbody>(query.m_ID2);

					//If no rigidbody ignore
					if (rigidbodyCheck)
					{
						auto collisionRule = GetCollisionRule(_1colliderIdentifier.collisionLayer, _2colliderIdentifier.collisionLayer);

						if (collisionRule == Components::Collider::COLLISION_RULE::CR_IGNORE)
							continue;

						SetParsed(query.m_ID1, query.m_ID2);

						auto objects = m_triggerList.equal_range(query.m_ID1);

						bool collided = false;

						switch (_1colliderIdentifier.colliderType)
						{
						case Components::ColliderIdentifier::COLLIDER_TYPE::CT_BOX:
						{
							auto _1collider = m_registry->GetReg().try_get<Components::BoxCollider>(query.m_ID1);

							switch (_2colliderIdentifier.colliderType)
							{
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_BOX:
							{
								//Box box
								auto _2collider = m_registry->GetReg().try_get<Components::BoxCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::StaticAABBAABB(_1collider->aabb, _2collider->aabb);
								break;
							}
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_GJKMESH:
							{
								//Box Mesh
								auto _2collider = m_registry->GetReg().try_get<Components::MeshCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_SPHERE:
							{
								//Box sphere
								auto _2collider = m_registry->GetReg().try_get<Components::SphereCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_OBBBOX:
							{
								//Box OBB
								auto _2collider = m_registry->GetReg().try_get<Components::OBBBoxCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							default:
								assert(false && "Unsupported type detected");
								break;
							}
							break;
						}
						case Components::ColliderIdentifier::COLLIDER_TYPE::CT_GJKMESH:
						{
							auto _1collider = m_registry->GetReg().try_get<Components::MeshCollider>(query.m_ID1);

							switch (_2colliderIdentifier.colliderType)
							{
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_BOX:
							{
								//Mesh BOX
								auto _2collider = m_registry->GetReg().try_get<Components::BoxCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_GJKMESH:
							{
								//Mesh GJK
								auto _2collider = m_registry->GetReg().try_get<Components::MeshCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_SPHERE:
							{
								//Mesh sphere
								auto _2collider = m_registry->GetReg().try_get<Components::SphereCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_OBBBOX:
							{
								//Mesh OBB
								auto _2collider = m_registry->GetReg().try_get<Components::OBBBoxCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							default:
								assert(false && "Unsupported type detected");
								break;
							}
							break;
						}
						case Components::ColliderIdentifier::COLLIDER_TYPE::CT_SPHERE:
						{
							auto _1collider = m_registry->GetReg().try_get<Components::SphereCollider>(query.m_ID1);

							switch (_2colliderIdentifier.colliderType)
							{
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_BOX:
							{
								//Sphere BOX
								auto _2collider = m_registry->GetReg().try_get<Components::BoxCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_GJKMESH:
							{
								//Sphere Mesh
								auto _2collider = m_registry->GetReg().try_get<Components::MeshCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_SPHERE:
							{
								//Sphere sphere
								auto _2collider = m_registry->GetReg().try_get<Components::SphereCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::StaticSphereSphere(_1collider->sphere, _2collider->sphere);
								break;
							}
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_OBBBOX:
							{
								//Sphere OBB
								auto _2collider = m_registry->GetReg().try_get<Components::OBBBoxCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							default:
								assert(false && "Unsupported type detected");
								break;
							}
							break;
						}
						case Components::ColliderIdentifier::COLLIDER_TYPE::CT_OBBBOX:
						{
							auto _1collider = m_registry->GetReg().try_get<Components::OBBBoxCollider>(query.m_ID1);

							switch (_2colliderIdentifier.colliderType)
							{
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_BOX:
							{
								//OBB BOX
								auto _2collider = m_registry->GetReg().try_get<Components::BoxCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_GJKMESH:
							{
								//OBB Mesh
								auto _2collider = m_registry->GetReg().try_get<Components::MeshCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_SPHERE:
							{
								//OBB sphere
								auto _2collider = m_registry->GetReg().try_get<Components::SphereCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							case Components::ColliderIdentifier::COLLIDER_TYPE::CT_OBBBOX:
							{
								//OBB OBB
								auto _2collider = m_registry->GetReg().try_get<Components::OBBBoxCollider>(query.m_ID2);
								collided = PhysicsDLC::Collision::GJK::GJK::Intersect(_1collider, _2collider);
								break;
							}
							default:
								assert(false && "Unsupported type detected");
								break;
							}
							break;
						}
						default:
							assert(false && "Unsupported type detected");
							break;
						}

						if (collided)
						{
							//If not in the list trigger
							if (objects.first == objects.second)
							{
								SetTrigger(query.m_ID1, query.m_ID2);
							}
							else
							{
								bool shouldCall = true;
								//Find through
								for (auto it = objects.first; it != objects.second; ++it)
								{
									if ((*it).second == query.m_ID2)
									{
										shouldCall = false;
										break;
									}
								}

								//Can't find so trigger
								if (shouldCall)
									SetTrigger(query.m_ID1, query.m_ID2);
							}
						}
						else if (objects.first != objects.second)
						{
							SetUntrigger(query.m_ID1, query.m_ID2);
						}
					}
				}
			}

			m_shouldContinue[threadID]->release();
		}//*/

		/*if (!t_EXIT_THREADS)
		{
			auto collidableEntities = m_registry->view<Components::Transform, Components::ColliderIdentifier>();
			auto movableEntities = m_registry->view<Components::Transform, Components::Rigidbody, Components::ColliderIdentifier>();

			int entityIndex = -1;
			for (auto _1entity : collidableEntities)
			{
				++entityIndex;

				if ((threadID + entityIndex) % NUM_TRIGGER_THREADS != 0)
					continue;

				auto& _1colliderIdentifier = collidableEntities.get<Components::ColliderIdentifier>(_1entity);

				if (!_1colliderIdentifier.isTrigger)
					continue;

				//Get list of entities triggered with this
				auto objects = m_triggerList.equal_range(_1entity);

				for (auto _2entity : movableEntities)
				{
					if (_1entity == _2entity)
						continue;


					auto& _2colliderIdentifier = movableEntities.get<Components::ColliderIdentifier>(_2entity);

					auto collisionRule = GetCollisionRule(_1colliderIdentifier.collisionLayer, _2colliderIdentifier.collisionLayer);
					if (collisionRule == Components::Collider::COLLISION_RULE::CR_IGNORE)
						continue;

					auto _1collider = m_registry->GetReg().try_get<Components::BoxCollider>(_1entity);
					auto _2collider = m_registry->GetReg().try_get<Components::BoxCollider>(_2entity);

					if (_1collider && _2collider && PhysicsDLC::Collision::StaticAABBAABB(_1collider->aabb, _2collider->aabb))
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

			m_shouldContinue[threadID]->release();
		}//*/
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
#ifdef TRACY_ENABLE
	ZoneScoped
#endif

	//Update all colliders
	auto boxColliders = m_registry->view<Components::Transform, Components::BoxCollider>();
	for (auto& collidable : boxColliders)
	{
		auto& transform = boxColliders.get<Components::Transform>(collidable);
		auto& boxCollider = boxColliders.get<Components::BoxCollider>(collidable);

		boxCollider.aabb.CalculateAABBFromExtends(
			transform.GetGlobalPosition() + boxCollider.centre,
			boxCollider.extends * transform.GetGlobalScale());

		//Update broadphase and identifiers
		auto colliderIdentifier = m_registry->GetReg().try_get<Components::ColliderIdentifier>(collidable);
		if (!colliderIdentifier)
		{
			auto& identifier = m_registry->GetReg().emplace<Components::ColliderIdentifier>(
				collidable,
				Components::ColliderIdentifier::COLLIDER_TYPE::CT_BOX,
				boxCollider.isTrigger,
				GetCollisionLayer(boxCollider.collisionLayer));

			m_broadphase.InsertData(&identifier.broadPhaseKey, collidable, boxCollider.aabb);
		}
		else
		{
			colliderIdentifier->isTrigger = boxCollider.isTrigger;
			colliderIdentifier->collisionLayer = GetCollisionLayer(boxCollider.collisionLayer);

			if (colliderIdentifier->broadPhaseKey)
			{
				auto rigidbody = m_registry->GetReg().try_get<Components::Rigidbody>(collidable);

				if (rigidbody && !rigidbody->isKinematic)
				{
					static PhysicsDLC::Collision::Shapes::AABB dynamicAABB;

					dynamicAABB.CalculateAABBFromExtends(
						transform.GetGlobalPosition() + boxCollider.centre,
						boxCollider.extends * transform.GetGlobalScale() * (1.f + (float)rigidbody->velocity.length()));

					m_broadphase.UpdateData(&colliderIdentifier->broadPhaseKey, collidable, dynamicAABB);
				}
				else
				{
					m_broadphase.UpdateData(&colliderIdentifier->broadPhaseKey, collidable, boxCollider.aabb);
				}
			}
			else
				m_broadphase.InsertData(&colliderIdentifier->broadPhaseKey, collidable, boxCollider.aabb);
		}
	}

	auto meshColliders = m_registry->view<Components::Transform, Components::Renderer, Components::MeshCollider>();
	for (auto& collidable : meshColliders)
	{
		auto& transform = meshColliders.get<Components::Transform>(collidable);
		auto& meshCollider = meshColliders.get<Components::MeshCollider>(collidable);
		auto& renderer = meshColliders.get<Components::Renderer>(collidable);

		meshCollider.m_id = collidable;

		float maxScale = std::max({ transform.m_scale.x, transform.m_scale.y, transform.m_scale.z });

		meshCollider.aabb.CalculateAABBFromExtends(
			transform.GetGlobalPosition(),
			glm::vec3{
			renderer.m_RenderModel->m_Bounds.longest,
			renderer.m_RenderModel->m_Bounds.longest,
			renderer.m_RenderModel->m_Bounds.longest
			} *maxScale
		);

		//Update broadphase and identifiers
		auto colliderIdentifier = m_registry->GetReg().try_get<Components::ColliderIdentifier>(collidable);
		if (!colliderIdentifier)
		{
			auto& identifier = m_registry->GetReg().emplace<Components::ColliderIdentifier>(
				collidable,
				Components::ColliderIdentifier::COLLIDER_TYPE::CT_GJKMESH,
				meshCollider.isTrigger,
				GetCollisionLayer(meshCollider.collisionLayer));

			m_broadphase.InsertData(&identifier.broadPhaseKey, collidable, meshCollider.aabb);
		}
		else
		{
			colliderIdentifier->isTrigger = meshCollider.isTrigger;
			colliderIdentifier->collisionLayer = GetCollisionLayer(meshCollider.collisionLayer);

			if (colliderIdentifier->broadPhaseKey)
				m_broadphase.UpdateData(&colliderIdentifier->broadPhaseKey, collidable, meshCollider.aabb);
			else
				m_broadphase.InsertData(&colliderIdentifier->broadPhaseKey, collidable, meshCollider.aabb);
		}
	}

	auto sphereColliders = m_registry->view<Components::Transform, Components::SphereCollider>();
	for (auto& collidable : sphereColliders)
	{
		auto& transform = sphereColliders.get<Components::Transform>(collidable);
		auto& sphereCollider = sphereColliders.get<Components::SphereCollider>(collidable);

		auto tmpScale = transform.GetGlobalScale();
		sphereCollider.sphere.m_pos = transform.GetGlobalPosition() + sphereCollider.centre;
		sphereCollider.sphere.m_radius = sphereCollider.radius * std::max({ tmpScale.x, tmpScale.y, tmpScale.z });

		sphereCollider.aabb.CalculateAABBFromExtends(
			sphereCollider.sphere.m_pos,
			glm::vec3{
				sphereCollider.sphere.m_radius,
				sphereCollider.sphere.m_radius,
				sphereCollider.sphere.m_radius
			} *1.1f
		);

		auto colliderIdentifier = m_registry->GetReg().try_get<Components::ColliderIdentifier>(collidable);
		if (!colliderIdentifier)
		{
			auto& identifier = m_registry->GetReg().emplace<Components::ColliderIdentifier>(
				collidable,
				Components::ColliderIdentifier::COLLIDER_TYPE::CT_SPHERE,
				sphereCollider.isTrigger,
				GetCollisionLayer(sphereCollider.collisionLayer));

			m_broadphase.InsertData(&identifier.broadPhaseKey, collidable, sphereCollider.aabb);
		}
		else
		{
			colliderIdentifier->isTrigger = sphereCollider.isTrigger;
			colliderIdentifier->collisionLayer = GetCollisionLayer(sphereCollider.collisionLayer);

			if (colliderIdentifier->broadPhaseKey)
				m_broadphase.UpdateData(&colliderIdentifier->broadPhaseKey, collidable, sphereCollider.aabb);
			else
				m_broadphase.InsertData(&colliderIdentifier->broadPhaseKey, collidable, sphereCollider.aabb);
		}
	}

	auto obbBoxColliders = m_registry->view<Components::Transform, Components::OBBBoxCollider>();
	for (auto& collidable : obbBoxColliders)
	{
		auto& transform = obbBoxColliders.get<Components::Transform>(collidable);
		auto& obbBoxCollider = obbBoxColliders.get<Components::OBBBoxCollider>(collidable);

		glm::vec3 scale = (obbBoxCollider.extends * transform.GetGlobalScale());

		float maxLength = std::max({ scale.x, scale.y, scale.z }) * 2.f;

		obbBoxCollider.aabb.CalculateAABBFromExtends(transform.GetGlobalPosition() + obbBoxCollider.centre, glm::vec3{ maxLength, maxLength, maxLength });
		obbBoxCollider.m_id = collidable;

		auto colliderIdentifier = m_registry->GetReg().try_get<Components::ColliderIdentifier>(collidable);
		if (!colliderIdentifier)
		{
			auto& identifier = m_registry->GetReg().emplace<Components::ColliderIdentifier>(
				collidable,
				Components::ColliderIdentifier::COLLIDER_TYPE::CT_OBBBOX,
				obbBoxCollider.isTrigger,
				GetCollisionLayer(obbBoxCollider.collisionLayer));

			m_broadphase.InsertData(&identifier.broadPhaseKey, collidable, obbBoxCollider.aabb);
		}
		else
		{
			colliderIdentifier->isTrigger = obbBoxCollider.isTrigger;
			colliderIdentifier->collisionLayer = GetCollisionLayer(obbBoxCollider.collisionLayer);

			if (colliderIdentifier->broadPhaseKey)
				m_broadphase.UpdateData(&colliderIdentifier->broadPhaseKey, collidable, obbBoxCollider.aabb);
			else
				m_broadphase.InsertData(&colliderIdentifier->broadPhaseKey, collidable, obbBoxCollider.aabb);
		}
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
	//Update AABBTree
	UpdateMovingObjects();

	/*for (auto entity : m_registry->m_EntitiesToDelete)
	{
		m_broadphase.RemoveData(entity);
	}//*/

	//Query AABBTree
	m_collisionQuery.Clear();
	m_broadphase.QueryTree(m_collisionQuery);

	//std::cout << m_collisionQuery.m_query.size() << std::endl;

	m_triggerPurgeList = m_triggerList;

	//Set the other threads to update trigger behavior
	for (size_t i = 0; i < NUM_TRIGGER_THREADS; i++)
		m_hasTriggerJob[i]->release();

	auto rigidBodyEntities = m_registry->view<Components::Transform, Components::Rigidbody>();

	//Update new rigidbody positions
	for (auto& _1entity : rigidBodyEntities)
	{
		auto& _1rigidbody = rigidBodyEntities.get<Components::Rigidbody>(_1entity);

		if (_1rigidbody.isKinematic)
			continue;

		auto& _1transform = rigidBodyEntities.get<Components::Transform>(_1entity);

		if (_1rigidbody.useGravity)
			_1rigidbody.acceleration.y += m_gravityAcc;

#ifdef PPD_EDITOR_BUILD
		PP::DebugDraw::DebugLine(_1rigidbody.newPosition, _1rigidbody.newPosition + _1rigidbody.acceleration * _1rigidbody.mass * 0.5f);
		PP::DebugDraw::DebugLine(_1rigidbody.newPosition, _1rigidbody.newPosition + _1rigidbody.impulseAcceleration * _1rigidbody.mass * 0.5f);
#endif

		_1rigidbody.velocity += _1rigidbody.acceleration * c_dt + _1rigidbody.impulseAcceleration;
		_1rigidbody.acceleration = PhysicsDLC::Vector::Zero;
		_1rigidbody.impulseAcceleration = PhysicsDLC::Vector::Zero;
		_1rigidbody.newPosition = _1transform.GetGlobalPosition() + _1rigidbody.velocity * c_dt;

		/*if (!_1colliderIdentifier.isTrigger)
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
		}//*/
	}

	//Handle/resolve positions
	CollisionUpdate(c_dt);

	//Update positions of colliders after collision checking
	for (auto& _1entity : rigidBodyEntities)
	{
		auto& _1rigidbody = rigidBodyEntities.get<Components::Rigidbody>(_1entity);

		if (_1rigidbody.isKinematic)
			continue;

		auto& _1transform = rigidBodyEntities.get<Components::Transform>(_1entity);

		_1transform.SetGlobalPosition(_1rigidbody.newPosition);
	}

	//wait for all threads to be done
	for (size_t i = 0; i < NUM_TRIGGER_THREADS; i++)
	{
		while (!m_shouldContinue[i]->try_acquire())
		{
		}
	}

	//Execute queued actions by the trigger
	if (!m_triggerQueue.empty())
	{
		m_mTriggerQueueMutex.lock();

		while (!m_triggerQueue.empty())
		{
			auto& queuedAction = m_triggerQueue.back();

			if (queuedAction.onEnter)
			{
				if (m_registry->GetReg().valid(queuedAction.entity1) && m_registry->GetReg().valid(queuedAction.entity2))
					m_eventBus->emit(
						std::make_shared<PPE::OnTriggerEnterEvent>(
							queuedAction.entity1,
							queuedAction.entity2
							)
					);

				m_triggerList.insert(std::make_pair(queuedAction.entity1, queuedAction.entity2));

				//std::cout << "Triggered" << (uint16_t)queuedAction.entity1 << " " << (uint16_t)queuedAction.entity2 << std::endl;
			}
			else
			{
				if (m_registry->GetReg().valid(queuedAction.entity1) && m_registry->GetReg().valid(queuedAction.entity2))
					m_eventBus->emit(
						std::make_shared<PPE::OnTriggerExitEvent>(
							queuedAction.entity1,
							queuedAction.entity2
							)
					);

				auto objects = m_triggerList.equal_range(queuedAction.entity1);

				for (auto it = objects.first; it != objects.second; ++it)
				{
					if ((*it).second == queuedAction.entity2)
					{
						m_triggerList.erase(it);
						break;
					}
				}

				//std::cout << "UnTriggered" << (uint16_t)queuedAction.entity1 << " " << (uint16_t)queuedAction.entity2 << std::endl;
			}

			m_triggerQueue.pop_back();
		}

		m_mTriggerQueueMutex.unlock();
	}

	//Process purge list, seems like gonna be slow
	for (auto purgeItr : m_triggerPurgeList)
	{
		if (m_registry->GetReg().valid(purgeItr.first) && m_registry->GetReg().valid(purgeItr.second))
			m_eventBus->emit(
				std::make_shared<PPE::OnTriggerExitEvent>(
					purgeItr.first,
					purgeItr.second
					)
			);

		auto objects = m_triggerList.equal_range(purgeItr.first);

		for (auto it = objects.first; it != objects.second; ++it)
		{
			if ((*it).second == purgeItr.second)
			{
				m_triggerList.erase(it);
				break;
			}
		}

		break;
	}
}

/******************************************************************************/
/*!
\brief
	Should be called when exiting a ecs/changing a scene
*/
/******************************************************************************/
void PhysicsSystem::Clear()
{
	auto collidersList = m_registry->view<Components::Transform, Components::ColliderIdentifier>();
	for (auto& collidable : collidersList)
	{
		auto& collisionIdentifier = collidersList.get<Components::ColliderIdentifier>(collidable);

		collisionIdentifier.broadPhaseKey = nullptr;
	}

	m_broadphase.Clear();
	m_collisionQuery.Clear();
}

void PhysicsSystem::LoadLayers()
{
	Json::Value physicsLayers = PPU::CustomSaver::GetValueJson("CollisionLayers", false);

	if (physicsLayers["Layers"] && physicsLayers["LayerData"])
	{
		for (int i = 0; i < (int)physicsLayers["Layers"].size(); i++)
		{
			//std::cout << physicsLayers["Layers"][i].asString() << std::endl;
			CreateCollisionLayer(physicsLayers["Layers"][i].asString());
		}

		Json::Value layersData = physicsLayers["LayerData"];
		{
			for (auto itr = m_collisionLayers.cbegin(); itr != m_collisionLayers.cend(); ++itr)
			{
				if (layersData[itr->first])
				{
					Json::Value layer = layersData[itr->first];

					//layer.begin
				}
			}

			for (auto itr = layersData.begin(); itr != layersData.end(); ++itr)
			{
				Json::Value layer = *itr;

				for (auto layerItr = layer.begin(); layerItr != layer.end(); ++layerItr)
				{
					//std::cout << itr.name() << " & " << layerItr.name() << " = " << layerItr->asInt() << std::endl;
					SetCollisionRule(GetCollisionLayer(itr.name()), GetCollisionLayer(layerItr.name()), (Components::Collider::COLLISION_RULE)layerItr->asInt());
				}
			}
		}
	}
	else
	{
		Pogplant::Logger::Log(
			Pogplant::LogEntry{ "PhysicsSystem::LoadLayers", Pogplant::LogEntry::LOGTYPE::ERROR, "internal.pog has been corrupted" }, true);
	}
}

void PhysicsSystem::SaveLayers()
{
	Json::Value physicsLayers;

	Json::Value layersArray{ Json::arrayValue };
	{
		for (auto itr = m_collisionLayers.cbegin(); itr != m_collisionLayers.cend(); ++itr)
		{
			layersArray.append(itr->first);
		}

		physicsLayers["Layers"] = layersArray;
	}

	Json::Value layersData;
	{
		for (auto itr = m_collisionMatrix.cbegin(); itr != m_collisionMatrix.cend(); ++itr)
		{
			Json::Value layer;
			
			for (auto layerItr = itr->second.cbegin(); layerItr != itr->second.cend(); ++layerItr)
			{
				layer[GetCollisionLayer(layerItr->first)] = layerItr->second;
			}

			layersData[GetCollisionLayer(itr->first)] = layer;
		}

		physicsLayers["LayerData"] = layersData;
	}

	PPU::CustomSaver::Append("CollisionLayers", physicsLayers, false);
	PPU::CustomSaver::Save();
}

bool PhysicsSystem::RayCastObject(const glm::vec3& pos, const glm::vec3& dir, entt::entity entityToCast)
{
	if (!m_registry->GetReg().valid(entityToCast))
	{
		std::cout << "RayCastObject: You casted an invalid entity!" << std::endl;
		return false;
	}

	PhysicsDLC::Collision::Shapes::Ray ray{ pos, dir };
	auto boxCollider = m_registry->GetReg().try_get<Components::BoxCollider>(entityToCast);
	auto sphereCollider = m_registry->GetReg().try_get<Components::SphereCollider>(entityToCast);

	if (boxCollider)
	{
		float castTime;
		return PhysicsDLC::Collision::RayAABB(ray, boxCollider->aabb, castTime);
	}
	else if (sphereCollider)
	{
		float castTime;
		return PhysicsDLC::Collision::RaySphere(ray, sphereCollider->sphere, castTime);
	}

	return false;
}

bool PhysicsSystem::SphereCastObject(const glm::vec3& pos, const glm::vec3& dir, float radius, entt::entity entityToCast)
{
	if (!m_registry->GetReg().valid(entityToCast))
	{
		std::cout << "SphereCastObject: You casted an invalid entity!" << std::endl;
		return false;
	}

	auto boxCollider = m_registry->GetReg().try_get<Components::BoxCollider>(entityToCast);
	auto sphereCollider = m_registry->GetReg().try_get<Components::SphereCollider>(entityToCast);

	if (boxCollider)
	{
		float castTime;
		return PhysicsDLC::Collision::SphereCastAABB(pos, dir, radius, boxCollider->aabb.m_min, boxCollider->aabb.m_max, castTime);
	}
	else if (sphereCollider)
	{
		float castTime;
		return PhysicsDLC::Collision::SphereCastSphere(pos, dir, radius, sphereCollider->sphere.m_pos, sphereCollider->sphere.m_radius, castTime);
	}

	return false;
}
