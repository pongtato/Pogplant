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

        bool isFiring = false;

        int trueBulletInterval;
        int currentBulletInterval = 0;

        public EnemyScript()
        {
            // initialize private variables here

            fireRate = 1 / 3.0f;
            trueBulletInterval = -1;
            fire_duration = 3.0f;
        }

        public override void Start()
        {

        }

        public void Awake()
        {
        }

        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            //if (transform.Position.X >= 10.0f)
            //{
            //    rigidbody.AddForce(new Vector3(-3.0f, 0, 0));
            //}

            //if (transform.Position.X < 10.0f)
            //{
            //    rigidbody.AddForce(new Vector3(3.0f, 0, 0));
            //}

            if (isFiring)
            {
                fire_duration -= dt;
                if (fire_duration <= 0.0f)
                    isFiring = false;

                fire_timer += dt;

                if (fire_timer >= fireRate)
                {
                    fire_timer = 0.0f;
                }

                for (int i = 0; i < muzzle_transforms.Length; ++i)
                {
                    // Call C++ side bullet firing
                    // FireEnemyBullet(muzzle_transforms[i])
                }
            }
        }

        // Call this function to make this enemy start firing
        public void StartFiring()
        {
            isFiring = true;
        }

        //public void LateUpdate(ref Transform transform, ref Rigidbody rigidbody)
        //{
        //}

        //public void FixedUpdate()
        //{
        //}
    }
}
