﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    // Enemy script class
    public class BaseFlock : MonoBehaviour
    {
        public float health = 20.0f;

        float deathAnimationTime = 4.0f; // seconds
        bool isAlive = true;


        public BaseFlock()
        {
            // initialize private variables here

        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
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
            }
            else
            {
                deathAnimationTime -= dt;
                if (deathAnimationTime > 0.0f)
                {

                }
                else
                {
                    GameUtilities.PlayEnemyDeathAnimation(entityID);
                    GameUtilities.SpawnStaticExplosion(ECS.GetGlobalPosition(entityID), 0);
                    ECS.DestroyEntity(entityID);
                }
            }
        }

        public void TakeDamage(float damage)
        {
            if (health > 0)
            {
                health -= damage;
                ECS.PlayAudio(entityID, 0);
                GameUtilities.SpawnStaticExplosion(ECS.GetGlobalPosition(entityID), 1);
            }
            else
            {
                HandleDeath();
            }
        }

        void HandleDeath()
        {
            // This is a hardcoded way of destroying this instance, need to be replaced!
            if (isAlive)
            {
                isAlive = false;
                ECS.PlayAudio(entityID, 1);
                GameUtilities.PlayEnemyDeathAnimation(entityID);
                GameUtilities.SpawnStaticExplosion(ECS.GetGlobalPosition(entityID), 0);
            }
        }

        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
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
