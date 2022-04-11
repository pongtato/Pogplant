using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class TriggerAudioPlay : MonoBehaviour
    {
        public string audio_type;
        public bool double_control_audio;

        public int subs_begin_index;        //For kb/mouse
        public int subs_end_index;
        public int subs_controller_begin_index;
        public string subs_level_id;
        uint player_id;
        uint sub_renderer_id;
        public bool play_subs;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        public override void Start()
        {
            player_id = ECS.FindEntityWithName("PlayerShip");
            audio_type = ECS.GetValue<string>(entityID, "VO", "AudioType");
            double_control_audio = ECS.GetValue<bool>(entityID, false, "DoubleControlAudio");

            subs_begin_index = ECS.GetValue<int>(entityID, 0, "SubBeginIndex");
            subs_controller_begin_index = ECS.GetValue<int>(entityID, 0, "SubCtrlerBeginIndex");
            subs_end_index = ECS.GetValue<int>(entityID, 1, "SubEndIndex");
            subs_level_id = GameUtilities.GetSceneName();
            sub_renderer_id = ECS.FindEntityWithName("Subs_Renderer");
            play_subs = ECS.GetValue<bool>(entityID, true, "PlaySubs");
        }

        public override void Update(float dt)
        {

        }

        public override void LateUpdate(float dt)
        {

        }

        public override void OnTriggerEnter(uint id)
        {
            if (id == player_id)
            {
                if (double_control_audio)
                {
                    //Controller audio
                    if (InputUtility.IsControlledBeingUsed())
                    {
                        if(play_subs)
                            ECS.SetSubtitles(sub_renderer_id, subs_level_id, subs_controller_begin_index, subs_end_index);
                        ECS.PlayAudio(entityID, 0, audio_type);
                    }
                    //Keyboard audio
                    else
                    {
                        if (play_subs)
                            ECS.SetSubtitles(sub_renderer_id, subs_level_id, subs_begin_index, subs_end_index);
                        ECS.PlayAudio(entityID, 1, audio_type);
                    }
                }
                else
                {
                    if (play_subs)
                        ECS.SetSubtitles(sub_renderer_id, subs_level_id, subs_begin_index, subs_end_index);
                    ECS.PlayAudio(entityID, 0, audio_type);
                }
            }
        }

        public override void OnTriggerExit(uint id)
        {

        }
    }
}
