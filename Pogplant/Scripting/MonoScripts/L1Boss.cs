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
        public float lerp_position_speed;
        public Vector3 lerp_position_upper_limit;
        public Vector3 lerp_position_lower_limit;

        public float lerp_rotation_speed;
        public Vector3 lerp_rotation_upper_limit;
        public Vector3 lerp_rotation_lower_limit;

        public bool toggle_spin;
        public Vector3 spin_rotation;

        public bool lerp_position_positive_direction;       //+ve means going towards upper limit, -ve going towards lower limit
        public bool lerp_rotation_positive_direction;

        public bool lerp_position_ping_pong;
        public bool lerp_rotation_ping_pong;

        public bool update_position = false;
        public bool update_rotation = false;

        //Position
        public void SetLerpPosSpeed(float value)
        {
            lerp_position_speed = value;
        }

        public void SetLerpPosUpperLimit(Vector3 value)
        {
            lerp_position_upper_limit = value;
        }

        public void SetLerpPosLowerLimit(Vector3 value)
        {
            lerp_position_lower_limit = value;
        }

        public void SetLerpPosPositiveDirection(bool value)
        {
            lerp_position_positive_direction = value;
        }

        public void SetLerpPosPingPong(bool value)
        {
            lerp_position_ping_pong = value;
        }

        public void SetUpdatePosition(bool value)
        {
            update_position = value;
        }

        //Rotation
        public void SetLerpRotSpeed(float value)
        {
            lerp_rotation_speed = value;
        }

        public void SetLerpRotUpperLimit(Vector3 value)
        {
            lerp_rotation_upper_limit = value;
        }

        public void SetLerpRotLowerLimit(Vector3 value)
        {
            lerp_rotation_lower_limit = value;
        }

        public void SetLerpRotPositiveDirection(bool value)
        {
            lerp_rotation_positive_direction = value;
        }

        public void SetLerpRotPingPong(bool value)
        {
            lerp_rotation_ping_pong = value;
        }

        public void SetUpdateRotation(bool value)
        {
            update_rotation = value;
        }

        public void SetToggleSpin(bool value)
        {
            toggle_spin = value;
        }
    }

    public class L1Boss : MonoBehaviour
    {
        enum BOSS_BEHAVIOUR_STATE
        {
            IDLE,
            MOVING,
            ARM_LASER_ATTACK,
            LEFT_LAUNCH,
            RIGHT_LAUNCH,
            LAUNCH_LASER_ADDS,
            ARTI_ATTACK,
            DEATH_SEQUENCE
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
        //  State vars
        /////////////////////////////////////////////////////////////////////////

        //Moving state


        //Arm laser attack state

        //Left launch state

        //Right launch state

        //Arti attack state


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

        //For ECS get transform
        Vector3 pos = new Vector3();
        Vector3 rot = new Vector3();
        Vector3 scale = new Vector3();

        public override void Init(ref uint _entityID)
        {

        }

        public override void Start()
        {
            FindEntities();

            health = 5;
            is_shield_broken = false;
            ECS.SetActive(black_screen_id, false);
            ECS.SetActive(sparks_particle_id, false);
            rotate_angle = new Vector3(0, 0, 40.0f);
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
        }

        public override void Update(float dt)
        {
            //Testing
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_G))
            {
                SetState(BOSS_BEHAVIOUR_STATE.MOVING);
                //TakeDamage(1);
            }

            switch (current_state)
            {
                case BOSS_BEHAVIOUR_STATE.MOVING:
                    RunMovingSequence(dt);
                    break;
            }

            if (is_shield_broken)
            {
                DeathSequence(dt);
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
            
            
            current_state = BOSS_BEHAVIOUR_STATE.MOVING;

            //Update animation sets based on state
            switch (current_state)
            {
                case BOSS_BEHAVIOUR_STATE.MOVING:
                    SetMovingStateAnimations();
                    break;
            }
        }

        void SetMovingStateAnimations()
        {
            //Arms
            SetMovingPartsRotation(left_arm_middle_joint_id, new Vector3(0, 0, 0), new Vector3(45, 30, 20), 15.0f, true, true);
            SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, 0), new Vector3(45, 15, 35), 15.0f, true, true);
            SetMovingPartsRotation(right_arm_middle_joint_id, new Vector3(0, 0, 0), new Vector3(45, 60, 35), 15.0f, true, true);
            SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 0), new Vector3(45, 80, 20), 15.0f, true, true);

            //Arm lasers
            SetMovingPartsRotation(left_large_laser_spin_id, new Vector3(0, 0, 0), new Vector3(45, 0, 0), 15.0f, true, true);
            SetMovingPartsRotation(right_large_laser_spin_id, new Vector3(0, 0, 0), new Vector3(45, 0, 0), 3.2f, true, true);

            //Legs
            SetMovingPartsRotation(left_leg_middle_joint_id, new Vector3(-3, -3, -3), new Vector3(10, 10, 10), 15.0f, true, true);
            SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(-3, -3, -3), new Vector3(10, 10, 10), 15.0f, true, true);
            SetMovingPartsRotation(right_leg_middle_joint_id, new Vector3(-3, -3, -3), new Vector3(10, 10, 10), 15.0f, true, true);
            SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(-3, -3, -3), new Vector3(10, 10, 10), 15.0f, true, true);

            //Launching bays
            SetMovingPartsRotation(right_launching_bay_one_id, new Vector3(), new Vector3(), 3.2f, true, false);
            SetMovingPartsRotation(right_launching_bay_two_id, new Vector3(), new Vector3(), 3.2f, true, false);
            SetMovingPartsRotation(right_launching_bay_three_id, new Vector3(), new Vector3(), 3.2f, true, false);

            SetMovingPartsRotation(left_launching_bay_one_id, new Vector3(), new Vector3(), 3.2f, true, false);
            SetMovingPartsRotation(left_launching_bay_two_id, new Vector3(), new Vector3(), 3.2f, true, false);
            SetMovingPartsRotation(left_launching_bay_three_id, new Vector3(), new Vector3(), 3.2f, true, false);

            //Artillery
            SetMovingPartsRotation(artillery_axis_id, new Vector3(), new Vector3(-35, 0, 0), 3.2f, true, false);
            SetMovingPartsPosition(artillery_barrel_id, new Vector3(), new Vector3(), 3.2f, true, false);
        }

        void RunMovingSequence(float dt)
        {
            //Set the boss in a moving state where the arms and legs flail a little bit

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
            
            //SpinObjectEndless(left_arm_end_joint_id, new Vector3(-1, 0, 0), 200, dt);
        }

        void SetMovingPartsPosition(uint id, Vector3 set_pos_lower_limit, Vector3 set_pos_upper_limit, float set_lerp_speed, bool set_positive_direction, bool set_ping_pong)
        {
            //Update the moving part's parameters
            moving_parts_dict[id].SetLerpPosSpeed(set_lerp_speed);
            moving_parts_dict[id].SetLerpPosUpperLimit(set_pos_upper_limit);
            moving_parts_dict[id].SetLerpPosLowerLimit(set_pos_lower_limit);
            moving_parts_dict[id].SetLerpPosPingPong(set_ping_pong);
            moving_parts_dict[id].SetLerpPosPositiveDirection(set_positive_direction);

            //Automatically enable since already setting
            moving_parts_dict[id].SetUpdatePosition(true);
        }

        void SetMovingPartsRotation(uint id, Vector3 set_pos_lower_limit, Vector3 set_pos_upper_limit, float set_lerp_speed, bool set_positive_direction, bool set_ping_pong)
        {
            //Update the moving part's parameters
            moving_parts_dict[id].SetLerpRotSpeed(set_lerp_speed);
            moving_parts_dict[id].SetLerpRotUpperLimit(set_pos_upper_limit);
            moving_parts_dict[id].SetLerpRotLowerLimit(set_pos_lower_limit);
            moving_parts_dict[id].SetLerpRotPingPong(set_ping_pong);
            moving_parts_dict[id].SetLerpRotPositiveDirection(set_positive_direction);

            //Automatically enable since already setting
            moving_parts_dict[id].SetUpdateRotation(true);
        }

        void UpdateMovingParts(uint id, float dt)
        {
            //Update position
            if (moving_parts_dict[id].update_position)
            {
                //Keep playing if ping pong
                if (moving_parts_dict[id].lerp_position_ping_pong)
                {
                    //Check where current position is (if below lower limit, animate to upper limit and vice versa) (ONHOLD)

                    ////X axis
                    //if (pos.X < moving_part.lerp_position_lower_limit.X)
                    //{
                    //    ECS.SetPosition(part_id, Vector3.Lerp(pos, new Vector3(moving_part.lerp_position_upper_limit.X, pos.Y, pos.Z), moving_part.lerp_position_speed * dt));
                    //}
                    //else if (pos.X > moving_part.lerp_position_upper_limit.X)
                    //{
                    //    ECS.SetPosition(part_id, Vector3.Lerp(pos, new Vector3(moving_part.lerp_rotation_lower_limit.X, pos.Y, pos.Z), moving_part.lerp_position_speed * dt));
                    //}
                    ////if in between, lerp to closer value
                    //else
                    //{
                    //    ECS.SetPosition(part_id, Vector3.Lerp(pos, new Vector3(CheckNearest(pos.X, moving_part.lerp_position_upper_limit.X, moving_part.lerp_position_lower_limit.X), pos.Y, pos.Z), moving_part.lerp_position_speed * dt));
                    //}

                    ////Y axis
                    //if (pos.Y < moving_part.lerp_position_lower_limit.Y)
                    //{
                    //    ECS.SetPosition(part_id, Vector3.Lerp(pos, new Vector3(pos.X, moving_part.lerp_position_upper_limit.Y, pos.Z), moving_part.lerp_position_speed * dt));
                    //}
                    //else if (pos.Y > moving_part.lerp_position_upper_limit.Y)
                    //{
                    //    ECS.SetPosition(part_id, Vector3.Lerp(pos, new Vector3(pos.X, moving_part.lerp_rotation_lower_limit.Y, pos.Z), moving_part.lerp_position_speed * dt));
                    //}
                    ////if in between, lerp to closer value
                    //else
                    //{
                    //    ECS.SetPosition(part_id, Vector3.Lerp(pos, new Vector3(CheckNearest(pos.X, moving_part.lerp_position_upper_limit.Y, pos.Z), pos.Y, pos.Z), moving_part.lerp_position_speed * dt));
                    //}

                    ////Z axis
                    //if (pos.Z < moving_part.lerp_position_lower_limit.Z)
                    //{
                    //    ECS.SetPosition(part_id, Vector3.Lerp(pos, new Vector3(pos.X, pos.Y, moving_part.lerp_position_upper_limit.Z), moving_part.lerp_position_speed * dt));
                    //}
                    //else if (pos.Z > moving_part.lerp_position_upper_limit.Z)
                    //{
                    //    ECS.SetPosition(part_id, Vector3.Lerp(pos, new Vector3(pos.X, pos.Y, moving_part.lerp_rotation_lower_limit.Z), moving_part.lerp_position_speed * dt));
                    //}
                    ////if in between, lerp to closer value
                    //else
                    //{
                    //    ECS.SetPosition(part_id, Vector3.Lerp(pos, new Vector3(CheckNearest(pos.X, pos.Y, moving_part.lerp_position_lower_limit.Z), pos.Y, pos.Z), moving_part.lerp_position_speed * dt));
                    //}

                    ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                    //Move 
                    if (moving_parts_dict[id].lerp_position_positive_direction)
                    {
                        ECS.SetPosition(id, Vector3.Lerp(pos, moving_parts_dict[id].lerp_position_upper_limit, moving_parts_dict[id].lerp_position_speed * dt));
                        
                        //Check for limit and reverse
                        if (pos.X >= moving_parts_dict[id].lerp_position_upper_limit.X - 0.1f)
                        {
                            moving_parts_dict[id].lerp_position_positive_direction = false;
                        }
                    }
                    else
                    {
                        ECS.SetPosition(id, Vector3.Lerp(pos, moving_parts_dict[id].lerp_position_lower_limit, moving_parts_dict[id].lerp_position_speed * dt));

                        if (pos.X <= moving_parts_dict[id].lerp_position_lower_limit.X + 0.1f)
                        {
                            moving_parts_dict[id].lerp_position_positive_direction = true;
                        }
                    }
                }
                //Play anim once if no ping pong
                else
                {
                    //Move 
                    if (moving_parts_dict[id].lerp_position_positive_direction)
                    {
                        ECS.SetPosition(id, Vector3.Lerp(pos, moving_parts_dict[id].lerp_position_upper_limit, moving_parts_dict[id].lerp_position_speed * dt));
                    }
                    else
                    {
                        ECS.SetPosition(id, Vector3.Lerp(pos, moving_parts_dict[id].lerp_position_lower_limit, moving_parts_dict[id].lerp_position_speed * dt));
                    }
                }
            }

            //Update rotation
            if (moving_parts_dict[id].update_rotation)
            {
                ECS.GetTransformECS(id, ref pos, ref rot, ref scale);

                if (moving_parts_dict[id].lerp_rotation_ping_pong)
                {
                    //Rotate 
                    if (moving_parts_dict[id].lerp_rotation_positive_direction)
                    {
                        ECS.SetRotation(id, Vector3.Lerp(rot, moving_parts_dict[id].lerp_rotation_upper_limit, moving_parts_dict[id].lerp_rotation_speed * dt));

                        //Check for limit and reverse
                        if (rot.X >= moving_parts_dict[id].lerp_rotation_upper_limit.X - 0.1f)
                        {
                            moving_parts_dict[id].lerp_rotation_positive_direction = false;
                        }
                    }
                    else
                    {
                        ECS.SetRotation(id, Vector3.Lerp(rot, moving_parts_dict[id].lerp_rotation_lower_limit, moving_parts_dict[id].lerp_rotation_speed * dt));

                        if (rot.X <= moving_parts_dict[id].lerp_rotation_lower_limit.X + 0.1f)
                        {
                            moving_parts_dict[id].lerp_rotation_positive_direction = true;
                        }
                    }
                }
                else
                {
                    //Rotate 
                    if (moving_parts_dict[id].lerp_rotation_ping_pong)
                    {
                        ECS.SetRotation(id, Vector3.Lerp(rot, moving_parts_dict[id].lerp_rotation_upper_limit, moving_parts_dict[id].lerp_rotation_speed * dt));
                    }
                    else
                    {
                        ECS.SetRotation(id, Vector3.Lerp(rot, moving_parts_dict[id].lerp_rotation_lower_limit, moving_parts_dict[id].lerp_rotation_speed * dt));
                    }
                }
            }
        }

        void SpinObjectEndless(uint id, Vector3 axis, float spin_speed, float dt)
        {
            if (moving_parts_dict[id].toggle_spin)
            {
                moving_parts_dict[id].spin_rotation += axis * spin_speed * dt;
                ClampRotationValue(ref moving_parts_dict[id].spin_rotation.X, -360.0f, 360.0f);
                ClampRotationValue(ref moving_parts_dict[id].spin_rotation.Y, -360.0f, 360.0f);
                ClampRotationValue(ref moving_parts_dict[id].spin_rotation.Z, -360.0f, 360.0f);

                ECS.SetRotation(id, moving_parts_dict[id].spin_rotation);
            }
        }

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
    }
}
