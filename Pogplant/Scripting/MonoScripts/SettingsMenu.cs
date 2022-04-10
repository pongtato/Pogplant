using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class SettingsMenu : MonoBehaviour
    {
        enum SETTINGS_MODE
        {
            LEFT,
            RIGHT
        }
        enum SETTINGS_MENU_BUTTONS
        {
            BGM_VOLUME,
            SFX_VOLUME,
            VOICE_OVERS,
            //GAMMA,
            DEFAULT
        }

        enum RESOLUTIONS
        {
            FULLSCREEN,
            WINDOWED_1920x1080,
            WINDOWED_1280x720
        }

        enum SETTINGS_RIGHT_MENU_BUTTONS
        {
            RESOLUTION,
            GAMMA,
            DEFAULT
        }

        private Dictionary<string, GameObject> button_map = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> button_map_right = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> faded_button_map = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> faded_button_map_right = new Dictionary<string, GameObject>();

        private List<uint> bgm_bars_list;
        private List<uint> sfx_bars_list;
        private List<uint> vo_bars_list;
        private List<uint> gamma_bars_list;
        private List<uint> resolution_bars_list;

        const float default_value = 0.5f;
        const float gamma_default_value = 2.2f;
        const float volume_change_amount = 0.1f;
        const float gamma_change_amount = 0.4f;
        const float arrow_animation_speed = 20.0f;
        const float max_arrow_scale = 0.075f;
        const float min_arrow_scale = 0.05f;
        bool scale_left_arrow;
        bool scale_right_arrow;
        bool play_left_arrow_anim;
        bool play_right_arrow_anim;

        int active_index;
        uint left_arrow_id;
        uint right_arrow_id;

        uint bgm_bar_fg_id;
        uint sfx_bar_fg_id;
        uint vo_bar_fg_id;
        uint gamma_bar_fg_id;
        uint resolution_bar_fg_id;

        uint bgm_parent_id;
        uint sfx_parent_id;
        uint vo_parent_id;
        uint gamma_parent_id;
        uint resolution_parent_id;

        uint rightBarID;
        uint leftBarID;

        public static bool refresh;
        private bool m_resetBars;
        private SETTINGS_MODE m_Mode;
        private RESOLUTIONS m_currentResolution;
        private SETTINGS_RIGHT_MENU_BUTTONS m_rightIndex = SETTINGS_RIGHT_MENU_BUTTONS.RESOLUTION;

        public enum ANIM_STATE
        { 
            OPENING,
            CLOSING,
            INPUT_READY,
            INACTIVE
        }

        public static ANIM_STATE current_state;
        public static float anim_lerp_step;
        const float anim_lerp_speed = 1.5f;
        const float lerp_min = 0.0f;
        const float lerp_max = 1.0f;
        Vector3 lerp_initial_position;
        Vector3 lerp_final_position;

        //Control type UI
        uint kb_instructions_id;
        uint ctrler_instructions_id;

        uint q_button_id;
        uint lbumper_button_id;

        uint e_button_id;
        uint rbumper_button_id;


        public SettingsMenu()
        {

        }

        public override void Start()
        {

        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();

            bgm_bars_list = new List<uint>();
            sfx_bars_list = new List<uint>();
            vo_bars_list = new List<uint>();
            gamma_bars_list = new List<uint>();
            resolution_bars_list = new List<uint>();

            bgm_parent_id = ECS.FindChildEntityWithName(entityID, "BGM Parent");
            sfx_parent_id = ECS.FindChildEntityWithName(entityID, "SFX Parent");
            vo_parent_id = ECS.FindChildEntityWithName(entityID, "VO Parent");
            gamma_parent_id = ECS.FindChildEntityWithName(entityID, "Gamma Parent");
            resolution_parent_id = ECS.FindChildEntityWithName(entityID, "Resolution Parent");

            //Settings menu full opacity
            uint bgm_volume_button_id = ECS.FindChildEntityWithName(bgm_parent_id, "BGM Volume Button");
            ECS.GetTransformECS(bgm_volume_button_id, ref pos, ref rot, ref scale);
            button_map.Add("BGM Volume Button", new GameObject(bgm_volume_button_id, new Transform(pos, rot, scale), "BGM Volume Button"));

            uint sfx_volume_button_id = ECS.FindChildEntityWithName(sfx_parent_id, "SFX Volume Button");
            ECS.GetTransformECS(sfx_volume_button_id, ref pos, ref rot, ref scale);
            button_map.Add("SFX Volume Button", new GameObject(sfx_volume_button_id, new Transform(pos, rot, scale), "SFX Volume Button"));

            uint vo_volume_button_id = ECS.FindChildEntityWithName(vo_parent_id, "VO Volume Button");
            ECS.GetTransformECS(vo_volume_button_id, ref pos, ref rot, ref scale);
            button_map.Add("VO Volume Button", new GameObject(vo_volume_button_id, new Transform(pos, rot, scale), "VO Volume Button"));

            uint default_volume_button_id = ECS.FindChildEntityWithName(entityID, "Default Volume Button");
            ECS.GetTransformECS(default_volume_button_id, ref pos, ref rot, ref scale);
            button_map.Add("Default Volume Button", new GameObject(default_volume_button_id, new Transform(pos, rot, scale), "Default Volume Button"));
            button_map_right.Add("Default Volume Button", new GameObject(default_volume_button_id, new Transform(pos, rot, scale), "Default Volume Button"));

            //Settings menu faded
            uint bgm_volume_button_faded_id = ECS.FindChildEntityWithName(bgm_parent_id, "BGM Volume Button Faded");
            ECS.GetTransformECS(bgm_volume_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map.Add("BGM Volume Button Faded", new GameObject(bgm_volume_button_faded_id, new Transform(pos, rot, scale), "BGM Volume Button Faded"));

            uint sfx_volume_button_faded_id = ECS.FindChildEntityWithName(sfx_parent_id, "SFX Volume Button Faded");
            ECS.GetTransformECS(sfx_volume_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map.Add("SFX Volume Button Faded", new GameObject(sfx_volume_button_faded_id, new Transform(pos, rot, scale), "SFX Volume Button Faded"));

            uint vo_volume_button_faded_id = ECS.FindChildEntityWithName(vo_parent_id, "VO Volume Button Faded");
            ECS.GetTransformECS(vo_volume_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map.Add("VO Volume Button Faded", new GameObject(vo_volume_button_faded_id, new Transform(pos, rot, scale), "VO Volume Button Faded"));

            uint default_volume_button_faded_id = ECS.FindChildEntityWithName(entityID, "Default Volume Button Faded");
            ECS.GetTransformECS(default_volume_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map.Add("Default Volume Button Faded", new GameObject(default_volume_button_faded_id, new Transform(pos, rot, scale), "Default Volume Button Faded"));
            faded_button_map_right.Add("Default Volume Button Faded", new GameObject(default_volume_button_faded_id, new Transform(pos, rot, scale), "Default Volume Button Faded"));

            // For the right bar
            uint default_gamma_button_id = ECS.FindChildEntityWithName(gamma_parent_id, "Gamma Button");
            ECS.GetTransformECS(default_gamma_button_id, ref pos, ref rot, ref scale);
            button_map_right.Add("Gamma Button", new GameObject(default_gamma_button_id, new Transform(pos, rot, scale), "Gamma Button"));

            uint default_gamma_button_faded_id = ECS.FindChildEntityWithName(gamma_parent_id, "Gamma Button Faded");
            ECS.GetTransformECS(default_gamma_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map_right.Add("Gamma Button Faded", new GameObject(default_gamma_button_faded_id, new Transform(pos, rot, scale), "Gamma Button Faded"));

            uint resolution_button_id = ECS.FindChildEntityWithName(resolution_parent_id, "Resolution Button");
            ECS.GetTransformECS(resolution_button_id, ref pos, ref rot, ref scale);
            button_map_right.Add("Resolution Button", new GameObject(resolution_button_id, new Transform(pos, rot, scale), "Resolution Button"));

            uint resolution_button_faded_id = ECS.FindChildEntityWithName(resolution_parent_id, "Resolution Button Faded");
            ECS.GetTransformECS(resolution_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map_right.Add("Resolution Button Faded", new GameObject(resolution_button_faded_id, new Transform(pos, rot, scale), "Resolution Button Faded"));

            bgm_bar_fg_id = ECS.FindChildEntityWithName(bgm_parent_id, "BGM Bar BG");
            sfx_bar_fg_id = ECS.FindChildEntityWithName(sfx_parent_id, "SFX Bar BG");
            vo_bar_fg_id = ECS.FindChildEntityWithName(vo_parent_id, "VO Bar BG");
            gamma_bar_fg_id = ECS.FindChildEntityWithName(gamma_parent_id, "Gamma Bar BG");
            resolution_bar_fg_id = ECS.FindChildEntityWithName(resolution_parent_id, "Resolution Bar BG");
            rightBarID = ECS.FindEntityWithName("SM_RightTab");
            leftBarID = ECS.FindEntityWithName("SM_LeftTab");

            //BGM bars
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_parent_id, "BGM Bar 1"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_parent_id, "BGM Bar 2"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_parent_id, "BGM Bar 3"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_parent_id, "BGM Bar 4"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_parent_id, "BGM Bar 5"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_parent_id, "BGM Bar 6"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_parent_id, "BGM Bar 7"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_parent_id, "BGM Bar 8"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_parent_id, "BGM Bar 9"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_parent_id, "BGM Bar 10"));

            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_parent_id, "SFX Bar 1"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_parent_id, "SFX Bar 2"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_parent_id, "SFX Bar 3"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_parent_id, "SFX Bar 4"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_parent_id, "SFX Bar 5"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_parent_id, "SFX Bar 6"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_parent_id, "SFX Bar 7"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_parent_id, "SFX Bar 8"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_parent_id, "SFX Bar 9"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_parent_id, "SFX Bar 10"));

            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_parent_id, "VO Bar 1"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_parent_id, "VO Bar 2"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_parent_id, "VO Bar 3"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_parent_id, "VO Bar 4"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_parent_id, "VO Bar 5"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_parent_id, "VO Bar 6"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_parent_id, "VO Bar 7"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_parent_id, "VO Bar 8"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_parent_id, "VO Bar 9"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_parent_id, "VO Bar 10"));

            gamma_bars_list.Add(ECS.FindChildEntityWithName(gamma_parent_id, "Gamma Bar 1"));
            gamma_bars_list.Add(ECS.FindChildEntityWithName(gamma_parent_id, "Gamma Bar 2"));
            gamma_bars_list.Add(ECS.FindChildEntityWithName(gamma_parent_id, "Gamma Bar 3"));
            gamma_bars_list.Add(ECS.FindChildEntityWithName(gamma_parent_id, "Gamma Bar 4"));
            gamma_bars_list.Add(ECS.FindChildEntityWithName(gamma_parent_id, "Gamma Bar 5"));

            resolution_bars_list.Add(ECS.FindChildEntityWithName(resolution_parent_id, "Resolution Bar 1"));
            resolution_bars_list.Add(ECS.FindChildEntityWithName(resolution_parent_id, "Resolution Bar 2"));
            resolution_bars_list.Add(ECS.FindChildEntityWithName(resolution_parent_id, "Resolution Bar 3"));

            left_arrow_id = ECS.FindChildEntityWithName(entityID, "Left Arrow");
            right_arrow_id = ECS.FindChildEntityWithName(entityID, "Right Arrow");

            active_index = 0;
            m_Mode = SETTINGS_MODE.LEFT;
            m_rightIndex = SETTINGS_RIGHT_MENU_BUTTONS.RESOLUTION;
            m_currentResolution = (RESOLUTIONS)PlayerPrefs.GetValue<int>("Resolution", (int)RESOLUTIONS.FULLSCREEN);
            m_resetBars = false;
            ECS.SetActive(entityID, false);

            current_state = ANIM_STATE.INACTIVE;
            anim_lerp_step = 0.0f;
            lerp_initial_position = new Vector3(0.0f, -1.1f, ECS.GetGlobalPosition(entityID).Z);
            lerp_final_position = new Vector3(0.0f, 0.0f, ECS.GetGlobalPosition(entityID).Z);

            kb_instructions_id = ECS.FindEntityWithName("KB_Instructions");
            ctrler_instructions_id = ECS.FindEntityWithName("Ctrler_Instructions");

            q_button_id = ECS.FindEntityWithName("SM_Q");
            lbumper_button_id = ECS.FindEntityWithName("SM_LB");

            e_button_id = ECS.FindEntityWithName("SM_E");
            rbumper_button_id = ECS.FindEntityWithName("SM_RB");
        }

        public override void Update(float dt)
        {
            if (refresh)
            {
                if (m_Mode == SETTINGS_MODE.LEFT)
                {
                    ECS.SetActive(leftBarID, true);
                    ECS.SetActive(rightBarID, false);
                    RefreshVolumeBars(bgm_bars_list, "BGM");
                    RefreshVolumeBars(sfx_bars_list, "SFX");
                    RefreshVolumeBars(vo_bars_list, "VO");

                    if (InputUtility.IsControlledBeingUsed())
                    {
                        ECS.SetActive(e_button_id, false);
                        ECS.SetActive(rbumper_button_id, true);
                    }
                    else
                    {
                        ECS.SetActive(e_button_id, true);
                        ECS.SetActive(rbumper_button_id, false);
                    }

                }
                else if (m_Mode == SETTINGS_MODE.RIGHT)
                {
                    ECS.SetActive(leftBarID, false);
                    ECS.SetActive(rightBarID, true);
                    UpdateGammaBar(gamma_bars_list);
                    UpdateResolutionBar(resolution_bars_list);

                    if (InputUtility.IsControlledBeingUsed())
                    {
                        ECS.SetActive(q_button_id, false);
                        ECS.SetActive(lbumper_button_id, true);
                    }
                    else
                    {
                        ECS.SetActive(q_button_id, true);
                        ECS.SetActive(lbumper_button_id, false);
                    }
                }
                UpdatePauseMenuButtonFade();
                ResetArrowAnimation();
                refresh = false;
            }

            if (m_resetBars)
            {
                if (m_Mode == SETTINGS_MODE.LEFT)
                {
                    RefreshVolumeBars(bgm_bars_list, "BGM");
                    RefreshVolumeBars(sfx_bars_list, "SFX");
                    RefreshVolumeBars(vo_bars_list, "VO");
                }
                else if (m_Mode == SETTINGS_MODE.RIGHT)
                {
                    UpdateGammaBar(gamma_bars_list);
                    UpdateResolutionBar(resolution_bars_list);
                }
                UpdatePauseMenuButtonFade();
                ResetArrowAnimation();
                m_resetBars = false;
            }

            switch (current_state)
            {
                case ANIM_STATE.INPUT_READY:
                    UpdateSettingsMenuInput(dt);
                    break;
                case ANIM_STATE.OPENING:
                    UpdateSettingsMenuAnimation(true, dt);
                    break;
                case ANIM_STATE.CLOSING:
                    UpdateSettingsMenuAnimation(false, dt);
                    break;
            }
        }

        void UpdateSettingsMenuAnimation(bool opening, float dt)
        {
            anim_lerp_step += dt * anim_lerp_speed;
            
            //Menu slide in from below
            if (opening)
            {
                if (anim_lerp_step > lerp_max)
                {
                    anim_lerp_step = lerp_max;
                    current_state = ANIM_STATE.INPUT_READY;
                }

                ECS.SetGlobalPosition(entityID, Vector3.Lerp(lerp_initial_position, lerp_final_position, anim_lerp_step));
            }
            //Menu slide out to below
            else
            {
                if (anim_lerp_step > lerp_max)
                {
                    anim_lerp_step = lerp_max;
                    MainMenuController.menu_state = MainMenuController.MENU_STATE.INPUT_READY;
                    current_state = ANIM_STATE.INACTIVE;
                }

                ECS.SetGlobalPosition(entityID, Vector3.Lerp(lerp_final_position, lerp_initial_position, anim_lerp_step));
            }
        }

        void UpdateSettingsMenuInput(float dt)
        {
            if (InputUtility.onKeyTriggered("MENUBACK") || InputUtility.onKeyTriggered("ESCAPE"))
            {
                //ECS.SetActive(entityID, false);
                anim_lerp_step = lerp_min;
                current_state = ANIM_STATE.CLOSING;
            }

            if (InputUtility.IsControlledBeingUsed())
            {
                ECS.SetActive(kb_instructions_id, false);
                ECS.SetActive(ctrler_instructions_id, true);
            }
            else
            {
                ECS.SetActive(kb_instructions_id, true);
                ECS.SetActive(ctrler_instructions_id, false);
            }

            //Key input
            if (m_Mode == SETTINGS_MODE.LEFT)
            {
                if (InputUtility.IsControlledBeingUsed())
                {
                    ECS.SetActive(e_button_id, false);
                    ECS.SetActive(rbumper_button_id, true);
                }
                else
                {
                    ECS.SetActive(e_button_id, true);
                    ECS.SetActive(rbumper_button_id, false);
                }

                if (InputUtility.onKeyTriggered("MENUTABRIGHT"))
                {
                    m_Mode = SETTINGS_MODE.RIGHT;
                    ECS.SetActive(rightBarID, true);
                    ECS.SetActive(leftBarID, false);


                    UpdatePauseMenuButtonFade();
                    ResetArrowAnimation();
                    m_resetBars = true;
                    return;
                }

                if (InputUtility.onKeyTriggered("MENUUP"))
                {
                    ECS.PlayAudio(entityID, 1, "SFX");

                    --active_index;

                    if (active_index < (int)SETTINGS_MENU_BUTTONS.BGM_VOLUME)
                    {
                        active_index = (int)SETTINGS_MENU_BUTTONS.DEFAULT;
                    }
                    UpdatePauseMenuButtonFade();
                    ResetArrowAnimation();
                }
                else if (InputUtility.onKeyTriggered("MENUDOWN"))
                {
                    ECS.PlayAudio(entityID, 2, "SFX");

                    ++active_index;

                    if (active_index > (int)SETTINGS_MENU_BUTTONS.DEFAULT)
                    {
                        active_index = (int)SETTINGS_MENU_BUTTONS.BGM_VOLUME;
                    }
                    UpdatePauseMenuButtonFade();
                    ResetArrowAnimation();
                }

                if (InputUtility.onKeyTriggered("MENULEFT"))
                {
                    ECS.PlayAudio(entityID, 1, "SFX");

                    switch (active_index)
                    {
                        case 0:
                            {
                                float amount = AudioEngine.GetChannelGroupVolume("BGM") > 0.1f ? AudioEngine.GetChannelGroupVolume("BGM") - volume_change_amount : 0.0f;
                                AudioEngine.SetChannelGroupVolume("BGM", amount);
                                PlayerPrefs.SetValue<float>("AudioBGM", amount);
                                UpdateVolumeBars(bgm_bars_list);
                            }
                            break;
                        case 1:
                            {
                                float amount = AudioEngine.GetChannelGroupVolume("SFX") > 0.1f ? AudioEngine.GetChannelGroupVolume("SFX") - volume_change_amount : 0.0f;
                                AudioEngine.SetChannelGroupVolume("SFX", amount);
                                PlayerPrefs.SetValue<float>("AudioSFX", amount);
                                UpdateVolumeBars(sfx_bars_list);
                            }
                            break;
                        case 2:
                            {
                                float amount = AudioEngine.GetChannelGroupVolume("VO") > 0.1f ? AudioEngine.GetChannelGroupVolume("VO") - volume_change_amount : 0.0f;
                                AudioEngine.SetChannelGroupVolume("VO", amount);
                                PlayerPrefs.SetValue<float>("AudioVO", amount);
                                UpdateVolumeBars(vo_bars_list);
                            }
                            break;
                            //case 3:
                            //    {
                            //        float amount = GameUtilities.GetGamma() >= 1.4f ? GameUtilities.GetGamma() - gamma_change_amount : 1.0f;
                            //        GameUtilities.SetGamma(amount);
                            //        PlayerPrefs.SetValue<float>("Gamma", amount);
                            //        UpdateGammaBar(gamma_bars_list);
                            //    }
                            //    break;
                    }
                    scale_left_arrow = true;
                    play_left_arrow_anim = true;
                }
                else if (InputUtility.onKeyTriggered("MENURIGHT"))
                {
                    ECS.PlayAudio(entityID, 2, "SFX");

                    switch (active_index)
                    {
                        case 0:
                            {
                                float amount = AudioEngine.GetChannelGroupVolume("BGM") < 0.9f ? AudioEngine.GetChannelGroupVolume("BGM") + volume_change_amount : 1.0f;
                                AudioEngine.SetChannelGroupVolume("BGM", amount);
                                PlayerPrefs.SetValue<float>("AudioBGM", amount);
                                UpdateVolumeBars(bgm_bars_list);
                            }
                            break;
                        case 1:
                            {
                                float amount = AudioEngine.GetChannelGroupVolume("SFX") < 0.9f ? AudioEngine.GetChannelGroupVolume("SFX") + volume_change_amount : 1.0f;
                                AudioEngine.SetChannelGroupVolume("SFX", amount);
                                PlayerPrefs.SetValue<float>("AudioSFX", amount);
                                UpdateVolumeBars(sfx_bars_list);
                            }
                            break;
                        case 2:
                            {
                                float amount = AudioEngine.GetChannelGroupVolume("VO") < 0.9f ? AudioEngine.GetChannelGroupVolume("VO") + volume_change_amount : 1.0f;
                                AudioEngine.SetChannelGroupVolume("VO", amount);
                                PlayerPrefs.SetValue<float>("AudioVO", amount);
                                UpdateVolumeBars(vo_bars_list);
                            }
                            break;
                            //case 3:
                            //    {
                            //        float amount = GameUtilities.GetGamma() <= 2.6 ? GameUtilities.GetGamma() + gamma_change_amount : 3.0f;
                            //        GameUtilities.SetGamma(amount);
                            //        PlayerPrefs.SetValue<float>("Gamma", amount);
                            //        UpdateGammaBar(gamma_bars_list);
                            //    }
                            //    break;
                    }
                    scale_right_arrow = true;
                    play_right_arrow_anim = true;
                }

                //Set all to default volume
                if (InputUtility.onKeyTriggered("MENUSELECT"))
                {
                    ECS.PlayAudio(entityID, 0, "SFX");

                    if (active_index == 3)
                    {
                        AudioEngine.SetChannelGroupVolume("BGM", default_value);
                        AudioEngine.SetChannelGroupVolume("SFX", default_value);
                        AudioEngine.SetChannelGroupVolume("VO", default_value);

                        PlayerPrefs.SetValue<float>("AudioBGM", default_value);
                        PlayerPrefs.SetValue<float>("AudioSFX", default_value);
                        PlayerPrefs.SetValue<float>("AudioVO", default_value);

                        RefreshVolumeBars(bgm_bars_list, "BGM");
                        RefreshVolumeBars(sfx_bars_list, "SFX");
                        RefreshVolumeBars(vo_bars_list, "VO");
                    }
                }

                if (play_left_arrow_anim)
                    UpdateArrowAnimation(true, dt);
                if (play_right_arrow_anim)
                    UpdateArrowAnimation(false, dt);
            }
            else if (m_Mode == SETTINGS_MODE.RIGHT)
            {
                if (InputUtility.IsControlledBeingUsed())
                {
                    ECS.SetActive(q_button_id, false);
                    ECS.SetActive(lbumper_button_id, true);
                }
                else
                {
                    ECS.SetActive(q_button_id, true);
                    ECS.SetActive(lbumper_button_id, false);
                }

                if (InputUtility.onKeyTriggered("MENUTABLEFT"))
                {
                    m_Mode = SETTINGS_MODE.LEFT;
                    ECS.SetActive(leftBarID, true);
                    ECS.SetActive(rightBarID, false);

                    UpdatePauseMenuButtonFade();
                    ResetArrowAnimation();
                    m_resetBars = true;
                    return;
                }

                if (InputUtility.onKeyTriggered("MENUUP"))
                {
                    ECS.PlayAudio(entityID, 1, "SFX");

                    --m_rightIndex;

                    if (m_rightIndex < SETTINGS_RIGHT_MENU_BUTTONS.RESOLUTION)
                    {
                        m_rightIndex = SETTINGS_RIGHT_MENU_BUTTONS.DEFAULT;
                    }
                    UpdatePauseMenuButtonFade();
                    ResetArrowAnimation();
                }
                else if (InputUtility.onKeyTriggered("MENUDOWN"))
                {
                    ECS.PlayAudio(entityID, 2, "SFX");

                    ++m_rightIndex;

                    if (m_rightIndex > SETTINGS_RIGHT_MENU_BUTTONS.DEFAULT)
                    {
                        m_rightIndex = SETTINGS_RIGHT_MENU_BUTTONS.RESOLUTION;
                    }
                    UpdatePauseMenuButtonFade();
                    ResetArrowAnimation();
                }

                if (InputUtility.onKeyTriggered("MENULEFT"))
                {
                    ECS.PlayAudio(entityID, 1, "SFX");

                    switch (m_rightIndex)
                    {
                        case SETTINGS_RIGHT_MENU_BUTTONS.RESOLUTION:
                            {
                                ++m_currentResolution;

                                if (m_currentResolution > RESOLUTIONS.WINDOWED_1280x720)
                                {
                                    m_currentResolution = RESOLUTIONS.FULLSCREEN;
                                }
                                UpdateResolutionBar(resolution_bars_list);
                            }
                            break;
                        case SETTINGS_RIGHT_MENU_BUTTONS.GAMMA:
                            {
                                float amount = GameUtilities.GetGamma() >= 1.4f ? GameUtilities.GetGamma() - gamma_change_amount : 1.0f;
                                GameUtilities.SetGamma(amount);
                                PlayerPrefs.SetValue<float>("Gamma", amount);
                                UpdateGammaBar(gamma_bars_list);
                            }
                            break;
                    }
                    scale_left_arrow = true;
                    play_left_arrow_anim = true;
                }
                else if (InputUtility.onKeyTriggered("MENURIGHT"))
                {
                    ECS.PlayAudio(entityID, 2, "SFX");

                    switch (m_rightIndex)
                    {
                        case SETTINGS_RIGHT_MENU_BUTTONS.RESOLUTION:
                            {
                                --m_currentResolution;
                                if (m_currentResolution < RESOLUTIONS.FULLSCREEN)
                                {
                                    m_currentResolution = RESOLUTIONS.WINDOWED_1280x720;
                                }
                                UpdateResolutionBar(resolution_bars_list);
                            }
                            break;
                        case SETTINGS_RIGHT_MENU_BUTTONS.GAMMA:
                            {
                                float amount = GameUtilities.GetGamma() <= 2.6 ? GameUtilities.GetGamma() + gamma_change_amount : 3.0f;
                                GameUtilities.SetGamma(amount);
                                PlayerPrefs.SetValue<float>("Gamma", amount);
                                UpdateGammaBar(gamma_bars_list);
                            }
                            break;
                    }
                    scale_right_arrow = true;
                    play_right_arrow_anim = true;
                }

                //Set all to default resolution & gamma
                if (InputUtility.onKeyTriggered("MENUSELECT"))
                {
                    ECS.PlayAudio(entityID, 0, "SFX");

                    if (m_rightIndex == SETTINGS_RIGHT_MENU_BUTTONS.DEFAULT)
                    {
                        GameUtilities.SetGamma(gamma_default_value);
                        PlayerPrefs.SetValue<float>("Gamma", gamma_default_value);
                        GameUtilities.GameSetWindowSize(1920, 1080);
                        GameUtilities.GameSetFullscreen(false);
                        PlayerPrefs.SetValue<int>("Resolution", (int)RESOLUTIONS.WINDOWED_1920x1080);
                        UpdateResolutionBar(resolution_bars_list);
                        UpdateGammaBar(gamma_bars_list);
                    }
                    else if (m_rightIndex == SETTINGS_RIGHT_MENU_BUTTONS.RESOLUTION)
                    {
                        switch (m_currentResolution)
                        {
                            case RESOLUTIONS.FULLSCREEN:
                                {
                                    GameUtilities.GameSetFullscreen(true);
                                }
                                break;
                            case RESOLUTIONS.WINDOWED_1920x1080:
                                {
                                    GameUtilities.GameSetFullscreen(false);
                                    GameUtilities.GameSetWindowSize(1920, 1080);
                                }
                                break;
                            case RESOLUTIONS.WINDOWED_1280x720:
                                {
                                    GameUtilities.GameSetFullscreen(false);
                                    GameUtilities.GameSetWindowSize(1280, 720);
                                }
                                break;
                        }
                        PlayerPrefs.SetValue<int>("Resolution", (int)m_currentResolution);
                        UpdateResolutionBar(resolution_bars_list);
                    }
                }

                if (play_left_arrow_anim)
                    UpdateArrowAnimation(true, dt);
                if (play_right_arrow_anim)
                    UpdateArrowAnimation(false, dt);
            }
        }

        public void UpdatePauseMenuButtonFade()
        {
            if (m_Mode == SETTINGS_MODE.LEFT)
            {
                switch (active_index)
                {
                    case 0:
                        {
                            //Set only the selected button to be non faded
                            ToggleButtonFade(button_map, faded_button_map, "BGM Volume Button", "BGM Volume Button Faded");
                            ECS.SetGlobalPosition(left_arrow_id, new Vector3(ECS.GetGlobalPosition(left_arrow_id).X, ECS.GetGlobalPosition(bgm_bar_fg_id).Y, ECS.GetGlobalPosition(left_arrow_id).Z));
                            ECS.SetGlobalPosition(right_arrow_id, new Vector3(ECS.GetGlobalPosition(right_arrow_id).X, ECS.GetGlobalPosition(bgm_bar_fg_id).Y, ECS.GetGlobalPosition(right_arrow_id).Z));
                            ECS.SetActive(left_arrow_id, true);
                            ECS.SetActive(right_arrow_id, true);
                        }
                        break;
                    case 1:
                        {
                            //Set only the selected button to be non faded
                            ToggleButtonFade(button_map, faded_button_map, "SFX Volume Button", "SFX Volume Button Faded");
                            ECS.SetGlobalPosition(left_arrow_id, new Vector3(ECS.GetGlobalPosition(left_arrow_id).X, ECS.GetGlobalPosition(sfx_bar_fg_id).Y, ECS.GetGlobalPosition(left_arrow_id).Z));
                            ECS.SetGlobalPosition(right_arrow_id, new Vector3(ECS.GetGlobalPosition(right_arrow_id).X, ECS.GetGlobalPosition(sfx_bar_fg_id).Y, ECS.GetGlobalPosition(right_arrow_id).Z));
                            ECS.SetActive(left_arrow_id, true);
                            ECS.SetActive(right_arrow_id, true);
                        }
                        break;
                    case 2:
                        {
                            //Set only the selected button to be non faded
                            ToggleButtonFade(button_map, faded_button_map, "VO Volume Button", "VO Volume Button Faded");
                            ECS.SetGlobalPosition(left_arrow_id, new Vector3(ECS.GetGlobalPosition(left_arrow_id).X, ECS.GetGlobalPosition(vo_bar_fg_id).Y, ECS.GetGlobalPosition(left_arrow_id).Z));
                            ECS.SetGlobalPosition(right_arrow_id, new Vector3(ECS.GetGlobalPosition(right_arrow_id).X, ECS.GetGlobalPosition(vo_bar_fg_id).Y, ECS.GetGlobalPosition(right_arrow_id).Z));
                            ECS.SetActive(left_arrow_id, true);
                            ECS.SetActive(right_arrow_id, true);
                        }
                        break;
                    case 3:
                        {
                            //Set only the selected button to be non faded
                            ToggleButtonFade(button_map, faded_button_map, "Default Volume Button", "Default Volume Button Faded");
                            ECS.SetActive(left_arrow_id, false);
                            ECS.SetActive(right_arrow_id, false);
                        }
                        break;
                        //case 4:
                        //    {
                        //        //Set only the selected button to be non faded
                        //        ToggleButtonFade(button_map, faded_button_map, "Default Volume Button", "Default Volume Button Faded");
                        //        ECS.SetActive(left_arrow_id, false);
                        //        ECS.SetActive(right_arrow_id, false);
                        //    }
                        //    break;
                }

            }
            else if (m_Mode == SETTINGS_MODE.RIGHT)
            {
                switch (m_rightIndex)
                {
                    case SETTINGS_RIGHT_MENU_BUTTONS.RESOLUTION:
                        {
                            //Set only the selected button to be non faded
                            ToggleButtonFade(button_map_right, faded_button_map_right, "Resolution Button", "Resolution Button Faded");
                            ECS.SetGlobalPosition(left_arrow_id, new Vector3(ECS.GetGlobalPosition(left_arrow_id).X, ECS.GetGlobalPosition(resolution_bar_fg_id).Y, ECS.GetGlobalPosition(left_arrow_id).Z));
                            ECS.SetGlobalPosition(right_arrow_id, new Vector3(ECS.GetGlobalPosition(right_arrow_id).X, ECS.GetGlobalPosition(resolution_bar_fg_id).Y, ECS.GetGlobalPosition(right_arrow_id).Z));
                            ECS.SetActive(left_arrow_id, true);
                            ECS.SetActive(right_arrow_id, true);
                        }
                        break;
                    case SETTINGS_RIGHT_MENU_BUTTONS.GAMMA:
                        {
                            //Set only the selected button to be non faded
                            ToggleButtonFade(button_map_right, faded_button_map_right, "Gamma Button", "Gamma Button Faded");
                            ECS.SetGlobalPosition(left_arrow_id, new Vector3(ECS.GetGlobalPosition(left_arrow_id).X, ECS.GetGlobalPosition(gamma_bar_fg_id).Y, ECS.GetGlobalPosition(left_arrow_id).Z));
                            ECS.SetGlobalPosition(right_arrow_id, new Vector3(ECS.GetGlobalPosition(right_arrow_id).X, ECS.GetGlobalPosition(gamma_bar_fg_id).Y, ECS.GetGlobalPosition(right_arrow_id).Z));
                            ECS.SetActive(left_arrow_id, true);
                            ECS.SetActive(right_arrow_id, true);
                        }
                        break;
                    case SETTINGS_RIGHT_MENU_BUTTONS.DEFAULT:
                        {
                            //Set only the selected button to be non faded
                            ToggleButtonFade(button_map_right, faded_button_map_right, "Default Volume Button", "Default Volume Button Faded");
                            ECS.SetActive(left_arrow_id, false);
                            ECS.SetActive(right_arrow_id, false);
                        }
                        break;
                }
            }
        }

        void UpdateArrowAnimation(bool left, float dt)
        {
            if (left)
            {
                if (scale_left_arrow)
                {
                    ECS.SetGlobalScale(left_arrow_id, Vector3.Lerp(ECS.GetGlobalScale(left_arrow_id), new Vector3(max_arrow_scale, max_arrow_scale, max_arrow_scale), arrow_animation_speed * dt));

                    if (ECS.GetGlobalScale(left_arrow_id).X > max_arrow_scale - 0.01f)
                    {
                        scale_left_arrow = false;
                    }
                }
                else
                {
                    ECS.SetGlobalScale(left_arrow_id, Vector3.Lerp(ECS.GetGlobalScale(left_arrow_id), new Vector3(min_arrow_scale, min_arrow_scale, min_arrow_scale), arrow_animation_speed * dt));
                    if (ECS.GetGlobalScale(left_arrow_id).X < min_arrow_scale + 0.01f)
                    {
                        play_left_arrow_anim = false;
                        scale_left_arrow = false;
                    }
                }
            }
            else
            {
                if (scale_right_arrow)
                {
                    ECS.SetGlobalScale(right_arrow_id, Vector3.Lerp(ECS.GetGlobalScale(right_arrow_id), new Vector3(max_arrow_scale, max_arrow_scale, max_arrow_scale), arrow_animation_speed * dt));

                    if (ECS.GetGlobalScale(right_arrow_id).X > max_arrow_scale - 0.01f)
                    {
                        scale_right_arrow = false;
                    }
                }
                else
                {
                    ECS.SetGlobalScale(right_arrow_id, Vector3.Lerp(ECS.GetGlobalScale(right_arrow_id), new Vector3(min_arrow_scale, min_arrow_scale, min_arrow_scale), arrow_animation_speed * dt));

                    if (ECS.GetGlobalScale(right_arrow_id).X < min_arrow_scale - 0.01f)
                    {
                        play_right_arrow_anim = false;
                        scale_right_arrow = false;
                    }
                }
            }
        }

        void ResetArrowAnimation()
        {
            play_left_arrow_anim = false;
            play_right_arrow_anim = false;
            scale_left_arrow = false;
            scale_right_arrow = false;
            ECS.SetGlobalScale(left_arrow_id, new Vector3(min_arrow_scale, min_arrow_scale, 1.0f));
            ECS.SetGlobalScale(right_arrow_id, new Vector3(min_arrow_scale, min_arrow_scale, 1.0f));
        }

        void UpdateVolumeBars(List<uint> bars_to_update, string channel_type = "")
        {
            int volumeLevel = 0;

            switch (active_index)
            {
                case 0:
                    volumeLevel = (int)(Math.Round(AudioEngine.GetChannelGroupVolume("BGM"), 1) * 10);  //have to round because of floating point
                    break;
                case 1:
                    volumeLevel = (int)(Math.Round(AudioEngine.GetChannelGroupVolume("SFX"), 1) * 10);
                    break;
                case 2:
                    volumeLevel = (int)(Math.Round(AudioEngine.GetChannelGroupVolume("VO"), 1) * 10);
                    break;
                case 4:
                    volumeLevel = (int)(Math.Round(AudioEngine.GetChannelGroupVolume(channel_type), 1) * 10);
                    break;
            }

            for (int i = 0; i < bars_to_update.Count; ++i)
            {
                if (i < volumeLevel)
                {
                    ECS.SetActive(bars_to_update[i], true);
                }
                else
                {
                    ECS.SetActive(bars_to_update[i], false);
                }
            }
        }

        void UpdateGammaBar(List<uint> bars_to_update)
        {
            int gamma_level = 0;

            if (GameUtilities.GetGamma() <= 1.01f)
            {
                gamma_level = 0;
            }
            if (GameUtilities.GetGamma() > 1.01f && GameUtilities.GetGamma() <= 1.41f)
            {
                gamma_level = 1;
            }
            else if (GameUtilities.GetGamma() > 1.41f && GameUtilities.GetGamma() <= 1.81f)
            {
                gamma_level = 2;
            }
            else if (GameUtilities.GetGamma() > 1.81f && GameUtilities.GetGamma() <= 2.21f)
            {
                gamma_level = 3;
            }
            else if (GameUtilities.GetGamma() > 2.21f && GameUtilities.GetGamma() <= 2.61f)
            {
                gamma_level = 4;
            }
            else if (GameUtilities.GetGamma() > 2.61f && GameUtilities.GetGamma() <= 3.1f)
            {
                gamma_level = 5;
            }

            for (int i = 0; i < bars_to_update.Count; ++i)
            {
                if (i < gamma_level)
                {
                    ECS.SetActive(bars_to_update[i], true);
                }
                else
                {
                    ECS.SetActive(bars_to_update[i], false);
                }
            }
        }

        void RefreshVolumeBars(List<uint> bars_to_update, string channel_type)
        {
            int volumeLevel = (int)(Math.Round(AudioEngine.GetChannelGroupVolume(channel_type), 1) * 10);

            for (int i = 0; i < bars_to_update.Count; ++i)
            {
                if (i < volumeLevel)
                {
                    ECS.SetActive(bars_to_update[i], true);
                }
                else
                {
                    ECS.SetActive(bars_to_update[i], false);
                }
            }
        }

        private void UpdateResolutionBar(List<uint> resolution_bars_list)
        {
            for(int i = 0; i < resolution_bars_list.Count; ++i)
            {
                if(i == (int)m_currentResolution)
                {
                    ECS.SetActive(resolution_bars_list[i], true);
                }
                else
                {
                    ECS.SetActive(resolution_bars_list[i], false);
                }
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
