using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class LaserTriggerController : MonoBehaviour
    {
        uint playerID;
        public class LaserTriggerGroup
        {
            public uint ParentID = 0;
            public bool ActivateLasers = false;

            //public float accumulated_dt = 0.0f;

            //public float activated_time = 0.0f;
            public List<uint> Indi_Lasers = new List<uint>();
        }

        List<LaserTriggerGroup> Total_laser_groups = new List<LaserTriggerGroup>();


        public LaserTriggerController()
        {
            // initialize private variables here



        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            playerID = ECS.FindEntityWithName("PlayerShip");
            // Initialise the The individual controller with thier timer
            AddLaserGroup("LaserTriggerGroup1");
            AddLaserGroup("LaserTriggerGroup2");
            AddLaserGroup("LaserTriggerGroup3");
            AddLaserGroup("LaserTriggerGroup4");
            AddLaserGroup("LaserTriggerGroup5");
            AddLaserGroup("LaserTriggerGroup6");
            UpdateLaserGroup(_entityID);
        }

        public override void Start()
        {

        }
        public override void Update(float dt)
        {
            for (int i = 0; i < Total_laser_groups.Count; ++i)
            {
                if (Total_laser_groups[i].ActivateLasers)
                {
                    for(int j = 0; j < Total_laser_groups[i].Indi_Lasers.Count; ++ j)
                    {
                        GameUtilities.StartLaser(Total_laser_groups[i].Indi_Lasers[j]);
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
                ActivateLaserGroup(entityID);
            }
        }
        public override void OnTriggerExit(uint id)
        {

        }

        void UpdateLaserGroup(uint Parent_ID)
        {
            //Call update on all missle groups
            for (int i = 0; i < Total_laser_groups.Count; ++i)
            {
                int counter = ECS.GetChildCount(Parent_ID);
                string laser_name = "Laser";
                string loop_name;
                for (int j = 1; j < counter + 1; ++j)
                {
                    loop_name = laser_name + j;
                    uint LaserNo = ECS.FindChildEntityWithName(entityID, loop_name);
                    Total_laser_groups[i].Indi_Lasers.Add(LaserNo);
                }
            }
        }

        void ActivateLaserGroup(uint ParentID)
        {
            for (int i = 0; i < Total_laser_groups.Count; ++i)
            {
                if (Total_laser_groups[i].ParentID == ParentID)
                {
                    Total_laser_groups[i].ActivateLasers = true;
                }
            }
        }

        void AddLaserGroup(string LaserGroupName)
        {
            LaserTriggerGroup tempLaserGroup = new LaserTriggerGroup();
            tempLaserGroup.ParentID = ECS.FindEntityWithName(LaserGroupName);

            Total_laser_groups.Add(tempLaserGroup);
        }
    }
}
