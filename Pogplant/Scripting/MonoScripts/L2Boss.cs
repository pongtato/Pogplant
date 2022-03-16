using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class L2Boss : MonoBehaviour
    {
        static public L2Boss m_singleton;
        public Dictionary<uint, MovingParts> moving_parts_dict;

        public enum BOSS_BEHAVIOUR_STATE
        {
            EMPTY,
            SPINNING_UP,
            FLYING_UP,
            MOVING,
            PROTECTION,
            LAUNCH_NORMAL_ADDS,
            STUN_FIELD,
            STUN_RECOVERY,
            LASER_SWEEP_ATTACK,
            VACUUM_ATTACK,
            REPEL_ATTACK,
            DEATH_SEQUENCE,
            TRANSIT_SCENE,
            TOTAL
        }

        public BOSS_BEHAVIOUR_STATE current_state;

        //For ECS get transform
        Vector3 pos = new Vector3();
        Vector3 rot = new Vector3();
        Vector3 scale = new Vector3();

        //Entities
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

        //Main Laser
        uint main_laser_rails_id;
        uint main_laser_rail_vent_id;
        uint main_laser_barrel_id;
        uint main_laser_beam_id;
        uint main_laser_object_id;

        //Mouth
        uint left_mouth_id;
        uint right_mouth_id;

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
        
        //Shield
        uint false_core_id;

        //Laser
        float laser_spin_addition;
        const float laser_spin_addition_speed = 10.0f;

        //Eyes
        public uint left_eyeball_id;
        public uint right_eyeball_id;

        //Protection
        bool left_eye_color_one_active;
        bool right_eye_color_one_active;
        Vector3 protection_color_one;
        Vector3 protection_color_two;
        Vector3 default_color;
        float left_protection_count_timer;
        float right_protection_count_timer;
        const float eye_color_change_duration = 0.1f;
        public bool left_ball_protection;
        public bool right_ball_protection;

        //Color eyes
        uint cheek_lights_one_id;
        uint cheek_lights_two_id;
        uint cheek_lights_three_id;

        //Color turrets
        Vector3 turret_green_color;
        Vector3 turret_blue_color;
        Vector3 turret_emissive_blue_color;
        Vector3 turret_red_color;
        bool cheek_lights_color_one_active;
        const float cheek_color_change_duration = 0.15f;
        const int max_blink_counts = 6;
        int blink_counter;
        float cheek_lights_timer;

        //Player
        uint player_ship_id;

        uint end_screen_trigger_id;

        bool any_key_scale_up;
        uint any_key_continue_id;
        Vector3 any_key_max_scale;
        Vector3 any_key_min_scale;
        const float any_key_scale_speed = 5.0f;
        float fly_up_delay;
        const float fly_up_short_delay = 2.5f;

        AnimationSystem boss_animation_system;
        public bool left_arm_broken;
        public bool right_arm_broken;


        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            m_singleton = this;
        }

        public override void Start()
        {
            fly_up_delay = 7.0f;
            boss_animation_system = new AnimationSystem();
            boss_animation_system.Init();

            FindEntities();
            SetColors();
        }

        void SetColors()
        {
            protection_color_one = new Vector3(0.0f, 1.0f, 1.0f);
            protection_color_two = new Vector3(0.0f, 0.55f, 1.0f);
            default_color = new Vector3(1.0f, 1.0f, 1.0f);

            turret_green_color = new Vector3(0, 1.0f, 0);
            turret_blue_color = new Vector3(0, 0, 1.0f);
            turret_emissive_blue_color = new Vector3(0, 0.28f, 1.0f);
            turret_red_color = new Vector3(1.0f, 0, 0);
        }

        void FindEntities()
        {
            end_screen_trigger_id = ECS.FindEntityWithName("L1 Black Screen");
            any_key_continue_id = ECS.FindEntityWithName("Any Key Continue");

            //Player ship
            player_ship_id = ECS.FindEntityWithName("PlayerBox");

            //Core
            false_core_id = ECS.FindEntityWithName("FalseCore");
            left_mouth_id = ECS.FindEntityWithName("Mouth_L");
            right_mouth_id = ECS.FindEntityWithName("Mouth_R");

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
            right_leg_middle_joint_id = ECS.FindEntityWithName("Right_Leg_Middle_Joint");
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

            //Mini eye
            cheek_lights_one_id = ECS.FindEntityWithName("Cheeck_Lights_1");
            cheek_lights_two_id = ECS.FindEntityWithName("Cheeck_Lights_2");
            cheek_lights_three_id = ECS.FindEntityWithName("Cheeck_Lights_3");

            //Artillery
            artillery_axis_id = ECS.FindEntityWithName("Arti_PivotObj");
            artillery_barrel_id = ECS.FindEntityWithName("Arti_Barrel");

            //Main Laser
            main_laser_rails_id = ECS.FindEntityWithName("MainLaser_Rails");
            main_laser_rail_vent_id = ECS.FindEntityWithName("RailVent_Panels");
            main_laser_barrel_id = ECS.FindEntityWithName("MainLaser");
            main_laser_beam_id = ECS.FindEntityWithName("Laser_Beam");
            main_laser_object_id = ECS.FindEntityWithName("LaserObject");
            
            //Core
            false_core_id = ECS.FindEntityWithName("FalseCore");
            left_eyeball_id = ECS.FindEntityWithName("Left_Eye");
            right_eyeball_id = ECS.FindEntityWithName("Right_Eye");

            //Create and initialize the list of moving parts
            moving_parts_dict = new Dictionary<uint, MovingParts>();

            //Boss body
            moving_parts_dict.Add(entityID, new MovingParts() { entity_id = entityID });

            //Core
            moving_parts_dict.Add(false_core_id, new MovingParts() { entity_id = false_core_id });
            moving_parts_dict.Add(left_mouth_id, new MovingParts() { entity_id = left_mouth_id });
            moving_parts_dict.Add(right_mouth_id, new MovingParts() { entity_id = right_mouth_id });

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

            //Main Laser
            moving_parts_dict.Add(main_laser_rails_id, new MovingParts() { entity_id = main_laser_rails_id });
            moving_parts_dict.Add(main_laser_rail_vent_id, new MovingParts() { entity_id = main_laser_rail_vent_id });
            moving_parts_dict.Add(main_laser_barrel_id, new MovingParts() { entity_id = main_laser_barrel_id });
            moving_parts_dict.Add(main_laser_object_id, new MovingParts() { entity_id = main_laser_object_id });

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);

            any_key_scale_up = false;
            any_key_min_scale = new Vector3(0.45f, 0.45f, 1.0f);
            any_key_max_scale = new Vector3(0.5f, 0.5f, 1.0f);

            SetState(BOSS_BEHAVIOUR_STATE.FLYING_UP.ToString());
        }

        public override void Update(float dt)
        {
            moving_parts_dict[left_large_laser_spin_id].SpinObjectEndless(1.0f, 0, 0, 200.0f, dt);
            moving_parts_dict[right_large_laser_spin_id].SpinObjectEndless(1.0f, 0, 0, 200.0f, dt);

            /*if (InputUtility.onKeyTriggered(KEY_ID.KEY_J))
            {
                //SetState(BOSS_BEHAVIOUR_STATE.VACUUM_ATTACK.ToString());
                SetState(BOSS_BEHAVIOUR_STATE.REPEL_ATTACK.ToString());
                SetTurretColors();
                SetColorTurretPreparingFire();
            }
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_I))
            {
                //SetState(BOSS_BEHAVIOUR_STATE.VACUUM_ATTACK.ToString());
                SetState(BOSS_BEHAVIOUR_STATE.VACUUM_ATTACK.ToString());
                SetTurretColors();
                SetColorTurretPreparingFire();
            }
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_O))
            {
                //SetState(BOSS_BEHAVIOUR_STATE.VACUUM_ATTACK.ToString());
                SetState(BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK.ToString());
                SetTurretColors();
                SetColorTurretPreparingFire();
            }
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_K))
            {
                //SetState(BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK.ToString());
                SetColorTurretFiring();
            }
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_L))
            {
                ResetAllColors();
                SetColorTurretRecovery();
            }*/

            UpdateMouths(dt);

            switch (current_state)
            {
                case BOSS_BEHAVIOUR_STATE.MOVING:
                    RunMovingSequence(dt);
                    break;
                //case BOSS_BEHAVIOUR_STATE.PROTECTION:
                //    RunProtectionSequence(dt);
                //    break;
                case BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK:
                    UpdateCheeks(dt, turret_red_color);
                    break;
                case BOSS_BEHAVIOUR_STATE.REPEL_ATTACK:
                    UpdateCheeks(dt, turret_blue_color);
                    break;
                case BOSS_BEHAVIOUR_STATE.VACUUM_ATTACK:
                    UpdateCheeks(dt, turret_green_color);
                    break;
                case BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE:
                    laser_spin_addition += laser_spin_addition_speed * dt;
                    moving_parts_dict[main_laser_barrel_id].SpinObjectEndless(0, 0, 1.0f, laser_spin_addition, dt);
                    break;
                case BOSS_BEHAVIOUR_STATE.TRANSIT_SCENE:
                    UpdateAnyKeyScaling(dt);
                    break;
            }

            boss_animation_system.Update(dt);
        }

        public void SetState(string set_state)
        {
            current_state = (BOSS_BEHAVIOUR_STATE)Enum.Parse(typeof(BOSS_BEHAVIOUR_STATE), set_state);

            //Update animation sets based on state
            switch (current_state)
            {
                case BOSS_BEHAVIOUR_STATE.STUN_FIELD:
                    boss_animation_system.StopAnimation(true, moving_parts_dict);
                    boss_animation_system.AddAnimationSpecsStack(SetStunStateAnimationsOne, 1.5f);
                    boss_animation_system.AddAnimationSpecsStack(SetStunStateAnimationsTwo, 0.75f);
                    boss_animation_system.AddAnimationSpecsStack(SetStunStateAnimationsThree, 0.5f);
                    boss_animation_system.AddAnimationUpdateStack(RunStunStateAnimationsOne);
                    boss_animation_system.AddAnimationUpdateStack(RunStunStateAnimationsTwo);
                    boss_animation_system.AddAnimationUpdateStack(RunStunStateAnimationsThree);
                    boss_animation_system.PlayAnimation();
                    break;
                case BOSS_BEHAVIOUR_STATE.STUN_RECOVERY:
                    left_arm_broken = false;
                    right_arm_broken = false;
                    boss_animation_system.StopAnimation(true, moving_parts_dict);
                    boss_animation_system.AddAnimationSpecsStack(SetRecoveryStateAnimationsOne, 1.5f);
                    boss_animation_system.AddAnimationSpecsStack(SetRecoveryStateAnimationsTwo, 1.75f);
                    boss_animation_system.AddAnimationUpdateStack(RunRecoveryStateAnimationsOne);
                    boss_animation_system.AddAnimationUpdateStack(RunRecoveryStateAnimationsTwo);
                    boss_animation_system.SetStateQueue(SetState, BOSS_BEHAVIOUR_STATE.MOVING.ToString());
                    boss_animation_system.PlayAnimation();
                    break;
                case BOSS_BEHAVIOUR_STATE.FLYING_UP:
                    boss_animation_system.StopAnimation(true, moving_parts_dict);
                    boss_animation_system.AddAnimationSpecsStack(SetFlyingUpStateAnimationsOne, fly_up_delay);
                    boss_animation_system.AddAnimationSpecsStack(SetFlyingUpStateAnimationsTwo, 1.35f);
                    boss_animation_system.AddAnimationSpecsStack(SetFlyingUpStateAnimationsThree, 4.0f);
                    boss_animation_system.AddAnimationUpdateStack(RunFlyingUpSequenceOne);
                    boss_animation_system.AddAnimationUpdateStack(RunFlyingUpSequenceTwo);
                    boss_animation_system.AddAnimationUpdateStack(RunFlyingUpSequenceThree);
                    boss_animation_system.SetStateQueue(SetState, BOSS_BEHAVIOUR_STATE.MOVING.ToString());
                    boss_animation_system.PlayAnimation();
                    break;
                case BOSS_BEHAVIOUR_STATE.MOVING:
                    boss_animation_system.StopAnimation(true, moving_parts_dict);
                    SetMovingStateAnimations();
                    boss_animation_system.PlayAnimation();
                    break;
                //case BOSS_BEHAVIOUR_STATE.PROTECTION:
                //    boss_animation_system.StopAnimation(true, moving_parts_dict);
                //    SetProtectionStateAnimations();
                //    boss_animation_system.PlayAnimation();
                //    break;
                case BOSS_BEHAVIOUR_STATE.VACUUM_ATTACK:
                    ResetCheekVars();
                    boss_animation_system.StopAnimation(true, moving_parts_dict);
                    boss_animation_system.PlayAnimation();
                    break;
                case BOSS_BEHAVIOUR_STATE.REPEL_ATTACK:
                    ResetCheekVars();
                    boss_animation_system.StopAnimation(true, moving_parts_dict);
                    boss_animation_system.PlayAnimation();
                    break;
                case BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK:
                    ResetCheekVars();
                    boss_animation_system.StopAnimation(true, moving_parts_dict);
                    boss_animation_system.PlayAnimation();
                    break;
                case BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS:
                    boss_animation_system.StopAnimation(true, moving_parts_dict);
                    boss_animation_system.AddAnimationSpecsStack(SetLaunchStateAnimationsOne, 2.0f);
                    boss_animation_system.AddAnimationSpecsStack(SetLaunchStateAnimationsTwo, 2.0f);
                    boss_animation_system.AddAnimationSpecsStack(SetLaunchStateAnimationsThree, 3.5f);
                    boss_animation_system.AddAnimationSpecsStack(SetLaunchStateAnimationsFour, 2.0f);
                    boss_animation_system.AddAnimationSpecsStack(SetLaunchStateAnimationsFive, 0.5f);
                    boss_animation_system.AddAnimationUpdateStack(RunLaunchStateSequenceOne);
                    boss_animation_system.AddAnimationUpdateStack(RunLaunchStateSequenceTwo);
                    boss_animation_system.AddAnimationUpdateStack(RunLaunchStateSequenceThree);
                    boss_animation_system.AddAnimationUpdateStack(RunLaunchStateSequenceFour);
                    boss_animation_system.AddAnimationUpdateStack(RunLaunchStateSequenceFive);
                    boss_animation_system.SetStateQueue(SetState, BOSS_BEHAVIOUR_STATE.MOVING.ToString());
                    boss_animation_system.PlayAnimation();
                    break;
                case BOSS_BEHAVIOUR_STATE.TRANSIT_SCENE:
                    ECS.SetActive(any_key_continue_id, true);
                    break;
            }
        }

        void UpdateMouths(float dt)
        {
            if (left_ball_protection)
            {
                ECS.GetTransformECS(left_mouth_id, ref pos, ref rot, ref scale);

                //Pulse color
                if (left_protection_count_timer < eye_color_change_duration)
                {
                    left_protection_count_timer += dt;
                }
                else
                {
                    left_protection_count_timer = 0.0f;

                    if (left_eye_color_one_active)
                    {
                        ECS.SetDiffuseTint(left_eyeball_id, ref protection_color_one);
                        left_eye_color_one_active = false;
                    }
                    else
                    {
                        ECS.SetDiffuseTint(left_eyeball_id, ref protection_color_two);
                        left_eye_color_one_active = true;
                    }
                }

                if (rot.Y > 0.0f)
                {
                    ECS.SetRotation(left_mouth_id, Vector3.Lerp(rot, new Vector3(), 3.0f * dt));
                }
            }

            if (right_ball_protection)
            {
                ECS.GetTransformECS(right_mouth_id, ref pos, ref rot, ref scale);

                //Pulse color
                if (right_protection_count_timer < eye_color_change_duration)
                {
                    right_protection_count_timer += dt;
                }
                else
                {
                    right_protection_count_timer = 0.0f;

                    if (right_eye_color_one_active)
                    {
                        ECS.SetDiffuseTint(right_eyeball_id, ref protection_color_one);
                        right_eye_color_one_active = false;
                    }
                    else
                    {
                        ECS.SetDiffuseTint(right_eyeball_id, ref protection_color_two);
                        right_eye_color_one_active = true;
                    }
                }

                if (rot.Y < 0.0f)
                {
                    ECS.SetRotation(right_mouth_id, Vector3.Lerp(rot, new Vector3(), 3.0f * dt));
                }
            }
        }

        void ResetCheekVars()
        {
            cheek_lights_timer = 0.0f;
            blink_counter = 0;
        }

        void UpdateCheeks(float dt, Vector3 color_to_flash)
        {
            //Update cheek lights blinking, blink for x times
            if (blink_counter < max_blink_counts)
            {
                if (cheek_lights_timer < cheek_color_change_duration)
                {
                    cheek_lights_timer += dt;
                }
                else
                {   
                    ++blink_counter;
                    cheek_lights_timer = 0.0f;

                    if (cheek_lights_color_one_active)
                    {
                        ECS.SetDiffuseTint(cheek_lights_one_id, ref color_to_flash);
                        ECS.SetEmissiveTint(cheek_lights_one_id, ref color_to_flash);
                        ECS.SetDiffuseTint(cheek_lights_two_id, ref color_to_flash);
                        ECS.SetEmissiveTint(cheek_lights_two_id, ref color_to_flash);
                        ECS.SetDiffuseTint(cheek_lights_three_id, ref color_to_flash);
                        ECS.SetEmissiveTint(cheek_lights_three_id, ref color_to_flash);
                        cheek_lights_color_one_active = false;
                    }
                    else
                    {
                        ECS.SetDiffuseTint(cheek_lights_one_id, ref default_color);
                        ECS.SetEmissiveTint(cheek_lights_one_id, ref default_color);
                        ECS.SetDiffuseTint(cheek_lights_two_id, ref default_color);
                        ECS.SetEmissiveTint(cheek_lights_two_id, ref default_color);
                        ECS.SetDiffuseTint(cheek_lights_three_id, ref default_color);
                        ECS.SetEmissiveTint(cheek_lights_three_id, ref default_color);
                        cheek_lights_color_one_active = true;
                    }
                }
            }
        }

        void UpdateAnyKeyScaling(float dt)
        {
            ECS.GetTransformECS(any_key_continue_id, ref pos, ref rot, ref scale);

            if (scale.X >= any_key_max_scale.X - 0.01f)
            {
                any_key_scale_up = false;
            }
            if (scale.X <= any_key_min_scale.X + 0.01f)
            {
                any_key_scale_up = true;
            }

            if (any_key_scale_up)
            {
                ECS.SetScale(any_key_continue_id, Vector3.Lerp(scale, any_key_max_scale, any_key_scale_speed * dt));
            }
            else
            {
                ECS.SetScale(any_key_continue_id, Vector3.Lerp(scale, any_key_min_scale, any_key_scale_speed * dt));
            }

            if (InputUtility.onAnyKey())
            {
                GameUtilities.LoadScene("Level02");
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

        #region[Flying Up Animation Sequence]
        /// <summary>
        /// [Flying Up State] 
        /// 1. Empty delay
        /// 2. Rotate and fly up
        /// 3. Rotate back to axis
        /// </summary>

        void SetFlyingUpStateAnimationsOne()
        {
            ECS.SetGlobalPosition(entityID, new Vector3(0, -50, 0));
            moving_parts_dict[entityID].SetToggleSpin(false);
        }

        void SetFlyingUpStateAnimationsTwo()
        {
            moving_parts_dict[entityID].SetPingPongPosition(new Vector3(0, -50, 0), new Vector3(0, 5, 0), new Vector3(0, 4.0f, 0), false, true, false, false, false, false);
            ECS.SetRotation(entityID, new Vector3(-90, 0, 0));
            moving_parts_dict[entityID].SetPingPongRotation(new Vector3(-90, 0, 0), new Vector3(0, 0, 0), new Vector3(1.0f, 0, 0), true, false, false, false, false, false);
        }

        void SetFlyingUpStateAnimationsThree()
        {
            moving_parts_dict[entityID].SetPingPongPosition(new Vector3(0, 0, 0), new Vector3(0, 5, 0), new Vector3(0, 2.0f, 0), false, false, false, false, false, false);
            moving_parts_dict[entityID].SetPingPongRotation(new Vector3(ECS.GetGlobalRotation(entityID).X, 0, 0), new Vector3(0, 0, 0), new Vector3(2.0f, 0, 0), true, false, false, false, false, false);
        }

        void RunFlyingUpSequenceOne(float dt)
        {
            //Nothing as used for a delay
        }

        void RunFlyingUpSequenceTwo(float dt)
        {
            //Body
            moving_parts_dict[entityID].UpdateMovingParts(dt);
        }

        void RunFlyingUpSequenceThree(float dt)
        {
            //Body
            moving_parts_dict[entityID].UpdateMovingParts(dt);
        }

        #endregion

        #region[Moving Animation Sequence]

        /// <summary>
        /// [Moving State] Boss swaying around and flailing its arms
        /// </summary>
        void SetMovingStateAnimations()
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
            moving_parts_dict[left_mouth_id].SetPingPongRotation(new Vector3(), new Vector3(0, 45, 0), new Vector3(0, 2.0f, 0), false, true, false, false, true, false);
            moving_parts_dict[right_mouth_id].SetPingPongRotation(new Vector3(0, -45, 0), new Vector3(), new Vector3(0, 2.0f, 0), false, false, false, false, true, false);

            //Artillery
            moving_parts_dict[artillery_axis_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[artillery_barrel_id].SetPingPongPosition(new Vector3(), new Vector3(0, 10.3f, 0), new Vector3(10.0f, 10.0f, 10.0f), false, true, false, false, false, false);

            //Color turret
            moving_parts_dict[left_color_turret_1_id].SetLinearRotation(new Vector3(-70, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);
            moving_parts_dict[left_color_turret_2_id].SetLinearRotation(new Vector3(120, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);
            moving_parts_dict[left_color_turret_body_pivot_id].SetLinearRotation(new Vector3(-50, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);

            moving_parts_dict[right_color_turret_1_id].SetLinearRotation(new Vector3(-70, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);
            moving_parts_dict[right_color_turret_2_id].SetLinearRotation(new Vector3(120, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);
            moving_parts_dict[right_color_turret_body_pivot_id].SetLinearRotation(new Vector3(-50, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);
        }

        void RunMovingSequence(float dt)
        {
            //Set the boss in a moving state where the arms and legs flail a little bit
            moving_parts_dict[entityID].UpdateMovingParts(dt);

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
            if (!left_ball_protection)
            {
                moving_parts_dict[left_mouth_id].UpdateMovingParts(dt);
            }
            if (!right_ball_protection)
            {
                moving_parts_dict[right_mouth_id].UpdateMovingParts(dt);
            }
        }
        #endregion

        #region[Protection]

        ///// <summary>
        ///// [Protection State] Boss swaying around and flailing its arms
        ///// </summary>
        //void SetProtectionStateAnimations()
        //{
        //    //Body
        //    moving_parts_dict[entityID].SetPingPongPosition(new Vector3(-0.2f, 0, 0), new Vector3(0.2f, 0, 0), new Vector3(1.0f, 0, 0), true, false, false, true, false, false);
        //    moving_parts_dict[entityID].SetPingPongRotation(new Vector3(0, 0, -4), new Vector3(0, 0, 4), new Vector3(0, 0, 3.0f), false, false, true, false, false, true);

        //    //Arms
        //    moving_parts_dict[left_arm_middle_joint_id].SetPingPongRotation(new Vector3(-90, -10, 0), new Vector3(0, 10, 10), new Vector3(10.0f, 5.0f, 0), false, false, true, false, true, false);
        //    moving_parts_dict[left_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -85), new Vector3(0, 0, -65), new Vector3(0, 0, 5.0f), false, false, false, false, false, true);
        //    moving_parts_dict[right_arm_middle_joint_id].SetPingPongRotation(new Vector3(-90, -10, 0), new Vector3(0, 10, 10), new Vector3(10.0f, 5.0f, 0), false, false, true, false, true, false);
        //    moving_parts_dict[right_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 65), new Vector3(0, 0, 85), new Vector3(0.0f, 0.0f, 5.0f), false, false, false, false, false, true);

        //    //Legs
        //    moving_parts_dict[left_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90, 0, -35), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
        //    moving_parts_dict[left_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 50), new Vector3(0, 0, 100), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
        //    moving_parts_dict[right_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90, 0, 0), new Vector3(0, 0, 35), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
        //    moving_parts_dict[right_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -100), new Vector3(0, 0, -50), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);

        //    //Launching bays
        //    moving_parts_dict[right_launching_bay_one_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
        //    moving_parts_dict[right_launching_bay_two_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
        //    moving_parts_dict[right_launching_bay_three_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

        //    moving_parts_dict[left_launching_bay_one_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
        //    moving_parts_dict[left_launching_bay_two_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
        //    moving_parts_dict[left_launching_bay_three_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

        //    //Mouth
        //    moving_parts_dict[left_mouth_id].SetPingPongRotation(new Vector3(0, -45, 0), new Vector3(), new Vector3(0, 10.0f, 0), false, true, false, false, false, false);
        //    moving_parts_dict[right_mouth_id].SetPingPongRotation(new Vector3(), new Vector3(0, 45, 0), new Vector3(0, 10.0f, 0), false, false, false, false, false, false);

        //    //Artillery
        //    moving_parts_dict[artillery_axis_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
        //    moving_parts_dict[artillery_barrel_id].SetPingPongPosition(new Vector3(), new Vector3(0, 10.3f, 0), new Vector3(10.0f, 10.0f, 10.0f), false, true, false, false, false, false);

        //    moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
        //    moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);
        //}

        //void RunProtectionSequence(float dt)
        //{
        //    //Set the boss in a moving state where the arms and legs flail a little bit

        //    moving_parts_dict[entityID].UpdateMovingParts(dt);

        //    //Arms
        //    moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
        //    moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);
        //    moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
        //    moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);

        //    //Arm Lasers
        //    moving_parts_dict[left_large_laser_spin_id].UpdateMovingParts(dt);
        //    moving_parts_dict[right_large_laser_spin_id].UpdateMovingParts(dt);

        //    //Legs
        //    moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(dt);
        //    moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(dt);
        //    moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(dt);
        //    moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(dt);

        //    //Launching bays
        //    moving_parts_dict[right_launching_bay_one_id].UpdateMovingParts(dt);
        //    moving_parts_dict[right_launching_bay_two_id].UpdateMovingParts(dt);
        //    moving_parts_dict[right_launching_bay_three_id].UpdateMovingParts(dt);

        //    moving_parts_dict[left_launching_bay_one_id].UpdateMovingParts(dt);
        //    moving_parts_dict[left_launching_bay_two_id].UpdateMovingParts(dt);
        //    moving_parts_dict[left_launching_bay_three_id].UpdateMovingParts(dt);

        //    //Artillery
        //    moving_parts_dict[artillery_axis_id].UpdateMovingParts(dt);
        //    moving_parts_dict[artillery_barrel_id].UpdateMovingParts(dt);

        //    //Mouth
        //    moving_parts_dict[left_mouth_id].UpdateMovingParts(dt);
        //    moving_parts_dict[right_mouth_id].UpdateMovingParts(dt);
        //}
        public void EnableProtection()
        {
            left_ball_protection = true;
            right_ball_protection = true;

            boss_animation_system.ResetSingleAnimation(moving_parts_dict[left_mouth_id]);
            boss_animation_system.ResetSingleAnimation(moving_parts_dict[right_mouth_id]);
            boss_animation_system.PauseSingleAnimationPartRotation(moving_parts_dict[left_mouth_id]);
            boss_animation_system.PauseSingleAnimationPartRotation(moving_parts_dict[right_mouth_id]);
        }

        public void DamagedLeftBall()
        {
            left_ball_protection = false;

            //Change color of eye to default
            ECS.SetDiffuseTint(left_eyeball_id, ref default_color);
        }

        public void DamagedRightBall()
        {
            right_ball_protection = false;

            //Change color of eye to default
            ECS.SetDiffuseTint(right_eyeball_id, ref default_color);
        }
        #endregion

        #region[Launch Animation Sequence]

        /// <summary>
        /// [Launch Normal Enemy State]
        /// State 1: Arms rotate and arm ends tilt 90 degrees
        /// State 2: Boss tilts and arm rotate downwards
        /// State 3: Open launch bays and spawns enemies
        /// State 4: Launch bays close
        /// State 5: Arms and body rotate back
        /// </summary>

        void SetLaunchStateAnimationsOne()
        {
            //Body
            moving_parts_dict[entityID].SetPingPongPosition(new Vector3(), new Vector3(), new Vector3(1.0f, 0, 0), true, false, false, false, false, false);
            moving_parts_dict[entityID].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(0, 0, 3.0f), false, false, true, false, false, false);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetPingPongRotation(new Vector3(-160.0f, 0, -30.0f), new Vector3(), new Vector3(10.0f, 10.0f, 1.0f), false, false, false, false, false, false);
            moving_parts_dict[left_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -95.0f), new Vector3(0, 0, 0), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            moving_parts_dict[right_arm_middle_joint_id].SetPingPongRotation(new Vector3(-160.0f, 0, 0), new Vector3(0, 0, 30.0f), new Vector3(10.0f, 10.0f, 1.0f), false, false, true, false, false, false);
            moving_parts_dict[right_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 95.0f), new Vector3(0.0f, 0.0f, 5.0f), false, false, true, false, false, false);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90.0f, 0, -35.0f), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[left_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 50.0f), new Vector3(0, 0, 100.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90.0f, 0, 0), new Vector3(0, 0, 35.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -100.0f), new Vector3(0, 0, -50.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_two_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_three_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            moving_parts_dict[left_launching_bay_one_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_two_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_three_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            //Mouth
            moving_parts_dict[left_mouth_id].SetPingPongRotation(new Vector3(0, -15.0f, 0), new Vector3(), new Vector3(0, 10.0f, 0), false, true, false, false, true, false);
            moving_parts_dict[right_mouth_id].SetPingPongRotation(new Vector3(), new Vector3(0, 15.0f, 0), new Vector3(0, 10.0f, 0), false, false, false, false, true, false);

            //Artillery
            moving_parts_dict[artillery_axis_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[artillery_barrel_id].SetPingPongPosition(new Vector3(), new Vector3(0, 10.3f, 0), new Vector3(10.0f, 10.0f, 10.0f), false, true, false, false, false, false);
        }

        void RunLaunchStateSequenceOne(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);

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
            if (!left_ball_protection)
            {
                moving_parts_dict[left_mouth_id].UpdateMovingParts(dt);
            }
            if (!right_ball_protection)
            {
                moving_parts_dict[right_mouth_id].UpdateMovingParts(dt);
            }
        }

        void SetLaunchStateAnimationsTwo()
        {
            //Body
            moving_parts_dict[entityID].SetPingPongRotation(new Vector3(), new Vector3(18.0f, 0, 0), new Vector3(5.0f, 0, 0), true, false, false, false, false, false);
            moving_parts_dict[entityID].SetPingPongPosition(new Vector3(0, -0.1f, 0), new Vector3(), new Vector3(0, 3.0f, 0), false, false, false, false, false, false);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetPingPongRotation(new Vector3(), new Vector3(130.0f, 0, 0), new Vector3(5.0f, 0, 0), false, false, true, false, false, false);
            moving_parts_dict[left_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -95.0f), new Vector3(0, 0, 0), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            moving_parts_dict[right_arm_middle_joint_id].SetPingPongRotation(new Vector3(), new Vector3(130.0f, 0, 0), new Vector3(5.0f, 0, 0), false, false, true, false, false, false);
            moving_parts_dict[right_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 95.0f), new Vector3(0, 0, 5.0f), false, false, true, false, false, false);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90.0f, 0, -35.0f), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[left_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 50.0f), new Vector3(0, 0, 100.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90.0f, 0, 0), new Vector3(0, 0, 35.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -100.0f), new Vector3(0, 0, -50.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
        }

        void RunLaunchStateSequenceTwo(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(dt);
        }

        void SetLaunchStateAnimationsThree()
        {
            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].SetPingPongRotation(new Vector3(0, 0, -90.0f), new Vector3(), new Vector3(0, 0, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_two_id].SetPingPongRotation(new Vector3(0, 0, -90.0f), new Vector3(), new Vector3(0, 0, 6.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_three_id].SetPingPongRotation(new Vector3(0, 0, -90.0f), new Vector3(), new Vector3(0, 0, 2.0f), false, false, false, false, false, false);

            moving_parts_dict[left_launching_bay_one_id].SetPingPongRotation(new Vector3(), new Vector3(0, 0, 90.0f), new Vector3(0, 0, 10.0f), false, false, true, false, false, false);
            moving_parts_dict[left_launching_bay_two_id].SetPingPongRotation(new Vector3(), new Vector3(0, 0, 90.0f), new Vector3(0, 0, 6.0f), false, false, true, false, false, false);
            moving_parts_dict[left_launching_bay_three_id].SetPingPongRotation(new Vector3(), new Vector3(0, 0, 90.0f), new Vector3(0, 0, 2.0f), false, false, true, false, false, false);

            //Enable the launch particles
            ECS.SetParticlePlay(right_launching_bay_one_id, true);
            ECS.SetParticlePlay(right_launching_bay_two_id, true);
            ECS.SetParticlePlay(right_launching_bay_three_id, true);

            ECS.SetParticlePlay(left_launching_bay_one_id, true);
            ECS.SetParticlePlay(left_launching_bay_two_id, true);
            ECS.SetParticlePlay(left_launching_bay_three_id, true);
        }

        void RunLaunchStateSequenceThree(float dt)
        {
            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].UpdateMovingParts(dt);
            moving_parts_dict[right_launching_bay_two_id].UpdateMovingParts(dt);
            moving_parts_dict[right_launching_bay_three_id].UpdateMovingParts(dt);

            moving_parts_dict[left_launching_bay_one_id].UpdateMovingParts(dt);
            moving_parts_dict[left_launching_bay_two_id].UpdateMovingParts(dt);
            moving_parts_dict[left_launching_bay_three_id].UpdateMovingParts(dt);
        }

        void SetLaunchStateAnimationsFour()
        {
            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(0, 0, 10.0f), false, false, true, false, false, false);
            moving_parts_dict[right_launching_bay_two_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(0, 0, 6.0f), false, false, true, false, false, false);
            moving_parts_dict[right_launching_bay_three_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(0, 0, 2.0f), false, false, true, false, false, false);

            moving_parts_dict[left_launching_bay_one_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(0, 0, 10.0f), false, false, true, false, false, false);
            moving_parts_dict[left_launching_bay_two_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(0, 0, 6.0f), false, false, true, false, false, false);
            moving_parts_dict[left_launching_bay_three_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(0, 0, 2.0f), false, false, true, false, false, false);
        }

        void RunLaunchStateSequenceFour(float dt)
        {
            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].UpdateMovingParts(dt);
            moving_parts_dict[right_launching_bay_two_id].UpdateMovingParts(dt);
            moving_parts_dict[right_launching_bay_three_id].UpdateMovingParts(dt);

            moving_parts_dict[left_launching_bay_one_id].UpdateMovingParts(dt);
            moving_parts_dict[left_launching_bay_two_id].UpdateMovingParts(dt);
            moving_parts_dict[left_launching_bay_three_id].UpdateMovingParts(dt);
        }

        void SetLaunchStateAnimationsFive()
        {
            //Body
            moving_parts_dict[entityID].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(5.0f, 0, 0), false, false, false, false, false, false); ;
            moving_parts_dict[entityID].SetPingPongPosition(new Vector3(), new Vector3(0, 0.1f, 0), new Vector3(0, 3.0f, 0), false, true, false, false, false, false);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetPingPongRotation(new Vector3(), new Vector3(130.0f, 0, 0), new Vector3(5.0f, 0, 0), false, false, true, false, false, false);
            moving_parts_dict[left_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -95.0f), new Vector3(0, 0, 0), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            moving_parts_dict[right_arm_middle_joint_id].SetPingPongRotation(new Vector3(), new Vector3(130.0f, 0, 0), new Vector3(5.0f, 0, 0), false, false, true, false, false, false);
            moving_parts_dict[right_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 95.0f), new Vector3(0, 0, 5.0f), false, false, true, false, false, false);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90.0f, 0, -35.0f), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[left_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 50.0f), new Vector3(0, 0, 100.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90.0f, 0, 0), new Vector3(0, 0, 35.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -100.0f), new Vector3(0, 0, -50.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
        }

        void RunLaunchStateSequenceFive(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(dt);
        }
        #endregion

        #region[Spinning Animation Sequence]
        /// <summary>
        /// [Spinning State] 
        /// 1. Arms and legs stretch out
        /// 2. Slow spin
        /// 3. Mid spin
        /// 4. Fast spin
        /// 4. Move towards player
        /// </summary>

        void SetSpinningStateAnimationsOne()
        {
            //Body
            moving_parts_dict[entityID].SetPingPongPosition(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1.0f, 0, 0), true, false, false, true, false, false);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetPingPongRotation(new Vector3(0, -90.0f, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, false, true, false, false, false);
            moving_parts_dict[left_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), false, false, false, false, false, false);
            moving_parts_dict[right_arm_middle_joint_id].SetPingPongRotation(new Vector3(0, 0, 0), new Vector3(0, 90.0f, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true, false, false, false);
            moving_parts_dict[right_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), false, false, false, false, false, false);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetPingPongRotation(new Vector3(0, 0, 0), new Vector3(0, 90, 0), new Vector3(5.0f, 5.0f, 5.0f), false, true, false, false, false, false);
            moving_parts_dict[left_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), false, false, true, false, false, true);
            moving_parts_dict[right_leg_middle_joint_id].SetPingPongRotation(new Vector3(0, -90, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), false, false, false, false, false, false);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_one_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_three_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            moving_parts_dict[left_launching_bay_one_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_two_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_three_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            //Mouth
            moving_parts_dict[left_mouth_id].SetPingPongRotation(new Vector3(0, 0, 0), new Vector3(), new Vector3(0, 10.0f, 0), false, true, false, false, true, false);
            moving_parts_dict[right_mouth_id].SetPingPongRotation(new Vector3(), new Vector3(0, 0, 0), new Vector3(0, 10.0f, 0), false, false, false, false, true, false);

            //Artillery
            moving_parts_dict[artillery_axis_id].SetPingPongRotation(new Vector3(), new Vector3(61, 0, 0), new Vector3(10.0f, 0, 0), true, false, false, false, false, false);
            moving_parts_dict[artillery_barrel_id].SetPingPongPosition(new Vector3(), new Vector3(0, 6.0f, 0), new Vector3(0, 5.0f, 0), false, true, false, false, false, false);

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);
        }

        void RunSpinningStateAnimationsOne(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(dt);

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
            if (!left_ball_protection)
            {
                moving_parts_dict[left_mouth_id].UpdateMovingParts(dt);
            }
            if (!right_ball_protection)
            {
                moving_parts_dict[right_mouth_id].UpdateMovingParts(dt);
            }
        }

        void SetSpinningStateAnimationsTwo()
        {
            moving_parts_dict[entityID].SetToggleSpin(true);
        }

        void RunSpinningStateAnimationsTwo(float dt)
        {
            moving_parts_dict[entityID].SpinObjectEndless(0, 0, 1.0f, 200.0f, dt);
        }

        void SetSpinningStateAnimationsThree()
        {

        }

        void RunSpinningStateAnimationsThree(float dt)
        {
            moving_parts_dict[entityID].SpinObjectEndless(0, 0, 1.0f, 400.0f, dt);
        }

        void SetSpinningStateAnimationsFour()
        {

        }

        void RunSpinningStateAnimationsFour(float dt)
        {
            moving_parts_dict[entityID].SpinObjectEndless(0, 0, 1.0f, 800.0f, dt);
        }

        void SetSpinningStateAnimationsFive()
        {
            //Body
            moving_parts_dict[entityID].SetPingPongPosition(new Vector3(0, 0, 0), new Vector3(0, 0, 40), new Vector3(0, 0, 1.25f), false, false, true, false, false, false);
        }

        void RunSpinningStateAnimationsFive(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(dt);
            moving_parts_dict[entityID].SpinObjectEndless(0, 0, 1.0f, 1000.0f, dt);
        }

        #endregion

        #region[Stun Field Animation Sequence]
        /// <summary>
        /// [Stun Field State] 
        /// 1. Arms and legs curls back
        /// 2. Delay
        /// 2. Charge forward towards player
        /// 3. Unleash stun field from arms
        /// </summary>

        void SetStunStateAnimationsOne()
        {
            //Body
            moving_parts_dict[entityID].SetLinearPosition(new Vector3(0, 0, -1.0f), new Vector3(0, 0, 1.0f), false, false, false);
            moving_parts_dict[entityID].SetLinearRotation(new Vector3(-8, 0, 0), new Vector3(3.0f, 0, 0), true, false, false);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetLinearRotation(new Vector3(-99, 35, 0), new Vector3(10.0f, 5.0f, 5.0f), true, true, true);
            moving_parts_dict[left_arm_end_joint_id].SetLinearRotation(new Vector3(0, 0, -111), new Vector3(0, 0, 5.0f), false, false, true);
            moving_parts_dict[right_arm_middle_joint_id].SetLinearRotation(new Vector3(-99, -35, 0), new Vector3(10.0f, 5.0f, 5.0f), true, true, true);
            moving_parts_dict[right_arm_end_joint_id].SetLinearRotation(new Vector3(0, 0, 111), new Vector3(0.0f, 0.0f, 5.0f), false, false, true);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetLinearRotation(new Vector3(-99, 35, 0), new Vector3(10.0f, 5.0f, 0), true, true, false);
            moving_parts_dict[left_leg_end_joint_id].SetLinearRotation(new Vector3(0, 0, -111), new Vector3(0, 0, 5.0f), false, false, true);
            moving_parts_dict[right_leg_middle_joint_id].SetLinearRotation(new Vector3(-99, -35, 0), new Vector3(10.0f, 5.0f, 0), true, true, false);
            moving_parts_dict[right_leg_end_joint_id].SetLinearRotation(new Vector3(0, 0, 111), new Vector3(0.0f, 0.0f, 5.0f), false, false, true);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].SetLinearRotation(new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);
            moving_parts_dict[right_launching_bay_one_id].SetLinearRotation(new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);
            moving_parts_dict[right_launching_bay_three_id].SetLinearRotation(new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);

            moving_parts_dict[left_launching_bay_one_id].SetLinearRotation(new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);
            moving_parts_dict[left_launching_bay_two_id].SetLinearRotation(new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);
            moving_parts_dict[left_launching_bay_three_id].SetLinearRotation(new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);

            //Color turrets
            moving_parts_dict[right_color_turret_1_id].SetLinearRotation(new Vector3(-100.0f, 0, 0), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);
            moving_parts_dict[right_color_turret_2_id].SetLinearRotation(new Vector3(120, 0, 0), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);
            moving_parts_dict[right_color_turret_body_pivot_id].SetLinearRotation(new Vector3(-50, 0, 0), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);

            moving_parts_dict[left_color_turret_1_id].SetLinearRotation(new Vector3(-100.0f, 0, 0), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);
            moving_parts_dict[left_color_turret_2_id].SetLinearRotation(new Vector3(120, 0, 0), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);
            moving_parts_dict[left_color_turret_body_pivot_id].SetLinearRotation(new Vector3(-50, 0 ,0), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);

            //Mouth
            moving_parts_dict[left_mouth_id].SetLinearRotation(new Vector3(), new Vector3(2.0f, 2.0f, 2.0f), true, true, true);
            moving_parts_dict[right_mouth_id].SetLinearRotation(new Vector3(), new Vector3(2.0f, 2.0f, 2.0f), true, true, true);

            //Artillery
            moving_parts_dict[artillery_axis_id].SetLinearRotation(new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);
            moving_parts_dict[artillery_barrel_id].SetLinearRotation(new Vector3(0, 10.3f, 0), new Vector3(0, 10.0f, 0), false, true, false);

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);
        }

        void RunStunStateAnimationsOne(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(dt);

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

            //Color turrets
            moving_parts_dict[right_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[right_color_turret_2_id].UpdateMovingParts(dt);
            moving_parts_dict[right_color_turret_body_pivot_id].UpdateMovingParts(dt);

            moving_parts_dict[left_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_2_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_body_pivot_id].UpdateMovingParts(dt);

            //Artillery
            moving_parts_dict[artillery_axis_id].UpdateMovingParts(dt);
            moving_parts_dict[artillery_barrel_id].UpdateMovingParts(dt);

            //Mouth
            if (!left_ball_protection)
            {
                moving_parts_dict[left_mouth_id].UpdateMovingParts(dt);
            }
            if (!right_ball_protection)
            {
                moving_parts_dict[right_mouth_id].UpdateMovingParts(dt);
            }
        }

        void SetStunStateAnimationsTwo()
        {
            
            moving_parts_dict[entityID].SetPingPongPosition(new Vector3(0, -0.2f, 0), new Vector3(0, 0.2f, 0), new Vector3(0, 25.0f, 0), false, true, false, false, true, false);
        }

        void RunStunStateAnimationsTwo(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(dt);
        }

        void SetStunStateAnimationsThree()
        {
            moving_parts_dict[entityID].SetPingPongPosition(new Vector3(0, -0.2f, 0), new Vector3(0, 0.2f, 0), new Vector3(0, 0, 0), false, true, false, false, true, false);
            //Body
            moving_parts_dict[entityID].SetLinearRotation(new Vector3(0, 0, 0), new Vector3(3.0f, 0, 0), true, false, false);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetLinearRotation(new Vector3(-99, -65, 0), new Vector3(15.0f, 10.0f, 0), true, true, false);
            moving_parts_dict[left_arm_end_joint_id].SetLinearRotation(new Vector3(0, 0, -95), new Vector3(0, 0, 10.0f), false, false, true);
            moving_parts_dict[right_arm_middle_joint_id].SetLinearRotation(new Vector3(-99, 65, 0), new Vector3(15.0f, 10.0f, 0), true, true, false);
            moving_parts_dict[right_arm_end_joint_id].SetLinearRotation(new Vector3(0, 0, 95), new Vector3(0.0f, 0.0f, 10.0f), false, false, true);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetLinearRotation(new Vector3(-99, 35, 0), new Vector3(10.0f, 5.0f, 0), true, true, false);
            moving_parts_dict[left_leg_end_joint_id].SetLinearRotation(new Vector3(0, 0, 65), new Vector3(0, 0, 5.0f), false, false, true);
            moving_parts_dict[right_leg_middle_joint_id].SetLinearRotation(new Vector3(-99, -35, 0), new Vector3(10.0f, 5.0f, 0), true, true, false);
            moving_parts_dict[right_leg_end_joint_id].SetLinearRotation(new Vector3(0, 0, 65), new Vector3(0.0f, 0.0f, 5.0f), false, false, true);

            //Color turrets
            moving_parts_dict[right_color_turret_1_id].SetLinearRotation(new Vector3(47, 20, 10), new Vector3(15.0f, 15.0f, 15.0f), true, true, true);
            moving_parts_dict[right_color_turret_2_id].SetLinearRotation(new Vector3(45, 0, 0), new Vector3(15.0f, 15.0f, 15.0f), true, true, true);
            moving_parts_dict[right_color_turret_body_pivot_id].SetLinearRotation(new Vector3(-43, 0, 0), new Vector3(15.0f, 15.0f, 15.0f), true, true, true);

            moving_parts_dict[left_color_turret_1_id].SetLinearRotation(new Vector3(47, -20, -10), new Vector3(15.0f, 15.0f, 15.0f), true, true, true);
            moving_parts_dict[left_color_turret_2_id].SetLinearRotation(new Vector3(45, 0, 0), new Vector3(15.0f, 15.0f, 15.0f), true, true, true);
            moving_parts_dict[left_color_turret_body_pivot_id].SetLinearRotation(new Vector3(-43, 0, 0), new Vector3(15.0f, 15.0f, 15.0f), true, true, true);
        }

        void RunStunStateAnimationsThree(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(dt);

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

            //Color turrets
            moving_parts_dict[right_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[right_color_turret_2_id].UpdateMovingParts(dt);
            moving_parts_dict[right_color_turret_body_pivot_id].UpdateMovingParts(dt);

            moving_parts_dict[left_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_2_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_body_pivot_id].UpdateMovingParts(dt);

            //Artillery
            moving_parts_dict[artillery_axis_id].UpdateMovingParts(dt);
            moving_parts_dict[artillery_barrel_id].UpdateMovingParts(dt);

            //Mouth
            moving_parts_dict[left_mouth_id].UpdateMovingParts(dt);
            moving_parts_dict[right_mouth_id].UpdateMovingParts(dt);
        }

        void SetStunStateAnimationsFour()
        {

        }

        void RunStunStateAnimationsFour(float dt)
        {
            
        }

        #endregion

        #region[Recovery Animation Sequence]
        /// <summary>
        /// [Recovery State] 
        /// 1. Left & Right arm and color turret back off
        /// 2. Both arms retract
        /// </summary>
       
        void SetRecoveryStateAnimationsOne()
        {
            //Arms
            moving_parts_dict[right_arm_middle_joint_id].SetLinearRotation(new Vector3(-82, 30, 55), new Vector3(10.0f, 5.0f, 5.0f), true, true, true);
            moving_parts_dict[right_arm_end_joint_id].SetLinearRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 5.0f), false, false, true);
            moving_parts_dict[left_arm_middle_joint_id].SetLinearRotation(new Vector3(-82, -30, -55), new Vector3(10.0f, 5.0f, 5.0f), true, true, true);
            moving_parts_dict[left_arm_end_joint_id].SetLinearRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 5.0f), false, false, true);

            //Color turret
            moving_parts_dict[right_color_turret_1_id].SetLinearRotation(new Vector3(35, -12, 31), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);
            moving_parts_dict[left_color_turret_1_id].SetLinearRotation(new Vector3(35, 12, -31), new Vector3(10.0f, 10.0f, 10.0f), true, true, true);
        }

        void RunRecoveryStateAnimationsOne(float dt)
        {
            //Arms
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);

            //Color turret
            moving_parts_dict[right_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_1_id].UpdateMovingParts(dt);
        }

        void SetRecoveryStateAnimationsTwo()
        {
            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetLinearRotation(new Vector3(-82, 30, 0), new Vector3(5.0f, 2.5f, 2.5f), true, true, true);
            moving_parts_dict[left_arm_end_joint_id].SetLinearRotation(new Vector3(0, 0, -125), new Vector3(0, 0, 2.5f), false, false, true);

           
            moving_parts_dict[right_arm_middle_joint_id].SetLinearRotation(new Vector3(-82, -30, 0), new Vector3(5.0f, 2.5f, 2.5f), true, true, true);
            moving_parts_dict[right_arm_end_joint_id].SetLinearRotation(new Vector3(0, 0, 125), new Vector3(0, 0, 2.5f), false, false, true);

            //Color turret
            moving_parts_dict[right_color_turret_1_id].SetLinearRotation(new Vector3(-45, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);
            moving_parts_dict[left_color_turret_1_id].SetLinearRotation(new Vector3(-45, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);
        }

        void RunRecoveryStateAnimationsTwo(float dt)
        {
            //Arms
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);

            //Color turret
            moving_parts_dict[right_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_1_id].UpdateMovingParts(dt);
        }

        #endregion

        #region[Color Turret Attack Animation Sequence]
        public void SetTurretColors()
        {
            ResetCheekVars();
            switch (current_state)
            {
                case BOSS_BEHAVIOUR_STATE.REPEL_ATTACK:
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
                case BOSS_BEHAVIOUR_STATE.VACUUM_ATTACK:
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
                    break;
                case BOSS_BEHAVIOUR_STATE.LASER_SWEEP_ATTACK:
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
                    break;
            }

        }

        public void ResetAllColors()
        {
            //Reset cheek and turret colors
            ECS.SetDiffuseTint(cheek_lights_one_id, ref default_color);
            ECS.SetDiffuseTint(cheek_lights_two_id, ref default_color);
            ECS.SetDiffuseTint(cheek_lights_three_id, ref default_color);

            ECS.SetDiffuseTint(right_color_turret_light_1_id, ref default_color);
            ECS.SetEmissiveTint(right_color_turret_light_1_id, ref default_color);
            ECS.SetDiffuseTint(right_color_turret_light_2_id, ref default_color);
            ECS.SetEmissiveTint(right_color_turret_light_2_id, ref default_color);
            ECS.SetDiffuseTint(right_color_turret_light_3_id, ref default_color);
            ECS.SetEmissiveTint(right_color_turret_light_3_id, ref default_color);
            ECS.SetDiffuseTint(right_color_turret_light_toggle_id, ref default_color);

            ECS.SetDiffuseTint(left_color_turret_light_1_id, ref default_color);
            ECS.SetEmissiveTint(left_color_turret_light_1_id, ref default_color);
            ECS.SetDiffuseTint(left_color_turret_light_2_id, ref default_color);
            ECS.SetEmissiveTint(left_color_turret_light_2_id, ref default_color);
            ECS.SetDiffuseTint(left_color_turret_light_3_id, ref default_color);
            ECS.SetEmissiveTint(left_color_turret_light_3_id, ref default_color);
            ECS.SetDiffuseTint(left_color_turret_light_toggle_id, ref default_color);
        }

        public void SetColorTurretPreparingFire()
        {
            boss_animation_system.StopAnimation(true, moving_parts_dict);
            boss_animation_system.AddAnimationSpecsStack(SetColorTurretAttackStateAnimationsOne, 2.0f);
            boss_animation_system.AddAnimationUpdateStack(RunColorTurretAttackStateAnimationsOne);
            boss_animation_system.PlayAnimation();
        }

        public void SetColorTurretFiring()
        {
            boss_animation_system.StopAnimation(true, moving_parts_dict);
            boss_animation_system.AddAnimationSpecsStack(SetColorTurretAttackStateAnimationsTwo, 0.3f);
            boss_animation_system.AddAnimationSpecsStack(SetColorTurretAttackStateAnimationsThree, 0.3f);
            boss_animation_system.AddAnimationUpdateStack(RunColorTurretAttackStateAnimationsTwo);
            boss_animation_system.AddAnimationUpdateStack(RunColorTurretAttackStateAnimationsThree);
            boss_animation_system.PlayAnimation();
        }

        public void SetColorTurretRecovery()
        {
            boss_animation_system.StopAnimation(true, moving_parts_dict);
            boss_animation_system.AddAnimationSpecsStack(SetColorTurretAttackStateAnimationsFour, 2.0f);
            boss_animation_system.AddAnimationUpdateStack(RunRepelAttackStateAnimationsFour);
            boss_animation_system.PlayAnimation();
        }


        /// <summary>
        /// [Repel State] 
        /// 1. Load
        /// 2. Shoot
        /// 3. Recoil
        /// 4. Unload
        /// </summary>

        void SetColorTurretAttackStateAnimationsOne()
        {
            moving_parts_dict[left_color_turret_tube_1_id].SetLinearPosition(new Vector3(0, -0.65f, 0), new Vector3(0, 2.0f, 0), false, true, false);
            moving_parts_dict[left_color_turret_tube_2_id].SetLinearPosition(new Vector3(0, -0.65f, 0), new Vector3(0, 2.0f, 0), false, true, false);
            moving_parts_dict[left_color_turret_tube_3_id].SetLinearPosition(new Vector3(0, -0.65f, 0), new Vector3(0, 2.0f, 0), false, true, false);

            moving_parts_dict[right_color_turret_tube_1_id].SetLinearPosition(new Vector3(0, -0.65f, 0), new Vector3(0, 2.0f, 0), false, true, false);
            moving_parts_dict[right_color_turret_tube_2_id].SetLinearPosition(new Vector3(0, -0.65f, 0), new Vector3(0, 2.0f, 0), false, true, false);
            moving_parts_dict[right_color_turret_tube_3_id].SetLinearPosition(new Vector3(0, -0.65f, 0), new Vector3(0, 2.0f, 0), false, true, false);
        }

        void RunColorTurretAttackStateAnimationsOne(float dt)
        {
            moving_parts_dict[left_color_turret_tube_1_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_tube_2_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_tube_3_id].UpdateMovingParts(dt);

            moving_parts_dict[right_color_turret_tube_1_id].UpdateMovingParts(dt);
            moving_parts_dict[right_color_turret_tube_2_id].UpdateMovingParts(dt);
            moving_parts_dict[right_color_turret_tube_3_id].UpdateMovingParts(dt);
        }

        void SetColorTurretAttackStateAnimationsTwo()
        {
            moving_parts_dict[left_color_turret_1_id].SetLinearRotation(new Vector3(-75.0f, 0, 0), new Vector3(10.0f, 0, 0), true, false, false);
            moving_parts_dict[left_color_turret_2_id].SetLinearRotation(new Vector3(112.0f, 0, 0), new Vector3(10.0f, 0, 0), true, false, false);

            moving_parts_dict[right_color_turret_1_id].SetLinearRotation(new Vector3(-75.0f, 0, 0), new Vector3(10.0f, 0, 0), true, false, false);
            moving_parts_dict[right_color_turret_2_id].SetLinearRotation(new Vector3(112.0f, 0, 0), new Vector3(10.0f, 0, 0), true, false, false);
        }

        void RunColorTurretAttackStateAnimationsTwo(float dt)
        {
            moving_parts_dict[left_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_2_id].UpdateMovingParts(dt);

            moving_parts_dict[right_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[right_color_turret_2_id].UpdateMovingParts(dt);
        }

        void SetColorTurretAttackStateAnimationsThree()
        {
            moving_parts_dict[left_color_turret_1_id].SetLinearRotation(new Vector3(-70, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);
            moving_parts_dict[left_color_turret_2_id].SetLinearRotation(new Vector3(120, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);

            moving_parts_dict[right_color_turret_1_id].SetLinearRotation(new Vector3(-70, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);
            moving_parts_dict[right_color_turret_2_id].SetLinearRotation(new Vector3(120, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);
        }

        void RunColorTurretAttackStateAnimationsThree(float dt)
        {
            moving_parts_dict[left_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_2_id].UpdateMovingParts(dt);

            moving_parts_dict[right_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[right_color_turret_2_id].UpdateMovingParts(dt);
        }

        void SetColorTurretAttackStateAnimationsFour()
        {
            ECS.SetDiffuseTint(right_color_turret_light_1_id, ref default_color);
            ECS.SetDiffuseTint(right_color_turret_light_2_id, ref default_color);
            ECS.SetDiffuseTint(right_color_turret_light_3_id, ref default_color);
            ECS.SetDiffuseTint(right_color_turret_light_toggle_id, ref default_color);

            ECS.SetDiffuseTint(left_color_turret_light_1_id, ref default_color);
            ECS.SetDiffuseTint(left_color_turret_light_2_id, ref default_color);
            ECS.SetDiffuseTint(left_color_turret_light_3_id, ref default_color);
            ECS.SetDiffuseTint(left_color_turret_light_toggle_id, ref default_color);

            moving_parts_dict[left_color_turret_tube_1_id].SetLinearPosition(new Vector3(0, 0.58f, 0), new Vector3(0, 2.0f, 0), false, true, false);
            moving_parts_dict[left_color_turret_tube_2_id].SetLinearPosition(new Vector3(0, 0.58f, 0), new Vector3(0, 2.0f, 0), false, true, false);
            moving_parts_dict[left_color_turret_tube_3_id].SetLinearPosition(new Vector3(0, 0.58f, 0), new Vector3(0, 2.0f, 0), false, true, false);

            moving_parts_dict[right_color_turret_tube_1_id].SetLinearPosition(new Vector3(0, 0.58f, 0), new Vector3(0, 2.0f, 0), false, true, false);
            moving_parts_dict[right_color_turret_tube_2_id].SetLinearPosition(new Vector3(0, 0.58f, 0), new Vector3(0, 2.0f, 0), false, true, false);
            moving_parts_dict[right_color_turret_tube_3_id].SetLinearPosition(new Vector3(0, 0.58f, 0), new Vector3(0, 2.0f, 0), false, true, false);

            ECS.SetDiffuseTint(cheek_lights_one_id, ref default_color);
            ECS.SetDiffuseTint(cheek_lights_two_id, ref default_color);
            ECS.SetDiffuseTint(cheek_lights_three_id, ref default_color);
        }

        void RunRepelAttackStateAnimationsFour(float dt)
        {
            moving_parts_dict[left_color_turret_tube_1_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_tube_2_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_tube_3_id].UpdateMovingParts(dt);

            moving_parts_dict[right_color_turret_tube_1_id].UpdateMovingParts(dt);
            moving_parts_dict[right_color_turret_tube_2_id].UpdateMovingParts(dt);
            moving_parts_dict[right_color_turret_tube_3_id].UpdateMovingParts(dt);
        }
        #endregion
    }
}
