﻿/******************************************************************************/
/*!
\file	BaseEnemy.cs
\author Ng Tian Kiat
\par	email: tiankiat.ng\@digipen.edu
\details
    This file holds all classes needed to create and script an enemy.

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
            or disclosure of this file or its contents without the prior
            written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
using System;
using System.Collections.Generic;

namespace Scripting
{
    // Abstract action class that all enemy actions will derive from.
    public abstract class BaseAction
    {
        public abstract bool Execute(float dt, GameObject owner = null, EnemyManager manager = null);
        public abstract bool GetIsFinished();

    }

    // This action moves an enemy from waypoint to waypoint in a given time
    public class MoveAction : BaseAction
    {
        private GameObject start_position; // starting location (local space)
        private GameObject end_position; // ending location (local space)
        private float duration; // duration of this action

        private float current_time = 0.0f;
        private bool is_finished = false;

        public MoveAction(GameObject startPos, GameObject endPos, float totalDuration)
        {
            start_position = startPos;
            end_position = endPos;
            duration = totalDuration;
        }

        public override bool Execute(float dt, GameObject owner = null, EnemyManager manager = null)
        {
            float progress = current_time / duration; // calculate current progress

            // if progress is 0, do some initialization
            if (progress <= 0.0f)
            {
                ECS.RemoveParentFrom(owner.id);
                ECS.SetTransformECS(owner.id, ECS.GetGlobalPosition(start_position.id), start_position.transform.Rotation, start_position.transform.Scale);
                ECS.PlayAudio(owner.id, 2, "SFX");
            }

            if (is_finished)
                return is_finished;

            current_time += dt;

            Vector3 startPos = ECS.GetGlobalPosition(start_position.id);
            Vector3 endPos = ECS.GetGlobalPosition(end_position.id);

            // if progress more than or equal to 1 then finish
            if (progress >= 1.0f)
            {
                is_finished = true;
                ECS.SetTransformParent(owner.id, ECS.GetTransformParent(end_position.id));
            }
            // else lerp between 2 global positions because the enemy is unparented now
            else
            {
                Vector3 target_pos = Vector3.Lerp(startPos, endPos, progress);
                ECS.SetTransformECS(owner.id, target_pos, ECS.GetGlobalRotation(end_position.id), owner.transform.Scale);
            }

            return is_finished;
        }

        public MoveAction MakeCopy()
        {
            return (MoveAction)this.MemberwiseClone();
        }

        public override bool GetIsFinished()
        {
            return is_finished;
        }
    }

    // This action causes the enemy to stop for a duration.
    public class WaitAction : BaseAction
    {
        private float duration;
        private bool is_finished = false;

        private float current_time;


        public WaitAction(float totalDuration)
        {
            duration = totalDuration;
        }
        public override bool Execute(float dt, GameObject owner = null, EnemyManager manager = null)
        {
            if (is_finished)
                return is_finished;

            current_time += dt;

            if (current_time >= duration)
                is_finished = true;

            return is_finished;
        }

        public WaitAction MakeCopy()
        {
            return (WaitAction)this.MemberwiseClone();
        }
        public override bool GetIsFinished()
        {
            return is_finished;
        }
    }

    // This action makes the enemy attack
    public class AttackAction : BaseAction
    {
        private string attack_animation; // the pattern the enemy will shoot in
        private float fire_rate; // how many bullets per second the enemy will shoot
        private int true_bullet_interval; // how many false bullets in between a true bullet
        private float duration; // how long the enemy will attack for

        //private Animator animator = null;
        //private EnemyManager em;
        private uint laser_object_id; // entityID to the laser object (optional)
        private uint Laser_object;
        private float current_time;
        //private int current_interval = 0;
        private float fire_timer = 0.0f;
        private bool is_finished = false;

        private bool isInit = false;
        private bool is_primed = false;
        private bool is_reseting = false;
        private float primer_timer;
        private float primer_time = 0.5f;
        private float bullet_speed;

        public AttackAction(string attackPattern, float fireRate, int trueBulletInterval, float totalDuration, float bulletSpeed = 5.0f)
        {
            fire_rate = 1 / fireRate;
            attack_animation = attackPattern;
            true_bullet_interval = 0; // ignore truebulletinterval param
            duration = totalDuration;
            bullet_speed = bulletSpeed;

        }
        public override bool Execute(float dt, GameObject owner = null, EnemyManager manager = null)
        {
            if (is_finished)
                return is_finished;


            // Play animation once, runtime initialization
            if (!isInit)
            {
                isInit = true;
                if (attack_animation == "Laser")
                {
                    laser_object_id = ECS.FindChildEntityWithName(owner.id, "Laser1");
                    Laser_object = ECS.FindChildEntityWithName(laser_object_id, "LaserObject");
                    ECS.SetActive(Laser_object, false);
                    Console.WriteLine("Laser entity is " + laser_object_id);
                }
            }

            if (is_reseting)
            {
                primer_timer = primer_time;
            }

            //countdown for attack to prime itself
            primer_timer -= dt;
            if (primer_timer <= 0.001f) is_primed = true;

            current_time += dt;
            fire_timer += dt;

            // Spawn bullets from enemy projectile pool
            if (fire_timer >= fire_rate && is_primed == true && is_reseting == false)
            {
                fire_timer = 0.0f;

                if (attack_animation == "Spiral")
                {
                    ++true_bullet_interval;
                    const float muzzle_number = 8;

                    if (true_bullet_interval >= muzzle_number)
                        true_bullet_interval = 0;

                    const double angle = ( 2 * Math.PI) / muzzle_number;

                    Vector3 forward_vector = Transform.GetForwardVector(owner.id);
                    Vector3 up_vector = Transform.GetUpVector(owner.id);
                    Vector3 right_vector = Vector3.CrossProduct(forward_vector, up_vector);

                    right_vector *= (float)Math.Cos(angle * true_bullet_interval) * 1.2f;
                    up_vector *= (float)Math.Sin(angle * true_bullet_interval) * 1.2f;

                    Vector3 direction = forward_vector + right_vector + up_vector;

                    GameUtilities.FireEnemyBullet(owner.id, ECS.GetGlobalPosition(owner.id) + direction * 0.3f, direction, bullet_speed, 10.0f);

                }
                else if (attack_animation == "Laser")
                {
                    //ECS.SetActive(Laser_Particle, false);
                    ECS.ResetLaser(Laser_object);
                }
                else
                    GameUtilities.FireEnemyBullet(owner.id, ECS.GetGlobalPosition(owner.id) + Transform.GetForwardVector(owner.id) * 0.2f, Transform.GetForwardVector(owner.id), bullet_speed, 3.0f);

                ECS.PlayAudio(owner.id, 3, "SFX");
            }
            else if (attack_animation == "Laser")
            {
                //ECS.SetActive(Laser_Particle, true);
                ECS.SetLaserStart(Laser_object, true);
            }


            if (current_time >= duration)
            {
                is_finished = true;
                if (attack_animation == "Laser")
                {
                    ECS.ResetLaser(Laser_object);
                    //GameUtilities.StartLaser(laser_object_id);
                }
            }

            return is_finished;
        }

        public AttackAction MakeCopy()
        {
            return (AttackAction)this.MemberwiseClone();
        }

        public override bool GetIsFinished()
        {
            return is_finished;
        }
    }

    // This action is a collection of actions, use this action when you want the enemy to perform multiple actions at once.
    public class CompositeAction : BaseAction
    {
        private BaseAction[] action_array; // list of actions to execute

        private bool is_finished = false;
        private int actions_finished = 0;
        public CompositeAction(BaseAction[] actions)
        {
            action_array = actions;
        }
        public override bool Execute(float dt, GameObject owner = null, EnemyManager manager = null)
        {
            if (is_finished)
                return is_finished;

            foreach (BaseAction item in action_array)
            {
                if (!item.GetIsFinished())
                    if (item.Execute(dt, owner, manager))
                        ++actions_finished;
            }

            if (actions_finished == action_array.Length)
                is_finished = true;
            return is_finished;
        }

        public float progress()
        {
            return (float)actions_finished / action_array.Length;
        }

        public CompositeAction MakeCopy()
        {
            return (CompositeAction)this.MemberwiseClone();
        }

        public override bool GetIsFinished()
        {
            return is_finished;
        }
    }

    // This class defines a enemy template, this is basically a set of actions the enemy will perform
    // This class is just data to create a real enemy gameobject from.
    public class EnemyTemplate
    {
        public GameObject start_location; // where the enemy will start
        public List<BaseAction> commands; // the list of instructions/actions the enemy will do after spawning
        public float life_time; // how long the enemy will stay active.

        public float health; // health of the enemy

        public EnemyTemplate(GameObject startLocation, float lifeTime, float startHealth, List<BaseAction> actions = null)
        {
            start_location = startLocation;
            life_time = lifeTime;
            health = startHealth;
            commands = actions;

            if (commands == null)
                commands = new List<BaseAction>();
        }

        public EnemyTemplate(EnemyTemplate rhs)
        {
            start_location = rhs.start_location;
            life_time = rhs.life_time;
            commands = rhs.commands;
        }

        // Add a action to an enemy
        public void AddCommand(BaseAction command)
        {
            if (command == null)
                return;
            commands.Add(command);
        }

        // Clear all actions
        public void ClearCommands()
        {
            commands.Clear();
        }

        public EnemyTemplate MakeCopy()
        {
            EnemyTemplate copy = (EnemyTemplate)this.MemberwiseClone();
            copy.ClearCommands();
            return copy;
        }
    }

    // This class defines enemy behaviour
    public class BaseEnemy
    {
        public GameObject m_DynamicExplosion_Trail;
        public GameObject m_StaticExplosionSmall;
        public GameObject m_StaticExplosion;

        EnemyTemplate my_info;
        EnemyManager em;
        GameObject gameObject;
        public Transform[] muzzles;

        private float current_lifetime = 0.0f;
        private const float deathAnimTime = 1.0f;
        private float deathAnimationTime = deathAnimTime;
        public bool is_alive;

        private bool isDiedFromPlayer = false;
        private uint DashboardScreenID = 0;

        public BaseEnemy()
        {

        }

        public BaseEnemy(EnemyTemplate template, GameObject go, uint _DashboardScreenID)
        {
            DashboardScreenID = _DashboardScreenID;
            SetTemplate(template);
            gameObject = go;
        }

        public void SetTemplate(EnemyTemplate template)
        {
            my_info = template;
        }

        public void SetManager(EnemyManager enemyManager)
        {
            em = enemyManager;
        }

        public void TakeDamage(float damage)
        {
            // if health is more than 0, take damage and play effects
            my_info.health -= damage;
            if (my_info.health > 0)
            {
                ECS.PlayAudio(gameObject.id, 0, "SFX");
            }
            else
            {
                GameUtilities.SpawnStaticExplosion(ECS.GetGlobalPosition(gameObject.id), 1);
                //GameUtilities.UpdateDashboardFace(DashboardScreenID, 1);
                DashboardScreen.SwapFace(DashboardScreen.FACES.HAPPY);
                HandleDeath(true);
            }

            //else
            //{
            //    GameUtilities.UpdateDashboardFace(DashboardScreenID, 1);
            //    HandleDeath(true);
            //}
        }

        // This function handles what happens when enemy dies,
        // when fromPlayer is true, it means that the enemy died from the player
        private void HandleDeath(bool fromPlayer)
        {
            if (is_alive)
            {
                is_alive = false;
                ECS.RemoveParentFrom(gameObject.id);
                GameUtilities.PlayEnemyDeathAnimation(gameObject.id);

                if (fromPlayer)
                {
                    ECS.PlayAudio(gameObject.id, 1, "SFX");
                    GameUtilities.SpawnStaticExplosion(ECS.GetGlobalPosition(gameObject.id), 0);
                    ++PlayerScript.m_EnemyDestroyedCount;
                }

                isDiedFromPlayer = fromPlayer;
            }
        }

        // Start is called before the first frame update
        public void Start()
        {
            current_lifetime = 0.0f;
            is_alive = true;
        }

        public void Update(float dt)
        {
            if (my_info == null)
            {
                DebugUtilities.LogToEditor("BaseEnemy", "No enemy template info is found!");
                return;
            }
            if (my_info.commands == null)
            {
                DebugUtilities.LogToEditor("BaseEnemy", "Enemy command list is null");
                return;
            }

            if (is_alive)
            {
                // Execute the actions like a sequence node in a BT
                // upate actions only if the enemy is alive
                foreach (BaseAction action in my_info.commands)
                {
                    if (!action.Execute(dt, gameObject, em))
                        break;
                }

                current_lifetime += dt;


                if (current_lifetime >= my_info.life_time && is_alive)
                    // call handledeath with false to die
                    HandleDeath(false);

            }
            else
            {
                deathAnimationTime -= dt;

                if (deathAnimationTime <= 0.0f)
                    em.DeleteEnemyInstance(gameObject.id, isDiedFromPlayer);
            }
        }

        public bool GetAlive()
        {
            return is_alive;
        }
    }
}
