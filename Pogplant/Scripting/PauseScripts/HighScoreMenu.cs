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
			L2 = 1,
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

		//used to track the next scene
		static string m_next_scene;
		static int m_score_array_index;
		//private for score
		public struct ScoreEntry
		{

			public ScoreEntry(uint _score, uint _score_id, string _name, uint _name_id)
			{
				m_score = _score;
				m_score_id = _score_id;
				m_name = _name;
				m_name_id = _name_id;
			}

			public uint m_score;
			public uint m_score_id;
			public string m_name;
			public uint m_name_id;
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
			AddTitle("Title_L2");

			m_Arrow_Left_A = ECS.FindChildEntityWithName(entityID, "Arrow_Left_A");
			m_Arrow_Left_DPad = ECS.FindChildEntityWithName(entityID, "Arrow_Left_DPad");
			m_Arrow_Right_D = ECS.FindChildEntityWithName(entityID, "Arrow_Right_D");
			m_Arrow_Right_DPad = ECS.FindChildEntityWithName(entityID, "Arrow_Right_DPad");

			//cinematic_bar_top_id = ECS.FindChildEntityWithName(entityID, "Top Cinematic Bar");
			//cinematic_bar_bottom_id = ECS.FindChildEntityWithName(entityID, "Bottom Cinematic Bar");

			//cinematic_bar_speed = ECS.GetValue<float>(entityID, 3.0f, "cinematic_bar_speed");


			//populate ScoreList
			LoadScore();
			UpdateScoreBoard(ref m_index);

			prev_input = !InputUtility.IsControlledBeingUsed();
			UpdateInputUi(InputUtility.IsControlledBeingUsed());
		}

		public override void Start()
		{
		}

		public void LoadScore()
        {
			Random rnd = new Random();
			foreach (TITLE_ORDER entry in Enum.GetValues(typeof(TITLE_ORDER)))
			{
				ScoreList[(uint)entry] = new List<ScoreEntry>();
				for (uint i = 0; i < 5; i++)
                {
					uint parent_id = ECS.FindEntityWithName("Score_0" + (i + 1));
					if (parent_id != m_null_entity)
                    {
                        ScoreList[(uint)entry].Add(new ScoreEntry(	PlayerPrefs.GetValue<uint>(entry.ToString() + "_R" + (i + 1) + "_score", 00000000),
																	ECS.FindChildEntityWithName(parent_id, "Score"),
																	PlayerPrefs.GetValue<String>(entry.ToString() + "_R" + (i + 1) + "_name", "AAA"),
																	ECS.FindChildEntityWithName(parent_id, "Name")));
                    }
					else
                    {
						Console.WriteLine("Unable to find Score_0" + (i + 1));
                    }
				}
			}
		}

		public void SaveScore()
		{
			foreach (TITLE_ORDER entry in Enum.GetValues(typeof(TITLE_ORDER)))
			{
				for (int i = 0; i < 5; i++)
				{
					
					PlayerPrefs.SetValue<uint>(entry.ToString() + "_R" + (i + 1) + "_score", ScoreList[(uint)entry][i].m_score);
					PlayerPrefs.SetValue<String>(entry.ToString() + "_R" + (i + 1) + "_name", ScoreList[(uint)entry][i].m_name);

				}
			}
			PlayerPrefs.Save();
		}

		public void UpdateScoreBoard(ref int cur_index)
		{
			List<ScoreEntry> temp = new List<ScoreEntry>();
			temp = ScoreList[(uint)cur_index].ToList();
			temp.Sort((x, y) => y.m_score.CompareTo(x.m_score));

			//Console.WriteLine("=======================");

			for (int i = 0; i < 5; i++)
            {
				string score_str = temp[i].m_score.ToString();
				int padding_size = 8 - score_str.Length;
				string padding = "";
				for (int j = 0; j < padding_size; j++)
					padding += 0;
				//Console.WriteLine(ScoreList[(uint)cur_index][i].m_score_id);
				//Console.WriteLine(padding + score_str);

				GameUtilities.UpdateText(ScoreList[(uint)cur_index][i].m_name_id, temp[i].m_name);
                GameUtilities.UpdateText(ScoreList[(uint)cur_index][i].m_score_id, padding + score_str);
			}
			//Console.WriteLine("=======================");
		}

		public override void Update(float dt)
		{
			if (m_enabled)
			{
				if (m_entering_animation)
				{
					//Bottom bar
					if (ECS.GetGlobalPosition(m_score_board).Y < -0.15f)
						ECS.SetGlobalPosition(m_score_board, Vector3.Lerp(ECS.GetGlobalPosition(m_score_board), new Vector3(0.0f, -0.149f, -0.2f), m_sliding_speed * dt));
					else
						m_entering_animation = false;
				}

				if (m_exiting_animation)
				{
					//Bottom bar
					if (ECS.GetGlobalPosition(m_score_board).Y > -1.3f)
						ECS.SetGlobalPosition(m_score_board, Vector3.Lerp(ECS.GetGlobalPosition(m_score_board), new Vector3(0.0f, -1.31f, -0.2f), m_sliding_speed * dt));
					else
					{
						m_exiting_animation = false;

						//GameUtilities.ResumeScene();
						GameUtilities.LoadScene(m_next_scene);
					}
				}

				if (!m_exiting_animation && !m_entering_animation)
				{
					if (m_index != m_prev_index)
					{
						UpdateInputUi(InputUtility.IsControlledBeingUsed());
						UpdateScoreBoard(ref m_index);
						UpdateScoreUICategory();

						m_prev_index = m_index;
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

		//ScoreArrayIndex = 0 -> L1Boss
		//scoreArrayIndex = 1 -> L2Boss
		public static void Enable(bool isResumeScene, string next_scene = "None", int ScoreArrayIndex = -1)
		{
			m_enabled = isResumeScene;
			m_entering_animation = true;

			m_next_scene = next_scene;
			m_score_array_index = ScoreArrayIndex;
		}

		private void UpdateMenuInput()
		{
			//different controls based on the scene
			//main menu controls
			//if (m_next_scene == "None")
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
						m_index = (int)TITLE_ORDER.L2;
					else
						m_index--;
				}
				else if (InputUtility.onKeyTriggered("MENURIGHT"))
				{
					ECS.PlayAudio(entityID, 1, "SFX");

					m_prev_index = m_index;

					if (m_index + 1 > (int)TITLE_ORDER.L2)
						m_index = (int)TITLE_ORDER.L1;
					else
						m_index++;
				}

				if (InputUtility.onKeyTriggered(KEY_ID.KEY_SLASH))
				{
					Console.WriteLine("Saved score");
					SaveScore();
				}
			}
			//else //controls during L1Boss and L2Boss
   //         {

   //         }
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
