using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class FirstPersonFiringSystem : MonoBehaviour
    {

        List<uint> enemy_in_range = new List<uint>();
        List<uint> removal_list = new List<uint>();
        // private GameObject[] target_enemy_arr;
        private Vector3 Aimshot;
        bool isStraight = true;
        bool target = true;

        uint PlayerShip;
        uint shipCamera;
        uint Turret1;

        //Player Firing 
        float p_fireRate = 1.0f;
        float p_fire_timer = 0.0f;



        public FirstPersonFiringSystem()
        {
            // initialize private variables here
            PlayerShip = ECS.FindEntityWithName("PlayerShip");
            shipCamera = ECS.FindEntityWithName("PlayerCam");
            Turret1 = ECS.FindEntityWithName("PlayerTurret1");
            Aimshot = Vector3.Zero();
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
            int i = 0;
            bool onlytarget = true;
            if(enemy_in_range.Count == 0)
            {
                isStraight = true;
            }
            else
            {
                isStraight = false;
                foreach (var Enemy in enemy_in_range)
                {
                    if(GameUtilities.GetTurretAlive(Enemy))
                    {
                        if(onlytarget)
                        {
                            Transform enemyTrans = new Transform();
                            enemyTrans.Position = ECS.GetGlobalPosition(Enemy);
                            //Get Player transform
                            Transform PlayerTrans = new Transform();
                            PlayerTrans.Position = ECS.GetGlobalPosition(PlayerShip);
                            Transform.LookAt(Turret1, Enemy);
                            //Aimshot = Vector3.Normalise(enemyTrans.Position - PlayerTrans.Position);
                            onlytarget = false;
                        }
                    }
                    else
                    {
                        removal_list.Add(Enemy);
                    }
                }
            }
            DoRemovalList();

        }
        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

            p_fire_timer += dt;
            if ((InputUtility.onKeyHeld("SHOOT")))
            {
                if (p_fire_timer >= p_fireRate)
                {
                    // Call C++ side bullet firing
                    Transform PlayerTrans = new Transform();
                    Vector3 Forward = Transform.GetForwardVector(Turret1);
                    PlayerTrans.Position = ECS.GetGlobalPosition(PlayerShip);
                    PlayerTrans.Rotation = ECS.GetGlobalRotation(PlayerShip);

                    if (isStraight)
                    {
                        GameUtilities.FirePlayerBullet(entityID, Transform.GetForwardVector(PlayerShip), PlayerTrans.Rotation);
                    }
                    else
                    {
                        GameUtilities.FirePlayerBullet(entityID, Forward, PlayerTrans.Rotation);
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
                //if (GameUtilities.GetTurretAlive(id))
                //{
                //    Console.WriteLine("Booya");
                //}
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
            foreach (uint Enemy in enemy_in_range)
            {
                Console.Write(Enemy + ", ");
            }
            Console.WriteLine(" End");
        }
    }
}
