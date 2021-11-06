#include "Pogplant.h"

#include "../Src/ImguiHelper.h"
#include "../Src/ECS/Entity.h"
#include "../Src/ECS/Components/Components.h"
#include "../Src/ECS/Components/DependantComponents.h"
#include "../Src/ECS/Systems/ScriptSystemHelper.h"
#include "../Src/Serialiser/Serializer.h"
#include "GameScript.h"

ECS* Scripting::GameplayECS::m_GameScriptECS;
namespace PPD = PogplantDriver;
using namespace Components;
namespace Scripting
{
	std::uint32_t Instantiate(MonoString* name, glm::vec3 _Position, glm::vec3 _Rotation)
	{
		std::string _name = mono_string_to_utf8(name);

		PogplantDriver::Serializer serial(*GameplayECS::m_GameScriptECS);
		entt::entity new_entitiy = serial.Instantiate(_name, _Position, _Rotation);

		return std::uint32_t(new_entitiy);
	}


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

	void FollowPlayerCam(glm::vec3 _Position, glm::vec3 _Rotation)
	{
		//Offset the camera based on the box world position
		entt::entity player_cam = GameplayECS::m_GameScriptECS->FindEntityWithName("PlayerCam");
		entt::entity player_box = GameplayECS::m_GameScriptECS->FindEntityWithName("PlayerBox");
		if (player_cam != entt::null && player_box != entt::null)
		{
			auto box_pos = GameplayECS::m_GameScriptECS->GetReg().try_get<Transform>(player_box);
			auto box_collider = GameplayECS::m_GameScriptECS->GetReg().try_get<BoxCollider>(player_box);
			//Natural Z forward offset
			glm::vec3 offset = glm::vec3{ 0,0,3.f };
			//Concatanate the offsets
			offset += _Position;

		  auto cam_pos =  GameplayECS::m_GameScriptECS->GetReg().try_get<Transform>(player_cam);
			cam_pos->m_position = box_pos->m_position + offset;

			auto cam_comp = GameplayECS::m_GameScriptECS->GetReg().try_get<Camera>(player_cam);
			//Account for the offset 0.01f that is hardcoded

			//Hardcoded until i find a better way to save the inspector camera variables
			cam_comp->m_Yaw = box_pos->m_rotation.y +  90.f + -_Rotation.y - 0.01f;
			cam_comp->m_Pitch  = box_pos->m_rotation.x +  0 + -_Rotation.x;			


		}
	}

	void FirePlayerBullet(std::uint32_t entityID, glm::vec3 _Position, glm::vec3 _Rotation)
	{
		//Need to find player ship transform 
		entt::entity player_ship = static_cast<entt::entity>(entityID);

		glm::mat4 decompose{ 0 };
		glm::vec3 position{ 0 };
		if (player_ship != entt::null)
		{
			auto ship_trans = GameplayECS::m_GameScriptECS->GetReg().try_get<Transform>(player_ship);
			decompose = ship_trans->m_ModelMtx; // Modlemtx = world.
			position.x = decompose[3][0];
			position.y = decompose[3][1];
			position.z = decompose[3][2];

			decompose[3][0] = 0.f;
			decompose[3][1] = 0.f;
			decompose[3][2] = 0.f;
			float scale_x = glm::length(decompose[0]);
			float scale_y = glm::length(decompose[1]);
			float scale_z = glm::length(decompose[2]);

			decompose[0][0] /= scale_x;
			decompose[0][1] /= scale_x;
			decompose[0][2] /= scale_x;

			decompose[1][0] /= scale_y;
			decompose[1][1] /= scale_y;
			decompose[1][2] /= scale_y;

			decompose[2][0] /= scale_z;
			decompose[2][1] /= scale_z;
			decompose[2][2] /= scale_z;
		}
		//std::cout << "Position" << position.x << ", " << position.y << ", " << position.z << std::endl;
		PogplantDriver::Serializer serial( *GameplayECS::m_GameScriptECS );

		entt::entity bullet = serial.Instantiate("Bullet", position, _Rotation);
		GameplayECS::m_GameScriptECS->GetReg().emplace<Projectile>(bullet, 3.f, 10.f, Components::Projectile::OwnerType::Player);

			//auto& sp_collider = new_bullet.AddComponent<BoxCollider>();
			//auto& identi = new_bullet.AddComponent<Components::ColliderIdentifier>();
			//identi.colliderType = ColliderIdentifier::COLLIDER_TYPE::CT_BOX;
			//identi.isTrigger = true;
			//sp_collider.isTrigger = true;
			//new_bullet.AddComponent<Rigidbody>(1.f);



		auto body = GameplayECS::m_GameScriptECS->GetReg().try_get<Rigidbody>(bullet);
		//Hardcoded for now
		glm::vec4 forward{ 0.f,0.f,1.f ,1.f};
		glm::vec3 forward_vec = decompose * forward;
		//std::cout << "Forward_vec" << forward_vec.x << ", " << forward_vec.y << ", " << forward_vec.z << std::endl;
		//Add power to the shots
		forward_vec *= 100.f; 
		body->AddImpulseForce(forward_vec);

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
		const auto& player_projectile_script = GameplayECS::m_GameScriptECS->GetReg().try_get<Components::Projectile>(object);
		const auto& enemy_object_script = GameplayECS::m_GameScriptECS->GetReg().try_get<Components::Scriptable>(other);

		if (player_projectile_script && enemy_object_script)
		{
			bool enemy = enemy_object_script->m_ScriptTypes.contains("BaseTurret");
			if (player_projectile_script->m_Ownertype == Projectile::OwnerType::Player && enemy)
			{
				GameplayECS::m_GameScriptECS->DestroyEntity(object);
				// Should be able to call CallTakeDamageFunction(player_projectile_script->damage, other) here
				SSH::InvokeFunction("BaseTurret", "TakeDamage", other, player_projectile_script->m_Damage);
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
