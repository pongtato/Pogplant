#include "../Components/Components.h"
#include "../Components/PhysicsComponents.h"

#include "PhysicsSystem.h"
#include "../Entity.h"

#include <PhysicsDLC.h>

#include <imgui.h>

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
	t_EXIT_THREADS = true;

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
	auto colliders = m_registry->GetReg().view<Components::Transform, Components::BoxCollider>()
		| m_registry->GetReg().view<Components::SphereCollider>();

	for (auto collidable : colliders)
	{
		auto& transform = colliders.get<Components::Transform>(collidable);

		auto boxCollider = m_registry->GetReg().try_get<Components::BoxCollider>(collidable);
		auto sphereCollider = m_registry->GetReg().try_get<Components::SphereCollider>(collidable);

		if (boxCollider)
		{
			boxCollider->colliderType = Components::Collider::COLLIDER_TYPE::CT_BOX;
			boxCollider->aabb.m_min = transform.m_position + boxCollider->centre - boxCollider->extends;
			boxCollider->aabb.m_max = transform.m_position + boxCollider->centre + boxCollider->extends;
		}

		if (sphereCollider)
		{
			sphereCollider->colliderType = Components::Collider::COLLIDER_TYPE::CT_SPHERE;
			sphereCollider->sphere.m_pos = transform.m_position + sphereCollider->centre;
			sphereCollider->sphere.m_radius = sphereCollider->radius;
		}
	}
}

void PhysicsSystem::TriggerUpdate()
{
	while (!t_EXIT_THREADS)
	{
		//perform busy wait LOL
		if (m_hasJob.try_acquire())
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
						SetUntrigger(_1entity, _2entity);
					}
				}
			}
		}
	}
}

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

		_1collider.aabb.m_min = _1transform.m_position + _1collider.centre - _1collider.extends * _1transform.m_scale;
		_1collider.aabb.m_max = _1transform.m_position + _1collider.centre + _1collider.extends * _1transform.m_scale;

		_1rigidbody.velocity += _1rigidbody.acceleration * c_dt;
		_1rigidbody.acceleration = PhysicsDLC::Vector::Zero;
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
					|| _1collider.isTrigger)
					continue;

				auto _2rigidbody = m_registry->GetReg().try_get<Components::Rigidbody>(_2entity);
				
				//auto& _2transform = collidableEntities.get<Components::Transform>(_2entity);
				if (PhysicsDLC::Collision::StaticAABBAABB(_1collider.aabb, _2collider.aabb))
				{
					PhysicsDLC::Physics::ResolveAABBDynamic(_1transform.m_position, _1rigidbody, _2rigidbody, _1collider.aabb, _2collider.aabb, 0.f, c_dt);
				}
				else
				{
					float collisionTime = 01.f;
					if(_2rigidbody)
						collisionTime = PhysicsDLC::Collision::DynamicAABBAABB(_1collider.aabb, _1rigidbody.velocity, _2collider.aabb, _2rigidbody->velocity, c_dt);
					else
						collisionTime = PhysicsDLC::Collision::DynamicAABBAABB(_1collider.aabb, _1rigidbody.velocity, _2collider.aabb, PhysicsDLC::Vector::Zero, c_dt);
					
					if(collisionTime > std::numeric_limits<float>::epsilon() && collisionTime < c_dt)
						PhysicsDLC::Physics::ResolveAABBDynamic(_1transform.m_position, _1rigidbody, _2rigidbody, _1collider.aabb, _2collider.aabb, collisionTime, c_dt);
				}
			}
		}

		_1transform.m_position = _1rigidbody.newPosition;
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
	std::cout << "OnTriggerEnter: "
		<< (uint32_t)c_triggerEntity << " "
		<< (uint32_t)c_triggeringEntity << std::endl;

	m_triggerList.insert(std::make_pair(c_triggerEntity, c_triggeringEntity));
}

void PhysicsSystem::SetUntrigger(entt::entity c_triggerEntity, entt::entity c_triggeringEntity)
{
	//Call ontriggerexit function here
	std::cout << "OnTriggerExit: "
		<< (uint32_t)c_triggerEntity << " "
		<< (uint32_t)c_triggeringEntity << std::endl;

	auto objects = m_triggerList.equal_range(c_triggerEntity);

	for (auto it = objects.first; it != objects.second; ++it)
	{
		if ((*it).second == c_triggeringEntity)
		{
			m_triggerList.erase(it);
			return;
		}
	}
}
