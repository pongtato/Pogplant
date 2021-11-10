#pragma once
#include <filesystem>
#include "Panels.h"
#include "../Src/ECS/ECS.h"
#include "ECS/entt_ecs/entt.hpp"
#include "../Src/Model.h"
#include "../Src/Mesh3D.h"


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
		void LoadModel();

		//test code for GUID prefab system
		void LoadPrefab2();
	private:
		static bool m_Loading;
		void ConstructModel
		(
			Entity& _Entity,
			Pogplant::Model* _Model,
			Pogplant::Mesh3D* _Mesh3D,
			const glm::vec3& _Color = glm::vec3{ 1 },
			const glm::vec3& _Emissive = glm::vec3{ 1 },
			bool _UseLight = true,
			bool _EditorOnly = false,
			bool _FirstIt = true
		);
	};
}
namespace PPD = PogplantDriver;
