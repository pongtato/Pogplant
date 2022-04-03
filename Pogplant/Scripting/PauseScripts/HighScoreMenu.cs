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
		static private int m_index = 0;
		private int m_prev_index = 1;

		//false = keyboard, true = controller
		private bool prev_input = true;
		uint m_Arrow_Left_A;
		uint m_Arrow_Left_DPad;
		uint m_Arrow_Right_D;
		uint m_Arrow_Right_DPad;

		//used to track the next scene
		static string m_next_scene;
		static bool m_after_endgame = false;

		char[] m_cur_name = new char[3];
		uint[] m_cur_name_letters = new uint[3];
		char[] m_valid_letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};
		static int m_name_placement = 0;
		int m_cur_name_cursor = 0; //range is 0 to 2
		int m_prev_name_cursor = 0; //range is 0 to 2
		int[] m_cur_letter_index = { 0, 0, 0 }; //range is A to Z;
		int[] m_prev_letter_index = { 0, 0, 0 }; //range is A to Z;
		float m_up_down_offset;
		Vector3 m_up_down_pos = new Vector3();
		uint m_cur_name_cursor_id;

		//private for score
		public struct ScoreEntry
		{

			public ScoreEntry(uint _score, uint _score_id, string _name, uint _letter_1, uint _letter_2, uint _letter_3)
			{
				m_score = _score;
				m_score_id = _score_id;
				m_name = _name;
				m_letter_1 = _letter_1;
				m_letter_2 = _letter_2;
				m_letter_3 = _letter_3;
			}

			public uint m_score;
			public uint m_score_id;
			public string m_name;
			public uint m_letter_1;
			public uint m_letter_2;
			public uint m_letter_3;
		}

		static private List<ScoreEntry>[] ScoreList = new List<ScoreEntry>[2];

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

			//used for inputting of name for highscore
			m_cur_name_cursor_id = ECS.FindChildEntityWithName(entityID, "Arrow_Up_Down");

			//cinematic_bar_top_id = ECS.FindChildEntityWithName(entityID, "Top Cinematic Bar");
			//cinematic_bar_bottom_id = ECS.FindChildEntityWithName(entityID, "Bottom Cinematic Bar");

			//cinematic_bar_speed = ECS.GetValue<float>(entityID, 3.0f, "cinematic_bar_speed");

			m_up_down_offset = ECS.GetValue<float>(entityID, 0.025f, "Arrow_Up_Down_Offset");
			m_up_down_pos = ECS.GetGlobalPosition(m_cur_name_cursor_id);

			//populate ScoreList
			LoadScore();
			UpdateScoreBoard(ref m_index);

			prev_input = !InputUtility.IsControlledBeingUsed();
			UpdateInputUi(InputUtility.IsControlledBeingUsed());

			for (int i = 0; i < 3; i++)
			{
				m_cur_name[i] = 'A';

			}
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
						uint _name_id = ECS.FindChildEntityWithName(parent_id, "Name");

                        ScoreList[(uint)entry].Add(new ScoreEntry(PlayerPrefs.GetValue<uint>(entry.ToString() + "_R" + (i + 1) + "_score", 0),
                                                                    ECS.FindChildEntityWithName(parent_id, "Score"),
                                                                    PlayerPrefs.GetValue<String>(entry.ToString() + "_R" + (i + 1) + "_name", "AAA"),
																	ECS.FindChildEntityWithName(parent_id, "Letter_" + 1),
																	ECS.FindChildEntityWithName(parent_id, "Letter_" + 2),
																	ECS.FindChildEntityWithName(parent_id, "Letter_" + 3)
																	));
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

        static public void UpdateScoreBoard(ref int cur_index)
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


				char[] temp_c_array = temp[i].m_name.ToCharArray();
				GameUtilities.UpdateText(ScoreList[(uint)cur_index][i].m_letter_1, temp_c_array[0].ToString());
				GameUtilities.UpdateText(ScoreList[(uint)cur_index][i].m_letter_2, temp_c_array[1].ToString());
				GameUtilities.UpdateText(ScoreList[(uint)cur_index][i].m_letter_3, temp_c_array[2].ToString());


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

						if (m_after_endgame)
                        {
							SaveScore();
							GameUtilities.LoadScene(m_next_scene);
						}
						
						GameUtilities.ResumeScene();
					}
				}

				if (!m_exiting_animation && !m_entering_animation)
				{
					if ((m_index != m_prev_index && !m_after_endgame) ||
						(m_cur_name_cursor != m_prev_name_cursor && m_after_endgame) ||
						(m_prev_letter_index[m_cur_name_cursor] != m_cur_letter_index[m_cur_name_cursor] && m_after_endgame))
					{

						if(!m_after_endgame)
							UpdateInputUi(InputUtility.IsControlledBeingUsed());
						else
                        {
							//update cursor position
							if (m_cur_name_cursor != m_prev_name_cursor)
                            {
								UpdateNameCursorPos();
								m_prev_name_cursor = m_cur_name_cursor;
							}

							if (m_prev_letter_index[m_cur_name_cursor] != m_cur_letter_index[m_cur_name_cursor])
							{
								UpdateNameLetterIndex();
								m_prev_letter_index[m_cur_name_cursor] = m_cur_letter_index[m_cur_name_cursor];
								Console.WriteLine("do once");
							}
						}
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
		public static void Enable(bool isResumeScene, string next_scene = "None")
		{
			m_enabled = isResumeScene;
			m_entering_animation = true;

			m_next_scene = next_scene;

			
			if (next_scene != "None")
            {
				m_after_endgame = true;

				string current_scene = GameUtilities.GetSceneName();

				m_index = current_scene == "Level01_Boss" ? 0 : 1;

				if (PlayerScript.score > ScoreList[m_index][4].m_score)
                {
					//manually sort lol
					for (int i = 0; i < 5; i++)
                    {
						//Console.WriteLine("Adding into index: " + i);
						if (PlayerScript.score > ScoreList[m_index][i].m_score)
                        {
							for (int j = 3; j > (i-1); j--)
                            {
								//Console.WriteLine("Copying " + j +  " into " + (j+1));
								ScoreEntry temp = ScoreList[m_index][j+1];
								temp.m_score = ScoreList[m_index][j].m_score;
								temp.m_name = ScoreList[m_index][j].m_name;
								ScoreList[m_index][j+1] = temp;
							}

							m_name_placement = i;

							ScoreEntry asd = ScoreList[m_index][m_name_placement];
							asd.m_score = PlayerScript.score;
							ScoreList[m_index][m_name_placement] = asd;
							//Console.WriteLine("Added into index: " + m_name_placement);

							UpdateScoreBoard(ref m_index);
							break;
                        }

					}

					//add into scorelist
                }
				else
                {
					GameUtilities.LoadScene(m_next_scene);
					GameUtilities.ResumeScene();
				}

			}
		}

		private void UpdateMenuInput()
		{

			if (InputUtility.onKeyTriggered("ESCAPE") && !m_after_endgame)
			{
				//ECS.PlayAudio(entityID, 3, "SFX");
				m_exiting_animation = true;
			}



			if (InputUtility.onKeyTriggered("MENULEFT"))
			{
				ECS.PlayAudio(entityID, 0, "SFX");
				//used for main menu
				if (!m_after_endgame)
                {
					m_prev_index = m_index;

					if (m_index - 1 < (int)TITLE_ORDER.L1)
						m_index = (int)TITLE_ORDER.L2;
					else
						m_index--;
				}
				else //used for after endgame score
                {
					m_prev_name_cursor = m_cur_name_cursor;

					if (m_cur_name_cursor - 1 < 0)
						m_cur_name_cursor = 2;
					else
						m_cur_name_cursor--;
                }
				
			}
			else if (InputUtility.onKeyTriggered("MENURIGHT"))
			{
				ECS.PlayAudio(entityID, 1, "SFX");

				//used for main menu
				if (!m_after_endgame)
                {
					m_prev_index = m_index;

					if (m_index + 1 > (int)TITLE_ORDER.L2)
						m_index = (int)TITLE_ORDER.L1;
					else
						m_index++;

					//after moving left and right, gotta update the letter index to the current letter
				}
				else //used for after endgame score
				{
					m_prev_name_cursor = m_cur_name_cursor;

					if (m_cur_name_cursor + 1 > 2)
						m_cur_name_cursor = 0;
					else
						m_cur_name_cursor++;
				}
			}

			//used for updating letters in inputting name
			if (InputUtility.onKeyTriggered("MENUUP") && m_after_endgame)
			{
				//ECS.PlayAudio(entityID, 2, "SFX");
				m_prev_letter_index[m_cur_name_cursor] = m_cur_letter_index[m_cur_name_cursor];
				
				m_cur_letter_index[m_cur_name_cursor] += 1;
				//Console.WriteLine(m_cur_letter_index[m_cur_name_cursor]);
				if (m_cur_letter_index[m_cur_name_cursor] > m_valid_letters.Length - 1)
					m_cur_letter_index[m_cur_name_cursor] = 0;
			}
			else if (InputUtility.onKeyTriggered("MENUDOWN") && m_after_endgame)
			{
				//ECS.PlayAudio(entityID, 3, "SFX");
				m_prev_letter_index[m_cur_name_cursor] = m_cur_letter_index[m_cur_name_cursor];

				m_cur_letter_index[m_cur_name_cursor] -= 1;

				//Console.WriteLine(m_cur_letter_index[m_cur_name_cursor]);
				if (m_cur_letter_index[m_cur_name_cursor] < 0)
					m_cur_letter_index[m_cur_name_cursor] = m_valid_letters.Length - 1;
			}




			if (InputUtility.onKeyTriggered("MENUSELECT") && m_after_endgame)
            {
				m_exiting_animation = true;
			}

			if (InputUtility.onKeyTriggered(KEY_ID.KEY_SLASH))
			{
				Console.WriteLine("Saved score");
				SaveScore();
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

		private void UpdateNameCursorPos()
        {
			ECS.SetGlobalPosition(m_cur_name_cursor_id, new Vector3(m_up_down_pos.X + m_up_down_offset * m_cur_name_cursor, -0.15f, m_up_down_pos.Z));
        }

		private void UpdateNameLetterIndex()
        {
			//GameUtilities.UpdateText(ScoreList[(uint)cur_index][i].m_letter_1, temp_c_array[0].ToString());

			//if(m_cur_name_cursor)
			ScoreEntry asd = ScoreList[m_index][m_name_placement];
			string new_name = "";
			switch (m_cur_name_cursor)
            {
				case 0:
					new_name = "" + m_valid_letters[m_cur_letter_index[m_cur_name_cursor]] + asd.m_name[1] + asd.m_name[2];
					break;
				case 1:
					new_name = "" + asd.m_name[0] + m_valid_letters[m_cur_letter_index[m_cur_name_cursor]]  + asd.m_name[2];
					break;
				case 2:
					new_name = "" + asd.m_name[0] + asd.m_name[1] + m_valid_letters[m_cur_letter_index[m_cur_name_cursor]];
					break;

            }
			asd.m_name = new_name;
			ScoreList[m_index][m_name_placement] = asd;

            //Console.WriteLine("===============================================");
            //Console.WriteLine(m_valid_letters[m_cur_letter_index[m_cur_name_cursor]]);
            //Console.WriteLine(asd);
            //Console.WriteLine(ScoreList[m_index][m_name_placement].m_name);
            //Console.WriteLine("===============================================");
        }
	}
}
