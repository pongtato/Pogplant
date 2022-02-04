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
        uint player_id;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            player_id = ECS.FindEntityWithName("PlayerShip");
            audio_type = ECS.GetValue<string>(entityID, "SFX", "AudioType");
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
                ECS.PlayAudio(entityID, 0, audio_type);
            }
        }

        public override void OnTriggerExit(uint id)
        {

        }
    }
}
