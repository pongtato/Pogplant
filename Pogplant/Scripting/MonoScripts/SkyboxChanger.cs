using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class SkyboxChanger : MonoBehaviour
    {
        public string skybox_to_load;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            skybox_to_load = ECS.GetValue<string>(entityID, "Skybox", "SkyboxToLoad");
        }

        public override void Start()
        {
            GameUtilities.ChangeSkybox(skybox_to_load);
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
