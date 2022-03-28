/******************************************************************************/
/*!
\file	BossHitbox.cs
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details

	This file contains the code to handle the level 1's boss hitbox collider

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
	public class BossHitbox : MonoBehaviour
	{
		/**> The level of the boss so that it'll be calling the correct functions */
		public int m_bossLevel = 1;
		public int m_coreNumber = 0;
		public uint mID_playerShip;
		public int m_damageAmount = 50;
		public bool m_damageHitBox = false;
		public bool m_applySucc = false;
		public bool m_applyForceOnHit = false;
		public Vector3 m_forceDirection;
		float m_succForce;
		float m_initialSuccForce;

		public Vector3 m_cameraShakeInitMul;
		public Vector3 m_cameraShakeMag;
		public float m_cameraShakeDuration;

		public float m_vibrationForce;
		public float m_vibrationDuration;

		List<uint> m_succList = new List<uint>();

		public override void Init(ref uint _entityID)
		{
			entityID = _entityID;
		}

		public override void Start()
		{
			m_bossLevel = ECS.GetValue<int>(entityID, 1, "BossLevel");
			m_coreNumber = ECS.GetValue<int>(entityID, 0, "CoreNumber");
			m_damageAmount = ECS.GetValue<int>(entityID, 50, "DamageAmount");
			m_damageHitBox = ECS.GetValue<bool>(entityID, false, "IsHitbox");
			m_applyForceOnHit = ECS.GetValue<bool>(entityID, false, "ApplyForce");
			m_forceDirection = ECS.GetValue<Vector3>(entityID, new Vector3(0.0f, -100f, 0.0f), "ForceDirection");

			m_applySucc = ECS.GetValue<bool>(entityID, false, "ApplySucc");
			m_succForce = ECS.GetValue<float>(entityID, 10f, "SuccForce");
			m_initialSuccForce = ECS.GetValue<float>(entityID, 20f, "SuccForceInit");

			m_vibrationDuration = ECS.GetValue<float>(entityID, 0.2f, "VibrationForce");
			m_vibrationForce = ECS.GetValue<float>(entityID, 1.0f, "VibrationDuration");

			m_cameraShakeInitMul = ECS.GetValue<Vector3>(entityID, new Vector3(0.6f, 0.6f, 0.4f), "CameraShakeInit");
			m_cameraShakeMag = ECS.GetValue<Vector3>(entityID, new Vector3(10f, 10f, 10f), "CameraShakeMag");
			m_cameraShakeDuration = ECS.GetValue<float>(entityID, 1f, "CameraShakeDuration");

			mID_playerShip = ECS.FindEntityWithName("PlayerShip");
		}

		public override void Update(float dt)
		{

		}

		public override void LateUpdate(float dt)
		{
			for(int i = 0; i < m_succList.Count(); ++i)
			{
				if(ECS.CheckValidEntity(m_succList[i]))
				{
					ECS.RigidbodyAddForce(m_succList[i], (ECS.GetGlobalPosition(entityID) - ECS.GetGlobalPosition(m_succList[i])) * m_succForce);
					//ECS.RigidbodyAddForce(m_succList[i], 0);
				}
				else
				{
					m_succList.RemoveAt(i);
					--i;
				}
			}
		}

		public override void OnTriggerEnter(uint id)
		{
			if (m_damageHitBox)
			{
				if (id == mID_playerShip)
				{
					PlayerScript.m_singleton.TriggerCameraShake(
						new Vector3(m_cameraShakeInitMul.X, m_cameraShakeInitMul.Y, m_cameraShakeInitMul.Z),
						new Vector3(m_cameraShakeMag.X, m_cameraShakeMag.Y, m_cameraShakeMag.Z),
						m_cameraShakeDuration);
					PlayerScript.AddScore(false, false, (uint)m_damageAmount);

					//Add controller vibration
					InputUtility.VibrateControllerHeavyMotor(m_vibrationForce, m_vibrationDuration);
				}

				if (m_applyForceOnHit)
				{
					ECS.RigidbodyAddImpulseForce(id, m_forceDirection);
				}

				if (m_applySucc)
				{
					m_succList.Add(id);

					ECS.RigidbodyAddImpulseForce(id, (ECS.GetGlobalPosition(entityID) - ECS.GetGlobalPosition(id)) * m_initialSuccForce);
				}
			}
			else
			{
				switch (m_bossLevel)
				{
					case 1:
					{
						switch (m_coreNumber)
						{
							case 0:
								L1BossBehaviour.m_singleton?.DamageLeftCore(1f);
								break;
							case 1:
								L1BossBehaviour.m_singleton?.DamageRightCore(1f);
								break;
							default:
								Console.WriteLine("BossHitbox.cs: Lvl1 Unknown core number");
								break;
						}
					}
					break;

					case 2:
					{
						switch (m_coreNumber)
						{
							case 0:
								L2BossBehaviour.m_singleton?.DamageLeftCore(1f);
								break;
							case 1:
								L2BossBehaviour.m_singleton?.DamageRightCore(1f);
								break;
							default:
								Console.WriteLine("BossHitbox.cs: Lvl2 Unknown core number");
								break;
						}
					}
					break;

					default:
						Console.WriteLine("BossHitbox.cs: Unintended/not implemented boss level");
						break;
				}
			}
		}

		public override void OnTriggerExit(uint id)
		{
			if(m_damageHitBox && m_applySucc)
			{
				m_succList.Remove(id);
			}
		}
	}
}
