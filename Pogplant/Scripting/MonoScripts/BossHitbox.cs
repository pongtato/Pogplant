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

		public Vector3 m_cameraShakeInitMul;
		public Vector3 m_cameraShakeMag;
		public float m_cameraShakeDuration;

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

			m_cameraShakeInitMul = ECS.GetValue<Vector3>(entityID, new Vector3(0.5f, 0.5f, 0.5f), "CameraShakeInit");
			m_cameraShakeMag = ECS.GetValue<Vector3>(entityID, new Vector3(8f, 8f, 8f), "CameraShakeMag");
			m_cameraShakeDuration = ECS.GetValue<float>(entityID, 1f, "CameraShakeDuration");

			mID_playerShip = ECS.FindEntityWithName("PlayerShip");
		}

		public override void Update(float dt)
		{

		}

		public override void LateUpdate(float dt)
		{

		}

		public override void OnTriggerEnter(uint id)
		{
			PPMath.RandomFloat(-5f, 5f);
			if (m_damageHitBox && id == mID_playerShip)
			{
				PlayerScript.m_singleton.TriggerCameraShake(
					new Vector3(PPMath.RandomFloat(-m_cameraShakeInitMul.X, m_cameraShakeInitMul.X), PPMath.RandomFloat(-m_cameraShakeInitMul.Y, m_cameraShakeInitMul.Y), PPMath.RandomFloat(-m_cameraShakeInitMul.Z, m_cameraShakeInitMul.Z)),
					new Vector3(PPMath.RandomFloat(-m_cameraShakeMag.X, m_cameraShakeMag.X), PPMath.RandomFloat(-m_cameraShakeMag.Y, m_cameraShakeMag.Y), PPMath.RandomFloat(-m_cameraShakeMag.Z, m_cameraShakeMag.Z)),
					m_cameraShakeDuration);
				PlayerScript.AddScore(false, false, (uint)m_damageAmount);
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
							case 2:
							{

								break;
							}
							default:
								Console.WriteLine("BossHitbox.cs: Unknown core number");
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

		}
	}
}
