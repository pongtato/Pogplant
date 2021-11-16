using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class FirstPersonFiringSystem : MonoBehaviour
    {

        private List<uint> enemy_in_range;
        private GameObject[] target_enemy_arr;

        public FirstPersonFiringSystem()
        {
            // initialize private variables here
            
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        public override void Start()
        {
            
        }
        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

        }
        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

        }

        public override void OnTriggerEnter(uint id)
        {
            Tag other_tag = ECS.GetComponent<Tag>(id);
            if (other_tag.tag == "Targetable")
                Console.WriteLine("Firingsystem Other ID" + id);
        }
        public override void OnTriggerExit(uint id)
        {

        }
    }
}
