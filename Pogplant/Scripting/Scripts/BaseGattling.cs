using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    // Enemy script class
    public class BaseGattling : MonoBehaviour
    {
        uint PlayerBox = 0;
        uint DashboardScreenID;
        uint muzzle_id = 0;
        uint turret_pivot = 0;
        //public Transform muzzle_transform = new Transform();
        //public Transform turret_pivot_transform = new Transform();
        public float health = 20.0f;

        //Turning speed 
        float pivot_turning = 10.0f;
        float gattling_turning = 250.0f;
        float max_angle = 10.0f;
        bool y_direction_switch = true;

        //Firing stuff
        float can_fire_timer = 0.0f;
        float fire_window = 10.0f;
        float fire_cooldown = 1.0f;
        float fireRate;
        float fire_timer = 0.0f;
        bool can_fire  = true;

        bool isFiring = false;
        bool isAlive = true;
        float deathAnimationTime = 4.0f; 

        public BaseGattling()
        {
            // initialize private variables here
            fireRate =  0.1f;

        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            muzzle_id = ECS.FindChildEntityWithName(_entityID, "Rotating_Gatling_Barrel");
            turret_pivot = ECS.FindChildEntityWithName(_entityID, "Turret_Pivot");
            PlayerBox = ECS.FindEntityWithName("PlayerBox");
            DashboardScreenID = ECS.FindEntityWithName("DashboardScreenFace");
            //muzzle_transform= ECS.GetComponent<Transform>(muzzle_id);
            //turret_pivot_transform = ECS.GetComponent<Transform>(turret_pivot);
            //Console.WriteLine("Turret Enemy ID:" + entityID + " has spawned.");

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
                //Do the rotation for gattling( Move by Z)
                Transform move_muzzle = ECS.GetComponent<Transform>(muzzle_id);
                move_muzzle.Rotation.Z += gattling_turning * dt;
                if(move_muzzle.Rotation.Z >= 180.0f)
                {
                    move_muzzle.Rotation.Z = -179.0f;
                }
                ECS.SetTransformECS(muzzle_id, move_muzzle.Position, move_muzzle.Rotation, move_muzzle.Scale);
                //Do pivot rotation for gattling (Move by Y)
                Transform move_pivot = ECS.GetComponent<Transform>(turret_pivot);
                if(y_direction_switch)
                    move_pivot.Rotation.Y += pivot_turning * dt;
                if (!y_direction_switch)
                    move_pivot.Rotation.Y -= pivot_turning * dt;
                if(Math.Abs(move_pivot.Rotation.Y) >= max_angle)
                    y_direction_switch = !y_direction_switch;
                ECS.SetTransformECS(turret_pivot, move_pivot.Position, move_pivot.Rotation, move_pivot.Scale);

                if (isFiring)
                {
                    can_fire_timer += dt;
                    if (can_fire)
                    {
                        fire_timer += dt;
                        if (fire_timer >= fireRate)
                        {
                            // Call C++ side bullet firing
                            GameUtilities.FireEnemyBullet(muzzle_id, ECS.GetGlobalPosition(muzzle_id) + Transform.GetForwardVector(muzzle_id) , ECS.GetGlobalRotation(muzzle_id), 10.0f, 7.0f);
                            ECS.PlayAudio(entityID, 2);
                            fire_timer = 0.0f;
                        }
                        if(can_fire_timer >= fire_window)
                        {
                            can_fire = false;
                            fire_timer = 0.0f;
                            can_fire_timer = 0.0f;
                        }
                    }
                    if(!can_fire)
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
                    //transform.Rotation.X += 180.0f * dt;
                    //transform.Rotation.Y += 90.0f * dt;
                    //transform.Rotation.Z += 270.0f * dt;
                }
                else
                {
                    //Console.WriteLine("Turret (id: " + entityID + ") has died");
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
                ECS.PlayAudio(entityID, 0);
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
            if (isAlive)
            {
                isAlive = false;
                isFiring = false;
                ECS.PlayAudio(entityID, 1);
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
