#ifndef GAMESCRIPT_H_
#define GAMESCRIPT_H_

#include <gtc/type_ptr.hpp>

namespace Scripting
{
	// Only checking the bound for player to it's parent and will not work anywhere else
	int CheckBounds(glm::vec3 _Position);

	void FirePlayerBullet(glm::vec3 _Position, glm::vec3 _Rotation);
}

#endif