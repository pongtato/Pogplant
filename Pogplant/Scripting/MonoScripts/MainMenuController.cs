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
            CREDITS = 3,
            QUIT_GAME = 4
        }

        private int active_index = 0;
        private Dictionary<string, GameObject> buttonMap = new Dictionary<string, GameObject>();

        //Menu buttons
        uint start_button_id;
        uint start_button_faded_id;
        uint how_to_play_button_id;
        uint how_to_play_button_faded_id;
        uint settings_button_id;
        uint settings_button_faded_id;
        uint credits_button_id;
        uint credits_button_faded_id;
        uint quit_button_id;
        uint quit_button_faded_id;

        //UI elements
        uint logo_id;
        uint bg1_id;   
        uint bg2_id;    //With the controls
        uint any_key_to_continue_id;
        uint arrow_id;
        const float arrow_anim_speed = 0.5f;
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
        const float max_logo_expand_scale = 2.2f;
        const float final_logo_expand_scale = 1.9f;
        const float logo_expand_anim_speed = 6.0f;
        bool shrink_logo = false;

        //Press any key flashing
        const float any_key_scale_speed = 0.5f;
        const float any_key_expanded_scale = 1.1f;
        const float any_key_spin_speed = 2.0f;
        const float any_key_move_out_speed = 4.0f;
        bool any_key_expand;
        bool begin_any_key_spin;

        //Logo move to top left
        const float logo_move_top_left_anim_speed = 4.0f;

        //Buttons sliding in
        const float slide_in_speed = 4.0f;

        uint settings_menu_id;

        public MainMenuController()
        {
            
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            ECS.PlayAudio(entityID, 0, "BGM");

            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();

            start_button_id = ECS.FindChildEntityWithName(entityID, "Start Button");
            ECS.GetTransformECS(start_button_id, ref pos, ref rot, ref scale);
            buttonMap.Add("Start Button", new GameObject(start_button_id, new Transform(pos, rot, scale), "Start Button"));

            how_to_play_button_id = ECS.FindChildEntityWithName(entityID, "How To Play Button");
            ECS.GetTransformECS(how_to_play_button_id, ref pos, ref rot, ref scale);
            buttonMap.Add("How To Play Button", new GameObject(how_to_play_button_id, new Transform(pos, rot, scale), "How To Play Button"));

            settings_button_id = ECS.FindChildEntityWithName(entityID, "Settings Button");
            ECS.GetTransformECS(settings_button_id, ref pos, ref rot, ref scale);
            buttonMap.Add("Settings Button", new GameObject(settings_button_id, new Transform(pos, rot, scale), "Settings Button"));

            credits_button_id = ECS.FindChildEntityWithName(entityID, "Credits Button");
            ECS.GetTransformECS(credits_button_id, ref pos, ref rot, ref scale);
            buttonMap.Add("Credits Button", new GameObject(credits_button_id, new Transform(pos, rot, scale), "Credits Button "));

            quit_button_id = ECS.FindChildEntityWithName(entityID, "Quit Button");
            ECS.GetTransformECS(quit_button_id, ref pos, ref rot, ref scale);
            buttonMap.Add("Quit Button", new GameObject(quit_button_id, new Transform(pos, rot, scale), "Quit Button"));

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

            start_button_faded_id = ECS.FindEntityWithName("Start Button Faded");
            how_to_play_button_faded_id = ECS.FindEntityWithName("How To Play Button Faded");
            settings_button_faded_id = ECS.FindEntityWithName("Settings Button Faded");
            credits_button_faded_id = ECS.FindEntityWithName("Credits Button Faded");
            quit_button_faded_id = ECS.FindEntityWithName("Quit Button Faded");

            ECS.SetActive(start_button_faded_id, false);
            ECS.SetActive(how_to_play_button_faded_id, false);
            ECS.SetActive(settings_button_faded_id, false);
            ECS.SetActive(credits_button_faded_id, false);
            ECS.SetActive(quit_button_faded_id, false);
        }

        public override void Start()
        {

        }

        public override void Update(float dt)
        {
            if (InputUtility.onKeyTriggered("ESCAPE") && menu_state == MENU_STATE.IN_SUB_MENU)
            {
                menu_state = MENU_STATE.INPUT_READY;
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
                            ECS.SetGlobalScale(logo_id, Vector3.Lerp(ECS.GetGlobalScale(logo_id), new Vector3(max_logo_expand_scale, max_logo_expand_scale), dt * logo_expand_anim_speed));

                            if (ECS.GetGlobalScale(logo_id).X >= max_logo_expand_scale - 0.1f)
                            {
                                shrink_logo = true;
                            }
                        }
                        else
                        {
                            ECS.SetGlobalScale(logo_id, Vector3.Lerp(ECS.GetGlobalScale(logo_id), new Vector3(final_logo_expand_scale, final_logo_expand_scale), dt * logo_expand_anim_speed));

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
                        if (InputUtility.onAnyKey())
                        {
                            begin_any_key_spin = true;
                        }
                        else
                        {
                            if (!begin_any_key_spin)
                            {
                                if (any_key_expand)
                                {
                                    ECS.SetGlobalScale(any_key_to_continue_id, Vector3.Lerp(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(any_key_expanded_scale, any_key_expanded_scale, 1.0f), any_key_scale_speed * dt));

                                    if (ECS.GetGlobalScale(any_key_to_continue_id).X >= any_key_expanded_scale - 0.01f)
                                    {
                                        any_key_expand = false;
                                    }
                                }
                                else
                                {
                                    ECS.SetGlobalScale(any_key_to_continue_id, Vector3.Lerp(new Vector3(any_key_expanded_scale, any_key_expanded_scale, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), any_key_scale_speed * dt));

                                    if (ECS.GetGlobalScale(any_key_to_continue_id).X <= 0.99f)
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
                            ECS.SetGlobalScale(any_key_to_continue_id, Vector3.Lerp(ECS.GetGlobalScale(any_key_to_continue_id), new Vector3(0.5f, 0.5f, 0.5f), any_key_move_out_speed * dt));
                        }

                        if (ECS.GetGlobalScale(any_key_to_continue_id).X <= 0.57f)
                        {
                            menu_state = MENU_STATE.MOVE_LOGO_TOP_LEFT;
                            ECS.SetActive(any_key_to_continue_id, false);
                        }
                    }
                    break;
                case MENU_STATE.MOVE_LOGO_TOP_LEFT:
                    {
                        if (ECS.GetGlobalPosition(logo_id).X < -0.39f)
                        {
                            menu_state = MENU_STATE.BUTTONS_SLIDE_IN;
                        }
                        else
                        {
                            ECS.SetGlobalPosition(logo_id, Vector3.Lerp(ECS.GetGlobalPosition(logo_id), new Vector3(-0.4f, 0.3f), dt * logo_move_top_left_anim_speed));
                        }
                    }
                    break;
                case MENU_STATE.BUTTONS_SLIDE_IN:
                    {
                        ECS.SetGlobalPosition(start_button_id, Vector3.Lerp(ECS.GetGlobalPosition(start_button_id), new Vector3(0.7f, -0.25f, 0f), slide_in_speed * dt));
                        ECS.SetGlobalPosition(how_to_play_button_id, Vector3.Lerp(ECS.GetGlobalPosition(how_to_play_button_id), new Vector3(0.7f, -0.4f, 0f), slide_in_speed * dt));
                        ECS.SetGlobalPosition(settings_button_id, Vector3.Lerp(ECS.GetGlobalPosition(settings_button_id), new Vector3(0.7f, -0.55f, 0f), slide_in_speed * dt));
                        ECS.SetGlobalPosition(credits_button_id, Vector3.Lerp(ECS.GetGlobalPosition(credits_button_id), new Vector3(0.7f, -0.7f, 0f), slide_in_speed * dt));
                        ECS.SetGlobalPosition(quit_button_id, Vector3.Lerp(ECS.GetGlobalPosition(quit_button_id), new Vector3(0.7f, -0.85f, 0f), slide_in_speed * dt));

                        if (ECS.GetGlobalPosition(quit_button_id).X <= 0.701f)
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

        void UpdateInputs()
        {
            //Key input
            if (InputUtility.onKeyTriggered("MENUUP"))
            {
                --active_index;
                
                if (active_index < (int)BUTTONS.START_GAME)
                {
                    active_index = (int)BUTTONS.QUIT_GAME;

                }
                //Console.WriteLine("Active index is: " + active_index);
            }
            else if (InputUtility.onKeyTriggered("MENUDOWN"))
            {
                ++active_index;

                if (active_index > (int)BUTTONS.QUIT_GAME)
                {
                    active_index = (int)BUTTONS.START_GAME;

                }
            }
            
            switch (active_index)
            {
                case 0:
                    {
                        ECS.SetTransformECS(buttonMap["Arrow Parent"].id,
                            new Vector3(GetXFromButton(active_index), buttonMap["Start Button"].transform.Position.Y, buttonMap["Start Button"].transform.Position.Z),
                            buttonMap["Arrow Parent"].transform.Rotation,
                            buttonMap["Arrow Parent"].transform.Scale);
                        
                        //Set only the selected button to be non faded
                        ECS.SetActive(start_button_id, true);
                        ECS.SetActive(how_to_play_button_id, false);
                        ECS.SetActive(settings_button_id, false);
                        ECS.SetActive(credits_button_id, false);
                        ECS.SetActive(quit_button_id, false);

                        ECS.SetActive(start_button_faded_id, false);
                        ECS.SetActive(how_to_play_button_faded_id, true);
                        ECS.SetActive(settings_button_faded_id, true);
                        ECS.SetActive(credits_button_faded_id, true);
                        ECS.SetActive(quit_button_faded_id, true);
                    }
                    break;
                case 1:
                    {
                        ECS.SetTransformECS(buttonMap["Arrow Parent"].id,
                            new Vector3(GetXFromButton(active_index), buttonMap["How To Play Button"].transform.Position.Y, buttonMap["How To Play Button"].transform.Position.Z),
                            buttonMap["Arrow Parent"].transform.Rotation,
                            buttonMap["Arrow Parent"].transform.Scale);

                        //Set only the selected button to be non faded
                        ECS.SetActive(start_button_id, false);
                        ECS.SetActive(how_to_play_button_id, true);
                        ECS.SetActive(settings_button_id, false);
                        ECS.SetActive(credits_button_id, false);
                        ECS.SetActive(quit_button_id, false);

                        ECS.SetActive(start_button_faded_id, true);
                        ECS.SetActive(how_to_play_button_faded_id, false);
                        ECS.SetActive(settings_button_faded_id, true);
                        ECS.SetActive(credits_button_faded_id, true);
                        ECS.SetActive(quit_button_faded_id, true);
                    }
                    break;
                case 2:
                    {
                        ECS.SetTransformECS(buttonMap["Arrow Parent"].id,
                            new Vector3(GetXFromButton(active_index), buttonMap["Settings Button"].transform.Position.Y, buttonMap["Settings Button"].transform.Position.Z),
                            buttonMap["Arrow Parent"].transform.Rotation,
                            buttonMap["Arrow Parent"].transform.Scale);

                        //Set only the selected button to be non faded
                        ECS.SetActive(start_button_id, false);
                        ECS.SetActive(how_to_play_button_id, false);
                        ECS.SetActive(settings_button_id, true);
                        ECS.SetActive(credits_button_id, false);
                        ECS.SetActive(quit_button_id, false);

                        ECS.SetActive(start_button_faded_id, true);
                        ECS.SetActive(how_to_play_button_faded_id, true);
                        ECS.SetActive(settings_button_faded_id, false);
                        ECS.SetActive(credits_button_faded_id, true);
                        ECS.SetActive(quit_button_faded_id, true);
                    }
                    break;
                case 3:
                    {
                        ECS.SetTransformECS(buttonMap["Arrow Parent"].id,
                            new Vector3(GetXFromButton(active_index), buttonMap["Credits Button"].transform.Position.Y, buttonMap["Credits Button"].transform.Position.Z),
                            buttonMap["Arrow Parent"].transform.Rotation,
                            buttonMap["Arrow Parent"].transform.Scale);

                        //Set only the selected button to be non faded
                        ECS.SetActive(start_button_id, false);
                        ECS.SetActive(how_to_play_button_id, false);
                        ECS.SetActive(settings_button_id, false);
                        ECS.SetActive(credits_button_id, true);
                        ECS.SetActive(quit_button_id, false);

                        ECS.SetActive(start_button_faded_id, true);
                        ECS.SetActive(how_to_play_button_faded_id, true);
                        ECS.SetActive(settings_button_faded_id, true);
                        ECS.SetActive(credits_button_faded_id, false);
                        ECS.SetActive(quit_button_faded_id, true);
                    }
                    break;
                case 4:
                    {
                        ECS.SetTransformECS(buttonMap["Arrow Parent"].id,
                            new Vector3(GetXFromButton(active_index), buttonMap["Quit Button"].transform.Position.Y, buttonMap["Quit Button"].transform.Position.Z),
                            buttonMap["Arrow Parent"].transform.Rotation,
                            buttonMap["Arrow Parent"].transform.Scale);

                        //Set only the selected button to be non faded
                        ECS.SetActive(start_button_id, false);
                        ECS.SetActive(how_to_play_button_id, false);
                        ECS.SetActive(settings_button_id, false);
                        ECS.SetActive(credits_button_id, false);
                        ECS.SetActive(quit_button_id, true);

                        ECS.SetActive(start_button_faded_id, true);
                        ECS.SetActive(how_to_play_button_faded_id, true);
                        ECS.SetActive(settings_button_faded_id, true);
                        ECS.SetActive(credits_button_faded_id, true);
                        ECS.SetActive(quit_button_faded_id, false);
                    }
                    break;
            }

            if (InputUtility.onKeyTriggered("MENUSELECT"))
            {
                //Start or quit game based on index
                switch (active_index)
                {
                    case 0:
                        GameUtilities.LoadScene("Level01_M3_SplineReimport");
                        break;
                    case 2:
                        menu_state = MENU_STATE.IN_SUB_MENU;
                        SettingsMenu.refresh = true;
                        ECS.SetActive(settings_menu_id, true);
                        break;
                    case 4:
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
            if (ECS.GetGlobalPosition(arrow_id).X > GetXFromButton(active_index) + 0.01f)
            {
                move_arrow_left = true;
            }
            else if (ECS.GetGlobalPosition(arrow_id).X < GetXFromButton(active_index) - 0.01f)
            {
                move_arrow_left = false;
            }

            if (move_arrow_left)
            {
                ECS.SetGlobalPosition(arrow_id, Vector3.Lerp(new Vector3(defaultPos.X, defaultPos.Y, defaultPos.Z), new Vector3(defaultPos.X - 0.1f, defaultPos.Y, defaultPos.Z), dt * arrow_anim_speed));
            }
            else
            {
                ECS.SetGlobalPosition(arrow_id, Vector3.Lerp(new Vector3(defaultPos.X, defaultPos.Y, defaultPos.Z), new Vector3(defaultPos.X + 0.1f, defaultPos.Y, defaultPos.Z), dt * arrow_anim_speed));
            }
        }

        void FlashingAnimAnyKey(float dt)
        {
            //ECS.GetComponent<Renderer>(any_key_to_continue_id).
        }

        float GetXFromButton(int index)
        {
            switch (index)
            {
                case 0:
                    return ECS.GetGlobalPosition(start_button_id).X - 0.250f;
                case 1:
                    return ECS.GetGlobalPosition(how_to_play_button_id).X - 0.275f;
                case 2:
                    return ECS.GetGlobalPosition(settings_button_id).X - 0.175f;
                case 3:
                    return ECS.GetGlobalPosition(credits_button_id).X - 0.125f;
                case 4:
                    return ECS.GetGlobalPosition(quit_button_id).X - 0.26f;
                    //case 0:
                    //    return menu_buttons_default_x - 0.250f;
                    //case 1:
                    //    return menu_buttons_default_x - 0.275f;
                    //case 2:
                    //    return menu_buttons_default_x - 0.175f;
                    //case 3:
                    //    return menu_buttons_default_x - 0.125f;
                    //case 4:
                    //    return menu_buttons_default_x - 0.26f;
            }

            return 0.0f;
        }
    }
}
