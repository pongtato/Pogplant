using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class Level2OutroCutscene : MonoBehaviour
    {
        Dictionary<uint, MovingParts> moving_parts_dict;

        public enum BOSS_ANIM_STATE
        {
            TAKEOFF,
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

        //Color turret
        uint right_color_turret_1_id;
        uint right_color_turret_2_id;
        uint right_color_turret_tube_1_id;
        uint right_color_turret_tube_2_id;
        uint right_color_turret_tube_3_id;
        uint right_color_turret_light_1_id;
        uint right_color_turret_light_2_id;
        uint right_color_turret_light_3_id;
        uint right_color_turret_light_toggle_id;
        uint right_color_turret_body_pivot_id;

        uint left_color_turret_1_id;
        uint left_color_turret_2_id;
        uint left_color_turret_tube_1_id;
        uint left_color_turret_tube_2_id;
        uint left_color_turret_tube_3_id;
        uint left_color_turret_light_1_id;
        uint left_color_turret_light_2_id;
        uint left_color_turret_light_3_id;
        uint left_color_turret_light_toggle_id;
        uint left_color_turret_body_pivot_id;

        //Platforms
        uint rising_platform_id;
        uint rising_runway_id;
        uint runway_left_wing_id;
        uint runway_right_wing_id;
        uint gate_upper_id;
        uint gate_upper_2_id;
        uint gate_lower_id;
        uint gate_lower_2_id;

        uint camera_id;
        uint cinematic_bar_top_id;
        uint cinematic_bar_bottom_id;
        bool cinematic_cover_screen;
        const float cinematic_bar_speed = 3.0f;

        //Glow for turret before launch
        Vector3 turret_green_color;
        Vector3 turret_blue_color;
        Vector3 turret_emissive_blue_color;
        Vector3 turret_red_color;
        int current_color_index;
        float glow_timer;
        bool glow_change_fast;
        bool begin_glow;
        const float glow_change_slow_duration = 0.25f;
        const float glow_change_fast_duration = 0.1f;
        
        float yaw, pitch, roll;
        float cam_lerp_value;
        
        AnimationSystem boss_anim_system;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        public override void Start()
        {
            boss_anim_system = new AnimationSystem();
            boss_anim_system.Init();

            turret_green_color = new Vector3(0, 1.0f, 0);
            turret_blue_color = new Vector3(0, 0, 1.0f);
            turret_emissive_blue_color = new Vector3(0, 0.28f, 1.0f);
            turret_red_color = new Vector3(1.0f, 0, 0);

            glow_timer = 0.0f;
            current_color_index = 0;
            glow_change_fast = false;

            FindEntities();
        }

        void FindEntities()
        {
            //Main body
            cinematic_bar_top_id = ECS.FindEntityWithName("Top Cinematic Bar");
            cinematic_bar_bottom_id = ECS.FindEntityWithName("Bottom Cinematic Bar");
            camera_id = ECS.FindEntityWithName("Cinematic Camera");

            boss_model_parent_id = ECS.FindEntityWithName("Boss_Mk2");

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

            //Color turrets
            right_color_turret_1_id = ECS.FindEntityWithName("Right_ColourTurret_Pivot_1");
            right_color_turret_2_id = ECS.FindEntityWithName("Right_ColourTurret_Pivot_2");
            right_color_turret_body_pivot_id = ECS.FindEntityWithName("Right_ColourTurret_Body_Pivot");
            right_color_turret_light_1_id = ECS.FindEntityWithName("Right_ColoutTube_Emmisive_01");
            right_color_turret_light_2_id = ECS.FindEntityWithName("Right_ColoutTube_Emmisive_02");
            right_color_turret_light_3_id = ECS.FindEntityWithName("Right_ColoutTube_Emmisive_03");
            right_color_turret_tube_1_id = ECS.FindEntityWithName("Right_ColoutTube_Light_01");
            right_color_turret_tube_2_id = ECS.FindEntityWithName("Right_ColoutTube_Light_02");
            right_color_turret_tube_3_id = ECS.FindEntityWithName("Right_ColoutTube_Light_03");
            right_color_turret_light_toggle_id = ECS.FindEntityWithName("Right_Colour_ToggleObj");

            left_color_turret_1_id = ECS.FindEntityWithName("Left_ColourTurret_Pivot_1");
            left_color_turret_2_id = ECS.FindEntityWithName("Left_ColourTurret_Pivot_2");
            left_color_turret_body_pivot_id = ECS.FindEntityWithName("Left_ColourTurret_Body_Pivot");
            left_color_turret_light_1_id = ECS.FindEntityWithName("Left_ColourTube_Emmisive_01");
            left_color_turret_light_2_id = ECS.FindEntityWithName("Left_ColourTube_Emmisive_02");
            left_color_turret_light_3_id = ECS.FindEntityWithName("Left_ColourTube_Emmisive_03");
            left_color_turret_tube_1_id = ECS.FindEntityWithName("Left_ColourTube_Light_01");
            left_color_turret_tube_2_id = ECS.FindEntityWithName("Left_ColourTube_Light_02");
            left_color_turret_tube_3_id = ECS.FindEntityWithName("Left_ColourTube_Light_03");
            left_color_turret_light_toggle_id = ECS.FindEntityWithName("Left_Colour_ToggleObj");

            //Artillery
            artillery_axis_id = ECS.FindEntityWithName("Arti_Axis");
            artillery_barrel_id = ECS.FindEntityWithName("Arti_Barrel");

            //Takeoff platforms
            rising_platform_id = ECS.FindEntityWithName("Rising_Platform");
            rising_runway_id = ECS.FindEntityWithName("Runway_Platform");
            runway_left_wing_id = ECS.FindEntityWithName("Rising_Runway_Wing_01");
            runway_right_wing_id = ECS.FindEntityWithName("Rising_Runway_Wing_02");
            gate_upper_id = ECS.FindEntityWithName("Top_Gate_Part_01");
            gate_upper_2_id = ECS.FindEntityWithName("Top_Gate_Part_02");
            gate_lower_id = ECS.FindEntityWithName("Btm_Gate_Part_01");
            gate_lower_2_id = ECS.FindEntityWithName("Btm_Gate_Part_02");

            //Create and initialize the list of moving parts
            moving_parts_dict = new Dictionary<uint, MovingParts>();

            //Camera
            moving_parts_dict.Add(camera_id, new MovingParts() { entity_id = camera_id });

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

            //Color turrets
            moving_parts_dict.Add(right_color_turret_1_id, new MovingParts() { entity_id = right_color_turret_1_id });
            moving_parts_dict.Add(right_color_turret_2_id, new MovingParts() { entity_id = right_color_turret_2_id });
            moving_parts_dict.Add(right_color_turret_body_pivot_id, new MovingParts() { entity_id = right_color_turret_body_pivot_id });
            moving_parts_dict.Add(right_color_turret_tube_1_id, new MovingParts() { entity_id = right_color_turret_tube_1_id });
            moving_parts_dict.Add(right_color_turret_tube_2_id, new MovingParts() { entity_id = right_color_turret_tube_2_id });
            moving_parts_dict.Add(right_color_turret_tube_3_id, new MovingParts() { entity_id = right_color_turret_tube_3_id });

            moving_parts_dict.Add(left_color_turret_1_id, new MovingParts() { entity_id = left_color_turret_1_id });
            moving_parts_dict.Add(left_color_turret_2_id, new MovingParts() { entity_id = left_color_turret_2_id });
            moving_parts_dict.Add(left_color_turret_body_pivot_id, new MovingParts() { entity_id = left_color_turret_body_pivot_id });
            moving_parts_dict.Add(left_color_turret_tube_1_id, new MovingParts() { entity_id = left_color_turret_tube_1_id });
            moving_parts_dict.Add(left_color_turret_tube_2_id, new MovingParts() { entity_id = left_color_turret_tube_2_id });
            moving_parts_dict.Add(left_color_turret_tube_3_id, new MovingParts() { entity_id = left_color_turret_tube_3_id });

            //Artillery
            moving_parts_dict.Add(artillery_axis_id, new MovingParts() { entity_id = artillery_axis_id });
            moving_parts_dict.Add(artillery_barrel_id, new MovingParts() { entity_id = artillery_barrel_id });

            //Platform and runway
            moving_parts_dict.Add(rising_platform_id, new MovingParts() { entity_id = rising_platform_id });
            moving_parts_dict.Add(rising_runway_id, new MovingParts() { entity_id = rising_runway_id });
            moving_parts_dict.Add(runway_left_wing_id, new MovingParts() { entity_id = runway_left_wing_id });
            moving_parts_dict.Add(runway_right_wing_id, new MovingParts() { entity_id = runway_right_wing_id });
            moving_parts_dict.Add(gate_upper_id, new MovingParts() { entity_id = gate_upper_id });
            moving_parts_dict.Add(gate_upper_2_id, new MovingParts() { entity_id = gate_upper_2_id });
            moving_parts_dict.Add(gate_lower_id, new MovingParts() { entity_id = gate_lower_id });
            moving_parts_dict.Add(gate_lower_2_id, new MovingParts() { entity_id = gate_lower_2_id });

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);

            SetState(BOSS_ANIM_STATE.TAKEOFF.ToString());
        }

        public override void Update(float dt)
        {
            moving_parts_dict[left_large_laser_spin_id].SpinObjectEndless(1.0f, 0, 0, 200.0f, dt);
            moving_parts_dict[right_large_laser_spin_id].SpinObjectEndless(1.0f, 0, 0, 200.0f, dt);

            UpdateCinematicBars(dt);
            if (begin_glow)
            {
                UpdateFlashingLights(dt);
            }
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
                case BOSS_ANIM_STATE.TAKEOFF:
                    boss_anim_system.StopAnimation(true, moving_parts_dict);
                    boss_anim_system.AddAnimationSpecsStack(SetTakeOffAnimationsOne, 3.5f);
                    boss_anim_system.AddAnimationSpecsStack(SetTakeOffAnimationsTwo, 3.0f);
                    boss_anim_system.AddAnimationSpecsStack(SetTakeOffAnimationsThree, 3.0f);
                    boss_anim_system.AddAnimationSpecsStack(SetTakeOffAnimationsFour, 2.0f);
                    boss_anim_system.AddAnimationSpecsStack(SetTakeOffAnimationsFive, 4.0f);
                    boss_anim_system.AddAnimationSpecsStack(SetTakeOffAnimationsSix, 1.0f);
                    boss_anim_system.AddAnimationSpecsStack(SetTakeOffAnimationsSeven, 2.0f);
                    boss_anim_system.AddAnimationSpecsStack(SetTakeOffAnimationsEight, 2.0f);
                    boss_anim_system.AddAnimationUpdateStack(RunTakeOffSequenceOne);
                    boss_anim_system.AddAnimationUpdateStack(RunTakeOffSequenceTwo);
                    boss_anim_system.AddAnimationUpdateStack(RunTakeOffSequenceThree);
                    boss_anim_system.AddAnimationUpdateStack(RunTakeOffSequenceFour);
                    boss_anim_system.AddAnimationUpdateStack(RunTakeOffSequenceFive);
                    boss_anim_system.AddAnimationUpdateStack(RunTakeOffSequenceSix);
                    boss_anim_system.AddAnimationUpdateStack(RunTakeOffSequenceEight);
                    boss_anim_system.PlayAnimation();
                    break;
            }
        }

        void UpdateFlashingLights(float dt)
        {
            if (glow_change_fast)
            {
                if (glow_timer < glow_change_fast_duration)
                {
                    glow_timer += dt;
                }
                else
                {
                    glow_timer = 0.0f;
                    ChangeTurretLightColor();
                }
            }
            else
            {
                if (glow_timer < glow_change_slow_duration)
                {
                    glow_timer += dt;
                }
                else
                {
                    glow_timer = 0.0f;
                    ChangeTurretLightColor();
                }
            }
        }

        void ChangeTurretLightColor()
        {
            if (current_color_index < 2) //Only 3 colors (R,G,B)
            {
                ++current_color_index;
            }
            else
            {
                current_color_index = 0;
            }

            switch (current_color_index)
            {
                case 0:
                    {
                        ECS.SetDiffuseTint(right_color_turret_light_1_id, ref turret_blue_color);
                        ECS.SetEmissiveTint(right_color_turret_light_1_id, ref turret_emissive_blue_color);
                        ECS.SetDiffuseTint(right_color_turret_light_2_id, ref turret_blue_color);
                        ECS.SetEmissiveTint(right_color_turret_light_2_id, ref turret_emissive_blue_color);
                        ECS.SetDiffuseTint(right_color_turret_light_3_id, ref turret_blue_color);
                        ECS.SetEmissiveTint(right_color_turret_light_3_id, ref turret_emissive_blue_color);
                        ECS.SetDiffuseTint(right_color_turret_light_toggle_id, ref turret_blue_color);

                        ECS.SetDiffuseTint(left_color_turret_light_1_id, ref turret_blue_color);
                        ECS.SetEmissiveTint(left_color_turret_light_1_id, ref turret_emissive_blue_color);
                        ECS.SetDiffuseTint(left_color_turret_light_2_id, ref turret_blue_color);
                        ECS.SetEmissiveTint(left_color_turret_light_2_id, ref turret_emissive_blue_color);
                        ECS.SetDiffuseTint(left_color_turret_light_3_id, ref turret_blue_color);
                        ECS.SetEmissiveTint(left_color_turret_light_3_id, ref turret_emissive_blue_color);
                        ECS.SetDiffuseTint(left_color_turret_light_toggle_id, ref turret_blue_color);
                    }
                    break;
                case 1:
                    {
                        ECS.SetDiffuseTint(right_color_turret_light_1_id, ref turret_green_color);
                        ECS.SetEmissiveTint(right_color_turret_light_1_id, ref turret_green_color);
                        ECS.SetDiffuseTint(right_color_turret_light_2_id, ref turret_green_color);
                        ECS.SetEmissiveTint(right_color_turret_light_2_id, ref turret_green_color);
                        ECS.SetDiffuseTint(right_color_turret_light_3_id, ref turret_green_color);
                        ECS.SetEmissiveTint(right_color_turret_light_3_id, ref turret_green_color);
                        ECS.SetDiffuseTint(right_color_turret_light_toggle_id, ref turret_green_color);

                        ECS.SetDiffuseTint(left_color_turret_light_1_id, ref turret_green_color);
                        ECS.SetEmissiveTint(left_color_turret_light_1_id, ref turret_green_color);
                        ECS.SetDiffuseTint(left_color_turret_light_2_id, ref turret_green_color);
                        ECS.SetEmissiveTint(left_color_turret_light_2_id, ref turret_green_color);
                        ECS.SetDiffuseTint(left_color_turret_light_3_id, ref turret_green_color);
                        ECS.SetEmissiveTint(left_color_turret_light_3_id, ref turret_green_color);
                        ECS.SetDiffuseTint(left_color_turret_light_toggle_id, ref turret_green_color);
                    }
                    break;
                case 2:
                    {
                        ECS.SetDiffuseTint(right_color_turret_light_1_id, ref turret_red_color);
                        ECS.SetEmissiveTint(right_color_turret_light_1_id, ref turret_red_color);
                        ECS.SetDiffuseTint(right_color_turret_light_2_id, ref turret_red_color);
                        ECS.SetEmissiveTint(right_color_turret_light_2_id, ref turret_red_color);
                        ECS.SetDiffuseTint(right_color_turret_light_3_id, ref turret_red_color);
                        ECS.SetEmissiveTint(right_color_turret_light_3_id, ref turret_red_color);
                        ECS.SetDiffuseTint(right_color_turret_light_toggle_id, ref turret_red_color);

                        ECS.SetDiffuseTint(left_color_turret_light_1_id, ref turret_red_color);
                        ECS.SetEmissiveTint(left_color_turret_light_1_id, ref turret_red_color);
                        ECS.SetDiffuseTint(left_color_turret_light_2_id, ref turret_red_color);
                        ECS.SetEmissiveTint(left_color_turret_light_2_id, ref turret_red_color);
                        ECS.SetDiffuseTint(left_color_turret_light_3_id, ref turret_red_color);
                        ECS.SetEmissiveTint(left_color_turret_light_3_id, ref turret_red_color);
                        ECS.SetDiffuseTint(left_color_turret_light_toggle_id, ref turret_red_color);
                    }
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

        #region[Prepare takeoff Animation Sequence]
        /// <summary>
        /// [Prepare takeoff State] 
        /// 1. Boss Platform + boss raise
        /// 2. Boss stretch and move to runway
        /// 3. Runway platform fold up
        /// 4. Door open halfway
        /// 5. Door part 2 opens slowly
        /// 6. 
        /// </summary>

        void SetTakeOffAnimationsOne()
        {
            moving_parts_dict[rising_platform_id].SetLinearPosition(new Vector3(0, 192.6f, 0), new Vector3(0, 1.0f, 0), false, true, false);
            moving_parts_dict[rising_runway_id].SetLinearPosition(new Vector3(0, 201, 0), new Vector3(0, 1.0f, 0), false, true, false);
            moving_parts_dict[boss_model_parent_id].SetLinearPosition(new Vector3(0, 220.5f, 0), new Vector3(0, 1.0f, 0), false, true, false);
            moving_parts_dict[camera_id].SetLinearPosition(new Vector3(0, 242.0f, 0), new Vector3(0, 1.0f, 0), false, true, false);
        }

        void RunTakeOffSequenceOne(float dt)
        {
            moving_parts_dict[rising_platform_id].UpdateMovingParts(dt);
            moving_parts_dict[rising_runway_id].UpdateMovingParts(dt);
            moving_parts_dict[boss_model_parent_id].UpdateMovingParts(dt);
            moving_parts_dict[camera_id].UpdateMovingParts(dt);
        }

        void SetTakeOffAnimationsTwo()
        {
            moving_parts_dict[boss_model_parent_id].SetLinearPosition(new Vector3(0, 0, -588.6f), new Vector3(0, 0, 0.8f), false, false, true);

            moving_parts_dict[left_arm_middle_joint_id].SetLinearRotation(new Vector3(0, -90.0f, 0), new Vector3(0, 2.5f, 0), false, true, false);
            moving_parts_dict[left_arm_end_joint_id].SetLinearRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 2.5f), false, false, true);

            moving_parts_dict[right_arm_middle_joint_id].SetLinearRotation(new Vector3(0, 90.0f, 0), new Vector3(0, 2.5f, 0), false, true, false);
            moving_parts_dict[right_arm_end_joint_id].SetLinearRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 2.5f), false, false, true);

            moving_parts_dict[left_leg_middle_joint_id].SetLinearRotation(new Vector3(0, 90.0f, 0), new Vector3(0, 2.5f, 0), false, true, false);
            moving_parts_dict[left_leg_end_joint_id].SetLinearRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 2.5f), false, false, true);

            moving_parts_dict[right_leg_middle_joint_id].SetLinearRotation(new Vector3(0, -90.0f, 0), new Vector3(0, 2.5f, 0), false, true, false);
            moving_parts_dict[right_leg_end_joint_id].SetLinearRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 2.5f), false, false, true);

            moving_parts_dict[camera_id].SetLinearPosition(new Vector3(-623.0f, 238.0f, -549.0f), new Vector3(0.2f, 0.2f, 0.2f), true, true, true);
        }

        void RunTakeOffSequenceTwo(float dt)
        {
            moving_parts_dict[boss_model_parent_id].UpdateMovingParts(dt);

            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);

            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);

            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(dt);

            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(dt);

            moving_parts_dict[camera_id].UpdateMovingParts(dt);

            if (cam_lerp_value < 1.0f)
            {
                cam_lerp_value += 0.5f * dt;
            }
            Camera.GetCamera(entityID, ref yaw, ref pitch, ref roll);
            Vector3 updated_cam_values = Vector3.Lerp(new Vector3(yaw, pitch, roll), new Vector3(30.0f, -22.0f, 0), cam_lerp_value);
            Camera.SetCamera(camera_id, updated_cam_values.X, updated_cam_values.Y, updated_cam_values.Z);
        }

        void SetTakeOffAnimationsThree()
        {
            moving_parts_dict[boss_model_parent_id].SetPingPongPosition(new Vector3(0, 220.5f, 0), new Vector3(0, 221.0f, 0), new Vector3(0, 8.0f, 0), false, true, false, false, true, false);

            moving_parts_dict[runway_right_wing_id].SetLinearRotation(new Vector3(0, 0, 35), new Vector3(0, 0, 1.5f), false, false, true);
            moving_parts_dict[runway_left_wing_id].SetLinearRotation(new Vector3(0, 0, -35), new Vector3(0, 0, 1.5f), false, false, true);

            begin_glow = true;
        }

        void RunTakeOffSequenceThree(float dt)
        {
            moving_parts_dict[runway_left_wing_id].UpdateMovingParts(dt);
            moving_parts_dict[runway_right_wing_id].UpdateMovingParts(dt);
        }

        void SetTakeOffAnimationsFour()
        {
            moving_parts_dict[boss_model_parent_id].SetPingPongPosition(new Vector3(0, 220.75f, 0), new Vector3(0, 221.0f, 0), new Vector3(0, 8.0f, 0), false, true, false, false, true, false);

            moving_parts_dict[camera_id].SetLinearPosition(new Vector3(-625.0f, 236.0f, -562.0f), new Vector3(1.0f, 1.0f, 1.0f), true, true, true);

            moving_parts_dict[gate_upper_2_id].SetLinearPosition(new Vector3(0, 7.0f, 0), new Vector3(0, 0.7f, 0), true, true, true);
            moving_parts_dict[gate_lower_2_id].SetLinearPosition(new Vector3(0, -7.0f, 0), new Vector3(0, 0.7f, 0), true, true, true);

            cam_lerp_value = 0.0f;
        }

        void RunTakeOffSequenceFour(float dt)
        {
            if (cam_lerp_value < 1.0f)
            {
                cam_lerp_value += 0.125f * dt;
            }
            Camera.GetCamera(entityID, ref yaw, ref pitch, ref roll);
            Vector3 updated_cam_values = Vector3.Lerp(new Vector3(yaw, pitch, roll), new Vector3(21.0f, -19.5f, 0), cam_lerp_value);
            Camera.SetCamera(camera_id, updated_cam_values.X, updated_cam_values.Y, updated_cam_values.Z);

            moving_parts_dict[boss_model_parent_id].UpdateMovingParts(dt);
            moving_parts_dict[camera_id].UpdateMovingParts(dt);
            moving_parts_dict[gate_upper_2_id].UpdateMovingParts(dt);
            moving_parts_dict[gate_lower_2_id].UpdateMovingParts(dt);
        }

        void SetTakeOffAnimationsFive()
        {
            glow_change_fast = true;
            moving_parts_dict[boss_model_parent_id].SetPingPongPosition(new Vector3(0, 220.75f, 0), new Vector3(0, 221.0f, 0), new Vector3(0, 16.0f, 0), false, true, false, false, true, false);

            moving_parts_dict[gate_upper_2_id].SetLinearPosition(new Vector3(0, 7.0f, 0), new Vector3(0, 1.0f, 0), true, true, true);
            moving_parts_dict[gate_lower_2_id].SetLinearPosition(new Vector3(0, -7.0f, 0), new Vector3(0, 1.0f, 0), true, true, true);

            moving_parts_dict[gate_upper_id].SetLinearPosition(new Vector3(0, 236.0f, 0), new Vector3(0, 1.0f, 0), true, true, true);
            moving_parts_dict[gate_lower_id].SetLinearPosition(new Vector3(0, 203.0f, 0), new Vector3(0, 1.0f, 0), true, true, true);
        }

        void RunTakeOffSequenceFive(float dt)
        {
            moving_parts_dict[boss_model_parent_id].UpdateMovingParts(dt);

            moving_parts_dict[gate_upper_2_id].UpdateMovingParts(dt);
            moving_parts_dict[gate_lower_2_id].UpdateMovingParts(dt);

            moving_parts_dict[gate_upper_id].UpdateMovingParts(dt);
            moving_parts_dict[gate_lower_id].UpdateMovingParts(dt);
        }

        void SetTakeOffAnimationsSix()
        {
            moving_parts_dict[boss_model_parent_id].SetUpdatePingPongPosition(false);
            moving_parts_dict[boss_model_parent_id].SetLinearPosition(new Vector3(0, 120.0f, -2300), new Vector3(0, 0.01f, 0.5f), false, true, true);
            cam_lerp_value = 0.0f;
        }

        void RunTakeOffSequenceSix(float dt)
        {
            moving_parts_dict[boss_model_parent_id].UpdateMovingParts(dt);

            if (cam_lerp_value < 1.0f)
            {
                cam_lerp_value += 0.5f * dt;
            }
            Camera.GetCamera(entityID, ref yaw, ref pitch, ref roll);
            Vector3 updated_cam_values = Vector3.Lerp(new Vector3(yaw, pitch, roll), new Vector3(18.0f, -18.0f, 0), cam_lerp_value);
            Camera.SetCamera(camera_id, updated_cam_values.X, updated_cam_values.Y, updated_cam_values.Z);
        }

        void SetTakeOffAnimationsSeven()
        {
            cinematic_cover_screen = true;
        }

        void RunTakeOffSequenceSeven(float dt)
        {
        }

        void SetTakeOffAnimationsEight()
        {
            cinematic_cover_screen = true;
            GameUtilities.LoadScene("Level02_Boss");
        }

        void RunTakeOffSequenceEight(float dt)
        {
        }
        #endregion
    }
}
