using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class PauseMenu : PauseBehaviour
    {
        public static PauseMenu m_singleton;

        enum PAUSE_MENU_BUTTONS
        {
            RESUME_GAME = 0,
            HOW_TO_PLAY = 1,
            SETTINGS = 2,
            RETURN_TO_MENU = 3,
            QUIT_GAME
        }

        enum CONFIRMATION_MENU_BUTTONS
        {
            NO = 0,
            YES = 1
        }

        public enum MENU_STATE
        {
            INACTIVE,
            OPENING_MENU,
            INPUT_READY,
            CONFIRMATION,
            QUIT_CONFIRMATION,
            IN_SUB_MENU
        }

        bool enabled;

        public static MENU_STATE menu_state;

        uint pause_menu_id;
        private Dictionary<string, GameObject> button_map = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> faded_button_map = new Dictionary<string, GameObject>();

        uint confirm_menu_id;
        private Dictionary<string, GameObject> confirm_button_map = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> faded_confirm_button_map = new Dictionary<string, GameObject>();

        int active_index;
        int confirmation_index;
        int quit_confirmation_index;

        uint quit_confirm_menu_id;
        private Dictionary<string, GameObject> quit_confirm_button_map = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> quit_faded_confirm_button_map = new Dictionary<string, GameObject>();

        uint settings_menu_id;
        const float opening_speed = 8.0f;
        bool pulling_down;

        public bool can_pause;

        //Audio tracks
        /// <summary>
        /// 0. Select
        /// 1. Up
        /// 2. Down
        /// 3. Back
        /// 4. Scrolling
        /// </summary>

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            m_singleton = this;

            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();

            pause_menu_id = ECS.FindEntityWithName("Pause Menu");
            confirm_menu_id = ECS.FindEntityWithName("Confirmation Menu");
            settings_menu_id = ECS.FindEntityWithName("Settings Menu");
            quit_confirm_menu_id = ECS.FindEntityWithName("Quit Confirmation Menu");

            //Pause menu full opacity
            uint resume_game_button_id = ECS.FindChildEntityWithName(pause_menu_id, "Resume Game Button");
            ECS.GetTransformECS(resume_game_button_id, ref pos, ref rot, ref scale);
            button_map.Add("Resume Game Button", new GameObject(resume_game_button_id, new Transform(pos, rot, scale), "Resume Game Button"));

            uint how_to_play_button_id = ECS.FindChildEntityWithName(pause_menu_id, "How To Play Button");
            ECS.GetTransformECS(how_to_play_button_id, ref pos, ref rot, ref scale);
            button_map.Add("How To Play Button", new GameObject(how_to_play_button_id, new Transform(pos, rot, scale), "How To Play Button"));
            //ECS.SetActive(how_to_play_button_id, false);

            uint settings_button_id = ECS.FindChildEntityWithName(pause_menu_id, "Settings Button");
            ECS.GetTransformECS(settings_button_id, ref pos, ref rot, ref scale);
            button_map.Add("Settings Button", new GameObject(settings_button_id, new Transform(pos, rot, scale), "Settings Button"));

            uint main_menu_button_id = ECS.FindChildEntityWithName(pause_menu_id, "Main Menu Button");
            ECS.GetTransformECS(main_menu_button_id, ref pos, ref rot, ref scale);
            button_map.Add("Main Menu Button", new GameObject(main_menu_button_id, new Transform(pos, rot, scale), "Main Menu Button"));

            uint quit_game_button_id = ECS.FindChildEntityWithName(pause_menu_id, "Quit Game Button");
            ECS.GetTransformECS(quit_game_button_id, ref pos, ref rot, ref scale);
            button_map.Add("Quit Game Button", new GameObject(quit_game_button_id, new Transform(pos, rot, scale), "Quit Game Button"));

            //Pause menu faded
            uint resume_game_button_faded_id = ECS.FindChildEntityWithName(pause_menu_id, "Resume Game Button Faded");
            ECS.GetTransformECS(resume_game_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map.Add("Resume Game Button Faded", new GameObject(resume_game_button_faded_id, new Transform(pos, rot, scale), "Resume Game Button Faded"));

            uint how_to_play_button_faded_id = ECS.FindChildEntityWithName(pause_menu_id, "How To Play Button Faded");
            ECS.GetTransformECS(how_to_play_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map.Add("How To Play Button Faded", new GameObject(how_to_play_button_faded_id, new Transform(pos, rot, scale), "How To Play Button Faded"));
            //ECS.SetActive(how_to_play_button_faded_id, false);

            uint settings_button_faded_id = ECS.FindChildEntityWithName(pause_menu_id, "Settings Button Faded");
            ECS.GetTransformECS(settings_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map.Add("Settings Button Faded", new GameObject(settings_button_faded_id, new Transform(pos, rot, scale), "Settings Button Faded"));

            uint main_menu_button_faded_id = ECS.FindChildEntityWithName(pause_menu_id, "Main Menu Button Faded");
            ECS.GetTransformECS(main_menu_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map.Add("Main Menu Button Faded", new GameObject(main_menu_button_faded_id, new Transform(pos, rot, scale), "Main Menu Button Faded"));

            uint quit_game_button_faded_id = ECS.FindChildEntityWithName(pause_menu_id, "Quit Game Button Faded");
            ECS.GetTransformECS(quit_game_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map.Add("Quit Game Button Faded", new GameObject(quit_game_button_faded_id, new Transform(pos, rot, scale), "Quit Game Button Faded"));

            //Confirmation menu full opacity
            uint confirm_yes_button_id = ECS.FindChildEntityWithName(confirm_menu_id, "Confirm Yes Button");
            ECS.GetTransformECS(confirm_yes_button_id, ref pos, ref rot, ref scale);
            confirm_button_map.Add("Confirm Yes Button", new GameObject(confirm_yes_button_id, new Transform(pos, rot, scale), "Confirm Yes Button"));

            uint confirm_no_button_id = ECS.FindChildEntityWithName(confirm_menu_id, "Confirm No Button");
            ECS.GetTransformECS(confirm_no_button_id, ref pos, ref rot, ref scale);
            confirm_button_map.Add("Confirm No Button", new GameObject(confirm_no_button_id, new Transform(pos, rot, scale), "Confirm No Button"));

            //Confirmation menu faded
            uint confirm_yes_button_faded_id = ECS.FindChildEntityWithName(confirm_menu_id, "Confirm Yes Button Faded");
            ECS.GetTransformECS(confirm_yes_button_faded_id, ref pos, ref rot, ref scale);
            faded_confirm_button_map.Add("Confirm Yes Button Faded", new GameObject(confirm_yes_button_faded_id, new Transform(pos, rot, scale), "Confirm Yes Button Faded"));

            uint confirm_no_button_faded_id = ECS.FindChildEntityWithName(confirm_menu_id, "Confirm No Button Faded");
            ECS.GetTransformECS(confirm_no_button_faded_id, ref pos, ref rot, ref scale);
            faded_confirm_button_map.Add("Confirm No Button Faded", new GameObject(confirm_no_button_faded_id, new Transform(pos, rot, scale), "Confirm No Button Faded"));


            //Quit Confirmation menu full opacity
            uint quit_confirm_yes_button_id = ECS.FindChildEntityWithName(quit_confirm_menu_id, "Quit Confirm Yes Button");
            ECS.GetTransformECS(quit_confirm_yes_button_id, ref pos, ref rot, ref scale);
            quit_confirm_button_map.Add("Quit Confirm Yes Button", new GameObject(quit_confirm_yes_button_id, new Transform(pos, rot, scale), "Quit Confirm Yes Button"));

            uint quit_confirm_no_button_id = ECS.FindChildEntityWithName(quit_confirm_menu_id, "Quit Confirm No Button");
            ECS.GetTransformECS(quit_confirm_no_button_id, ref pos, ref rot, ref scale);
            quit_confirm_button_map.Add("Quit Confirm No Button", new GameObject(quit_confirm_no_button_id, new Transform(pos, rot, scale), "Quit Confirm No Button"));

            //Quit Confirmation menu faded
            uint quit_confirm_yes_button_faded_id = ECS.FindChildEntityWithName(quit_confirm_menu_id, "Quit Confirm Yes Button Faded");
            ECS.GetTransformECS(quit_confirm_yes_button_faded_id, ref pos, ref rot, ref scale);
            quit_faded_confirm_button_map.Add("Quit Confirm Yes Button Faded", new GameObject(quit_confirm_yes_button_faded_id, new Transform(pos, rot, scale), "Quit Confirm Yes Button Faded"));

            uint quit_confirm_no_button_faded_id = ECS.FindChildEntityWithName(quit_confirm_menu_id, "Quit Confirm No Button Faded");
            ECS.GetTransformECS(quit_confirm_no_button_faded_id, ref pos, ref rot, ref scale);
            quit_faded_confirm_button_map.Add("Quit Confirm No Button Faded", new GameObject(quit_confirm_no_button_faded_id, new Transform(pos, rot, scale), "Quit Confirm No Button Faded"));

            menu_state = MENU_STATE.INACTIVE;
            ECS.SetActive(confirm_menu_id, false);
            ECS.SetActive(quit_confirm_menu_id, false);
            ECS.SetActive(pause_menu_id, false);

            active_index = 0;
            confirmation_index = 0;
            quit_confirmation_index = 0;
            enabled = false;
            can_pause = false;
        }

        public override void Start()
        {
        }

        public override void Update(float dt)
        {
            if (InputUtility.onKeyTriggered("ESCAPE"))
            {
                ECS.PlayAudio(entityID, 3, "SFX");
                //menu_state = MENU_STATE.INPUT_READY;
            }

            if (PlayerScript.m_EnablePauseMenu)
            {
                if (!enabled && can_pause)
                {
                    enabled = true;
                    can_pause = false;
                    ECS.PlayAudio(entityID, 4, "SFX");
                    ECS.SetGlobalPosition(pause_menu_id, new Vector3(0, 1.5f, -0.9f));
                    ECS.SetActive(pause_menu_id, true);
                    pulling_down = true;
                    menu_state = MENU_STATE.OPENING_MENU;
                }
            }

            if (enabled)
            {
                switch (menu_state)
                {
                    case MENU_STATE.OPENING_MENU:
                        UpdateOpeningMenu(dt);
                        break;
                    case MENU_STATE.INPUT_READY:
                        UpdatePauseMenuInput();
                        break;
                    case MENU_STATE.INACTIVE:
                        {
                            UpdatePauseMenuButtonFade();
                            ECS.SetActive(confirm_menu_id, false);
                            menu_state = MENU_STATE.INPUT_READY;
                        }
                        break;
                    case MENU_STATE.CONFIRMATION:
                        UpdateConfirmationInput();
                        break;
                    case MENU_STATE.QUIT_CONFIRMATION:
                        UpdateQuitConfirmationInput();
                        break;
                }
            }
        }

        void UpdateOpeningMenu(float dt)
        {
            if (pulling_down)
            {
                ECS.SetGlobalPosition(pause_menu_id, Vector3.Lerp(ECS.GetGlobalPosition(pause_menu_id), new Vector3(0, 0.0f, -0.9f), opening_speed * dt));

                if (ECS.GetGlobalPosition(pause_menu_id).Y <= 0.05f)
                {
                    pulling_down = false;
                }
            }
            else
            {
                ECS.SetGlobalPosition(pause_menu_id, Vector3.Lerp(ECS.GetGlobalPosition(pause_menu_id), new Vector3(0, 0.1f, -0.9f), opening_speed * dt));

                if (ECS.GetGlobalPosition(pause_menu_id).Y >= 0.09f)
                {
                    menu_state = MENU_STATE.INPUT_READY;
                }
            }
        }

        void UpdatePauseMenuInput()
        {
            //Key input
            if (InputUtility.onKeyTriggered("MENUUP"))
            {
                ECS.PlayAudio(entityID, 1, "SFX");

                --active_index;

                if (active_index < (int)PAUSE_MENU_BUTTONS.RESUME_GAME)
                {
                    active_index = (int)PAUSE_MENU_BUTTONS.QUIT_GAME;

                }
                UpdatePauseMenuButtonFade();
            }
            else if (InputUtility.onKeyTriggered("MENUDOWN"))
            {
                ECS.PlayAudio(entityID, 2, "SFX");

                ++active_index;

                if (active_index > (int)PAUSE_MENU_BUTTONS.QUIT_GAME)
                {
                    active_index = (int)PAUSE_MENU_BUTTONS.RESUME_GAME;
                }
                UpdatePauseMenuButtonFade();
            }

            if (InputUtility.onKeyTriggered("MENUSELECT"))
            {
                ECS.PlayAudio(entityID, 0, "SFX");

                //Select option on pause menu based on index
                switch (active_index)
                {
                    case 0:
                        menu_state = MENU_STATE.INACTIVE;
                        ECS.SetActive(pause_menu_id, false);
                        enabled = false;
                        PlayerScript.m_EnablePauseMenu = false;
                        GameUtilities.ResumeScene();
                        can_pause = true;
                        AudioEngine.ResumeChannelGroup("VO");
                        break;
                    case 1:
                        menu_state = MENU_STATE.IN_SUB_MENU;
                        //PauseSettingsMenu.refresh = true;
                        HowToPlayMenu.EnableHowToPlayMenu(false);
                        HowToPlayMenu.current_state = HowToPlayMenu.ANIM_STATE.OPENING;
                        break;
                    case 2:
                        menu_state = MENU_STATE.IN_SUB_MENU;
                        PauseSettingsMenu.anim_lerp_step = 0.0f;
                        PauseSettingsMenu.current_state = PauseSettingsMenu.ANIM_STATE.OPENING;
                        PauseSettingsMenu.refresh = true;
                        ECS.SetActive(settings_menu_id, true);
                        break;
                    case 3:
                        menu_state = MENU_STATE.CONFIRMATION;
                        ECS.SetActive(confirm_menu_id, true);
                        break;
                    case 4:
                        menu_state = MENU_STATE.QUIT_CONFIRMATION;
                        ECS.SetActive(quit_confirm_menu_id, true);
                        break;
                    default:
                        break;
                }
            }
        }

        void UpdateConfirmationInput()
        {
            //Key input
            if (InputUtility.onKeyTriggered("MENULEFT"))
            {
                ECS.PlayAudio(entityID, 2, "SFX");

                --confirmation_index;

                if (confirmation_index < (int)CONFIRMATION_MENU_BUTTONS.NO)
                {
                    confirmation_index = (int)CONFIRMATION_MENU_BUTTONS.YES;
                }
                UpdateConfirmationButtonFade();
            }
            else if (InputUtility.onKeyTriggered("MENURIGHT"))
            {
                ECS.PlayAudio(entityID, 1, "SFX");

                ++confirmation_index;

                if (confirmation_index > (int)CONFIRMATION_MENU_BUTTONS.YES)
                {
                    confirmation_index = (int)CONFIRMATION_MENU_BUTTONS.NO;
                }
                UpdateConfirmationButtonFade();
            }

            if (InputUtility.onKeyTriggered("MENUSELECT"))
            {
                ECS.PlayAudio(entityID, 0, "SFX");

                //Cancel or confirm return to main menu based on index
                switch (confirmation_index)
                {
                    case 0:
                        ECS.SetActive(confirm_menu_id, false);
                        menu_state = MENU_STATE.INPUT_READY;
                        break;
                    case 1:
                        menu_state = MENU_STATE.INACTIVE;
                        GameUtilities.ResumeScene();
                        GameUtilities.LoadScene("MainMenu");
                        break;
                    default:
                        break;
                }
            }
        }

        void UpdateQuitConfirmationInput()
        {
            //Key input
            if (InputUtility.onKeyTriggered("MENULEFT"))
            {
                ECS.PlayAudio(entityID, 2, "SFX");

                --quit_confirmation_index;

                if (quit_confirmation_index < (int)CONFIRMATION_MENU_BUTTONS.NO)
                {
                    quit_confirmation_index = (int)CONFIRMATION_MENU_BUTTONS.YES;
                }
                UpdateQuitConfirmationButtonFade();
            }
            else if (InputUtility.onKeyTriggered("MENURIGHT"))
            {
                ECS.PlayAudio(entityID, 1, "SFX");

                ++quit_confirmation_index;

                if (quit_confirmation_index > (int)CONFIRMATION_MENU_BUTTONS.YES)
                {
                    quit_confirmation_index = (int)CONFIRMATION_MENU_BUTTONS.NO;
                }
                UpdateQuitConfirmationButtonFade();
            }

            if (InputUtility.onKeyTriggered("MENUSELECT"))
            {
                ECS.PlayAudio(entityID, 0, "SFX");

                //Cancel or confirm quit game based on index
                switch (quit_confirmation_index)
                {
                    case 0:
                        ECS.SetActive(quit_confirm_menu_id, false);
                        menu_state = MENU_STATE.INPUT_READY;
                        break;
                    case 1:
                        menu_state = MENU_STATE.INACTIVE;
                        GameUtilities.ExitScene();
                        break;
                    default:
                        break;
                }
            }
        }

        void UpdatePauseMenuButtonFade()
        {
            switch (active_index)
            {
                case 0:
                    {
                        //Set only the selected button to be non faded
                        ToggleButtonFade(button_map, faded_button_map, "Resume Game Button", "Resume Game Button Faded");
                    }
                    break;
                case 1:
                    {
                        //Set only the selected button to be non faded
                        ToggleButtonFade(button_map, faded_button_map, "How To Play Button", "How To Play Button Faded");
                        //ToggleButtonFade(button_map, faded_button_map, "Settings Button", "Settings Button Faded");
                    }
                    break;
                case 2:
                    {
                        //Set only the selected button to be non faded
                        ToggleButtonFade(button_map, faded_button_map, "Settings Button", "Settings Button Faded");
                        //ToggleButtonFade(button_map, faded_button_map, "Main Menu Button", "Main Menu Button Faded");
                    }
                    break;
                case 3:
                    {
                        //Set only the selected button to be non faded
                        ToggleButtonFade(button_map, faded_button_map, "Main Menu Button", "Main Menu Button Faded");
                    }
                    break;
                case 4:
                    {
                        //Set only the selected button to be non faded
                        ToggleButtonFade(button_map, faded_button_map, "Quit Game Button", "Quit Game Button Faded");
                    }
                    break;
            }
        }

        void UpdateConfirmationButtonFade()
        {
            switch (confirmation_index)
            {
                case 0:
                    //Set only the selected button to be non faded
                    ToggleButtonFade(confirm_button_map, faded_confirm_button_map, "Confirm No Button", "Confirm No Button Faded");
                    break;
                case 1:
                    //Set only the selected button to be non faded
                    ToggleButtonFade(confirm_button_map, faded_confirm_button_map, "Confirm Yes Button", "Confirm Yes Button Faded");
                    break;
            }
        }


        void UpdateQuitConfirmationButtonFade()
        {
            switch (quit_confirmation_index)
            {
                case 0:
                    //Set only the selected button to be non faded
                    ToggleButtonFade(quit_confirm_button_map, quit_faded_confirm_button_map, "Quit Confirm No Button", "Quit Confirm No Button Faded");
                    break;
                case 1:
                    //Set only the selected button to be non faded
                    ToggleButtonFade(quit_confirm_button_map, quit_faded_confirm_button_map, "Quit Confirm Yes Button", "Quit Confirm Yes Button Faded");
                    break;
            }
        }

        void ToggleButtonFade(Dictionary<string, GameObject> unfaded_buttons, Dictionary<string, GameObject> faded_buttons, string active_button_name, string inactive_button_name)
        {
            foreach (KeyValuePair<string, GameObject> entry in unfaded_buttons)
            {
                if (entry.Key == active_button_name)
                {
                    ECS.SetActive(entry.Value.id, true);
                }
                else
                {
                    ECS.SetActive(entry.Value.id, false);
                }
            }

            foreach (KeyValuePair<string, GameObject> entry in faded_buttons)
            {
                if (entry.Key == inactive_button_name)
                {
                    ECS.SetActive(entry.Value.id, false);
                }
                else
                {
                    ECS.SetActive(entry.Value.id, true);
                }
            }
        }

        public void ToggleManualCanPause(bool toggle)
        {
            can_pause = toggle;
        }

        public override void OnTriggerEnter(uint id)
        {
        }

        public override void OnTriggerExit(uint id)
        {
        }
    }
}
