#ifndef GAMESCRIPT_H_
#define GAMESCRIPT_H_

#include <gtc/type_ptr.hpp>
#include "Events/Events.h"

namespace Scripting
{
	struct GameplayECS
	{
		static ECS* m_GameScriptECS;
	};

	// Only checking the bound for player to it's parent and will not work anywhere else
	int CheckBounds(glm::vec3 _Position);

	void FirePlayerBullet(glm::vec3 _Position, glm::vec3 _Rotation);

	void OnTriggerEnterEvent(std::shared_ptr<PPE::OnTriggerEnterEvent> onTriggerEnterEvent);
}

#endif