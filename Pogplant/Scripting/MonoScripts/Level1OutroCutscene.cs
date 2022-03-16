using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class Level1OutroCutscene : MonoBehaviour
    {
        public Dictionary<uint, MovingParts> moving_parts_dict;

        public enum BOSS_ANIM_STATE
        {
            PREPARING_FLYING_UP,
            FLYING_UP,
            APPROACH_PLAYER
        }

        BOSS_ANIM_STATE current_state;

        //Entities
        uint boss_model_parent_id;

        //Arms
        uint left_arm_middle_joint_id;
        uint left_arm_end_joint_id;
        uint right_arm_middle_joint_id;
        uint right_arm_end_joint_id;

        //Arm lasers
        uint left_large_laser_spin_id;
        uint right_large_laser_spin_id;

        //Legs
        uint left_leg_middle_joint_id;
        uint right_leg_middle_joint_id;
        uint left_leg_end_joint_id;
        uint right_leg_end_joint_id;

        //Launching bays
        uint right_launching_bay_one_id;
        uint right_launching_bay_two_id;
        uint right_launching_bay_three_id;

        uint left_launching_bay_one_id;
        uint left_launching_bay_two_id;
        uint left_launching_bay_three_id;

        //Artillery
        uint artillery_axis_id;
        uint artillery_barrel_id;

        //Mouth
        uint mouth_left_id;
        uint mouth_right_id;

        uint camera_id;
        uint cinematic_bar_top_id;
        uint cinematic_bar_bottom_id;
        bool cinematic_cover_screen;
        const float cinematic_bar_speed = 3.0f;

        AnimationSystem boss_anim_system;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        public override void Start()
        {
            ECS.PlayAudio(entityID, 0, "SFX");
            boss_anim_system = new AnimationSystem();
            boss_anim_system.Init();
            FindEntities();
        }

        void FindEntities()
        {
            cinematic_bar_top_id = ECS.FindEntityWithName("Top Cinematic Bar");
            cinematic_bar_bottom_id = ECS.FindEntityWithName("Bottom Cinematic Bar");
            camera_id = ECS.FindEntityWithName("Cinematic Camera");

            //Main body
            boss_model_parent_id = ECS.FindEntityWithName("Boss");

            //Mouth
            mouth_left_id = ECS.FindEntityWithName("Mouth_L");
            mouth_right_id = ECS.FindEntityWithName("Mouth_R");

            //Arms
            left_arm_middle_joint_id = ECS.FindEntityWithName("Left_Arm_Middle_Joint");
            left_arm_end_joint_id = ECS.FindEntityWithName("Left_Arm_End_Joint");
            right_arm_middle_joint_id = ECS.FindEntityWithName("Right_Arm_Middle_Joint");
            right_arm_end_joint_id = ECS.FindEntityWithName("Right_Arm_End_Joint");

            //Arm lasers
            left_large_laser_spin_id = ECS.FindEntityWithName("Left_Large_Laser_Spin");
            right_large_laser_spin_id = ECS.FindEntityWithName("Right_Large_Laser_Spin");

            //Legs
            left_leg_middle_joint_id = ECS.FindEntityWithName("Left_Leg_Middle_Joint");
            right_leg_middle_joint_id = ECS.FindEntityWithName("Right_Leg_Middle_Joint"); ;
            left_leg_end_joint_id = ECS.FindEntityWithName("Left_Leg_End_Joint");
            right_leg_end_joint_id = ECS.FindEntityWithName("Right_Leg_End_Joint");

            //Launching bays
            right_launching_bay_one_id = ECS.FindEntityWithName("Right_Lid_01");
            right_launching_bay_two_id = ECS.FindEntityWithName("Right_Lid_02"); ;
            right_launching_bay_three_id = ECS.FindEntityWithName("Right_Lid_03");

            left_launching_bay_one_id = ECS.FindEntityWithName("Left_Lid_01");
            left_launching_bay_two_id = ECS.FindEntityWithName("Left_Lid_02");
            left_launching_bay_three_id = ECS.FindEntityWithName("Left_Lid_03");

            //Artillery
            artillery_axis_id = ECS.FindEntityWithName("Arti_Axis");
            artillery_barrel_id = ECS.FindEntityWithName("Arti_Barrel");


            //Create and initialize the list of moving parts
            moving_parts_dict = new Dictionary<uint, MovingParts>();

            //Camera
            moving_parts_dict.Add(camera_id, new MovingParts() { entity_id = camera_id } );

            //Boss body
            moving_parts_dict.Add(boss_model_parent_id, new MovingParts() { entity_id = boss_model_parent_id });

            //Core
            moving_parts_dict.Add(mouth_left_id, new MovingParts() { entity_id = mouth_left_id });
            moving_parts_dict.Add(mouth_right_id, new MovingParts() { entity_id = mouth_right_id });

            //Arms
            moving_parts_dict.Add(left_arm_middle_joint_id, new MovingParts() { entity_id = left_arm_middle_joint_id });
            moving_parts_dict.Add(left_arm_end_joint_id, new MovingParts() { entity_id = left_arm_end_joint_id });
            moving_parts_dict.Add(right_arm_middle_joint_id, new MovingParts() { entity_id = right_arm_middle_joint_id });
            moving_parts_dict.Add(right_arm_end_joint_id, new MovingParts() { entity_id = right_arm_end_joint_id });

            //Arm Lasers
            moving_parts_dict.Add(left_large_laser_spin_id, new MovingParts() { entity_id = left_large_laser_spin_id });
            moving_parts_dict.Add(right_large_laser_spin_id, new MovingParts() { entity_id = right_large_laser_spin_id });

            //Legs
            moving_parts_dict.Add(left_leg_middle_joint_id, new MovingParts() { entity_id = left_leg_middle_joint_id });
            moving_parts_dict.Add(right_leg_middle_joint_id, new MovingParts() { entity_id = right_leg_middle_joint_id });
            moving_parts_dict.Add(left_leg_end_joint_id, new MovingParts() { entity_id = left_leg_end_joint_id });
            moving_parts_dict.Add(right_leg_end_joint_id, new MovingParts() { entity_id = right_leg_end_joint_id });

            //Launching bays
            moving_parts_dict.Add(right_launching_bay_one_id, new MovingParts() { entity_id = right_launching_bay_one_id });
            moving_parts_dict.Add(right_launching_bay_two_id, new MovingParts() { entity_id = right_launching_bay_two_id });
            moving_parts_dict.Add(right_launching_bay_three_id, new MovingParts() { entity_id = right_launching_bay_three_id });

            moving_parts_dict.Add(left_launching_bay_one_id, new MovingParts() { entity_id = left_launching_bay_one_id });
            moving_parts_dict.Add(left_launching_bay_two_id, new MovingParts() { entity_id = left_launching_bay_two_id });
            moving_parts_dict.Add(left_launching_bay_three_id, new MovingParts() { entity_id = left_launching_bay_three_id });

            //Artillery
            moving_parts_dict.Add(artillery_axis_id, new MovingParts() { entity_id = artillery_axis_id });
            moving_parts_dict.Add(artillery_barrel_id, new MovingParts() { entity_id = artillery_barrel_id });

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);

            SetState(BOSS_ANIM_STATE.PREPARING_FLYING_UP.ToString());
        }

        public override void Update(float dt)
        {
            moving_parts_dict[left_large_laser_spin_id].SpinObjectEndless(1.0f, 0, 0, 200.0f, dt);
            moving_parts_dict[right_large_laser_spin_id].SpinObjectEndless(1.0f, 0, 0, 200.0f, dt);

            UpdateCinematicBars(dt);

            boss_anim_system.Update(dt);
        }

        void UpdateCinematicBars(float dt)
        {
            if (cinematic_cover_screen)
            {
                //Top bar
                if (ECS.GetGlobalPosition(cinematic_bar_top_id).Y > 0.8f)
                {
                    ECS.SetGlobalPosition(cinematic_bar_top_id, Vector3.Lerp(ECS.GetGlobalPosition(cinematic_bar_top_id), new Vector3(0.0f, 0.79f, 0.0f), cinematic_bar_speed * dt));
                }
                //Bottom bar
                if (ECS.GetGlobalPosition(cinematic_bar_bottom_id).Y < -0.8f)
                {
                    ECS.SetGlobalPosition(cinematic_bar_bottom_id, Vector3.Lerp(ECS.GetGlobalPosition(cinematic_bar_bottom_id), new Vector3(0.0f, -0.79f, 0.0f), cinematic_bar_speed * dt));
                }
            }
            else
            {
                //Top bar
                if (ECS.GetGlobalPosition(cinematic_bar_top_id).Y > 1.3f || ECS.GetGlobalPosition(cinematic_bar_top_id).Y < 1.29f)
                {
                    ECS.SetGlobalPosition(cinematic_bar_top_id, Vector3.Lerp(ECS.GetGlobalPosition(cinematic_bar_top_id), new Vector3(0.0f, 1.3f, 0.0f), cinematic_bar_speed * dt));
                }
                //Bottom bar
                if (ECS.GetGlobalPosition(cinematic_bar_bottom_id).Y < -1.3f || ECS.GetGlobalPosition(cinematic_bar_bottom_id).Y < 1.29f)
                {
                    ECS.SetGlobalPosition(cinematic_bar_bottom_id, Vector3.Lerp(ECS.GetGlobalPosition(cinematic_bar_bottom_id), new Vector3(0.0f, -1.3f, 0.0f), cinematic_bar_speed * dt));
                }
            }
        }

        public void SetState(string set_state)
        {
            current_state = (BOSS_ANIM_STATE)Enum.Parse(typeof(BOSS_ANIM_STATE), set_state);

            switch (current_state)
            {
                case BOSS_ANIM_STATE.PREPARING_FLYING_UP:
                    boss_anim_system.StopAnimation(true, moving_parts_dict);
                    boss_anim_system.AddAnimationSpecsStack(SetPreparingAnimationsOne, 1.0f);
                    boss_anim_system.AddAnimationSpecsStack(SetPreparingAnimationsTwo, 2.5f);
                    boss_anim_system.AddAnimationSpecsStack(SetPreparingAnimationsThree, 3.0f);
                    boss_anim_system.AddAnimationUpdateStack(RunPreparingSequenceOne);
                    boss_anim_system.AddAnimationUpdateStack(RunPreparingSequenceTwo);
                    boss_anim_system.AddAnimationUpdateStack(RunPreparingSequenceThree);
                    boss_anim_system.SetStateQueue(SetState, BOSS_ANIM_STATE.FLYING_UP.ToString());
                    boss_anim_system.PlayAnimation();
                    break;
                case BOSS_ANIM_STATE.FLYING_UP:
                    boss_anim_system.StopAnimation(true, moving_parts_dict);
                    boss_anim_system.AddAnimationSpecsStack(SetFlyingUpAnimationsOne, 1.2f);
                    boss_anim_system.AddAnimationSpecsStack(SetFlyingUpAnimationsTwo, 1.5f);
                    boss_anim_system.AddAnimationSpecsStack(SetFlyingUpAnimationsThree, 3.0f);
                    boss_anim_system.AddAnimationUpdateStack(RunFlyingSequenceOne);
                    boss_anim_system.AddAnimationUpdateStack(RunFlyingSequenceTwo);
                    boss_anim_system.AddAnimationUpdateStack(RunFlyingSequenceThree);
                    //boss_anim_system.SetStateQueue(SetState, BOSS_ANIM_STATE.APPROACH_PLAYER.ToString());
                    boss_anim_system.PlayAnimation();
                    break;
                case BOSS_ANIM_STATE.APPROACH_PLAYER:
                    boss_anim_system.StopAnimation(true, moving_parts_dict);
                    boss_anim_system.AddAnimationSpecsStack(SetApproachingAnimationsOne, 2.0f);
                    boss_anim_system.AddAnimationSpecsStack(SetApproachingAnimationsTwo, 1.0f);
                    boss_anim_system.AddAnimationSpecsStack(SetApproachingAnimationsThree, 0.5f);
                    boss_anim_system.AddAnimationUpdateStack(RunApproachSequenceOne);
                    boss_anim_system.AddAnimationUpdateStack(RunApproachSequenceTwo);
                    boss_anim_system.AddAnimationUpdateStack(RunApproachSequenceThree);
                    boss_anim_system.PlayAnimation();
                    break;
            }
        }

        public override void LateUpdate(float dt)
        {

        }

        public override void OnTriggerEnter(uint id)
        {

        }

        public override void OnTriggerExit(uint id)
        {

        }

        #region[Preparing Animation Sequence]
        /// <summary>
        /// [Preparing State] 
        /// 1. Arms move down
        /// 2. Arms move up and body rotates to horizontal axis
        /// 3. Vibration up/down
        /// </summary>

        void SetPreparingAnimationsOne()
        {
            //Body
            moving_parts_dict[boss_model_parent_id].SetPingPongPosition(new Vector3(0, 0, 0), new Vector3(0, 17.8f, 0), new Vector3(0, 0.1f, 0), false, true, false, false, false, false);
            moving_parts_dict[boss_model_parent_id].SetPingPongRotation(new Vector3(71.0f, 0, 0), new Vector3(0, 79.0f, 65.0f), new Vector3(0.1f, 0.1f, 0.1f), false, true, true, false, false, false);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetPingPongRotation(new Vector3(0, -42.0f, -44.0f), new Vector3(20.7f, 0, 0), new Vector3(2.0f, 2.0f, 2.0f), true, false, false, false, false, false);
            moving_parts_dict[left_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -66.0f), new Vector3(), new Vector3(0, 0, 1.0f), false, false, false, false, false, false);
            moving_parts_dict[right_arm_middle_joint_id].SetPingPongRotation(new Vector3(0, 0, 0), new Vector3(44.0f, 43.0f, 47.0f), new Vector3(2.0f, 2.0f, 2.0f), true, true, true, false, false, false);
            moving_parts_dict[right_arm_end_joint_id].SetPingPongRotation(new Vector3(), new Vector3(0, 0, 66.0f), new Vector3(0.0f, 0.0f, 1.0f), false, false, true, false, false, false);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetPingPongRotation(new Vector3(-22.5f, 0, -9.0f), new Vector3(0, 0, 0), new Vector3(12.5f, 0, 1.5f), false, false, false, false, false, false);
            moving_parts_dict[right_leg_middle_joint_id].SetPingPongRotation(new Vector3(-33.5f, -38.5f, 0), new Vector3(0, 0, 4.0f), new Vector3(1.5f, 1.5f, 1.5f), false, false, true, false, false, false);

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);
        }

        void SetPreparingAnimationsTwo()
        {
            //Camera
            moving_parts_dict[camera_id].SetPingPongPosition(new Vector3(0, 0, -154.0f), new Vector3(0, 31.0f, 0), new Vector3(0.5f, 2.0f, 0.5f), true, true, false, false, false, false);

            //Body
            moving_parts_dict[boss_model_parent_id].SetPingPongPosition(new Vector3(0, 0, 0), new Vector3(0, 23.0f, 0), new Vector3(0, 2.0f, 0), false, true, false, false, false, false);
            moving_parts_dict[boss_model_parent_id].SetPingPongRotation(new Vector3(71.0f, 0, 0), new Vector3(0, 79.0f, 65.0f), new Vector3(1.5f, 1.5f, 1.5f), false, true, true, false, false, false);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetPingPongRotation(new Vector3(-14.0f, -42.0f, 0), new Vector3(0, 0, 2.0f), new Vector3(3.0f, 3.0f, 3.0f), false, false, true, false, false, false);
            moving_parts_dict[left_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -97.0f), new Vector3(), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            moving_parts_dict[right_arm_middle_joint_id].SetPingPongRotation(new Vector3(0, 0, -14.0f), new Vector3(3.7f, 43.0f, 0), new Vector3(3.0f, 3.0f, 3.0f), true, true, false, false, false, false);
            moving_parts_dict[right_arm_end_joint_id].SetPingPongRotation(new Vector3(), new Vector3(0, 0, 97.0f), new Vector3(0.0f, 0.0f, 5.0f), false, false, true, false, false, false);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetPingPongRotation(new Vector3(-22.5f, 0, -9.0f), new Vector3(0, 0, 0), new Vector3(1.5f, 0, 1.5f), false, false, false, false, false, false);
            moving_parts_dict[left_leg_end_joint_id].SetPingPongRotation(new Vector3(-22.5f, 0, -9.0f), new Vector3(0, 0, 0), new Vector3(12.5f, 0, 1.5f), false, false, false, false, false, false);
            moving_parts_dict[right_leg_middle_joint_id].SetPingPongRotation(new Vector3(-18.5f, -38.5f, 0), new Vector3(0, 0, 16.0f), new Vector3(1.5f, 1.5f, 1.5f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_end_joint_id].SetPingPongRotation(new Vector3(), new Vector3(0, 20.0f, 72.0f), new Vector3(0, 1.5f, 1.5f), false, true, true, false, false, false);
        }

        void SetPreparingAnimationsThree()
        {
            //Body
            moving_parts_dict[boss_model_parent_id].SetPingPongPosition(new Vector3(0, 22.8f, 0), new Vector3(0, 23.2f, 0), new Vector3(0, 10.0f, 0), false, false, false, false, true, false);
        }


        void RunPreparingSequenceOne(float dt)
        {
            //Body
            moving_parts_dict[boss_model_parent_id].UpdateMovingParts(dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);

            //Arm Lasers
            moving_parts_dict[left_large_laser_spin_id].UpdateMovingParts(dt);
            moving_parts_dict[right_large_laser_spin_id].UpdateMovingParts(dt);
        }

        void RunPreparingSequenceTwo(float dt)
        {
            //Camera
            moving_parts_dict[camera_id].UpdateMovingParts(dt);
            

            //Body
            moving_parts_dict[boss_model_parent_id].UpdateMovingParts(dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);

            //Arm Lasers
            moving_parts_dict[left_large_laser_spin_id].UpdateMovingParts(dt);
            moving_parts_dict[right_large_laser_spin_id].UpdateMovingParts(dt);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(dt);
        }

        void RunPreparingSequenceThree(float dt)
        {
            //Body
            moving_parts_dict[boss_model_parent_id].UpdateMovingParts(dt);
        }
        #endregion

        #region[Flying Up Animation Sequence]
        /// <summary>
        /// [Flying State] 
        /// 1. Arms move down
        /// 2. Main body flies up
        /// 3. Black bars cover full screen, snap camera
        /// </summary>

        void SetFlyingUpAnimationsOne()
        {
            //Body
            moving_parts_dict[boss_model_parent_id].SetPingPongPosition(new Vector3(), new Vector3(0, 100.0f, 0), new Vector3(0, 1.50f, 0), false, true, false, false, false, false);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetPingPongRotation(new Vector3(0, -42.0f, -44.0f), new Vector3(20.7f, 0, 0), new Vector3(2.0f, 2.0f, 2.0f), true, false, false, false, false, false);
            moving_parts_dict[left_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -66.0f), new Vector3(), new Vector3(0, 0, 1.0f), false, false, false, false, false, false);
            moving_parts_dict[right_arm_middle_joint_id].SetPingPongRotation(new Vector3(0, 0, 0), new Vector3(44.0f, 43.0f, 47.0f), new Vector3(2.0f, 2.0f, 2.0f), true, true, true, false, false, false);
            moving_parts_dict[right_arm_end_joint_id].SetPingPongRotation(new Vector3(), new Vector3(0, 0, 66.0f), new Vector3(0.0f, 0.0f, 1.0f), false, false, true, false, false, false);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetPingPongRotation(new Vector3(-22.5f, 0, -9.0f), new Vector3(0, 0, 0), new Vector3(12.5f, 0, 1.5f), false, false, false, false, false, false);
            moving_parts_dict[right_leg_middle_joint_id].SetPingPongRotation(new Vector3(-33.5f, -38.5f, 0), new Vector3(0, 0, 4.0f), new Vector3(1.5f, 1.5f, 1.5f), false, false, true, false, false, false);
        }

        void SetFlyingUpAnimationsTwo()
        {
            //Black bars cover the whole screen
            cinematic_cover_screen = true;
        }

        void SetFlyingUpAnimationsThree()
        {
            ////Shrink black bars and move objects around
            //cinematic_cover_screen = false;
            //ECS.SetGlobalPosition(camera_id, new Vector3(-123.0f, 120.66f, -234.4f));
            //ECS.SetGlobalPosition(boss_model_parent_id, new Vector3(-123.5f, 50.0f, -259.7f));
            //Camera.SetCamera(camera_id, 0, 0, 0);

            ////Body
            //moving_parts_dict[boss_model_parent_id].SetMovingPartsPosition(boss_model_parent_id, new Vector3(), new Vector3(0, 120.7f, 0), new Vector3(0, 1.5f, 0), false, true, false, false, false, false);
            //moving_parts_dict[boss_model_parent_id].SetMovingPartsRotation(boss_model_parent_id, new Vector3(0, 0, 0), new Vector3(), new Vector3(3.0f, 3.0f, 3.0f), false, false, false, false, false, false);
            GameUtilities.LoadScene("Level01_Boss");
        }

        void RunFlyingSequenceOne(float dt)
        {
            //Body
            moving_parts_dict[boss_model_parent_id].UpdateMovingParts(dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);

            //Arm Lasers
            moving_parts_dict[left_large_laser_spin_id].UpdateMovingParts(dt);
            moving_parts_dict[right_large_laser_spin_id].UpdateMovingParts(dt);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(dt);
        }

        void RunFlyingSequenceTwo(float dt)
        {

        }

        void RunFlyingSequenceThree(float dt)
        {
            //Body
            //moving_parts_dict[boss_model_parent_id].UpdateMovingParts(boss_model_parent_id, dt);
        }

        #endregion

        #region[Approaching Player Animation Sequence]
        /// <summary>
        /// [Approaching State] 
        /// 1. Flailing animation for boss idle
        /// 2. Black screen
        /// 3. Change to L1Boss scene
        /// </summary>

        void SetApproachingAnimationsOne()
        {
            //Body
            moving_parts_dict[entityID].SetPingPongPosition(new Vector3(-0.2f, 0, 0), new Vector3(0.2f, 0, 0), new Vector3(1.0f, 0, 0), true, false, false, true, false, false);
            moving_parts_dict[entityID].SetPingPongRotation(new Vector3(0, 0, -4), new Vector3(0, 0, 4), new Vector3(0, 0, 3.0f), false, false, true, false, false, true);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetPingPongRotation(new Vector3(-90, -10, 0), new Vector3(0, 10, 10), new Vector3(10.0f, 5.0f, 0), false, false, true, false, true, false);
            moving_parts_dict[left_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -85), new Vector3(0, 0, -65), new Vector3(0, 0, 5.0f), false, false, false, false, false, true);
            moving_parts_dict[right_arm_middle_joint_id].SetPingPongRotation(new Vector3(-90, -10, 0), new Vector3(0, 10, 10), new Vector3(10.0f, 5.0f, 0), false, false, true, false, true, false);
            moving_parts_dict[right_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 65), new Vector3(0, 0, 85), new Vector3(0.0f, 0.0f, 5.0f), false, false, false, false, false, true);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90, 0, -35), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
            moving_parts_dict[left_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 50), new Vector3(0, 0, 100), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
            moving_parts_dict[right_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90, 0, 0), new Vector3(0, 0, 35), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
            moving_parts_dict[right_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -100), new Vector3(0, 0, -50), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_one_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_three_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            moving_parts_dict[left_launching_bay_one_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_two_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_three_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            //Mouth
            moving_parts_dict[mouth_left_id].SetPingPongRotation(new Vector3(0, -45, 0), new Vector3(), new Vector3(0, 2.0f, 0), false, true, false, false, true, false);
            moving_parts_dict[mouth_right_id].SetPingPongRotation(new Vector3(), new Vector3(0, 45, 0), new Vector3(0, 2.0f, 0), false, false, false, false, true, false);

            //Artillery
            moving_parts_dict[artillery_axis_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[artillery_barrel_id].SetPingPongPosition(new Vector3(), new Vector3(0, 10.3f, 0), new Vector3(10.0f, 10.0f, 10.0f), false, true, false, false, false, false);
        }

        void SetApproachingAnimationsTwo()
        {
            cinematic_cover_screen = true;
        }

        void SetApproachingAnimationsThree()
        {
            GameUtilities.LoadScene("Level01_Boss");
        }

        void RunApproachSequenceOne(float dt)
        {
            //Body
            moving_parts_dict[boss_model_parent_id].UpdateMovingParts(dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);

            //Arm Lasers
            moving_parts_dict[left_large_laser_spin_id].UpdateMovingParts(dt);
            moving_parts_dict[right_large_laser_spin_id].UpdateMovingParts(dt);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(dt);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].UpdateMovingParts(dt);
            moving_parts_dict[right_launching_bay_two_id].UpdateMovingParts(dt);
            moving_parts_dict[right_launching_bay_three_id].UpdateMovingParts(dt);

            moving_parts_dict[left_launching_bay_one_id].UpdateMovingParts(dt);
            moving_parts_dict[left_launching_bay_two_id].UpdateMovingParts(dt);
            moving_parts_dict[left_launching_bay_three_id].UpdateMovingParts(dt);

            //Artillery
            moving_parts_dict[artillery_axis_id].UpdateMovingParts(dt);
            moving_parts_dict[artillery_barrel_id].UpdateMovingParts(dt);

            //Mouth
            moving_parts_dict[mouth_left_id].UpdateMovingParts(dt);
            moving_parts_dict[mouth_right_id].UpdateMovingParts(dt);
        }

        void RunApproachSequenceTwo(float dt)
        {
            //Body
            moving_parts_dict[boss_model_parent_id].UpdateMovingParts(dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);

            //Arm Lasers
            moving_parts_dict[left_large_laser_spin_id].UpdateMovingParts(dt);
            moving_parts_dict[right_large_laser_spin_id].UpdateMovingParts(dt);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(dt);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].UpdateMovingParts(dt);
            moving_parts_dict[right_launching_bay_two_id].UpdateMovingParts(dt);
            moving_parts_dict[right_launching_bay_three_id].UpdateMovingParts(dt);

            moving_parts_dict[left_launching_bay_one_id].UpdateMovingParts(dt);
            moving_parts_dict[left_launching_bay_two_id].UpdateMovingParts(dt);
            moving_parts_dict[left_launching_bay_three_id].UpdateMovingParts(dt);

            //Artillery
            moving_parts_dict[artillery_axis_id].UpdateMovingParts(dt);
            moving_parts_dict[artillery_barrel_id].UpdateMovingParts(dt);

            //Mouth
            moving_parts_dict[mouth_left_id].UpdateMovingParts(dt);
            moving_parts_dict[mouth_right_id].UpdateMovingParts(dt);
        }

        void RunApproachSequenceThree(float dt)
        {

        }
        #endregion
    }
}
