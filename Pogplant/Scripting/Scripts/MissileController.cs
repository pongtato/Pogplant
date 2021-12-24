using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class Missile
    {
        public Missile()
        {
        }

        public Missile(uint _indicator, uint _dropmissile, uint _explosion, uint _entityID)
        {
            m_Indicator = _indicator;
            m_DropMissile = _dropmissile;
            m_Explosion = _explosion;
            m_EntityID = _entityID;

            m_InnerRing = ECS.FindChildEntityWithName(m_Explosion, "InnerRing");
            m_OuterRing = ECS.FindChildEntityWithName(m_Explosion, "OuterRing");
            start_scale = ECS.GetComponent<Transform>(m_Explosion).Scale;
            start_extends = ECS.GetComponent<BoxCollider>(m_Explosion).extends;
            start_centre = ECS.GetComponent<BoxCollider>(m_Explosion).centre;

            End_drop_pos = ECS.GetComponent<Transform>(m_Indicator).Position;

            //Always self-set minimizer scale
            Vector3 start_mini = new Vector3(small_blink_scale, small_blink_scale, small_blink_scale);
            ECS.SetScale(m_Indicator, start_mini);
        }

        public void Start()
        {
            m_Start = true;
        }

        public void Update(float dt)
        {
            if (m_Start && !m_End)
            {
                if (blink_phase1)
                {
                    //Calls blinking stage
                    Phase1(dt);
                }
                if (missle_drop_phase2)
                {
                    //Calls the dropping stage
                    Phase2(dt);
                }
                if (explode_phase3)
                {
                    //Calls the explosion part;
                    Phase3(dt);
                }
            }
        }

        void Phase1(float dt)
        {
            GameUtilities.RunMissilePhase1(m_EntityID , m_Indicator, ref blink_phase1,
           ref  large_blink_scale, ref small_blink_scale, ref accumulated_scale, ref Scale_duration,
           ref start_blinking,  ref blink_phase_dt,  ref accu_dt_blink, ref isBig,  ref missle_drop_phase2,
             dt);

            ////Expand to blink
            //Vector3 large_scale = new Vector3(large_blink_scale, large_blink_scale, large_blink_scale);
            //Vector3 small_scale = new Vector3(small_blink_scale, small_blink_scale, small_blink_scale);

            //if (start_blinking)
            //{
            //    //Expand in 0.5s
            //    accumulated_scale += dt;
            //    if (accumulated_scale >= Scale_duration)
            //    {
            //        accumulated_scale = Scale_duration;
            //        ECS.PlayAudio(m_EntityID, 0);
            //        start_blinking = false;
            //    }
            //    Vector3 scale = Vector3.Lerp(small_scale, large_scale, accumulated_scale / Scale_duration);
            //    ECS.SetScale(m_Indicator, scale);
            //}
            ////Just scale  to big and small
            //if (!start_blinking)
            //{
            //    blink_phase_dt += dt;
            //    accu_dt_blink += dt;
            //    if (accu_dt_blink >= 0.2f)
            //    {
            //        accu_dt_blink = 0.0f;
            //        Vector3 scale = new Vector3(0);
            //        if (isBig)
            //            scale = large_scale;
            //        if (!isBig)
            //            scale = small_scale;
            //        ECS.SetScale(m_Indicator, scale);
            //        isBig = !isBig;
            //    }
            //    if (blink_phase_dt >= 1.5f)
            //    {
            //        //Force set scale to small and initiate second phase
            //        ECS.SetScale(m_Indicator, small_scale);
            //        blink_phase1 = false;
            //        missle_drop_phase2 = true;
            //    }
            //}
        }

        void Phase2(float dt)
        {
            GameUtilities.RunMissilePhase2(m_DropMissile, ref set_missle_start,
            ref missile_scale, ref Start_drop_pos, ref End_drop_pos,
            ref accu_dt_drop, ref missle_drop_speed, ref missle_drop_phase2, ref explode_phase3, dt);

            ////Set missle drop high
            //if (!set_missle_start)
            //{
            //    Vector3 set_Scale = new Vector3(missile_scale, missile_scale, missile_scale);
            //    ECS.SetPosition(m_DropMissile, Start_drop_pos);
            //    ECS.SetScale(m_DropMissile, set_Scale);
            //    set_missle_start = true;
            //}

            //accu_dt_drop += missle_drop_speed * dt;
            ////Iterate the dropping now
            //Vector3 drop_pos = Vector3.Lerp(Start_drop_pos, End_drop_pos, accu_dt_drop);
            //ECS.SetPosition(m_DropMissile, drop_pos);
            ////If the aabb collider min Y only touches touches the bottem, end phase 2 and start phase 3
            //BoxCollider missle_collider = ECS.GetComponent<BoxCollider>(m_DropMissile);
            //Vector3 min = drop_pos - missle_collider.extends; 
            //if (min.Y <= End_drop_pos.Y)
            //{
            //    //Start phase 3 return the missle to unseeable
            //    Vector3 set_end_Scale = new Vector3(0.01f, 0.01f, 0.01f);
            //    ECS.SetScale(m_DropMissile, set_end_Scale);
            //    missle_drop_phase2 = false;
            //    explode_phase3 = true;
            //}
        }

        void Phase3(float dt)
        {
            //NEED TO SEPERATE INNER AND OUTER RING TIMINGS, TO GET BETTER FEEL.
            //TAKES LONGER TIME
            GameUtilities.RunMissilePhase3(m_Explosion, ref final_scale_value, ref  set_explode_start, ref  start_scale,
        ref  accu_dt_expand, ref explosion_expand_multiplier, ref  centre_shift_multiplier, ref  extends_multiplier_Y, ref extends_multiplier_XZ,
        ref  scale_down_dt, ref  scale_down_time, ref explode_phase3, ref  m_End, ref  start_centre, ref  start_extends,  dt);

            //Vector3 Final_scale = new Vector3(final_scale_value);
            //if (!set_explode_start)
            //{
            //    //Assume the trigger is already there, just have to not to step on it on a VERY TINY BOX
            //    Final_scale.Y = 0.0f;
            //    start_scale = Final_scale;
            //    ECS.SetScale(m_Explosion, Final_scale);
            //    set_explode_start = true;
            //}
            ////Only expand the outerring first then inner ring
            ////if(start_outerring)
            ////{
            ////    //Expand in 30s
            ////    accumulated_outerring_dt += dt;
            ////    if (accumulated_outerring_dt >= outerring_scale_duration)
            ////    {
            ////        accumulated_outerring_dt = outerring_scale_duration;
            ////        start_outerring = false;
            ////    }
            ////    Transform start_explode = ECS.GetComponent<Transform>(OuterRing);
            ////    Vector3 outerring_large_scale = new Vector3(final_scale_value, final_scale_value, final_scale_value);
            ////    Vector3 outer_ring_small_scale = new Vector3(final_scale_value, 0.0f, final_scale_value);
            ////    Vector3 outerring_scale = Vector3.Lerp(outer_ring_small_scale, outerring_large_scale, accumulated_outerring_dt / outerring_scale_duration);
            ////    ECS.SetTransformECS(OuterRing, blink.Position, blink.Rotation, scale);
            ////}

            ////Start expanding and lerping to scale size and set trigger to active
        
            //accu_dt_expand += dt;
            //if (accu_dt_expand < explosion_expand_multiplier)
            //{
            //    Vector3 curr_scale = Vector3.Lerp(start_scale, Final_scale, accu_dt_expand / explosion_expand_multiplier);
            //    ECS.SetScale(m_Explosion,curr_scale);
            //    //Change the centre colliderbox position also
            //    BoxCollider explosion_collider = ECS.GetComponent<BoxCollider>(m_Explosion);
            //    explosion_collider.centre.Y = curr_scale.Y * centre_shift_multiplier;
            //    explosion_collider.extends.Y = curr_scale.Y * extends_multiplier_Y;
            //    explosion_collider.extends.Z = curr_scale.Y * extends_multiplier_XZ;
            //    explosion_collider.extends.X = curr_scale.Y * extends_multiplier_XZ;
            //    ECS.SetColliderBox(m_Explosion, ref explosion_collider.isTrigger, ref explosion_collider.centre, ref explosion_collider.extends);
            //}
            ////Scale down the explosion after it happens
            //if (accu_dt_expand >= explosion_expand_multiplier)
            //{
            //    //Scale down
            //    scale_down_dt += dt;
            //    if (scale_down_dt > scale_down_time)
            //    {
            //        scale_down_dt = scale_down_time;
            //        explode_phase3 = false;
            //        m_End = true;
            //        BoxCollider end_explosion_collider = ECS.GetComponent<BoxCollider>(m_Explosion);
            //        end_explosion_collider.centre = start_centre;
            //        end_explosion_collider.extends = start_extends;
            //        ECS.SetColliderBox(m_Explosion, ref end_explosion_collider.isTrigger, ref end_explosion_collider.centre, ref end_explosion_collider.extends);
            //    }
            //    Vector3 scale_down = Vector3.Lerp(Final_scale, start_scale, scale_down_dt / scale_down_time);
            //    ECS.SetScale(m_Explosion, scale_down);
            //}
        }

        uint m_Indicator;
        uint m_DropMissile;
        uint m_Explosion;
        uint m_InnerRing;
        uint m_OuterRing;
        uint m_EntityID;

        Vector3 start_scale;
        Vector3 start_extends;
        Vector3 start_centre;

        //float accu_dt_start = 0.0f; // Time given until it starts
        public bool m_Start = false;
        public bool m_End = false;
        //first phase
        float accu_dt_blink = 1.0f; // Start big, time given until if flips
        bool blink_phase1 = true;
        float blink_phase_dt = 0.0f;
        bool isBig = true;

        bool start_blinking = true;
        float large_blink_scale = 0.1f;
        static float small_blink_scale = 0.001f;
        float Scale_duration = 0.5f;
        float accumulated_scale = 0.0f;
        //second phase
        bool missle_drop_phase2 = false;
        bool set_missle_start = false;
        float accu_dt_drop = 0.0f;

        float missle_drop_speed = 10f;
        Vector3 Start_drop_pos = new Vector3(0, 15, 0);
        Vector3 End_drop_pos = new Vector3(0);
        float missile_scale = 0.1f;
        // third phase explosion
        bool explode_phase3 = false;
        bool set_explode_start = false;
        float accu_dt_expand = 0.0f;

        float centre_shift_multiplier = 4.0f;
        float extends_multiplier_Y = 800.0f;
        float extends_multiplier_XZ = 70.0f;
        float final_scale_value = 0.1f;
        float explosion_expand_multiplier = 0.5f;
        //float linger_time = 0.0f;
        float scale_down_dt = 0.0f;
        float scale_down_time = 0.7f;

        //bool start_outerring = true;
        //float accumulated_outerring_dt = 0.0f;
        //float outerring_scale_duration = 0.2f;
    }

    public class MissleGroups
    {
        // This will be the arti group
        public uint m_ParentID;
        public bool m_Triggered = false;
        public List<Missile> m_MissleList = new List<Missile>();
    }

    public class MissileController : MonoBehaviour
    {
        // Contains the List of Missiles
        MissleGroups m_MsGroup = new MissleGroups();
        private List<float> m_Timers;
        float m_AccDt = 0.0f;
        uint playerID;

        public MissileController()
        {
            // initialize private variables here
        }

        public override void Init(ref uint _entityID)
        {
            // Initialize the necessary variables
            entityID = _entityID;
            m_MsGroup.m_ParentID = _entityID;
            string missile = "Missile";
            playerID = ECS.FindEntityWithName("PlayerShip");
            uint indicator, dropMissile, explosion, missileID;

            // Missile Trigger 1
            if (_entityID == ECS.FindEntityWithName("MissileTrigger1"))
            {
                m_Timers = new List<float> { 0.5f, 1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 3.5f, 4.0f };
                for (int i = 1; i < m_Timers.Count + 1; ++i)
                {
                    missileID = ECS.FindChildEntityWithName(_entityID, missile + i);
                    indicator = ECS.FindChildEntityWithName(missileID, "Indicator");
                    dropMissile = ECS.FindChildEntityWithName(missileID, "DropMissile");
                    explosion = ECS.FindChildEntityWithName(missileID, "Explosion");
                    m_MsGroup.m_MissleList.Add(new Missile(indicator, dropMissile, explosion, missileID));
                }
            }

            // Missile Trigger 2
            if (_entityID == ECS.FindEntityWithName("MissileTrigger2"))
            {
                m_Timers = new List<float> { 2.1f, 2.3f, 3f, 3.3f, 4f, 4.7f, 5.2f, 5.7f };
                for (int i = 1; i < m_Timers.Count + 1; ++i)
                {
                    missileID = ECS.FindChildEntityWithName(_entityID, missile + i);
                    indicator = ECS.FindChildEntityWithName(missileID, "Indicator");
                    dropMissile = ECS.FindChildEntityWithName(missileID, "DropMissile");
                    explosion = ECS.FindChildEntityWithName(missileID, "Explosion");
                    m_MsGroup.m_MissleList.Add(new Missile(indicator, dropMissile, explosion, missileID));
                }
            }

            // Missile Trigger 3
            if (_entityID == ECS.FindEntityWithName("MissileTrigger3"))
            {
                m_Timers = new List<float> { 2.6f, 2.9f, 3.3f, 4.4f };
                for (int i = 1; i < m_Timers.Count + 1; ++i)
                {
                    missileID = ECS.FindChildEntityWithName(_entityID, missile + i);
                    indicator = ECS.FindChildEntityWithName(missileID, "Indicator");
                    dropMissile = ECS.FindChildEntityWithName(missileID, "DropMissile");
                    explosion = ECS.FindChildEntityWithName(missileID, "Explosion");
                    m_MsGroup.m_MissleList.Add(new Missile(indicator, dropMissile, explosion, missileID));
                }
            }

            // Missile Trigger 4
            if (_entityID == ECS.FindEntityWithName("MissileTrigger4"))
            {
                m_Timers = new List<float> { 1f, 1.2f, 1.5f, 1.6f, 1.8f, 2.1f, 2.6f, 3f, 3.3f, 3.6f, 4f, 4.3f };
                for (int i = 1; i < m_Timers.Count + 1; ++i)
                {
                    missileID = ECS.FindChildEntityWithName(_entityID, missile + i);
                    indicator = ECS.FindChildEntityWithName(missileID, "Indicator");
                    dropMissile = ECS.FindChildEntityWithName(missileID, "DropMissile");
                    explosion = ECS.FindChildEntityWithName(missileID, "Explosion");
                    m_MsGroup.m_MissleList.Add(new Missile(indicator, dropMissile, explosion, missileID));
                }
            }

            // Missile Trigger 5
            if (_entityID == ECS.FindEntityWithName("MissileTrigger5"))
            {
                m_Timers = new List<float> { 1f, 1.2f, 1.5f, 1.6f, 1.8f, 2.1f, 2.6f, 3f, 3.3f, 3.6f, 4f };
                for (int i = 1; i < m_Timers.Count + 1; ++i)
                {
                    missileID = ECS.FindChildEntityWithName(_entityID, missile + i);
                    indicator = ECS.FindChildEntityWithName(missileID, "Indicator");
                    dropMissile = ECS.FindChildEntityWithName(missileID, "DropMissile");
                    explosion = ECS.FindChildEntityWithName(missileID, "Explosion");
                    m_MsGroup.m_MissleList.Add(new Missile(indicator, dropMissile, explosion, missileID));
                }
            }
        }

        public override void Start()
        {
        }

        public override void Update(float dt)
        {
            // Update if trigger box is hit
            if (m_MsGroup.m_Triggered)
            {
                m_AccDt += dt;
                // Start the missiles which passed the timer and not started
                for (int i = 0; i < m_MsGroup.m_MissleList.Count; ++i)
                {
                    if (!m_MsGroup.m_MissleList[i].m_Start && m_AccDt >= m_Timers[i])
                    {
                        m_MsGroup.m_MissleList[i].Start();
                    }

                    // Update all the missiles in the missile list
                    m_MsGroup.m_MissleList[i].Update(dt);
                }

                //Update all the missiles in the missile list
                //for (int i = 0; i < m_MsGroup.m_MissleList.Count; ++i)
                //{
                //    Console.WriteLine(i);
                //    m_MsGroup.m_MissleList[i].Update(dt);
                //}

                // Stop update once all is completed
                if (m_MsGroup.m_MissleList[m_MsGroup.m_MissleList.Count - 1].m_End)
                {
                    m_MsGroup.m_Triggered = false;
                }
            }
        }
        public override void LateUpdate(float dt)
        {

        }
        public override void OnTriggerEnter(uint id)
        {
            // Check if player has entered trigger box
            if (id == playerID)
            {
                //Console.WriteLine("Triggered");
                m_MsGroup.m_Triggered = true;
            }
        }
        public override void OnTriggerExit(uint id)
        {

        }
    }
}



// Old one
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Scripting
//{
//    public class MissileController : MonoBehaviour
//    {
//        //The child's must be the same name while only keep the difference of main parent group
//        //if this does not work go back Movestuff script
//        public class MissleGroups
//        {
//            public uint ParentID = 0;
//            public bool ActivateMissiles = false;
//            public bool FiredAlready = false;

//            public float accumulated_dt = 0.0f;
//            public int counter = 0;

//           public List<float> Timers = new List<float>();
//           public List<uint> Indi_Missiles = new List<uint>();
//        }

//        MissleGroups first_group = new MissleGroups();
//        MissleGroups second_group = new MissleGroups();
//        MissleGroups third_group = new MissleGroups();
//        MissleGroups fourth_group = new MissleGroups();
//        MissleGroups fitfh_group = new MissleGroups();
//        List<MissleGroups> Total_Missile_groups = new List<MissleGroups>();
//        public MissileController()
//        {
//            // initialize private variables here



//        }

//        public override void Init(ref uint _entityID)
//        {
//            entityID = _entityID;


//            // Initialise the The individual controller with thier timer
//            if (_entityID == ECS.FindEntityWithName("MissileTrigger1"))
//            {
//                first_group.Timers = new List<float> {0.3f, 1.1f, 2.5f, 2.9f, 3.6f, 3.8f, 4f, 4f};
//                first_group.ParentID = _entityID;
//                first_group.ActivateMissiles = false;
//            }
//            // Add new controllers here
//            //if (_entityID == ECS.FindEntityWithName("Parentxxx"))
//            //    Timers = new List<float> { 0.34f, 0.66f };

//            if (_entityID == ECS.FindEntityWithName("MissileTrigger2"))
//            {
//                first_group.Timers = new List<float> { 2.1f, 2.3f, 3f, 3.3f, 4f, 4.7f, 5.2f, 5.7f};
//                first_group.ParentID = _entityID;
//                first_group.ActivateMissiles = false;
//            }

//            if (_entityID == ECS.FindEntityWithName("MissileTrigger3"))
//            {
//                first_group.Timers = new List<float> { 2.6f, 2.9f, 3.3f, 4.4f};
//                first_group.ParentID = _entityID;
//                first_group.ActivateMissiles = false;
//            }

//            if (_entityID == ECS.FindEntityWithName("MissileTrigger4"))
//            {
//                first_group.Timers = new List<float> {1f, 1.2f, 1.5f, 1.6f, 1.8f, 2.1f, 2.6f, 3f, 3.3f, 3.6f, 4f, 4.3f};
//                first_group.ParentID = _entityID;
//                first_group.ActivateMissiles = false;
//            }

//            if (_entityID == ECS.FindEntityWithName("MissileTrigger5"))
//            {
//                first_group.Timers = new List<float> { 1f, 1.2f, 1.5f, 1.6f, 1.8f, 2.1f, 2.6f, 3f, 3.3f, 3.6f, 4f};
//                first_group.ParentID = _entityID;
//                first_group.ActivateMissiles = false;
//            }


//            Total_Missile_groups.Add(first_group);
//            Total_Missile_groups.Add(second_group);
//            Total_Missile_groups.Add(third_group);
//            Total_Missile_groups.Add(fourth_group);
//            Total_Missile_groups.Add(fitfh_group);

//            UpdateMissleGroup(_entityID);
//        }

//        public override void Start()
//        {

//        }
//        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
//        {
//            for (int i = 0; i < Total_Missile_groups.Count; ++i)
//            {
//                //If fired already we can skip
//                if (Total_Missile_groups[i].FiredAlready)
//                {
//                    continue;
//                }
//                //If missile enable
//                if (Total_Missile_groups[i].ActivateMissiles)
//                {
//                    Total_Missile_groups[i].accumulated_dt += dt;
//                    if (Total_Missile_groups[i].Timers[Total_Missile_groups[i].counter] <= Total_Missile_groups[i].accumulated_dt)
//                    {
//                        GameUtilities.StartMissile(Total_Missile_groups[i].Indi_Missiles[Total_Missile_groups[i].counter]);
//                        Total_Missile_groups[i].counter++;
//                    }
//                    if (Total_Missile_groups[i].Indi_Missiles.Count <= Total_Missile_groups[i].counter)
//                    {
//                        Total_Missile_groups[i].FiredAlready = true;
//                    }
//                }
//            }
//        }
//        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
//        {

//        }
//        public override void OnTriggerEnter(uint id)
//        {
//            MatchMissleGroupParentIDandActivate(entityID);
//        }
//        public override void OnTriggerExit(uint id)
//        {

//        }

//        void UpdateMissleGroup(uint _entityID)
//        {
//            //Call update on all missle groups
//            for (int i = 0; i < Total_Missile_groups.Count; ++i)
//            {
//                int counter = ECS.GetChildCount(_entityID);
//                string missile_name = "Missile";
//                string loop_name;
//                for (int j = 1; j < counter + 1; ++j)
//                {
//                    loop_name = missile_name + j;
//                    uint MissileNo = ECS.FindChildEntityWithName(entityID, loop_name);
//                    Total_Missile_groups[i].Indi_Missiles.Add(MissileNo);
//                }
//            }
//        }

//        void MatchMissleGroupParentIDandActivate(uint _ID)
//        {
//            for (int i = 0; i < Total_Missile_groups.Count; ++i)
//            {
//                if (Total_Missile_groups[i].ParentID == _ID)
//                {
//                    Total_Missile_groups[i].ActivateMissiles = true;
//                }
//            }
//        }
//    }
//}
