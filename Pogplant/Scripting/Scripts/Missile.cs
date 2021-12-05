using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class Missile : MonoBehaviour
    {
        uint Indicator;
        uint DropMissile;
        uint Explosion;
        uint InnerRing;
        uint OuterRing;
        //float accu_dt_start = 0.0f; // Time given until it starts
        bool start_ani = false;
        //first phase
        float accu_dt_blink = 1.0f; // Start big, time given until if flips
        bool blink_phase1 = true;
        float blink_phase_dt = 0.0f;
        bool isBig = true;

        bool start_blinking = true;
        float large_blink_scale = 0.1f;
        float small_blink_scale = 0.001f;
        float Scale_duration = 0.5f;
        float accumulated_scale = 0.0f;
        //second phase
        bool missle_drop_phase2 = false;
        bool set_missle_start = false;
        float accu_dt_drop = 0.0f;

        float missle_drop_speed = 10f;
        Vector3 Start_drop_pos = new Vector3(0, 15, 0);
        float  missile_scale = 0.1f;
        // third phase explosionlarge_blink_scale
        bool explode_phase3 = false;
        bool set_explode_start = false;
        Vector3 start_scale;
        Vector3 start_extends;
        Vector3 start_centre;
        float accu_dt_expand = 0.0f;

        float centre_shift_multiplier = 4.0f;
        float extends_multiplier_Y = 4.0f;
        float extends_multiplier_XZ = 2.0f;
        float final_scale_value = 0.1f;
        float explosion_expand_multiplier = 0.5f;
        float linger_time = 0.0f;
        float scale_down_dt = 0.0f;

        bool start_outerring = true;
        float accumulated_outerring_dt = 0.0f;
        float outerring_scale_duration = 0.2f;


        public Missile()
        {
            // initialize private variables here



        }

        public override void Init(ref uint _entityID)
        {
            //Only initialises if it has rigidbody
            entityID = _entityID;
            Indicator = ECS.FindChildEntityWithName(entityID, "Indicator");
            DropMissile = ECS.FindChildEntityWithName(entityID, "DropMissile");
            Explosion = ECS.FindChildEntityWithName(entityID, "Explosion");
            InnerRing = ECS.FindChildEntityWithName(Explosion, "InnerRing");
            OuterRing = ECS.FindChildEntityWithName(Explosion, "OuterRing");
            start_scale = ECS.GetComponent<Transform>(Explosion).Scale;
            start_extends = ECS.GetComponent<BoxCollider>(Explosion).extends;
            start_centre = ECS.GetComponent<BoxCollider>(Explosion).centre;

            //Always self-set minimizer scale
            Transform star_blink = ECS.GetComponent<Transform>(Indicator);
            Vector3 start_mini = new Vector3(small_blink_scale, small_blink_scale, small_blink_scale);
            ECS.SetTransformECS(Indicator, star_blink.Position, star_blink.Rotation, start_mini);
        }

        public override void Start()
        {

        }
        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            if(start_ani)
            {
                if (blink_phase1)
                {
                    //Calls blinking stage
                    Phase1(ref dt);
                }
                if(missle_drop_phase2)
                {
                    //Calls the dropping stage
                    Phase2(ref dt);
                }
                if(explode_phase3)
                {
                    //Calls the explosion part;
                    Phase3(ref dt);
                }
            }
        }
        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

        }

        public override void OnTriggerEnter(uint id)
        {


        }
        public override void OnTriggerExit(uint id)
        {

        }

        void SetMissile(bool isLaunched)
        {
            start_ani = isLaunched;
        }

        void Phase1(ref float dt)
        {
            //Just scale  to big and small
            Transform blink = ECS.GetComponent<Transform>(Indicator);
            if(start_blinking)
            {
                //Expand in 0.5s
                accumulated_scale += dt;
                if(accumulated_scale >= Scale_duration)
                {
                    accumulated_scale = Scale_duration;
                    ECS.PlayAudio(entityID, 0);
                    start_blinking = false;
                }
                Vector3 large_scale = new Vector3(large_blink_scale, large_blink_scale, large_blink_scale);
                Vector3 small_scale = new Vector3(small_blink_scale, small_blink_scale, small_blink_scale);
                Vector3 scale = Vector3.Lerp(small_scale, large_scale, accumulated_scale / Scale_duration);
                ECS.SetTransformECS(Indicator, blink.Position, blink.Rotation, scale);
            }
            if(!start_blinking)
            {
                blink_phase_dt += dt;
                accu_dt_blink += dt;
                if (accu_dt_blink >= 0.2f)
                {
                    accu_dt_blink = 0.0f;
                    Vector3 scale = new Vector3(0, 0, 0);
                    if (isBig)
                    {
                        //Set your enlargement scale
                        scale = new Vector3(large_blink_scale, large_blink_scale, large_blink_scale);
                    }
                    if (!isBig)
                    {
                        //Set minimizer scale
                        scale = new Vector3(small_blink_scale, small_blink_scale, small_blink_scale);
                    }
                    ECS.SetTransformECS(Indicator, blink.Position, blink.Rotation, scale);
                    isBig = !isBig;
                }
                if (blink_phase_dt >= 1.5f)
                {
                    //Force set scale to small and initiate second phase
                    Vector3 force_scale = new Vector3(small_blink_scale, small_blink_scale, small_blink_scale);
                    ECS.SetTransformECS(Indicator, blink.Position, blink.Rotation, force_scale);
                    blink_phase1 = false;
                    missle_drop_phase2 = true;
                }
            }
        }

        void Phase2(ref float dt)
        {
            //Set missle drop high
            if (!set_missle_start)
            {
                Transform dropper_set = ECS.GetComponent<Transform>(DropMissile);
                Vector3 set_Scale = new Vector3(missile_scale, missile_scale, missile_scale);
                ECS.SetTransformECS(DropMissile, Start_drop_pos, dropper_set.Rotation, set_Scale);
                set_missle_start = true;
            }

            accu_dt_drop += missle_drop_speed * dt;
            Transform dropper = ECS.GetComponent<Transform>(DropMissile);
            Transform end_pos = ECS.GetComponent<Transform>(Indicator);
            //Iterate the dropping now
            Vector3 drop_pos = Vector3.Lerp(Start_drop_pos, end_pos.Position, accu_dt_drop);
            ECS.SetTransformECS(DropMissile, drop_pos, dropper.Rotation, dropper.Scale);
            //If the aabb collider min Y only touches touches the bottem, end phase 2 and start phase 3
            BoxCollider missle_collider = ECS.GetComponent<BoxCollider>(DropMissile);
            Vector3 min = drop_pos - missle_collider.extends; // Might be wrong
            if (min.Y <= end_pos.Position.Y)
            {
                //Start phase 3 return the missle to unseeable
                Transform dropper_set = new Transform();
                ECS.GetTransformECS(DropMissile, ref dropper_set.Position, ref dropper_set.Rotation, ref dropper_set.Scale);
                Vector3 set_end_Scale = new Vector3(0.1f, 0.1f, 0.1f);
                ECS.SetTransformECS(DropMissile, dropper_set.Position, dropper_set.Rotation, set_end_Scale);
                missle_drop_phase2 = false;
                explode_phase3 = true;
            }
        }

        void Phase3(ref float dt)
        {
            //NEED TO SEPERATE INNER AND OUTER RING TIMINGS, TO GET BETTER FEEL.
            //TAKES LONGER TIME

            if (!set_explode_start)
            {
                //Assume the trigger is already there, just have to not to step on it on a VERY TINY BOX
                Vector3 start3_scale = new Vector3(final_scale_value);
                start3_scale.Y = 0.0f;
                start_scale = start3_scale;
                Transform start_explode = ECS.GetComponent<Transform>(Explosion);
                ECS.SetTransformECS(Explosion, start_explode.Position, start_explode.Rotation, start3_scale);
                set_explode_start = true;
            }
            //Only expand the outerring first then inner ring
            //if(start_outerring)
            //{
            //    //Expand in 30s
            //    accumulated_outerring_dt += dt;
            //    if (accumulated_outerring_dt >= outerring_scale_duration)
            //    {
            //        accumulated_outerring_dt = outerring_scale_duration;
            //        start_outerring = false;
            //    }
            //    Transform start_explode = ECS.GetComponent<Transform>(OuterRing);
            //    Vector3 outerring_large_scale = new Vector3(final_scale_value, final_scale_value, final_scale_value);
            //    Vector3 outer_ring_small_scale = new Vector3(final_scale_value, 0.0f, final_scale_value);
            //    Vector3 outerring_scale = Vector3.Lerp(outer_ring_small_scale, outerring_large_scale, accumulated_outerring_dt / outerring_scale_duration);
            //    ECS.SetTransformECS(OuterRing, blink.Position, blink.Rotation, scale);
            //}

            //Start expanding and lerping to scale size and set trigger to active
            Vector3 Final_scale = new Vector3(final_scale_value);
            accu_dt_expand += dt;
            Transform explode = ECS.GetComponent<Transform>(Explosion);
            if (accu_dt_expand < explosion_expand_multiplier)
            {
                Vector3 curr_scale = Vector3.Lerp(start_scale, Final_scale, accu_dt_expand / explosion_expand_multiplier);
                ECS.SetTransformECS(Explosion, explode.Position, explode.Rotation, curr_scale);
                //Change the centre colliderbox position also
                BoxCollider explosion_collider = ECS.GetComponent<BoxCollider>(Explosion);
                explosion_collider.centre.Y = curr_scale.Y * centre_shift_multiplier;
                explosion_collider.extends.Y = curr_scale.Y * extends_multiplier_Y;
                explosion_collider.extends.Z = curr_scale.Y * extends_multiplier_XZ;
                explosion_collider.extends.X = curr_scale.Y * extends_multiplier_XZ;
                ECS.SetColliderBox(Explosion, ref explosion_collider.isTrigger, ref explosion_collider.centre, ref explosion_collider.extends);
            }
            //Scale down the explosion after it happens
            if (accu_dt_expand >= explosion_expand_multiplier)
            {
                //Scale down
                scale_down_dt += dt;
                if(scale_down_dt > 0.7f)
                {
                    scale_down_dt = 0.7f;
                    explode_phase3 = false;
                    BoxCollider end_explosion_collider = ECS.GetComponent<BoxCollider>(Explosion);
                    end_explosion_collider.centre = start_centre;
                    end_explosion_collider.extends = start_extends;
                    ECS.SetColliderBox(Explosion, ref end_explosion_collider.isTrigger, ref end_explosion_collider.centre, ref end_explosion_collider.extends);
                }
                Vector3 scale_down = Vector3.Lerp(Final_scale, start_scale, scale_down_dt  / 0.7f);
                ECS.SetTransformECS(Explosion, explode.Position, explode.Rotation, scale_down);
            }
        }
    }
}
