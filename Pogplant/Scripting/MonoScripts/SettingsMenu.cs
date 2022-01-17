using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class SettingsMenu : MonoBehaviour
    {
        enum SETTINGS_MENU_BUTTONS
        {
            BGM_VOLUME,
            SFX_VOLUME,
            VOICE_OVERS,
            DEFAULT
        }

        private Dictionary<string, GameObject> button_map = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> faded_button_map = new Dictionary<string, GameObject>();

        private List<uint> bgm_bars_list;
        private List<uint> sfx_bars_list;
        private List<uint> vo_bars_list;

        const float default_value = 0.5f;
        const float volume_change_amount = 0.1f;
        const float arrow_animation_speed = 20.0f;
        const float max_arrow_scale = 0.15f;
        const float min_arrow_scale = 0.1f;
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

        public static bool refresh;

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

            //Settings menu full opacity
            uint master_volume_button_id = ECS.FindChildEntityWithName(entityID, "BGM Volume Button");
            ECS.GetTransformECS(master_volume_button_id, ref pos, ref rot, ref scale);
            button_map.Add("BGM Volume Button", new GameObject(master_volume_button_id, new Transform(pos, rot, scale), "BGM Volume Button"));

            uint sfx_volume_button_id = ECS.FindChildEntityWithName(entityID, "SFX Volume Button");
            ECS.GetTransformECS(sfx_volume_button_id, ref pos, ref rot, ref scale);
            button_map.Add("SFX Volume Button", new GameObject(sfx_volume_button_id, new Transform(pos, rot, scale), "SFX Volume Button"));

            uint vo_volume_button_id = ECS.FindChildEntityWithName(entityID, "VO Volume Button");
            ECS.GetTransformECS(vo_volume_button_id, ref pos, ref rot, ref scale);
            button_map.Add("VO Volume Button", new GameObject(vo_volume_button_id, new Transform(pos, rot, scale), "VO Volume Button"));

            uint default_volume_button_id = ECS.FindChildEntityWithName(entityID, "Default Volume Button");
            ECS.GetTransformECS(default_volume_button_id, ref pos, ref rot, ref scale);
            button_map.Add("Default Volume Button", new GameObject(default_volume_button_id, new Transform(pos, rot, scale), "Default Volume Button"));

            //Settings menu faded
            uint master_volume_button_faded_id = ECS.FindChildEntityWithName(entityID, "BGM Volume Button Faded");
            ECS.GetTransformECS(master_volume_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map.Add("BGM Volume Button Faded", new GameObject(master_volume_button_faded_id, new Transform(pos, rot, scale), "BGM Volume Button Faded"));

            uint sfx_volume_button_faded_id = ECS.FindChildEntityWithName(entityID, "SFX Volume Button Faded");
            ECS.GetTransformECS(sfx_volume_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map.Add("SFX Volume Button Faded", new GameObject(sfx_volume_button_faded_id, new Transform(pos, rot, scale), "SFX Volume Button Faded"));

            uint vo_volume_button_faded_id = ECS.FindChildEntityWithName(entityID, "VO Volume Button Faded");
            ECS.GetTransformECS(vo_volume_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map.Add("VO Volume Button Faded", new GameObject(vo_volume_button_faded_id, new Transform(pos, rot, scale), "VO Volume Button Faded"));

            uint default_volume_button_faded_id = ECS.FindChildEntityWithName(entityID, "Default Volume Button Faded");
            ECS.GetTransformECS(default_volume_button_faded_id, ref pos, ref rot, ref scale);
            faded_button_map.Add("Default Volume Button Faded", new GameObject(default_volume_button_faded_id, new Transform(pos, rot, scale), "Default Volume Button Faded"));

            bgm_bar_fg_id = ECS.FindChildEntityWithName(entityID, "BGM Bar BG");
            sfx_bar_fg_id = ECS.FindChildEntityWithName(entityID, "SFX Bar BG");
            vo_bar_fg_id = ECS.FindChildEntityWithName(entityID, "VO Bar BG");

            //BGM bars
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_bar_fg_id, "BGM Bar 1"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_bar_fg_id, "BGM Bar 2"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_bar_fg_id, "BGM Bar 3"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_bar_fg_id, "BGM Bar 4"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_bar_fg_id, "BGM Bar 5"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_bar_fg_id, "BGM Bar 6"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_bar_fg_id, "BGM Bar 7"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_bar_fg_id, "BGM Bar 8"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_bar_fg_id, "BGM Bar 9"));
            bgm_bars_list.Add(ECS.FindChildEntityWithName(bgm_bar_fg_id, "BGM Bar 10"));

            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_bar_fg_id, "SFX Bar 1"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_bar_fg_id, "SFX Bar 2"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_bar_fg_id, "SFX Bar 3"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_bar_fg_id, "SFX Bar 4"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_bar_fg_id, "SFX Bar 5"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_bar_fg_id, "SFX Bar 6"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_bar_fg_id, "SFX Bar 7"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_bar_fg_id, "SFX Bar 8"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_bar_fg_id, "SFX Bar 9"));
            sfx_bars_list.Add(ECS.FindChildEntityWithName(sfx_bar_fg_id, "SFX Bar 10"));

            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_bar_fg_id, "VO Bar 1"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_bar_fg_id, "VO Bar 2"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_bar_fg_id, "VO Bar 3"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_bar_fg_id, "VO Bar 4"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_bar_fg_id, "VO Bar 5"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_bar_fg_id, "VO Bar 6"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_bar_fg_id, "VO Bar 7"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_bar_fg_id, "VO Bar 8"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_bar_fg_id, "VO Bar 9"));
            vo_bars_list.Add(ECS.FindChildEntityWithName(vo_bar_fg_id, "VO Bar 10"));

            left_arrow_id = ECS.FindChildEntityWithName(entityID, "Left Arrow");
            right_arrow_id = ECS.FindChildEntityWithName(entityID, "Right Arrow");

            active_index = 0;

            ECS.SetActive(entityID, false);
        }

        public override void Update(float dt)
        {
            if (refresh)
            {
                UpdatePauseMenuButtonFade();
                refresh = false;
            }

            UpdateSettingsMenuInput(dt);
        }

        void UpdateSettingsMenuInput(float dt)
        {
            //Key input
            if (InputUtility.onKeyTriggered("MENUUP"))
            {
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
                switch (active_index)
                {
                    case 0:
                        {
                            float amount = AudioEngine.GetChannelGroupVolume("BGM") > 0.1f ? AudioEngine.GetChannelGroupVolume("BGM") - volume_change_amount : 0.0f;
                            AudioEngine.SetChannelGroupVolume("BGM", amount);
                            UpdateVolumeBars(bgm_bars_list);
                        }
                        break;
                    case 1:
                        {
                            float amount = AudioEngine.GetChannelGroupVolume("SFX") > 0.1f ? AudioEngine.GetChannelGroupVolume("SFX") - volume_change_amount : 0.0f;
                            AudioEngine.SetChannelGroupVolume("SFX", amount);
                            UpdateVolumeBars(sfx_bars_list);
                        }
                        break;
                    case 2:
                        {
                            float amount = AudioEngine.GetChannelGroupVolume("VO") > 0.1f ? AudioEngine.GetChannelGroupVolume("VO") - volume_change_amount : 0.0f;
                            AudioEngine.SetChannelGroupVolume("VO",amount);
                            UpdateVolumeBars(vo_bars_list);
                        }
                        break;
                }
                scale_left_arrow = true;
                play_left_arrow_anim = true;
            }
            else if (InputUtility.onKeyTriggered("MENURIGHT"))
            {
                switch (active_index)
                {
                    case 0:
                        {
                            float amount = AudioEngine.GetChannelGroupVolume("BGM") < 0.9f ? AudioEngine.GetChannelGroupVolume("BGM") + volume_change_amount : 1.0f;
                            AudioEngine.SetChannelGroupVolume("BGM", amount);
                            UpdateVolumeBars(bgm_bars_list);
                        }
                        break;
                    case 1:
                        {
                            float amount = AudioEngine.GetChannelGroupVolume("SFX") < 0.9f ? AudioEngine.GetChannelGroupVolume("SFX") + volume_change_amount : 1.0f;
                            AudioEngine.SetChannelGroupVolume("SFX", amount);
                            UpdateVolumeBars(sfx_bars_list);
                        }
                        break;
                    case 2:
                        {
                            float amount = AudioEngine.GetChannelGroupVolume("VO") < 0.9f ? AudioEngine.GetChannelGroupVolume("VO") + volume_change_amount : 1.0f;
                            AudioEngine.SetChannelGroupVolume("VO", amount);
                            UpdateVolumeBars(vo_bars_list);
                        }
                        break;
                }
                scale_right_arrow = true;
                play_right_arrow_anim = true;
            }

            //Set all to default volume
            if (InputUtility.onKeyTriggered("MENUSELECT"))
            {
                if (active_index == 3)
                {
                    AudioEngine.SetChannelGroupVolume("BGM", default_value);
                    AudioEngine.SetChannelGroupVolume("SFX", default_value);
                    AudioEngine.SetChannelGroupVolume("VO", default_value);

                    UpdateVolumeBars(bgm_bars_list, "BGM");
                    UpdateVolumeBars(sfx_bars_list, "SFX");
                    UpdateVolumeBars(vo_bars_list, "VO");
                }
            }

            if (InputUtility.onKeyTriggered("ESCAPE"))
            {
                ECS.SetActive(entityID, false);
            }

            if(play_left_arrow_anim)
                UpdateArrowAnimation(true, dt);
            if(play_right_arrow_anim)
                UpdateArrowAnimation(false, dt);
        }

        public void UpdatePauseMenuButtonFade()
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
            ECS.SetGlobalScale(left_arrow_id, new Vector3(0.1f, 0.1f, 1.0f));
            ECS.SetGlobalScale(right_arrow_id, new Vector3(0.1f, 0.1f, 1.0f));
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
                case 3:
                    volumeLevel = (int)(Math.Round(AudioEngine.GetChannelGroupVolume(channel_type), 1) * 10);
                    break;
            }
            Console.WriteLine("BGM lvl " + AudioEngine.GetChannelGroupVolume("BGM"));
            Console.WriteLine(volumeLevel);

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
