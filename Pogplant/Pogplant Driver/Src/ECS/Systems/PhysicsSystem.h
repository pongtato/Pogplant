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
	void DrawImGUI();

	void CleanUp();

	void SetCollisionRule(int collisionLayer1, int collisionLayer2, Components::Collider::COLLISION_RULE collisionRule);
	Components::Collider::COLLISION_RULE GetCollisionRule(int collisionLayer1, int collisionLayer2);

	std::map<std::pair<int, int>, int> m_collisionMatrix;
private:
	ECS* m_registry;
	std::shared_ptr<PPE::EventBus> m_eventBus;

	float m_gravityAcc = -9.81f;

	void TriggerUpdate();

	void HandleCollision(const entt::entity& c_1entity,
		const entt::entity& c_2entity,
		Components::Transform& c_1transform,
		Components::Transform& c_2transform,
		Components::Rigidbody& c_1rigidbody,
		Components::Rigidbody* c_2rigidbody,
		const Components::ColliderIdentifier& c_1colliderIdentifier,
		const Components::ColliderIdentifier& c_2colliderIdentifier,
		float c_dt);

	decltype(auto) GetTriggered(entt::entity c_triggerEntity, entt::entity c_triggeringEntity);
	void SetTrigger(entt::entity c_triggerEntity, entt::entity c_triggeringEntity);
	bool SetUntrigger(entt::entity c_triggerEntity, entt::entity c_triggeringEntity);

	std::binary_semaphore m_hasJob;
	std::atomic<bool> t_EXIT_THREADS;
	std::vector<std::thread> m_threads;

	std::unordered_multimap<entt::entity, entt::entity> m_triggerList;
};

#endif // !PHYSICSSYSTEM_H_
