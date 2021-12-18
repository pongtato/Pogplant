#ifndef GAMESCRIPT_H_
#define GAMESCRIPT_H_

#include <gtc/type_ptr.hpp>
#include "Events/Events.h"

#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>

namespace Scripting
{
	class GameScript
	{
	public:

	static void Init();
	//Utility Get Mouse using reference
	static void GetMousePos(float& _X, float& _Y);

	//Wrapper for instantiate due to monostring calls
	static std::uint32_t Instantiate(MonoString* name , glm::vec3 _Position, glm::vec3 _Rotation);
	static std::uint32_t InstantiateParticle(MonoString* name , glm::vec3 _Position, glm::vec3 _Rotation, bool _parented, uint32_t _parentID);

	static int CheckBounds(glm::vec3& _Position, glm::vec3& _Velocity);

	//Only works via checking for  "name" PlayerCam together with PlayerScript
	static void FollowPlayerCam(std::uint32_t player_cam, std::uint32_t player_box, std::uint32_t playerShip, glm::vec3 _Position, glm::vec3 _Rotation, float _deltaTime);

	static void FirePlayerBullet(glm::vec3 _Position, glm::vec3 _FowardVector, glm::vec3 _Rotation);

	static void FireEnemyBullet(std::uint32_t entityID, glm::vec3 _Position, glm::vec3 _Rotation, float _Speed, float _Lifetime, bool isTrue = false);

	static void SpawnStaticExplosion(glm::vec3& position, int type);

	static void OnTriggerEnterEvent(std::shared_ptr<PPE::OnTriggerEnterEvent> onTriggerEnterEvent);



	static void StartMissile(std::uint32_t entityID);

	static void SetTurretFire(std::uint32_t entityID, bool isActivated, int TurretIdentifier);

	//just store the object that contains the EncounterSystemDriver
	static void IncreaseScorefromEnv(std::uint32_t entityID);

	static void UpdateDashboardFace(std::uint32_t dashboardEntityID, std::uint32_t faceType);

	static void UpdateScore(std::uint32_t text_object, std::uint32_t score);

	// Get the player healths
	static float GetPlayerHealth();
	static float GetPlayerMaxHealth();
	static void PlayerTakeDamage(std::uint32_t Player_ID, float _Damage, std::uint32_t DashboardID, std::uint32_t _FaceIndex);
	// Updates the player health UI
	static void UpdatePlayerHealth_UI();

	//Get Enemy alive status(Only for base turret, can expand to others)
	//Calling this for others is ???
	static bool GetAlive(std::uint32_t entityID);
	static void PlayEnemyDeathAnimation(std::uint32_t entityID);

	private:

		static entt::entity m_playerbox;
		static entt::entity m_playership;
	};
}

#endif