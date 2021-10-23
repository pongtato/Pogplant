#pragma once
#include <filesystem>
#include "../Src/ECS/ECS.h"
#include "ECS/entt_ecs/entt.hpp"


namespace PogplantDriver
{

	class SceneHierarchy
	{
	public:
		SceneHierarchy();
		void Init(ECS*  ecs, entt::entity& current_entity);
		void RenderSceneHierarchy(entt::entity& current_entity);

		bool DrawEntityNode(entt::entity  entity, bool draw_childen = false);
		static void SavePrefab(entt::entity _object);
		static void LoadPrefab();
	private:
		ECS* m_ECS;
		entt::entity m_CurrentEntity;

	};
}
namespace PPD = PogplantDriver;
