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

namespace Scripting
{
	public class BossHitbox : MonoBehaviour
	{
		/**> The level of the boss so that it'll be calling the correct functions */
		public int m_bossLevel = 1;
		public int m_coreNumber = 0;

		public override void Init(ref uint _entityID)
		{
			entityID = _entityID;
		}

		public override void Start()
		{
			m_bossLevel = ECS.GetValue<int>(entityID, 1, "BossLevel");
			m_coreNumber = ECS.GetValue<int>(entityID, 0, "CoreNumber");
		}

		public override void Update(float dt)
		{

		}

		public override void LateUpdate(float dt)
		{

		}

		public override void OnTriggerEnter(uint id)
		{
			switch(m_bossLevel)
			{
				case 1:
				{
					switch(m_coreNumber)
					{
						case 0:
							L1BossBehaviour.m_singleton?.DamageLeftCore(1f);
							break;
						case 1:
							L1BossBehaviour.m_singleton?.DamageRightCore(1f);
							break;
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

		public override void OnTriggerExit(uint id)
		{

		}
	}
}
