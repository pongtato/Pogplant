#ifndef EVENTS_H_
#define EVENTS_H_

#include "EventSystem.h"
#include "../ECS/ECS.h"

namespace PPE
{
	//All events should inherit from BaseEvent

	struct OnTriggerEnterEvent : public BaseEvent
	{
		OnTriggerEnterEvent(entt::entity entity1, entt::entity entity2)
			: m_entity1{ entity1 }, m_entity2{ entity2 }
		{

		}

		entt::entity m_entity1;
		entt::entity m_entity2;
	};

	struct OnTriggerExitEvent : public BaseEvent
	{
		OnTriggerExitEvent(entt::entity entity1, entt::entity entity2)
			: m_entity1{ entity1 }, m_entity2{ entity2 }
		{

		}

		entt::entity m_entity1;
		entt::entity m_entity2;
	};
}

#endif // !EVENTS_H_
