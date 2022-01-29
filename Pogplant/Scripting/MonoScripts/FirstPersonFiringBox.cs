using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class FirstPersonFiringBox : MonoBehaviour
    {
        uint PlayerShip;
        uint BoxA;
        uint BoxB;

        public FirstPersonFiringBox()
        {
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            PlayerShip = ECS.FindEntityWithName("PlayerShip");
            BoxA = ECS.FindChildEntityWithName(PlayerShip, "BoxA");
            //BoxB = ECS.FindChildEntityWithName(PlayerShip, "BoxB");
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
            Tag other_tag = ECS.GetComponent<Tag>(id);
            if (other_tag.tag == "Targetable" && entityID == BoxA)
            {
               FirstPersonFiringSystem.AddEnemyToListOfTargets(id,0);
            }
            //if (other_tag.tag == "Targetable" && entityID == BoxB)
            //{
            //    FirstPersonFiringSystem.AddEnemyToListOfTargets(id, 1);
            //}
        }
        public override void OnTriggerExit(uint id)
        {
            Tag other_tag = ECS.GetComponent<Tag>(id);
            if (other_tag.tag == "Targetable" && entityID == BoxA)
            {
                FirstPersonFiringSystem.RemoveEnemyFromListOfTargets(id,0);
            }
            //if (other_tag.tag == "Targetable" && entityID == BoxB)
            //{
            //    FirstPersonFiringSystem.RemoveEnemyFromListOfTargets(id,1);
            //}
        }
    }
}
