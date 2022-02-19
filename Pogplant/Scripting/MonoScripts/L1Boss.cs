using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class L1Boss : MonoBehaviour
    {
        static public L1Boss m_singleton;
        public Dictionary<uint, MovingParts> moving_parts_dict;

        public enum BOSS_BEHAVIOUR_STATE
        {
            EMPTY,
            SPINNING_UP,
            FLYING_UP,
            MOVING,
            PROTECTION,
            SPINNING_ATTACK,
            LAUNCH_NORMAL_ADDS,
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

        //Main Laser
        public uint main_laser_rails_id;
        public uint main_laser_rail_vent_id;
        public uint main_laser_barrel_id;
        public uint main_laser_beam_id;
        public uint main_laser_object_id;

        //Mouth
        public uint mouth_left_id;
        public uint mouth_right_id;

        //Shield
        public float health;
        uint false_core_id;

        //Laser
        float laser_spin_addition;
        const float laser_spin_addition_speed = 10.0f;

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

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            m_singleton = this;
        }

        public override void Start()
        {
            health = 5;
            fly_up_delay = 7.0f;
            boss_animation_system = new AnimationSystem();
            boss_animation_system.Init();
            FindEntities();
        }

        void FindEntities()
        {
            end_screen_trigger_id = ECS.FindEntityWithName("L1 Black Screen");

            //Player ship
            player_ship_id = ECS.FindEntityWithName("PlayerBox");

            //Core
            false_core_id = ECS.FindEntityWithName("FalseCore");
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
            main_laser_beam_id = ECS.FindEntityWithName("Laser_Beam");
            main_laser_object_id = ECS.FindEntityWithName("LaserObject");

            //Core
            false_core_id = ECS.FindEntityWithName("FalseCore");

            //Create and initialize the list of moving parts
            moving_parts_dict = new Dictionary<uint, MovingParts>();

            //Boss body
            moving_parts_dict.Add(entityID, new MovingParts());

            //Core
            moving_parts_dict.Add(false_core_id, new MovingParts());
            moving_parts_dict.Add(mouth_left_id, new MovingParts());
            moving_parts_dict.Add(mouth_right_id, new MovingParts());

            //Arms
            moving_parts_dict.Add(left_arm_middle_joint_id, new MovingParts());
            moving_parts_dict.Add(left_arm_end_joint_id, new MovingParts());
            moving_parts_dict.Add(right_arm_middle_joint_id, new MovingParts());
            moving_parts_dict.Add(right_arm_end_joint_id, new MovingParts());

            //Arm Lasers
            moving_parts_dict.Add(left_large_laser_spin_id, new MovingParts());
            moving_parts_dict.Add(right_large_laser_spin_id, new MovingParts());

            //Legs
            moving_parts_dict.Add(left_leg_middle_joint_id, new MovingParts());
            moving_parts_dict.Add(right_leg_middle_joint_id, new MovingParts());
            moving_parts_dict.Add(left_leg_end_joint_id, new MovingParts());
            moving_parts_dict.Add(right_leg_end_joint_id, new MovingParts());

            //Launching bays
            moving_parts_dict.Add(right_launching_bay_one_id, new MovingParts());
            moving_parts_dict.Add(right_launching_bay_two_id, new MovingParts());
            moving_parts_dict.Add(right_launching_bay_three_id, new MovingParts());

            moving_parts_dict.Add(left_launching_bay_one_id, new MovingParts());
            moving_parts_dict.Add(left_launching_bay_two_id, new MovingParts());
            moving_parts_dict.Add(left_launching_bay_three_id, new MovingParts());

            //Artillery
            moving_parts_dict.Add(artillery_axis_id, new MovingParts());
            moving_parts_dict.Add(artillery_barrel_id, new MovingParts());

            //Main Laser
            moving_parts_dict.Add(main_laser_rails_id, new MovingParts());
            moving_parts_dict.Add(main_laser_rail_vent_id, new MovingParts());
            moving_parts_dict.Add(main_laser_barrel_id, new MovingParts());
            moving_parts_dict.Add(main_laser_object_id, new MovingParts());

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);

            any_key_scale_up = false;
            any_key_min_scale = new Vector3(0.45f, 0.45f, 1.0f);
            any_key_max_scale = new Vector3(0.5f, 0.5f, 1.0f);

            SetState(BOSS_BEHAVIOUR_STATE.FLYING_UP.ToString());
        }

        public override void Update(float dt)
        {
            moving_parts_dict[left_large_laser_spin_id].SpinObjectEndless(left_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);
            moving_parts_dict[right_large_laser_spin_id].SpinObjectEndless(right_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);

            if (InputUtility.onKeyTriggered(KEY_ID.KEY_J))
            {
                SetState(BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK.ToString());
            }

            switch (current_state)
            {
                case BOSS_BEHAVIOUR_STATE.MOVING:
                    RunMovingSequence(dt);
                    break;
                case BOSS_BEHAVIOUR_STATE.PROTECTION:
                    RunProtectionSequence(dt);
                    break;
                case BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE:
                    laser_spin_addition += laser_spin_addition_speed * dt;
                    moving_parts_dict[main_laser_barrel_id].SpinObjectEndless(main_laser_barrel_id, 0, 0, 1.0f, laser_spin_addition, dt);
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
                case BOSS_BEHAVIOUR_STATE.SPINNING_UP:

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
                    SetMovingStateAnimations();
                    break;
                case BOSS_BEHAVIOUR_STATE.PROTECTION:
                    SetProtectionStateAnimations();
                    break;
                case BOSS_BEHAVIOUR_STATE.SPINNING_ATTACK:
                    fly_up_delay = fly_up_short_delay;
                    boss_animation_system.StopAnimation(true, moving_parts_dict);
                    boss_animation_system.AddAnimationSpecsStack(SetSpinningStateAnimationsOne, 2.0f);
                    boss_animation_system.AddAnimationSpecsStack(SetSpinningStateAnimationsTwo, 1.5f);
                    boss_animation_system.AddAnimationSpecsStack(SetSpinningStateAnimationsThree, 1.5f);
                    boss_animation_system.AddAnimationSpecsStack(SetSpinningStateAnimationsFour, 1.5f);
                    boss_animation_system.AddAnimationSpecsStack(SetSpinningStateAnimationsFive, 1.5f);
                    boss_animation_system.AddAnimationUpdateStack(RunSpinningStateAnimationsOne);
                    boss_animation_system.AddAnimationUpdateStack(RunSpinningStateAnimationsTwo);
                    boss_animation_system.AddAnimationUpdateStack(RunSpinningStateAnimationsThree);
                    boss_animation_system.AddAnimationUpdateStack(RunSpinningStateAnimationsFour);
                    boss_animation_system.AddAnimationUpdateStack(RunSpinningStateAnimationsFive);
                    boss_animation_system.SetStateQueue(SetState, BOSS_BEHAVIOUR_STATE.FLYING_UP.ToString());
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
                case BOSS_BEHAVIOUR_STATE.DEATH_SEQUENCE:
                    boss_animation_system.StopAnimation(true, moving_parts_dict);
                    boss_animation_system.AddAnimationSpecsStack(SetDeathStateAnimationsOne, 1.5f);
                    boss_animation_system.AddAnimationSpecsStack(SetDeathStateAnimationsTwo, 7.0f);
                    boss_animation_system.AddAnimationSpecsStack(SetDeathStateAnimationsThree, 2.0f);
                    boss_animation_system.AddAnimationSpecsStack(SetDeathStateAnimationsFour, 2.0f);
                    boss_animation_system.AddAnimationSpecsStack(SetDeathStateAnimationsFive, 0.5f);
                    boss_animation_system.AddAnimationUpdateStack(RunDeathStateSequenceOne);
                    boss_animation_system.AddAnimationUpdateStack(RunDeathStateSequenceTwo);
                    boss_animation_system.AddAnimationUpdateStack(RunDeathStateSequenceThree);
                    boss_animation_system.AddAnimationUpdateStack(RunDeathStateSequenceFour);
                    boss_animation_system.AddAnimationUpdateStack(RunDeathStateSequenceFive);
                    boss_animation_system.SetStateQueue(SetState, BOSS_BEHAVIOUR_STATE.TRANSIT_SCENE.ToString());
                    boss_animation_system.PlayAnimation();
                    break;
                case BOSS_BEHAVIOUR_STATE.TRANSIT_SCENE:
                    ECS.SetActive(any_key_continue_id, true);
                    break;
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
            Console.WriteLine(scale.X);

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
            moving_parts_dict[entityID].SetMovingPartsPosition(entityID, new Vector3(0, -50, 0), new Vector3(0, 5, 0), new Vector3(0, 4.0f, 0), false, true, false, false, false, false);
            ECS.SetRotation(entityID, new Vector3(-90, 0, 0));
            moving_parts_dict[entityID].SetMovingPartsRotation(entityID, new Vector3(-90, 0, 0), new Vector3(0, 0, 0), new Vector3(1.0f, 0, 0), true, false, false, false, false, false);
        }

        void SetFlyingUpStateAnimationsThree()
        {
            moving_parts_dict[entityID].SetMovingPartsPosition(entityID, new Vector3(0, 0, 0), new Vector3(0, 5, 0), new Vector3(0, 2.0f, 0), false, false, false, false, false, false);
            moving_parts_dict[entityID].SetMovingPartsRotation(entityID, new Vector3(ECS.GetGlobalRotation(entityID).X, 0, 0), new Vector3(0, 0, 0), new Vector3(2.0f, 0, 0), true, false, false, false, false, false);
        }

        void RunFlyingUpSequenceOne(float dt)
        {
            //Nothing as used for a delay
        }

        void RunFlyingUpSequenceTwo(float dt)
        {
            //Body
            moving_parts_dict[entityID].UpdateMovingParts(entityID, dt);
        }

        void RunFlyingUpSequenceThree(float dt)
        {
            //Body
            moving_parts_dict[entityID].UpdateMovingParts(entityID, dt);
        }

        #endregion

        #region[Moving Animation Sequence]

        /// <summary>
        /// [Moving State] Boss swaying around and flailing its arms
        /// </summary>
        void SetMovingStateAnimations()
        {
            //Body
            moving_parts_dict[entityID].SetMovingPartsPosition(entityID, new Vector3(-0.2f, 0, 0), new Vector3(0.2f, 0, 0), new Vector3(1.0f, 0, 0), true, false, false, true, false, false);
            moving_parts_dict[entityID].SetMovingPartsRotation(entityID, new Vector3(0, 0, -4), new Vector3(0, 0, 4), new Vector3(0, 0, 3.0f), false, false, true, false, false, true);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetMovingPartsRotation(left_arm_middle_joint_id, new Vector3(-90, -10, 0), new Vector3(0, 10, 10), new Vector3(10.0f, 5.0f, 0), false, false, true, false, true, false);
            moving_parts_dict[left_arm_end_joint_id].SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, -85), new Vector3(0, 0, -65), new Vector3(0, 0, 5.0f), false, false, false, false, false, true);
            moving_parts_dict[right_arm_middle_joint_id].SetMovingPartsRotation(right_arm_middle_joint_id, new Vector3(-90, -10, 0), new Vector3(0, 10, 10), new Vector3(10.0f, 5.0f, 0), false, false, true, false, true, false);
            moving_parts_dict[right_arm_end_joint_id].SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 65), new Vector3(0, 0, 85), new Vector3(0.0f, 0.0f, 5.0f), false, false, false, false, false, true);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetMovingPartsRotation(left_leg_middle_joint_id, new Vector3(-90, 0, -35), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
            moving_parts_dict[left_leg_end_joint_id].SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, 50), new Vector3(0, 0, 100), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
            moving_parts_dict[right_leg_middle_joint_id].SetMovingPartsRotation(right_leg_middle_joint_id, new Vector3(-90, 0, 0), new Vector3(0, 0, 35), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
            moving_parts_dict[right_leg_end_joint_id].SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, -100), new Vector3(0, 0, -50), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].SetMovingPartsRotation(right_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_one_id].SetMovingPartsRotation(right_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_three_id].SetMovingPartsRotation(right_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            moving_parts_dict[left_launching_bay_one_id].SetMovingPartsRotation(left_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_two_id].SetMovingPartsRotation(left_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_three_id].SetMovingPartsRotation(left_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            //Mouth
            moving_parts_dict[mouth_left_id].SetMovingPartsRotation(mouth_left_id, new Vector3(0, -45, 0), new Vector3(), new Vector3(0, 2.0f, 0), false, true, false, false, true, false);
            moving_parts_dict[mouth_right_id].SetMovingPartsRotation(mouth_right_id, new Vector3(), new Vector3(0, 45, 0), new Vector3(0, 2.0f, 0), false, false, false, false, true, false);

            //Artillery
            moving_parts_dict[artillery_axis_id].SetMovingPartsRotation(artillery_axis_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[artillery_barrel_id].SetMovingPartsPosition(artillery_barrel_id, new Vector3(), new Vector3(0, 10.3f, 0), new Vector3(10.0f, 10.0f, 10.0f), false, true, false, false, false, false);

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);
        }

        void RunMovingSequence(float dt)
        {
            //Set the boss in a moving state where the arms and legs flail a little bit

            moving_parts_dict[entityID].UpdateMovingParts(entityID, dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(left_arm_middle_joint_id, dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(left_arm_end_joint_id, dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(right_arm_middle_joint_id, dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(right_arm_end_joint_id, dt);

            //Arm Lasers
            moving_parts_dict[left_large_laser_spin_id].UpdateMovingParts(left_large_laser_spin_id, dt);
            moving_parts_dict[right_large_laser_spin_id].UpdateMovingParts(right_large_laser_spin_id, dt);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(left_leg_middle_joint_id, dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(left_leg_end_joint_id, dt);
            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(right_leg_middle_joint_id, dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(right_leg_end_joint_id, dt);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].UpdateMovingParts(right_launching_bay_one_id, dt);
            moving_parts_dict[right_launching_bay_two_id].UpdateMovingParts(right_launching_bay_two_id, dt);
            moving_parts_dict[right_launching_bay_three_id].UpdateMovingParts(right_launching_bay_three_id, dt);

            moving_parts_dict[left_launching_bay_one_id].UpdateMovingParts(left_launching_bay_one_id, dt);
            moving_parts_dict[left_launching_bay_two_id].UpdateMovingParts(left_launching_bay_two_id, dt);
            moving_parts_dict[left_launching_bay_three_id].UpdateMovingParts(left_launching_bay_three_id, dt);

            //Artillery
            moving_parts_dict[artillery_axis_id].UpdateMovingParts(artillery_axis_id, dt);
            moving_parts_dict[artillery_barrel_id].UpdateMovingParts(artillery_barrel_id, dt);

            //Mouth
            moving_parts_dict[mouth_left_id].UpdateMovingParts(mouth_left_id, dt);
            moving_parts_dict[mouth_right_id].UpdateMovingParts(mouth_right_id, dt);
        }
        #endregion

        #region[Protection Animation Sequence]

        /// <summary>
        /// [Protection State] Boss swaying around and flailing its arms
        /// </summary>
        void SetProtectionStateAnimations()
        {
            //Body
            moving_parts_dict[entityID].SetMovingPartsPosition(entityID, new Vector3(-0.2f, 0, 0), new Vector3(0.2f, 0, 0), new Vector3(1.0f, 0, 0), true, false, false, true, false, false);
            moving_parts_dict[entityID].SetMovingPartsRotation(entityID, new Vector3(0, 0, -4), new Vector3(0, 0, 4), new Vector3(0, 0, 3.0f), false, false, true, false, false, true);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetMovingPartsRotation(left_arm_middle_joint_id, new Vector3(-90, -10, 0), new Vector3(0, 10, 10), new Vector3(10.0f, 5.0f, 0), false, false, true, false, true, false);
            moving_parts_dict[left_arm_end_joint_id].SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, -85), new Vector3(0, 0, -65), new Vector3(0, 0, 5.0f), false, false, false, false, false, true);
            moving_parts_dict[right_arm_middle_joint_id].SetMovingPartsRotation(right_arm_middle_joint_id, new Vector3(-90, -10, 0), new Vector3(0, 10, 10), new Vector3(10.0f, 5.0f, 0), false, false, true, false, true, false);
            moving_parts_dict[right_arm_end_joint_id].SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 65), new Vector3(0, 0, 85), new Vector3(0.0f, 0.0f, 5.0f), false, false, false, false, false, true);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetMovingPartsRotation(left_leg_middle_joint_id, new Vector3(-90, 0, -35), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
            moving_parts_dict[left_leg_end_joint_id].SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, 50), new Vector3(0, 0, 100), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
            moving_parts_dict[right_leg_middle_joint_id].SetMovingPartsRotation(right_leg_middle_joint_id, new Vector3(-90, 0, 0), new Vector3(0, 0, 35), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);
            moving_parts_dict[right_leg_end_joint_id].SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, -100), new Vector3(0, 0, -50), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, true);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].SetMovingPartsRotation(right_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_two_id].SetMovingPartsRotation(right_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_three_id].SetMovingPartsRotation(right_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            moving_parts_dict[left_launching_bay_one_id].SetMovingPartsRotation(left_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_two_id].SetMovingPartsRotation(left_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_three_id].SetMovingPartsRotation(left_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            //Mouth
            moving_parts_dict[mouth_left_id].SetMovingPartsRotation(mouth_left_id, new Vector3(0, -45, 0), new Vector3(), new Vector3(0, 10.0f, 0), false, true, false, false, false, false);
            moving_parts_dict[mouth_right_id].SetMovingPartsRotation(mouth_right_id, new Vector3(), new Vector3(0, 45, 0), new Vector3(0, 10.0f, 0), false, false, false, false, false, false);

            //Artillery
            moving_parts_dict[artillery_axis_id].SetMovingPartsRotation(artillery_axis_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[artillery_barrel_id].SetMovingPartsPosition(artillery_barrel_id, new Vector3(), new Vector3(0, 10.3f, 0), new Vector3(10.0f, 10.0f, 10.0f), false, true, false, false, false, false);

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);
        }

        void RunProtectionSequence(float dt)
        {
            //Set the boss in a moving state where the arms and legs flail a little bit

            moving_parts_dict[entityID].UpdateMovingParts(entityID, dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(left_arm_middle_joint_id, dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(left_arm_end_joint_id, dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(right_arm_middle_joint_id, dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(right_arm_end_joint_id, dt);

            //Arm Lasers
            moving_parts_dict[left_large_laser_spin_id].UpdateMovingParts(left_large_laser_spin_id, dt);
            moving_parts_dict[right_large_laser_spin_id].UpdateMovingParts(right_large_laser_spin_id, dt);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(left_leg_middle_joint_id, dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(left_leg_end_joint_id, dt);
            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(right_leg_middle_joint_id, dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(right_leg_end_joint_id, dt);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].UpdateMovingParts(right_launching_bay_one_id, dt);
            moving_parts_dict[right_launching_bay_two_id].UpdateMovingParts(right_launching_bay_two_id, dt);
            moving_parts_dict[right_launching_bay_three_id].UpdateMovingParts(right_launching_bay_three_id, dt);

            moving_parts_dict[left_launching_bay_one_id].UpdateMovingParts(left_launching_bay_one_id, dt);
            moving_parts_dict[left_launching_bay_two_id].UpdateMovingParts(left_launching_bay_two_id, dt);
            moving_parts_dict[left_launching_bay_three_id].UpdateMovingParts(left_launching_bay_three_id, dt);

            //Artillery
            moving_parts_dict[artillery_axis_id].UpdateMovingParts(artillery_axis_id, dt);
            moving_parts_dict[artillery_barrel_id].UpdateMovingParts(artillery_barrel_id, dt);

            //Mouth
            moving_parts_dict[mouth_left_id].UpdateMovingParts(mouth_left_id, dt);
            moving_parts_dict[mouth_right_id].UpdateMovingParts(mouth_right_id, dt);
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
            moving_parts_dict[entityID].SetMovingPartsPosition(entityID, new Vector3(), new Vector3(), new Vector3(1.0f, 0, 0), true, false, false, false, false, false);
            moving_parts_dict[entityID].SetMovingPartsRotation(entityID, new Vector3(), new Vector3(), new Vector3(0, 0, 3.0f), false, false, true, false, false, false);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetMovingPartsRotation(left_arm_middle_joint_id, new Vector3(-160.0f, 0, -30.0f), new Vector3(), new Vector3(10.0f, 10.0f, 1.0f), false, false, false, false, false, false);
            moving_parts_dict[left_arm_end_joint_id].SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, -95.0f), new Vector3(0, 0, 0), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            moving_parts_dict[right_arm_middle_joint_id].SetMovingPartsRotation(right_arm_middle_joint_id, new Vector3(-160.0f, 0, 0), new Vector3(0, 0, 30.0f), new Vector3(10.0f, 10.0f, 1.0f), false, false, true, false, false, false);
            moving_parts_dict[right_arm_end_joint_id].SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 0), new Vector3(0, 0, 95.0f), new Vector3(0.0f, 0.0f, 5.0f), false, false, true, false, false, false);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetMovingPartsRotation(left_leg_middle_joint_id, new Vector3(-90.0f, 0, -35.0f), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[left_leg_end_joint_id].SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, 50.0f), new Vector3(0, 0, 100.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_middle_joint_id].SetMovingPartsRotation(right_leg_middle_joint_id, new Vector3(-90.0f, 0, 0), new Vector3(0, 0, 35.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_end_joint_id].SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, -100.0f), new Vector3(0, 0, -50.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].SetMovingPartsRotation(right_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_two_id].SetMovingPartsRotation(right_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_three_id].SetMovingPartsRotation(right_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            moving_parts_dict[left_launching_bay_one_id].SetMovingPartsRotation(left_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_two_id].SetMovingPartsRotation(left_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_three_id].SetMovingPartsRotation(left_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            //Mouth
            moving_parts_dict[mouth_left_id].SetMovingPartsRotation(mouth_left_id, new Vector3(0, -15.0f, 0), new Vector3(), new Vector3(0, 10.0f, 0), false, true, false, false, true, false);
            moving_parts_dict[mouth_right_id].SetMovingPartsRotation(mouth_right_id, new Vector3(), new Vector3(0, 15.0f, 0), new Vector3(0, 10.0f, 0), false, false, false, false, true, false);

            //Artillery
            moving_parts_dict[artillery_axis_id].SetMovingPartsRotation(artillery_axis_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[artillery_barrel_id].SetMovingPartsPosition(artillery_barrel_id, new Vector3(), new Vector3(0, 10.3f, 0), new Vector3(10.0f, 10.0f, 10.0f), false, true, false, false, false, false);
        }

        void RunLaunchStateSequenceOne(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(entityID, dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(left_arm_middle_joint_id, dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(left_arm_end_joint_id, dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(right_arm_middle_joint_id, dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(right_arm_end_joint_id, dt);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(left_leg_middle_joint_id, dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(left_leg_end_joint_id, dt);
            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(right_leg_middle_joint_id, dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(right_leg_end_joint_id, dt);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].UpdateMovingParts(right_launching_bay_one_id, dt);
            moving_parts_dict[right_launching_bay_two_id].UpdateMovingParts(right_launching_bay_two_id, dt);
            moving_parts_dict[right_launching_bay_three_id].UpdateMovingParts(right_launching_bay_three_id, dt);

            moving_parts_dict[left_launching_bay_one_id].UpdateMovingParts(left_launching_bay_one_id, dt);
            moving_parts_dict[left_launching_bay_two_id].UpdateMovingParts(left_launching_bay_two_id, dt);
            moving_parts_dict[left_launching_bay_three_id].UpdateMovingParts(left_launching_bay_three_id, dt);

            //Artillery
            moving_parts_dict[artillery_axis_id].UpdateMovingParts(artillery_axis_id, dt);
            moving_parts_dict[artillery_barrel_id].UpdateMovingParts(artillery_barrel_id, dt);

            //Mouth
            moving_parts_dict[mouth_left_id].UpdateMovingParts(mouth_left_id, dt);
            moving_parts_dict[mouth_right_id].UpdateMovingParts(mouth_right_id, dt);
        }

        void SetLaunchStateAnimationsTwo()
        {
            //Body
            moving_parts_dict[entityID].SetMovingPartsRotation(entityID, new Vector3(), new Vector3(18.0f, 0, 0), new Vector3(5.0f, 0, 0), true, false, false, false, false, false);
            moving_parts_dict[entityID].SetMovingPartsPosition(entityID, new Vector3(0, -0.1f, 0), new Vector3(), new Vector3(0, 3.0f, 0), false, false, false, false, false, false);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetMovingPartsRotation(left_arm_middle_joint_id, new Vector3(), new Vector3(130.0f, 0, 0), new Vector3(5.0f, 0, 0), false, false, true, false, false, false);
            moving_parts_dict[left_arm_end_joint_id].SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, -95.0f), new Vector3(0, 0, 0), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            moving_parts_dict[right_arm_middle_joint_id].SetMovingPartsRotation(right_arm_middle_joint_id, new Vector3(), new Vector3(130.0f, 0, 0), new Vector3(5.0f, 0, 0), false, false, true, false, false, false);
            moving_parts_dict[right_arm_end_joint_id].SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 0), new Vector3(0, 0, 95.0f), new Vector3(0, 0, 5.0f), false, false, true, false, false, false);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetMovingPartsRotation(left_leg_middle_joint_id, new Vector3(-90.0f, 0, -35.0f), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[left_leg_end_joint_id].SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, 50.0f), new Vector3(0, 0, 100.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_middle_joint_id].SetMovingPartsRotation(right_leg_middle_joint_id, new Vector3(-90.0f, 0, 0), new Vector3(0, 0, 35.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_end_joint_id].SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, -100.0f), new Vector3(0, 0, -50.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
        }

        void RunLaunchStateSequenceTwo(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(entityID, dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(left_arm_middle_joint_id, dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(left_arm_end_joint_id, dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(right_arm_middle_joint_id, dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(right_arm_end_joint_id, dt);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(left_leg_middle_joint_id, dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(left_leg_end_joint_id, dt);
            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(right_leg_middle_joint_id, dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(right_leg_end_joint_id, dt);
        }

        void SetLaunchStateAnimationsThree()
        {
            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].SetMovingPartsRotation(right_launching_bay_one_id, new Vector3(0, 0, -90.0f), new Vector3(), new Vector3(0, 0, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_two_id].SetMovingPartsRotation(right_launching_bay_two_id, new Vector3(0, 0, -90.0f), new Vector3(), new Vector3(0, 0, 6.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_three_id].SetMovingPartsRotation(right_launching_bay_three_id, new Vector3(0, 0, -90.0f), new Vector3(), new Vector3(0, 0, 2.0f), false, false, false, false, false, false);

            moving_parts_dict[left_launching_bay_one_id].SetMovingPartsRotation(left_launching_bay_one_id, new Vector3(), new Vector3(0, 0, 90.0f), new Vector3(0, 0, 10.0f), false, false, true, false, false, false);
            moving_parts_dict[left_launching_bay_two_id].SetMovingPartsRotation(left_launching_bay_two_id, new Vector3(), new Vector3(0, 0, 90.0f), new Vector3(0, 0, 6.0f), false, false, true, false, false, false);
            moving_parts_dict[left_launching_bay_three_id].SetMovingPartsRotation(left_launching_bay_three_id, new Vector3(), new Vector3(0, 0, 90.0f), new Vector3(0, 0, 2.0f), false, false, true, false, false, false);

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
            moving_parts_dict[right_launching_bay_one_id].UpdateMovingParts(right_launching_bay_one_id, dt);
            moving_parts_dict[right_launching_bay_two_id].UpdateMovingParts(right_launching_bay_two_id, dt);
            moving_parts_dict[right_launching_bay_three_id].UpdateMovingParts(right_launching_bay_three_id, dt);

            moving_parts_dict[left_launching_bay_one_id].UpdateMovingParts(left_launching_bay_one_id, dt);
            moving_parts_dict[left_launching_bay_two_id].UpdateMovingParts(left_launching_bay_two_id, dt);
            moving_parts_dict[left_launching_bay_three_id].UpdateMovingParts(left_launching_bay_three_id, dt);
        }

        void SetLaunchStateAnimationsFour()
        {
            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].SetMovingPartsRotation(right_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(0, 0, 10.0f), false, false, true, false, false, false);
            moving_parts_dict[right_launching_bay_two_id].SetMovingPartsRotation(right_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(0, 0, 6.0f), false, false, true, false, false, false);
            moving_parts_dict[right_launching_bay_three_id].SetMovingPartsRotation(right_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(0, 0, 2.0f), false, false, true, false, false, false);

            moving_parts_dict[left_launching_bay_one_id].SetMovingPartsRotation(left_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(0, 0, 10.0f), false, false, true, false, false, false);
            moving_parts_dict[left_launching_bay_two_id].SetMovingPartsRotation(left_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(0, 0, 6.0f), false, false, true, false, false, false);
            moving_parts_dict[left_launching_bay_three_id].SetMovingPartsRotation(left_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(0, 0, 2.0f), false, false, true, false, false, false);
        }

        void RunLaunchStateSequenceFour(float dt)
        {
            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].UpdateMovingParts(right_launching_bay_one_id, dt);
            moving_parts_dict[right_launching_bay_two_id].UpdateMovingParts(right_launching_bay_two_id, dt);
            moving_parts_dict[right_launching_bay_three_id].UpdateMovingParts(right_launching_bay_three_id, dt);

            moving_parts_dict[left_launching_bay_one_id].UpdateMovingParts(left_launching_bay_one_id, dt);
            moving_parts_dict[left_launching_bay_two_id].UpdateMovingParts(left_launching_bay_two_id, dt);
            moving_parts_dict[left_launching_bay_three_id].UpdateMovingParts(left_launching_bay_three_id, dt);
        }

        void SetLaunchStateAnimationsFive()
        {
            //Body
            moving_parts_dict[entityID].SetMovingPartsRotation(entityID, new Vector3(), new Vector3(), new Vector3(5.0f, 0, 0), false, false, false, false, false, false); ;
            moving_parts_dict[entityID].SetMovingPartsPosition(entityID, new Vector3(), new Vector3(0, 0.1f, 0), new Vector3(0, 3.0f, 0), false, true, false, false, false, false);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetMovingPartsRotation(left_arm_middle_joint_id, new Vector3(), new Vector3(130.0f, 0, 0), new Vector3(5.0f, 0, 0), false, false, true, false, false, false);
            moving_parts_dict[left_arm_end_joint_id].SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, -95.0f), new Vector3(0, 0, 0), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            moving_parts_dict[right_arm_middle_joint_id].SetMovingPartsRotation(right_arm_middle_joint_id, new Vector3(), new Vector3(130.0f, 0, 0), new Vector3(5.0f, 0, 0), false, false, true, false, false, false);
            moving_parts_dict[right_arm_end_joint_id].SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 0), new Vector3(0, 0, 95.0f), new Vector3(0, 0, 5.0f), false, false, true, false, false, false);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetMovingPartsRotation(left_leg_middle_joint_id, new Vector3(-90.0f, 0, -35.0f), new Vector3(0, 0, 0), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[left_leg_end_joint_id].SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, 50.0f), new Vector3(0, 0, 100.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_middle_joint_id].SetMovingPartsRotation(right_leg_middle_joint_id, new Vector3(-90.0f, 0, 0), new Vector3(0, 0, 35.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_end_joint_id].SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, -100.0f), new Vector3(0, 0, -50.0f), new Vector3(10.0f, 15.0f, 5.0f), false, false, true, false, false, false);
        }

        void RunLaunchStateSequenceFive(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(entityID, dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(left_arm_middle_joint_id, dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(left_arm_end_joint_id, dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(right_arm_middle_joint_id, dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(right_arm_end_joint_id, dt);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(left_leg_middle_joint_id, dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(left_leg_end_joint_id, dt);
            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(right_leg_middle_joint_id, dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(right_leg_end_joint_id, dt);
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
            moving_parts_dict[entityID].SetMovingPartsPosition(entityID, new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1.0f, 0, 0), true, false, false, true, false, false);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetMovingPartsRotation(left_arm_middle_joint_id, new Vector3(0, -90.0f, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), true, false, true, false, false, false);
            moving_parts_dict[left_arm_end_joint_id].SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), false, false, false, false, false, false);
            moving_parts_dict[right_arm_middle_joint_id].SetMovingPartsRotation(right_arm_middle_joint_id, new Vector3(0, 0, 0), new Vector3(0, 90.0f, 0), new Vector3(5.0f, 5.0f, 5.0f), true, true, true, false, false, false);
            moving_parts_dict[right_arm_end_joint_id].SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), false, false, false, false, false, false);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetMovingPartsRotation(left_leg_middle_joint_id, new Vector3(0, 0, 0), new Vector3(0, 90, 0), new Vector3(5.0f, 5.0f, 5.0f), false, true, false, false, false, false);
            moving_parts_dict[left_leg_end_joint_id].SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), false, false, true, false, false, true);
            moving_parts_dict[right_leg_middle_joint_id].SetMovingPartsRotation(right_leg_middle_joint_id, new Vector3(0, -90, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), false, false, true, false, false, false);
            moving_parts_dict[right_leg_end_joint_id].SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 5.0f, 5.0f), false, false, false, false, false, false);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].SetMovingPartsRotation(right_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_one_id].SetMovingPartsRotation(right_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_three_id].SetMovingPartsRotation(right_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            moving_parts_dict[left_launching_bay_one_id].SetMovingPartsRotation(left_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_two_id].SetMovingPartsRotation(left_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_three_id].SetMovingPartsRotation(left_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            //Mouth
            moving_parts_dict[mouth_left_id].SetMovingPartsRotation(mouth_left_id, new Vector3(0, 0, 0), new Vector3(), new Vector3(0, 10.0f, 0), false, true, false, false, true, false);
            moving_parts_dict[mouth_right_id].SetMovingPartsRotation(mouth_right_id, new Vector3(), new Vector3(0, 0, 0), new Vector3(0, 10.0f, 0), false, false, false, false, true, false);

            //Artillery
            moving_parts_dict[artillery_axis_id].SetMovingPartsRotation(artillery_axis_id, new Vector3(), new Vector3(61, 0, 0), new Vector3(10.0f, 0, 0), true, false, false, false, false, false);
            moving_parts_dict[artillery_barrel_id].SetMovingPartsPosition(artillery_barrel_id, new Vector3(), new Vector3(0, 6.0f, 0), new Vector3(0, 5.0f, 0), false, true, false, false, false, false);

            moving_parts_dict[left_large_laser_spin_id].SetToggleSpin(true);
            moving_parts_dict[right_large_laser_spin_id].SetToggleSpin(true);
        }

        void RunSpinningStateAnimationsOne(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(entityID, dt);

            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(left_arm_middle_joint_id, dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(left_arm_end_joint_id, dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(right_arm_middle_joint_id, dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(right_arm_end_joint_id, dt);

            //Arm Lasers
            moving_parts_dict[left_large_laser_spin_id].UpdateMovingParts(left_large_laser_spin_id, dt);
            moving_parts_dict[right_large_laser_spin_id].UpdateMovingParts(right_large_laser_spin_id, dt);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(left_leg_middle_joint_id, dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(left_leg_end_joint_id, dt);
            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(right_leg_middle_joint_id, dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(right_leg_end_joint_id, dt);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].UpdateMovingParts(right_launching_bay_one_id, dt);
            moving_parts_dict[right_launching_bay_two_id].UpdateMovingParts(right_launching_bay_two_id, dt);
            moving_parts_dict[right_launching_bay_three_id].UpdateMovingParts(right_launching_bay_three_id, dt);

            moving_parts_dict[left_launching_bay_one_id].UpdateMovingParts(left_launching_bay_one_id, dt);
            moving_parts_dict[left_launching_bay_two_id].UpdateMovingParts(left_launching_bay_two_id, dt);
            moving_parts_dict[left_launching_bay_three_id].UpdateMovingParts(left_launching_bay_three_id, dt);

            //Artillery
            moving_parts_dict[artillery_axis_id].UpdateMovingParts(artillery_axis_id, dt);
            moving_parts_dict[artillery_barrel_id].UpdateMovingParts(artillery_barrel_id, dt);

            //Mouth
            moving_parts_dict[mouth_left_id].UpdateMovingParts(mouth_left_id, dt);
            moving_parts_dict[mouth_right_id].UpdateMovingParts(mouth_right_id, dt);
        }

        void SetSpinningStateAnimationsTwo()
        {
            moving_parts_dict[entityID].SetToggleSpin(true);
        }

        void RunSpinningStateAnimationsTwo(float dt)
        {
            moving_parts_dict[entityID].SpinObjectEndless(entityID, 0, 0, 1.0f, 200.0f, dt);
        }

        void SetSpinningStateAnimationsThree()
        {
            
        }

        void RunSpinningStateAnimationsThree(float dt)
        {
            moving_parts_dict[entityID].SpinObjectEndless(entityID, 0, 0, 1.0f, 400.0f, dt);
        }

        void SetSpinningStateAnimationsFour()
        {
            
        }

        void RunSpinningStateAnimationsFour(float dt)
        {
            moving_parts_dict[entityID].SpinObjectEndless(entityID, 0, 0, 1.0f, 800.0f, dt);
        }

        void SetSpinningStateAnimationsFive()
        {
            //Body
            moving_parts_dict[entityID].SetMovingPartsPosition(entityID, new Vector3(0, 0, 0), new Vector3(0, 0, 40), new Vector3(0, 0, 1.25f), false, false, true, false, false, false);
        }

        void RunSpinningStateAnimationsFive(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(entityID, dt);
            moving_parts_dict[entityID].SpinObjectEndless(entityID, 0, 0, 1.0f, 1000.0f, dt);
        }

        #endregion

        #region[Death Animation Sequence]
        /// <summary>
        /// [Death State] 
        /// 1. Boss arms move to aim forward
        /// 2. Boss arms and legs spasms and the main laser starts to appear
        /// 3. Fire recoil
        /// 4. Fire recoil recovery
        /// </summary>
        void SetDeathStateAnimationsOne()
        {
            //Arms
            moving_parts_dict[left_arm_middle_joint_id].SetMovingPartsRotation(left_arm_middle_joint_id, new Vector3(-100.0f, 0, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 0, 0), false, false, false, false, false, false);
            moving_parts_dict[left_arm_end_joint_id].SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, -98.0f), new Vector3(0, 0, -95.0f), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            moving_parts_dict[right_arm_middle_joint_id].SetMovingPartsRotation(right_arm_middle_joint_id, new Vector3(-100.0f, 0, 0), new Vector3(0, 0, 0), new Vector3(5.0f, 0, 0), false, false, false, false, false, false);
            moving_parts_dict[right_arm_end_joint_id].SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 95.0f), new Vector3(0, 0, 98.0f), new Vector3(0, 0, 5.0f), false, false, true, false, false, false);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].SetMovingPartsRotation(left_leg_middle_joint_id, new Vector3(0, 0, 0), new Vector3(75.0f, 0, 0), new Vector3(5.0f, 0, 0), true, false, false, false, false, false);
            moving_parts_dict[left_leg_end_joint_id].SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, -90.0f), new Vector3(0, 0, -70.0f), new Vector3(0, 0, 5.0f), false, false, false, false, false, false);
            moving_parts_dict[right_leg_middle_joint_id].SetMovingPartsRotation(right_leg_middle_joint_id, new Vector3(0, 0, 0), new Vector3(75.0f, 0, 0), new Vector3(5.0f, 0, 0), true, false, false, false, false, false);
            moving_parts_dict[right_leg_end_joint_id].SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, 70.0f), new Vector3(0, 0, 90.0f), new Vector3(0, 0, 5.0f), false, false, true, false, false, false);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].SetMovingPartsRotation(right_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_two_id].SetMovingPartsRotation(right_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[right_launching_bay_three_id].SetMovingPartsRotation(right_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            moving_parts_dict[left_launching_bay_one_id].SetMovingPartsRotation(left_launching_bay_one_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_two_id].SetMovingPartsRotation(left_launching_bay_two_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[left_launching_bay_three_id].SetMovingPartsRotation(left_launching_bay_three_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);

            //Mouth
            moving_parts_dict[mouth_left_id].SetMovingPartsRotation(mouth_left_id, new Vector3(0, -105, 0), new Vector3(), new Vector3(0, 5.0f, 0), false, false, false, false, false, false);
            moving_parts_dict[mouth_right_id].SetMovingPartsRotation(mouth_right_id, new Vector3(), new Vector3(0, 105, 0), new Vector3(0, 5.0f, 0), false, true, false, false, false, false);

            //Artillery
            moving_parts_dict[artillery_axis_id].SetMovingPartsRotation(artillery_axis_id, new Vector3(), new Vector3(), new Vector3(10.0f, 10.0f, 10.0f), false, false, false, false, false, false);
            moving_parts_dict[artillery_barrel_id].SetMovingPartsPosition(artillery_barrel_id, new Vector3(), new Vector3(0, 10.3f, 0), new Vector3(10.0f, 10.0f, 10.0f), false, true, false, false, false, false);

            ECS.SetActive(false_core_id, false);
        }

        void RunDeathStateSequenceOne(float dt)
        {
            //Arms
            moving_parts_dict[left_arm_middle_joint_id].UpdateMovingParts(left_arm_middle_joint_id, dt);
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(left_arm_end_joint_id, dt);
            moving_parts_dict[right_arm_middle_joint_id].UpdateMovingParts(right_arm_middle_joint_id, dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(right_arm_end_joint_id, dt);

            //Legs
            moving_parts_dict[left_leg_middle_joint_id].UpdateMovingParts(left_leg_middle_joint_id, dt);
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(left_leg_end_joint_id, dt);
            moving_parts_dict[right_leg_middle_joint_id].UpdateMovingParts(right_leg_middle_joint_id, dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(right_leg_end_joint_id, dt);

            //Launching bays
            moving_parts_dict[right_launching_bay_one_id].UpdateMovingParts(right_launching_bay_one_id, dt);
            moving_parts_dict[right_launching_bay_two_id].UpdateMovingParts(right_launching_bay_two_id, dt);
            moving_parts_dict[right_launching_bay_three_id].UpdateMovingParts(right_launching_bay_three_id, dt);

            moving_parts_dict[left_launching_bay_one_id].UpdateMovingParts(left_launching_bay_one_id, dt);
            moving_parts_dict[left_launching_bay_two_id].UpdateMovingParts(left_launching_bay_two_id, dt);
            moving_parts_dict[left_launching_bay_three_id].UpdateMovingParts(left_launching_bay_three_id, dt);

            //Artillery
            moving_parts_dict[artillery_axis_id].UpdateMovingParts(artillery_axis_id, dt);
            moving_parts_dict[artillery_barrel_id].UpdateMovingParts(artillery_barrel_id, dt);

            //Mouth
            moving_parts_dict[mouth_left_id].UpdateMovingParts(mouth_left_id, dt);
            moving_parts_dict[mouth_right_id].UpdateMovingParts(mouth_right_id, dt);

            //Laser
            moving_parts_dict[main_laser_rails_id].UpdateMovingParts(main_laser_rails_id, dt);
            moving_parts_dict[main_laser_barrel_id].UpdateMovingParts(main_laser_barrel_id, dt);
        }

        void SetDeathStateAnimationsTwo()
        {
            //Body
            moving_parts_dict[entityID].SetMovingPartsPosition(entityID, new Vector3(0, -0.2f, 0), new Vector3(0, 0.2f, 0), new Vector3(0, 5.0f, 0), false, false, false, false, true, false);

            //Arms
            moving_parts_dict[left_arm_end_joint_id].SetMovingPartsRotation(left_arm_end_joint_id, new Vector3(0, 0, -98.0f), new Vector3(0, 0, -95.0f), new Vector3(0, 0, 55.0f), false, false, false, false, false, true);
            moving_parts_dict[right_arm_end_joint_id].SetMovingPartsRotation(right_arm_end_joint_id, new Vector3(0, 0, 95.0f), new Vector3(0, 0, 98.0f), new Vector3(0, 0, 55.0f), false, false, true, false, false, true);

            //Legs
            moving_parts_dict[left_leg_end_joint_id].SetMovingPartsRotation(left_leg_end_joint_id, new Vector3(0, 0, -90.0f), new Vector3(0, 0, -70.0f), new Vector3(0, 0, 55.0f), false, false, false, false, false, true);
            moving_parts_dict[right_leg_end_joint_id].SetMovingPartsRotation(right_leg_end_joint_id, new Vector3(0, 0, 70.0f), new Vector3(0, 0, 90.0f), new Vector3(0, 0, 55.0f), false, false, true, false, false, true);

            //Main Laser
            ECS.SetActive(main_laser_rail_vent_id, false);
            moving_parts_dict[main_laser_rails_id].SetMovingPartsPosition(main_laser_rails_id, new Vector3(), new Vector3(0, 0, 0.802f), new Vector3(0, 0, 0.5f), false, false, true, false, false, false);
            moving_parts_dict[main_laser_barrel_id].SetMovingPartsPosition(main_laser_barrel_id, new Vector3(), new Vector3(0, 0, 1.344f), new Vector3(0, 0, 0.25f), false, false, true, false, false, false);

            moving_parts_dict[main_laser_barrel_id].SetToggleSpin(true);
        }

        void RunDeathStateSequenceTwo(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(entityID, dt);

            //Arms
            moving_parts_dict[left_arm_end_joint_id].UpdateMovingParts(left_arm_end_joint_id, dt);
            moving_parts_dict[right_arm_end_joint_id].UpdateMovingParts(right_arm_end_joint_id, dt);

            //Legs
            moving_parts_dict[left_leg_end_joint_id].UpdateMovingParts(left_leg_end_joint_id, dt);
            moving_parts_dict[right_leg_end_joint_id].UpdateMovingParts(right_leg_end_joint_id, dt);

            //Artillery
            moving_parts_dict[artillery_axis_id].UpdateMovingParts(artillery_axis_id, dt);
            moving_parts_dict[artillery_barrel_id].UpdateMovingParts(artillery_barrel_id, dt);

            //Laser
            moving_parts_dict[main_laser_rails_id].UpdateMovingParts(main_laser_rails_id, dt);
            moving_parts_dict[main_laser_barrel_id].UpdateMovingParts(main_laser_barrel_id, dt);

            moving_parts_dict[main_laser_barrel_id].SpinObjectEndless(main_laser_barrel_id, 0, 0, 1.0f, 100.0f, dt);
            moving_parts_dict[left_large_laser_spin_id].SpinObjectEndless(left_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);
            moving_parts_dict[right_large_laser_spin_id].SpinObjectEndless(right_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);

            GameUtilities.StartLaser(main_laser_beam_id);
        }

        void SetDeathStateAnimationsThree()
        {
            
        }

        void RunDeathStateSequenceThree(float dt)
        {
            
        }

        void SetDeathStateAnimationsFour()
        {
            //Body
            moving_parts_dict[entityID].SetMovingPartsPosition(entityID, new Vector3(0, -0.2f, -4), new Vector3(0, 0.2f, 0), new Vector3(0, 0.0f, 10.0f), false, false, false, false, true, false);
            moving_parts_dict[main_laser_object_id].SetMovingPartsScale(main_laser_object_id, new Vector3(0.1f, 0.1f, 3.0f), new Vector3(30.0f, 30.0f, 30.0f), new Vector3(2.0f, 2.0f, 0), true, true, false, false, false, false);

            ECS.SetActive(main_laser_rails_id, false);

            moving_parts_dict[main_laser_barrel_id].SetToggleSpin(true);
        }

        void RunDeathStateSequenceFour(float dt)
        {
            //Body
            moving_parts_dict[entityID].UpdateMovingParts(entityID, dt);

            //Laser beam
            moving_parts_dict[main_laser_object_id].UpdateMovingParts(main_laser_object_id, dt);

            moving_parts_dict[main_laser_barrel_id].SpinObjectEndless(main_laser_barrel_id, 0, 0, 1.0f, 100.0f, dt);
            moving_parts_dict[left_large_laser_spin_id].SpinObjectEndless(left_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);
            moving_parts_dict[right_large_laser_spin_id].SpinObjectEndless(right_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);
        }

        void SetDeathStateAnimationsFive()
        {
            //Body
            moving_parts_dict[entityID].SetMovingPartsPosition(entityID, new Vector3(0, -0.2f, 0), new Vector3(0, 0.2f, 2), new Vector3(0, 0.0f, 10.0f), false, false, false, false, true, false);

            //Laser beam
            moving_parts_dict[main_laser_object_id].SetMovingPartsScale(main_laser_object_id, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(30.0f, 30.0f, 30.0f), new Vector3(8.0f, 8.0f, 8.0f), false, false, false, false, false, false);

            moving_parts_dict[main_laser_barrel_id].SetToggleSpin(false);

            ECS.SetActive(end_screen_trigger_id, true);
            SetState(BOSS_BEHAVIOUR_STATE.TRANSIT_SCENE.ToString());
        }

        void RunDeathStateSequenceFive(float dt)
        {
            moving_parts_dict[entityID].UpdateMovingParts(entityID, dt);
            moving_parts_dict[main_laser_object_id].UpdateMovingParts(main_laser_object_id, dt);

            moving_parts_dict[left_large_laser_spin_id].SpinObjectEndless(left_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);
            moving_parts_dict[right_large_laser_spin_id].SpinObjectEndless(right_large_laser_spin_id, 1.0f, 0, 0, 200.0f, dt);
        }
        #endregion
    }
}
