#ifndef GAMESCRIPT_H_
#define GAMESCRIPT_H_

#include <gtc/type_ptr.hpp>
#include "Events/Events.h"

#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>

namespace Scripting
{
	struct GameplayECS
	{
		static ECS* m_GameScriptECS;
	};

	//Wrapper for instantiate due to monostring calls
	std::uint32_t Instantiate(MonoString* name , glm::vec3 _Position, glm::vec3 _Rotation);

	int CheckBounds(glm::vec3& _Position, glm::vec3& _Velocity);

	//Only works via checking for  "name" PlayerCam together with PlayerScript
	void FollowPlayerCam(glm::vec3 _Position, glm::vec3 _Rotation);

	void FirePlayerBullet(std::uint32_t entityID, glm::vec3 _Position, glm::vec3 _Rotation);

	void FireEnemyBullet(std::uint32_t entityID, glm::vec3 _Position, glm::vec3 _Rotation, bool isTrue = false);

	void OnTriggerEnterEvent(std::shared_ptr<PPE::OnTriggerEnterEvent> onTriggerEnterEvent);
}

#endif