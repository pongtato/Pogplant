using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
	public class HighScoreMenu : PauseBehaviour
	{
		//uint cinematic_bar_top_id;
		//static bool move_top_bar = false;
		//uint cinematic_bar_bottom_id;
		//static bool move_bottom_bar = false;
		//float cinematic_bar_speed;

		public enum TITLE_ORDER
		{
			L1 = 0,
			L1Boss = 1,
			L2 = 2,
			L2Boss = 3
		}

		static bool m_enabled = false;

		//scoreboard
		uint m_score_board;
		float m_sliding_speed;

		//title bar
		private Dictionary<string, uint> TitleMap = new Dictionary<string, uint>();
		private Dictionary<TITLE_ORDER, string> EnumToTitleMap = new Dictionary<TITLE_ORDER, string>();

		//to keep track of the current index
		private int m_index = 0;
		private int m_prev_index = 1;

		//false = keyboard, true = controller
		private bool prev_input = true;
		uint m_Arrow_Left_A;
		uint m_Arrow_Left_DPad;
		uint m_Arrow_Right_D;
		uint m_Arrow_Right_DPad;

		//private for score
		private struct ScoreEntry
		{
				
			public ScoreEntry(uint _score, string _name)
			{
				m_score = _score;
				m_name = _name;
			}
			uint m_score;
			string m_name;
		}

		private List<ScoreEntry>[] ScoreList = new List<ScoreEntry>[4];

		static bool m_entering_animation = false;
		static bool m_exiting_animation = false;

		uint m_null_entity;
		static uint m_title_counter = 0;

		void AddTitle(string object_to_find)
		{
			uint temp_id = ECS.FindChildEntityWithName(entityID, object_to_find);

			if (temp_id != m_null_entity)
			{
				TitleMap.Add(object_to_find, temp_id);
				EnumToTitleMap.Add((TITLE_ORDER)m_title_counter, object_to_find);
				m_title_counter++;
			}
			else
			{
				Console.WriteLine(object_to_find + "is a null entity(HighScoreMenu.CS)!");
			}
		}

		public override void Init(ref uint _entityID)
		{
			entityID = _entityID;
			m_null_entity = ECS.GetNull();

			m_score_board = ECS.FindChildEntityWithName(entityID, "Background");
			m_sliding_speed = ECS.GetValue<float>(entityID, 3.0f, "cinematic_bar_speed");

			AddTitle("Title_L1");
			AddTitle("Title_L1Boss");
			AddTitle("Title_L2");
			AddTitle("Title_L2Boss");

			m_Arrow_Left_A = ECS.FindChildEntityWithName(entityID, "Arrow_Left_A");
			m_Arrow_Left_DPad = ECS.FindChildEntityWithName(entityID, "Arrow_Left_DPad");
			m_Arrow_Right_D = ECS.FindChildEntityWithName(entityID, "Arrow_Right_D");
			m_Arrow_Right_DPad = ECS.FindChildEntityWithName(entityID, "Arrow_Right_DPad");

			//cinematic_bar_top_id = ECS.FindChildEntityWithName(entityID, "Top Cinematic Bar");
			//cinematic_bar_bottom_id = ECS.FindChildEntityWithName(entityID, "Bottom Cinematic Bar");

			//cinematic_bar_speed = ECS.GetValue<float>(entityID, 3.0f, "cinematic_bar_speed");


			//populate ScoreList
			//foreach (TITLE_ORDER entry in Enum.GetValues(typeof(TITLE_ORDER)))
			//{
			//	for (uint i = 0; i < 5; i++)
			//		ScoreList[(uint)entry].Add(new ScoreEntry(i, "test" ));
			//}

			prev_input = !InputUtility.IsControlledBeingUsed();
			UpdateInputUi(InputUtility.IsControlledBeingUsed());
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
					//Bottom bar
					if (ECS.GetGlobalPosition(m_score_board).Y < -0.15f)
						ECS.SetGlobalPosition(m_score_board, Vector3.Lerp(ECS.GetGlobalPosition(m_score_board), new Vector3(0.0f, -0.149f, -0.010f), m_sliding_speed * dt));
					else
						m_entering_animation = false;
				}

				if (m_exiting_animation)
				{
					//Bottom bar
					if (ECS.GetGlobalPosition(m_score_board).Y > -1.3f)
						ECS.SetGlobalPosition(m_score_board, Vector3.Lerp(ECS.GetGlobalPosition(m_score_board), new Vector3(0.0f, -1.31f, -0.010f), m_sliding_speed * dt));
					else
					{
						m_exiting_animation = false;

						GameUtilities.ResumeScene();
					}
				}

				if (!m_exiting_animation && !m_entering_animation)
				{
					if (m_index != m_prev_index)
					{
						UpdateInputUi(InputUtility.IsControlledBeingUsed());
						UpdateScoreUICategory();
					}


					UpdateMenuInput();
				}


				//if (m_entering_animation)
				//{
				//    //Top bar
				//    if (ECS.GetGlobalPosition(cinematic_bar_top_id).Y > 0.8f)
				//        ECS.SetGlobalPosition(cinematic_bar_top_id, Vector3.Lerp(ECS.GetGlobalPosition(cinematic_bar_top_id), new Vector3(0.0f, 0.79f, 0.0f), cinematic_bar_speed * dt));
				//    else
				//        move_top_bar = false;

				//    //Bottom bar
				//    if (ECS.GetGlobalPosition(cinematic_bar_bottom_id).Y < -0.8f)
				//        ECS.SetGlobalPosition(cinematic_bar_bottom_id, Vector3.Lerp(ECS.GetGlobalPosition(cinematic_bar_bottom_id), new Vector3(0.0f, -0.79f, 0.0f), cinematic_bar_speed * dt));
				//    else
				//        move_bottom_bar = false;

				//    if (!move_top_bar && !move_bottom_bar)
				//    {
				//        move_top_bar = true;
				//        move_bottom_bar = true;
				//        m_entering_animation = false;
				//        m_exiting_animation = false;
				//    }
				//}

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

		public static void Enable(bool isResumeScene)
		{
			m_enabled = isResumeScene;
			m_entering_animation = true;
		}

		private void UpdateMenuInput()
		{

			if (InputUtility.onKeyTriggered("ESCAPE"))
			{
				//ECS.PlayAudio(entityID, 3, "SFX");

				m_exiting_animation = true;
			}

			if (InputUtility.onKeyTriggered("MENULEFT"))
			{
				ECS.PlayAudio(entityID, 0, "SFX");

				m_prev_index = m_index;

				if (m_index - 1 < (int)TITLE_ORDER.L1)
					m_index = (int)TITLE_ORDER.L2Boss;
				else
					m_index--;
			}
			else if (InputUtility.onKeyTriggered("MENURIGHT"))
			{
				ECS.PlayAudio(entityID, 1, "SFX");

				m_prev_index = m_index;

				if (m_index + 1 > (int)TITLE_ORDER.L2Boss)
					m_index = (int)TITLE_ORDER.L1;
				else
					m_index++;
			}
		}

		private void UpdateInputUi(bool controller_used)
		{
			if (controller_used != prev_input)
			{
				ECS.SetActive(m_Arrow_Right_D, !controller_used);
				ECS.SetActive(m_Arrow_Left_A, !controller_used);

				ECS.SetActive(m_Arrow_Left_DPad, controller_used);
				ECS.SetActive(m_Arrow_Right_DPad, controller_used);

				prev_input = controller_used;
			}
		}

		private void UpdateScoreUICategory()
		{
			foreach (KeyValuePair<TITLE_ORDER, string> entry in EnumToTitleMap)
				ECS.SetActive(TitleMap[entry.Value], entry.Key == (TITLE_ORDER)m_index ? true : false);
		}
	}
}
