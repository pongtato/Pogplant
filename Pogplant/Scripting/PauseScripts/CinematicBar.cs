using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
	public class CinematicBar : PauseBehaviour
	{
        uint cinematic_bar_top_id;
        static bool move_top_bar = false;
        uint cinematic_bar_bottom_id;
        static bool move_bottom_bar = false;
        float cinematic_bar_speed;

		static bool m_entering_animation = false;
		static bool m_exiting_animation = false;
		static bool m_enabled = false;

		public override void Init(ref uint _entityID)
		{
			entityID = _entityID;
			
			cinematic_bar_top_id = ECS.FindChildEntityWithName(entityID, "Top Cinematic Bar");
			cinematic_bar_bottom_id = ECS.FindChildEntityWithName(entityID, "Bottom Cinematic Bar");

            cinematic_bar_speed = ECS.GetValue<float>(entityID, 1.0f, "cinematic_bar_speed");

        }

		public override void Start()
		{
		}

		public override void Update(float dt)
		{
			if (m_enabled)
			{
                if (m_entering_animation)
                {
                    //Top bar
                    if (ECS.GetGlobalPosition(cinematic_bar_top_id).Y > 0.8f)
                    {
                        ECS.SetGlobalPosition(cinematic_bar_top_id, Vector3.Lerp(ECS.GetGlobalPosition(cinematic_bar_top_id), new Vector3(0.0f, 0.79f, 0.0f), cinematic_bar_speed * dt));
                    }
                    else
                        move_top_bar = false;

                    //Bottom bar
                    if (ECS.GetGlobalPosition(cinematic_bar_bottom_id).Y < -0.8f)
                        ECS.SetGlobalPosition(cinematic_bar_bottom_id, Vector3.Lerp(ECS.GetGlobalPosition(cinematic_bar_bottom_id), new Vector3(0.0f, -0.79f, 0.0f), cinematic_bar_speed * dt));
                    else
                        move_bottom_bar = false;

                    if (!move_top_bar && !move_bottom_bar)
                    {
                        m_enabled = false;
                        move_top_bar = true;
                        move_bottom_bar = true;
                        m_entering_animation = false;
                        m_exiting_animation = false;
                    }
                }

                //if (m_exiting_animation)
                //{
                //    //Top bar
                //    if (ECS.GetGlobalPosition(cinematic_bar_top_id).Y < 1.5f)
                //        ECS.SetGlobalPosition(cinematic_bar_top_id, Vector3.Lerp(ECS.GetGlobalPosition(cinematic_bar_top_id), new Vector3(0.0f, 1.51f, 0.0f), cinematic_bar_speed * dt));
                //    else
                //        move_top_bar = false;

                //    //Bottom bar
                //    if (ECS.GetGlobalPosition(cinematic_bar_bottom_id).Y > -1.5f)
                //        ECS.SetGlobalPosition(cinematic_bar_bottom_id, Vector3.Lerp(ECS.GetGlobalPosition(cinematic_bar_bottom_id), new Vector3(0.0f, -1.51f, 0.0f), cinematic_bar_speed * dt));
                //    else
                //        move_bottom_bar = false;

                //    if (!move_top_bar && !move_bottom_bar)
                //    {
                //        m_exiting_animation = false;

                //        GameUtilities.ResumeScene();
                //    }
                //}
            }
		}

		public override void OnTriggerEnter(uint id)
		{
		}

		public override void OnTriggerExit(uint id)
		{
		}

		//ScoreArrayIndex = 0 -> L1Boss
		//scoreArrayIndex = 1 -> L2Boss
		public static void Enable(bool isResumeScene, string next_scene = "None")
		{
			m_enabled = isResumeScene;
			m_entering_animation = true;
            move_top_bar = true;
            move_bottom_bar = true;
        }

		private void UpdateMenuInput()
		{

			if (InputUtility.onKeyTriggered("ESCAPE"))
			{
				//ECS.PlayAudio(entityID, 3, "SFX");
				m_exiting_animation = true;
			}
		}

	}
}
