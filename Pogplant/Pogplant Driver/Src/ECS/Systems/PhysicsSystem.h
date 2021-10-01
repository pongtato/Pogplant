#ifndef PHYSICSSYSTEM_H_
#define PHYSICSSYSTEM_H_

#include "../ECS.h"
#include "../Components/PhysicsComponents.h"

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
	void DrawImGUI();

	void SetCollisionRule(int collisionLayer1, int collisionLayer2, Components::Collider::COLLISION_RULE collisionRule);
	Components::Collider::COLLISION_RULE GetCollisionRule(int collisionLayer1, int collisionLayer2);

	std::map<std::pair<int, int>, int> m_collisionMatrix;
private:
	ECS* m_registry;

	void TriggerUpdate();

	decltype(auto) GetTriggered(entt::entity c_triggerEntity, entt::entity c_triggeringEntity);
	void SetTrigger(entt::entity c_triggerEntity, entt::entity c_triggeringEntity);
	void SetUntrigger(entt::entity c_triggerEntity, entt::entity c_triggeringEntity);

	std::binary_semaphore m_hasJob;
	std::atomic<bool> t_EXIT_THREADS;
	std::vector<std::thread> m_threads;

	std::unordered_multimap<entt::entity, entt::entity> m_triggerList;
};

#endif // !PHYSICSSYSTEM_H_
