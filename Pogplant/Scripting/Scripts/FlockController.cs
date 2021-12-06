using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class FlockController : MonoBehaviour
    {
        public class FlockGroup
        {
            public uint ParentID = 0;
            public bool ActivateFlock = false;
            public uint Flock1 = 0;
            public uint Flock2 = 0;
            public uint Flock3 = 0;
            public uint Flock4 = 0;
            public uint Flock5 = 0;
            public uint Flock6 = 0;

            public uint EndPos = 0;
            public Vector3 vec3_EndPos = new Vector3(); // Local Position
            public Vector3 vec3_Global_EndPos = new Vector3(); // Global Position
            public float accumulated_dt = 0.0f;

            public float travelling_speed = 0.0f;
            public List<uint> Indi_Flock = new List<uint>();
            public List<Vector3> FlockStart = new List<Vector3>();
        }

        FlockGroup first_group = new FlockGroup();
        List<FlockGroup> Total_flock_groups = new List<FlockGroup>();

        public FlockController()
        {
            // initialize private variables here
            
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            // Initialise the The individual controller with thier timer
            if (_entityID == ECS.FindEntityWithName("FlockGroup1"))
            {
                first_group.ParentID = _entityID;
                first_group.travelling_speed = 0.5f;



            }
            // Add new controllers here
            //if (_entityID == ECS.FindEntityWithName("Parentxxx"))
            //    Timers = new List<float> { 0.34f, 0.66f };


            Total_flock_groups.Add(first_group);

            UpdateFlockGroup(_entityID);
        }

        public override void Start()
        {
            
        }
        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            for (int i = 0; i < Total_flock_groups.Count; ++i)
            {
                //If flocking start enabled
                if (Total_flock_groups[i].ActivateFlock)
                {
                    Total_flock_groups[i].accumulated_dt += dt * Total_flock_groups[i].travelling_speed;
                    for (int j = 0; j < Total_flock_groups[i].Indi_Flock.Count; ++j)
                    {
                        Vector3 lerp_val = Vector3.Lerp(Total_flock_groups[i].FlockStart[j], Total_flock_groups[i].vec3_EndPos, Total_flock_groups[i].accumulated_dt);
                        Transform.LookAt(Total_flock_groups[i].Indi_Flock[j], Total_flock_groups[i].vec3_Global_EndPos);
                        Transform current_flock = ECS.GetComponent<Transform>(Total_flock_groups[i].Indi_Flock[j]);
                        Vector3 global_rot = ECS.GetGlobalRotation(Total_flock_groups[i].Indi_Flock[j]);

                        ECS.SetTransformECS(Total_flock_groups[i].Indi_Flock[j], lerp_val, global_rot, current_flock.Scale);
                    }
                    if(Total_flock_groups[i].accumulated_dt > 1.0f)
                    {
                        Total_flock_groups[i].ActivateFlock = false;
                    }
                }
            }
        }
        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

        }

        public override void OnTriggerEnter(uint id)
        {
            ActivateFlockGroup(entityID);
        }
        public override void OnTriggerExit(uint id)
        {

        }

        void UpdateFlockGroup(uint Parent_ID)
        {
            //Call update on all missle groups
            for (int i = 0; i < Total_flock_groups.Count; ++i)
            {
                int counter = ECS.GetChildCount(Parent_ID);
                string flock_name = "MiniFlock";
                --counter;
                string loop_name;
                for (int j = 1; j < counter + 1; ++j)
                {
                    loop_name = flock_name + j;
                    uint FlockNo = ECS.FindChildEntityWithName(entityID, loop_name);
                    Total_flock_groups[i].Indi_Flock.Add(FlockNo);
                    //Get child position for proper lerping
                    Total_flock_groups[i].FlockStart.Add(ECS.GetComponent<Transform>(FlockNo).Position);
                    //Add MiniFlock end
                    Total_flock_groups[i].EndPos = ECS.FindChildEntityWithName(Parent_ID, "EndPos");
                    Total_flock_groups[i].vec3_EndPos = ECS.GetComponent<Transform>(Total_flock_groups[i].EndPos).Position;
                    Total_flock_groups[i].vec3_Global_EndPos = ECS.GetGlobalPosition(Total_flock_groups[i].EndPos);
                }
            }
        }

        void ActivateFlockGroup(uint ParentID)
        {
            for (int i = 0; i < Total_flock_groups.Count; ++i)
            {
                if (Total_flock_groups[i].ParentID == ParentID)
                {
                    Total_flock_groups[i].ActivateFlock = true;
                }
            }
        }
    }
}
