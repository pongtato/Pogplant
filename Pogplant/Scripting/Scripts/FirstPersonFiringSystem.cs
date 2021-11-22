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
        float p_fireRate = 0.66f;
        float p_fire_timer = 0.0f;

        //Player Crosshair(The smaller one)
        uint Crosshair;
        public float max_offset_value = 5f; //
        public float move_multipler = 0.1f; // Need to MEGAREDUCE it
        public float lerp_speed = 20.0f; // Needs high lerp speed to be smooth
        Vector3 current_offset_value; //Use only X,Y
        Vector3 shooting_box_initial_pos;
        Transform original_reticle_initial = new Transform();
        Vector3 MousePos =  new Vector3(0, 0, 0);
        bool onceFlag = false;


        public FirstPersonFiringSystem()
        {
            // initialize private variables here
            PlayerShip = ECS.FindEntityWithName("PlayerShip");
            shipCamera = ECS.FindEntityWithName("PlayerCam");
            ShootingBox = ECS.FindEntityWithName("ShootingBox");
            Crosshair = ECS.FindEntityWithName("Crosshair");
            uint Turret1 = ECS.FindEntityWithName("PlayerTurret1");
            uint Turret2 = ECS.FindEntityWithName("PlayerTurret2");
            Turrets.Add(Turret1);
            Turrets.Add(Turret2);
        }

        public override void Init(ref uint _entityID)
        {
           entityID = _entityID;
            p_fireRate = 1 / 10.0f;
            current_offset_value = new Vector3(0, 0, 0);

            Transform burner = new Transform();
            ECS.GetTransformECS(ShootingBox, ref shooting_box_initial_pos, ref burner.Rotation, ref burner.Scale);
            ECS.GetTransformECS(Crosshair, ref original_reticle_initial.Position, ref original_reticle_initial.Rotation, ref original_reticle_initial.Scale);
        }

        public override void Start()
        {
            
        }
        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            //float X = 0, Y = 0;
            //GameUtilities.GetMousePos(ref X,ref  Y);
            //Console.WriteLine("Mouse Pos: " + X + ", " + Y);
            UpdateReticleMovement(ref transform, ref dt);

            //Add homing capablities
            int lower_count = enemy_in_range.Count < Turrets.Count ? enemy_in_range.Count : Turrets.Count;
            for (int i = 0; i < lower_count; ++i)
            {
                if (GameUtilities.GetAlive(enemy_in_range[i]))
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
                    ECS.SetTransformECS(Turrret, TurretTrans.Position, TurretTrans.Rotation, TurretTrans.Scale);
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
                    ECS.SetTransformECS(Turrets[j], TurretTrans.Position, TurretTrans.Rotation, TurretTrans.Scale);
                }

            }
        }
        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

            p_fire_timer += dt;
            if ((InputUtility.onKeyHeld("SHOOT")) || InputUtility.onKeyHeld("LEFTCLICK"))
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
            enemy_to_target.Remove(baseEnemy);
            //UpdateReticleTargets();
        }

        void UpdateReticleMovement(ref Transform transform, ref float dt)
        {
            //Get mouse difference
            //Using left to update first
            Vector3 diff = new Vector3(0, 0, 0);
            //Use this to test as we need to set mouse to the middle when in game
            if (InputUtility.onKeyHeld("RIGHTCLICK") || InputUtility.onKeyTriggered("RIGHTCLICK"))
            {
                Vector3 newMousePos = new Vector3(0, 0, 0);
                if (!onceFlag)
                {
                    GameUtilities.GetMousePos(ref MousePos.X, ref MousePos.Y);
                    onceFlag = true; ;
                    newMousePos = MousePos;
                }
                GameUtilities.GetMousePos(ref newMousePos.X, ref newMousePos.Y);
                diff = newMousePos - MousePos;
                MousePos = newMousePos;

                if (InputUtility.onKeyReleased("RIGHTCLICK"))
                {
                    onceFlag = false;
                }
            }

            //Need to flip X and Y for world since mouse > is postive
            diff *= -1.0f;
            if (diff.X > 0 && current_offset_value.X < max_offset_value)
                current_offset_value.X += (diff.X * move_multipler);
            if(diff.X < 0 && current_offset_value.X > -max_offset_value)
                current_offset_value.X += (diff.X * move_multipler);

            if (diff.Y > 0 && current_offset_value.Y < max_offset_value)
                current_offset_value.Y += (diff.Y * move_multipler);
            if (diff.Y < 0 && current_offset_value.Y > -max_offset_value)
                current_offset_value.Y += (diff.Y * move_multipler);


            //Console.WriteLine("current_offset_value X: " + current_offset_value.X);
            //Console.WriteLine("current_offset_value Y: " + current_offset_value.Y);

            //Update the reticle and hitbox zone
            Transform inner_ret = new Transform();
            inner_ret.Position = new Vector3(0, 0, 0);
            ECS.GetTransformECS(Crosshair, ref inner_ret.Position, ref inner_ret.Rotation, ref inner_ret.Scale);
            inner_ret.Position = Vector3.Lerp(inner_ret.Position, original_reticle_initial.Position + current_offset_value, lerp_speed * dt);
            ECS.SetTransformECS(Crosshair, inner_ret.Position, original_reticle_initial.Rotation, original_reticle_initial.Scale);
            //ShootingBox
            transform.Position = Vector3.Lerp(transform.Position, shooting_box_initial_pos + current_offset_value, lerp_speed * dt);

            float dist = current_offset_value.magnitude();
            //Clamp the reticle
            if (dist > max_offset_value)
            {
                Vector3 maxRange = Vector3.Normalise(current_offset_value) * max_offset_value;
                transform.Position = new Vector3(maxRange.X, maxRange.Y, transform.Position.Z);
                inner_ret.Position = new Vector3(maxRange.X, maxRange.Y, original_reticle_initial.Position.Z);
                ECS.SetTransformECS(Crosshair, inner_ret.Position, original_reticle_initial.Rotation, original_reticle_initial.Scale);
            }


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
