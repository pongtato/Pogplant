using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class MoveStuff : MonoBehaviour
    {
        bool Trigger1 = false;
        uint Trigger1_ID = 0;
        bool Trigger2 = false;
        uint Trigger2_ID = 0;
        //List of object you want to move together
        List<uint> Trigger1_parent = new List<uint>();
        List<uint> Trigger2_parent = new List<uint>();
        //Map trigger for multiple parents
        Dictionary<uint, List<uint>> Movable = new Dictionary<uint, List<uint>>();
        public MoveStuff()
        {
            // initialize private variables here

        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            //Save all ID of the script it has
            Trigger1_ID = ECS.FindEntityWithName("xxx"); // Save ur trigger ID name
            //Save what ever needs to be moved Here (only a parent, add child and move all)
            //Needs to find by NAME
            uint parent1_move = ECS.FindEntityWithName("parent");
            Trigger1_parent.Add(parent1_move);
            Movable.Add(Trigger1_ID, Trigger1_parent);
            //Repeat Below  for different Triggers, uncomment 

            //Trigger2_ID = ECS.FindEntityWithName("xxx"); // Save ur trigger ID name
            ////Save what ever needs to be moved Here (only a parent, add child and move all)
            ////Needs to find by NAME
            //uint parent2_move = ECS.FindEntityWithName("parent2");
            //uint parent3_move = ECS.FindEntityWithName("parent3");
            //Trigger2_parent.Add(parent2_move);
            //Trigger2_parent.Add(parent3_move);
            //Movable.Add(Trigger2_ID, Trigger1_parent);
        }

        public override void Start()
        {

        }
        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            if(Trigger1)
            {
                List<uint> temp = Movable[Trigger1_ID];
                foreach(uint parent_id in temp)
                {
                    Transform id = ECS.GetComponent<Transform>(parent_id);
                    //Do changes
                    id.Rotation.Z += 1.0f;
                    ECS.SetTransformECS(parent_id,ref id.Position, ref id.Rotation,ref id.Scale);
                }

            }
        }
        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

        }

        public override void OnTriggerEnter(uint id)
        {
            if (entityID == Trigger1_ID)
                Trigger1 = true;
            if (entityID == Trigger2_ID)
                Trigger2 = true;

        }
        public override void OnTriggerExit(uint id)
        {

        }
    }
}
