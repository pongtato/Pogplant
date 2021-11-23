/******************************************************************************/
/*!
\file	PhysicsSystem.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	
	System to handle components with colliders and rigidbodies

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef PHYSICSSYSTEM_H_
#define PHYSICSSYSTEM_H_

#include "../ECS.h"
#include "../Components/Components.h"
#include "../Components/DependantComponents.h"
#include "../../Events/Events.h"

#include <thread>
#include <semaphore>
#include <mutex>
#include <tuple>

//Defines the number of threads that should be used to do triggers
#define NUM_TRIGGER_THREADS 3

class ECS;

class PhysicsSystem
{
public:
	PhysicsSystem();
	~PhysicsSystem();

	void Init(ECS* ecs, std::shared_ptr<PPE::EventBus>& eventBus);
	void InitPlayState();

	void UpdateEditor();
	void Update(float c_dt);
	void DrawColliders();

	void CleanUp();

	void SetCollisionRule(int collisionLayer1, int collisionLayer2, Components::Collider::COLLISION_RULE collisionRule);
	Components::Collider::COLLISION_RULE GetCollisionRule(int collisionLayer1, int collisionLayer2);

	int CreateCollisionLayer(const std::string& name);
	void DestroyCollisionLayer(const std::string& name);

	int GetCollisionLayer(const std::string& layerName);

	void Clear();

	inline void RemoveEntityFromTree(const entt::entity& entity)
	{
		m_broadphase.RemoveData(entity);
	}

	std::unordered_map<int, std::unordered_map<int, int>> m_collisionMatrix;
	std::map<std::string, int> m_collisionLayers;
private:
	struct QueuedTriggerAction
	{
		entt::entity entity1;
		entt::entity entity2;
		bool onEnter;
	};


	ECS* m_registry;
	std::shared_ptr<PPE::EventBus> m_eventBus;

	float m_gravityAcc = -9.81f;

	

	void UpdateMovingObjects();

	/**************************
	*
	* Collision
	*
	**************************/
	PhysicsDLC::Broadphase::DynamicAABBTree<entt::entity> m_broadphase;
	PhysicsDLC::Broadphase::BroadphaseQuery<entt::entity> m_collisionQuery;


	void HandleCollision(const entt::entity& c_1entity,
		const entt::entity& c_2entity,
		Components::Transform& c_1transform,
		Components::Transform& c_2transform,
		Components::Rigidbody& c_1rigidbody,
		Components::Rigidbody* c_2rigidbody,
		const Components::ColliderIdentifier& c_1colliderIdentifier,
		const Components::ColliderIdentifier& c_2colliderIdentifier,
		float c_dt);


	/**************************
	*
	* Threading
	*
	**************************/
	std::atomic<bool> t_EXIT_THREADS;
	std::vector<std::thread> m_threads;
	std::unique_ptr<std::binary_semaphore> m_hasTriggerJob[NUM_TRIGGER_THREADS];
	std::unique_ptr<std::binary_semaphore> m_shouldContinue[NUM_TRIGGER_THREADS];
	std::mutex m_mTriggerQueueMutex;

	/**************************
	*
	* Trigger stuff
	*
	**************************/

	void TriggerUpdate(int threadID);

	decltype(auto) GetTriggered(entt::entity c_triggerEntity, entt::entity c_triggeringEntity);
	void SetTrigger(entt::entity c_triggerEntity, entt::entity c_triggeringEntity);
	bool SetUntrigger(entt::entity c_triggerEntity, entt::entity c_triggeringEntity);
	
	std::vector<QueuedTriggerAction> m_triggerQueue;
	

	std::unordered_multimap<entt::entity, entt::entity> m_triggerList;
};

#endif // !PHYSICSSYSTEM_H_
