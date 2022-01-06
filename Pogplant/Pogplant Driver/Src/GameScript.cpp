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
	entt::entity GameScript::m_playerbox = entt::null;
	entt::entity GameScript::m_playership = entt::null;

	void GameScript::Init()
	{
		GameScript::m_playerbox = PogplantDriver::Application::GetInstance().m_activeECS->FindEntityWithName("PlayerBox");
		GameScript::m_playership = PogplantDriver::Application::GetInstance().m_activeECS->FindEntityWithName("PlayerShip");

	}

	void GameScript::GetMousePos(float& _X, float& _Y)
	{
		double castX = 0;
		double castY = 0;
		PPI::GLFWInputManager::Instance().getMouse(castX, castY);
		_X = static_cast<float>(castX);
		_Y = static_cast<float>(castY);
	}

	std::uint32_t GameScript::Instantiate(MonoString* name, glm::vec3 _Position, glm::vec3 _Rotation)
	{
		std::string _name = mono_string_to_utf8(name);

		PogplantDriver::Serializer serial(*PogplantDriver::Application::GetInstance().m_activeECS);
		entt::entity new_entitiy = serial.Instantiate(_name, _Position, _Rotation);

		return std::uint32_t(new_entitiy);
	}

	/**> General purpose particle creation that will autoplay*/
	std::uint32_t GameScript::InstantiateParticle(MonoString* name, glm::vec3 _Position, glm::vec3 _Rotation, bool _parented, uint32_t _parentID)
	{
		std::string _name = mono_string_to_utf8(name);

		PogplantDriver::Serializer serial(*PogplantDriver::Application::GetInstance().m_activeECS);
		entt::entity newEntity = serial.Instantiate(_name, _Position, _Rotation);

		auto particleSystem = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::ParticleSystem>(newEntity);
		auto& transform = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().get<Components::Transform>(newEntity);

		if (particleSystem)
		{
			particleSystem->init();
			particleSystem->m_Play = true;
		}

		if (!transform.m_children.empty())
		{
			for (auto child : transform.m_children)
			{
				auto childParticle = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::ParticleSystem>(child);

				if (childParticle)
				{
					childParticle->init();
					childParticle->m_Play = true;
				}
			}
		}

		if (_parented)
		{
			PogplantDriver::Application::GetInstance().m_activeECS->SetParent((entt::entity)_parentID, newEntity);
		}
		
		return std::uint32_t(newEntity);
	}


	// Only checking the bound for player to it's parent and will not work anywhere else
	int GameScript::CheckBounds(glm::vec3& _Position, glm::vec3& _Velocity)
	{
		//std::cout << "Player Health Is: " << GetPlayerHealth() << std::endl;
		if (m_playerbox != entt::null)
		{
			//GET AND COMPARE GLOBAL DUE TO LOCAL FUNKYNESS
			auto boxcollider_comp = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<BoxCollider>(m_playerbox);
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
	void GameScript::FollowPlayerCam(std::uint32_t player_cam, std::uint32_t player_box, std::uint32_t playerShip, glm::vec3 _Position, glm::vec3 _Rotation, float _deltaTime)
	{
		//Offset the camera based on the box world position
		if (player_cam != entt::null && player_box != entt::null)
		{
			auto box_pos = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Transform>((entt::entity)player_box);
			//std::cout << "Rotation: " << box_pos->m_rotation.x << ", " << box_pos->m_rotation.y << ", " << box_pos->m_rotation.z << std::endl;

			auto playerTransform = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Transform>((entt::entity)playerShip);

			auto cam_comp = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Camera>((entt::entity)player_cam);
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

	void GameScript::FirePlayerBullet(glm::vec3 _Position, glm::vec3 _FowardVector, glm::vec3 _Rotation)
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
	void GameScript::FireEnemyBullet(std::uint32_t entityID, glm::vec3 _Position, glm::vec3 _Rotation, float _Speed, float _Lifetime, bool isTrue)
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

	void GameScript::SpawnStaticExplosion(glm::vec3& position, int type)
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


	void PlayerProjectileCollision(entt::entity object, entt::entity other)
	{
		if (!PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(object) || !PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(other))
			return;

		auto player_projectile_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Projectile>(object);
		Components::Scriptable* enemy_object_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(other);

		//There's no order between object and other, so there's a chance that the player is other and the turret is object, and vice versa

		if (!player_projectile_script)
		{
			player_projectile_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Projectile>(other);
			enemy_object_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(object);

			std::swap(other, object);
		}
		else
		{
			enemy_object_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(other);
		}

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

			bool enemy_flock = enemy_object_script->m_ScriptTypes.contains("BaseFlock");
			if (player_projectile_script->m_Ownertype == Projectile::OwnerType::Player && enemy_flock)
			{
				PogplantDriver::Application::GetInstance().m_activeECS->DestroyEntity(object);
				// Should be able to call CallTakeDamageFunction(player_projectile_script->damage, other) here
				SSH::InvokeFunction("BaseFlock", "TakeDamage", other, player_projectile_script->m_Damage);
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
						if (playerbox_scriptable && playerbox_scriptable->m_ScriptTypes.contains("EncounterSystemDriver"))
						{
							PogplantDriver::Application::GetInstance().m_activeECS->DestroyEntity(object);
							SSH::InvokeFunction("EncounterSystemDriver", "TakeDamage", playerbox, static_cast<std::uint32_t>(other), player_projectile_script->m_Damage);
						}
					}
				}
			}
		}
	}

	void EnemyProjectileCollision(entt::entity object, entt::entity other)
	{
		if (!PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(object) || !PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(other))
			return;

		auto enemy_projectile_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Projectile>(object);
		Components::Scriptable* player_object_script = nullptr;

		//There's no order between object and other, so there's a chance that the player is other and the turret is object, and vice versa
		if (!enemy_projectile_script)
		{
			enemy_projectile_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Projectile>(other);
			player_object_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(object);

			std::swap(object, other);
		}
		else
			player_object_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(other);

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

	void ObstaclesCollision(entt::entity object, entt::entity other)
	{
		auto player_collider = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::ColliderIdentifier>(object);
		auto other_collider = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::ColliderIdentifier>(other);

		int playerLayer = PogplantDriver::Application::GetInstance().m_sPhysicsSystem.GetCollisionLayer("PLAYER");
		if (other_collider && other_collider->collisionLayer == playerLayer)
		{
			std::swap(player_collider, other_collider);
			std::swap(object, other);
		}

		if (player_collider && player_collider->collisionLayer == playerLayer)
		{
			if (other_collider && other_collider->collisionLayer == PogplantDriver::Application::GetInstance().m_sPhysicsSystem.GetCollisionLayer("OBSTACLES"))
			{
				auto player_object_script = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(object);

				if (player_object_script && player_object_script->m_ScriptTypes.contains("PlayerScript"))
				{
					SSH::InvokeFunction("PlayerScript", "TakeDamage", object, 30.f);
				}
			}
		}
	}

	void TriggerWave(entt::entity object, entt::entity other)
	{
		if (!PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(object) || !PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(other))
			return;

		auto player_collider = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::BoxCollider>(object);
		auto other_collider = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::BoxCollider>(other);

		if (player_collider && other_collider)
		{
			//Check and swap since order can be different, it's ok to swap cause local pointers
			//Like that no need duplicate the code
			if (other_collider->collisionLayer == "PLAYER")
			{
				std::swap(player_collider, other_collider);
				std::swap(object, other);
			}

			if (player_collider->collisionLayer == "PLAYER" && other_collider->collisionLayer == "WAVETRIGGERS")
			{
				auto tag = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Tag>(other);

				if (tag)
				{
					MonoString* tag_mono = mono_string_new(mono_domain_get(), tag->m_tag.c_str());

					auto parent_id = static_cast<std::uint32_t>(ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(object))->m_parent);
					auto player_box_scriptable = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(static_cast<entt::entity>(parent_id));

					if (player_box_scriptable->m_ScriptTypes.contains("EncounterSystemDriver"))
						SSH::InvokeFunction("EncounterSystemDriver", "SpawnWave", ScriptSystem::GetECS()->GetReg().try_get<Components::Transform>(static_cast<entt::entity>(object))->m_parent, *tag_mono);
					//SSH::InvokeFunction("PlayerScript", "SpawnWave", other);
				}
			}
		}
	}

	void GameScript::OnTriggerEnterEvent(std::shared_ptr<PPE::OnTriggerEnterEvent> onTriggerEnterEvent)
	{
		if (!PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(onTriggerEnterEvent->m_entity1) || !PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(onTriggerEnterEvent->m_entity2))
			return;

		auto& object = onTriggerEnterEvent->m_entity1;
		auto& other = onTriggerEnterEvent->m_entity2;

		PlayerProjectileCollision(object, other);
		EnemyProjectileCollision(object, other);
		ObstaclesCollision(object, other);
		TriggerWave(object, other);
	}

	void GameScript::StartMissile(std::uint32_t entityID)
	{
		entt::entity missile_id = static_cast<entt::entity>(entityID);
		if (missile_id != entt::null)
		{
			SSH::InvokeFunction("Missile", "SetMissile", missile_id, true);
		}
	}

	void GameScript::SetTurretFire(std::uint32_t entityID, bool isActivated, int TurretIdentifier)
	{
		entt::entity turret_id = static_cast<entt::entity>(entityID);
		if (turret_id != entt::null)
		{
			//Base normal turret
			if (TurretIdentifier == 1)
			{
				SSH::InvokeFunction("BaseTurret", "StartFiring", turret_id, isActivated);
			}
			//Base Gattling turret
			if (TurretIdentifier == 2)
			{
				SSH::InvokeFunction("BaseGattling", "StartFiring", turret_id, isActivated);
			}
		}
	}

	float GameScript::GetPlayerHealth()
	{
		float health = 0.f;
		if (m_playership != entt::null)
		{
			health = SSH::InvokeFunctionWithReturn<float>("PlayerScript", "GetPlayerHealth", m_playership);
		}
		return health;
	}

	float GameScript::GetPlayerMaxHealth()
	{
		float health = 0.f;
		if (m_playership != entt::null)
		{
			health = SSH::InvokeFunctionWithReturn<float>("PlayerScript", "GetPlayerMaxHealth", m_playership);
		}
		return health;
	}

	void GameScript::PlayerTakeDamage(std::uint32_t Player_ID, float _Damage, std::uint32_t DashboardID, std::uint32_t _FaceIndex)
	{
		entt::entity player_id = static_cast<entt::entity>(Player_ID);
		SSH::InvokeFunction("PlayerScript", "TakeDamage", player_id, _Damage);

		entt::entity dashboard_id = static_cast<entt::entity>(DashboardID);
		if (dashboard_id != entt::null)
		{
			auto scriptable = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(dashboard_id);
			if (scriptable)
			{
				if (scriptable->m_ScriptTypes.contains("PlayerScript"))
				{
					SSH::InvokeFunction("DashboardScreen", "SwapFace", dashboard_id, _FaceIndex);
				}
			}
		}
	}

	// Updates the player health UI
	//Not used anymore
	void GameScript::UpdatePlayerHealth_UI()
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

	bool GameScript::GetAlive(std::uint32_t entityID)
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
			if (scriptable->m_ScriptTypes.contains("BaseFlock"))
				isAlive = SSH::InvokeFunctionWithReturn<bool>("BaseFlock", "GetAlive", id);
		}
		if(!scriptable)
		{
			isAlive = SSH::InvokeFunctionWithReturn<bool>("EncounterSystemDriver", "GetAlive" , player_id, entityID);
		}
		return isAlive;
	}

	void GameScript::IncreaseScorefromEnv(std::uint32_t entityID)
	{
		entt::entity encounterdriverID = static_cast<entt::entity>(entityID);

		if (encounterdriverID != entt::null)
		{
			auto scriptable = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(encounterdriverID);
			if (scriptable)
			{
				if (scriptable->m_ScriptTypes.contains("EncounterSystemDriver"))
				{
					SSH::InvokeFunction("EncounterSystemDriver", "AddScore", encounterdriverID);
				}
			}
		}
	}

	void GameScript::UpdateDashboardFace(std::uint32_t dashboardEntityID, std::uint32_t faceType)
	{
		entt::entity dashboardID = static_cast<entt::entity>(dashboardEntityID);

		if (PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(dashboardID))
		{
			auto scriptable = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Scriptable>(dashboardID);
			if (scriptable)
			{
				if (scriptable->m_ScriptTypes.contains("DashboardScreen"))
				{
					SSH::InvokeFunction("DashboardScreen", "SwapFace", dashboardID, faceType);
				}
			}
		}
	}

	void GameScript::UpdateScore(std::uint32_t text_object, std::uint32_t score)
	{
		if (!PogplantDriver::Application::GetInstance().m_activeECS->GetReg().valid(static_cast<entt::entity>(text_object)))
			return;

		auto text = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Components::Text>(static_cast<entt::entity>(text_object));
		
		static constexpr size_t numPadding = 8;
		static std::string scoreText;
		if (text)
		{
			scoreText = std::to_string(score);
			int limit = (int)numPadding - (int)scoreText.size();

			if (limit < 0)
			{
				text->m_Text = scoreText;
			}
			else
			{
				text->m_Text = "";
				for (size_t i = 0; i < limit; i++)
				{
					text->m_Text += "0";
				}

				text->m_Text += scoreText;
			}
		}
	}

	void GameScript::PlayEnemyDeathAnimation(std::uint32_t entityID)
	{
		auto rb = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<Rigidbody>(static_cast<entt::entity>(entityID));

		if (rb)
		{
			rb->isKinematic = false;
			rb->useGravity = true;

			std::random_device rd;  // Will be used to obtain a seed for the random number engine
			std::mt19937 gen(rd());
			auto dist = std::uniform_real_distribution(10.0, 20.0);

			glm::vec3 dir = { dist(gen), dist(gen), dist(gen) };
			rb->AddImpulseForce(dir);
		}
	}



	void GameScript::RunMissilePhase1(std::uint32_t entityID, std::uint32_t IndicatorID, bool& blink_phase1,
		float& large_blink_scale, float& small_blink_scale, float& accumulated_scale, float& Scale_duration,
		bool& start_blinking, float& blink_phase_dt, float& accu_dt_blink, bool& isBig, bool& missle_drop_phase2,
		float dt)
	{
		//Expand to blink
		glm::vec3 large_scale{ large_blink_scale };
		glm::vec3 small_scale{ small_blink_scale };

		if (start_blinking)
		{
			//Expand in 0.5s
			accumulated_scale += dt;
			if (accumulated_scale >= Scale_duration)
			{
				accumulated_scale = Scale_duration;
				SSH::PlayAudio(entityID, 0);
				start_blinking = false;
			}
			glm::vec3 scale = glm::mix(small_scale, large_scale, (accumulated_scale / Scale_duration));
			SSH::SetScale(IndicatorID, scale);
		}
		//Just scale  to big and small
		if (!start_blinking)
		{
			blink_phase_dt += dt;
			accu_dt_blink += dt;
			if (accu_dt_blink >= 0.2f)
			{
				accu_dt_blink = 0.0f;
				glm::vec3 scale{ 0 };
				if (isBig)
					scale = large_scale;
				if (!isBig)
					scale = small_scale;
				SSH::SetScale(IndicatorID, scale);
				isBig = !isBig;
			}
			if (blink_phase_dt >= 1.5f)
			{
				//Force set scale to small and initiate second phase
				SSH::SetScale(IndicatorID, small_scale);
				blink_phase1 = false;
				missle_drop_phase2 = true;
			}
		}
	}

	void GameScript::RunMissilePhase2(std::uint32_t m_DropMissile, bool& set_missle_start,
		float& missile_scale, glm::vec3& Start_drop_pos, glm::vec3& End_drop_pos,
	float& accu_dt_drop , float& missle_drop_speed, bool& missle_drop_phase2, bool& explode_phase3 , float dt)
	{
		//Set missle drop high
		if (!set_missle_start)
		{
			glm::vec3 set_Scale{ missile_scale };
			SSH::SetPosition(m_DropMissile, Start_drop_pos);
			SSH::SetScale(m_DropMissile, set_Scale);
			set_missle_start = true;
		}

		accu_dt_drop += missle_drop_speed * dt;
		//Iterate the dropping now
		glm::vec3 drop_pos = glm::mix(Start_drop_pos, End_drop_pos, accu_dt_drop);
		SSH::SetPosition(m_DropMissile, drop_pos);
		//If the aabb collider min Y only touches touches the bottem, end phase 2 and start phase 3
		BoxCollider* missle_collider =  PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<BoxCollider>(static_cast<entt::entity>(m_DropMissile));
		glm::vec3 min = drop_pos - missle_collider->extends;
		if (min.y <= End_drop_pos.y)
		{
			//Start phase 3 return the missle to unseeable
			glm::vec3 set_end_Scale{ 0.01 };
			SSH::SetScale(m_DropMissile, set_end_Scale);
			missle_drop_phase2 = false;
			explode_phase3 = true;
		}
	}

	void GameScript::RunMissilePhase3(std::uint32_t m_Explosion, float& final_scale_value, bool& set_explode_start, glm::vec3& start_scale, 
		float& accu_dt_expand, float& explosion_expand_multiplier, float& centre_shift_multiplier, float& extends_multiplier_Y, float& extends_multiplier_XZ,
		float& scale_down_dt, float& scale_down_time, bool& explode_phase3, bool& m_End, glm::vec3& start_centre, glm::vec3& start_extends, float dt)
	{

		glm::vec3 Final_scale{ final_scale_value };
		if (!set_explode_start)
		{
			//Assume the trigger is already there, just have to not to step on it on a VERY TINY BOX
			Final_scale.y = 0.0f;
			start_scale = Final_scale;
			SSH::SetScale(m_Explosion, Final_scale);
			set_explode_start = true;
		}
		//Start expanding and lerping to scale size and set trigger to active
		accu_dt_expand += dt;
		if (accu_dt_expand < explosion_expand_multiplier)
		{
			glm::vec3 curr_scale = glm::mix(start_scale, Final_scale, accu_dt_expand / explosion_expand_multiplier);
			SSH::SetScale(m_Explosion, curr_scale);
			//Change the centre colliderbox position also
			BoxCollider* explosion_collider = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<BoxCollider>(static_cast<entt::entity>(m_Explosion));
			explosion_collider->centre.y = curr_scale.y * centre_shift_multiplier;
			explosion_collider->extends.y = curr_scale.y * extends_multiplier_Y;
			explosion_collider->extends.z = curr_scale.y * extends_multiplier_XZ;
			explosion_collider->extends.x = curr_scale.y * extends_multiplier_XZ;
		}
		//Scale down the explosion after it happens
		if (accu_dt_expand >= explosion_expand_multiplier)
		{
			//Scale down
			scale_down_dt += dt;
			if (scale_down_dt > scale_down_time)
			{
				scale_down_dt = scale_down_time;
				explode_phase3 = false;
				m_End = true;
				BoxCollider* end_explosion_collider = PogplantDriver::Application::GetInstance().m_activeECS->GetReg().try_get<BoxCollider>(static_cast<entt::entity>(m_Explosion));
				end_explosion_collider->centre = start_centre;
				end_explosion_collider->extends = start_extends;
			}
			glm::vec3 scale_down = glm::mix(Final_scale, start_scale, scale_down_dt / scale_down_time);
			SSH::SetScale(m_Explosion, scale_down);
		}
	}

}


