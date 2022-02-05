/*****************************************************************************/
/*!
\file	PlayerScript.cs
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	PlayerScript class for scripting c# project

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Scripting.Utilities;

namespace Scripting
{
	// Player script class
	public class PlayerScript : MonoBehaviour
	{
		public bool mm_useNewMovementSystem = false;
		public float movement_speed = 200.0f;
		private float horizontal_input = 0;
		private float vertical_input = 0;
		private float slowForce = 4.0f;
		//private float maxSpeed = 10.0f;


		//Player Rotation movement (Camera duplicates this effect)
		//float camera_roll = -1.5f;

		/*public float max_rotate_angle = 12.5f;
		public float rotation_speed_horizontal = 1.0f;
		public float rotation_speed_vertical = 1.0f;
		public float revert_speed = 2.0f; // changed from 20
		public float max_dampening_speed = 100.0f;

		public float dampening_reduction = 1.0f;
		public float dampening_modifier = 4.0f;//*/

		//private float current_vertical_dampening;
		//private float current_horizontal_dampening;

		//private float timeCount;
		public float ship_follow_rot_speed;
		static public float maxHealth = 30000.0f;
		public float health = maxHealth;

		uint shipCameraEntity;
		uint boxEntityID;

		//Used to calculate ship tilt
		private Vector3 lastPosition;
		private Vector3 calculatedVelocity;
		private Vector3 targetRotation;

		private const float shipYawFollowSpeed = 8f;
		private const float shipRollFollowSpeed = 8f;
		private const float shipPitchFollowSpeed = 8f;

		//Change this to make the ship tilt more according to movement
		private const float shipYawMultiplier = 2.5f;
		private const float shipPitchMultiplier = 3.5f;
		private const float shipRollMultiplier = 3.5f;

		private const float cameraShakeInitMultiplier = 0.007f;
		private const float cameraShakeMagMultiplier = 5f;

		//How long the player should be invulnerable if damage is taken
		private const float damageInvulPeriod = 0.1f;

		private bool damageInvul = false;
		private float damageInvulTimer = 0.0f;


		//How much ship should roll based off rotation from the spline
		//private float shipRollMultiplierRotation = 0.25f;


		//// THIS IS ONLY FOR TESTING + EXAMPLE PURPOSES
		//public List<uint> entityIDList = new List<uint>();
		//// parentID first, childID second
		//public List<Tuple<uint, uint>> childIDList = new List<Tuple<uint, uint>>();

		public List<GameObject> entityList = new List<GameObject>();
		public List<Tuple<GameObject, GameObject>> childList = new List<Tuple<GameObject, GameObject>>();
		BoxCollider boxCollider;
		Camera camera;
		Transform playerTrans;
		//EnemyManager enemyManager = new EnemyManager();

		bool isAlive = true;
		uint DashboardScreenFaceID;
		static uint DashboardScreenID;
		uint VOEntityID;
		//uint m_TrailRenderer;

		//PlayerSpline Speed 
		float m_Maxspeed = 12.0f;
		float m_Minspeed = 9.0f;
		float m_SpeedChange = 0.5f;
		float m_IncrementInterval = 0.1f;
		float m_IncrementTimer = 0.0f;

		// Bonus Item / Bonus Effects
		static public int m_BonusItem = 0;
		const int m_BonusItemMax = 4;
		static public bool m_EnableBonusScreen = false;
		uint m_BobHeadMenuID;
		static public uint m_ScoreMultiplierBobbleCount = 1;
		static public uint m_ShieldBobbleCount = 1;
		static public uint m_ComboDecayBobbleCount = 1;
		static public float m_ComboDecayTimeLimit = 3.0f;
		static public float m_ComboDecayTimer = m_ComboDecayTimeLimit;
		static public bool m_ComboActive = false;
		static public bool m_ActivateBobble = false;
		uint m_BobbleTimmy;
		uint m_BobbleProf;
		uint m_BobbleBoobas;

		// Bonus Effects

		//Pause menu
		static public bool m_EnablePauseMenu = false;

		// Laser Attack
		uint m_LaserWeaponID;
		static public bool m_EnableLaserAttack = false;
		bool m_LaserReady = false;
		const float m_LaserCooldown = 2.0f;
		float m_LaserTimer = m_LaserCooldown;

		// EndGameMenu Stats
		public static uint m_EnemyDestroyedCount = 0;
		public static uint m_PlayerHitCount = 0;
		public static uint m_CollectiblesCount = 0;

		// Combo UI
		public static uint m_ComboNumber = 0;
		public static uint m_ComboNumberID;
		public static uint m_ComboLargeFireID;
		const uint m_ComboThresholdLarge = 10;
		public static uint m_ComboSmallFireID;
		const uint m_ComboThresholdSmall = 5;
		public static uint m_ComboBarID;
		public static uint m_ComboTextID;
		public static uint m_ComboXID;
		public static Vector3 m_FullComboBarScale = new Vector3(2.0f, 1.0f, 1.0f);
		public static Vector3 m_EmptyComboBarScale = new Vector3(0.0f, 1.0f, 1.0f);
		//public static Transform m_ComboBarTrans = new Transform();

		// Score
		public static uint score = 0;                                    // Current score
		public static uint m_OldScore = 0;                               // Old score
		private static uint kill_score = 100;                            // Addition to base score everytime an enemy is killed
		private static uint minus_score = 10;                            // Minus to base score everytime player is hit
		static uint m_ScoreTextID;
		static float m_ScoreResetTimer;
		const float m_ScoreResetTimeLimit = 1.0f;
		static List<uint> m_AddScoreListIDs = new List<uint>();
		static List<int> m_DestroyAddScoreListIndex = new List<int>();
		Vector3 m_AddScoreEndPos = new Vector3(-0.25f, 0.30f, -0.165f);
		Vector3 m_AddScoreEndScale = new Vector3(0.3f, 0.3f, 0.3f);
		const float m_ScoreAnimationSpeed = 3.0f;
		static uint entity_id;

		public override void Init(ref uint _entityID)
		{
			entityID = _entityID;
			entity_id = _entityID;
		}

		public override void Start()
		{
			shipCameraEntity = ECS.FindEntityWithName("PlayerCam");
			boxEntityID = ECS.FindEntityWithName("PlayerBox");
			DashboardScreenFaceID = ECS.FindEntityWithName("DashboardScreenFace");
			DashboardScreenID = ECS.FindEntityWithName("DashBoard");
			ECS.PlayAudio(shipCameraEntity, 0, "BGM");
			boxCollider = ECS.GetComponent<BoxCollider>(boxEntityID);
			lastPosition = ECS.GetGlobalPosition(entityID);
			targetRotation = ECS.GetComponent<Transform>(entityID).Rotation;
			m_initialCameraPosition = ECS.GetComponent<Transform>(shipCameraEntity).Position;
			VOEntityID = ECS.FindEntityWithName("VO_AudioSource");
			//m_TrailRenderer = ECS.FindEntityWithName("TrailRender");

			// For the bobble things
			m_BobHeadMenuID = ECS.FindEntityWithName("BobbleHeadMenu");
			m_BobbleTimmy = ECS.FindEntityWithName("BobbleTimmy");
			m_BobbleProf = ECS.FindEntityWithName("BobbleProf");
			m_BobbleBoobas = ECS.FindEntityWithName("BobbleBoobas");
			m_ScoreMultiplierBobbleCount = 1;
			m_ShieldBobbleCount = 1;
			m_ComboDecayBobbleCount = 1;
			m_ComboDecayTimer = m_ComboDecayTimeLimit;
			m_ComboActive = false;

			// Variables for the laser
			m_LaserWeaponID = ECS.FindEntityWithName("LaserWeapon");
			m_EnableLaserAttack = false;
			m_LaserReady = false;
			m_LaserTimer = m_LaserCooldown;

			// Variables for the ComboNumber
			m_ComboNumber = 0;
			m_ComboNumberID = ECS.FindEntityWithName("PlayerComboNumber");
			m_ComboLargeFireID = ECS.FindEntityWithName("PlayerComboLargeFire");
			m_ComboSmallFireID = ECS.FindEntityWithName("PlayerComboSmallFire");
			m_ComboBarID = ECS.FindEntityWithName("PlayerComboBar");
			m_ComboTextID = ECS.FindEntityWithName("PlayerComboText");
			m_ComboXID = ECS.FindEntityWithName("PlayerComboX");

			// Variables for Score
			m_ScoreTextID = ECS.FindEntityWithName("Score_Text");
			m_ScoreResetTimer = 0.0f;

			ECS.SetActive(m_ComboSmallFireID, false);
			ECS.SetActive(m_ComboLargeFireID, false);

			float yaw, pitch, roll;
			yaw = pitch = roll = 0;
			Camera.GetCamera(shipCameraEntity, ref yaw, ref pitch, ref roll);
			camera = new Camera(yaw, pitch, roll);

			Vector3 pos = new Vector3();
			Vector3 rot = new Vector3();
			Vector3 scale = new Vector3();
			ECS.GetTransformECS(entityID, ref pos, ref rot, ref scale);
			playerTrans = new Transform(pos, rot, scale);

			//ECS.PlayAudio(VOEntityID, 0, "VO");

			movement_speed = ECS.GetValue<float>(entityID, 200.0f, "MovementSpeed");
			slowForce = ECS.GetValue<float>(entityID, 4f, "SlowForce");
			mm_useNewMovementSystem = ECS.GetValue<bool>(entityID, false, "NewMovement");
		}

		public void Awake()
		{
		}

		public override void Update(float dt)
		{
			if (InputUtility.onKeyTriggered("SLOWMO"))
			{
				TimeManager.TriggerTimeAlter(0.5f, 2f);
			}
			//Return to main menu key
			//if (InputUtility.onKeyTriggered(KEY_ID.KEY_ESCAPE))
			//{
			//    GameUtilities.LoadScene("MainMenu");
			//}

			//// Example usage, pause particle
			//if (InputUtility.onKeyTriggered(KEY_ID.KEY_0))
			//{
			//    ECS.SetParticlePause(m_TrailRenderer, true);
			//}

			//// Example usage, play particle
			//if (InputUtility.onKeyTriggered(KEY_ID.KEY_1))
			//{
			//    ECS.SetParticlePause(m_TrailRenderer, false);
			//}

			//Updates Follow Spline
			UpdateFollowSplineSpeed(dt);

			// Updates bonus item 
			UpdateBonusItem();

			// Updates the combo bonus
			UpdateCombo(dt);

			// Updates Combo bonus
			UpdateLaser(dt);

			//if(m_OldScore != score)
			// Update player score
			UpdateScore(dt);

			ECS.GetTransformECS(entityID, ref playerTrans.Position, ref playerTrans.Rotation, ref playerTrans.Scale);
			Camera.GetCamera(shipCameraEntity, ref camera.m_Yaw, ref camera.m_Pitch, ref camera.m_Roll);

			bool rightPushed = InputUtility.onKeyHeld("RIGHT");
			bool leftPushed = InputUtility.onKeyHeld("LEFT");
			bool upPushed = InputUtility.onKeyHeld("UP");
			bool downPushed = InputUtility.onKeyHeld("DOWN");

			if (rightPushed || leftPushed)
				horizontal_input = (rightPushed ? 1.0f : 0.0f) + (leftPushed ? -1.0f : 0.0f);
			else
				horizontal_input = InputUtility.GetAxis("MOVEX");

			if (upPushed || downPushed)
				vertical_input = (upPushed ? 1.0f : 0.0f) + (downPushed ? -1.0f : 0.0f);
			else
				vertical_input = -InputUtility.GetAxis("MOVEY");

			Vector3 up_vec = Transform.GetUpVector(entityID);
			Vector3 forward_vec = Transform.GetForwardVector(entityID);
			Vector3 right_vec = Vector3.CrossProduct(forward_vec, up_vec);
			Vector3 direc_vector = (right_vec * horizontal_input) + (up_vec * vertical_input);

			//BoxCollider boxCollider =  ECS.GetComponent<BoxCollider>(boxEntityID);
			Transform boxTransform = ECS.GetComponent<Transform>(boxEntityID);
			float length = playerTrans.Position.magnitude();

			Vector3 playerGlobalPos = ECS.GetGlobalPosition(entityID);
			Vector3 boxGlobalPos = ECS.GetGlobalPosition(boxEntityID);

			//Console.WriteLine("playerGlobalPos Vector: " + playerGlobalPos.X + " | " + playerGlobalPos.Y + " | " + playerGlobalPos.Z);
			//Console.WriteLine("boxGlobalPos Vector: " + boxGlobalPos.X + " | " + boxGlobalPos.Y + " | " + boxGlobalPos.Z);

			if (!mm_useNewMovementSystem)
			{
				if (length > boxCollider.extends.X)
				{
					Vector3 newPosNoY = new Vector3(playerGlobalPos.X, 0.0f, playerGlobalPos.Z);
					Vector3 newForwardNoY = new Vector3(forward_vec.X, 0.0f, forward_vec.Z);
					Vector3 newBoxPosNoY = new Vector3(boxGlobalPos.X, 0.0f, boxGlobalPos.Z);
					Vector3 boxtoplayer = Vector3.Normalise(newPosNoY - newBoxPosNoY);

					float dotproduct = Vector3.Dot(right_vec, boxtoplayer);

					//Console.WriteLine("BoxToPlayer Vector: " + boxtoplayer.X + " | " + boxtoplayer.Y + " | " + boxtoplayer.Z);
					//Console.WriteLine("Forward Vector: " + newForwardNoY.X + " | " + newForwardNoY.Y + " | " + newForwardNoY.Z);
					//Console.WriteLine("Right Vector: " + right_vec.X + " | " + right_vec.Y + " | " + right_vec.Z);
					//Console.WriteLine("Dot Product: " + dotproduct);

					if (dotproduct > 0.0f)
					{
						ECS.RigidbodyAddForce(entityID, right_vec * -dotproduct * 300f);
						//rigidbody.AddForce(right_vec * -dotproduct * 700f);
						//Console.WriteLine("Exceed +X bounds");
					}
					else
					{
						ECS.RigidbodyAddForce(entityID, right_vec * -dotproduct * 300f);
						//rigidbody.AddForce(right_vec * -dotproduct * 700f);
						//Console.WriteLine("Exceed -X bounds");
					}
				}

				if (playerTrans.Position.Y > boxCollider.extends.Y)
				{
					ECS.RigidbodyAddForce(entityID, up_vec * (boxCollider.extends.Y - playerTrans.Position.Y) * 150f);
					//rigidbody.AddForce(up_vec * (boxCollider.extends.Y - transform.Position.Y) * 150f);
					//Console.WriteLine("Exceed +Y bounds");
				}

				if (playerTrans.Position.Y < -boxCollider.extends.Y)
				{
					ECS.RigidbodyAddForce(entityID, up_vec * (-boxCollider.extends.Y - playerTrans.Position.Y) * 350f);
					//rigidbody.AddForce(up_vec * (-boxCollider.extends.Y - transform.Position.Y) * 450f);
					//Console.WriteLine("Exceed -Y bounds");
				}
			}

			float directionalMag = direc_vector.magnitude();

			if (directionalMag > 1.0f)
				direc_vector *= 1 / directionalMag;

			Vector3 force_dir = direc_vector * movement_speed;
			ECS.RigidbodyAddForce(entityID, force_dir);
			//rigidbody.AddForce(force_dir);

			Vector3 playerVel = ECS.GetVelocity(entityID);
			float maxslowforce = playerVel.magnitude();
			if (Math.Abs(maxslowforce) <= float.Epsilon)
				maxslowforce = 0.0f;
			else
			{
				Vector3 SlowDownVec = -playerVel * (1 / maxslowforce);
				playerVel += SlowDownVec * Math.Min(maxslowforce, maxslowforce * slowForce * dt);
			}

			if (dt > 0)
			{
				calculatedVelocity = lastPosition - playerGlobalPos;
				calculatedVelocity = calculatedVelocity * (1 / dt);
			}

			//float maxslowforce = rigidbody.velocity.magnitude();
			//if (Math.Abs(maxslowforce) <= float.Epsilon)
			//    maxslowforce = 0.0f;
			//else
			//{
			//    Vector3 SlowDownVec = -rigidbody.velocity * (1 / maxslowforce);
			//    rigidbody.velocity += SlowDownVec * Math.Min(maxslowforce, maxslowforce * slowForce * dt);
			//}

			//if (dt > 0)
			//{
			//    calculatedVelocity = lastPosition - playerGlobalPos;
			//    calculatedVelocity = calculatedVelocity * (1 / dt);
			//}

			lastPosition = playerGlobalPos;

			//Ship tilter
			float relativeVelX = Vector3.Dot(right_vec, calculatedVelocity);

			//Left right tilt
			targetRotation.Y = relativeVelX * shipYawMultiplier;

			//Roll tilt
			targetRotation.Z = -relativeVelX * shipRollMultiplier;

			//Up down tilt
			targetRotation.X = Vector3.Dot(up_vec, calculatedVelocity) * shipPitchMultiplier;

			playerTrans.Rotation.Y += (targetRotation.Y - playerTrans.Rotation.Y) * shipYawFollowSpeed * dt;
			playerTrans.Rotation.Z += (targetRotation.Z - playerTrans.Rotation.Z) * shipRollFollowSpeed * dt;
			playerTrans.Rotation.X += (targetRotation.X - playerTrans.Rotation.X) * shipPitchFollowSpeed * dt;

			playerTrans.Position.Z = 0.0f;

			Vector3 pos = new Vector3();
			Vector3 rot = new Vector3();
			Vector3 scale = new Vector3();
			ECS.GetTransformECS(boxEntityID, ref pos, ref rot, ref scale);
			Transform box_pos = new Transform(pos, rot, scale);

			Vector3 rotationTarget = new Vector3(
				(-box_pos.Rotation.X - playerTrans.Rotation.X),
				(box_pos.Rotation.Y + playerTrans.Rotation.Y) + 180.0f,
				-playerTrans.Rotation.Z);

			if (camera.m_Yaw - rotationTarget.Y > 180.0f)
				camera.m_Yaw -= 360.0f;
			else if (camera.m_Yaw - rotationTarget.Y < -180.0f)
				camera.m_Yaw += 360.0f;

			if (camera.m_Pitch - rotationTarget.X > 180.0f)
				camera.m_Pitch -= 360.0f;
			else if (camera.m_Pitch - rotationTarget.X < -180.0f)
				camera.m_Pitch += 360.0f;

			if (camera.m_Roll - rotationTarget.Z > 180.0f)
				camera.m_Roll -= 360.0f;
			else if (camera.m_Roll - rotationTarget.Z < -180.0f)
				camera.m_Roll += 360.0f;

			//Lerps yaw and pitch over time
			camera.m_Yaw += (rotationTarget.Y - camera.m_Yaw) * dt * 20.0f;
			camera.m_Pitch += (rotationTarget.X - camera.m_Pitch) * dt * 20.0f;
			camera.m_Roll += (rotationTarget.Z - camera.m_Roll) * dt * 20.0f;

			Camera.SetCamera(shipCameraEntity, camera.m_Yaw, camera.m_Pitch, camera.m_Roll);

			//Console.WriteLine("Position: " + playerTrans.Position.X + '|' + playerTrans.Position.Y + '|' + playerTrans.Position.Z);
			//Console.WriteLine("Rotation: " + playerTrans.Rotation.X + '|' + playerTrans.Rotation.Y + '|' + playerTrans.Rotation.Z);
			//Console.WriteLine("Scale: " + playerTrans.Scale.X + '|' + playerTrans.Scale.Y + '|' + playerTrans.Scale.Z);

			ECS.SetVelocity(entityID, playerVel);
			ECS.SetTransformECS(entityID, playerTrans.Position, playerTrans.Rotation, playerTrans.Scale);


			//Pause menu
			if (InputUtility.onKeyTriggered("ESCAPE"))
			{
				m_EnablePauseMenu = true;
				GameUtilities.PauseScene();
			}
		}


		private Vector3 m_initialCameraPosition;
		private Vector3 m_cameraPosition;
		private Vector3 m_cameraRotation;
		private Vector3 m_cameraScale;

		bool m_isShaking = false;
		float m_shakeTimer = 0f;
		float m_shakeAddUnit = 1f;
		Vector3 m_shakeMagnitude;
		Vector3 m_shakeInit;

		public void TriggerCameraShake(Vector3 initialShake, Vector3 shakeAmount, float duration)
		{
			if (duration < float.Epsilon)
				return;

			m_isShaking = true;
			m_shakeInit = initialShake;
			m_shakeMagnitude = shakeAmount;
			m_shakeAddUnit = 1 / duration;
			m_shakeTimer = 0f;
		}

		public override void LateUpdate(float dt)
		{

			if (m_isShaking)
			{
				ECS.GetTransformECS(shipCameraEntity, ref m_cameraPosition, ref m_cameraRotation, ref m_cameraScale);

				m_shakeTimer += dt * m_shakeAddUnit;

				m_cameraPosition.X = m_initialCameraPosition.X + m_shakeInit.X * (1f - Ease.EaseOutElastic(m_shakeTimer, m_shakeMagnitude.X));
				m_cameraPosition.Y = m_initialCameraPosition.Y + m_shakeInit.Y * (1f - Ease.EaseOutElastic(m_shakeTimer, m_shakeMagnitude.Y));
				m_cameraPosition.Z = m_initialCameraPosition.Z + m_shakeInit.Z * (1f - Ease.EaseOutElastic(m_shakeTimer, m_shakeMagnitude.Z));

				if (m_shakeTimer >= 1f)
				{
					m_isShaking = false;
					m_cameraPosition = m_initialCameraPosition;
				}

				ECS.SetTransformECS(shipCameraEntity, m_cameraPosition, m_cameraRotation, m_cameraScale);
			}

			if (damageInvul)
			{
				damageInvulTimer += dt;

				if (damageInvulTimer > (damageInvulPeriod * m_ShieldBobbleCount))
					damageInvul = false;

			}
		}

		public void SpawnWave()
		{
			//var rand = new Random();
			//Transform location = new Transform(Vector3.Zero(), Vector3.Zero(), Vector3.One());
			//location.Position.X += (((float)rand.Next() / int.MaxValue) * 10.0f - 5.0f);
			//location.Position.Y += (((float)rand.Next() / int.MaxValue) * 10.0f - 5.0f);
			//location.Position.Z = 30.0f;
			//location.Rotation.Y = 180.0f;

			Console.WriteLine("Spawn Wave called!");

			float z_value = 15.0f;

			int max_spawns = 5;

			for (int x = -10; x <= 10; x += 5)
			{
				for (int y = -10; y <= 10; y += 5)
				{
					if (x != 0 && y != 0 && GetRandFloat() > 0.80f && max_spawns-- >= 0)
					{
						Transform location = new Transform();
						location.Rotation.Y = 180.0f;
						location.Position.X = x;
						location.Position.Y = y;
						location.Position.Z = z_value;
						//enemyManager.InstantiateTempEnemy(location, "Enemy", "PlayerBox");
					}
				}
				z_value -= 0.2f;
			}

		}

		// return float between zero and one
		float GetRandFloat()
		{
			var rand = new Random();
			return (float)rand.Next() / int.MaxValue;
		}

		public void FixedUpdate()
		{

		}

		public override void OnTriggerEnter(uint id)
		{
			//Console.WriteLine(" Other ID" + id);
			if (ECS.GetComponent<Tag>(id).tag == "BonusCoin")
			{
				++m_BonusItem;
			}
		}

		public override void OnTriggerExit(uint id)
		{

		}

		public float GetPlayerHealth()
		{
			return health;
		}

		public float GetPlayerMaxHealth()
		{
			return maxHealth;
		}

		public void TakeDamage(float damage)
		{
			if (damageInvul)
				return;

			damageInvul = true;
			damageInvulTimer = 0.0f;
			++m_PlayerHitCount;

			if (health > 0)
			{
				health -= damage;
			}

			//GameUtilities.UpdatePlayerHealth_UI();

			if (health <= 0)
			{
				HandleDeath();
			}

			//Console.WriteLine("Player took damage, health is now: " + health + " Entity ID: " + entityID);
			//GameUtilities.UpdateDashboardFace(DashboardScreenID, 2);
			DashboardScreen.SwapFace(DashboardScreen.FACES.HURT);
			ECS.PlayAudio(shipCameraEntity, 2, "SFX");
			AddScore(false);

			//Triggers a random camera shake upon taking damage, scales with damage taken
			TriggerCameraShake(new Vector3(GetRandFloat() * cameraShakeInitMultiplier * damage, GetRandFloat() * cameraShakeInitMultiplier * damage, GetRandFloat() * cameraShakeInitMultiplier * damage),
				new Vector3(GetRandFloat() * cameraShakeMagMultiplier, GetRandFloat() * cameraShakeMagMultiplier, GetRandFloat() * cameraShakeMagMultiplier), 1f);

			//Add controller vibration
			InputUtility.VibrateControllerHeavyMotor(0.8f, 0.2f);
		}

		void HandleDeath()
		{

			// This is a hardcoded way of destroying this instance, need to be replaced!
			if (isAlive)
			{
				Console.WriteLine("Player (id: " + entityID + ") has died");
				ECS.DestroyEntity(entityID);
				isAlive = false;
			}
		}

		void UpdateFollowSplineSpeed(float dt)
		{
			//Add speed
			if (m_IncrementTimer >= 0)
			{
				m_IncrementTimer -= dt;
			}
			else
			{
				if (InputUtility.onKeyTriggered(KEY_ID.KEY_Q) || InputUtility.onKeyHeld(KEY_ID.KEY_Q))
				{
					if ((FollowSpline.follow_speed += m_SpeedChange) > m_Maxspeed)
						FollowSpline.follow_speed = m_Maxspeed;
					m_IncrementTimer = m_IncrementInterval;
				}
				if (InputUtility.onKeyTriggered(KEY_ID.KEY_E) || InputUtility.onKeyHeld(KEY_ID.KEY_E))
				{
					if ((FollowSpline.follow_speed -= m_SpeedChange) < m_Minspeed)
						FollowSpline.follow_speed = m_Minspeed;

					//DebugUtilities.LogToEditor("PlayerScript", "Current FollowSpline speed is: " + FollowSpline.follow_speed);
					//Console.WriteLine("Current FollowSpline speed is: " + FollowSpline.follow_speed);
					m_IncrementTimer = m_IncrementInterval;
				}
			}
		}

		void UpdateBonusItem()
		{
			//if(m_BonusItem >= m_BonusItemMax)
			//{
			//    m_EnableBonusScreen = true;
			//    ECS.SetActive(m_BobHeadMenuID, true);
			//    GameUtilities.PauseScene();
			//}

			if (m_ActivateBobble)
			{
				switch (m_CollectiblesCount)
				{
					case 1:
						ECS.SetActive(m_BobbleBoobas, true);
						++m_ScoreMultiplierBobbleCount;
						break;
					case 2:
						ECS.SetActive(m_BobbleProf, true);
						++m_ComboDecayBobbleCount;
						break;
					case 3:
						ECS.SetActive(m_BobbleTimmy, true);
						++m_ShieldBobbleCount;
						break;
					default:
						break;
				}
				m_ActivateBobble = false;
			}

		}

		void UpdateCombo(float dt)
		{
			//ECS.GetTransformECS(m_ComboBarID, ref m_ComboBarTrans.Position, ref m_ComboBarTrans.Rotation, ref m_ComboBarTrans.Scale);

			if (m_ComboDecayTimer <= 0.0f)
			{
				ResetCombo();
			}

			// Have to put this here cause of the update enable/disable entity doesn't function correctly
			if (m_ComboNumber >= m_ComboThresholdSmall && m_ComboNumber < m_ComboThresholdLarge)
			{
				m_ComboActive = true;
				ECS.SetActive(m_ComboSmallFireID, true);
				ECS.SetActive(m_ComboLargeFireID, false);
			}
			else if (m_ComboNumber >= m_ComboThresholdLarge)
			{
				m_ComboActive = true;
				ECS.SetActive(m_ComboSmallFireID, false);
				ECS.SetActive(m_ComboLargeFireID, true);
			}
			else if (m_ComboNumber >= 1 && m_ComboNumber < m_ComboThresholdSmall)
			{
				m_ComboActive = true;
			}
			else if (m_ComboNumber == 0)
			{
				EnableComboUI(false);
			}

			// Combo is active
			if (m_ComboActive)
			{
				m_ComboDecayTimer -= dt;
				//ECS.SetScale(m_ComboBarID, Vector3.Lerp(m_ComboBarTrans.Scale, m_EmptyComboBarScale, 1.5f * dt));
				EnableComboUI(true);
				ECS.SetScale(m_ComboBarID, new Vector3(m_FullComboBarScale.X * (m_ComboDecayTimer / m_ComboDecayTimeLimit), m_FullComboBarScale.Y, m_FullComboBarScale.Z));
			}
			// Combo is inactive
			else
			{
				ECS.SetScale(m_ComboBarID, m_EmptyComboBarScale);
			}
		}

		private void UpdateLaser(float dt)
		{
			if (m_LaserTimer <= 0.0f && !m_LaserReady)
			{
				//Console.WriteLine("Special Ready");
				//ECS.SetActive(m_LaserWeaponID, true);
				m_LaserReady = true;
			}
			else
			{
				m_LaserTimer -= dt;
			}

			if (InputUtility.onKeyHeld("LASER") && m_LaserReady)
			{
				//Console.WriteLine("Special Used");
				m_EnableLaserAttack = true;
				m_LaserReady = false;
				m_LaserTimer = m_LaserCooldown;
				GameUtilities.PauseScene();
			}
			else if (InputUtility.onKeyHeld("LASER") && !m_LaserReady)
			{
				//Console.WriteLine("Special Not Ready");
			}
		}

		public static void AddScore(bool increment)
		{
			//Increase score
			if (increment)
			{
				// Just change the color first
				GameUtilities.UpdateTextColor(m_ScoreTextID, new Vector3(0.0f, 1.0f, 0.0f));

				m_OldScore = score;

				++PlayerScript.m_EnemyDestroyedCount;
				// Max 99
				if (PlayerScript.m_ComboNumber < 99)
					++PlayerScript.m_ComboNumber;

				// Start the combo decay timer
				PlayerScript.m_ComboDecayTimer = PlayerScript.m_ComboDecayTimeLimit;
				ECS.SetScale(PlayerScript.m_ComboBarID, PlayerScript.m_FullComboBarScale);

				if (PlayerScript.m_ComboActive)
				{
					if (PlayerScript.m_ComboNumber >= m_ComboThresholdSmall && PlayerScript.m_ComboNumber < m_ComboThresholdLarge)
					{
						ECS.PlayAudio(entity_id, 1, "SFX");
						ECS.SetActive(PlayerScript.m_ComboSmallFireID, true);
						ECS.SetActive(PlayerScript.m_ComboLargeFireID, false);
					}
					else if (PlayerScript.m_ComboNumber >= m_ComboThresholdLarge)
					{
						ECS.PlayAudio(entity_id, 2, "SFX");
						ECS.SetActive(PlayerScript.m_ComboSmallFireID, false);
						ECS.SetActive(PlayerScript.m_ComboLargeFireID, true);
					}
					else
					{
						ECS.PlayAudio(entity_id, 0, "SFX");
					}

					uint addscore = kill_score * PlayerScript.m_ScoreMultiplierBobbleCount * PlayerScript.m_ComboNumber;
					m_AddScoreListIDs.Add(GameUtilities.UpdateScore_AddMinus(DashboardScreenID, addscore, true));
					m_ScoreResetTimer = 0.01f;

					score += addscore;
				}
				else
				{
					ECS.PlayAudio(entity_id, 0, "SFX");

					uint addscore = kill_score * PlayerScript.m_ScoreMultiplierBobbleCount;
					m_AddScoreListIDs.Add(GameUtilities.UpdateScore_AddMinus(DashboardScreenID, addscore, true));
					m_ScoreResetTimer = 0.01f;

					score += addscore;
				}

				//GameUtilities.UpdateDashboardFace(DashboardScreenID, 2);

			}
			//Decrease score
			else
			{
				// Just change the color first
				GameUtilities.UpdateTextColor(m_ScoreTextID, new Vector3(1.0f, 0.0f, 0.0f));
				if (score > minus_score)
				{
					score -= minus_score;
					m_AddScoreListIDs.Add(GameUtilities.UpdateScore_AddMinus(DashboardScreenID, minus_score, false));
				}
				m_ScoreResetTimer = 0.01f;
				ResetCombo();
			}

			GameUtilities.UpdateComboUI(PlayerScript.m_ComboNumberID, PlayerScript.m_ComboNumber);
		}

		private static void ResetCombo()
		{
			PlayerScript.m_ComboNumber = 0;
			ECS.SetScale(PlayerScript.m_ComboBarID, PlayerScript.m_EmptyComboBarScale);
			PlayerScript.m_ComboDecayTimer = 0.0f;
			PlayerScript.m_ComboActive = false;
			ECS.SetActive(PlayerScript.m_ComboSmallFireID, false);
			ECS.SetActive(PlayerScript.m_ComboLargeFireID, false);
			EnableComboUI(false);
			GameUtilities.UpdateComboUI(PlayerScript.m_ComboNumberID, PlayerScript.m_ComboNumber);
		}

		private static void EnableComboUI(bool isEnable)
		{
			ECS.SetActive(PlayerScript.m_ComboTextID, isEnable);
			ECS.SetActive(PlayerScript.m_ComboXID, isEnable);
			ECS.SetActive(PlayerScript.m_ComboNumberID, isEnable);
		}

		private void UpdateScore(float dt)
		{
			if(m_ScoreResetTimer >= 0.01f)
				m_ScoreResetTimer += dt;

			Transform t = new Transform();

			foreach (int index in m_DestroyAddScoreListIndex)
			{

				if (index < m_AddScoreListIDs.Count && index >= 0)
				{
					ECS.DestroyEntity(m_AddScoreListIDs[index]);
					m_AddScoreListIDs.RemoveAt(index);
				}
			}
			m_DestroyAddScoreListIndex.Clear();

			for (int i = 0; i < m_AddScoreListIDs.Count; ++i)
			{
				ECS.GetTransformECS(m_AddScoreListIDs[i], ref t.Position, ref t.Rotation, ref t.Scale);
				ECS.SetScale(m_AddScoreListIDs[i], Vector3.Lerp(t.Scale, m_AddScoreEndScale, m_ScoreAnimationSpeed * dt));
				ECS.SetPosition(m_AddScoreListIDs[i], Vector3.Lerp(t.Position, m_AddScoreEndPos, m_ScoreAnimationSpeed * dt));

				if (t.Scale.X <= m_AddScoreEndScale.X + 0.2f)
				{
					m_DestroyAddScoreListIndex.Add(i);
				}
			}

			if (m_ScoreResetTimer >= m_ScoreResetTimeLimit)
			{
				GameUtilities.UpdateTextColor(m_ScoreTextID, new Vector3(1.0f, 1.0f, 0.0f));
				m_ScoreResetTimer = 0.0f;
				m_OldScore = score;
			}
			else
			{
				if (score > m_OldScore)
					GameUtilities.UpdateScore(m_ScoreTextID, m_OldScore + (uint)((score - m_OldScore) * (m_ScoreResetTimer / m_ScoreResetTimeLimit)));
				else
					GameUtilities.UpdateScore(m_ScoreTextID, m_OldScore - (uint)((m_OldScore - score) * (m_ScoreResetTimer / m_ScoreResetTimeLimit)));
			}
		}
	}
}
