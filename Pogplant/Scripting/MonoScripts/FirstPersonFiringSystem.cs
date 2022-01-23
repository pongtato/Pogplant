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
        List<uint> enemy_to_target = new List<uint>();
        List<uint> Turrets_A = new List<uint>();
        List<uint> Turrets_B = new List<uint>();
        static List<uint> enemy_in_range_A = new List<uint>();
        static List<uint> enemy_to_target_A = new List<uint>();
        static List<uint> enemy_in_range_B = new List<uint>();
        static List<uint> enemy_to_target_B = new List<uint>();
        List<uint> removal_list = new List<uint>();
        List<uint> duplicates = new List<uint>();

        uint PlayerShip;
        uint shipCamera;
        uint ShootingBox;
        uint ReticleGroupID;
        uint PlayerBox;

        uint ScoreText; // id of score text object

        //Player Firing 
        float p_fireRate = 0.1f;
        float p_fire_timer = 0.0f;
        float m_rotspeed = 10.0f; 

        //Player Crosshair(The smaller one)
        uint Crosshair;
        public float controller_move_multiplier = 10.0f;
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
        Vector3 Small_hide_reticle = new Vector3(0.01f, 0.01f, 0f);
        Vector3 Show_reticle = new Vector3(1f, 1f, 0f);
        Vector3 start_reticle = new Vector3(3f, 3f, 0f);
        float rotateAmount = 7.5f;
        float animateSpeed = 6.0f;

        public class  Reticle
        {
            public uint parent_id = 0;
            public uint child_id = 0;           
            public bool enabled = false;
            public float accu_angle = 0;
            public float step = 0;
            public Reticle( uint parentid, uint chilid, bool enable = false) { enabled = enable; parent_id = parentid; child_id = chilid; }
        };
        List<Reticle> ReticleGroup = new List<Reticle>();
        Transform transform;
        public FirstPersonFiringSystem()
        {

            //ReticleGroup.Add(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle2"));
            //ReticleGroup.Add(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle3"));
            //ReticleGroup.Add(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle4"));
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            // initialize private variables here
            PlayerBox = ECS.FindEntityWithName("PlayerBox");
            PlayerShip = ECS.FindEntityWithName("PlayerShip");
            shipCamera = ECS.FindEntityWithName("PlayerCam");
            ShootingBox = ECS.FindEntityWithName("ShootingBox");
            Crosshair = ECS.FindEntityWithName("Crosshair");
            ReticleGroupID = ECS.FindEntityWithName("ReticleGroup");
            ScoreText = ECS.FindEntityWithName("Score_Text");

            Turrets_A.Add(ECS.FindChildEntityWithName(PlayerShip, "PlayerTurret1"));
            Turrets_A.Add(ECS.FindChildEntityWithName(PlayerShip, "PlayerTurret2"));
            Turrets_B.Add(ECS.FindChildEntityWithName(PlayerShip, "PlayerTurret3"));
            Turrets_B.Add(ECS.FindChildEntityWithName(PlayerShip, "PlayerTurret4"));

            ReticleGroup.Add(new Reticle(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle1"), ECS.FindChildEntityWithName(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle1"), "Child")));
            ReticleGroup.Add(new Reticle(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle2"), ECS.FindChildEntityWithName(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle2"), "Child")));
            ReticleGroup.Add(new Reticle(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle3"), ECS.FindChildEntityWithName(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle3"), "Child")));
            ReticleGroup.Add(new Reticle(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle4"), ECS.FindChildEntityWithName(ECS.FindChildEntityWithName(ReticleGroupID, "Reticle4"), "Child")));

            current_offset_value = new Vector3(0, 0, 0);

            Transform burner = new Transform();
            ECS.GetTransformECS(ShootingBox, ref shooting_box_initial_pos, ref burner.Rotation, ref burner.Scale);
            ECS.GetTransformECS(Crosshair, ref original_reticle_initial.Position, ref original_reticle_initial.Rotation, ref original_reticle_initial.Scale);
            ECS.GetTransformECS(entityID, ref transform.Position, ref transform.Rotation, ref transform.Scale);
        }

        public override void Start()
        {
            
        }

        public override void Update(float dt)
        {
            //UpdateReticleMovement(ref transform, ref dt);

            ECS.GetTransformECS(entityID, ref transform.Position, ref transform.Rotation, ref transform.Scale);

            UpdateReticleMovementCanvas(ref transform, ref dt);

            //Add homing capablities
            SetTargetEnemies(ref Turrets_A, ref enemy_in_range_A, ref enemy_to_target_A);
            SetTargetEnemies(ref Turrets_B, ref enemy_in_range_B, ref enemy_to_target_B);
            DoRemovalList();
            //RemoveDuplicates(ref enemy_to_target_A, ref enemy_to_target_B);
            if (enemy_to_target_A.Count == 0)
            {
                SetTurretStraight(ref Turrets_A, 0, dt);
            }
            if (enemy_to_target_B.Count == 0)
            {
                SetTurretStraight(ref Turrets_B, 2, dt);
            }
            if (enemy_to_target_A.Count != 0)
            {
                SetReticleandTurret(ref Turrets_A, ref enemy_to_target_A, 0, dt);
            }
            if (enemy_to_target_B.Count != 0)
            {
                SetReticleandTurret(ref Turrets_B, ref enemy_to_target_B, 2, dt);
            }
            ECS.SetTransformECS(entityID, transform.Position, transform.Rotation, transform.Scale);
        }
        public override void LateUpdate(float dt)
        {

            p_fire_timer += dt;
            if ((InputUtility.onKeyHeld("SHOOT")) || InputUtility.onKeyHeld("LEFTCLICK"))
            {
                if (p_fire_timer >= p_fireRate)
                {
                    // Call C++ side bullet firing
                    CallTurretShoot(ref Turrets_A);
                    CallTurretShoot(ref Turrets_B);
                    ECS.PlayAudio(shipCamera, 1, "SFX");
                    p_fire_timer = 0.0f;

                    //Vibrate controller
                    InputUtility.VibrateControllerLightMotor(0.25f, 0.05f);
                }
            }
        }

        public override void OnTriggerEnter(uint id)
        {

        }
        public override void OnTriggerExit(uint id)
        {
        }

        public static void AddEnemyToListOfTargets(uint baseEnemy, uint BoxType)
        {
            if (BoxType == 0)
            {
                //Go Box Type A
                if (!enemy_in_range_A.Contains(baseEnemy))
                {
                    enemy_in_range_A.Add(baseEnemy);
                }
            }
            else if (BoxType == 1)
            {
                //Go Box Type B
                if (!enemy_in_range_B.Contains(baseEnemy))
                {
                    enemy_in_range_B.Add(baseEnemy);
                }
            }
            else
            {
                Console.WriteLine("Error adding Enemy list of targets");
            }

        }
        void DoRemovalList()
        {
            foreach (uint removable in removal_list)
            {
                enemy_in_range_A.Remove(removable);
                enemy_to_target_A.Remove(removable);
                enemy_in_range_B.Remove(removable);
                enemy_to_target_B.Remove(removable);
            }
            removal_list.Clear();
        }
        public static void RemoveEnemyFromListOfTargets(uint baseEnemy, uint BoxType)
        {
            if (BoxType == 0)
            {
                //Go Box Type A
                enemy_in_range_A.Remove(baseEnemy);
                enemy_to_target_A.Remove(baseEnemy);
            }
            else if (BoxType == 1)
            {
                //Go Box Type B
                enemy_in_range_B.Remove(baseEnemy);
                enemy_to_target_B.Remove(baseEnemy);
            }
            else
            {
                Console.WriteLine("Error removing Enemy list of targets");
            }
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

            //Check for controller movement if no mouse
            if((Math.Abs(diff.X) < float.Epsilon) && (Math.Abs(diff.Y) < float.Epsilon))
            {
                diff.X = InputUtility.GetAxis("AIMX") * controller_move_multiplier;
                diff.Y = InputUtility.GetAxis("AIMY") * controller_move_multiplier;
            }

            diff.Y *= -1.0f;

            if (diff.X > 0 && current_offset_value.X < max_offset_value)
                current_offset_value.X += (diff.X * move_multipler);
            if (diff.X < 0 && current_offset_value.X > -max_offset_value)
                current_offset_value.X += (diff.X * move_multipler);

            if (diff.Y > 0 && current_offset_value.Y < max_offset_value)
                current_offset_value.Y += (diff.Y * move_multipler);
            if (diff.Y < 0 && current_offset_value.Y > -max_offset_value)
                current_offset_value.Y += (diff.Y * move_multipler);


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

        void ResetReticle(Reticle ret)
        {
            ret.enabled = false;
            ret.accu_angle = 0.0f;
            ret.step = 0.0f;
            ECS.SetRotation(ret.child_id, new Vector3(0));
            ECS.SetTransformECS(ret.parent_id, start_position, start_position, Small_hide_reticle);
        }

        void SetTargetEnemies(ref List<uint> TurretGroup, ref List<uint> EnemyinRangeGroup, ref List<uint> EnemytoTarget)
        {
            int lower_count = EnemyinRangeGroup.Count < TurretGroup.Count ? EnemyinRangeGroup.Count : TurretGroup.Count;
            for (int i = 0; i < lower_count; ++i)
            {
                if (GameUtilities.GetAlive(EnemyinRangeGroup[i]))
                {
                    if (EnemytoTarget.Contains(EnemyinRangeGroup[i]))
                        continue;
                    EnemytoTarget.Add(EnemyinRangeGroup[i]);
                }
                else
                {
                    removal_list.Add(EnemyinRangeGroup[i]);
                }
            }
        }

        void SetTurretStraight(ref List<uint> TurretGroup, int Offset, float dt)
        {
            for (int i = 0; i < TurretGroup.Count; ++i)
            {
                Vector3 CurrTurrRot = ECS.GetComponent<Transform>(TurretGroup[i]).Rotation;
                ECS.SetRotation(TurretGroup[i], Vector3.Lerp(CurrTurrRot, new Vector3(0, 0, 0), dt * m_rotspeed));
                //Reset Reticle
                ResetReticle(ReticleGroup[i + Offset]);
            }
        }

        public void LockonEnemies(uint Turret, uint EnemytoTarget, float dt)
        {
            Vector3 CurrTurrRot = ECS.GetComponent<Transform>(Turret).Rotation;
            Transform.LookAt(Turret, ECS.GetGlobalPosition(EnemytoTarget));
            Vector3 AfterTurrRot = ECS.GetComponent<Transform>(Turret).Rotation;
            ECS.SetRotation(EnemytoTarget, Vector3.Lerp(CurrTurrRot, AfterTurrRot, dt * m_rotspeed));
        }

        public void SetReticleandTurret(ref List<uint> TurretGroup, ref List<uint> EnemytoTarget, int RetOffSet, float dt)
        {
            int get_lower = EnemytoTarget.Count < TurretGroup.Count ? EnemytoTarget.Count : TurretGroup.Count;
            for (int i = 0; i < get_lower; ++i)
            {
                //Turret in use
                LockonEnemies(TurretGroup[i], EnemytoTarget[i], dt);

                //Update Reticle 
                //To ensure the reticle doesnt clip agaisnt big enemies
                Vector3 EnemyPos = ECS.GetGlobalPosition(EnemytoTarget[i]) - (Transform.GetForwardVector(shipCamera) * 0.3F);
                Vector3 Rot = ECS.GetGlobalRotation(shipCamera);
                if (Rot.X < 0)
                    Rot.X += 180.0f;
                else
                    Rot.X -= 180.0f;
                int k = i + RetOffSet;
                ReticleGroup[k].step += dt * animateSpeed;
                if (ReticleGroup[k].step >= 1.0f)
                    ReticleGroup[k].step = 1.0f;
                ECS.SetTransformECS(ReticleGroup[k].parent_id, EnemyPos, Rot, Vector3.Lerp(start_reticle, Show_reticle, ReticleGroup[k].step));
                //Rotate child reticle aftewards
                if (!ReticleGroup[k].enabled)
                    ReticleGroup[k].enabled = true;
                else
                {
                    ReticleGroup[k].accu_angle -= rotateAmount * animateSpeed * dt;
                    ECS.SetRotation(ReticleGroup[k].child_id, new Vector3(0, 0, ReticleGroup[k].accu_angle));
                }


            }
            for (int j = get_lower; j < TurretGroup.Count; ++j)
            {
                //Set the other to target the same enemy
                LockonEnemies(TurretGroup[j], EnemytoTarget[j - 1], dt);

                //Vector3 CurrTurrRot = ECS.GetComponent<Transform>(Turrets[j]).Rotation;
                //ECS.SetRotation(Turrets[j], Vector3.Lerp(CurrTurrRot, new Vector3(0, 0, 0), dt * m_rotspeed));

                //Reset Reticle
                ResetReticle(ReticleGroup[j + RetOffSet]);
            }
        }

        public void CallTurretShoot(ref List<uint> TurretGroup)
        {
            foreach (var Turret in TurretGroup)
            {
                Vector3 Forward = Transform.GetForwardVector(Turret);
                Vector3 Position = ECS.GetGlobalPosition(Turret) + Forward * 0.55f;
                Vector3 Rotation = ECS.GetGlobalRotation(Turret);
                GameUtilities.FirePlayerBullet(Position, Forward, Rotation);

                GameUtilities.InstantiateParticle("GunFire", Position, Rotation, true, PlayerShip);
            }
        }

        public void RemoveDuplicates(ref List<uint> TurretGroup1, ref List<uint> TurretGroup2)
        {
            duplicates = TurretGroup1.Intersect(TurretGroup2).ToList();
            //Condition to remove
            if (TurretGroup1.Count > TurretGroup2.Count)
            {
                foreach (var duplicate in duplicates)
                {
                    TurretGroup1.Remove(duplicate);
                }
            }
            else
            {
                foreach (var duplicate in duplicates)
                {
                    TurretGroup2.Remove(duplicate);
                }
            }
            duplicates.Clear();
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
