using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scripting.Utilities;

namespace Scripting
{
    public class SplashScreen : MonoBehaviour
    {
        enum ANIM_STATE
        {
            DP_SPLASH,
            SOFTWARE_SPLASH,
            TEAM_SPLASH,
            END
        }

        ANIM_STATE current_state;

        enum ALPHA_STATE
        {
            ALPHA_UP,
            ALPHA_DOWN,
            END
        }

        ALPHA_STATE alpha_state;

        uint dp_splash_id;
        uint software_splash_id;
        uint team_splash_id;
        uint current_id;

        const float zero_alpha = 0.0f;
        const float full_alpha = 1.0f;
        const float lerp_speed = 1.0f;
        const float full_alpha_delay = 2.5f;
        float full_alpha_timer;
        float lerp_step;

        //uint skip_kb_id;
        //uint skip_ctrler_id;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        public override void Start()
        {
            dp_splash_id = ECS.FindEntityWithName("DigiPen_Splash");
            software_splash_id = ECS.FindEntityWithName("Software_Splash");
            team_splash_id = ECS.FindEntityWithName("Team_Splash");

            //skip_kb_id = ECS.FindEntityWithName("KB_Hint");
            //skip_ctrler_id = ECS.FindEntityWithName("Ctrler_Hint");

            current_state = ANIM_STATE.DP_SPLASH;
            alpha_state = ALPHA_STATE.ALPHA_UP;
        }

        public override void Update(float dt)
        {
            ////Controller/KB text swapping
            //if (InputUtility.IsControlledBeingUsed())
            //{
            //    ECS.SetActive(skip_kb_id, false);
            //    ECS.SetActive(skip_ctrler_id, true);
            //}
            //else
            //{
            //    ECS.SetActive(skip_kb_id, true);
            //    ECS.SetActive(skip_ctrler_id, false);
            //}

            //Skip splash
            if (InputUtility.onKeyTriggered("ESCAPE"))
            {
                lerp_step = 0.0f;
                ECS.SetCanvasAlpha(current_id, 0.0f);
                current_state = ++current_state;
                alpha_state = ALPHA_STATE.ALPHA_UP;
            }

            switch (current_state)
            {
                case ANIM_STATE.DP_SPLASH:
                    current_id = dp_splash_id;
                    UpdateAlpha(dt);
                    break;
                case ANIM_STATE.SOFTWARE_SPLASH:
                    current_id = software_splash_id;
                    UpdateAlpha(dt);
                    break;
                case ANIM_STATE.TEAM_SPLASH:
                    current_id = team_splash_id;
                    UpdateAlpha(dt);
                    break;
                case ANIM_STATE.END:
                    GameUtilities.LoadScene("MainMenu");
                    break;
            }
        }

        void UpdateAlpha(float dt)
        {
            lerp_step += lerp_speed * dt;
            float alpha = 0.0f;

            if (lerp_step >= full_alpha)
            {
                lerp_step = 1.0f;

                if (alpha_state == ALPHA_STATE.ALPHA_UP)
                {
                    full_alpha_timer += dt;

                    if (full_alpha_timer > full_alpha_delay)
                    {
                        ++alpha_state;
                        full_alpha_timer = 0.0f;
                        lerp_step = 0.0f;
                    }
                }
                else
                {
                    ++alpha_state;
                    lerp_step = 0.0f;
                }
            }

            switch (alpha_state)
            {
                case ALPHA_STATE.ALPHA_UP:
                    alpha = PPMath.Lerp(zero_alpha, full_alpha, lerp_step);
                    break;
                case ALPHA_STATE.ALPHA_DOWN:
                    alpha = PPMath.Lerp(full_alpha, zero_alpha, lerp_step);
                    break;
                case ALPHA_STATE.END:
                    current_state = ++current_state;
                    alpha_state = ALPHA_STATE.ALPHA_UP;
                    break;
            }

            ECS.SetCanvasAlpha(current_id, alpha);
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
