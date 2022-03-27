/******************************************************************************/
/*!
\file	L1BossBehaviour.cs
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details

	This file contains the code to handle the level 1's boss behaviour

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
	public class L1BossBehaviour : MonoBehaviour
	{
		public static L1BossBehaviour m_singleton;
		public bool m_debugMode = true;

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

		public float m_minMovingTime;
		public float m_maxMovingTime;

		public float m_minDurationBetweenSpawns;
		public float m_maxDurationBetweenSpawns;

		public float m_bossShootingDelay = 0.15f;
		public float m_bossShootingCoolDown = 1f;
		public int m_bossShootCountLimit = 20;

		public float m_gunBarrelYRotationLimit = 25f;
		public float m_gunBarrelYRotationSpeedMin = 10f;
		public float m_gunBarrelYRotationSpeedMax = 20f;

		public uint[] mID_ventSpawnpoints = new uint[6];

		public GunBarrel[] m_gunBarrels = new GunBarrel[6];

		public uint mID_leftCore;
		public uint mID_rightCore;

		//HP Bar vars
		uint mID_hpBar;
		Vector3 m_bossHpScale;
		const float m_bossHPDefaultScaling = 2.0f;
		const float m_bossHPOffset = 0.1f;
		const float m_bossHPBarAnimateSpeed = 3.0f;
		float m_bossHPBarLerpValue;
		bool m_hasBossHPBarAnimated;
		public bool m_beginBossHPBarAnimation;

		//--- Spawner variables

		//This is hardcoded second for spawner

		/**> How many seconds into the animation to choose to spawn enemies*/
		float mSpawner_timeStartSpawnEnemies = 4.2f;
		float mSpawner_timeEndSpawnEnemies = 7f;
		public float mSpawner_durationBetweenSpawns = 2f;

		//Hardcoded hitboxes
		uint mID_smashHitBox;
		uint mID_clapHitBox;
		bool m_hitboxEnableOnce = false;
		float mSmash_timeToEnableCollider = 4f;
		float mSmash_timeToDisableCollider = 4.2f;

		float mClap_timeToEnableCollider = 1.5f;
		float mClap_timeToDisableCollider = 2.0f;

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

		struct RuntimeStateVariables
		{
			RuntimeStateVariables(int defaultVal = 0)
			{
				lastState = L1Boss.BOSS_BEHAVIOUR_STATE.TOTAL;
				lastRNGState = -1;
				lastIsMelee = true;
				timer = 0f;
				secondaryTimer = 0f;
				stateDuration = 1f;
				timeSinceLastDamageTimer = 0f;
				timeToNextSpawn = 0f;
				timeSinceLastSpawnTimer = 90f;
				damageTakenPeriod = 0f;
				bossShootTimer = 0f;
				bossShootCount = 0;
				leftBallHealth = 0f;
				rightBallHealth = 0f;
				canDamageSideCores = false;
				canDamageMainCore = false;
			}

			public L1Boss.BOSS_BEHAVIOUR_STATE lastState;
			public int lastRNGState;
			public bool lastIsMelee;
			public float timer;
			public float secondaryTimer;
			public float stateDuration;
			public float timeSinceLastDamageTimer;
			public float timeToNextSpawn;
			public float timeSinceLastSpawnTimer;

			/**> Damage taken in this period of time*/
			public float damageTakenPeriod;

			public float bossShootTimer;
			public float bossShootCount;

			public float leftBallHealth;
			public float rightBallHealth;

			public bool canDamageSideCores;
			public bool canDamageMainCore;
		}

		public struct GunBarrel
		{
			public uint ID_barrel;
			public uint ID_shootPoint;
			public float rotationXLock;

			public bool rotationState;
			public float rotationSpeed;
		}
		#endregion

		RuntimeStateVariables m_runStateInfo;

		BossStateBehaviour[] m_stateBehaviours = new BossStateBehaviour[(int)L1Boss.BOSS_BEHAVIOUR_STATE.TOTAL];

		/**************************
		*
		* Temporary for enemy spawns
		*
		**************************/
		private List<GameObject> m_enemySpawnInstances = new List<GameObject>();
		Vector3 pos = new Vector3();
		Vector3 rot = new Vector3();
		Vector3 scale = new Vector3();
		private bool c_playedSpawnAnimation = false;
		private float c_hardCodedUpAnimationSpeed = 12f;
		private float c_animationDuration = 2.5f;

		bool updateCoreLockOn = false;

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
			//mh_coreHealth = ECS.GetValue<float>(entityID, 100f, "CoreHealth");
			mh_defaultHealth = ECS.GetValue<float>(entityID, 100f, "CoreHealth");
			mh_leftBallHealth = ECS.GetValue<float>(entityID, 50f, "LeftBallHealth");
			mh_rightBallHealth = ECS.GetValue<float>(entityID, 50f, "RightBallHealth");

			m_coreProtectionThreshold = ECS.GetValue<float>(entityID, 1f, "CoreProtectionThreshold");
			m_coreProtectionDamageDuration = ECS.GetValue<float>(entityID, 1f, "CoreProtectionDamageDuration");

			m_minMovingTime = ECS.GetValue<float>(entityID, 5f, "IdleTimeMin");
			m_maxMovingTime = ECS.GetValue<float>(entityID, 10f, "IdleTimeMax");

			m_minDurationBetweenSpawns = ECS.GetValue<float>(entityID, 10f, "DurationBetweenSpawnMin");
			m_maxDurationBetweenSpawns = ECS.GetValue<float>(entityID, 15f, "DurationBetweenSpawnMax");

			m_gunBarrelYRotationLimit = ECS.GetValue<float>(entityID, 25f, "GunBarrelRotationLimit");
			m_gunBarrelYRotationSpeedMin = ECS.GetValue<float>(entityID, 10f, "GunBarrelRotationSpeedMin");
			m_gunBarrelYRotationSpeedMax = ECS.GetValue<float>(entityID, 20f, "GunBarrelRotationSpeedMax");

			InitStateBehaviours();

			mID_leftCore = ECS.FindEntityWithName("Left_Eye");
			mID_rightCore = ECS.FindEntityWithName("Right_Eye");

			uint bossPanelSpawns = ECS.FindEntityWithName("BossPanelSpawnPoints");
			uint bossShootPoints = ECS.FindEntityWithName("Boss");

			for (int i = 0; i < 6; ++i)
			{
				mID_ventSpawnpoints[i] = ECS.FindChildEntityWithName(bossPanelSpawns, (i + 1).ToString());
			}
			
			m_gunBarrels[0].ID_barrel = ECS.FindChildEntityWithName(bossShootPoints, "Right_MiniLaser_Launcher_01");
			m_gunBarrels[1].ID_barrel = ECS.FindChildEntityWithName(bossShootPoints, "Right_MiniLaser_Launcher_02");
			m_gunBarrels[2].ID_barrel = ECS.FindChildEntityWithName(bossShootPoints, "Right_MiniLaser_Launcher_03");

			m_gunBarrels[3].ID_barrel = ECS.FindChildEntityWithName(bossShootPoints, "Left_MiniLaser_Launcher_01");
			m_gunBarrels[4].ID_barrel = ECS.FindChildEntityWithName(bossShootPoints, "Left_MiniLaser_Launcher_02");
			m_gunBarrels[5].ID_barrel = ECS.FindChildEntityWithName(bossShootPoints, "Left_MiniLaser_Launcher_03");

			for(int i = 0; i < 6; ++i)
			{
				m_gunBarrels[i].rotationState = (PPMath.RandomInt(0, 1) == 0);
				m_gunBarrels[i].rotationSpeed = PPMath.RandomFloat(m_gunBarrelYRotationSpeedMin, m_gunBarrelYRotationSpeedMax);
				m_gunBarrels[i].rotationXLock = 0f;
				m_gunBarrels[i].ID_shootPoint = ECS.FindChildEntityWithName(m_gunBarrels[i].ID_barrel, "Shoot");
			}

			mID_smashHitBox = ECS.FindChildEntityWithName(bossShootPoints, "SmashHitbox");
			mID_clapHitBox = ECS.FindChildEntityWithName(bossShootPoints, "ClapHitbox");

			//Hardcode right and left 1 launcher to shoot around player angle
			m_gunBarrels[0].rotationXLock = 20f;
			m_gunBarrels[3].rotationXLock = 20f;

			m_runStateInfo.leftBallHealth = mh_leftBallHealth;
			m_runStateInfo.rightBallHealth = mh_rightBallHealth;

			m_runStateInfo.canDamageMainCore = true;
			m_runStateInfo.canDamageSideCores = false;

			//Init default hp scale
			mID_hpBar = ECS.FindEntityWithName("HP_Bar_Red");
			m_bossHpScale = new Vector3(0.1f, 0.3f, 1.0f);
			m_bossHPBarLerpValue = 0.0f;
		}

		void InitStateBehaviours()
		{
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].isVulnerable = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].shouldReturnToDefault = true;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].stateDurationMin = 20.5f;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].stateDurationMax = 20.5f;

			m_runStateInfo.stateDuration = 8.8f;

			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.MOVING].isVulnerable = true;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.MOVING].shouldReturnToDefault = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.MOVING].stateDurationMin = m_minMovingTime;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.MOVING].stateDurationMax = m_maxMovingTime;


			//m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION].isVulnerable = false;
			//m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION].shouldReturnToDefault = false;

			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].isVulnerable = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].shouldReturnToDefault = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].stateDurationMin = 20f;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].stateDurationMax = 20f;

			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK].isVulnerable = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK].shouldReturnToDefault = true;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK].stateDurationMin = 13.3f;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK].stateDurationMax = 13.3f;

			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.CLAP_ATTACK].isVulnerable = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.CLAP_ATTACK].shouldReturnToDefault = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.CLAP_ATTACK].stateDurationMin = 12f;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.CLAP_ATTACK].stateDurationMax = 12f;

			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SMASH_ATTACK].isVulnerable = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SMASH_ATTACK].shouldReturnToDefault = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SMASH_ATTACK].stateDurationMin = 12f;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SMASH_ATTACK].stateDurationMax = 12f;

			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE].isVulnerable = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE].shouldReturnToDefault = false;
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
			if (m_debugMode)
			{
				//Debug stuff
				if (InputUtility.onKeyTriggered(KEY_ID.KEY_H))
				{
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.SMASH_ATTACK);
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_J))
				{
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE);
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_K))
				{
					//DestroyedBothCores();
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK);
					Console.WriteLine("L1BossBehaviour.cs: Triggered Spinning");
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_L))
				{
					//DestroyedBothCores();
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.CLAP_ATTACK);
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_0))
				{
					Console.WriteLine("L1BossBehaviour.cs: Boss state is: " + L1Boss.m_singleton.current_state);
					Console.WriteLine("L1BossBehaviour.cs: Timer is: " + m_runStateInfo.timer);
				}
				//return;
			}

			//if Is invulnerable to main core
			if ((!m_stateBehaviours[(int)L1Boss.m_singleton.current_state].isVulnerable || !m_runStateInfo.canDamageMainCore) && updateCoreLockOn)
			{
				updateCoreLockOn = false;

				//Remove core from lock on list
				FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Remove(entityID);
				FirstPersonFiringSystem.RemoveEnemyFromListOfTargets(entityID, 0);
			}
			//If is vulnerable
			else if(!updateCoreLockOn && m_stateBehaviours[(int)L1Boss.m_singleton.current_state].isVulnerable && m_runStateInfo.canDamageMainCore)
			{
				updateCoreLockOn = true;
				FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(entityID, true);
			}

			//Timers
			m_runStateInfo.timer += dt;
			m_runStateInfo.timeSinceLastDamageTimer += dt;
			m_runStateInfo.timeSinceLastSpawnTimer += dt;


			switch (L1Boss.m_singleton.current_state)
			{
				case L1Boss.BOSS_BEHAVIOUR_STATE.MOVING:
				{
					if (m_runStateInfo.timer > m_runStateInfo.stateDuration)
					{
						//When timer is hit
						if (m_runStateInfo.timeSinceLastSpawnTimer > m_runStateInfo.timeToNextSpawn)
						{
							if (m_runStateInfo.lastIsMelee)
							{
								TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS);

								c_playedSpawnAnimation = false;
								m_runStateInfo.timeSinceLastSpawnTimer = 0f;
								m_runStateInfo.timeToNextSpawn = PPMath.RandomFloat(m_minDurationBetweenSpawns, m_maxDurationBetweenSpawns);

								m_runStateInfo.lastIsMelee = false;
							}
							else
							{
								m_runStateInfo.lastIsMelee = true;

								int stateSelection;

								do
								{
									stateSelection = PPMath.RandomInt(0, 2);
								}
								while (m_runStateInfo.lastRNGState == stateSelection);

								switch (stateSelection)
								{
									case 0:
										TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK);
										break;
									case 1:
										TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.CLAP_ATTACK);
										break;
									case 2:
										TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.SMASH_ATTACK);
										break;
									default:
										Console.WriteLine("L1BossBehaviour.cs: RNG out of range");
										break;
								}

								m_runStateInfo.lastRNGState = stateSelection;
							}
						}
					}

					UpdateShootingBehaviour(dt);
				}
				break;
				//case L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION:
				//{

				//}
				//break;
				case L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE:
					break;
				case L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS:
				{
					if (m_runStateInfo.timer > mSpawner_timeStartSpawnEnemies && m_runStateInfo.timer < mSpawner_timeEndSpawnEnemies)
					{
						m_runStateInfo.secondaryTimer += dt;

						if (m_runStateInfo.secondaryTimer > mSpawner_durationBetweenSpawns)
						{
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

							m_runStateInfo.secondaryTimer = 0f;
						}

						if (!c_playedSpawnAnimation)
						{
							c_playedSpawnAnimation = true;

							//Console.WriteLine("SpawnAnimation");

							//Do enemy spawn animation
							for (int i = 0; i < 6; ++i)
							{
								rot = ECS.GetGlobalRotation(mID_ventSpawnpoints[i]);
								pos = ECS.GetGlobalPosition(mID_ventSpawnpoints[i]);
								GameObject instance = GameUtilities.InstantiateObject("Enemy_01", pos, rot);
								m_enemySpawnInstances.Add(instance);
							}
						}

						UpdateEnemySpawnAnimation(dt);
					}
				}
				break;

				case L1Boss.BOSS_BEHAVIOUR_STATE.SMASH_ATTACK:
				{
					if(m_runStateInfo.timer > mSmash_timeToEnableCollider && !m_hitboxEnableOnce && m_runStateInfo.timer < mSmash_timeToDisableCollider)
					{
						m_hitboxEnableOnce = true;
						ECS.SetActive(mID_smashHitBox, true);
					}
					
					if(m_runStateInfo.timer > mSmash_timeToDisableCollider && m_hitboxEnableOnce)
					{
						m_hitboxEnableOnce = false;
						ECS.SetActive(mID_smashHitBox, false);
					}
				}
				break;

				case L1Boss.BOSS_BEHAVIOUR_STATE.CLAP_ATTACK:
				{
					if (m_runStateInfo.timer > mClap_timeToEnableCollider && !m_hitboxEnableOnce && m_runStateInfo.timer < mClap_timeToDisableCollider)
					{
						m_hitboxEnableOnce = true;
						ECS.SetActive(mID_clapHitBox, true);
					}

					if (m_runStateInfo.timer > mClap_timeToDisableCollider && m_hitboxEnableOnce)
					{
						m_hitboxEnableOnce = false;
						ECS.SetActive(mID_clapHitBox, false);
					}
				}
				break;
			}

			UpdateGunRotationBehaviour(dt);

			//When Past the state duration threshold, it will return back to moving state
			if (m_runStateInfo.timer > m_runStateInfo.stateDuration)
			{
				if (m_stateBehaviours[(int)L1Boss.m_singleton.current_state].shouldReturnToDefault)
				{
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.MOVING, false, false);
					Console.WriteLine("Boss: Changing to MOVING");
				}
			}

			//Enter protection mode if damager threshold is reached
			if (m_runStateInfo.damageTakenPeriod > m_coreProtectionThreshold)
			{
				TriggerProtectionState();
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

			UpdateHPBar(dt);
		}

		/***************************************************************************/
		/*!
		\brief
			Updates shooting behaviour, if this is called,
			The boss will automatically shoot from the turrets,
			no prerequisites are needed at all.

		\param dt
			Delta time
		*/
		/***************************************************************************/
		void UpdateShootingBehaviour(float dt)
		{
			m_runStateInfo.bossShootTimer += dt;

			if (m_runStateInfo.bossShootTimer > m_bossShootingDelay)
			{
				//Shoot until hit limit
				if (m_runStateInfo.bossShootCount < m_bossShootCountLimit)
				{
					++m_runStateInfo.bossShootCount;
					m_runStateInfo.bossShootTimer = 0f;
					//Shoot bullets
					for (int i = 0; i < 6; ++i)
					{
						GameUtilities.FireEnemyBullet(m_gunBarrels[i].ID_shootPoint, ECS.GetGlobalPosition(m_gunBarrels[i].ID_shootPoint), Transform.GetForwardVector(m_gunBarrels[i].ID_shootPoint), 4.0f, 10.0f);
					}
				}
				//Once hit limit wait till cooldown ends
				else if (m_runStateInfo.bossShootTimer > m_bossShootingCoolDown)
				{
					m_runStateInfo.bossShootCount = 0;
					m_runStateInfo.bossShootTimer = 0f;
				}
			}
		}

		/***************************************************************************/
		/*!
		\brief
			Rotates the gun barrel randomly, will just randomly rotate the guns
			no prerequisites are needed at all.

		\param dt
			Delta time
		*/
		/***************************************************************************/
		void UpdateGunRotationBehaviour(float dt)
		{
			for(int i = 0; i < 6; ++i)
			{
				ECS.GetTransformECS(m_gunBarrels[i].ID_barrel, ref pos, ref rot, ref scale);

				if (m_gunBarrels[i].rotationState)
				{
					rot.Y += dt * m_gunBarrels[i].rotationSpeed;

					if (rot.Y > m_gunBarrelYRotationLimit)
					{
						m_gunBarrels[i].rotationState = false;
						m_gunBarrels[i].rotationSpeed = PPMath.RandomFloat(m_gunBarrelYRotationSpeedMin, m_gunBarrelYRotationSpeedMax);
					}
				}
				else
				{
					rot.Y -= dt * m_gunBarrels[i].rotationSpeed;

					if (rot.Y < -m_gunBarrelYRotationLimit)
					{
						m_gunBarrels[i].rotationState = true;
						m_gunBarrels[i].rotationSpeed = PPMath.RandomFloat(m_gunBarrelYRotationSpeedMin, m_gunBarrelYRotationSpeedMax);
					}
				}

				rot.X = m_gunBarrels[i].rotationXLock;

				ECS.SetRotation(m_gunBarrels[i].ID_barrel, rot);
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

					//Console.WriteLine("Erased animation");
				}
			}
			else
			{
				for (int i = 0; i < m_enemySpawnInstances.Count; ++i)
				{
					ECS.GetTransformECS(m_enemySpawnInstances[i].id, ref pos, ref rot, ref scale);

					pos.Y += c_hardCodedUpAnimationSpeed * dt;

					ECS.SetTransformECS(m_enemySpawnInstances[i].id, pos, rot, scale);
				}
			}
		}

		public override void LateUpdate(float dt)
		{

		}

		void TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE nextState, bool forceNonReturn = false, bool dontResetTimer = false)
		{
			m_runStateInfo.lastState = L1Boss.m_singleton.current_state;
			L1Boss.m_singleton.SetState(nextState.ToString());

			if (!dontResetTimer)
				m_runStateInfo.timer = 0f;

			m_runStateInfo.secondaryTimer = 0f;

			if (forceNonReturn)
			{
				m_runStateInfo.stateDuration = float.PositiveInfinity;
			}
			else
			{
				m_runStateInfo.stateDuration = PPMath.RandomFloat(
					m_stateBehaviours[(int)nextState].stateDurationMin,
					m_stateBehaviours[(int)nextState].stateDurationMax);

				Console.WriteLine("L1BossBehaviour: Duration " + m_runStateInfo.stateDuration);
				Console.WriteLine("L1BossBehaviour: Range " + m_stateBehaviours[(int)nextState].stateDurationMin + ", " + m_stateBehaviours[(int)nextState].stateDurationMax);
			}
		}

		public void DamageLeftCore(float damageAmount)
		{
			if (m_runStateInfo.canDamageSideCores && L1Boss.m_singleton.left_ball_protection)
			{
				ECS.PlayAudio(entityID, 0, "SFX");

				//Console.WriteLine("L1BossBehaviour.cs: Left damage taken");
				m_runStateInfo.leftBallHealth -= damageAmount;

				if (m_runStateInfo.leftBallHealth <= 0f)
				{
					m_runStateInfo.leftBallHealth = 0f;

					//Trigger left core death
					L1Boss.m_singleton.DamagedLeftBall();

					//remove left core from lock on
					FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Remove(mID_leftCore);
					FirstPersonFiringSystem.RemoveEnemyFromListOfTargets(mID_leftCore, 0);

					if (!L1Boss.m_singleton.right_ball_protection)
						DestroyedBothCores();

					Console.WriteLine("L1BossBehaviour.cs: Left core dead");
				}
			}
			else
			{
				ECS.PlayAudio(entityID, 1, "SFX");
			}
		}

		public void DamageRightCore(float damageAmount)
		{
			if (m_runStateInfo.canDamageSideCores && L1Boss.m_singleton.right_ball_protection)
			{
				ECS.PlayAudio(entityID, 0, "SFX");

				//Console.WriteLine("Right damage taken");
				m_runStateInfo.rightBallHealth -= damageAmount;

				if (m_runStateInfo.rightBallHealth <= 0f)
				{
					m_runStateInfo.rightBallHealth = 0f;

					//Trigger right core death
					L1Boss.m_singleton.DamagedRightBall();

					//remove right core from lock on
					FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Remove(mID_rightCore);
					FirstPersonFiringSystem.RemoveEnemyFromListOfTargets(mID_rightCore, 0);

					if (!L1Boss.m_singleton.left_ball_protection)
						DestroyedBothCores();

					Console.WriteLine("L1BossBehaviour.cs: Right core dead");
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
			L1Boss.m_singleton.EnableProtection();
			m_runStateInfo.canDamageMainCore = false;
			m_runStateInfo.canDamageSideCores = true;

			m_runStateInfo.damageTakenPeriod = 0f;

			//add left and right cores to list
			FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(mID_leftCore, true);
			FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(mID_rightCore, true);

			Console.WriteLine("L1BossBehaviour.cs: Boss hit damage threshold, entering protection");
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
			if (!m_stateBehaviours[(int)L1Boss.m_singleton.current_state].isVulnerable || !m_runStateInfo.canDamageMainCore)
			{
				ECS.PlayAudio(entityID, 1, "SFX");
				return;
			}

			//Console.WriteLine("Boss Taken damage");
			ECS.PlayAudio(entityID, 0, "SFX");

			mh_coreHealth -= 1f;
			m_runStateInfo.damageTakenPeriod += 1f;
			m_runStateInfo.timeSinceLastDamageTimer = 0f;

			if (mh_coreHealth <= 0f)
			{
				mh_coreHealth = 0f;

				if (L1Boss.m_singleton.current_state != L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE)
				{
					Console.WriteLine("L1BossBehaviour.cs: Boss is dead, triggering sequence");
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE);
				}
			}
		}

		public override void OnTriggerExit(uint id)
		{

		}
	}
}
