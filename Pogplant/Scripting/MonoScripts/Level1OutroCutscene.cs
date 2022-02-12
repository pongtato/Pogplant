using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class Level1OutroCutscene : MonoBehaviour
    {
        uint boss_model_id;
        uint cinematic_bar_top_id;
        uint cinematic_bar_bottom_id;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        public override void Start()
        {
            //boss_model_id = ECS.FindEntityWithName("Fake Boss Model");
            //cinematic_bar_top_id = ECS.FindEntityWithName("Top Cinematic Bar");
            //cinematic_bar_bottom_id = ECS.FindEntityWithName("Bottom Cinematic Bar");
        }

        public override void Update(float dt)
        {
            
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
