#pragma once
#include <filesystem>
#include "Panels.h"
#include "../Src/ECS/ECS.h"
#include "ECS/entt_ecs/entt.hpp"


namespace PogplantDriver
{
	class SceneHierarchy : public Panels
	{
	public:
		SceneHierarchy() = default;
		void Init(ECS* ecs, entt::entity& current_entity) override;
		void Render(entt::entity& current_entity) override;

		bool DrawEntityNode(entt::entity  entity, bool draw_childen = false);
		void SavePrefab(entt::entity _object);
	  void LoadPrefab();

	};
}
namespace PPD = PogplantDriver;
