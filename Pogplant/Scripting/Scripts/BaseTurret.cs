/*****************************************************************************/
/*!
\file	BaseTurret.cs
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
    public class BaseTurret : MonoBehaviour
    {
        public Transform[] muzzle_transforms = new Transform[2];
        public float health = 20.0f;

        float fireRate;
        float fire_timer = 0.0f;
        float fire_duration = 0.0f;
        float current_fireDuration = 0.0f;

        // temp variables
        float fire_delay;
        float fire_delay_accumulator = 0.0f;

        bool isFiring = false;

        bool isAlive = true;
        float deathAnimationTime = 4.0f; // seconds
        uint disc_id;
        public BaseTurret()
        {
            // initialize private variables here

            fireRate = 1 / 2.0f;
            fire_duration = current_fireDuration = 1.0f;

            var rand = new Random();
            fire_delay = ((float)rand.Next() / int.MaxValue)  * 5.0f;
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            disc_id = ECS.FindChildEntityWithName(entityID, "Spinning_Disk");

            Console.WriteLine("Turret Enemy ID:" + entityID + " has spawned.");
        }

        public override void Start()
        {
        }

        public void Awake()
        {
        }

        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

            if ((fire_delay_accumulator += dt) > fire_delay)
                StartFiring();

            if (fire_delay_accumulator >= 10.0f)
                HandleDeath();

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
                        // hard coded muzzle position
                        Vector3 offset = new Vector3(0.0f, -1.076f, 0.454f);
                        GameUtilities.FireEnemyBullet(entityID, transform.Position + offset, transform.Rotation);
                    }
                    fire_timer = 0.0f;
                }

            }

            // spin disk
            Transform disk_transform = ECS.GetComponent<Transform>(disc_id);
            Vector3 disk_rotation = disk_transform.Rotation;
            disk_rotation.Y += 90.0f * dt;
            ECS.SetTransformECS(disc_id, ref disk_transform.Position, ref disk_rotation, ref disk_transform.Scale);

            if (!isAlive)
            {
                if (deathAnimationTime == 4.0f)
                {
                    var rand = new Random();
                    Vector3 dir = new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()) * 800.0f;
                    rigidbody.AddImpulseForce(dir);
                    rigidbody.useGravity = true;
                    rigidbody.mass = 5000.0f;
                }
                deathAnimationTime -= dt;
                if (deathAnimationTime > 0.0f)
                {
                    transform.Rotation.X += 180.0f * dt;
                    transform.Rotation.Y += 90.0f * dt;
                    transform.Rotation.Z += 270.0f * dt;
                }
                else
                {
                    Console.WriteLine("Turret (id: " + entityID + ") has died");
                    ECS.DestroyEntity(entityID);
                }
            }
        }

        // Call this function to make this enemy start firing
        public void StartFiring()
        {
            if (!isFiring)
                isFiring = true;
        }

        public void TakeDamage(float damage)
        {
            if (health > 0)
                health -= damage;
            if (health <= 0)
                HandleDeath();

            Console.WriteLine("Turret took damage, health is now: " + health + "Entity ID: " + entityID);
        }

        void HandleDeath()
        {

            // This is a hardcoded way of destroying this instance, need to be replaced!
            if (isAlive)
            {
                isAlive = false;
            }
        }

        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            //Console.WriteLine("MY CURRENT ID IS:" + entityID);
        }

        public void FixedUpdate()
        {
        }

        public override void OnTriggerEnter(uint id)
        {

        }
        public void OnTriggerExit()
        {
          
        }
        public override void OnTriggerExit(uint id)
        {

        }
    }
}
