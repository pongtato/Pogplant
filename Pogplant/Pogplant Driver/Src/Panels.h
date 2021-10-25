#pragma once
#include <filesystem>
#include "../Src/ECS/ECS.h"
#include "ECS/entt_ecs/entt.hpp"
#include <string>
#include <vector>


namespace PogplantDriver
{

	class Panels
	{
	public:
		virtual void Init(ECS* ecs, entt::entity& current_entity) = 0;
		virtual void Render(entt::entity& current_entity) = 0;
		virtual ~Panels() {};
	protected:

		ECS* m_ECS = nullptr;
		entt::entity m_CurrentEntity = entt::null;
	};
}
namespace PPD = PogplantDriver;
