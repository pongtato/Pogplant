using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class FirstPersonFiringSystem : MonoBehaviour
    {
        List<uint> Turrets = new List<uint>();
        List<uint> enemy_in_range = new List<uint>();
        List<uint> enemy_to_target = new List<uint>();
        List<uint> removal_list = new List<uint>();

        uint PlayerShip;
        uint shipCamera;
        uint ShootingBox;

        //Player Firing 
        float p_fireRate = 1.0f;
        float p_fire_timer = 0.0f;



        public FirstPersonFiringSystem()
        {
            // initialize private variables here
            PlayerShip = ECS.FindEntityWithName("PlayerShip");
            shipCamera = ECS.FindEntityWithName("PlayerCam");
            ShootingBox = ECS.FindEntityWithName("ShootingBox");
            uint Turret1 = ECS.FindEntityWithName("PlayerTurret1");
            uint Turret2 = ECS.FindEntityWithName("PlayerTurret2");
            Turrets.Add(Turret1);
            Turrets.Add(Turret2);
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            p_fireRate = 1 / 10.0f;
        }

        public override void Start()
        {
            
        }
        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

            //Add homing capablities
            int lower_count = enemy_in_range.Count < Turrets.Count ? enemy_in_range.Count : Turrets.Count;
            for (int i = 0; i < lower_count; ++i)
            {
                if (GameUtilities.GetTurretAlive(enemy_in_range[i]))
                {
                    if (enemy_to_target.Contains(enemy_in_range[i]))
                        continue;
                    enemy_to_target.Add(enemy_in_range[i]);
                }
                else
                {
                    removal_list.Add(enemy_in_range[i]);
                }
            }
            DoRemovalList();
            if (enemy_to_target.Count == 0)
            {
                //Shoot straight if there is no enemy
                foreach (var Turrret in Turrets)
                {
                    Transform TurretTrans = new Transform();
                    ECS.GetTransformECS(Turrret, ref TurretTrans.Position, ref TurretTrans.Rotation, ref TurretTrans.Scale);
                    TurretTrans.Rotation = new Vector3(0, 0, 0);
                    ECS.SetTransformECS(Turrret, ref TurretTrans.Position, ref TurretTrans.Rotation, ref TurretTrans.Scale);
                }

            }
            if (enemy_to_target.Count != 0)
            {
                int get_lower = enemy_to_target.Count < Turrets.Count ? enemy_to_target.Count : Turrets.Count;
                for (int i = 0; i < get_lower; ++i)
                {
                    Transform.LookAt(Turrets[i], enemy_to_target[i]);
                }
                for (int j = get_lower; j < Turrets.Count; ++j)
                {
                    Transform TurretTrans = new Transform();
                    ECS.GetTransformECS(Turrets[j], ref TurretTrans.Position, ref TurretTrans.Rotation, ref TurretTrans.Scale);
                    TurretTrans.Rotation = new Vector3(0, 0, 0);
                    ECS.SetTransformECS(Turrets[j], ref TurretTrans.Position, ref TurretTrans.Rotation, ref TurretTrans.Scale);
                }

            }
        }
        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

            p_fire_timer += dt;
            if ((InputUtility.onKeyHeld("SHOOT")))
            {
                if (p_fire_timer >= p_fireRate)
                {
                    // Call C++ side bullet firing
                    foreach(var Turret in Turrets)
                    {
                        Vector3 Forward = Transform.GetForwardVector(Turret);
                        Vector3 Position = ECS.GetGlobalPosition(Turret);
                        Vector3 Rotation = ECS.GetGlobalRotation(Turret);
                        GameUtilities.FirePlayerBullet(Position, Forward, Rotation);
                    }

                    ECS.PlayAudio(shipCamera, 1);
                    p_fire_timer = 0.0f;
                }
            }
        }

        public override void OnTriggerEnter(uint id)
        {

            Tag other_tag = ECS.GetComponent<Tag>(id);
            if (other_tag.tag == "Targetable")
            {
                AddEnemyToListOfTargets(id);
            }

        }
        public override void OnTriggerExit(uint id)
        {
            Tag other_tag = ECS.GetComponent<Tag>(id);
            if (other_tag.tag == "Targetable")
            {
                RemoveEnemyFromListOfTargets(id);
            }
        }

        void AddEnemyToListOfTargets(uint baseEnemy)
        {
            if(!enemy_in_range.Contains(baseEnemy))
            {
                enemy_in_range.Add(baseEnemy);
            }
            //UpdateReticleTargets();
        }

        void DoRemovalList()
        {
            foreach (uint removable in removal_list)
            {
                enemy_in_range.Remove(removable);
                enemy_to_target.Remove(removable);
            }
            removal_list.Clear();
        }

        public void RemoveEnemyFromListOfTargets(uint baseEnemy)
        {
            if (enemy_in_range.Contains(baseEnemy))
            {
                enemy_in_range.Remove(baseEnemy);
            }
            //UpdateReticleTargets();
        }

        void CoutMyEnemy()
        {
            Console.Write("Start: ");
            foreach (uint Enemy in enemy_to_target)
            {
                Console.Write(Enemy + ", ");
            }
            Console.WriteLine(" End");
        }
    }
}
