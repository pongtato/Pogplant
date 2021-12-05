/******************************************************************************/
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    // Abstract action class that all enemy actions will derive from.
    public abstract class BaseAction
    {
        public abstract bool Execute(float dt, GameObject owner = null, EnemyManager manager = null);

    }

    // This action moves an enemy from waypoint to waypoint in a given time
    public class MoveAction : BaseAction
    {
        private GameObject start_position; // starting location (local space)
        private GameObject end_position; // ending location (local space)
        private float duration; // duration of this action

        private float current_time = 0.0f;
        private bool is_finished = false;
        private Transform transform;

        public MoveAction(GameObject startPos, GameObject endPos, float totalDuration)
        {
            start_position = startPos;
            end_position = endPos;
            duration = totalDuration;
        }

        public override bool Execute(float dt, GameObject owner = null, EnemyManager manager = null)
        {
            float progress = current_time / duration;
            if (progress <= 0.0f)
            {
                //Console.WriteLine("Started moving enemy");
                ECS.RemoveParentFrom(owner.id);
                ECS.SetTransformECS(owner.id, ECS.GetGlobalPosition(start_position.id), start_position.transform.Value.Rotation, start_position.transform.Value.Scale);
                ECS.PlayAudio(owner.id, 2);
            }

            if (is_finished)
                return is_finished;

            //Console.WriteLine("Executing move action, current progress is: " + progress);
            current_time += dt;


            if (owner != null)
            {
                transform = owner.transform.Value;
            }

            Vector3 startPos = ECS.GetGlobalPosition(start_position.id);
            Vector3 endPos = ECS.GetGlobalPosition(end_position.id);

            if (progress >= 1.0f)
            {
                //Console.WriteLine("Stopped moving enemy");
                is_finished = true;
                //ECS.SetTransformECS(owner.id, end_position.transform.Value.Position, end_position.transform.Value.Rotation, end_position.transform.Value.Scale);
                ECS.SetTransformParent(owner.id, ECS.GetTransformParent(end_position.id));
            }
            else
            {
                Vector3 target_pos = Vector3.Lerp(startPos, endPos, progress);
                //Console.WriteLine("Enemy " + owner.id + " target position: " + target_pos.X + ", " + target_pos.Y + ", " + target_pos.Z);
                ECS.SetTransformECS(owner.id, target_pos, ECS.GetGlobalRotation(end_position.id), owner.transform.Value.Scale);
            }

            return is_finished;
        }

        public MoveAction MakeCopy()
        {
            return (MoveAction)this.MemberwiseClone();
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
    }

    // This action makes the enemy attack
    public class AttackAction : BaseAction
    {
        private string attack_animation; // the pattern the enemy will shoot in
        private float fire_rate; // how many bullets per second the enemy will shoot
        private int true_bullet_interval; // how many false bullets in between a true bullet
        private float duration; // how long the enemy will attack for

        //private Animator animator = null;
        private Transform[] muzzle_transforms = new Transform[1];
        //private EnemyManager em;
        private float current_time;
        //private int current_interval = 0;
        private float fire_timer = 0.0f;
        private bool is_finished = false;

        private bool is_primed = false;
        private bool is_reseting = false;
        private float primer_timer;
        private float primer_time = 0.5f;

        public AttackAction(string attackPattern, float fireRate, int trueBulletInterval, float totalDuration)
        {
            fire_rate = 1 / fireRate;
            attack_animation = attackPattern;
            true_bullet_interval = trueBulletInterval;
            duration = totalDuration;

        }
        public override bool Execute(float dt, GameObject owner = null, EnemyManager manager = null)
        {
            if (is_finished)
                return is_finished;


            // Play animation once, runtime initialization
            if (owner != null)
            {
                //animator = owner.GetComponentInChildren<Animator>();
                //em = manager;
                //animator.Play(attack_animation);

                //muzzle_transforms = owner.GetComponent<BaseEnemy>().muzzles;
                //is_primed = false;
                //primer_timer = primer_time;
            }

            if (is_reseting)
            {
                //resets primer and reset
                //is_primed = false;
                //is_reseting = false;
                primer_timer = primer_time;
            }

            //countdown for attack to prime itself
            primer_timer -= dt;
            if (primer_timer <= 0.001f) is_primed = true;

            //checks for is_reseting
            //if (is_reseting == false)
            //{
            //    float temp = current_time + primer_time;
            //    if (temp <= duration) is_reseting = true;
            //}

            current_time += dt;
            fire_timer += dt;

            // Spawn bullets from enemy projectile pool
            if (fire_timer >= fire_rate && is_primed == true && is_reseting == false)
            {
                fire_timer = 0.0f;

                //for (int i = 0; i < muzzle_transforms.Length; ++i)
                //{
                //    if (current_interval == true_bullet_interval)
                //    {
                //        current_interval = 0;

                //        //FireBullet(em.GetBullet(true), i);
                //        GameUtilities.FireEnemyBullet(owner.id, ECS.GetGlobalPosition(owner.id), ECS.GetGlobalRotation(owner.id), true);
                //        //Debug.Log("Firing true bullet");
                //    }
                //    else
                //    {
                //        ++current_interval;
                //        //FireBullet(em.GetBullet(false), i);
                //        GameUtilities.FireEnemyBullet(owner.id, ECS.GetGlobalPosition(owner.id), ECS.GetGlobalRotation(owner.id));
                //        //Debug.Log("Firing false bullet");
                //    }
                //}
                //Console.WriteLine("Firing bullet");
                GameUtilities.FireEnemyBullet(owner.id, ECS.GetGlobalPosition(owner.id) + Transform.GetForwardVector(owner.id) * 0.2f, owner.transform.Value.Rotation);
                ECS.PlayAudio(owner.id, 3);
            }

            if (current_time >= duration)
            {
                is_finished = true;
                //animator.Play("Default");
            }

            return is_finished;
        }

        private void FireBullet(GameObject bullet, int muzzleIndex)
        {
            //if (bullet)
            //{
            //    bullet.SetActive(true);

            //    bullet.transform.SetPositionAndRotation(muzzle_transforms[muzzleIndex].position, muzzle_transforms[muzzleIndex].rotation);

            //    Projectile prj_comp = bullet.GetComponent<Projectile>();
            //    prj_comp.InitBullet();
            //}

            // Use C++ fire bullet now
            //GameUtilities.FireEnemyBullet(muzzle_transforms[muzzleIndex].Position, muzzle_transforms[muzzleIndex].Rotation);
        }

        public AttackAction MakeCopy()
        {
            return (AttackAction)this.MemberwiseClone();
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
                if (item.Execute(dt, owner, manager)) ++actions_finished;
            }

            if (actions_finished >= action_array.Length)
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

        public BaseEnemy()
        {

        }

        public BaseEnemy(EnemyTemplate template, GameObject go)
        {
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
            if (my_info.health >= 0)
            {
                my_info.health -= damage;
                ECS.PlayAudio(gameObject.id, 0);
                GameUtilities.SpawnStaticExplosion(ECS.GetGlobalPosition(gameObject.id), 1);
                //Console.WriteLine("Enemy with ID " + gameObject.id + " has taken " + damage + " damage, health is now " + my_info.health);
            }
            else
            {
                HandleDeath(true);
            }
        }

        private void HandleDeath(bool fromPlayer)
        {
            //Console.WriteLine("Enemy with ID " + gameObject.id + " has called Handle death");
            if (is_alive)
            {
                is_alive = false;
                ECS.RemoveParentFrom(gameObject.id);
                GameUtilities.PlayEnemyDeathAnimation(gameObject.id);

                if (fromPlayer)
                {
                    ECS.PlayAudio(gameObject.id, 1);
                    GameUtilities.SpawnStaticExplosion(ECS.GetGlobalPosition(gameObject.id), 0);
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
                //Console.WriteLine("No enemy template info is found!");
                return;
            }
            if (my_info.commands == null)
            {
                //Console.WriteLine("Enemy command list is null");
                return;
            }

            if (is_alive)
            {
                // Execute the actions like a sequence node in a BT
                foreach (BaseAction action in my_info.commands)
                {
                    if (!action.Execute(dt, gameObject, em))
                        break;
                }

                current_lifetime += dt;
                if (current_lifetime >= my_info.life_time && is_alive)
                {
                    //FirstPersonFiringSystem.Instance.RemoveEnemyFromListOfTargets(gameObject);
                    //Destroy(gameObject);
                    HandleDeath(false);
                }
            }
            else
            {
                deathAnimationTime -= dt;
                if (deathAnimationTime > 0.0f)
                {
                    //Transform transform = gameObject.GetComponent<Transform>();
                    //Vector3 new_rotation = transform.Rotation;
                    //new_rotation.X += 180.0f * dt;
                    //new_rotation.Y += 90.0f * dt;
                    //new_rotation.Z += 270.0f * dt;

                    //ECS.SetTransformECS(gameObject.id, transform.Position, new_rotation, transform.Scale);
                }
                else
                {
                    //GameUtilities.SpawnStaticExplosion(ECS.GetGlobalPosition(gameObject.id), 0);
                    em.DeleteEnemyInstance(gameObject.id, isDiedFromPlayer);
                }
            }
            //Console.WriteLine("End of BaseEnemy.Update()");
        }

        public bool GetAlive()
        {
            return is_alive;
        }
    }
}
