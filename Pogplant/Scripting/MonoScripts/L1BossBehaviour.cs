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
				m_timer = 0f;
				m_secondaryTimer = 0f;
				m_stateDuration = 1f;
				m_timeSinceLastDamage = 0f;
				m_damageTakenPeriod = 0f;
			}

			public float m_timer;
			public float m_secondaryTimer;
			public float m_stateDuration;
			public float m_timeSinceLastDamage;

			/**> Damage taken in this period of time*/
			public float m_damageTakenPeriod;
		}
		#endregion

		RuntimeStateVariables m_runStateInfo;

		BossStateBehaviour[] m_stateBehaviours = new BossStateBehaviour[(int)L1Boss.BOSS_BEHAVIOUR_STATE.TOTAL];

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

			InitStateBehaviours();

			uint bossPanelSpawns = ECS.FindEntityWithName("BossPanelSpawnPoints");

			for (int i = 0; i < 6; ++i)
			{
				mID_ventSpawnpoints[i] = ECS.FindChildEntityWithName(bossPanelSpawns, (i + 1).ToString());
			}
		}

		void InitStateBehaviours()
		{
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.MOVING].isVulnerable = true;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.MOVING].shouldReturnToDefault = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.MOVING].stateDurationMin = 10f;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.MOVING].stateDurationMax = 10f;


			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION].isVulnerable = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION].shouldReturnToDefault = true;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION].stateDurationMin = 5f;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION].stateDurationMax = 10f;

			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].isVulnerable = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].shouldReturnToDefault = false;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].stateDurationMin = 20f;
			m_stateBehaviours[(int)L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS].stateDurationMax = 20f;

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
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION);
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_K))
				{
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE);
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_0))
				{
					Console.WriteLine("Boss state is: " + L1Boss.m_singleton.current_state);
					Console.WriteLine("Timer is: " + m_runStateInfo.m_timer);
				}
				//return;
			}

			//Timers
			m_runStateInfo.m_timer += dt;
			m_runStateInfo.m_timeSinceLastDamage += dt;

			switch (L1Boss.m_singleton.current_state)
			{
				case L1Boss.BOSS_BEHAVIOUR_STATE.MOVING:
					break;
				case L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION:
					break;
				case L1Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE:
					break;
				case L1Boss.BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS:
				{
					if(m_runStateInfo.m_timer > mSpawner_timeStartSpawnEnemies && m_runStateInfo.m_timer < mSpawner_timeEndSpawnEnemies)
					{
						m_runStateInfo.m_secondaryTimer += dt;
						
						if(m_runStateInfo.m_secondaryTimer > mSpawner_durationBetweenSpawns)
						{
							Console.WriteLine("SPAWNING ENEMIES");

							/*for (int i = 0; i < 6; ++i)
							{
								ECS.GetGlobalPosition(mID_ventSpawnpoints[i]);
							}//*/

							//EncounterSystemDriver.m_singleton.SpawnWave("Boss_Low1");
							//EncounterSystemDriver.m_singleton.SpawnWave("Boss_Low2");
							//EncounterSystemDriver.m_singleton.SpawnWave("Boss_Med1");
							//EncounterSystemDriver.m_singleton.SpawnWave("Boss_Med2");
							EncounterSystemDriver.m_singleton.SpawnWave("Boss_High1");
							//EncounterSystemDriver.m_singleton.SpawnWave("Boss_High2");

							m_runStateInfo.m_secondaryTimer = 0f;
						}
					}
				}
				break;
			}

			//Enter protection mode if damager threshold is reached
			if (m_runStateInfo.m_damageTakenPeriod > m_coreProtectionThreshold)
			{
				TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.PROTECTION);
				m_runStateInfo.m_damageTakenPeriod = 0f;
				Console.WriteLine("Boss hit damage threshold, entering protection");
			}

			//When Past the state duration threshold, it will return back to moving state
			if (m_runStateInfo.m_timer > m_runStateInfo.m_stateDuration)
			{
				if (m_stateBehaviours[(int)L1Boss.m_singleton.current_state].shouldReturnToDefault)
				{
					TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE.MOVING);
					Console.WriteLine("Changing to MOVING");
				}
			}

			//Damage threshold reset if boss doesn't take damage for awhile
			if (m_runStateInfo.m_timeSinceLastDamage > m_coreProtectionDamageDuration)
			{
				m_runStateInfo.m_damageTakenPeriod = 0f;
			}
		}
		public override void LateUpdate(float dt)
		{

		}

		void TriggerNextState(L1Boss.BOSS_BEHAVIOUR_STATE nextState, bool forceNonReturn = false)
		{
			L1Boss.m_singleton.SetState(nextState);
			m_runStateInfo.m_timer = 0f;
			m_runStateInfo.m_secondaryTimer = 0f;

			if (forceNonReturn)
			{
				m_runStateInfo.m_stateDuration = float.PositiveInfinity;
			}
			else
			{
				m_runStateInfo.m_stateDuration = PPMath.RandomFloat(
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

			Console.WriteLine("Boss Taken damage");

			m_coreHealth -= 1f;
			m_runStateInfo.m_damageTakenPeriod += 1f;
			m_runStateInfo.m_timeSinceLastDamage = 0f;

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
