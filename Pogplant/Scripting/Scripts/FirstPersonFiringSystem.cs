using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class FirstPersonFiringSystem : MonoBehaviour
    {
        List<uint> ReticleGroup = new List<uint>();
        List<uint> Turrets = new List<uint>();
        List<uint> enemy_in_range = new List<uint>();
        List<uint> enemy_to_target = new List<uint>();
        List<uint> removal_list = new List<uint>();

        uint PlayerShip;
        uint shipCamera;
        uint ShootingBox;
        uint ReticleGroupID;
        uint PlayerBox;

        uint ScoreText; // id of score text object

        //Player Firing 
        float p_fireRate = 0.1f;
        float p_fire_timer = 0.0f;

        //Player Crosshair(The smaller one)
        uint Crosshair;
        public float max_offset_value = 0.1f; // For non Canvas is 5
        public float move_multipler = 0.001f; // Non-Canvas value is 0.1;
        public float lerp_speed = 20.0f; // Needs high lerp speed to be smooth
        Vector3 current_offset_value; //Use only X,Y
        Vector3 shooting_box_initial_pos;
        Transform original_reticle_initial = new Transform();
        Vector3 MousePos =  new Vector3(0, 0, 0);
        bool onceFlag = false;

        //Reticle stuff
        Vector3 start_position = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 Small_hide_reticle = new Vector3(0.01f, 0.01f, 0.01f);
        Vector3 Show_reticle = new Vector3(1f, 1f, 1f);

        public FirstPersonFiringSystem()
        {
            // initialize private variables here
            PlayerBox = ECS.FindEntityWithName("PlayerBox");
            PlayerShip = ECS.FindEntityWithName("PlayerShip");
            shipCamera = ECS.FindEntityWithName("PlayerCam");
            ShootingBox = ECS.FindEntityWithName("ShootingBox");
            Crosshair = ECS.FindEntityWithName("Crosshair");
            ReticleGroupID = ECS.FindEntityWithName("ReticleGroup");
            ScoreText = ECS.FindEntityWithName("Score_Text");

            Turrets.Add(ECS.FindChildEntityWithName(PlayerShip, "PlayerTurret1"));
            Turrets.Add(ECS.FindChildEntityWithName(PlayerShip, "PlayerTurret2"));
            Turrets.Add(ECS.FindChildEntityWithName(PlayerShip, "PlayerTurret3"));
            Turrets.Add(ECS.FindChildEntityWithName(PlayerShip, "PlayerTurret4"));

            ReticleGroup.Add(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle1"));
            ReticleGroup.Add(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle2"));
            ReticleGroup.Add(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle3"));
            ReticleGroup.Add(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle4"));
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            current_offset_value = new Vector3(0, 0, 0);

            Transform burner = new Transform();
            ECS.GetTransformECS(ShootingBox, ref shooting_box_initial_pos, ref burner.Rotation, ref burner.Scale);
            ECS.GetTransformECS(Crosshair, ref original_reticle_initial.Position, ref original_reticle_initial.Rotation, ref original_reticle_initial.Scale);
        }

        public override void Start()
        {
            
        }
        public override void Update(float dt)
        {
            //float X = 0, Y = 0;
            //GameUtilities.GetMousePos(ref X,ref  Y);
            //Console.WriteLine("Mouse Pos: " + X + ", " + Y);

            //UpdateReticleMovement(ref transform, ref dt);
            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();
            ECS.GetTransformECS(entityID, ref pos, ref rot, ref scale);
            Transform transform = new Transform(pos, rot, scale);

            UpdateReticleMovementCanvas(ref transform, ref dt);

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
                for (int i = 0; i < Turrets.Count; ++i)
                {
                    Transform TurretTrans = new Transform();
                    ECS.GetTransformECS(Turrets[i], ref TurretTrans.Position, ref TurretTrans.Rotation, ref TurretTrans.Scale);
                    TurretTrans.Rotation = new Vector3(0, 0, 0);
                    ECS.SetTransformECS(Turrets[i], TurretTrans.Position, TurretTrans.Rotation, TurretTrans.Scale);
                    ECS.SetTransformECS(ReticleGroup[i], start_position, start_position, Small_hide_reticle);
                }
            }
            if (enemy_to_target.Count != 0)
            {
                int get_lower = enemy_to_target.Count < Turrets.Count ? enemy_to_target.Count : Turrets.Count;
                for (int i = 0; i < get_lower; ++i)
                {
                    //Turret in use
                    //Need to look at the future position
                    Transform.LookAt(Turrets[i], ECS.GetGlobalPosition(enemy_to_target[i]));
                    //Update Reticle need to flip Z and X
                    Vector3 EnemyPos = ECS.GetGlobalPosition(enemy_to_target[i]);
                    Vector3 ShipForward = Transform.GetForwardVector(PlayerShip);
                    EnemyPos.X *= -1.0f;
                    EnemyPos.Z *= -1.0f;
                    ECS.SetTransformECS(ReticleGroup[i], EnemyPos - ShipForward, ECS.GetGlobalRotation(PlayerShip), Show_reticle);
                }
                for (int j = get_lower; j < Turrets.Count; ++j)
                {
                    //Turrets not in use
                    Transform TurretTrans = new Transform();
                    ECS.GetTransformECS(Turrets[j], ref TurretTrans.Position, ref TurretTrans.Rotation, ref TurretTrans.Scale);
                    TurretTrans.Rotation = new Vector3(0, 0, 0);
                    ECS.SetTransformECS(Turrets[j], TurretTrans.Position, TurretTrans.Rotation, TurretTrans.Scale);
                    //Reset Reticle
                    ECS.SetTransformECS(ReticleGroup[j], start_position, start_position, Small_hide_reticle);
                }
            }

            ECS.SetTransformECS(entityID, pos, rot, scale);
        }
        public override void LateUpdate(float dt)
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
                        Vector3 Position = ECS.GetGlobalPosition(Turret) + Forward * 0.55f;
                        Vector3 Rotation = ECS.GetGlobalRotation(Turret);
                        GameUtilities.FirePlayerBullet(Position, Forward, Rotation);

                        GameUtilities.InstantiateParticle("GunFire", Position, Rotation, true, PlayerShip);
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
            //if (InputUtility.onKeyHeld("RIGHTCLICK") || InputUtility.onKeyTriggered("RIGHTCLICK"))
            //{
            //    Vector3 newMousePos = new Vector3(0, 0, 0);
            //    if (!onceFlag)
            //    {
            //        GameUtilities.GetMousePos(ref MousePos.X, ref MousePos.Y);
            //        onceFlag = true; ;
            //        newMousePos = MousePos;
            //    }
            //    GameUtilities.GetMousePos(ref newMousePos.X, ref newMousePos.Y);
            //    diff = newMousePos - MousePos;
            //    MousePos = newMousePos;

            //    if (InputUtility.onKeyReleased("RIGHTCLICK"))
            //    {
            //        onceFlag = false;
            //    }
            //}

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

        void UpdateReticleMovementCanvas(ref Transform transform, ref float dt)
        {
            //Get mouse difference
            //Using left to update first
            Vector3 diff = new Vector3(0, 0, 0);

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

            diff.Y *= -1.0f;

            if (diff.X > 0 && current_offset_value.X < max_offset_value)
                current_offset_value.X += (diff.X * move_multipler);
            if (diff.X < 0 && current_offset_value.X > -max_offset_value)
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
            transform.Position = Vector3.Lerp(transform.Position, shooting_box_initial_pos + (current_offset_value  * 50.0f), lerp_speed * dt);

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
            foreach (uint Enemy in enemy_in_range)
            {
                Console.Write(Enemy + ", ");
            }
            Console.WriteLine(" End");
        }
    }
}
