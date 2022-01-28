using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    /// <summary>
    /// Left parts use negative to move counter clockwise
    /// </summary>

    public class MOVING_PARTS
    {
        public float lerp_position_speed_X;
        public float lerp_position_speed_Y;
        public float lerp_position_speed_Z;

        public Vector3 lerp_position_upper_limit;
        public Vector3 lerp_position_lower_limit;

        public float lerp_rotation_speed_X;
        public float lerp_rotation_speed_Y;
        public float lerp_rotation_speed_Z;

        public Vector3 lerp_rotation_upper_limit;
        public Vector3 lerp_rotation_lower_limit;

        public bool toggle_spin;
        public Vector3 spin_rotation;

        public bool lerp_position_positive_direction_X;       //+ve means going towards upper limit, -ve going towards lower limit
        public bool lerp_position_positive_direction_Y;
        public bool lerp_position_positive_direction_Z;

        public bool lerp_rotation_positive_direction_X;
        public bool lerp_rotation_positive_direction_Y;
        public bool lerp_rotation_positive_direction_Z;

        public bool lerp_position_ping_pong_X;
        public bool lerp_position_ping_pong_Y;
        public bool lerp_position_ping_pong_Z;

        public bool lerp_rotation_ping_pong_X;
        public bool lerp_rotation_ping_pong_Y;
        public bool lerp_rotation_ping_pong_Z;

        public bool update_position = false;
        public bool update_rotation = false;

        //Position
        public void SetLerpPosSpeedX(float value)
        {
            lerp_position_speed_X = value;
        }

        public void SetLerpPosSpeedY(float value)
        {
            lerp_position_speed_Y = value;
        }

        public void SetLerpPosSpeedZ(float value)
        {
            lerp_position_speed_Z = value;
        }

        public void SetLerpPosUpperLimit(Vector3 value)
        {
            lerp_position_upper_limit = value;
        }

        public void SetLerpPosLowerLimit(Vector3 value)
        {
            lerp_position_lower_limit = value;
        }

        public void SetLerpPosPositiveDirectionX(bool value)
        {
            lerp_position_positive_direction_X = value;
        }

        public void SetLerpPosPositiveDirectionY(bool value)
        {
            lerp_position_positive_direction_Y = value;
        }

        public void SetLerpPosPositiveDirectionZ(bool value)
        {
            lerp_position_positive_direction_Z = value;
        }

        public void SetLerpPosPingPongX(bool value)
        {
            lerp_position_ping_pong_X = value;
        }

        public void SetLerpPosPingPongY(bool value)
        {
            lerp_position_ping_pong_Y = value;
        }

        public void SetLerpPosPingPongZ(bool value)
        {
            lerp_position_ping_pong_Z = value;
        }

        public void SetUpdatePosition(bool value)
        {
            update_position = value;
        }

        //Rotation
        public void SetLerpRotSpeedX(float value)
        {
            lerp_rotation_speed_X = value;
        }

        public void SetLerpRotSpeedY(float value)
        {
            lerp_rotation_speed_Y = value;
        }

        public void SetLerpRotSpeedZ(float value)
        {
            lerp_rotation_speed_Z = value;
        }

        public void SetLerpRotUpperLimit(Vector3 value)
        {
            lerp_rotation_upper_limit = value;
        }

        public void SetLerpRotLowerLimit(Vector3 value)
        {
            lerp_rotation_lower_limit = value;
        }

        public void SetLerpRotPositiveDirectionX(bool value)
        {
            lerp_rotation_positive_direction_X = value;
        }

        public void SetLerpRotPositiveDirectionY(bool value)
        {
            lerp_rotation_positive_direction_Y = value;
        }

        public void SetLerpRotPositiveDirectionZ(bool value)
        {
            lerp_rotation_positive_direction_Z = value;
        }

        public void SetLerpRotPingPongX(bool value)
        {
            lerp_rotation_ping_pong_X = value;
        }

        public void SetLerpRotPingPongY(bool value)
        {
            lerp_rotation_ping_pong_Y = value;
        }

        public void SetLerpRotPingPongZ(bool value)
        {
            lerp_rotation_ping_pong_Z = value;
        }

        public void SetUpdateRotation(bool value)
        {
            update_rotation = value;
        }

        //Misc
        public void SetToggleSpin(bool value)
        {
            toggle_spin = value;
        }
    }

    public class L1Boss : MonoBehaviour
    {
        enum BOSS_BEHAVIOUR_STATE
        {
            EMPTY,
            MOVING,
            ARM_LASER_ATTACK,
            LAUNCH_NORMAL_ADDS,
            LAUNCH_LASER_ADDS,
            ARTI_ATTACK,
            DEATH_SEQUENCE,
            TRANSIT_SCENE
        }

        Dictionary<uint, MOVING_PARTS> moving_parts_dict;

        BOSS_BEHAVIOUR_STATE current_state;

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

        /////////////////////////////////////////////////////////////////////////
        //  Death sequence
        /////////////////////////////////////////////////////////////////////////

        //Shield
        public float health;
        bool is_shield_broken;
        bool start_shield_break_countdown;
        const float shield_break_delay_duration = 2.0f;
        float shield_break_delay_timer;
        uint false_core_id;
        uint sparks_particle_id;

        //Eye
        uint mouth_left_id;
        uint mouth_right_id;
        Vector3 rotate_angle;
        const float rotate_speed = 10.0f;
        bool start_eye_countdown;
        const float eye_delay_duration = 3.0f;
        float eye_delay_timer;

        //Black screen & particle
        bool start_particle_countdown;
        const float particle_delay_duration = 2.5f;
        float particle_timer;

        //Scene transition
        bool start_scene_change_countdown;
        const float scene_change_delay_duration = 5.0f;
        float scene_change_delay_timer;
        uint black_screen_id;

        //Laser
        float laser_spin_addition;
        const float laser_spin_addition_speed = 10.0f;

        //For ECS get transform
        Vector3 pos = new Vector3();
        Vector3 rot = new Vector3();
        Vector3 scale = new Vector3();

        public struct Animation_Stack
        {
            public Action action;
            public float animation_duration;
            public bool loop_animation;
        }

        //Animation stack
        List<Animation_Stack> animation_specs_stack;
        List<Action<float>> animation_update_stack;
        float current_animation_time;
        int current_animation_index;
        bool play_animation;
        BOSS_BEHAVIOUR_STATE state_queue;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        public override void Start()
        {
            FindEntities();

            health = 5;
            is_shield_broken = false;
            ECS.SetActive(black_screen_id, false);
            ECS.SetActive(sparks_particle_id, false);
            rotate_angle = new Vector3(0, 0, 40.0f);
            current_animation_index = 0;
        }

        void FindEntities()
        {
            //UI
            black_screen_id = ECS.FindEntityWithName("L1 Black Screen");

            //Core
            false_core_id = ECS.FindEntityWithName("FalseCore");
            sparks_particle_id = ECS.FindEntityWithName("Shield Sparks");
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

            //Main Laser
            main_laser_rails_id = ECS.FindEntityWithName("MainLaser_Rails");
            main_laser_rail_vent_id = ECS.FindEntityWithName("RailVent_Panels");
            main_laser_barrel_id = ECS.FindEntityWithName("MainLaser");

            //Create and initialize the list of moving parts
            moving_parts_dict = new Dictionary<uint, MOVING_PARTS>();

            //Boss body
            moving_parts_dict.Add(entityID, new MOVING_PARTS());

            //Core
            moving_parts_dict.Add(false_core_id, new MOVING_PARTS());
            moving_parts_dict.Add(mouth_left_id, new MOVING_PARTS());
            moving_parts_dict.Add(mouth_right_id, new MOVING_PARTS());

            //Arms
            moving_parts_dict.Add(left_arm_middle_joint_id, new MOVING_PARTS());
            moving_parts_dict.Add(left_arm_end_joint_id, new MOVING_PARTS());
            moving_parts_dict.Add(right_arm_middle_joint_id, new MOVING_PARTS());
            moving_parts_dict.Add(right_arm_end_joint_id, new MOVING_PARTS());

            //Arm Lasers
            moving_parts_dict.Add(left_large_laser_spin_id, new MOVING_PARTS());
            moving_parts_dict.Add(right_large_laser_spin_id, new MOVING_PARTS());

            //Legs
            moving_parts_dict.Add(left_leg_middle_joint_id, new MOVING_PARTS());
            moving_parts_dict.Add(right_leg_middle_joint_id, new MOVING_PARTS());
            moving_parts_dict.Add(left_leg_end_joint_id, new MOVING_PARTS());
            moving_parts_dict.Add(right_leg_end_joint_id, new MOVING_PARTS());

            //Launching bays
            moving_parts_dict.Add(right_launching_bay_one_id, new MOVING_PARTS());
            moving_parts_dict.Add(right_launching_bay_two_id, new MOVING_PARTS());
            moving_parts_dict.Add(right_launching_bay_three_id, new MOVING_PARTS());

            moving_parts_dict.Add(left_launching_bay_one_id, new MOVING_PARTS());
            moving_parts_dict.Add(left_launching_bay_two_id, new MOVING_PARTS());
            moving_parts_dict.Add(left_launching_bay_three_id, new MOVING_PARTS());

            //Artillery
            moving_parts_dict.Add(artillery_axis_id, new MOVING_PARTS());
            moving_parts_dict.Add(artillery_barrel_id, new MOVING_PARTS());

            //Main Laser
            moving_parts_dict.Add(main_laser_rails_id, new MOVING_PARTS());
            moving_parts_dict.Add(main_laser_rail_vent_id, new MOVING_PARTS());
            moving_parts_dict.Add(main_laser_barrel_id, new MOVING_PARTS());

            //current_state = BOSS_BEHAVIOUR_STATE.IDLE;
            animation_specs_stack = new List<Animation_Stack>();
            animation_update_stack = new List<Action<float>>();

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);

            state_queue = BOSS_BEHAVIOUR_STATE.EMPTY;
        }

        public override void Update(float dt)
        {
            SpinObjectEndless(left_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);
            SpinObjectEndless(right_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);

            //Testing
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_G))
            {
                SetState(BOSS_BEHAVIOUR_STATE.MOVING);

                //TakeDamage(1);
            }

            if (InputUtility.onKeyTriggered(KEY_ID.KEY_H))
            {
                SetState(BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS);
            }

            if (InputUtility.onKeyTriggered(KEY_ID.KEY_J))
            {
                SetState(BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE);
            }

            switch (current_state)
            {
                case BOSS_BEHAVIOUR_STATE.MOVING:
                    RunMovingSequence(dt);
                    break;
                case BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE:
                    laser_spin_addition += laser_spin_addition_speed * dt;
                    SpinObjectEndless(main_laser_barrel_id, 0, 0, 1.0f, laser_spin_addition, dt);
                    break;
            }

            if (is_shield_broken)
            {
                DeathSequence(dt);
            }

            if (animation_update_stack.Count > 0 && play_animation)
            {
                animation_update_stack[current_animation_index].Invoke(dt);

                //Go to the next stack in the animation if the duration exceeds
                if (current_animation_time >= animation_specs_stack[current_animation_index].animation_duration)
                {
                    RunNextAnimationStack();
                }
                else
                {
                    //Only add the timer if the stack has something inside
                    if (animation_specs_stack.Count > 0)
                    {
                        current_animation_time += dt;
                    }
                }
            }
        }

        void DeathSequence(float dt)
        {
            //Shield break animation
            if (start_shield_break_countdown)
            {
                if (shield_break_delay_timer < shield_break_delay_duration)
                {
                    shield_break_delay_timer += dt;
                }
                else
                {
                    start_shield_break_countdown = false;

                    //Stops the sparks particle and disable shields
                    ECS.SetParticlePause(sparks_particle_id, true);
                    ECS.SetActive(sparks_particle_id, false);
                    ECS.SetActive(false_core_id, false);

                    start_eye_countdown = true;
                }
            }

            //Eye rotation and fire particle cannon animation
            if (start_eye_countdown)
            {
                //Rotate the eye lids
                ECS.SetRotation(mouth_left_id, Vector3.Lerp(ECS.GetComponent<Transform>(mouth_left_id).Rotation, rotate_angle, dt * rotate_speed));
                ECS.SetRotation(mouth_right_id, Vector3.Lerp(ECS.GetComponent<Transform>(mouth_left_id).Rotation, -rotate_angle, dt * rotate_speed));

                if (eye_delay_timer < eye_delay_duration)
                {
                    eye_delay_timer += dt;
                }
                else
                {
                    start_eye_countdown = false;
                    start_particle_countdown = true;

                    ECS.SetParticlePause(main_laser_barrel_id, false);
                }
            }

            //Collision and black screen
            if (start_particle_countdown)
            {
                if (particle_timer < particle_delay_duration)
                {
                    particle_timer += dt;
                }
                else
                {
                    //Fire OHK cannon (activate large particle towards player and enable a black screen after being hit)
                    ECS.SetActive(black_screen_id, true);

                    //Play player take severe damage and crashing audio
                    //ECS.PlayAudio(entityID, 1);

                    //Transit to another scene after X seconds
                    start_scene_change_countdown = true;
                }
            }

            //Countdown for scene changing
            if (start_scene_change_countdown)
            {
                if (scene_change_delay_timer < scene_change_delay_duration)
                {
                    scene_change_delay_timer += dt;
                }
                else
                {
                    start_scene_change_countdown = false;
                    //GameUtilities.LoadScene("Level2");
                }
            }
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            //Make sure it does not accidently overflow
            if (health < -100.0f)
                health = -100.0f;
            //Console.WriteLine("Health: " + health);
            //Triggers the shield depleting and starts the sequence of the OHK
            if (health <= 0)
            {
                //Console.WriteLine("Dead");
                if (!is_shield_broken)
                {
                    //Play shield destroyed audio
                    //ECS.PlayAudio(entityID, 0);
                    Console.WriteLine("shield broken");

                    //Play electric sparks particle
                    ECS.SetActive(sparks_particle_id, true);
                    ECS.SetParticlePause(sparks_particle_id, false);

                    //Start counting down the delay
                    start_shield_break_countdown = true;

                    //Only need to happen once
                    is_shield_broken = true;
                }
            }
        }

        void SetState(BOSS_BEHAVIOUR_STATE set_state)
        {
            current_state = set_state;

            //Update animation sets based on state
            switch (current_state)
            {
                case BOSS_BEHAVIOUR_STATE.MOVING:
                    SetMovingStateAnimations();
                    //Set the moving animation parameters and adding it to the stack
                    //AddAnimationSpecsStack(SetMovingStateAnimations, 10.0f);
                    //AddAnimationSpecsStack(SetTestStateAnimations, 5.0f);
                    //RunNextAnimationStack();
                    //AddAnimationUpdateStack(RunMovingSequence);
                    //AddAnimationUpdateStack(RunTestSequence);
                    break;
                case BOSS_BEHAVIOUR_STATE.LAUNCH_NORMAL_ADDS:
                    StopAnimation(true);
                    AddAnimationSpecsStack(SetLaunchStateAnimationsOne, 2.0f);
                    AddAnimationSpecsStack(SetLaunchStateAnimationsTwo, 2.0f);
                    AddAnimationSpecsStack(SetLaunchStateAnimationsThree, 3.5f);
                    AddAnimationSpecsStack(SetLaunchStateAnimationsFour, 2.0f);
                    AddAnimationSpecsStack(SetLaunchStateAnimationsFive, 0.5f);
                    AddAnimationUpdateStack(RunLaunchStateSequenceOne);
                    AddAnimationUpdateStack(RunLaunchStateSequenceTwo);
                    AddAnimationUpdateStack(RunLaunchStateSequenceThree);
                    AddAnimationUpdateStack(RunLaunchStateSequenceFour);
                    AddAnimationUpdateStack(RunLaunchStateSequenceFive);
                    SetStateQueue(BOSS_BEHAVIOUR_STATE.MOVING);
                    PlayAnimation();
                    break;
                case BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE:
                    StopAnimation(true);
                    AddAnimationSpecsStack(SetDeathStateAnimationsOne, 1.5f);
                    AddAnimationSpecsStack(SetDeathStateAnimationsTwo, 15.0f);
                    AddAnimationSpecsStack(SetDeathStateAnimationsThree, 3.0f);
                    AddAnimationUpdateStack(RunDeathStateSequenceOne);
                    AddAnimationUpdateStack(RunDeathStateSequenceTwo);
                    AddAnimationUpdateStack(RunDeathStateSequenceThree);
                    SetStateQueue(BOSS_BEHAVIOUR_STATE.TRANSIT_SCENE);
                    PlayAnimation();
                    break;
            }
        }

        #region[State & animation queue]
        void SetStateQueue(BOSS_BEHAVIOUR_STATE setState)
        {
            state_queue = setState;
        }

        void ResetStateQueue()
        {
            state_queue = BOSS_BEHAVIOUR_STATE.EMPTY;
        }

        void AddAnimationSpecsStack(Action action, float anim_duration, bool loop = false)
        {
            animation_specs_stack.Add(new Animation_Stack()
            {
                action = action,
                animation_duration = anim_duration,
                loop_animation = loop
            });
        }

        void AddAnimationUpdateStack(Action<float> action)
        {
            animation_update_stack.Add(action);
        }

        void PlayAnimation()
        {
            current_animation_time = 0.0f;
            animation_specs_stack[current_animation_index].action.Invoke();
            play_animation = true;
        }

        void StopAnimation(bool clear_stack)
        {
            play_animation = false;

            if(clear_stack)
                ClearAnimationStack();

            //if there is a queue of animation state to transition to, transition and reset the state queue
            if (state_queue != BOSS_BEHAVIOUR_STATE.EMPTY)
            {
                SetState(state_queue);
                ResetStateQueue();
            }
        }

        void ClearAnimationStack()
        {
            current_animation_index = 0;
            current_animation_time = 0.0f;

            //Use the function so its easier to track
            animation_specs_stack.Clear();
            animation_update_stack.Clear();
        }

        void RunNextAnimationStack()
        {
            play_animation = true;
            current_animation_time = 0.0f;

            //Increase the index if not looping
            if (animation_specs_stack[current_animation_index].loop_animation == false)
            {
                ++current_animation_index;
            }

            if (current_animation_index <= animation_specs_stack.Count - 1)
            {
                //Only run if the stack still has actions
                if (animation_specs_stack[current_animation_index].action != null)
                {
                    animation_specs_stack[current_animation_index].action.Invoke();
                }
            }
            else
            {
                //Stop playing once the end of stack and end of animation is reached
                play_animation = false;
                ClearAnimationStack();

                //if there is a queue of animation state to transition to, transition and reset the state queue
                if (state_queue != BOSS_BEHAVIOUR_STATE.EMPTY)
                {
                    SetState(state_queue);
                    ResetStateQueue();
                }
            }
        }
        #endregion

        #region[Set & Update moving parts animation]

        void SetMovingPartsPosition(uint id, Vector3 set_pos_lower_limit, Vector3 set_pos_upper_limit, Vector3 lerp_speed,
            bool set_positive_direction_X, bool set_positive_direction_Y, bool set_positive_direction_Z,
            bool set_ping_pong_X, bool set_ping_pong_Y, bool set_ping_pong_Z)
        {
            //Update the moving part's parameters
            moving_parts_dict[id].SetLerpPosSpeedX(lerp_speed.X);
            moving_parts_dict[id].SetLerpPosSpeedY(lerp_speed.Y);
            moving_parts_dict[id].SetLerpPosSpeedZ(lerp_speed.Z);

            moving_parts_dict[id].SetLerpPosUpperLimit(set_pos_upper_limit);
            moving_parts_dict[id].SetLerpPosLowerLimit(set_pos_lower_limit);

            moving_parts_dict[id].SetLerpPosPingPongX(set_ping_pong_X);
            moving_parts_dict[id].SetLerpPosPingPongY(set_ping_pong_Y);
            moving_parts_dict[id].SetLerpPosPingPongZ(set_ping_pong_Z);

            moving_parts_dict[id].SetLerpPosPositiveDirectionX(set_positive_direction_X);
            moving_parts_dict[id].SetLerpPosPositiveDirectionY(set_positive_direction_Y);
            moving_parts_dict[id].SetLerpPosPositiveDirectionZ(set_positive_direction_Z);


            //Automatically enable since already setting
            moving_parts_dict[id].SetUpdatePosition(true);
        }

        void SetMovingPartsRotation(uint id, Vector3 set_pos_lower_limit, Vector3 set_pos_upper_limit, Vector3 lerp_speed,
            bool set_positive_direction_X, bool set_positive_direction_Y, bool set_positive_direction_Z,
            bool set_ping_pong_X, bool set_ping_pong_Y, bool set_ping_pong_Z)
        {
            //Update the moving part's parameters
            moving_parts_dict[id].SetLerpRotSpeedX(lerp_speed.X);
            moving_parts_dict[id].SetLerpRotSpeedY(lerp_speed.Y);
            moving_parts_dict[id].SetLerpRotSpeedZ(lerp_speed.Z);

            moving_parts_dict[id].SetLerpRotUpperLimit(set_pos_upper_limit);
            moving_parts_dict[id].SetLerpRotLowerLimit(set_pos_lower_limit);

            moving_parts_dict[id].SetLerpRotPingPongX(set_ping_pong_X);
            moving_parts_dict[id].SetLerpRotPingPongY(set_ping_pong_Y);
            moving_parts_dict[id].SetLerpRotPingPongZ(set_ping_pong_Z);

            moving_parts_dict[id].SetLerpRotPositiveDirectionX(set_positive_direction_X);
            moving_parts_dict[id].SetLerpRotPositiveDirectionY(set_positive_direction_Y);
            moving_parts_dict[id].SetLerpRotPositiveDirectionZ(set_positive_direction_Z);

            //Automatically enable since already setting
            moving_parts_dict[id].SetUpdateRotation(true);
        }

        void UpdateMovingParts(uint id, float dt)
        {
            //Update position only if needed
            if (moving_parts_dict[id].update_position)
            {
                UpdateMovingPartsPosition(id, dt);
            }
            //Update rotation only if needed
            if (moving_parts_dict[id].update_rotation)
            {
                UpdateMovingPartsRotation(id, dt);
            }
        }

        void UpdateMovingPartsPosition(uint id, float dt)
        {
            //===============================================================================================================================================//
            //X Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (moving_parts_dict[id].lerp_position_ping_pong_X)
            {
                ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                //Move 
                if (moving_parts_dict[id].lerp_position_positive_direction_X)
                {
                    ECS.SetPosition(id, Vector3.Lerp(pos, new Vector3(moving_parts_dict[id].lerp_position_upper_limit.X, pos.Y, pos.Z), moving_parts_dict[id].lerp_position_speed_X * dt));

                    //Check for limit and reverse
                    if (pos.X >= moving_parts_dict[id].lerp_position_upper_limit.X - 0.1f)
                    {
                        moving_parts_dict[id].lerp_position_positive_direction_X = false;
                    }
                }
                else
                {
                    ECS.SetPosition(id, Vector3.Lerp(pos, new Vector3(moving_parts_dict[id].lerp_position_lower_limit.X, pos.Y, pos.Z), moving_parts_dict[id].lerp_position_speed_X * dt));

                    if (pos.X <= moving_parts_dict[id].lerp_position_lower_limit.X + 0.1f)
                    {
                        moving_parts_dict[id].lerp_position_positive_direction_X = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                //Move 
                if (moving_parts_dict[id].lerp_position_positive_direction_X)
                {
                    ECS.SetPosition(id, Vector3.Lerp(pos, new Vector3(moving_parts_dict[id].lerp_position_upper_limit.X, pos.Y, pos.Z), moving_parts_dict[id].lerp_position_speed_X * dt));
                }
                else
                {
                    ECS.SetPosition(id, Vector3.Lerp(pos, new Vector3(moving_parts_dict[id].lerp_position_lower_limit.X, pos.Y, pos.Z), moving_parts_dict[id].lerp_position_speed_X * dt));
                }
            }

            //===============================================================================================================================================//
            //Y Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (moving_parts_dict[id].lerp_position_ping_pong_Y)
            {
                ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                //Move 
                if (moving_parts_dict[id].lerp_position_positive_direction_Y)
                {
                    ECS.SetPosition(id, Vector3.Lerp(pos, new Vector3(pos.X, moving_parts_dict[id].lerp_position_upper_limit.Y, pos.Z), moving_parts_dict[id].lerp_position_speed_Y * dt));

                    //Check for limit and reverse
                    if (pos.Y >= moving_parts_dict[id].lerp_position_upper_limit.Y - 0.1f)
                    {
                        moving_parts_dict[id].lerp_position_positive_direction_Y = false;
                    }
                }
                else
                {
                    ECS.SetPosition(id, Vector3.Lerp(pos, new Vector3(pos.X, moving_parts_dict[id].lerp_position_lower_limit.Y, pos.Z), moving_parts_dict[id].lerp_position_speed_Y * dt));

                    if (pos.Y <= moving_parts_dict[id].lerp_position_lower_limit.Y + 0.1f)
                    {
                        moving_parts_dict[id].lerp_position_positive_direction_Y = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                //Move 
                if (moving_parts_dict[id].lerp_position_positive_direction_Y)
                {
                    ECS.SetPosition(id, Vector3.Lerp(pos, new Vector3(pos.X, moving_parts_dict[id].lerp_position_upper_limit.Y, pos.Z), moving_parts_dict[id].lerp_position_speed_Y * dt));
                }
                else
                {
                    ECS.SetPosition(id, Vector3.Lerp(pos, new Vector3(pos.X, moving_parts_dict[id].lerp_position_lower_limit.Y, pos.Z), moving_parts_dict[id].lerp_position_speed_Y * dt));
                }
            }

            //===============================================================================================================================================//
            //Z Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (moving_parts_dict[id].lerp_position_ping_pong_Z)
            {
                ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                //Move 
                if (moving_parts_dict[id].lerp_position_positive_direction_Z)
                {
                    ECS.SetPosition(id, Vector3.Lerp(pos, new Vector3(pos.X, pos.Y, moving_parts_dict[id].lerp_position_upper_limit.Z), moving_parts_dict[id].lerp_position_speed_Z * dt));

                    //Check for limit and reverse
                    if (pos.Z >= moving_parts_dict[id].lerp_position_upper_limit.Z - 0.1f)
                    {
                        moving_parts_dict[id].lerp_position_positive_direction_Z = false;
                    }
                }
                else
                {
                    ECS.SetPosition(id, Vector3.Lerp(pos, new Vector3(pos.X, pos.Y, moving_parts_dict[id].lerp_position_lower_limit.Z), moving_parts_dict[id].lerp_position_speed_Z * dt));

                    if (pos.Z <= moving_parts_dict[id].lerp_position_lower_limit.Z + 0.1f)
                    {
                        moving_parts_dict[id].lerp_position_positive_direction_Z = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                //Move 
                if (moving_parts_dict[id].lerp_position_positive_direction_Z)
                {
                    ECS.SetPosition(id, Vector3.Lerp(pos, new Vector3(pos.X, pos.Y, moving_parts_dict[id].lerp_position_upper_limit.Z), moving_parts_dict[id].lerp_position_speed_Z * dt));
                }
                else
                {
                    ECS.SetPosition(id, Vector3.Lerp(pos, new Vector3(pos.X, pos.Y, moving_parts_dict[id].lerp_position_lower_limit.Z), moving_parts_dict[id].lerp_position_speed_Z * dt));
                }
            }
        }

        void UpdateMovingPartsRotation(uint id, float dt)
        {
            //===============================================================================================================================================//
            //X Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (moving_parts_dict[id].lerp_rotation_ping_pong_X)
            {
                ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                //Rotate 
                if (moving_parts_dict[id].lerp_rotation_positive_direction_X)
                {


                    ECS.SetRotation(id, Vector3.Lerp(rot, new Vector3(moving_parts_dict[id].lerp_rotation_upper_limit.X, rot.Y, rot.Z), moving_parts_dict[id].lerp_rotation_speed_X * dt));

                    //Check for limit and reverse
                    if (rot.X >= moving_parts_dict[id].lerp_rotation_upper_limit.X - 0.1f)
                    {
                        moving_parts_dict[id].lerp_rotation_positive_direction_X = false;
                    }
                }
                else
                {
                    ECS.SetRotation(id, Vector3.Lerp(rot, new Vector3(moving_parts_dict[id].lerp_rotation_lower_limit.X, rot.Y, rot.Z), moving_parts_dict[id].lerp_rotation_speed_X * dt));

                    if (rot.X <= moving_parts_dict[id].lerp_rotation_lower_limit.X + 0.1f)
                    {
                        moving_parts_dict[id].lerp_rotation_positive_direction_X = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                //Rotate 
                if (moving_parts_dict[id].lerp_rotation_positive_direction_X)
                {
                    ECS.SetRotation(id, Vector3.Lerp(rot, new Vector3(moving_parts_dict[id].lerp_rotation_upper_limit.X, rot.Y, rot.Z), moving_parts_dict[id].lerp_rotation_speed_X * dt));
                }
                else
                {
                    ECS.SetRotation(id, Vector3.Lerp(rot, new Vector3(moving_parts_dict[id].lerp_rotation_lower_limit.X, rot.Y, rot.Z), moving_parts_dict[id].lerp_rotation_speed_X * dt));
                }
            }

            //===============================================================================================================================================//
            //Y Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (moving_parts_dict[id].lerp_rotation_ping_pong_Y)
            {
                ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                //Rotate 
                if (moving_parts_dict[id].lerp_rotation_positive_direction_Y)
                {
                    ECS.SetRotation(id, Vector3.Lerp(rot, new Vector3(rot.X, moving_parts_dict[id].lerp_rotation_upper_limit.Y, rot.Z), moving_parts_dict[id].lerp_rotation_speed_Y * dt));

                    //Check for limit and reverse
                    if (rot.Y >= moving_parts_dict[id].lerp_rotation_upper_limit.Y - 0.1f)
                    {
                        moving_parts_dict[id].lerp_rotation_positive_direction_Y = false;
                    }
                }
                else
                {
                    ECS.SetRotation(id, Vector3.Lerp(rot, new Vector3(rot.X, moving_parts_dict[id].lerp_rotation_lower_limit.Y, rot.Z), moving_parts_dict[id].lerp_rotation_speed_Y * dt));

                    if (rot.Y <= moving_parts_dict[id].lerp_rotation_lower_limit.Y + 0.1f)
                    {
                        moving_parts_dict[id].lerp_rotation_positive_direction_Y = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                //Rotate 
                if (moving_parts_dict[id].lerp_rotation_positive_direction_Y)
                {
                    ECS.SetRotation(id, Vector3.Lerp(rot, new Vector3(rot.X, moving_parts_dict[id].lerp_rotation_upper_limit.Y, rot.Z), moving_parts_dict[id].lerp_rotation_speed_Y * dt));
                }
                else
                {
                    ECS.SetRotation(id, Vector3.Lerp(rot, new Vector3(rot.X, moving_parts_dict[id].lerp_rotation_lower_limit.Y, rot.Z), moving_parts_dict[id].lerp_rotation_speed_Y * dt));
                }
            }

            //===============================================================================================================================================//
            //Z Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (moving_parts_dict[id].lerp_rotation_ping_pong_Z)
            {
                ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                //Rotate 
                if (moving_parts_dict[id].lerp_rotation_positive_direction_Z)
                {
                    ECS.SetRotation(id, Vector3.Lerp(rot, new Vector3(rot.X, rot.Y, moving_parts_dict[id].lerp_rotation_upper_limit.Z), moving_parts_dict[id].lerp_rotation_speed_Z * dt));

                    //Check for limit and reverse
                    if (rot.Z >= moving_parts_dict[id].lerp_rotation_upper_limit.Z - 0.1f)
                    {
                        moving_parts_dict[id].lerp_rotation_positive_direction_Z = false;
                    }
                }
                else
                {
                    ECS.SetRotation(id, Vector3.Lerp(rot, new Vector3(rot.X, rot.Y, moving_parts_dict[id].lerp_rotation_lower_limit.Z), moving_parts_dict[id].lerp_rotation_speed_Z * dt));

                    if (rot.Z <= moving_parts_dict[id].lerp_rotation_lower_limit.Z + 0.1f)
                    {
                        moving_parts_dict[id].lerp_rotation_positive_direction_Z = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                //Rotate 
                if (moving_parts_dict[id].lerp_rotation_positive_direction_Z)
                {
                    ECS.SetRotation(id, Vector3.Lerp(rot, new Vector3(rot.X, rot.Y, moving_parts_dict[id].lerp_rotation_upper_limit.Z), moving_parts_dict[id].lerp_rotation_speed_Z * dt));
                }
                else
                {
                    ECS.SetRotation(id, Vector3.Lerp(rot, new Vector3(rot.X, rot.Y, moving_parts_dict[id].lerp_rotation_lower_limit.Z), moving_parts_dict[id].lerp_rotation_speed_Z * dt));
                }
            }
        }

        void SpinObjectEndless(uint id, float x_axis, float y_axis, float z_axis, float spin_speed, float dt)
        {
            if (moving_parts_dict[id].toggle_spin)
            {
                moving_parts_dict[id].spin_rotation.X += x_axis * spin_speed * dt;
                moving_parts_dict[id].spin_rotation.Y += y_axis * spin_speed * dt;
                moving_parts_dict[id].spin_rotation.Z += z_axis * spin_speed * dt;

                ClampRotationValue(ref moving_parts_dict[id].spin_rotation.X, -360.0f, 360.0f);
                ClampRotationValue(ref moving_parts_dict[id].spin_rotation.Y, -360.0f, 360.0f);
                ClampRotationValue(ref moving_parts_dict[id].spin_rotation.Z, -360.0f, 360.0f);

                ECS.SetRotation(id, moving_parts_dict[id].spin_rotation);
            }
        }

        void StopObjectSpin(uint id)
        {
            moving_parts_dict[id].toggle_spin = false;
        }
        #endregion

        float CheckNearest(float value, float comp1, float comp2)
        {
            return Math.Abs(comp1 - value) > Math.Abs(comp2 - value) ? comp2 : comp1;
        }

        void ClampRotationValue(ref float value, float min, float max)
        {
            if (value <= min)
                value = 0.0f;
            else if (value >= max)
                value = 0.0f;
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

        #region[Moving Animation Sequence]

        /// <summary>
        /// [Moving State] Boss swaying around and flailing its arms
        /// </summary>
        void SetMovingStateAnimations()
        {
            //Body
            SetMovingPartsPosition(entityID, new Vector3(-0.2f, 0, 0), new Vector3(0.2f, 0, 0), new Vector3(1.0f, 0, 0), true, false, false, true, false, false);
            SetMovingPartsRotation(entityID, new Vector3(0, 0, -4), new Vector3(0, 0, 4), new Vector3(0, 0, 3.0f), false, false, true, false, false, true);

            //Arms
            SetMovingPartsRotation(left_arm_middle_joint_id, new Vector3(-90, -10, 0), new Vector3(0, 10, 10), new Vector3(10.0f, 5.0f, 0), false, false, true, false, true, false);
            SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, -85), new Vector3(0, 0, -65), new Vector3(0, 0, 5.0f), false, false, false, false, false, true);
            SetMovingPartsRotation(right_arm_middle_joint_id, new Vector3(-90, -10, 0), new Vector3(0, 10, 10), new Vector3(10.0f, 5.0f, 0), false, false, true, false, true, false);
            SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 65), new Vector3(0, 0, 85), new Vector3(0.0f, 0.0f, 5.0f), false, false, false, false, false, true);

            //Legs
            SetMovingPartsRotation(left_leg_middle_joint_id, new Vector3(-90, 0, -35), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
            SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, 50), new Vector3(0, 0, 100), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
            SetMovingPartsRotation(right_leg_middle_joint_id, new Vector3(-90, 0, 0), new Vector3(0, 0, 35), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
            SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, -100), new Vector3(0, 0, -50), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);

            //Launching bays
            SetMovingPartsRotation(right_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(right_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(right_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            SetMovingPartsRotation(left_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(left_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(left_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            //Mouth
            SetMovingPartsRotation(mouth_left_id, new Vector3(0, -35, 0), new Vector3(), new Vector3(0, 10.0f, 0), false, true, false, false, true, false);
            SetMovingPartsRotation(mouth_right_id, new Vector3(), new Vector3(0, 35, 0), new Vector3(0, 10.0f, 0), false, false, false, false, true, false);

            //Artillery
            SetMovingPartsRotation(artillery_axis_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsPosition(artillery_barrel_id, new Vector3(), new Vector3(0, 10.3f, 0), new Vector3(10.0f, 10.0f, 10.0f), false, true, false, false, false, false);

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);
        }

        void RunMovingSequence(float dt)
        {
            //Set the boss in a moving state where the arms and legs flail a little bit

            UpdateMovingParts(entityID, dt);

            //Arms
            UpdateMovingParts(left_arm_middle_joint_id, dt);
            UpdateMovingParts(left_arm_end_joint_id, dt);
            UpdateMovingParts(right_arm_middle_joint_id, dt);
            UpdateMovingParts(right_arm_end_joint_id, dt);

            //Arm Lasers
            UpdateMovingParts(left_large_laser_spin_id, dt);
            UpdateMovingParts(right_large_laser_spin_id, dt);

            //Legs
            UpdateMovingParts(left_leg_middle_joint_id, dt);
            UpdateMovingParts(left_leg_end_joint_id, dt);
            UpdateMovingParts(right_leg_middle_joint_id, dt);
            UpdateMovingParts(right_leg_end_joint_id, dt);

            //Launching bays
            UpdateMovingParts(right_launching_bay_one_id, dt);
            UpdateMovingParts(right_launching_bay_two_id, dt);
            UpdateMovingParts(right_launching_bay_three_id, dt);

            UpdateMovingParts(left_launching_bay_one_id, dt);
            UpdateMovingParts(left_launching_bay_two_id, dt);
            UpdateMovingParts(left_launching_bay_three_id, dt);

            //Artillery
            UpdateMovingParts(artillery_axis_id, dt);
            UpdateMovingParts(artillery_barrel_id, dt);

            //Mouth
            UpdateMovingParts(mouth_left_id, dt);
            UpdateMovingParts(mouth_right_id, dt);
        }
        #endregion

        #region[Launch Animation Sequence]

        /// <summary>
        /// [Launch Normal Enemy State]
        /// State 1: Arms rotate and arm ends tilt 90 degrees
        /// State 2: Boss tilts and arm rotate downwards
        /// State 3: Open launch bays and spawns enemies
        /// </summary>

        void SetLaunchStateAnimationsOne()
        {
            //Body
            SetMovingPartsPosition(entityID, new Vector3(), new Vector3(), new Vector3(1.0f, 0, 0), true, false, false, false, false, false);
            SetMovingPartsRotation(entityID, new Vector3(), new Vector3(), new Vector3(0, 0, 3.0f), false, false, true, false, false, false);

            //Arms
            SetMovingPartsRotation(left_arm_middle_joint_id, new Vector3(-160.0f, 0, -30.0f), new Vector3(), new Vector3(10.0f, 0, 1.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, -95.0f), new Vector3(0, 0, 0), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(right_arm_middle_joint_id, new Vector3(-160.0f, 0, 0), new Vector3(0, 0, 30.0f), new Vector3(10.0f, 0, 1.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 0), new Vector3(0, 0, 95.0f), new Vector3(0.0f, 0.0f, 5.0f), false, false, true, false, false, false);

            //Legs
            SetMovingPartsRotation(left_leg_middle_joint_id, new Vector3(-90.0f, 0, -35.0f), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, 50.0f), new Vector3(0, 0, 100.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(right_leg_middle_joint_id, new Vector3(-90.0f, 0, 0), new Vector3(0, 0, 35.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, -100.0f), new Vector3(0, 0, -50.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);

            //Launching bays
            SetMovingPartsRotation(right_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(right_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(right_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            SetMovingPartsRotation(left_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(left_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(left_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            //Mouth
            SetMovingPartsRotation(mouth_left_id, new Vector3(0, -15.0f, 0), new Vector3(), new Vector3(0, 10.0f, 0), false, true, false, false, true, false);
            SetMovingPartsRotation(mouth_right_id, new Vector3(), new Vector3(0, 15.0f, 0), new Vector3(0, 10.0f, 0), false, false, false, false, true, false);

            //Artillery
            SetMovingPartsRotation(artillery_axis_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsPosition(artillery_barrel_id, new Vector3(), new Vector3(0, 10.3f, 0), new Vector3(10.0f, 10.0f, 10.0f), false, true, false, false, false, false);
        }

        void RunLaunchStateSequenceOne(float dt)
        {
            UpdateMovingParts(entityID, dt);

            //Arms
            UpdateMovingParts(left_arm_middle_joint_id, dt);
            UpdateMovingParts(left_arm_end_joint_id, dt);
            UpdateMovingParts(right_arm_middle_joint_id, dt);
            UpdateMovingParts(right_arm_end_joint_id, dt);

            //Legs
            UpdateMovingParts(left_leg_middle_joint_id, dt);
            UpdateMovingParts(left_leg_end_joint_id, dt);
            UpdateMovingParts(right_leg_middle_joint_id, dt);
            UpdateMovingParts(right_leg_end_joint_id, dt);

            //Launching bays
            UpdateMovingParts(right_launching_bay_one_id, dt);
            UpdateMovingParts(right_launching_bay_two_id, dt);
            UpdateMovingParts(right_launching_bay_three_id, dt);

            UpdateMovingParts(left_launching_bay_one_id, dt);
            UpdateMovingParts(left_launching_bay_two_id, dt);
            UpdateMovingParts(left_launching_bay_three_id, dt);

            //Artillery
            UpdateMovingParts(artillery_axis_id, dt);
            UpdateMovingParts(artillery_barrel_id, dt);

            //Mouth
            UpdateMovingParts(mouth_left_id, dt);
            UpdateMovingParts(mouth_right_id, dt);
        }

        void SetLaunchStateAnimationsTwo()
        {
            //Body
            SetMovingPartsRotation(entityID, new Vector3(), new Vector3(18.0f, 0, 0), new Vector3(5.0f, 0, 0), true, false, false, false, false, false);
            SetMovingPartsPosition(entityID, new Vector3(0, -0.1f, 0), new Vector3(), new Vector3(0, 3.0f, 0), false, false, false, false, false, false);

            //Arms
            SetMovingPartsRotation(left_arm_middle_joint_id, new Vector3(), new Vector3(130.0f, 0, 0), new Vector3(5.0f, 0, 0), false, false, true, false, false, false);
            SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, -95.0f), new Vector3(0, 0, 0), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(right_arm_middle_joint_id, new Vector3(), new Vector3(130.0f, 0, 0), new Vector3(5.0f, 0, 0), false, false, true, false, false, false);
            SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 0), new Vector3(0, 0, 95.0f), new Vector3(0, 0, 5.0f), false, false, true, false, false, false);

            //Legs
            SetMovingPartsRotation(left_leg_middle_joint_id, new Vector3(-90.0f, 0, -35.0f), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, 50.0f), new Vector3(0, 0, 100.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(right_leg_middle_joint_id, new Vector3(-90.0f, 0, 0), new Vector3(0, 0, 35.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, -100.0f), new Vector3(0, 0, -50.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
        }

        void RunLaunchStateSequenceTwo(float dt)
        {
            UpdateMovingParts(entityID, dt);

            //Arms
            UpdateMovingParts(left_arm_middle_joint_id, dt);
            UpdateMovingParts(left_arm_end_joint_id, dt);
            UpdateMovingParts(right_arm_middle_joint_id, dt);
            UpdateMovingParts(right_arm_end_joint_id, dt);

            //Legs
            UpdateMovingParts(left_leg_middle_joint_id, dt);
            UpdateMovingParts(left_leg_end_joint_id, dt);
            UpdateMovingParts(right_leg_middle_joint_id, dt);
            UpdateMovingParts(right_leg_end_joint_id, dt);
        }

        void SetLaunchStateAnimationsThree()
        {
            //Launching bays
            SetMovingPartsRotation(right_launching_bay_one_id, new Vector3(0, 0, -90.0f), new Vector3(), new Vector3(0, 0, 10.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(right_launching_bay_two_id, new Vector3(0, 0, -90.0f), new Vector3(), new Vector3(0, 0, 6.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(right_launching_bay_three_id, new Vector3(0, 0, -90.0f), new Vector3(), new Vector3(0, 0, 2.0f), false, false, false, false, false, false);

            SetMovingPartsRotation(left_launching_bay_one_id, new Vector3(), new Vector3(0, 0, 90.0f), new Vector3(0, 0, 10.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(left_launching_bay_two_id, new Vector3(), new Vector3(0, 0, 90.0f), new Vector3(0, 0, 6.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(left_launching_bay_three_id, new Vector3(), new Vector3(0, 0, 90.0f), new Vector3(0, 0, 2.0f), false, false, true, false, false, false);
        }

        void RunLaunchStateSequenceThree(float dt)
        {
            //Launching bays
            UpdateMovingParts(right_launching_bay_one_id, dt);
            UpdateMovingParts(right_launching_bay_two_id, dt);
            UpdateMovingParts(right_launching_bay_three_id, dt);

            UpdateMovingParts(left_launching_bay_one_id, dt);
            UpdateMovingParts(left_launching_bay_two_id, dt);
            UpdateMovingParts(left_launching_bay_three_id, dt);
        }

        void SetLaunchStateAnimationsFour()
        {
            //Launching bays
            SetMovingPartsRotation(right_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(0, 0, 10.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(right_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(0, 0, 6.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(right_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(0, 0, 2.0f), false, false, true, false, false, false);

            SetMovingPartsRotation(left_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(0, 0, 10.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(left_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(0, 0, 6.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(left_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(0, 0, 2.0f), false, false, true, false, false, false);
        }

        void RunLaunchStateSequenceFour(float dt)
        {
            //Launching bays
            UpdateMovingParts(right_launching_bay_one_id, dt);
            UpdateMovingParts(right_launching_bay_two_id, dt);
            UpdateMovingParts(right_launching_bay_three_id, dt);

            UpdateMovingParts(left_launching_bay_one_id, dt);
            UpdateMovingParts(left_launching_bay_two_id, dt);
            UpdateMovingParts(left_launching_bay_three_id, dt);
        }

        void SetLaunchStateAnimationsFive()
        {
            //Body
            SetMovingPartsRotation(entityID, new Vector3(), new Vector3(), new Vector3(5.0f, 0, 0), false, false, false, false, false, false); ;
            SetMovingPartsPosition(entityID, new Vector3(), new Vector3(0, 0.1f, 0), new Vector3(0, 3.0f, 0), false, true, false, false, false, false);

            //Arms
            SetMovingPartsRotation(left_arm_middle_joint_id, new Vector3(), new Vector3(130.0f, 0, 0), new Vector3(5.0f, 0, 0), false, false, true, false, false, false);
            SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, -95.0f), new Vector3(0, 0, 0), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(right_arm_middle_joint_id, new Vector3(), new Vector3(130.0f, 0, 0), new Vector3(5.0f, 0, 0), false, false, true, false, false, false);
            SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 0), new Vector3(0, 0, 95.0f), new Vector3(0, 0, 5.0f), false, false, true, false, false, false);

            //Legs
            SetMovingPartsRotation(left_leg_middle_joint_id, new Vector3(-90.0f, 0, -35.0f), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, 50.0f), new Vector3(0, 0, 100.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(right_leg_middle_joint_id, new Vector3(-90.0f, 0, 0), new Vector3(0, 0, 35.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, -100.0f), new Vector3(0, 0, -50.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
        }

        void RunLaunchStateSequenceFive(float dt)
        {
            UpdateMovingParts(entityID, dt);

            //Arms
            UpdateMovingParts(left_arm_middle_joint_id, dt);
            UpdateMovingParts(left_arm_end_joint_id, dt);
            UpdateMovingParts(right_arm_middle_joint_id, dt);
            UpdateMovingParts(right_arm_end_joint_id, dt);

            //Legs
            UpdateMovingParts(left_leg_middle_joint_id, dt);
            UpdateMovingParts(left_leg_end_joint_id, dt);
            UpdateMovingParts(right_leg_middle_joint_id, dt);
            UpdateMovingParts(right_leg_end_joint_id, dt);
        }
        #endregion

        #region[Death Animation Sequence]
        /// <summary>
        /// [Death State] 
        /// 1. Boss arms move to aim forward
        /// 2. Boss arms and legs spasms and the main laser starts to appear
        /// </summary>
        void SetDeathStateAnimationsOne()
        {
            //Arms
            SetMovingPartsRotation(left_arm_middle_joint_id, new Vector3(-100.0f, 0, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 0, 0), false, false, false, false, false, false);
            SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, -98.0f), new Vector3(0, 0, -95.0f), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(right_arm_middle_joint_id, new Vector3(-100.0f, 0, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 0, 0), false, false, false, false, false, false);
            SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 95.0f), new Vector3(0, 0, 98.0f), new Vector3(0, 0, 5.0f), false, false, true, false, false, false);

            //Legs
            SetMovingPartsRotation(left_leg_middle_joint_id, new Vector3(0, 0, 0), new Vector3(75.0f, 0, 0), new Vector3(5.0f, 0, 0), true, false, false, false, false, false);
            SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, -90.0f), new Vector3(0, 0, -70.0f), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(right_leg_middle_joint_id, new Vector3(0, 0, 0), new Vector3(75.0f, 0, 0), new Vector3(5.0f, 0, 0), true, false, false, false, false, false);
            SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, 70.0f), new Vector3(0, 0, 90.0f), new Vector3(0, 0, 5.0f), false, false, true, false, false, false);

            //Launching bays
            SetMovingPartsRotation(right_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(right_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(right_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            SetMovingPartsRotation(left_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(left_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsRotation(left_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            //Mouth
            SetMovingPartsRotation(mouth_left_id, new Vector3(0, -105, 0), new Vector3(), new Vector3(0, 10.0f, 0), false, false, false, false, false, false);
            SetMovingPartsRotation(mouth_right_id, new Vector3(), new Vector3(0, 105, 0), new Vector3(0, 10.0f, 0), false, true, false, false, false, false);

            //Artillery
            SetMovingPartsRotation(artillery_axis_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            SetMovingPartsPosition(artillery_barrel_id, new Vector3(), new Vector3(0, 10.3f, 0), new Vector3(10.0f, 10.0f, 10.0f), false, true, false, false, false, false);

            ECS.SetActive(false_core_id, false);
        }

        void RunDeathStateSequenceOne(float dt)
        {
            //Arms
            UpdateMovingParts(left_arm_middle_joint_id, dt);
            UpdateMovingParts(left_arm_end_joint_id, dt);
            UpdateMovingParts(right_arm_middle_joint_id, dt);
            UpdateMovingParts(right_arm_end_joint_id, dt);

            //Legs
            UpdateMovingParts(left_leg_middle_joint_id, dt);
            UpdateMovingParts(left_leg_end_joint_id, dt);
            UpdateMovingParts(right_leg_middle_joint_id, dt);
            UpdateMovingParts(right_leg_end_joint_id, dt);

            //Launching bays
            UpdateMovingParts(right_launching_bay_one_id, dt);
            UpdateMovingParts(right_launching_bay_two_id, dt);
            UpdateMovingParts(right_launching_bay_three_id, dt);

            UpdateMovingParts(left_launching_bay_one_id, dt);
            UpdateMovingParts(left_launching_bay_two_id, dt);
            UpdateMovingParts(left_launching_bay_three_id, dt);

            //Artillery
            UpdateMovingParts(artillery_axis_id, dt);
            UpdateMovingParts(artillery_barrel_id, dt);

            //Mouth
            UpdateMovingParts(mouth_left_id, dt);
            UpdateMovingParts(mouth_right_id, dt);

            //Laser
            UpdateMovingParts(main_laser_rails_id, dt);
            UpdateMovingParts(main_laser_barrel_id, dt);
        }

        void SetDeathStateAnimationsTwo()
        {
            //Body
            SetMovingPartsPosition(entityID, new Vector3(0, -0.1f, 0), new Vector3(0, 0.1f, 0), new Vector3(0, 5.0f, 0), false, false, false, false, true, false);

            //Arms
            SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, -98.0f), new Vector3(0, 0, -95.0f), new Vector3(0, 0, 55.0f), false, false, false, false, false, true);
            SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 95.0f), new Vector3(0, 0, 98.0f), new Vector3(0, 0, 55.0f), false, false, true, false, false, true);

            //Legs
            SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, -90.0f), new Vector3(0, 0, -70.0f), new Vector3(0, 0, 55.0f), false, false, false, false, false, true);
            SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, 70.0f), new Vector3(0, 0, 90.0f), new Vector3(0, 0, 55.0f), false, false, true, false, false, true);

            //Main Laser
            ECS.SetActive(main_laser_rail_vent_id, false);
            SetMovingPartsPosition(main_laser_rails_id, new Vector3(), new Vector3(0, 0, 0.802f), new Vector3(0, 0, 0.5f), false, false, true, false, false, false);
            SetMovingPartsPosition(main_laser_barrel_id, new Vector3(), new Vector3(0, 0, 1.344f), new Vector3(0, 0, 0.25f), false, false, true, false, false, false);

            moving_parts_dict[main_laser_barrel_id].SetToggleSpin(true);
        }

        void RunDeathStateSequenceTwo(float dt)
        {
            UpdateMovingParts(entityID, dt);

            //Arms
            UpdateMovingParts(left_arm_end_joint_id, dt);
            UpdateMovingParts(right_arm_end_joint_id, dt);

            //Legs
            UpdateMovingParts(left_leg_end_joint_id, dt);
            UpdateMovingParts(right_leg_end_joint_id, dt);

            //Artillery
            UpdateMovingParts(artillery_axis_id, dt);
            UpdateMovingParts(artillery_barrel_id, dt);

            //Laser
            UpdateMovingParts(main_laser_rails_id, dt);
            UpdateMovingParts(main_laser_barrel_id, dt);

            SpinObjectEndless(main_laser_barrel_id, 0, 0, 1.0f, 100.0f, dt);
            SpinObjectEndless(left_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);
            SpinObjectEndless(right_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);
        }

        void SetDeathStateAnimationsThree()
        {
            //Body
            SetMovingPartsPosition(entityID, new Vector3(0, 0, -5), new Vector3(0, 0, 0), new Vector3(0, 0, 30.0f), false, false, false, false, false, false);

            moving_parts_dict[main_laser_barrel_id].SetToggleSpin(true);
        }

        void RunDeathStateSequenceThree(float dt)
        {
            UpdateMovingParts(entityID, dt);

            SpinObjectEndless(main_laser_barrel_id, 0, 0, 1.0f, 100.0f, dt);
            SpinObjectEndless(left_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);
            SpinObjectEndless(right_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);
        }
        #endregion
    }
}
