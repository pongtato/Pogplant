#include "Pogplant.h"
#include "../Src/ImguiHelper.h"
#include "../Src/ECS/Components/Components.h"
#include "../Src/ECS/Components/DependantComponents.h"



namespace PPD = PogplantDriver;
using namespace Components;
namespace Scripting
{
	// Only checking the bound for player to it's parent and will not work anywhere else
	int CheckBounds(glm::vec3 _Position)
	{
		std::string parent{ "PlayerBox" };
		entt::entity parent_box = PPD::ImguiHelper::m_ecs->FindEntityWithName(parent);
		if (parent_box != entt::null)
		{
			auto boxcollider_comp = PPD::ImguiHelper::m_ecs->GetReg().try_get<BoxCollider>(parent_box);
			glm::vec3 max_bound = boxcollider_comp->extends;
			glm::vec3 min_bound = -(max_bound);
			int value = 0;
			if (_Position.x > max_bound.x || _Position.x < min_bound.x)
				value |= 1;
			if (_Position.y > max_bound.y || _Position.y < min_bound.y)
				value |= 2;
			return value;
		}
		return 0;
	}
}
