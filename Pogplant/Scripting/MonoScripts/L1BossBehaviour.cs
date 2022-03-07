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

		//--- Spawner variables

		//This is hardcoded second for spawner

		/**> How many seconds into the animation to choose to spawn enemies*/
		float mSpawner_timeStartSpawnEnemies = 4.2f;
		float mSpawner_timeEndSpawnEnemies = 7f;
		public float mSpawner_durationBetweenSpawns = 2f;

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
		private float c_hardCodedUpAnimationSpeed = 10f;
		private float c_animationDuration = 2f;

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
			mh_coreHealth = ECS.GetValue<float>(entityID, 100f, "CoreHealth");
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

			//Hardcode right and left 1 launcher to shoot around player angle
			m_gunBarrels[0].rotationXLock = 20f;
			m_gunBarrels[3].rotationXLock = 20f;

			m_runStateInfo.leftBallHealth = mh_leftBallHealth;
			m_runStateInfo.rightBallHealth = mh_rightBallHealth;

			m_runStateInfo.canDamageMainCore = true;
			m_runStateInfo.canDamageSideCores = false;
		}

		void InitStateBehaviours()
		{
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].isVulnerable = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].shouldReturnToDefault = true;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].stateDurationMin = 12.5f;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].stateDurationMax = 12.5f;

			m_runStateInfo.stateDuration = 9f;

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
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK].shouldReturnToDefault = false;
			//m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK].stateDurationMin = 30f;
			//m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK].stateDurationMin = 30f;

			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE].isVulnerable = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE].shouldReturnToDefault = false;
		}

		bool delayedInit = true;

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
				if (InputUtility.onKeyTriggered(KEY_ID.KEY_G))
				{
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.MOVING);
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_H))
				{
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS);
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_J))
				{
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK);
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_K))
				{
					//DestroyedBothCores();
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE);
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_0))
				{
					Console.WriteLine("L1BossBehaviour.cs: Boss state is: " + L1Boss.m_singleton.current_state);
					Console.WriteLine("L1BossBehaviour.cs: Timer is: " + m_runStateInfo.timer);
				}
				//return;
			}

			if(delayedInit)
			{
				//Add core to lock on list
				FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(entityID, true);
				delayedInit = false;
			}

			//Timers
			m_runStateInfo.timer += dt;
			m_runStateInfo.timeSinceLastDamageTimer += dt;
			m_runStateInfo.timeSinceLastSpawnTimer += dt;

			/*if (L1Boss.m_singleton.left_ball_protection || L1Boss.m_singleton.right_ball_protection)
			{
				UpdateShootingBehaviour(dt);

				if (m_runStateInfo.leftBallHealth <= 0 && m_runStateInfo.rightBallHealth <= 0)
				{
					//Reset health
					m_runStateInfo.leftBallHealth = mh_leftBallHealth;
					m_runStateInfo.rightBallHealth = mh_rightBallHealth;

					//Go back to moving state
					Console.WriteLine("L1BossBehaviour.cs: Boss returning to moving state");
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.MOVING, false, true);
				}
			}//*/

			switch (L1Boss.m_singleton.current_state)
			{
				case L1Boss.BOSS_BEHAVIOUR_STATE.MOVING:
				{
					if (m_runStateInfo.timer > m_runStateInfo.stateDuration)
					{
						if (m_runStateInfo.timeSinceLastSpawnTimer > m_runStateInfo.timeToNextSpawn)
						{
							TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS);

							c_playedSpawnAnimation = false;
							m_runStateInfo.timeSinceLastSpawnTimer = 0f;
							m_runStateInfo.timeToNextSpawn = PPMath.RandomFloat(m_minDurationBetweenSpawns, m_maxDurationBetweenSpawns);
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
								ECS.GetTransformECS(mID_ventSpawnpoints[i], ref pos, ref rot, ref scale);
								GameObject instance = GameUtilities.InstantiateObject("Enemy_01", pos, rot);
								m_enemySpawnInstances.Add(instance);
							}
						}

						UpdateEnemySpawnAnimation(dt);
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
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.MOVING, false, true);
					//Console.WriteLine("Boss: Changing to MOVING");
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
			if(nextState == L1Boss.BOSS_BEHAVIOUR_STATE.MOVING)
			{
				//Add boss core to lock on list
				//FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(entityID, true);
			}

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
			}
		}

		public void DamageLeftCore(float damageAmount)
		{
			if (m_runStateInfo.canDamageSideCores)
			{
				//Console.WriteLine("L1BossBehaviour.cs: Left damage taken");
				m_runStateInfo.leftBallHealth -= damageAmount;

				if (m_runStateInfo.leftBallHealth <= 0f && L1Boss.m_singleton.left_ball_protection)
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
		}

		public void DamageRightCore(float damageAmount)
		{
			if (m_runStateInfo.canDamageSideCores)
			{
				//Console.WriteLine("Right damage taken");
				m_runStateInfo.rightBallHealth -= damageAmount;

				if (m_runStateInfo.rightBallHealth <= 0f && L1Boss.m_singleton.right_ball_protection)
				{
					m_runStateInfo.rightBallHealth = 0f;

					//Trigger right core death
					L1Boss.m_singleton.DamagedRightBall();

					//remove right core from lock on
					FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Remove(mID_rightCore);
					FirstPersonFiringSystem.RemoveEnemyFromListOfTargets(mID_rightCore, 0);

					if (!L1Boss.m_singleton.left_ball_protection)
						DestroyedBothCores();

					Console.WriteLine("L1BossBehaviour.cs: L1BossBehaviour.cs: Right core dead");
				}
			}
		}

		public void DestroyedBothCores()
		{
			m_runStateInfo.canDamageMainCore = true;
			m_runStateInfo.canDamageSideCores = false;
			
			//reset health
			m_runStateInfo.leftBallHealth = mh_leftBallHealth;
			m_runStateInfo.rightBallHealth = mh_rightBallHealth;

			//Add core back to lock on list
			FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(entityID, true);
		}

		public void TriggerProtectionState()
		{
			L1Boss.m_singleton.EnableProtection();
			m_runStateInfo.canDamageMainCore = false;
			m_runStateInfo.canDamageSideCores = true;

			m_runStateInfo.damageTakenPeriod = 0f;

			//Remove core from lock on list
			FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Remove(entityID);
			FirstPersonFiringSystem.RemoveEnemyFromListOfTargets(entityID, 0);

			//add left and right cores to list
			FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(mID_leftCore, true);
			FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(mID_rightCore, true);

			Console.WriteLine("L1BossBehaviour.cs: Boss hit damage threshold, entering protection");
		}

		public override void OnTriggerEnter(uint id)
		{
			//Invulnerable in protection mode
			if (!m_stateBehaviours[(int)L1Boss.m_singleton.current_state].isVulnerable || !m_runStateInfo.canDamageMainCore)
			{
				return;
			}

			//Console.WriteLine("Boss Taken damage");

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
