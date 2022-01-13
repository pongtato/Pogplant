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
        uint PlayerBox = 0;
        public uint turret_muzzle = 0;
        public float health = 20.0f;

        //Firing stuff
        float fireRate = 0.2f;
        float fire_timer = 0.0f;

        float can_fire_timer = 0.0f;
        float fire_window = 0.7f;
        float fire_cooldown = 1.5f;
        bool can_fire = true;



        bool isFiring = false;
        bool isAlive = true;
        float deathAnimationTime = 4.0f; // seconds
        uint DashboardScreenID;


        public BaseTurret()
        {
            // initialize private variables here

        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            turret_muzzle = ECS.FindChildEntityWithName(entityID, "Turret_Muzzle");
            PlayerBox = ECS.FindEntityWithName("PlayerBox");
            DashboardScreenID = ECS.FindEntityWithName("DashboardScreenFace");
        }

        public override void Start()
        {
        }

        public void Awake()
        {
        }

        public override void Update(float dt)
        {

            if (isAlive)
            {
                if (isFiring)
                {
                    can_fire_timer += dt;
                    if (can_fire)
                    {
                        fire_timer += dt;
                        if (fire_timer >= fireRate)
                        {
                            // Call C++ side bullet firing
                            GameUtilities.FireEnemyBullet(turret_muzzle, ECS.GetGlobalPosition(turret_muzzle) + Transform.GetForwardVector(turret_muzzle), ECS.GetGlobalRotation(turret_muzzle), 5.0f, 5.0f);
                            ECS.PlayAudio(entityID, 2, "SFX");
                            fire_timer = 0.0f;
                        }
                        if (can_fire_timer >= fire_window)
                        {
                            can_fire = false;
                            fire_timer = 0.0f;
                            can_fire_timer = 0.0f;
                        }
                    }
                    if (!can_fire)
                    {
                        if (can_fire_timer >= fire_cooldown)
                        {
                            can_fire = true;
                            can_fire_timer = 0.0f;
                        }
                    }
                }
            }
            else
            {
                deathAnimationTime -= dt;
                if (deathAnimationTime > 0.0f)
                {

                }
                else
                {
                    GameUtilities.SpawnStaticExplosion(ECS.GetGlobalPosition(entityID), 0);
                    ECS.DestroyEntity(entityID);
                }
            }
        }

        // Call this function to make this enemy start firing
        public void StartFiring(bool fire_state)
        {
            isFiring = fire_state;
        }

        public void TakeDamage(float damage)
        {
            if (health > 0)
            {
                health -= damage;
                ECS.PlayAudio(entityID, 0, "SFX");
                GameUtilities.SpawnStaticExplosion(ECS.GetGlobalPosition(entityID), 1);
            }
            else
            {
                GameUtilities.IncreaseScorefromEnv(PlayerBox);
                GameUtilities.UpdateDashboardFace(DashboardScreenID, 1);
                HandleDeath();
            }
        }

        void HandleDeath()
        {
            // This is a hardcoded way of destroying this instance, need to be replaced!
            if (isAlive)
            {
                isAlive = false;
                isFiring = false;
                ECS.PlayAudio(entityID, 1, "SFX");
                GameUtilities.PlayEnemyDeathAnimation(entityID);
                GameUtilities.SpawnStaticExplosion(ECS.GetGlobalPosition(entityID), 0);
            }
        }

        public override void LateUpdate(float dt)
        {
            //Console.WriteLine("MY CURRENT ID IS:" + entityID);
        }
        public bool GetAlive()
        {
            return isAlive;
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
