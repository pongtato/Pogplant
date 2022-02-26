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
		public float m_coreHealth;

		/**> How much damage the boss will take before going into protection mode*/
		public float m_coreProtectionThreshold;

		/**> How much time before the damage threshold gets reset*/
		public float m_coreProtectionDamageDuration;

		public float m_minMovingTime;
		public float m_maxMovingTime;

		public float m_minDurationBetweenSpawns;
		public float m_maxDurationBetweenSpawns;


		public uint[] mID_ventSpawnpoints = new uint[6];

		//--- Spawner variables

		//This is hardcoded second for spawner

		/**> How many seconds into the animation to choose to spawn enemies*/
		float mSpawner_timeStartSpawnEnemies = 4.2f;
		float mSpawner_timeEndSpawnEnemies = 7f;
		public float mSpawner_durationBetweenSpawns = 100f;

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
			m_coreHealth = ECS.GetValue<float>(entityID, 100f, "CoreHealth");
			m_coreProtectionThreshold = ECS.GetValue<float>(entityID, 1f, "CoreProtectionThreshold");
			m_coreProtectionDamageDuration = ECS.GetValue<float>(entityID, 1f, "CoreProtectionDamageDuration");
			mSpawner_durationBetweenSpawns = ECS.GetValue<float>(entityID, 0.5f, "DurationBetweenEnemySpawns");

			m_minMovingTime = ECS.GetValue<float>(entityID, 5f, "IdleTimeMin");
			m_maxMovingTime = ECS.GetValue<float>(entityID, 10f, "IdleTimeMax");

			m_minDurationBetweenSpawns = ECS.GetValue<float>(entityID, 10f, "DurationBetweenSpawnMin");
			m_maxDurationBetweenSpawns = ECS.GetValue<float>(entityID, 15f, "DurationBetweenSpawnMax");


			InitStateBehaviours();

			uint bossPanelSpawns = ECS.FindEntityWithName("BossPanelSpawnPoints");

			for (int i = 0; i < 6; ++i)
			{
				mID_ventSpawnpoints[i] = ECS.FindChildEntityWithName(bossPanelSpawns, (i + 1).ToString());
			}
		}

		void InitStateBehaviours()
		{
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].isVulnerable = true;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].shouldReturnToDefault = false;

			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.MOVING].isVulnerable = true;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.MOVING].shouldReturnToDefault = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.MOVING].stateDurationMin = m_minMovingTime;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.MOVING].stateDurationMax = m_maxMovingTime;


			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION].isVulnerable = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION].shouldReturnToDefault = true;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION].stateDurationMin = 5f;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION].stateDurationMax = 10f;

			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].isVulnerable = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].shouldReturnToDefault = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].stateDurationMin = 20f;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].stateDurationMax = 20f;

			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK].isVulnerable = true;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK].shouldReturnToDefault = false;
			//m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK].stateDurationMin = 30f;
			//m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK].stateDurationMin = 30f;

			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE].isVulnerable = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE].shouldReturnToDefault = false;
		}

		/******************************************************************************/
		/*!
		\brief
			Update
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
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE);
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_0))
				{
					Console.WriteLine("Boss state is: " + L1Boss.m_singleton.current_state);
					Console.WriteLine("Timer is: " + m_runStateInfo.timer);
				}
				//return;
			}

			//Timers
			m_runStateInfo.timer += dt;
			m_runStateInfo.timeSinceLastDamageTimer += dt;
			m_runStateInfo.timeSinceLastSpawnTimer += dt;

			switch (L1Boss.m_singleton.current_state)
			{
				case L1Boss.BOSS_BEHAVIOUR_STATE.MOVING:

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

					break;
				case L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION:
					break;
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

							Console.WriteLine("SPAWNING ENEMIES Randomed: " + randomInt);

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
				TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION);
				m_runStateInfo.damageTakenPeriod = 0f;
				Console.WriteLine("Boss hit damage threshold, entering protection");
			}

			//Damage threshold reset if boss doesn't take damage for awhile
			if (m_runStateInfo.timeSinceLastDamageTimer > m_coreProtectionDamageDuration)
			{
				m_runStateInfo.damageTakenPeriod = 0f;
			}
		}

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
			}
		}

		public override void OnTriggerEnter(uint id)
		{
			//Invulnerable in protection mode
			if (!m_stateBehaviours[(int)L1Boss.m_singleton.current_state].isVulnerable)
			{
				return;
			}

			//Console.WriteLine("Boss Taken damage");

			m_coreHealth -= 1f;
			m_runStateInfo.damageTakenPeriod += 1f;
			m_runStateInfo.timeSinceLastDamageTimer = 0f;

			if (m_coreHealth <= 0f)
			{
				m_coreHealth = 0f;

				if (L1Boss.m_singleton.current_state != L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE)
				{
					Console.WriteLine("Boss is dead, triggering sequence");
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE);
				}
			}
		}

		public override void OnTriggerExit(uint id)
		{

		}
	}
}
