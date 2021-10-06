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
#include "../Components/DependantComponents.h"

#include <thread>
#include <semaphore>

class ECS;

class PhysicsSystem
{
public:
	PhysicsSystem();
	~PhysicsSystem();

	void Init(ECS* ecs);
	void InitPlayState();

	void Update(float c_dt);
	void DrawColliders();
	void DrawImGUI();

	void CleanUp();

	void SetCollisionRule(int collisionLayer1, int collisionLayer2, Components::Collider::COLLISION_RULE collisionRule);
	Components::Collider::COLLISION_RULE GetCollisionRule(int collisionLayer1, int collisionLayer2);

	std::map<std::pair<int, int>, int> m_collisionMatrix;
private:
	ECS* m_registry;

	float m_gravityAcc = -9.81f;

	void TriggerUpdate();

	decltype(auto) GetTriggered(entt::entity c_triggerEntity, entt::entity c_triggeringEntity);
	void SetTrigger(entt::entity c_triggerEntity, entt::entity c_triggeringEntity);
	bool SetUntrigger(entt::entity c_triggerEntity, entt::entity c_triggeringEntity);

	std::binary_semaphore m_hasJob;
	std::atomic<bool> t_EXIT_THREADS;
	std::vector<std::thread> m_threads;

	std::unordered_multimap<entt::entity, entt::entity> m_triggerList;
};

#endif // !PHYSICSSYSTEM_H_
