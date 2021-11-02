/*****************************************************************************/
/*!
\author Wee Ding Wei
\par	email: w.dingwei\@digipen.edu
\details
	PlayerFire class for scripting c# project

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

namespace Scripting
{
    // PlayerFire script class
    public class PlayerFire: MonoBehaviour
    {

        float p_fireRate;
        float p_fire_timer = 0.0f;

        bool isFiring = false;

        int trueBulletInterval;
        int currentBulletInterval = 0;

        bool isInit = false;
        public PlayerFire()
        {
            // initialize private variables here

            p_fireRate = 1 / 3.0f;
            trueBulletInterval = -1;
        }

        public override void Start()
        {
        }

        public void Awake()
        {
        }

        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            p_fire_timer += dt;
            if ((InputUtility.onKeyTriggered(KEY_ID.KEY_T) || (InputUtility.onKeyHeld(KEY_ID.KEY_T))))
            {
                if (p_fire_timer >= p_fireRate)
                {
                    // Call C++ side bullet firing
                    GameUtilities.FirePlayerBullet(transform.Position, transform.Rotation);
                    p_fire_timer = 0.0f;
                }
            }
        }

        public void LateUpdate(ref Transform transform, ref Rigidbody rigidbody)
        {
        }

        public void FixedUpdate()
        {
        }

        public void OnTriggerEnter()
        {
            //StartFiring();
        }
        public void OnTriggerExit()
        {

        }
    }
}
