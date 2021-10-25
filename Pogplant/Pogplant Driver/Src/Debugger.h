#pragma once
#include "Panels.h"


namespace PogplantDriver
{

	class Debugger : public Panels
	{
	public:
		void Init(ECS*  ecs, entt::entity& current_entity) override;
		void Render(entt::entity& current_entity) override;
	};
}
namespace PPD = PogplantDriver;
