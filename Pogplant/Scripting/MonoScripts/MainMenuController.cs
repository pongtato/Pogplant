/*****************************************************************************/
/*!
\file	MainMenuController.cs
\author Lee Wei Liang
\par	email: weiliang.lee\@digipen.edu
\details
	Main Menu script for keyboard and controller to navigate to game scene.

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
	public class MainMenuController : MonoBehaviour
	{
		//========================================================//
		//Main menu
		//========================================================//

		public enum BUTTONS
		{
			START_GAME = 0,
			HOW_TO_PLAY = 1,
			SETTINGS = 2,
			HIGH_SCORE = 3,
			CREDITS = 4,
			QUIT_GAME = 5
		}

		private int active_index = 0;
		private Dictionary<string, GameObject> buttonMap = new Dictionary<string, GameObject>();

		private Dictionary<string, uint> buttonfadedMap = new Dictionary<string, uint>();
		private Dictionary<int, string> buttonNameMap = new Dictionary<int, string>();

		private int number_of_buttons = 0;

		//UI elements
		uint logo_id;
		uint bg1_id;   
		uint bg2_id;    //With the controls
		uint any_key_to_continue_id;
		uint arrow_id;
		const float arrow_anim_speed = 0.75f;
		const float menu_buttons_default_x = 0.7f;
		bool move_arrow_left;

		//Anim sequences
		enum MENU_STATE
		{
			LOGO_SHRINK_EXPAND,
			ANY_KEY_FLASHING,
			MOVE_LOGO_TOP_LEFT,
			BUTTONS_SLIDE_IN,
			INPUT_READY,
			IN_SUB_MENU
		}
		MENU_STATE menu_state;

		//Logo expand
		const float max_logo_expand_scale = 1.4f;
		const float final_logo_expand_scale = 0.9f;
		const float logo_expand_anim_speed = 4.0f;
		bool shrink_logo = false;

		//Press any key flashing
		const float any_key_scale_speed = 5.0f;
		const float any_key_expanded_scale = 0.52f;
		const float any_key_default_scale = 0.48f;
		const float any_key_spin_speed = 2.0f;
		const float any_key_move_out_speed = 4.0f;
		bool any_key_expand;
		bool begin_any_key_spin;

		//Logo move to top left
		const float logo_move_top_left_anim_speed = 4.0f;

		//Buttons sliding in
		const float slide_in_speed = 4.0f;

		uint settings_menu_id;

		uint m_null_entity;
		//Audio tracks
		/// <summary>
		/// 0. BGM
		/// 1. Select
		/// 2. Up
		/// 3. Down
		/// 4. Back
		/// </summary>

		public MainMenuController()
		{
			
		}

		void AddButton(string button_name)
		{
			uint temp_id = ECS.FindChildEntityWithName(entityID, button_name);
			if (temp_id != m_null_entity)
			{
				Vector3 pos = new Vector3();
				Vector3 rot = new Vector3();
				Vector3 scale = new Vector3();

				ECS.GetTransformECS(temp_id, ref pos, ref rot, ref scale);
				buttonMap.Add(button_name, new GameObject(temp_id, new Transform(pos, rot, scale), button_name));
				buttonNameMap.Add(number_of_buttons++, button_name);
			}
			else
			{
				Console.WriteLine(button_name + " is a null entity!");
			}
		}

		void AddButtonFaded(string button_name)
        {
			uint temp_id = ECS.FindEntityWithName(button_name);
			if (temp_id != m_null_entity)
            {
				buttonfadedMap.Add(button_name, temp_id);
				ECS.SetActive(temp_id, false);
			}
			else
            {
				Console.WriteLine(button_name + " is a null entity!");
            }
		}

		public override void Init(ref uint _entityID)
		{
			entityID = _entityID;
			m_null_entity = ECS.GetNull();

			ECS.PlayAudio(entityID, 0, "BGM");

			AddButton("Start Button");			//0
			AddButton("How To Play Button");    //1
			AddButton("Settings Button");		//2
			AddButton("High Score Button");		//3
			AddButton("Credits Button");		//4
			AddButton("Quit Button");			//5

			Vector3 pos = new Vector3();
			Vector3 rot = new Vector3();
			Vector3 scale = new Vector3();

			uint ap = ECS.FindChildEntityWithName(entityID, "Arrow Parent");
			ECS.GetTransformECS(ap, ref pos, ref rot, ref scale);
			buttonMap.Add("Arrow Parent", new GameObject(ap, new Transform(pos, rot, scale), "Arrow Parent"));

			arrow_id = ECS.FindChildEntityWithName(ap, "Arrow");
			ECS.SetActive(arrow_id, false);

			menu_state = MENU_STATE.LOGO_SHRINK_EXPAND;
			shrink_logo = false;

			any_key_to_continue_id = ECS.FindEntityWithName("Any Key Continue");
			ECS.SetActive(any_key_to_continue_id, false);

			logo_id = ECS.FindEntityWithName("Logo");
			bg1_id = ECS.FindEntityWithName("Background 1");
			bg2_id = ECS.FindEntityWithName("Background 2");
			ECS.SetActive(bg2_id, false);

			settings_menu_id = ECS.FindEntityWithName("Settings Menu");

			AddButtonFaded("Start Button Faded");
			AddButtonFaded("How To Play Button Faded");
			AddButtonFaded("Settings Button Faded");
			AddButtonFaded("High Score Button Faded");
			AddButtonFaded("Credits Button Faded");
			AddButtonFaded("Quit Button Faded");

			begin_any_key_spin = false;
			shrink_logo = false;
			any_key_expand = true;
		}

		public override void Start()
		{

		}

		public override void Update(float dt)
		{
			if (InputUtility.onKeyTriggered("ESCAPE"))
			{
				menu_state = MENU_STATE.INPUT_READY;
				ECS.PlayAudio(entityID, 4, "SFX");
			}

			if (menu_state == MENU_STATE.INPUT_READY)
			{
				UpdateInputs();
				UpdateArrowAnimation(dt);
			}
			else
			{
				UpdateAnimSequence(dt);
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

		void UpdateAnimSequence(float dt)
		{
			switch (menu_state)
			{
				case MENU_STATE.LOGO_SHRINK_EXPAND:
					{
						if (!shrink_logo)
						{
							ECS.SetGlobalScale(logo_id, Vector3.Lerp(ECS.GetGlobalScale(logo_id), new Vector3(max_logo_expand_scale, max_logo_expand_scale, 1.0f), dt * logo_expand_anim_speed));

							if (ECS.GetGlobalScale(logo_id).X >= max_logo_expand_scale - 0.1f)
							{
								shrink_logo = true;
							}
						}
						else
						{
							ECS.SetGlobalScale(logo_id, Vector3.Lerp(ECS.GetGlobalScale(logo_id), new Vector3(final_logo_expand_scale, final_logo_expand_scale, 1.0f), dt * logo_expand_anim_speed));

							if (ECS.GetGlobalScale(logo_id).X <= final_logo_expand_scale + 0.1f)
							{
								menu_state = MENU_STATE.ANY_KEY_FLASHING;
								ECS.SetActive(any_key_to_continue_id, true);
							}
						}
					}
					break;
				case MENU_STATE.ANY_KEY_FLASHING:
					{
						//Button spin out
						if (InputUtility.onAnyKey() && !begin_any_key_spin)
						{
							begin_any_key_spin = true;
							ECS.PlayAudio(entityID, 1, "SFX");
						}
						else
						{
							if (!begin_any_key_spin)
							{
								if (any_key_expand)
								{
									ECS.SetGlobalScale(any_key_to_continue_id, Vector3.Lerp(new Vector3(ECS.GetGlobalScale(any_key_to_continue_id).X, ECS.GetGlobalScale(any_key_to_continue_id).Y, 1.0f), new Vector3(any_key_expanded_scale, any_key_expanded_scale, 1.0f), any_key_scale_speed * dt));
									if (ECS.GetGlobalScale(any_key_to_continue_id).X >= any_key_expanded_scale - 0.01f)
									{
										any_key_expand = false;
									}
								}
								else
								{
									ECS.SetGlobalScale(any_key_to_continue_id, Vector3.Lerp(new Vector3(ECS.GetGlobalScale(any_key_to_continue_id).X, ECS.GetGlobalScale(any_key_to_continue_id).Y, 1.0f), new Vector3(any_key_default_scale, any_key_default_scale, 1.0f), any_key_scale_speed * dt));
									if (ECS.GetGlobalScale(any_key_to_continue_id).X <= any_key_default_scale + 0.01f)
									{
										any_key_expand = true;
									}
								}
							}
						}

						if (begin_any_key_spin)
						{
							//ECS.SetGlobalPosition(any_key_to_continue_id, Vector3.Lerp(ECS.GetGlobalPosition(any_key_to_continue_id), new Vector3(-1.1f, 0.7f, 0.0f), any_key_move_out_speed * dt));
							ECS.SetGlobalRotation(any_key_to_continue_id, Vector3.Lerp(ECS.GetGlobalRotation(any_key_to_continue_id), new Vector3(0.0f, 0.0f, 540.0f), any_key_spin_speed * dt));
							ECS.SetGlobalScale(any_key_to_continue_id, Vector3.Lerp(ECS.GetGlobalScale(any_key_to_continue_id), new Vector3(any_key_default_scale * 0.5f, any_key_default_scale * 0.5f, 1.0f), any_key_move_out_speed * dt));
						}

						if (ECS.GetGlobalScale(any_key_to_continue_id).X <= 0.26f)
						{
							menu_state = MENU_STATE.MOVE_LOGO_TOP_LEFT;
							ECS.SetActive(any_key_to_continue_id, false);
						}
					}
					break;
				case MENU_STATE.MOVE_LOGO_TOP_LEFT:
					{
						if (ECS.GetGlobalPosition(logo_id).X < -0.19f)
						{
							menu_state = MENU_STATE.BUTTONS_SLIDE_IN;
						}
						else
						{
							ECS.SetGlobalPosition(logo_id, Vector3.Lerp(ECS.GetGlobalPosition(logo_id), new Vector3(-0.2f, 0.18f), dt * logo_move_top_left_anim_speed));
						}
					}
					break;
				case MENU_STATE.BUTTONS_SLIDE_IN:
					{
						foreach (KeyValuePair<string, GameObject> entry in buttonMap)
						{
							if (entry.Key == "Arrow Parent")
								continue;

							ECS.SetGlobalPosition(entry.Value.id, Vector3.Lerp(ECS.GetGlobalPosition(entry.Value.id), new Vector3(0.38f, entry.Value.transform.Position.Y, -entry.Value.transform.Position.Z), slide_in_speed * dt));
						}

						if (ECS.GetGlobalPosition(buttonMap["Quit Button"].id).X <= 0.381f)
						{
							menu_state = MENU_STATE.INPUT_READY;
							ECS.SetActive(bg1_id, false);
							ECS.SetActive(bg2_id, true);
							ECS.SetActive(arrow_id, true);
						}
					}
					break;
			}
		}
			
		void UpdateButton(string button_name)
        {
			//update position of arrow
			ECS.SetTransformECS(buttonMap["Arrow Parent"].id,
							new Vector3(GetXFromButton(active_index), buttonMap[button_name].transform.Position.Y, buttonMap[button_name].transform.Position.Z),
							buttonMap["Arrow Parent"].transform.Rotation,
							buttonMap["Arrow Parent"].transform.Scale);

			foreach (KeyValuePair<string, GameObject> entry in buttonMap)
            {
				if (entry.Key == "Arrow Parent")
					continue;

				if (entry.Key == button_name)
                {
					ECS.SetActive(entry.Value.id, true);
					ECS.SetActive(buttonfadedMap[entry.Key + " Faded"], false);
                }
				else
                {
					ECS.SetActive(entry.Value.id, false);
					ECS.SetActive(buttonfadedMap[entry.Key + " Faded"], true);
				}
            }
		}

		void UpdateInputs()
		{
			//Key input
			if (InputUtility.onKeyTriggered("MENUUP"))
			{
				ECS.PlayAudio(entityID, 2, "SFX");

				--active_index;
				
				if (active_index < (int)BUTTONS.START_GAME)
					active_index = (int)BUTTONS.QUIT_GAME;

				//Console.WriteLine("Active index is: " + active_index);
			}
			else if (InputUtility.onKeyTriggered("MENUDOWN"))
			{
				ECS.PlayAudio(entityID, 3, "SFX");

				++active_index;

				if (active_index > (int)BUTTONS.QUIT_GAME)
					active_index = (int)BUTTONS.START_GAME;
				//Console.WriteLine("Active index is: " + active_index);
			}

			UpdateButton(buttonNameMap[active_index]);
		
			if (InputUtility.onKeyTriggered("MENUSELECT"))
			{
				ECS.PlayAudio(entityID, 1, "SFX");

				//Start or quit game based on index
				switch ((BUTTONS)active_index)
				{
					case BUTTONS.START_GAME:
						GameUtilities.LoadScene("CutScene");
						break;
					case BUTTONS.HOW_TO_PLAY:
						HowToPlayMenu.EnableHowToPlayMenu(true);
						GameUtilities.PauseScene();
						break;
					case BUTTONS.SETTINGS:
						menu_state = MENU_STATE.IN_SUB_MENU;
						SettingsMenu.refresh = true;
						ECS.SetActive(settings_menu_id, true);
						break;
					case BUTTONS.HIGH_SCORE:
						HighScoreMenu.Enable(true);
						GameUtilities.PauseScene();
						break;
					case BUTTONS.CREDITS:
						break;
					case BUTTONS.QUIT_GAME:
						GameUtilities.ExitScene();
						break;
					default:
						break;
				}
			}
		}

		void UpdateArrowAnimation(float dt)
		{
			//Move left and right
			Vector3 defaultPos = new Vector3(ECS.GetGlobalPosition(arrow_id).X, ECS.GetGlobalPosition(arrow_id).Y, ECS.GetGlobalPosition(arrow_id).Z);
			
			//Move left
			if (ECS.GetGlobalPosition(arrow_id).X > GetXFromButton(active_index) + 0.005f)
			{
				move_arrow_left = true;
			}
			else if (ECS.GetGlobalPosition(arrow_id).X < GetXFromButton(active_index) - 0.005f)
			{
				move_arrow_left = false;
			}

			if (move_arrow_left)
			{
				ECS.SetGlobalPosition(arrow_id, Vector3.Lerp(new Vector3(defaultPos.X, defaultPos.Y, defaultPos.Z), new Vector3(defaultPos.X - 0.025f, defaultPos.Y, defaultPos.Z), dt * arrow_anim_speed));
			}
			else
			{
				ECS.SetGlobalPosition(arrow_id, Vector3.Lerp(new Vector3(defaultPos.X, defaultPos.Y, defaultPos.Z), new Vector3(defaultPos.X + 0.025f, defaultPos.Y, defaultPos.Z), dt * arrow_anim_speed));
			}
		}

		void FlashingAnimAnyKey(float dt)
		{
			//ECS.GetComponent<Renderer>(any_key_to_continue_id).
		}

		float GetXFromButton(int index)
		{
			//vinceen
			//i assume this for fine tuning the x position
			switch ((BUTTONS)index)
			{
				case BUTTONS.START_GAME:
					return ECS.GetGlobalPosition(buttonMap["Start Button"].id).X - 0.130f;
				case BUTTONS.HOW_TO_PLAY:
					return ECS.GetGlobalPosition(buttonMap["How To Play Button"].id).X - 0.150f;
				case BUTTONS.SETTINGS:
					return ECS.GetGlobalPosition(buttonMap["Settings Button"].id).X - 0.09f;
				case BUTTONS.HIGH_SCORE:
					return ECS.GetGlobalPosition(buttonMap["High Score Button"].id).X - 0.12f;
				case BUTTONS.CREDITS:
					return ECS.GetGlobalPosition(buttonMap["Credits Button"].id).X - 0.07f;
					//return ECS.GetGlobalPosition(credits_button_id).X - 0.125f;
				case BUTTONS.QUIT_GAME:
					return ECS.GetGlobalPosition(buttonMap["Quit Button"].id).X - 0.10f;

			}

			return 0.0f;
		}
	}
}
