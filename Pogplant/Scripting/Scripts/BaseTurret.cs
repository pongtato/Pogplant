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
        public Transform[] muzzle_transforms = new Transform[1];
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

        //Temporary sine wave movement to smoke
        Vector3 m_initialPos;
        Vector3 m_sineOffset;
        float m_sineTimer;
        Vector3 m_sineScale;
        Vector3 m_sineSpeed;

        public BaseTurret()
        {
            // initialize private variables here

            fireRate = 1 / 2.0f;
            fire_duration = current_fireDuration = 1.0f;

            var rand = new Random();
            fire_delay = ((float)rand.Next() / int.MaxValue) * 5.0f;
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            disc_id = ECS.FindChildEntityWithName(entityID, "Spinning_Disk");

            //Console.WriteLine("Turret Enemy ID:" + entityID + " has spawned.");

            //Temporary sine wave movement
            var rand = new Random();
            m_initialPos = ECS.GetComponent<Transform>(entityID).Position;
            m_sineOffset.X = (float)rand.NextDouble();
            m_sineOffset.Y = (float)rand.NextDouble();
            m_sineOffset.Z = (float)rand.NextDouble();


            m_sineTimer = 0f;
            m_sineScale.X = (float)rand.NextDouble() * 2.0f + 1.0f;
            m_sineScale.Y = (float)rand.NextDouble() * 3.0f + 1.0f;
            m_sineScale.Z = (float)rand.NextDouble() * 5.0f + 1.0f;
            m_sineSpeed.X = (float)rand.NextDouble() * 1.0f + 0.5f;
            m_sineSpeed.Y = (float)rand.NextDouble() * 1.5f + 0.5f;
            m_sineSpeed.Z = (float)rand.NextDouble() * 1.5f + 0.5f;
        }

        public override void Start()
        {
        }

        public void Awake()
        {
        }

        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

            if (isAlive)
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
                            
                            GameUtilities.FireEnemyBullet(entityID, ECS.GetGlobalPosition(entityID) + Transform.GetForwardVector(entityID) * 5f, transform.Rotation);
                        }
                        fire_timer = 0.0f;
                    }

                }

                // spin disk
                Transform disk_transform = ECS.GetComponent<Transform>(disc_id);
                Vector3 disk_rotation = disk_transform.Rotation;
                disk_rotation.Y += 90.0f * dt;

                if (disk_rotation.Y > 360f)
                    disk_rotation.Y -= 360f;

                ECS.SetTransformECS(disc_id, ref disk_transform.Position, ref disk_rotation, ref disk_transform.Scale);

                sineMovement(ref transform, ref dt);

            }
            else
            {
                if (deathAnimationTime == 4.0f)
                {
                    var rand = new Random();
                    Vector3 dir = new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()) * 1000.0f;
                    rigidbody.AddImpulseForce(dir);
                    rigidbody.useGravity = true;
                    rigidbody.mass = 100.0f;
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
                    //Console.WriteLine("Turret (id: " + entityID + ") has died");
                    ECS.DestroyEntity(entityID);
                }
            }

        }

        public void sineMovement(ref Transform transform, ref float dt)
        {
            m_sineTimer += dt;

            transform.Position.X = (float)Math.Sin(m_sineOffset.X + m_sineTimer * m_sineSpeed.X) * m_sineScale.X + m_initialPos.Y;
            transform.Position.Y = (float)Math.Sin(m_sineOffset.Y + m_sineTimer * m_sineSpeed.Y) * m_sineScale.Y + m_initialPos.Y;
            transform.Position.Z = (float)Math.Sin(m_sineOffset.Z + m_sineTimer * m_sineSpeed.Z) * m_sineScale.Z + m_initialPos.Z;
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

            //Console.WriteLine("Turret took damage, health is now: " + health + "Entity ID: " + entityID);
        }

        void HandleDeath()
        {

            // This is a hardcoded way of destroying this instance, need to be replaced!
            if (isAlive)
            {
                isAlive = false;
                isFiring = false;
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
