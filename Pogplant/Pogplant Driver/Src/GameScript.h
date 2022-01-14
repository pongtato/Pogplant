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

	static void RunMissilePhase1(std::uint32_t entityID, std::uint32_t IndicatorID,  bool& blink_phase1,
		float& large_blink_scale, float& small_blink_scale, float& accumulated_scale, float& Scale_duration,
		bool& start_blinking, float& blink_phase_dt, float& accu_dt_blink, bool& isBig, bool& missle_drop_phase2,
		float dt);

	static 	void RunMissilePhase2(std::uint32_t m_DropMissile, bool& set_missle_start,
		float& missile_scale, glm::vec3& Start_drop_pos, glm::vec3& End_drop_pos,
		float& accu_dt_drop, float& missle_drop_speed, bool& missle_drop_phase2, bool& explode_phase3, float dt);

	static 	void RunMissilePhase3(std::uint32_t m_Explosion, float& final_scale_value, bool& set_explode_start, glm::vec3& start_scale,
		float& accu_dt_expand, float& explosion_expand_multiplier, float& centre_shift_multiplier, float& extends_multiplier_Y, float& extends_multiplier_XZ,
		float& scale_down_dt, float& scale_down_time, bool& explode_phase3, bool& m_End, glm::vec3& start_centre, glm::vec3& start_extends, float dt);

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


	static entt::entity GetPlayerBox() { return m_playerbox; }
	static entt::entity GetPlayerShip() { return m_playership; }
	// Helper Function for taking damage
	static void InvokeEnemyTakeDamage(std::string _ScriptName,
																		entt::entity object ,entt::entity other, 
																		Components::Projectile* player_projectile_pointer, Components::Scriptable* enemy_script_pointer);  
	private:

	static entt::entity m_playerbox;
	static entt::entity m_playership;
	};
}

#endif