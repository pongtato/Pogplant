using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class LevelEndCinematicBar : MonoBehaviour
    {
        public string scene_to_load;
        uint cinematic_bar_top_id;
        uint cinematic_bar_bottom_id;
        bool cinematic_bars_enter_screen;
        const float cinematic_bar_speed = 3.0f;

        float delay_timer;
        public float delay_duration;
        bool begin_delay_countdown;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }
        public override void Start()
        {
            scene_to_load = ECS.GetValue<string>(entityID, "Level01_Boss", "SceneToLoad");
            delay_duration = ECS.GetValue<float>(entityID, 2.0f, "DelayDuration");
            cinematic_bar_top_id = ECS.FindEntityWithName("Top Cinematic Bar");
            cinematic_bar_bottom_id = ECS.FindEntityWithName("Bottom Cinematic Bar");
            cinematic_bars_enter_screen = false;
        }

        public override void Update(float dt)
        {
            UpdateCinematicBars(dt);

            if (begin_delay_countdown)
            {
                if (delay_timer < delay_duration)
                {
                    delay_timer += dt;
                }
                else
                {

                    PlayerPrefs.SetValue<uint>("CurrentScore", PlayerScript.score);
                    PlayerPrefs.Save();

                    //Console.WriteLine("========================================================================");
                    //Console.WriteLine("saving score: " + PlayerScript.score);
                    //Console.WriteLine("========================================================================");


                    GameUtilities.LoadScene(scene_to_load);
                }
            }
        }

        void UpdateCinematicBars(float dt)
        { 
            if (cinematic_bars_enter_screen)
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

        public override void LateUpdate(float dt)
        {

        }

        public override void OnTriggerEnter(uint id)
        {
            if (id == PlayerScript.m_singleton.entityID)
            {
                cinematic_bars_enter_screen = true;
                begin_delay_countdown = true;
            }
        }

        public override void OnTriggerExit(uint id)
        {

        }
    }
}
