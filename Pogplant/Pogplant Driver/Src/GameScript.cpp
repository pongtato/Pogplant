#include "Pogplant.h"
#include "../Src/ImguiHelper.h"
#include "../Src/ECS/Entity.h"
#include "../Src/ECS/Components/Components.h"
#include "../Src/ECS/Components/DependantComponents.h"
#include "GameScript.h"


ECS* Scripting::GameplayECS::m_GameScriptECS;
namespace PPD = PogplantDriver;
using namespace Components;
namespace Scripting
{
	// Only checking the bound for player to it's parent and will not work anywhere else
	int CheckBounds(glm::vec3 _Position)
	{
		std::string parent{ "PlayerBox" };
		entt::entity parent_box = GameplayECS::m_GameScriptECS->FindEntityWithName(parent);
		if (parent_box != entt::null)
		{
			auto boxcollider_comp = GameplayECS::m_GameScriptECS->GetReg().try_get<BoxCollider>(parent_box);
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
	void FirePlayerBullet(glm::vec3 _Position, glm::vec3 _Rotation)
	{
		auto new_bullet = GameplayECS::m_GameScriptECS->CreateEntity("Bullet", _Position, _Rotation);
		new_bullet.AddComponent<Projectile>(3.f, 10.f, Components::Projectile::OwnerType::Player);
		new_bullet.AddComponent<Renderer>(glm::vec3{ 1.0f }, glm::vec3{ 1.0f }, PP::ModelResource::m_ModelPool["sphere"], &PP::ModelResource::m_ModelPool["sphere"]->m_Meshes.begin()->second);
		auto& sp_collider = new_bullet.AddComponent<BoxCollider>();
		auto&identi = new_bullet.AddComponent<Components::ColliderIdentifier>();
		identi.colliderType = ColliderIdentifier::COLLIDER_TYPE::CT_BOX;
		identi.isTrigger = true;
		sp_collider.isTrigger = true;
		new_bullet.AddComponent<Rigidbody>(1.f);

		auto& body = new_bullet.GetComponent<Rigidbody>();
		body.AddImpulseForce({ 0.f,0.f,10.f });

	}
	void OnTriggerEnterEvent(std::shared_ptr<PPE::OnTriggerEnterEvent> onTriggerEnterEvent)
	{
		auto enter1 = GameplayECS::m_GameScriptECS->GetReg().try_get<Components::Projectile>(onTriggerEnterEvent->m_entity1);
		auto enter2 = GameplayECS::m_GameScriptECS->GetReg().try_get<Components::Scriptable>(onTriggerEnterEvent->m_entity2);

		if (enter1 && enter2)
		{
			bool enemy = enter2->m_ScriptTypes.contains("EnemyScript");
			if (enter1->m_Ownertype == Projectile::OwnerType::Player && enemy)
			{
				GameplayECS::m_GameScriptECS->DestroyEntity(onTriggerEnterEvent->m_entity1);
			}
		}









	}

}
