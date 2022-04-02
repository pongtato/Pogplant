﻿/******************************************************************************/
/*!
\file	L2BossBehaviour.cs
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details

	This file contains the code to handle the level 2's boss behaviour

\copyright	Copyright (c) 2022 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Scripting.Utilities;

namespace Scripting
{
	public class L2BossBehaviour : MonoBehaviour
	{
		public static L2BossBehaviour m_singleton;

		/**************************
		*
		* Variables
		*
		**************************/
		#region[Core Variables]
		/**> The health of the boss core*/
		public float mh_coreHealth;
		float mh_defaultHealth;

		public float mh_leftBallHealth;
		public float mh_rightBallHealth;

		/**> How much damage the boss will take before going into protection mode*/
		public float m_coreProtectionThreshold;

		/**> How much time before the damage threshold gets reset*/
		public float m_coreProtectionDamageDuration;

		//Shooting stuff
		public float m_bossShootingDelay = 0.6f;
		public float m_bossShootingCoolDown = 3f;
		public int m_bossShootCountLimit = 8;

		//IDs
		public uint mID_leftCore;
		public uint mID_rightCore;
		public uint mID_falseCore;
		public uint mID_falseCoreMouthL;
		public uint mID_falseCoreMouthR;
		public uint mID_playerShip;
		public uint[] mID_ventSpawnpoints = new uint[6];

		//HP Bar vars
		uint mID_hpBar;
		Vector3 m_bossHpScale;
		const float m_bossHPDefaultScaling = 2.0f;
		const float m_bossHPOffset = 0.1f;
		const float m_bossHPBarAnimateSpeed = 3.0f;
		float m_bossHPBarLerpValue;
		bool m_hasBossHPBarAnimated;
		public bool m_beginBossHPBarAnimation;

		/**************************
		*
		* Temporary for enemy spawns
		*
		**************************/
		private List<GameObject> m_enemySpawnInstances = new List<GameObject>();
		private bool c_spawnedEnemies = false;
		private bool c_playedSpawnAnimation = false;
		private float c_hardCodedUpAnimationSpeed = 12f;
		private float c_animationDuration = 2.5f;

		//--- Spawner variables
		//This is hardcoded second for spawner
		/**> How many seconds into the animation to choose to spawn enemies*/
		float mSpawner_timeStartSpawnEnemies = 4.2f;
		float mSpawner_durationToSpawn = 6f;
		float mSpawner_timeEndSpawnEnemies = 7f;
		#endregion

		/**************************
		*
		* Structs
		*
		**************************/
		#region[Structures]
		public struct BossStateBehaviour
		{
			//Default constructor
			BossStateBehaviour(int defaultVal = 0)
			{
				shouldReturnToDefault = true;
				isVulnerable = true;
				stateDurationMin = 5f;
				stateDurationMax = 10f;
			}

			/**> Whether the state should return back to default state*/
			public bool shouldReturnToDefault;
			public bool isVulnerable;

			/**> How long it should stay in this state, randomised from min to max*/
			public float stateDurationMin;
			public float stateDurationMax;
		}

		public struct TurretGun
		{
			public uint ID_turretBody;
			public uint ID_shootPoint;
			public uint ID_laserObject;
			public uint ID_laserOutline;
			public uint ID_laserHitbox;

			public bool shouldLerp;

			public Vector3 restingRotation;

			public Vector3 targetRotation;

			public float health;
		}

		struct RuntimeStateVariables
		{
			RuntimeStateVariables(int defaultVal = 0)
			{
				lastState = L2Boss.BOSS_BEHAVIOUR_STATE.EMPTY;
				timer = 0f;
				stateDuration = 0f;
				timeSinceLastDamageTimer = 0f;
				damageTakenPeriod = 0f;
				bossShootTimer = 0f;
				bossShootCount = 0;
				bossShootReloading = false;
				bossTurretAimState = 0;
				bossTurretShouldFire = false;
				leftBallHealth = 0f;
				rightBallHealth = 0f;
				canDamageSideCores = false;
				canDamageMainCore = false;
				lastRNGState = 0;
			}

			public L2Boss.BOSS_BEHAVIOUR_STATE lastState;

			public float timer;
			public float stateDuration;
			public float timeSinceLastDamageTimer;

			/**> Damage taken in this period of time*/
			public float damageTakenPeriod;

			public float bossShootTimer;
			public float bossShootCount;
			public bool bossShootReloading;
			public int bossTurretAimState;
			public bool bossTurretShouldFire;

			public float leftBallHealth;
			public float rightBallHealth;

			public bool canDamageSideCores;
			public bool canDamageMainCore;

			/**> The last state randomised, doesn't allow same number again*/
			public int lastRNGState;
		}
		#endregion

		RuntimeStateVariables m_runStateInfo;
		BossStateBehaviour[] m_stateBehaviours = new BossStateBehaviour[(int)L2Boss.BOSS_BEHAVIOUR_STATE.TOTAL];
		TurretGun[] m_turretGuns = new TurretGun[2];

		Transform m_tempTransform;

		#region[Laser Stuff]
		public float mLaser_timer = 0f;
		bool mLaser_useOnce = false;

		Vector3 mLaserBounds_TL;
		Vector3 mLaserBounds_TR;
		Vector3 mLaserBounds_BL;
		Vector3 mLaserBounds_BR;

		#endregion

		#region[Damage Animations]
		Vector3 mColor_falseCoreNormal = new Vector3(1f, 0.725f, 0f);
		Vector3 mColor_falseCoreCurrent = new Vector3(1f, 0.725f, 0f);
		Vector3 mColor_falseCoreDamaged = new Vector3(1f, 0f, 0f);

		Vector3 mColor_falseCoreCoverNormal = new Vector3(1f, 1f, 1f);
		Vector3 mColor_falseCoreCoverCurrent = new Vector3(1f, 1f, 1f);
		Vector3 mColor_falseCoreCoverDamaged = new Vector3(0.25f, 0.25f, 0.25f);
		#endregion

		/******************************************************************************/
		/*!
		\brief
			Init for entity and singleton
		*/
		/******************************************************************************/
		public override void Init(ref uint _entityID)
		{
			entityID = _entityID;
			m_singleton = this;
		}

		/******************************************************************************/
		/*!
		\brief
			Start function to initialise variables/etc
		*/
		/******************************************************************************/
		public override void Start()
		{
			//mh_coreHealth = ECS.GetValue<float>(entityID, 200f, "CoreHealth");
			mh_defaultHealth = ECS.GetValue<float>(entityID, 200f, "CoreHealth");
			mh_leftBallHealth = ECS.GetValue<float>(entityID, 70f, "LeftBallHealth");
			mh_rightBallHealth = ECS.GetValue<float>(entityID, 70f, "RightBallHealth");

			m_coreProtectionThreshold = ECS.GetValue<float>(entityID, 60f, "CoreProtectionThreshold");
			m_coreProtectionDamageDuration = ECS.GetValue<float>(entityID, 5f, "CoreProtectionDamageDuration");

			mID_playerShip = ECS.FindEntityWithName("PlayerShip");

			mID_leftCore = ECS.FindEntityWithName("Left_Eye");
			mID_rightCore = ECS.FindEntityWithName("Right_Eye");
			mID_falseCore = ECS.FindEntityWithName("FalseCore");
			mID_falseCoreMouthL = ECS.FindChildEntityWithName(mID_falseCore, "Mouth_L");
			mID_falseCoreMouthR = ECS.FindChildEntityWithName(mID_falseCore, "Mouth_R");
			ECS.SetEmissiveTint(mID_falseCore, ref mColor_falseCoreNormal);

			m_turretGuns[0].ID_turretBody = ECS.FindEntityWithName("Left_ColourTurret_Body_Pivot");
			m_turretGuns[1].ID_turretBody = ECS.FindEntityWithName("Right_ColourTurret_Body_Pivot");

			uint bossPanelSpawns = ECS.FindEntityWithName("BossPanelSpawnPoints");

			for (int i = 0; i < 6; ++i)
			{
				mID_ventSpawnpoints[i] = ECS.FindChildEntityWithName(bossPanelSpawns, (i + 1).ToString());
			}

			//Init turrets
			for (int i = 0; i < 2; ++i)
			{
				m_turretGuns[i].ID_shootPoint = ECS.FindChildEntityWithName(m_turretGuns[i].ID_turretBody, "Shoot");
				m_turretGuns[i].ID_laserObject = ECS.FindChildEntityWithName(m_turretGuns[i].ID_turretBody, "LaserObject");
				m_turretGuns[i].ID_laserOutline = ECS.FindChildEntityWithName(m_turretGuns[i].ID_turretBody, "LaserOutLine");
				m_turretGuns[i].ID_laserHitbox = ECS.FindChildEntityWithName(m_turretGuns[i].ID_turretBody, "LaserHitbox");

				ECS.GetTransformECS(m_turretGuns[i].ID_turretBody, ref m_tempTransform.Position, ref m_tempTransform.Rotation, ref m_tempTransform.Scale);
				m_turretGuns[i].restingRotation = m_tempTransform.Rotation;

				m_turretGuns[i].targetRotation.Z = 0f;
				m_turretGuns[i].shouldLerp = true;
				m_turretGuns[i].health = mh_leftBallHealth;

				ECS.SetActive(m_turretGuns[i].ID_laserObject, false);
				ECS.SetActive(m_turretGuns[i].ID_laserOutline, false);
				ECS.SetActive(m_turretGuns[i].ID_laserHitbox, false);
			}

			//Get laser positions;
			uint laserBounds = ECS.FindEntityWithName("BossLaserTargetBounds");
			mLaserBounds_TL = ECS.GetGlobalPosition(ECS.FindChildEntityWithName(laserBounds, "TL"));
			mLaserBounds_TR = ECS.GetGlobalPosition(ECS.FindChildEntityWithName(laserBounds, "TR"));
			mLaserBounds_BL = ECS.GetGlobalPosition(ECS.FindChildEntityWithName(laserBounds, "BL"));
			mLaserBounds_BR = ECS.GetGlobalPosition(ECS.FindChildEntityWithName(laserBounds, "BR"));

			m_runStateInfo.canDamageSideCores = false;
			m_runStateInfo.canDamageMainCore = true;

			InitStateBehaviours();

			//Init default hp scale
			mID_hpBar = ECS.FindEntityWithName("HP_Bar_Red");
			m_bossHpScale = new Vector3(2.1f, 0.3f, 1.0f);
			m_bossHPBarLerpValue = 0.0f;
		}

		void InitStateBehaviours()
		{
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.MOVING].isVulnerable = true;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.MOVING].shouldReturnToDefault = false;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.MOVING].stateDurationMin = 1f;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.MOVING].stateDurationMax = 5f;

			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.REPEL_ATTACK].isVulnerable = false;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.REPEL_ATTACK].shouldReturnToDefault = true;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.REPEL_ATTACK].stateDurationMin = 6f;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.REPEL_ATTACK].stateDurationMax = 6f;

			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK].isVulnerable = false;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK].shouldReturnToDefault = true;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK].stateDurationMin = 9.1f;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK].stateDurationMax = 9.1f;

			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.STUN_FIELD].isVulnerable = false;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.STUN_FIELD].shouldReturnToDefault = true;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.STUN_FIELD].stateDurationMin = 9.1f;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.STUN_FIELD].stateDurationMax = 9.1f;

			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].isVulnerable = false;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].shouldReturnToDefault = true;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].stateDurationMin = 20f;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].stateDurationMax = 20f;

			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].isVulnerable = false;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].shouldReturnToDefault = true;

			m_runStateInfo.stateDuration = 8.8f;
		}


		/******************************************************************************/
		/*!
		\brief
			Default boss update

		\param dt
			Delta time
		*/
		/******************************************************************************/
		public override void Update(float dt)
		{
			//Debug test codes
			{
				if (InputUtility.onKeyHeld(KEY_ID.KEY_LEFT_SHIFT))
				{
					if (InputUtility.onKeyTriggered(KEY_ID.KEY_G))
					{
						GameUtilities.LoadScene("EndGameCutscene");
					}
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_0))
				{
					Console.WriteLine("L2BossBehaviour.cs: Boss state is: " + L2Boss.m_singleton.current_state);
					Console.WriteLine("L2BossBehaviour.cs: Timer is: " + m_runStateInfo.timer);
					Console.WriteLine("L2BossBehaviour.cs: AimState is: " + m_runStateInfo.bossTurretAimState);
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_J))
				{
					TriggerNextState(L2Boss.BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK);
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_K))
				{
					TriggerNextState(L2Boss.BOSS_BEHAVIOUR_STATE.VACUUM_ATTACK);
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_L))
				{
					TriggerNextState(L2Boss.BOSS_BEHAVIOUR_STATE.REPEL_ATTACK);
				}
			}

			//Update shooting gun behaviour
			UpdateGunBehaviour(dt);

			m_runStateInfo.bossTurretAimState = 0;
			m_runStateInfo.bossTurretShouldFire = false;

			switch (L2Boss.m_singleton.current_state)
			{
				case L2Boss.BOSS_BEHAVIOUR_STATE.MOVING:
				{
					//m_runStateInfo.bossTurretReady = true;
					//m_runStateInfo.bossTurretShouldFire = true;

					if (m_runStateInfo.timer > m_runStateInfo.stateDuration)
					{
						int stateSelection;

						do
						{
							stateSelection = PPMath.RandomInt(0, 1);
						}
						while (m_runStateInfo.lastRNGState == stateSelection);

						switch (stateSelection)
						{
							case 0:
								//Temp 2 shoot modes
								if (PPMath.RandomInt(0, 1) == 0)
									TriggerNextState(L2Boss.BOSS_BEHAVIOUR_STATE.REPEL_ATTACK);
								else
									TriggerNextState(L2Boss.BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK);
								break;
							case 1:
								TriggerNextState(L2Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS);
								break;
							case 2:
								TriggerNextState(L2Boss.BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK);
								break;
							default:
								Console.WriteLine("L2BossBehaviour.cs: RNG out of range");
								break;
						}

						m_runStateInfo.lastRNGState = stateSelection;
					}
				}
				break;
				case L2Boss.BOSS_BEHAVIOUR_STATE.REPEL_ATTACK:
				{
					m_runStateInfo.bossTurretAimState = 1;
					m_runStateInfo.bossTurretShouldFire = true;
				}
				break;
				case L2Boss.BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK:
				{
					m_runStateInfo.bossTurretAimState = 2;
					m_runStateInfo.bossTurretShouldFire = true;
				}
				break;

				case L2Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS:
				{
					if (m_runStateInfo.timer > mSpawner_timeStartSpawnEnemies && m_runStateInfo.timer < mSpawner_timeEndSpawnEnemies)
					{
						if (m_runStateInfo.timer > mSpawner_durationToSpawn && !c_spawnedEnemies)
						{
							c_spawnedEnemies = true;
							int randomInt = PPMath.RandomInt(0, 5);

							Console.WriteLine("L1BossBehaviour.cs: SPAWNING ENEMIES Randomed: " + randomInt);

							switch (randomInt)
							{
								case 0:
									EncounterSystemDriver.m_singleton.SpawnWave("Boss_Low1");
									break;
								case 1:
									EncounterSystemDriver.m_singleton.SpawnWave("Boss_Low2");
									break;
								case 2:
									EncounterSystemDriver.m_singleton.SpawnWave("Boss_Med1");
									break;
								case 3:
									EncounterSystemDriver.m_singleton.SpawnWave("Boss_Med2");
									break;
								case 4:
									EncounterSystemDriver.m_singleton.SpawnWave("Boss_High1");
									break;
								case 5:
									EncounterSystemDriver.m_singleton.SpawnWave("Boss_High2");
									break;
								default:
									Console.WriteLine("THIS ISN'T SUPPOSED TO HAPPEN");
									break;
							}
						}

						if (!c_playedSpawnAnimation)
						{
							c_playedSpawnAnimation = true;

							Console.WriteLine("L2BossBehaviour.cs: SpawnAnimation");

							//Do enemy spawn animation
							for (int i = 0; i < 6; ++i)
							{
								m_tempTransform.Rotation = ECS.GetGlobalRotation(mID_ventSpawnpoints[i]);
								m_tempTransform.Position = ECS.GetGlobalPosition(mID_ventSpawnpoints[i]);
								GameObject instance = GameUtilities.InstantiateObject("Enemy_01", m_tempTransform.Position, m_tempTransform.Rotation);
								m_enemySpawnInstances.Add(instance);
							}
						}

						UpdateEnemySpawnAnimation(dt);
					}
				}
				break;
			}

			UpdateLockOnBehaviour();


			//Enter protection mode if damager threshold is reached
			if (m_runStateInfo.damageTakenPeriod > m_coreProtectionThreshold)
			{
				TriggerProtectionState();
			}

			//Timers
			m_runStateInfo.timer += dt;
			m_runStateInfo.timeSinceLastDamageTimer += dt;

			//When Past the state duration threshold, it will return back to moving state
			if (m_runStateInfo.timer > m_runStateInfo.stateDuration)
			{
				if (m_stateBehaviours[(int)L2Boss.m_singleton.current_state].shouldReturnToDefault)
				{
					TriggerNextState(L2Boss.BOSS_BEHAVIOUR_STATE.MOVING);
				}
			}

			//Damage threshold reset if boss doesn't take damage for awhile
			if (m_runStateInfo.timeSinceLastDamageTimer > m_coreProtectionDamageDuration)
			{
				m_runStateInfo.damageTakenPeriod = 0f;
			}

			//Update the hp bar to lerp when the boss first appears
			if (!m_hasBossHPBarAnimated && m_beginBossHPBarAnimation)
			{
				m_bossHPBarLerpValue += m_bossHPBarAnimateSpeed * dt;
				mh_coreHealth = PPMath.Lerp(mh_coreHealth, mh_defaultHealth, m_bossHPBarLerpValue);
				if (mh_coreHealth >= mh_defaultHealth)
				{
					m_hasBossHPBarAnimated = true;
				}
			}

			UpdateDamageColors(dt);
			UpdateHPBar(dt);
		}

		bool updateCoreLockOn = false;
		void UpdateLockOnBehaviour()
		{
			//if Is invulnerable to main core
			if ((!m_stateBehaviours[(int)L2Boss.m_singleton.current_state].isVulnerable || !m_runStateInfo.canDamageMainCore) && updateCoreLockOn)
			{
				updateCoreLockOn = false;

				//Remove core from lock on list
				FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Remove(entityID);
				FirstPersonFiringSystem.RemoveEnemyFromListOfTargets(entityID, 0);
			}
			//If is vulnerable
			else if (!updateCoreLockOn && m_stateBehaviours[(int)L2Boss.m_singleton.current_state].isVulnerable && m_runStateInfo.canDamageMainCore)
			{
				updateCoreLockOn = true;

				//Add core to lock on list
				FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(entityID, true);
			}
		}

		/***************************************************************************/
		/*!
		\brief
			Updates the movement of the fake enemy ships

		\param dt
			Delta time
		*/
		/***************************************************************************/
		void UpdateEnemySpawnAnimation(float dt)
		{
			if (m_runStateInfo.timer > c_animationDuration + mSpawner_timeStartSpawnEnemies)
			{
				if (m_enemySpawnInstances.Count > 0)
				{
					for (int i = 0; i < m_enemySpawnInstances.Count; ++i)
					{
						ECS.DestroyEntity(m_enemySpawnInstances[i].id);
					}

					m_enemySpawnInstances.Clear();

					Console.WriteLine("L2BossBehaviour.cs: Erased animation");
				}
			}
			else
			{
				for (int i = 0; i < m_enemySpawnInstances.Count; ++i)
				{
					ECS.GetTransformECS(m_enemySpawnInstances[i].id, ref m_tempTransform.Position, ref m_tempTransform.Rotation, ref m_tempTransform.Scale);

					m_tempTransform.Position.Y += c_hardCodedUpAnimationSpeed * dt;

					ECS.SetTransformECS(m_enemySpawnInstances[i].id, m_tempTransform.Position, m_tempTransform.Rotation, m_tempTransform.Scale);
				}
			}
		}

		//Used to update colors
		void UpdateDamageColors(float dt)
		{
			mColor_falseCoreCurrent = Vector3.Lerp(mColor_falseCoreCurrent, mColor_falseCoreNormal, Math.Min(dt * 15f, 1f));
			ECS.SetEmissiveTint(mID_falseCore, ref mColor_falseCoreCurrent);

			mColor_falseCoreCoverCurrent = Vector3.Lerp(mColor_falseCoreCoverCurrent, mColor_falseCoreCoverNormal, Math.Min(dt * 15f, 1f));
			ECS.SetDiffuseTint(mID_falseCoreMouthL, ref mColor_falseCoreCoverCurrent);
			ECS.SetDiffuseTint(mID_falseCoreMouthR, ref mColor_falseCoreCoverCurrent);
		}

		public override void LateUpdate(float dt)
		{

		}

		void UpdateGunBehaviour(float dt)
		{
			switch (m_runStateInfo.bossTurretAimState)
			{
				//Resting state
				case 0:
				{
					for (int i = 0; i < 2; ++i)
					{
						ECS.GetTransformECS(m_turretGuns[i].ID_turretBody, ref m_tempTransform.Position, ref m_tempTransform.Rotation, ref m_tempTransform.Scale);
						m_tempTransform.Rotation = Vector3.Lerp(m_tempTransform.Rotation, m_turretGuns[i].restingRotation, dt * 2f);
						ECS.SetRotation(m_turretGuns[i].ID_turretBody, m_tempTransform.Rotation);
					}
				}
				break;

				//Aim at player
				case 1:
				{
					UpdateAimAtPlayerFireBehaviour(dt);
				}
				break;

				//Sweep attack
				case 2:
				{
					UpdateLaserAttack(dt);
				}
				break;//*/
			}
		}

		void UpdateAimAtPlayerFireBehaviour(float dt)
		{
			Vector3 shipPos = ECS.GetGlobalPosition(mID_playerShip);

			//Lerp gun to target player
			for (int i = 0; i < 2; ++i)
			{
				ECS.GetTransformECS(m_turretGuns[i].ID_turretBody, ref m_tempTransform.Position, ref m_tempTransform.Rotation, ref m_tempTransform.Scale);
				Vector3 beforeLerp = m_tempTransform.Rotation;
				Transform.LookAt(m_turretGuns[i].ID_turretBody, shipPos);
				ECS.GetTransformECS(m_turretGuns[i].ID_turretBody, ref m_tempTransform.Position, ref m_tempTransform.Rotation, ref m_tempTransform.Scale);

				m_tempTransform.Rotation = Vector3.Lerp(beforeLerp, m_tempTransform.Rotation, dt * 10f);

				ECS.SetRotation(m_turretGuns[i].ID_turretBody, m_tempTransform.Rotation);
			}

			//Fire
			if (m_runStateInfo.bossTurretShouldFire)
			{
				m_runStateInfo.bossShootTimer += dt;

				if (m_runStateInfo.bossShootTimer > m_bossShootingDelay && !m_runStateInfo.bossShootReloading)
				{
					m_runStateInfo.bossShootTimer = 0f;
					m_runStateInfo.bossShootCount++;

					if (m_runStateInfo.bossShootCount > m_bossShootCountLimit)
					{
						m_runStateInfo.bossShootCount = 0;
						m_runStateInfo.bossShootTimer = 0f;
						m_runStateInfo.bossShootReloading = true;

						L2Boss.m_singleton.SetColorTurretRecovery();
					}
					else
					{
						L2Boss.m_singleton.SetColorTurretFiring();
						ECS.PlayAudio(entityID, 3, "SFX");

						InputUtility.VibrateControllerHeavyMotor(1f, 0.15f);

						for (int i = 0; i < 2; ++i)
						{
							GameUtilities.FireEnemyBullet(
								m_turretGuns[i].ID_shootPoint,
								ECS.GetGlobalPosition(m_turretGuns[i].ID_shootPoint),
								Transform.GetForwardVector(m_turretGuns[i].ID_shootPoint), 22.0f, 3.0f, false, 3f);
						}
					}
				}
				else if (m_runStateInfo.bossShootReloading)
				{
					if (m_runStateInfo.bossShootTimer > m_bossShootingCoolDown)
					{
						m_runStateInfo.bossShootReloading = false;
						m_runStateInfo.bossShootTimer = -0.2f;

						L2Boss.m_singleton.SetTurretColors();
						L2Boss.m_singleton.SetColorTurretPreparingFire();
					}
				}
			}
		}

		void UpdateLaserAttack(float dt)
		{
			Vector3 shipPos = ECS.GetGlobalPosition(mID_playerShip) - new Vector3(0, 0.55f, 0);

			if (m_runStateInfo.bossShootTimer < 5.85f)
			{
				//Aim/lerp to player
				for (int i = 0; i < 2; ++i)
				{
					if (m_turretGuns[i].shouldLerp)
					{
						Vector3 beforeLerp = ECS.GetRotation(m_turretGuns[i].ID_turretBody);
						Transform.LookAt(m_turretGuns[i].ID_turretBody, shipPos);
						m_tempTransform.Rotation = Vector3.Lerp(beforeLerp, ECS.GetRotation(m_turretGuns[i].ID_turretBody), dt * 10f);

						ECS.SetRotation(m_turretGuns[i].ID_turretBody, m_tempTransform.Rotation);
					}
				}
			}

			//Fire
			if (m_runStateInfo.bossTurretShouldFire)
			{
				m_runStateInfo.bossShootTimer += dt;

				if (!m_runStateInfo.bossShootReloading)
				{
					//Wait for the audio to play then turn on the laser LOL


					if (m_runStateInfo.bossShootTimer > 6f && m_runStateInfo.bossShootTimer < 7f)
					{
						for (int i = 0; i < 2; ++i)
						{
							ECS.SetActive(m_turretGuns[i].ID_laserOutline, true);
							ECS.SetActive(m_turretGuns[i].ID_laserHitbox, true);

							ECS.SetScale(m_turretGuns[i].ID_laserOutline, Vector3.Lerp(ECS.GetScale(m_turretGuns[i].ID_laserOutline), new Vector3(0.9f, 0.9f, 3f), dt * 3f));
							ECS.SetScale(m_turretGuns[i].ID_laserObject, Vector3.Lerp(ECS.GetScale(m_turretGuns[i].ID_laserObject), new Vector3(0.8f, 0.8f, 3f), dt * 3f));
						}
					}
					else if (m_runStateInfo.bossShootTimer > 7f && !mLaser_useOnce)
					{
						for (int i = 0; i < 2; ++i)
						{
							ECS.SetActive(m_turretGuns[i].ID_laserHitbox, false);
							ECS.SetScale(m_turretGuns[i].ID_laserOutline, Vector3.Lerp(ECS.GetScale(m_turretGuns[i].ID_laserOutline), new Vector3(0.0f, 0.0f, 3f), dt * 6f));
							ECS.SetScale(m_turretGuns[i].ID_laserObject, Vector3.Lerp(ECS.GetScale(m_turretGuns[i].ID_laserObject), new Vector3(0.0f, 0.0f, 3f), dt * 6f));
						}

						if (m_runStateInfo.bossShootTimer > 7.6f)
						{
							mLaser_useOnce = true;

							L2Boss.m_singleton.SetColorTurretRecovery();
							for (int i = 0; i < 2; ++i)
							{
								ECS.SetActive(m_turretGuns[i].ID_laserObject, false);
								ECS.SetActive(m_turretGuns[i].ID_laserOutline, false);
							}
						}
					}
				}
				else if (m_runStateInfo.bossShootReloading)
				{
					if (m_runStateInfo.bossShootTimer > m_bossShootingCoolDown)
					{
						m_runStateInfo.bossShootReloading = false;
						m_runStateInfo.bossShootTimer = -0.2f;

						L2Boss.m_singleton.SetTurretColors();
						L2Boss.m_singleton.SetColorTurretPreparingFire();

						ECS.PlayAudio(entityID, 4, "SFX");

						for (int i = 0; i < 2; ++i)
						{
							ECS.SetActive(m_turretGuns[i].ID_laserObject, true);
							ECS.SetScale(m_turretGuns[i].ID_laserOutline, new Vector3(0f, 0f, 3f));
							ECS.SetScale(m_turretGuns[i].ID_laserObject, new Vector3(0.1f, 0.1f, 3f));

							m_turretGuns[i].shouldLerp = true;
						}

						mLaser_useOnce = false;
					}
				}
			}
		}

		void TriggerNextState(L2Boss.BOSS_BEHAVIOUR_STATE nextState, bool forceNonReturn = false, bool dontResetTimer = false)
		{
			m_runStateInfo.lastState = L2Boss.m_singleton.current_state;
			L2Boss.m_singleton.SetState(nextState.ToString());


			switch (L2Boss.m_singleton.current_state)
			{
				case L2Boss.BOSS_BEHAVIOUR_STATE.VACUUM_ATTACK:
				case L2Boss.BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK:
				case L2Boss.BOSS_BEHAVIOUR_STATE.REPEL_ATTACK:
				{
					m_runStateInfo.bossShootReloading = true;
					m_runStateInfo.bossShootTimer = m_bossShootingCoolDown;
					m_runStateInfo.bossShootCount = 0;
				}
				break;
				case L2Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS:
				{
					c_playedSpawnAnimation = false;
					c_spawnedEnemies = false;
				}
				break;
			}

			if (!dontResetTimer)
				m_runStateInfo.timer = 0f;

			if (forceNonReturn)
			{
				m_runStateInfo.stateDuration = float.PositiveInfinity;
			}
			else
			{
				m_runStateInfo.stateDuration = PPMath.RandomFloat(
					m_stateBehaviours[(int)nextState].stateDurationMin,
					m_stateBehaviours[(int)nextState].stateDurationMax);
			}
		}

		public void DamageLeftCore(float damageAmount)
		{
			if (m_runStateInfo.canDamageSideCores && L2Boss.m_singleton.left_ball_protection)
			{
				ECS.PlayAudio(entityID, 0, "SFX");

				//Console.WriteLine("L2BossBehaviour.cs: Left damage taken");
				m_runStateInfo.leftBallHealth -= damageAmount;

				if (m_runStateInfo.leftBallHealth <= 0f)
				{
					m_runStateInfo.leftBallHealth = 0f;

					//Trigger left core death
					L2Boss.m_singleton.DamagedLeftBall();

					//remove left core from lock on
					FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Remove(mID_leftCore);
					FirstPersonFiringSystem.RemoveEnemyFromListOfTargets(mID_leftCore, 0);

					if (!L2Boss.m_singleton.right_ball_protection)
						DestroyedBothCores();

					Console.WriteLine("L2BossBehaviour.cs: Left core dead");
				}
			}
			else
			{
				ECS.PlayAudio(entityID, 1, "SFX");
			}
		}

		public void DamageRightCore(float damageAmount)
		{
			if (m_runStateInfo.canDamageSideCores && L2Boss.m_singleton.right_ball_protection)
			{
				ECS.PlayAudio(entityID, 0, "SFX");

				//Console.WriteLine("Right damage taken");
				m_runStateInfo.rightBallHealth -= damageAmount;

				if (m_runStateInfo.rightBallHealth <= 0f)
				{
					m_runStateInfo.rightBallHealth = 0f;

					//Trigger right core death
					L2Boss.m_singleton.DamagedRightBall();

					//remove right core from lock on
					FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Remove(mID_rightCore);
					FirstPersonFiringSystem.RemoveEnemyFromListOfTargets(mID_rightCore, 0);

					if (!L2Boss.m_singleton.left_ball_protection)
						DestroyedBothCores();

					Console.WriteLine("L2BossBehaviour.cs: Right core dead");
				}
			}
			else
			{
				ECS.PlayAudio(entityID, 1, "SFX");
			}
		}

		public void DestroyedBothCores()
		{
			m_runStateInfo.canDamageMainCore = true;
			m_runStateInfo.canDamageSideCores = false;

			//reset health
			m_runStateInfo.leftBallHealth = mh_leftBallHealth;
			m_runStateInfo.rightBallHealth = mh_rightBallHealth;
		}

		public void TriggerProtectionState()
		{
			L2Boss.m_singleton.EnableProtection();
			m_runStateInfo.canDamageMainCore = false;
			m_runStateInfo.canDamageSideCores = true;

			m_runStateInfo.damageTakenPeriod = 0f;
			m_runStateInfo.leftBallHealth = mh_leftBallHealth;
			m_runStateInfo.rightBallHealth = mh_rightBallHealth;

			//add left and right cores to list
			FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(mID_leftCore, true);
			FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(mID_rightCore, true);

			Console.WriteLine("L2BossBehaviour.cs: Boss hit damage threshold, entering protection");
		}

		void UpdateHPBar(float dt)
		{
			//Hp Bar
			m_bossHpScale.X = PPMath.Lerp(m_bossHpScale.X, (((mh_coreHealth / mh_defaultHealth) * m_bossHPDefaultScaling) + m_bossHPOffset), m_bossHPBarAnimateSpeed * dt);
			ECS.SetScale(mID_hpBar, m_bossHpScale);
		}

		public override void OnTriggerEnter(uint id)
		{
			//Invulnerable in protection mode
			//Invulnerable if angle between mouth too low
			if (!m_stateBehaviours[(int)L2Boss.m_singleton.current_state].isVulnerable || !m_runStateInfo.canDamageMainCore
				|| Math.Abs(ECS.GetRotation(mID_falseCoreMouthL).Y) + Math.Abs(ECS.GetRotation(mID_falseCoreMouthR).Y) < 30f)
			{
				mColor_falseCoreCoverCurrent = mColor_falseCoreCoverDamaged;
				ECS.PlayAudio(entityID, 1, "SFX");
				return;
			}

			//Console.WriteLine("Boss Taken damage");
			ECS.PlayAudio(entityID, 0, "SFX");

			mColor_falseCoreCurrent = mColor_falseCoreDamaged;

			mh_coreHealth -= 1f;
			m_runStateInfo.damageTakenPeriod += 1f;
			m_runStateInfo.timeSinceLastDamageTimer = 0f;

			if (mh_coreHealth <= 0f)
			{
				mh_coreHealth = 0f;
				if (L2Boss.m_singleton.current_state != L2Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE)
				{
					Console.WriteLine("L2BossBehaviour.cs: Boss is dead, triggering sequence");
					//TriggerNextState(L2Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE);
					GameUtilities.LoadScene("EndGameCutscene");
				}
			}
		}

		public override void OnTriggerExit(uint id)
		{

		}
	}
}
