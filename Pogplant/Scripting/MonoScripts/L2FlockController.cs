using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class L2FlockController : MonoBehaviour
    {
        uint playerID = 0;

        //To move flock
        public class L2FlockFunction
        {
            public uint ParentID = 0;
            public bool ActivateFlock = false;
            public float travelling_speed = 0.0f;
            public List<uint> FlockGroup = new List<uint>();
            public List<Vector3> StartPos = new List<Vector3>();
            public uint EndPos_entity = 0;
            public Vector3 EndPos = new Vector3();
            public float accumulated_dt = 0;

        }

        List<L2FlockFunction> Total_flock_function_l2 = new List<L2FlockFunction>();

        public L2FlockController()
        {
            // initialize private variables here
            
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            playerID = ECS.FindEntityWithName("PlayerShip");

            //Change speed inside editor
            AddFlockGroup("FlockGroup1");


        }

        public override void Start()
        {

        }
        public override void Update(float dt)
        {

            for(int i= 0; i < Total_flock_function_l2.Count; ++i)
            {
                if(Total_flock_function_l2[i].ActivateFlock)
                {
                    for(int j = 0; j < Total_flock_function_l2[i].FlockGroup.Count; ++j)
                    {
                        Total_flock_function_l2[i].accumulated_dt += (Total_flock_function_l2[i].travelling_speed * dt);
                        if (Total_flock_function_l2[i].accumulated_dt > 1.0f)
                            Total_flock_function_l2[i].accumulated_dt = 1.0f;
                        Vector3 UpdatePos = Vector3.Lerp(Total_flock_function_l2[i].StartPos[j], Total_flock_function_l2[i].EndPos, Total_flock_function_l2[i].accumulated_dt);
                        ECS.SetGlobalPosition(Total_flock_function_l2[i].FlockGroup[j], UpdatePos);
                    }
                }
            }
        }
        public override void LateUpdate(float dt)
        {

        }

        public override void OnTriggerEnter(uint id)
        {
            if (id == playerID)
            {
                ActivateFlockGroup(entityID);
            }
        }
        public override void OnTriggerExit(uint id)
        {

        }

        void ActivateFlockGroup(uint ParentID)
        {
            for (int i = 0; i < Total_flock_function_l2.Count; ++i)
            {
                if (Total_flock_function_l2[i].ParentID == ParentID)
                {
                    Total_flock_function_l2[i].ActivateFlock = true;
                }
            }
        }

        void AddFlockGroup(string group_name)
        {
            L2FlockFunction tempFlockfunction = new L2FlockFunction();
            tempFlockfunction.ParentID = ECS.FindEntityWithName(group_name);

            tempFlockfunction.travelling_speed = ECS.GetValue<float>(tempFlockfunction.ParentID, 1.0f, "Speed");
            tempFlockfunction.EndPos_entity = ECS.FindChildEntityWithName(tempFlockfunction.ParentID, "EndPos");
            tempFlockfunction.EndPos = ECS.GetGlobalPosition(tempFlockfunction.EndPos_entity);

            int counter = ECS.GetChildCount(tempFlockfunction.ParentID);
            string flock_name = "MiniFlock";
            string loop_name;
            //Account for child EndPos so no -1 on counter
            for (int j = 1; j < counter; ++j)
            {
                loop_name = flock_name + j;
                uint FlockNo = ECS.FindChildEntityWithName(tempFlockfunction.ParentID, loop_name);
                tempFlockfunction.FlockGroup.Add(FlockNo);
                tempFlockfunction.StartPos.Add(ECS.GetGlobalPosition(FlockNo));
            }


            Total_flock_function_l2.Add(tempFlockfunction);
        }
    }
}
