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
		auto& identi = new_bullet.AddComponent<Components::ColliderIdentifier>();
		identi.colliderType = ColliderIdentifier::COLLIDER_TYPE::CT_BOX;
		identi.isTrigger = true;
		sp_collider.isTrigger = true;
		new_bullet.AddComponent<Rigidbody>(1.f);

		auto& body = new_bullet.GetComponent<Rigidbody>();
		body.AddImpulseForce({ 0.f,0.f,10.f });

	}
	void FireEnemyBullet(glm::vec3 _Position, glm::vec3 _Rotation)
	{
		auto new_bullet = GameplayECS::m_GameScriptECS->CreateEntity("Bullet", _Position, _Rotation);
		new_bullet.AddComponent<Projectile>(3.f, 10.f, Components::Projectile::OwnerType::Enemy);
		new_bullet.GetComponent<Projectile>().m_Type = Projectile::ProjectileType::False;


		new_bullet.AddComponent<Renderer>(glm::vec3{ 1.0f }, glm::vec3{ 1.0f }, PP::ModelResource::m_ModelPool["sphere"], &PP::ModelResource::m_ModelPool["sphere"]->m_Meshes.begin()->second);
		auto& sp_collider = new_bullet.AddComponent<BoxCollider>();
		auto& col_identifier = new_bullet.AddComponent<Components::ColliderIdentifier>();

		col_identifier.colliderType = ColliderIdentifier::COLLIDER_TYPE::CT_BOX;
		col_identifier.isTrigger = true;
		sp_collider.isTrigger = true;

		new_bullet.AddComponent<Rigidbody>(1.f);

		auto& body = new_bullet.GetComponent<Rigidbody>();
		body.AddImpulseForce({ 0.f,0.f,10.f });

	}

	void PlayerProjectileCollision(entt::entity& object, entt::entity& other)
	{
		auto player_projectile_script = GameplayECS::m_GameScriptECS->GetReg().try_get<Components::Projectile>(object);
		auto enemy_object_script = GameplayECS::m_GameScriptECS->GetReg().try_get<Components::Scriptable>(other);

		if (player_projectile_script && enemy_object_script)
		{
			bool enemy = enemy_object_script->m_ScriptTypes.contains("EnemyScript");
			if (player_projectile_script->m_Ownertype == Projectile::OwnerType::Player && enemy)
			{
				GameplayECS::m_GameScriptECS->DestroyEntity(object);
			}
		}
	}

	void OnTriggerEnterEvent(std::shared_ptr<PPE::OnTriggerEnterEvent> onTriggerEnterEvent)
	{
		auto& object = onTriggerEnterEvent->m_entity1;
		auto& other = onTriggerEnterEvent->m_entity2;

		PlayerProjectileCollision(object, other);
	}
}
