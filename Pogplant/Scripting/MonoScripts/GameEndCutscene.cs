using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class GameEndCutscene : MonoBehaviour
    {
        Dictionary<uint, MovingParts> moving_parts_dict;
        Dictionary<uint, MovingParts> explosion_moving_parts_dict;

        public enum BOSS_ANIM_STATE
        {
            FLYING_DOWN,
            CRASHING
        }

        BOSS_ANIM_STATE current_state;

        //For ECS get transform
        Vector3 pos = new Vector3();
        Vector3 rot = new Vector3();
        Vector3 scale = new Vector3();

        //Entities
        public uint boss_model_parent_id;

        //Arms
        public uint left_arm_middle_joint_id;
        public uint left_arm_end_joint_id;
        public uint right_arm_middle_joint_id;
        public uint right_arm_end_joint_id;

        //Arm lasers
        public uint left_large_laser_spin_id;
        public uint right_large_laser_spin_id;

        //Legs
        public uint left_leg_middle_joint_id;
        public uint right_leg_middle_joint_id;
        public uint left_leg_end_joint_id;
        public uint right_leg_end_joint_id;

        //Launching bays
        public uint right_launching_bay_one_id;
        public uint right_launching_bay_two_id;
        public uint right_launching_bay_three_id;

        public uint left_launching_bay_one_id;
        public uint left_launching_bay_two_id;
        public uint left_launching_bay_three_id;

        //Artillery
        public uint artillery_axis_id;
        public uint artillery_barrel_id;

        //Mouth
        public uint mouth_left_id;
        public uint mouth_right_id;

        //Color turret
        public uint right_color_turret_1_id;
        public uint right_color_turret_2_id;
        public uint right_color_turret_tube_1_id;
        public uint right_color_turret_tube_2_id;
        public uint right_color_turret_tube_3_id;
        public uint right_color_turret_light_1_id;
        public uint right_color_turret_light_2_id;
        public uint right_color_turret_light_3_id;
        public uint right_color_turret_light_toggle_id;
        public uint right_color_turret_body_pivot_id;

        public uint left_color_turret_1_id;
        public uint left_color_turret_2_id;
        public uint left_color_turret_tube_1_id;
        public uint left_color_turret_tube_2_id;
        public uint left_color_turret_tube_3_id;
        public uint left_color_turret_light_1_id;
        public uint left_color_turret_light_2_id;
        public uint left_color_turret_light_3_id;
        public uint left_color_turret_light_toggle_id;
        public uint left_color_turret_body_pivot_id;

        //Cheeks
        uint cheek_lights_one_id;
        uint cheek_lights_two_id;
        uint cheek_lights_three_id;

        //Explosion & smokeeffects
        uint explosion_vfx_one_id;
        uint explosion_vfx_two_id;
        uint explosion_vfx_three_id;
        uint explosion_vfx_four_id;
        uint smoke_id;
        bool is_exploding;
        float explosion_loop_timer;
        const float explosion_speed = 5.0f;
        const float explosion_max_loop_duration = 6.0f;
        const float explosion_max_scale = 1.2f;

        uint camera_id;
        uint cinematic_bar_top_id;
        uint cinematic_bar_bottom_id;
        bool cinematic_cover_screen;
        const float cinematic_bar_speed = 3.0f;
        //float yaw, pitch, roll;

        float flash_timer;
        const float flash_duration = 0.05f;
        bool is_flash_color_on;
        Vector3 red_color;
        Vector3 black_color;

        AnimationSystem boss_anim_system;
        AnimationSystem explosion_anim_system;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        public override void Start()
        {
            ECS.PlayAudio(entityID, 0, "SFX");

            boss_anim_system = new AnimationSystem();
            boss_anim_system.Init();

            explosion_anim_system = new AnimationSystem();
            explosion_anim_system.Init();

            FindEntities();

            red_color = new Vector3(1.0f, 0, 0);
            black_color = new Vector3();
            explosion_loop_timer = 0.0f;
            is_exploding = true;
        }

        void FindEntities()
        {
            cinematic_bar_top_id = ECS.FindEntityWithName("Top Cinematic Bar");
            cinematic_bar_bottom_id = ECS.FindEntityWithName("Bottom Cinematic Bar");
            camera_id = ECS.FindEntityWithName("Cinematic Camera");

            //Main body
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

            //Cheeks
            cheek_lights_one_id = ECS.FindEntityWithName("Cheeck_Lights_1");
            cheek_lights_two_id = ECS.FindEntityWithName("Cheeck_Lights_2");
            cheek_lights_three_id = ECS.FindEntityWithName("Cheeck_Lights_3");

            //Artillery
            artillery_axis_id = ECS.FindEntityWithName("Arti_Axis");
            artillery_barrel_id = ECS.FindEntityWithName("Arti_Barrel");

            //Explosion VFX
            explosion_vfx_one_id = ECS.FindEntityWithName("Explosion_VFX_1");
            explosion_vfx_two_id = ECS.FindEntityWithName("Explosion_VFX_2");
            explosion_vfx_three_id = ECS.FindEntityWithName("Explosion_VFX_3");
            explosion_vfx_four_id = ECS.FindEntityWithName("Explosion_VFX_4");
            smoke_id = ECS.FindEntityWithName("Smoke_VFX");
            ECS.SetParticlePlay(smoke_id, false);

            //Create and initialize the list of moving parts
            moving_parts_dict = new Dictionary<uint, MovingParts>();
            explosion_moving_parts_dict = new Dictionary<uint, MovingParts>();

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

            explosion_moving_parts_dict.Add(explosion_vfx_one_id, new MovingParts() { entity_id = explosion_vfx_one_id });
            explosion_moving_parts_dict.Add(explosion_vfx_two_id, new MovingParts() { entity_id = explosion_vfx_two_id });
            explosion_moving_parts_dict.Add(explosion_vfx_three_id, new MovingParts() { entity_id = explosion_vfx_three_id });
            explosion_moving_parts_dict.Add(explosion_vfx_four_id, new MovingParts() { entity_id = explosion_vfx_four_id });

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);

            SetState(BOSS_ANIM_STATE.FLYING_DOWN.ToString());

            explosion_anim_system.AddAnimationSpecsStack(SetExplodingAnimationsOne, 0.2f);
            explosion_anim_system.AddAnimationSpecsStack(SetExplodingAnimationsTwo, 0.3f);
            explosion_anim_system.AddAnimationSpecsStack(SetExplodingAnimationsThree, 0.2f);
            explosion_anim_system.AddAnimationSpecsStack(SetExplodingAnimationsFour, 0.4f);
            explosion_anim_system.AddAnimationUpdateStack(RunExplodingAnimations);
            explosion_anim_system.AddAnimationUpdateStack(RunExplodingAnimations);
            explosion_anim_system.AddAnimationUpdateStack(RunExplodingAnimations);
            explosion_anim_system.AddAnimationUpdateStack(RunExplodingAnimations);
            explosion_anim_system.SetLoopAllAnimations(true);
            explosion_anim_system.PlayAnimation();
        }

        public override void Update(float dt)
        {
            moving_parts_dict[left_large_laser_spin_id].SpinObjectEndless(1.0f, 0, 0, 200.0f, dt);
            moving_parts_dict[right_large_laser_spin_id].SpinObjectEndless(1.0f, 0, 0, 200.0f, dt);

            UpdateCinematicBars(dt);
            UpdateFlashingLights(dt);
            if (is_exploding)
                UpdateExplosions(dt);
            boss_anim_system.Update(dt);
            explosion_anim_system.Update(dt);
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
                case BOSS_ANIM_STATE.FLYING_DOWN:
                    boss_anim_system.StopAnimation(true, moving_parts_dict);
                    boss_anim_system.AddAnimationSpecsStack(SetFlyingDownAnimationsOne, 6.0f);
                    boss_anim_system.AddAnimationSpecsStack(SetFlyingDownAnimationsTwo, 4.0f);
                    boss_anim_system.AddAnimationSpecsStack(MoveBlackBars, 2.0f);
                    boss_anim_system.AddAnimationSpecsStack(GoToMenu, 0.1f);
                    boss_anim_system.AddAnimationUpdateStack(RunFlyingDownSequenceOne);
                    boss_anim_system.AddAnimationUpdateStack(RunFlyingDownSequenceTwo);
                    boss_anim_system.AddAnimationUpdateStack(EmptyUpdate);
                    boss_anim_system.AddAnimationUpdateStack(EmptyUpdate);
                    //boss_anim_system.SetStateQueue(SetState, BOSS_ANIM_STATE.CRASHING.ToString());
                    boss_anim_system.PlayAnimation();
                    break;
                case BOSS_ANIM_STATE.CRASHING:
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

        void UpdateFlashingLights(float dt)
        {
            if (flash_timer < flash_duration)
            {
                flash_timer += dt;
            }
            else
            {
                flash_timer = 0.0f;

                if (is_flash_color_on)
                {
                    ECS.SetDiffuseTint(cheek_lights_one_id, ref black_color);
                    ECS.SetEmissiveTint(cheek_lights_one_id, ref black_color);
                    ECS.SetDiffuseTint(cheek_lights_two_id, ref black_color);
                    ECS.SetEmissiveTint(cheek_lights_two_id, ref black_color);
                    ECS.SetDiffuseTint(cheek_lights_three_id, ref black_color);
                    ECS.SetEmissiveTint(cheek_lights_three_id, ref black_color);

                    ECS.SetDiffuseTint(right_color_turret_light_1_id, ref black_color);
                    ECS.SetEmissiveTint(right_color_turret_light_1_id, ref black_color);
                    ECS.SetDiffuseTint(right_color_turret_light_2_id, ref black_color);
                    ECS.SetEmissiveTint(right_color_turret_light_2_id, ref black_color);
                    ECS.SetDiffuseTint(right_color_turret_light_3_id, ref black_color);
                    ECS.SetEmissiveTint(right_color_turret_light_3_id, ref black_color);
                    ECS.SetDiffuseTint(right_color_turret_light_toggle_id, ref black_color);

                    ECS.SetDiffuseTint(left_color_turret_light_1_id, ref black_color);
                    ECS.SetEmissiveTint(left_color_turret_light_1_id, ref black_color);
                    ECS.SetDiffuseTint(left_color_turret_light_2_id, ref black_color);
                    ECS.SetEmissiveTint(left_color_turret_light_2_id, ref black_color);
                    ECS.SetDiffuseTint(left_color_turret_light_3_id, ref black_color);
                    ECS.SetEmissiveTint(left_color_turret_light_3_id, ref black_color);
                    ECS.SetDiffuseTint(left_color_turret_light_toggle_id, ref black_color);

                    is_flash_color_on = false;
                }
                else
                {
                    ECS.SetDiffuseTint(cheek_lights_one_id, ref red_color);
                    ECS.SetEmissiveTint(cheek_lights_one_id, ref red_color);
                    ECS.SetDiffuseTint(cheek_lights_two_id, ref red_color);
                    ECS.SetEmissiveTint(cheek_lights_two_id, ref red_color);
                    ECS.SetDiffuseTint(cheek_lights_three_id, ref red_color);
                    ECS.SetEmissiveTint(cheek_lights_three_id, ref red_color);

                    ECS.SetDiffuseTint(right_color_turret_light_1_id, ref red_color);
                    ECS.SetEmissiveTint(right_color_turret_light_1_id, ref red_color);
                    ECS.SetDiffuseTint(right_color_turret_light_2_id, ref red_color);
                    ECS.SetEmissiveTint(right_color_turret_light_2_id, ref red_color);
                    ECS.SetDiffuseTint(right_color_turret_light_3_id, ref red_color);
                    ECS.SetEmissiveTint(right_color_turret_light_3_id, ref red_color);
                    ECS.SetDiffuseTint(right_color_turret_light_toggle_id, ref red_color);

                    ECS.SetDiffuseTint(left_color_turret_light_1_id, ref red_color);
                    ECS.SetEmissiveTint(left_color_turret_light_1_id, ref red_color);
                    ECS.SetDiffuseTint(left_color_turret_light_2_id, ref red_color);
                    ECS.SetEmissiveTint(left_color_turret_light_2_id, ref red_color);
                    ECS.SetDiffuseTint(left_color_turret_light_3_id, ref red_color);
                    ECS.SetEmissiveTint(left_color_turret_light_3_id, ref red_color);
                    ECS.SetDiffuseTint(left_color_turret_light_toggle_id, ref red_color);

                    is_flash_color_on = true;
                }
            }
        }

        void UpdateExplosions(float dt)
        {
            if (explosion_loop_timer < explosion_max_loop_duration)
            {
                explosion_loop_timer += dt;

                //Reset the explosion if max scale
                ECS.GetTransformECS(explosion_vfx_one_id, ref pos, ref rot, ref scale);
                if (scale.X >= explosion_max_scale - 0.05f)
                {
                    ECS.SetScale(explosion_vfx_one_id, new Vector3());
                }
                ECS.GetTransformECS(explosion_vfx_two_id, ref pos, ref rot, ref scale);
                if (scale.X >= explosion_max_scale - 0.05f)
                {
                    ECS.SetScale(explosion_vfx_two_id, new Vector3());
                }
                ECS.GetTransformECS(explosion_vfx_three_id, ref pos, ref rot, ref scale);
                if (scale.X >= explosion_max_scale - 0.05f)
                {
                    ECS.SetScale(explosion_vfx_three_id, new Vector3());
                }
                ECS.GetTransformECS(explosion_vfx_four_id, ref pos, ref rot, ref scale);
                if (scale.X >= explosion_max_scale - 0.05f)
                {
                    ECS.SetScale(explosion_vfx_four_id, new Vector3());
                }
            }
            else
            {
                is_exploding = false;
                explosion_anim_system.SetLoopAllAnimations(false);
                explosion_anim_system.StopAnimation(true, explosion_moving_parts_dict);
                ECS.SetScale(explosion_vfx_one_id, new Vector3());
                ECS.SetScale(explosion_vfx_two_id, new Vector3());
                ECS.SetScale(explosion_vfx_three_id, new Vector3());
                ECS.SetScale(explosion_vfx_four_id, new Vector3());
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

        #region[Exploding Animation Sequence]
        void SetExplodingAnimationsOne()
        {
            explosion_moving_parts_dict[explosion_vfx_one_id].SetLinearScale(new Vector3(explosion_max_scale, explosion_max_scale, explosion_max_scale), new Vector3(explosion_speed, explosion_speed, explosion_speed), true, true, true);
        }

        void SetExplodingAnimationsTwo()
        {
            explosion_moving_parts_dict[explosion_vfx_two_id].SetLinearScale(new Vector3(explosion_max_scale, explosion_max_scale, explosion_max_scale), new Vector3(explosion_speed, explosion_speed, explosion_speed), true, true, true);
        }

        void SetExplodingAnimationsThree()
        {
            explosion_moving_parts_dict[explosion_vfx_three_id].SetLinearScale(new Vector3(explosion_max_scale, explosion_max_scale, explosion_max_scale), new Vector3(explosion_speed, explosion_speed, explosion_speed), true, true, true);
        }

        void SetExplodingAnimationsFour()
        {
            explosion_moving_parts_dict[explosion_vfx_four_id].SetLinearScale(new Vector3(explosion_max_scale, explosion_max_scale, explosion_max_scale), new Vector3(explosion_speed, explosion_speed, explosion_speed), true, true, true);
        }

        void RunExplodingAnimations(float dt)
        {
            //Using the same for all
            explosion_moving_parts_dict[explosion_vfx_one_id].UpdateMovingParts(dt);
            explosion_moving_parts_dict[explosion_vfx_two_id].UpdateMovingParts(dt);
            explosion_moving_parts_dict[explosion_vfx_three_id].UpdateMovingParts(dt);
            explosion_moving_parts_dict[explosion_vfx_four_id].UpdateMovingParts(dt);
        }
        #endregion

        #region[Flying Down Animation Sequence]
        /// <summary>
        /// [Flying State] 
        /// 1. Boss spasming
        /// 2. Main body flies down with camera moving down
        /// 3. Black bars cover full screen, snap camera and show boss dropping down
        /// 4. Boss crash
        /// 5. Black screen and end
        /// </summary>

        void SetFlyingDownAnimationsOne()
        {
            moving_parts_dict[boss_model_parent_id].SetLinearRotation(new Vector3(-30, 0, 0), new Vector3(2.0f, 0, 0), true, false, false);

            moving_parts_dict[left_arm_middle_joint_id].SetPingPongRotation(new Vector3(-90.0f, -46.0f, 0), new Vector3(0, -36.0f, 0), new Vector3(0, 10.0f, 0), false, false, false, false, true, false);
            moving_parts_dict[left_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -15.0f), new Vector3(0, 0, 15.0f), new Vector3(0, 0, 10.0f), false, false, false, false, false, true);

            moving_parts_dict[right_arm_middle_joint_id].SetPingPongRotation(new Vector3(-90.0f, 36.0f, 0), new Vector3(0, 46.0f, 0), new Vector3(0, 10.0f, 0), false, true, false, false, true, false);
            moving_parts_dict[right_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -15.0f), new Vector3(0, 0, 15.0f), new Vector3(0, 0, 10.0f), false, false, false, false, false, true);

            moving_parts_dict[left_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90.0f, 36.0f, 0), new Vector3(0, 46.0f, 0), new Vector3(0, 10.0f, 0), false, false, false, false, true, false);
            moving_parts_dict[left_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -15.0f), new Vector3(0, 0, 15.0f), new Vector3(0, 0, 10.0f), false, false, false, false, false, true);
            
            moving_parts_dict[right_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90.0f, -46.0f, 0), new Vector3(0, 36.0f, 0), new Vector3(0, 10.0f, 0), false, false, false, false, true, false);
            moving_parts_dict[right_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -15.0f), new Vector3(0, 0, 15.0f), new Vector3(0, 0, 10.0f), false, false, false, false, false, true);

            moving_parts_dict[left_color_turret_1_id].SetPingPongRotation(new Vector3(-90.0f, 0, 0), new Vector3(-70.0f, 0, 0), new Vector3(10.0f, 0, 0), false, true, false, true, false, false);
            moving_parts_dict[left_color_turret_2_id].SetLinearRotation(new Vector3(120, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);

            moving_parts_dict[right_color_turret_1_id].SetPingPongRotation(new Vector3(-90.0f, 0, 0), new Vector3(-70.0f, 0, 0), new Vector3(10.0f, 0, 0), false, false, false, true, false, false);
            moving_parts_dict[right_color_turret_2_id].SetLinearRotation(new Vector3(120, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);

            moving_parts_dict[mouth_left_id].SetPingPongRotation(new Vector3(), new Vector3(0, 45, 0), new Vector3(0, 30.0f, 0), false, true, false, false, true, false);
            moving_parts_dict[mouth_right_id].SetPingPongRotation(new Vector3(0, -45, 0), new Vector3(), new Vector3(0, 30.0f, 0), false, false, false, false, true, false);
        }

        void RunFlyingDownSequenceOne(float dt)
        {
            moving_parts_dict[boss_model_parent_id].UpdateMovingParts(dt);

            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);

            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);

            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(dt);

            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);

            moving_parts_dict[left_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_2_id].UpdateMovingParts(dt);

            moving_parts_dict[right_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[right_color_turret_2_id].UpdateMovingParts(dt);

            moving_parts_dict[mouth_left_id].UpdateMovingParts(dt);
            moving_parts_dict[mouth_right_id].UpdateMovingParts(dt);
        }

        void SetFlyingDownAnimationsTwo()
        {
            moving_parts_dict[boss_model_parent_id].SetLinearPosition(new Vector3(0, 100.0f, 0), new Vector3(0, 1.0f, 0), false, true, false);
            moving_parts_dict[camera_id].SetLinearPosition(new Vector3(0, 100.0f, 0), new Vector3(0, 1.0f, 0), false, true, false);

            moving_parts_dict[boss_model_parent_id].SetPingPongRotation(new Vector3(), new Vector3(), new Vector3(0, 0, 0), false, false, false, false, false, false);
            moving_parts_dict[boss_model_parent_id].SetLinearRotation(new Vector3(0, 0, -15), new Vector3(0, 0, 0.5f), false, false, true);

            moving_parts_dict[left_arm_middle_joint_id].SetLinearRotation(new Vector3(-180, -46, 0), new Vector3(1.5f, 0.5f, 0.5f), true, true, true);
            moving_parts_dict[left_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -106.0f), new Vector3(0, 0, -96.0f), new Vector3(0, 0, 20.0f), false, false, false, false, false, true);

            moving_parts_dict[right_arm_middle_joint_id].SetLinearRotation(new Vector3(-180, -46, 0), new Vector3(1.5f, 0.5f, 0.5f), true, true, true);
            moving_parts_dict[right_arm_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 96.0f), new Vector3(0, 0, 106.0f), new Vector3(0, 0, 20.0f), false, false, false, false, false, true);

            moving_parts_dict[left_leg_middle_joint_id].SetUpdatePingPongRotation(false);
            moving_parts_dict[right_leg_middle_joint_id].SetUpdatePingPongRotation(false);
            //moving_parts_dict[left_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90.0f, 36.0f, 0), new Vector3(0, 46.0f, 0), new Vector3(0, 0, 0), false, false, false, false, false, false);
            //moving_parts_dict[right_leg_middle_joint_id].SetPingPongRotation(new Vector3(-90.0f, -46.0f, 0), new Vector3(0, 36.0f, 0), new Vector3(0, 0, 0), false, false, false, false, false, false);

            moving_parts_dict[left_leg_middle_joint_id].SetLinearRotation(new Vector3(179, 46, 0), new Vector3(1.5f, 0.5f, 0.5f), true, true, true);
            moving_parts_dict[left_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, -106.0f), new Vector3(0, 0, -96.0f), new Vector3(0, 0, 20.0f), false, false, false, false, false, true);

            moving_parts_dict[right_leg_middle_joint_id].SetLinearRotation(new Vector3(179, -46, 0), new Vector3(1.5f, 0.5f, 0.5f), true, true, true);
            moving_parts_dict[right_leg_end_joint_id].SetPingPongRotation(new Vector3(0, 0, 96.0f), new Vector3(0, 0, 106.0f), new Vector3(0, 0, 20.0f), false, false, false, false, false, true);

            moving_parts_dict[left_color_turret_1_id].SetLinearRotation(new Vector3(-98.0f, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);
            moving_parts_dict[left_color_turret_body_pivot_id].SetPingPongRotation(new Vector3(-78.0f, 0, 0), new Vector3(-66.0f, 0, 0), new Vector3(20.0f, 0, 0), false, false, false, true, false, false);
            
            moving_parts_dict[right_color_turret_1_id].SetLinearRotation(new Vector3(-98.0f, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true);
            moving_parts_dict[right_color_turret_body_pivot_id].SetPingPongRotation(new Vector3(-78.0f, 0, 0), new Vector3(-66.0f, 0, 0), new Vector3(20.0f, 0, 0), false, false, false, true, false, false);

            moving_parts_dict[mouth_left_id].SetPingPongRotation(new Vector3(), new Vector3(0, 45, 0), new Vector3(0, 45.0f, 0), false, true, false, false, true, false);
            moving_parts_dict[mouth_right_id].SetPingPongRotation(new Vector3(0, -45, 0), new Vector3(), new Vector3(0, 45.0f, 0), false, false, false, false, true, false);
            ECS.SetParticlePlay(smoke_id, true);
        }

        void RunFlyingDownSequenceTwo(float dt)
        {
            moving_parts_dict[boss_model_parent_id].UpdateMovingParts(dt);
            moving_parts_dict[camera_id].UpdateMovingParts(dt);

            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(dt);

            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(dt);

            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(dt);

            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(dt);

            moving_parts_dict[left_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[left_color_turret_body_pivot_id].UpdateMovingParts(dt);

            moving_parts_dict[right_color_turret_1_id].UpdateMovingParts(dt);
            moving_parts_dict[right_color_turret_body_pivot_id].UpdateMovingParts(dt);

            moving_parts_dict[mouth_left_id].UpdateMovingParts(dt);
            moving_parts_dict[mouth_right_id].UpdateMovingParts(dt);
        }

        void MoveBlackBars()
        {
            cinematic_cover_screen = true;
        }

        void EmptyUpdate(float dt)
        {

        }

        void GoToMenu()
        {
            GameUtilities.LoadScene("MainMenu");
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
            //GameUtilities.LoadScene("Level01_Boss");
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