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
        //float accu_dt_start = 0.0f; // Time given until it starts
        bool start_ani = false;
        //first phase
        float accu_dt_blink = 1.0f; // Start big, time given until if flips
        bool blink_phase1 = true;
        float blink_phase_dt = 0.0f;
        bool isBig = true;

        float large_blink_scale = 0.1f;
        float small_blink_scale = 0.001f;
        //second phase
        bool missle_drop_phase2 = false;
        bool set_missle_start = false;
        float accu_dt_drop = 0.0f;

        float missle_drop_speed = 0.5f;
        Vector3 Start_drop_pos = new Vector3(0, 100, 0);
        float  missile_scale = 1f;
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
        float explosion_expand_multiplier = 1.0f;
        float linger_time = 0.0f;

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
            blink_phase_dt += dt;
            accu_dt_blink += dt;
            if (accu_dt_blink >= 1.0f)
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
            if (blink_phase_dt >= 2.0f)
            {
                //Force set scale to small and initiate second phase
                Vector3 force_scale = new Vector3(small_blink_scale, small_blink_scale, small_blink_scale);
                ECS.SetTransformECS(Indicator, blink.Position, blink.Rotation, force_scale);
                blink_phase1 = false;
                missle_drop_phase2 = true;
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
            if (!set_explode_start)
            {
                //Assume the trigger is already there, just have to not to step on it on a VERY TINY BOX
                //BoxCollider explosion_collider = ECS.GetComponent<BoxCollider>(Explosion);
                //explosion_collider.isTrigger = true;
                set_explode_start = true;

            }

            //Start expanding and lerping to scale size and set trigger to active
            Vector3 Final_scale = new Vector3(final_scale_value);
            accu_dt_expand += explosion_expand_multiplier * dt;
            Transform explode = ECS.GetComponent<Transform>(Explosion);
            if (explode.Scale.Y < final_scale_value)
            {
                Vector3 curr_scale = Vector3.Lerp(start_scale, Final_scale, accu_dt_expand);
                ECS.SetTransformECS(Explosion, explode.Position, explode.Rotation, curr_scale);
                //Change the centre colliderbox position also
                BoxCollider explosion_collider = ECS.GetComponent<BoxCollider>(Explosion);
                explosion_collider.centre.Y = curr_scale.Y * centre_shift_multiplier;
                explosion_collider.extends.Y = curr_scale.Y * extends_multiplier_Y;
                explosion_collider.extends.Z = curr_scale.Y * extends_multiplier_XZ;
                explosion_collider.extends.X = curr_scale.Y * extends_multiplier_XZ;
                ECS.SetColliderBox(Explosion, ref explosion_collider.isTrigger, ref explosion_collider.centre, ref explosion_collider.extends);
            }
            //Once reach max value check how long you want to linger
            if (explode.Scale.Y >= final_scale_value)
            {
                linger_time += dt;
                if (linger_time > 1.0f)
                {
                    //Clear everything/reset
                    ECS.SetTransformECS(Explosion, explode.Position, explode.Rotation, start_scale);
                    BoxCollider end_explosion_collider = ECS.GetComponent<BoxCollider>(Explosion);
                    end_explosion_collider.centre = start_centre;
                    end_explosion_collider.extends = start_extends;
                    ECS.SetColliderBox(Explosion, ref end_explosion_collider.isTrigger, ref end_explosion_collider.centre, ref end_explosion_collider.extends);
                    explode_phase3 = false;
                }
            }
        }
    }
}
