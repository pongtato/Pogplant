/******************************************************************************/
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

		public float mh_leftBallHealth;
		public float mh_rightBallHealth;

		/**> How much damage the boss will take before going into protection mode*/
		public float m_coreProtectionThreshold;

		/**> How much time before the damage threshold gets reset*/
		public float m_coreProtectionDamageDuration;

		//Shooting stuff
		public float m_bossShootingDelay = 0.7f;
		public float m_bossShootingCoolDown = 5f;

		//IDs
		public uint mID_leftCore;
		public uint mID_rightCore;
		public uint mID_playerShip;

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

			public Vector3 restingRotation;
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
				bossTurretReady = false;
				leftBallHealth = 0f;
				rightBallHealth = 0f;
				canDamageSideCores = false;
				canDamageMainCore = false;
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
			public bool bossTurretReady;

			public float leftBallHealth;
			public float rightBallHealth;

			public bool canDamageSideCores;
			public bool canDamageMainCore;
		}
		#endregion

		RuntimeStateVariables m_runStateInfo;
		BossStateBehaviour[] m_stateBehaviours = new BossStateBehaviour[(int)L2Boss.BOSS_BEHAVIOUR_STATE.TOTAL];
		TurretGun[] m_turretGuns = new TurretGun[2];

		Transform m_tempTransform;

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
			mh_coreHealth = ECS.GetValue<float>(entityID, 200f, "CoreHealth");
			mh_leftBallHealth = ECS.GetValue<float>(entityID, 70f, "LeftBallHealth");
			mh_rightBallHealth = ECS.GetValue<float>(entityID, 70f, "RightBallHealth");

			m_coreProtectionThreshold = ECS.GetValue<float>(entityID, 60f, "CoreProtectionThreshold");
			m_coreProtectionDamageDuration = ECS.GetValue<float>(entityID, 5f, "CoreProtectionDamageDuration");

			mID_playerShip = ECS.FindEntityWithName("PlayerShip");

			mID_leftCore = ECS.FindEntityWithName("Left_Eye");
			mID_rightCore = ECS.FindEntityWithName("Right_Eye");

			m_turretGuns[0].ID_turretBody = ECS.FindEntityWithName("Left_ColourTurret_Body_Pivot");
			m_turretGuns[1].ID_turretBody = ECS.FindEntityWithName("Right_ColourTurret_Body_Pivot");

			for (int i = 0; i < 2; ++i)
			{
				m_turretGuns[i].ID_shootPoint = ECS.FindChildEntityWithName(m_turretGuns[i].ID_turretBody, "Shoot");

				ECS.GetTransformECS(m_turretGuns[i].ID_turretBody, ref m_tempTransform.Position, ref m_tempTransform.Rotation, ref m_tempTransform.Scale);
				m_turretGuns[i].restingRotation = m_tempTransform.Rotation;
			}

			m_runStateInfo.canDamageSideCores = false;
			m_runStateInfo.canDamageMainCore = true;

			InitStateBehaviours();
		}

		void InitStateBehaviours()
		{
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.MOVING].isVulnerable = true;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.MOVING].shouldReturnToDefault = false;

			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].isVulnerable = false;
			m_stateBehaviours[(int)L2Boss.BOSS_BEHAVIOUR_STATE.FLYING_UP].shouldReturnToDefault = true;

			m_runStateInfo.stateDuration = 9.5f;
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
			m_runStateInfo.bossTurretReady = InputUtility.onKeyHeld(KEY_ID.KEY_9);
			if (InputUtility.onKeyTriggered(KEY_ID.KEY_0))
			{
				Console.WriteLine("L2BossBehaviour.cs: Boss state is: " + L2Boss.m_singleton.current_state);
				Console.WriteLine("L2BossBehaviour.cs: Timer is: " + m_runStateInfo.timer);
			}

			if (delayedInit)
			{
				//Add core to lock on list
				FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(entityID, true);
				delayedInit = false;
			}

			UpdateGunBehaviour(dt);

			switch (L2Boss.m_singleton.current_state)
			{
				case L2Boss.BOSS_BEHAVIOUR_STATE.MOVING:
				{

				}
				break;
				case L2Boss.BOSS_BEHAVIOUR_STATE.REPEL_ATTACK:
				{

				}
				break;
			}

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
					TriggerNextState(L2Boss.BOSS_BEHAVIOUR_STATE.MOVING, false, true);
				}
			}

			//Damage threshold reset if boss doesn't take damage for awhile
			if (m_runStateInfo.timeSinceLastDamageTimer > m_coreProtectionDamageDuration)
			{
				m_runStateInfo.damageTakenPeriod = 0f;
			}
		}

		public override void LateUpdate(float dt)
		{

		}

		void UpdateGunBehaviour(float dt)
		{
			Vector3 shipPos = ECS.GetGlobalPosition(mID_playerShip);

			//Lerp gun target
			for (int i = 0; i < 2; ++i)
			{
				ECS.GetTransformECS(m_turretGuns[i].ID_turretBody, ref m_tempTransform.Position, ref m_tempTransform.Rotation, ref m_tempTransform.Scale);
				Vector3 beforeLerp = m_tempTransform.Rotation;
				Transform.LookAt(m_turretGuns[i].ID_turretBody, shipPos);
				ECS.GetTransformECS(m_turretGuns[i].ID_turretBody, ref m_tempTransform.Position, ref m_tempTransform.Rotation, ref m_tempTransform.Scale);

				m_tempTransform.Rotation = Vector3.Lerp(beforeLerp, m_tempTransform.Rotation, dt * 20f);

				ECS.SetRotation(m_turretGuns[i].ID_turretBody, m_tempTransform.Rotation);
			}


			//Fire
			if(m_runStateInfo.bossTurretReady)
			{
				m_runStateInfo.bossShootTimer += dt;

				if (m_runStateInfo.bossShootTimer > m_bossShootingDelay)
				{
					ECS.PlayAudio(entityID, 3, "SFX");
					m_runStateInfo.bossShootTimer = 0f;

					L2Boss.m_singleton.SetColorTurretFiring();

					for (int i = 0; i < 2; ++i)
					{
						Vector3 up = Transform.GetUpVector(m_turretGuns[i].ID_shootPoint);
						Vector3 shootPos = ECS.GetGlobalPosition(m_turretGuns[i].ID_shootPoint);
						Vector3 forwardVec = Transform.GetForwardVector(m_turretGuns[i].ID_shootPoint);
						Vector3 right = Vector3.CrossProduct(forwardVec, up);

						GameUtilities.FireEnemyBullet(
							m_turretGuns[i].ID_shootPoint,
							shootPos + up * 0.2f,
							forwardVec, 14.0f, 3.0f);

						GameUtilities.FireEnemyBullet(
							m_turretGuns[i].ID_shootPoint,
							shootPos - up * 0.2f,
							forwardVec, 14.0f, 3.0f);

						GameUtilities.FireEnemyBullet(
							m_turretGuns[i].ID_shootPoint,
							shootPos + right * 0.2f,
							forwardVec, 14.0f, 3.0f);

						GameUtilities.FireEnemyBullet(
							m_turretGuns[i].ID_shootPoint,
							shootPos - right * 0.2f,
							forwardVec, 14.0f, 3.0f);
					}
				}
			}
		}

		void TriggerNextState(L2Boss.BOSS_BEHAVIOUR_STATE nextState, bool forceNonReturn = false, bool dontResetTimer = false)
		{
			m_runStateInfo.lastState = L2Boss.m_singleton.current_state;
			L2Boss.m_singleton.SetState(nextState.ToString());

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

			//Add core back to lock on list
			FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(entityID, true);
		}

		public void TriggerProtectionState()
		{
			L2Boss.m_singleton.EnableProtection();
			m_runStateInfo.canDamageMainCore = false;
			m_runStateInfo.canDamageSideCores = true;

			m_runStateInfo.damageTakenPeriod = 0f;
			m_runStateInfo.leftBallHealth = mh_leftBallHealth;
			m_runStateInfo.rightBallHealth = mh_rightBallHealth;

			//Remove core from lock on list
			FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Remove(entityID);
			FirstPersonFiringSystem.RemoveEnemyFromListOfTargets(entityID, 0);

			//add left and right cores to list
			FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(mID_leftCore, true);
			FirstPersonFiringSystem.m_singleton.m_enemiesToRayCast.Add(mID_rightCore, true);

			Console.WriteLine("L2BossBehaviour.cs: Boss hit damage threshold, entering protection");
		}

		public override void OnTriggerEnter(uint id)
		{
			//Invulnerable in protection mode
			if (!m_stateBehaviours[(int)L2Boss.m_singleton.current_state].isVulnerable || !m_runStateInfo.canDamageMainCore)
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

				if (L2Boss.m_singleton.current_state != L2Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE)
				{
					Console.WriteLine("L2BossBehaviour.cs: Boss is dead, triggering sequence");
					TriggerNextState(L2Boss.BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE);
				}
			}
		}

		public override void OnTriggerExit(uint id)
		{

		}
	}
}
