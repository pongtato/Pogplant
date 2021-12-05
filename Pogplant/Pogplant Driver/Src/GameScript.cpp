#include "Pogplant.h"

#include "../Src/Input/GLFWInput.h"
#include "../Src/ImguiHelper.h"
#include "../Src/ECS/Entity.h"
#include "../Src/ECS/Components/Components.h"
#include "../Src/ECS/Components/DependantComponents.h"
#include "../Src/ECS/Systems/ScriptSystemHelper.h"
#include "../Src/Serialiser/Serializer.h"
#include "GameScript.h"
#include "Application.h"
#include <random>

namespace PPD = PogplantDriver;
using namespace Components;
namespace Scripting
{
	void GetMousePos(float& _X, float& _Y)
	{
		double castX = 0;
		double castY = 0;
		PPI::GLFWInputManager::Instance().getMouse(castX, castY);
		_X = static_cast<float>(castX);
		_Y = static_cast<float>(castY);
	}

	std::uint32_t Instantiate(MonoString* name, glm::vec3 _Position, glm::vec3 _Rotation)
	{
		std::string _name = mono_string_to_utf8(name);

		PogplantDriver::Serializer serial(*PogplantDriver::Application::GetInstance().m_activeECS);
		entt::entity new_entitiy = serial.Instantiate(_name, _Position, _Rotation);

		return std::uint32_t(new_entitiy);
	}


	// Only checking the bound for player to it's parent and will not work anywhere else
	int CheckBounds(glm::vec3& _Position, glm::vec3& _Velocity)
	{
		//std::cout << "Player Health Is: " << GetPlayerHealth() << std::endl;
		std::string parent{ "PlayerBox" };
		entt::entity parent_box = PogplantDriver::Application::GetInstance().m_activeECS->FindEntityWithName(parent);
		if (parent_box != entt::null)
		{
			//GET AND COMPARE GLOBAL DUE TO LOCAL FUNKYNESS
			auto boxcollider_comp = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<BoxCollider>(parent_box);
			int value = 0;
			glm::vec3 future_pos = _Position + _Velocity;
			if (future_pos.x > boxcollider_comp->aabb.m_max.x || future_pos.x < boxcollider_comp->aabb.m_min.x)
				value |= 1;
			if (future_pos.y > boxcollider_comp->aabb.m_max.y || future_pos.y < boxcollider_comp->aabb.m_min.y)
				value |= 2;
			if (future_pos.z > boxcollider_comp->aabb.m_max.z || future_pos.z < boxcollider_comp->aabb.m_min.z)
				value |= 4;
			return value;
		}
		return 0;
	}

	// Position is the player local position, Rotation is player local rotation
	void FollowPlayerCam(glm::vec3 _Position, glm::vec3 _Rotation, float _deltaTime)
	{
		//Offset the camera based on the box world position
		entt::entity player_cam = PogplantDriver::Application::GetInstance().m_activeECS->FindEntityWithName("PlayerCam");
		entt::entity player_box = PogplantDriver::Application::GetInstance().m_activeECS->FindEntityWithName("PlayerBox");
		entt::entity playerShip = PogplantDriver::Application::GetInstance().m_activeECS->FindEntityWithName("PlayerShip");
		if (player_cam != entt::null && player_box != entt::null)
		{
			auto box_pos = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Transform>(player_box);
			//std::cout << "Rotation: " << box_pos->m_rotation.x << ", " << box_pos->m_rotation.y << ", " << box_pos->m_rotation.z << std::endl;

			auto playerTransform = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Transform>(playerShip);

			auto cam_comp = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Camera>(player_cam);
			//Account for the offset 0.01f that is hardcoded

			//Hardcoded until i find a better way to save the inspector camera variables
			//cam_comp->m_Yaw = -box_pos->m_rotation.y + 90.f + -_Rotation.y;
			//cam_comp->m_Pitch  = -box_pos->m_rotation.x +  0 + -_Rotation.x;



			//Turns the camera left,right (-180, 180);
			glm::vec3 rotationTarget
			{
				(-box_pos->m_rotation.x - playerTransform->m_rotation.x),
				(box_pos->m_rotation.y + playerTransform->m_rotation.y) + 180.f,
				-playerTransform->m_rotation.z
			};

			if (cam_comp->m_Yaw - rotationTarget.y > 180)
				cam_comp->m_Yaw -= 360.f;
			else if (cam_comp->m_Yaw - rotationTarget.y < -180)
				cam_comp->m_Yaw += 360.f;

			if (cam_comp->m_Pitch - rotationTarget.x > 180)
				cam_comp->m_Pitch -= 360.f;
			else if (cam_comp->m_Pitch - rotationTarget.x < -180)
				cam_comp->m_Pitch += 360.f;

			if (cam_comp->m_Roll - rotationTarget.z > 180)
				cam_comp->m_Roll -= 360.f;
			else if (cam_comp->m_Roll - rotationTarget.z < -180)
				cam_comp->m_Roll += 360.f;

			//Lerps yaw and pitch over time
			cam_comp->m_Yaw += (rotationTarget.y - cam_comp->m_Yaw) * _deltaTime * 20.f;
			cam_comp->m_Pitch += (rotationTarget.x - cam_comp->m_Pitch) * _deltaTime * 20.f;
			cam_comp->m_Roll += (rotationTarget.z - cam_comp->m_Roll) * _deltaTime * 20.f;
		}
	}

	void FirePlayerBullet(glm::vec3 _Position, glm::vec3 _FowardVector, glm::vec3 _Rotation)
	{
		//Need to find player ship transform 
		//entt::entity player_ship = static_cast<entt::entity>(entityID);
		//auto ship_trans = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Transform>(player_ship);
		//glm::vec3 forward = ship_trans->GetForwardVector();
		float speed = 50.f;
		PogplantDriver::Serializer serial(*PogplantDriver::Application::GetInstance().m_activeECS);
		entt::entity bullet = serial.Instantiate("Bullet", _Position, _Rotation);
		PogplantDriver::Application::GetInstance().m_activeECS->GetReg().emplace<Projectile>(bullet, 3.f, speed, Components::Projectile::OwnerType::Player);

		auto body = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Rigidbody>(bullet);

		//Add power to the shots
		glm::vec3 Powershot = _FowardVector * speed;
		body->AddImpulseForce(Powershot);

	}
	void FireEnemyBullet(std::uint32_t entityID, glm::vec3 _Position, glm::vec3 _Rotation, float _Speed, float _Lifetime, bool isTrue)
	{
		//Get enemy transform 
		entt::entity enemy = static_cast<entt::entity>(entityID);
		auto enemy_trans = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Transform>(enemy);

		PogplantDriver::Serializer serial(*PogplantDriver::Application::GetInstance().m_activeECS);
		entt::entity bullet;
		if (isTrue)
		{
			bullet = serial.Instantiate("TrueBullet", _Position, enemy_trans->GetGlobalRotation());
			//std::cout << "spawned true bullet" << std::endl;
		}
		else
		{
			bullet = serial.Instantiate("FalseBullet", _Position, enemy_trans->GetGlobalRotation());
			//std::cout << "spawned false bullet" << std::endl;
		}

		PogplantDriver::Application::GetInstance().m_activeECS->GetReg().emplace<Projectile>(bullet, _Lifetime, 1.f, Components::Projectile::OwnerType::Enemy);

		auto body = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Rigidbody>(bullet);
		//Hardcoded for now
		glm::vec3 forward_vec = enemy_trans->GetForwardVector();

		//Add power to the shots
		forward_vec *= _Speed;
		body->AddImpulseForce(forward_vec);
	}

	void SpawnStaticExplosion(glm::vec3& position, int type)
	{
		PogplantDriver::Serializer serial(*PogplantDriver::Application::GetInstance().m_activeECS);
		if (type == 0)
		{
			entt::entity vfx = serial.Instantiate("Static_Explosion", position, {0,0,0});

			// play particle systems
			entt::entity rising = PogplantDriver::Application::GetInstance().m_activeECS->FindChildEntityWithName(vfx, "Explosion_Rising");
			entt::entity smoke = PogplantDriver::Application::GetInstance().m_activeECS->FindChildEntityWithName(vfx, "Explosion_Smoke");
			entt::entity bits = PogplantDriver::Application::GetInstance().m_activeECS->FindChildEntityWithName(vfx, "Explosion_Bits");

			auto rising_ps = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::ParticleSystem>(rising);
			auto smoke_ps = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::ParticleSystem>(smoke);
			auto bits_ps = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::ParticleSystem>(bits);

			rising_ps->init();
			rising_ps->m_Play = true;

			smoke_ps->init();
			smoke_ps->m_Play = true;

			bits_ps->init();
			bits_ps->m_Trail = true;
			bits_ps->m_Play = true;
		}
		else
		{
			entt::entity vfx = serial.Instantiate("Hit_VFX", position, { 0,0,0 });

			// play particle systems
			entt::entity smoke = PogplantDriver::Application::GetInstance().m_activeECS->FindChildEntityWithName(vfx, "Explosion_Smoke");
			entt::entity bits = PogplantDriver::Application::GetInstance().m_activeECS->FindChildEntityWithName(vfx, "Explosion_Bits");

			auto smoke_ps = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::ParticleSystem>(smoke);
			auto bits_ps = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::ParticleSystem>(bits);

			smoke_ps->init();
			smoke_ps->m_Play = true;

			bits_ps->init();
			bits_ps->m_Trail = true;
			bits_ps->m_Play = true;
		}
	}


	void PlayerProjectileCollision(entt::entity& object, entt::entity& other)
	{
		if (!PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(object) || !PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(other))
			return;

		const auto& player_projectile_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Projectile>(object);
		const auto& enemy_object_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(other);

		if (player_projectile_script && enemy_object_script)
		{
			bool enemy_turret = enemy_object_script->m_ScriptTypes.contains("BaseTurret");
			if (player_projectile_script->m_Ownertype == Projectile::OwnerType::Player && enemy_turret)
			{
				PogplantDriver::Application::GetInstance().m_activeECS->DestroyEntity(object);
				// Should be able to call CallTakeDamageFunction(player_projectile_script->damage, other) here
				SSH::InvokeFunction("BaseTurret", "TakeDamage", other, player_projectile_script->m_Damage);
			}

			bool enemy_gatling = enemy_object_script->m_ScriptTypes.contains("BaseGattling");
			if (player_projectile_script->m_Ownertype == Projectile::OwnerType::Player && enemy_gatling)
			{
				PogplantDriver::Application::GetInstance().m_activeECS->DestroyEntity(object);
				// Should be able to call CallTakeDamageFunction(player_projectile_script->damage, other) here
				SSH::InvokeFunction("BaseGattling", "TakeDamage", other, player_projectile_script->m_Damage);
			}
		}
		if (player_projectile_script)
		{
			if (player_projectile_script->m_Ownertype == Projectile::OwnerType::Player)
			{
				if (PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::BoxCollider>(other) && PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::BoxCollider>(other)->collisionLayer == "ENEMY")
				{
					entt::entity playerbox = PogplantDriver::Application::GetInstance().m_activeECS->FindEntityWithTag("Player");
					if (playerbox != entt::null)
					{
						const auto& playerbox_scriptable = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(playerbox);
						if (playerbox_scriptable->m_ScriptTypes.contains("EncounterSystemDriver"))
							SSH::InvokeFunction("EncounterSystemDriver", "TakeDamage", playerbox, static_cast<std::uint32_t>(other), player_projectile_script->m_Damage);

					}
				}
			}
		}
	}

	void EnemyProjectileCollision(entt::entity& object, entt::entity& other)
	{
		if (!PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(object) || !PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(other))
			return;

		const auto& enemy_projectile_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Projectile>(object);
		const auto& player_object_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(other);

		if (enemy_projectile_script && player_object_script)
		{
			bool player = player_object_script->m_ScriptTypes.contains("PlayerScript");
			if (enemy_projectile_script->m_Ownertype == Projectile::OwnerType::Enemy && player)
			{
				PogplantDriver::Application::GetInstance().m_activeECS->DestroyEntity(object);
				// Should be able to call CallTakeDamageFunction(player_projectile_script->damage, other) here
				SSH::InvokeFunction("PlayerScript", "TakeDamage", other, enemy_projectile_script->m_Damage);
			}
		}
	}

	void TriggerWave(entt::entity& object, entt::entity& other)
	{
		if (!PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(object) || !PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(other))
			return;

		const auto& player_collider = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::BoxCollider>(object);
		const auto& other_collider = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::BoxCollider>(other);

		if (player_collider && other_collider)
		{
			if (other_collider->collisionLayer == "PLAYER" && player_collider->collisionLayer == "TRIGGERS")
			{
				MonoString* tag_mono = mono_string_new(mono_domain_get(), PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Tag>(object)->m_tag.c_str());

				auto parent_id = static_cast<std::uint32_t>(ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(other))->m_parent);
				auto player_box_scriptable = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(static_cast<entt::entity>(parent_id));

				if (player_box_scriptable->m_ScriptTypes.contains("EncounterSystemDriver"))
					SSH::InvokeFunction("EncounterSystemDriver", "SpawnWave", ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(other))->m_parent, *tag_mono);
				//SSH::InvokeFunction("PlayerScript", "SpawnWave", other);
			}
			else if (player_collider->collisionLayer == "PLAYER" && other_collider->collisionLayer == "TRIGGERS")
			{
				MonoString* tag_mono = mono_string_new(mono_domain_get(), PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Tag>(other)->m_tag.c_str());

				auto parent_id = static_cast<std::uint32_t>(ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(object))->m_parent);
				auto player_box_scriptable = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(static_cast<entt::entity>(parent_id));

				if (player_box_scriptable->m_ScriptTypes.contains("EncounterSystemDriver"))
					SSH::InvokeFunction("EncounterSystemDriver", "SpawnWave", ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(object))->m_parent, *tag_mono);
				//SSH::InvokeFunction("PlayerScript", "SpawnWave", other);
			}
		}
	}

	void OnTriggerEnterEvent(std::shared_ptr<PPE::OnTriggerEnterEvent> onTriggerEnterEvent)
	{
		if (!PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(onTriggerEnterEvent->m_entity1) || !PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(onTriggerEnterEvent->m_entity2))
			return;

		auto& object = onTriggerEnterEvent->m_entity1;
		auto& other = onTriggerEnterEvent->m_entity2;

		PlayerProjectileCollision(object, other);
		EnemyProjectileCollision(object, other);
		TriggerWave(object, other);
	}
	glm::vec3 GetForwardVector(std::uint32_t entityID)
	{
		auto transform = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Transform>(static_cast<entt::entity>(entityID));
		glm::vec3 fv{ 0.f };

		if (transform)
		{
			fv = transform->GetForwardVector();
		}

		return fv;
	}

	void StartMissile(std::uint32_t entityID)
	{
		entt::entity missile_id = static_cast<entt::entity>(entityID);
		if (missile_id != entt::null)
		{
			SSH::InvokeFunction("Missile", "SetMissile", missile_id, true);
		}
	}

	void SetTurretFire(std::uint32_t entityID, bool isActivated, int TurretIdentifier)
	{
		entt::entity turret_id = static_cast<entt::entity>(entityID);
		if (turret_id != entt::null)
		{
			//Base normal turret
			if (TurretIdentifier == 1)
			{
				SSH::InvokeFunction("BaseTurret", "StartFiring", turret_id, isActivated);
			}
			//Base normal turret
			if (TurretIdentifier == 2)
			{
				SSH::InvokeFunction("BaseGattling", "StartFiring", turret_id, isActivated);
			}
		}
	}

	float GetPlayerHealth()
	{
		entt::entity player = PogplantDriver::Application::GetInstance().m_activeECS->FindEntityWithName("PlayerShip");
		float health = 0.f;
		if (player != entt::null)
		{
			health = SSH::InvokeFunctionWithReturn<float>("PlayerScript", "GetPlayerHealth", player);
		}
		return health;
	}

	float GetPlayerMaxHealth()
	{
		entt::entity player = PogplantDriver::Application::GetInstance().m_activeECS->FindEntityWithName("PlayerShip");
		float health = 0.f;
		if (player != entt::null)
		{
			health = SSH::InvokeFunctionWithReturn<float>("PlayerScript", "GetPlayerMaxHealth", player);
		}
		return health;
	}

	void PlayerTakeDamage(std::uint32_t Player_ID, float _Damage)
	{
		entt::entity player_id = static_cast<entt::entity>(Player_ID);
		SSH::InvokeFunction("PlayerScript", "TakeDamage", player_id, _Damage);
	}

	// Updates the player health UI
	void UpdatePlayerHealth_UI()
	{
		entt::entity hpBar = PogplantDriver::Application::GetInstance().m_activeECS->FindEntityWithName("HP_Bar");
		if (hpBar != entt::null)
		{
			auto hpTrans = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Transform>(hpBar);
			if (hpTrans != nullptr)
			{
				static float targetScale = 0.805f;
				static float maxHealth = GetPlayerMaxHealth();
				float healthCalc = GetPlayerHealth() / maxHealth * targetScale;

				hpTrans->m_scale.x = healthCalc;
			}
		}
	}

	bool Scripting::GetAlive(std::uint32_t entityID)
	{
		if (!PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(static_cast<entt::entity>(entityID)))
			return false;

		//Get Playerboxwith EncounterSystemDriver
		entt::entity player_id = PogplantDriver::Application::GetInstance().m_activeECS->FindEntityWithTag("Player");
		entt::entity id = static_cast<entt::entity>(entityID);
		bool isAlive = false;

		auto scriptable = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(id);
		if (scriptable)
		{
			if (scriptable->m_ScriptTypes.contains("BaseTurret"))
				isAlive = SSH::InvokeFunctionWithReturn<bool>("BaseTurret", "GetAlive", id);
			if (scriptable->m_ScriptTypes.contains("BaseGattling"))
				isAlive = SSH::InvokeFunctionWithReturn<bool>("BaseGattling", "GetAlive", id);
		}
		if(!scriptable)
		{
			isAlive = SSH::InvokeFunctionWithReturn<bool>("EncounterSystemDriver", "GetAlive" , player_id, entityID);
		}
		return isAlive;
	}

	void Scripting::UpdateScore(std::uint32_t text_object, std::uint32_t score)
	{
		if (!PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(static_cast<entt::entity>(text_object)))
			return;

		auto text = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Text>(static_cast<entt::entity>(text_object));

		if (text)
		{
			text->m_Text = "Score: " + std::to_string(score);
		}
	}

	void Scripting::PlayEnemyDeathAnimation(std::uint32_t entityID)
	{
		auto rb = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Rigidbody>(static_cast<entt::entity>(entityID));

		if (rb)
		{
			rb->isKinematic = false;
			rb->useGravity = true;

			std::random_device rd;  // Will be used to obtain a seed for the random number engine
			std::mt19937 gen(rd());
			auto dist = std::uniform_real_distribution(500.0, 1000.0);

			glm::vec3 dir = { dist(gen), dist(gen), dist(gen) };
			rb->AddImpulseForce(dir);
			rb->mass = 100.0f;

			//std::cout << "Death Animation called" << std::endl;
		}
	}
}
