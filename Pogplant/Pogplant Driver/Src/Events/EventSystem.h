/******************************************************************************/
/*!
\file	EventSystem.h
\author Ng Tian Kiat
\par	email: tiankiat.ng/@digipen.edu
\details
	This file contains classes needed for an Event system implementation
* \copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
				or disclosure of this file or its contents without the prior
				written consent of DigiPen Institute of Technology is prohibited.
!*/
/******************************************************************************/
#ifndef EVENTSYSTEM_H
#define EVENTSYSTEM_H

#include <list>
#include <map>
#include <typeindex>
#include <memory>

namespace PPE
{
	class BaseEvent
	{

	};

	// Declare a base interface for EventHandlers.
	class BaseEventHandler
	{
	public:
		using EventPtr = std::shared_ptr<BaseEvent>;

		void execute(EventPtr event)
		{
			call(event);
		}
		virtual ~BaseEventHandler() = default;
	private:
		virtual void call(EventPtr event) = 0;
	};

	// Specialized EventHandlers that takes in class member functions as callbacks.
	template <typename T, class EventType>
	class MemberFnEventHandler : public BaseEventHandler
	{
	public:
		using Callback = void(T::*)(std::shared_ptr<EventType>);

		MemberFnEventHandler(T* instance, Callback callback)
			: m_instance{ instance }, m_callback{ callback }
		{
		}
		~MemberFnEventHandler() = default;
		virtual void call(EventPtr event) override
		{
			(m_instance->*m_callback)(std::static_pointer_cast<EventType>(event));
		}

	private:
		T* m_instance;

		Callback m_callback;
	};

	// Specialized EventHandlers that takes in static functions as callbacks.
	template < class EventType>
	class StaticFnEventHandler : public BaseEventHandler
	{
	public:
		using Callback = void(*)(std::shared_ptr<EventType>);

		StaticFnEventHandler(Callback callback)
			: m_callback{ callback }
		{
		}
		~StaticFnEventHandler() = default;
		virtual void call(EventPtr event) override
		{
			(*m_callback)(std::static_pointer_cast<EventType>(event));
		}

	private:
		Callback m_callback;
	};

	// This EventBus class registers EventHandlers and handles events.
	class EventBus
	{
	public:
		using EventHandlerList = std::list<std::shared_ptr<BaseEventHandler>>;
		using EventHandlerListPtr = std::shared_ptr<EventHandlerList>;
		template <typename EventType>
		// This function is called when an event needs to be sent, the event is immediately handled by
		// EventHandlers that is listening to that event type.
		void emit(std::shared_ptr<EventType> event)
		{
			EventHandlerListPtr event_handlers = m_eventListeners[typeid(EventType)];

			assert(event_handlers != nullptr);

			if (!event_handlers) return;

			for (auto& event_handler : *event_handlers)
			{
				if (event_handler)
					event_handler->execute(event);
			}
		}

		// This function registers an EventHandler for a certain event type.
		template <typename T, class EventType>
		void listen(T* instance, void (T::* callback)(std::shared_ptr<EventType>))
		{
			EventHandlerListPtr event_handlers = m_eventListeners[typeid(EventType)];

			// if event handlers list is empty initialize a new list
			if (event_handlers == nullptr)
			{
				event_handlers = std::make_shared<EventHandlerList>();
				m_eventListeners[typeid(EventType)] = event_handlers;
			}

			event_handlers->push_back(std::make_shared<MemberFnEventHandler<T, EventType>>(instance, callback));
		}

		// This function registers an EventHandler for a certain event type.
		template <class EventType>
		void listen(void (*callback)(std::shared_ptr<EventType>))
		{
			EventHandlerListPtr event_handlers = m_eventListeners[typeid(EventType)];

			// if event handlers list is empty initialize a new list
			if (event_handlers == nullptr)
			{
				event_handlers = std::make_shared<EventHandlerList>();
				m_eventListeners[typeid(EventType)] = event_handlers;
			}

			event_handlers->push_back(std::make_shared<StaticFnEventHandler<EventType>>(callback));
		}

		EventBus() = default;
	private:
		std::map<std::type_index, EventHandlerListPtr> m_eventListeners;
	};
}

#endif

