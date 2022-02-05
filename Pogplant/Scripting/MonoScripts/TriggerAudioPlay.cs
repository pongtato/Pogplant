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
        uint player_id;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            player_id = ECS.FindEntityWithName("PlayerShip");
            audio_type = ECS.GetValue<string>(entityID, "VO", "AudioType");
            double_control_audio = ECS.GetValue<bool>(entityID, false, "DoubleControlAudio");
        }
        public override void Start()
        {

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
                    if (InputUtility.IsControlledBeingUsed())
                        ECS.PlayAudio(entityID, 0, audio_type);
                    else
                        ECS.PlayAudio(entityID, 1, audio_type);
                }
                else
                {
                    ECS.PlayAudio(entityID, 0, audio_type);
                }
            }
        }

        public override void OnTriggerExit(uint id)
        {

        }
    }
}
