using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class BossWarning : MonoBehaviour
    {
        enum ANIM_STATE
        {
            COUNT_DOWN,
            SCREEN_IN,
            BACKING_IN,
            TEXT_IN,
            TEXT_PULSE,
            BACKING_OUT,
            SCREEN_OUT
        }

        ANIM_STATE current_anim_state;

        uint warning_screen_left;
        uint warning_screen_right;
        uint warning_backing_left;
        uint warning_backing_right;
        uint warning_text;

        const float screen_Z = -0.2f;
        const float left_backing_Z = -0.15f;
        const float right_backing_Z = -0.1f;

        const float delay_till_animation = 1.0f;
        float delay_timer;
        bool begin_animation;

        const float screen_anim_speed = 6.0f;
        const float screen_begin_X = 1.0f;

        const float backing_anim_speed = 6.0f;
        const float backing_anim_speed_slow = 0.25f;
        const float backing_begin_X = 1.2f;

        const float text_anim_speed = 8.0f;
        const float text_begin_X = 1.1f;

        //Text
        const float text_pulsing_speed = 6.0f;
        const float text_pulsing_final_speed = 30.0f;
        const float text_pulse_max_scale = 0.65f;
        const float text_pulse_final_scale = 3.0f;
        const float text_pulse_min_scale = 0.55f;
        const int text_pulse_max_count = 6;
        int text_pulse_current_count;

        Vector3 pos = new Vector3();
        Vector3 rot = new Vector3();
        Vector3 scale = new Vector3();

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            warning_screen_left = ECS.FindChildEntityWithName(entityID, "Warning Screen Left");
            warning_screen_right = ECS.FindChildEntityWithName(entityID, "Warning Screen Right");
            warning_backing_left = ECS.FindChildEntityWithName(entityID, "Warning Backing Left");
            warning_backing_right = ECS.FindChildEntityWithName(entityID, "Warning Backing Right");
            warning_text = ECS.FindChildEntityWithName(entityID, "Warning Text");

            text_pulse_current_count = 0;
        }
        public override void Start()
        {

        }

        public override void Update(float dt)
        {
            if (delay_timer < delay_till_animation && !begin_animation)
            {
                delay_timer += dt;
            }
            else
            {
                if (current_anim_state == ANIM_STATE.COUNT_DOWN)
                {
                    begin_animation = true;
                    current_anim_state = ANIM_STATE.SCREEN_IN;
                }
            }

            UpdateAnimStates(dt);
        }

        void UpdateAnimStates(float dt)
        {
            switch (current_anim_state)
            {
                case ANIM_STATE.SCREEN_IN:
                    {
                        ECS.SetGlobalPosition(warning_screen_left, Vector3.Lerp(ECS.GetGlobalPosition(warning_screen_left), new Vector3(0, 0, screen_Z), screen_anim_speed * dt));
                        ECS.SetGlobalPosition(warning_screen_right, Vector3.Lerp(ECS.GetGlobalPosition(warning_screen_right), new Vector3(0, 0, screen_Z), screen_anim_speed * dt));

                        if (ECS.GetGlobalPosition(warning_screen_left).X >= -0.001f)
                        {
                            current_anim_state = ANIM_STATE.BACKING_IN;
                        }
                    }
                    break;
                case ANIM_STATE.BACKING_IN:
                    {
                        ECS.SetGlobalPosition(warning_backing_left, Vector3.Lerp(ECS.GetGlobalPosition(warning_backing_left), new Vector3(0, 0, left_backing_Z), backing_anim_speed * dt));
                        ECS.SetGlobalPosition(warning_backing_right, Vector3.Lerp(ECS.GetGlobalPosition(warning_backing_right), new Vector3(0, 0, right_backing_Z), backing_anim_speed * dt));

                        if (ECS.GetGlobalPosition(warning_backing_left).X >= -0.5f)
                        {
                            ECS.SetGlobalPosition(warning_text, Vector3.Lerp(ECS.GetGlobalPosition(warning_text), new Vector3(), text_anim_speed * dt));

                            if (ECS.GetGlobalPosition(warning_text).X >= -0.01f)
                            {
                                current_anim_state = ANIM_STATE.TEXT_PULSE;
                            }
                        }
                    }
                    break;
                case ANIM_STATE.TEXT_PULSE:
                    {
                        ECS.SetGlobalPosition(warning_backing_left, Vector3.Lerp(ECS.GetGlobalPosition(warning_backing_left), new Vector3(0.05f, 0, left_backing_Z), backing_anim_speed_slow * dt));
                        ECS.SetGlobalPosition(warning_backing_right, Vector3.Lerp(ECS.GetGlobalPosition(warning_backing_right), new Vector3(-0.05f, 0, right_backing_Z), backing_anim_speed_slow * dt));

                        if (text_pulse_current_count < text_pulse_max_count)
                        {
                            ECS.SetGlobalScale(warning_text, Vector3.Lerp(ECS.GetGlobalScale(warning_text), new Vector3(text_pulse_max_scale, text_pulse_max_scale, 1.0f), text_pulsing_speed * dt));

                            if (ECS.GetGlobalScale(warning_text).X >= text_pulse_max_scale - 0.01f)
                            {
                                ECS.SetGlobalScale(warning_text, new Vector3(text_pulse_min_scale, text_pulse_min_scale, 1.0f));
                                ++text_pulse_current_count;
                            }
                        }
                        else
                        {
                            ECS.SetGlobalScale(warning_text, Vector3.Lerp(ECS.GetGlobalScale(warning_text), new Vector3(text_pulse_final_scale, text_pulse_final_scale, 1.0f), text_pulsing_final_speed * dt));

                            if (ECS.GetGlobalScale(warning_text).X >= text_pulse_final_scale - 0.2f)
                            {
                                ECS.SetActive(warning_text, false);
                                current_anim_state = ANIM_STATE.BACKING_OUT;
                            }
                        }
                    }
                    break;
                case ANIM_STATE.BACKING_OUT:
                    {
                        ECS.SetGlobalPosition(warning_backing_left, Vector3.Lerp(ECS.GetGlobalPosition(warning_backing_left), new Vector3(-backing_begin_X, 0, left_backing_Z), backing_anim_speed * dt));
                        ECS.SetGlobalPosition(warning_backing_right, Vector3.Lerp(ECS.GetGlobalPosition(warning_backing_right), new Vector3(backing_begin_X, 0, right_backing_Z), backing_anim_speed * dt));

                        if (ECS.GetGlobalPosition(warning_backing_left).X <= -backing_begin_X + 0.01f)
                        {
                            current_anim_state = ANIM_STATE.SCREEN_OUT;
                        }
                    }
                    break;
                case ANIM_STATE.SCREEN_OUT:
                    {
                        ECS.SetGlobalPosition(warning_screen_left, Vector3.Lerp(ECS.GetGlobalPosition(warning_screen_left), new Vector3(-screen_begin_X, 0, screen_Z), screen_anim_speed * dt));
                        ECS.SetGlobalPosition(warning_screen_right, Vector3.Lerp(ECS.GetGlobalPosition(warning_screen_right), new Vector3(screen_begin_X, 0, screen_Z), screen_anim_speed * dt));

                        //if (ECS.GetGlobalPosition(warning_screen_left).X <= -screen_begin_X + 0.01f)
                        //{
                            
                        //}
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
    }
}
