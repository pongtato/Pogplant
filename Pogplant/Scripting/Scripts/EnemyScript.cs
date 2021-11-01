/*****************************************************************************/
/*!
\file	EnemyScript.cs
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\author Ng Tian Kiat
\par	email: tiankiat.ng\@digipen.edu
\details
	EnemyScript class for scripting c# project

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
    // Enemy script class
    public class EnemyScript : MonoBehaviour
    {
        public Transform[] muzzle_transforms = new Transform[4];
        public float health = 20;

        float fireRate;
        float fire_timer = 0.0f;
        float fire_duration = 0.0f;
        float current_fireDuration = 0.0f;

        bool isFiring = false;

        int trueBulletInterval;
        int currentBulletInterval = 0;

        bool isInit = false;
        public EnemyScript()
        {
            // initialize private variables here

            fireRate = 1 / 5.0f;
            trueBulletInterval = -1;
            fire_duration = current_fireDuration = 1.0f;
        }

        public override void Start()
        {
        }

        public void Awake()
        {
        }

        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            //Start(ref transform);

            if (InputUtility.onKeyTriggered(KEY_ID.KEY_G))
                StartFiring();

            if (isFiring)
            {
                current_fireDuration -= dt;
                if (current_fireDuration <= 0.0f)
                {
                    isFiring = false;
                    current_fireDuration = fire_duration;
                }

                fire_timer += dt;

                if (fire_timer >= fireRate)
                {
                    for (int i = 0; i < muzzle_transforms.Length; ++i)
                    {
                        // Call C++ side bullet firing
                        GameUtilities.FireEnemyBullet(transform.Position, transform.Rotation);
                    }
                    fire_timer = 0.0f;
                }

            }
        }

        // Call this function to make this enemy start firing
        public void StartFiring()
        {
            if (!isFiring)
                isFiring = true;
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
    }
}
