using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class L2TurretController : MonoBehaviour
    {
        uint playerID;
        public class TurretGroup
        {
            public uint ParentID = 0;
            public bool ActivateTurrets = false;
            public bool StartedFiring = false;
            public float accumulated_dt = 0.0f;

            public int Turret_type = 0;
            public float activated_time = 0.0f;
            public List<uint> Indi_Turret = new List<uint>();
        }

        List<TurretGroup> Total_turret_groups = new List<TurretGroup>();

        public L2TurretController()
        {
            // initialize private variables here



        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            playerID = ECS.FindEntityWithName("PlayerShip");
 
            AddTurretGroup("GatlingTurretGroup1");
            AddTurretGroup("GatlingTurretGroup2");
            AddTurretGroup("GatlingTurretGroup3");
            AddTurretGroup("GatlingTurretGroup4");
            AddTurretGroup("GatlingTurretGroup5");


            AddTurretGroup("TurretGroup1");
            AddTurretGroup("TurretGroup2");
            AddTurretGroup("TurretGroup3");
            AddTurretGroup("TurretGroup4");
        }

        public override void Start()
        {

        }
        public override void Update(float dt)
        {
            for (int i = 0; i < Total_turret_groups.Count; ++i)
            {
                //If turrets enabled
                if (Total_turret_groups[i].ActivateTurrets)
                {
                    Total_turret_groups[i].accumulated_dt += dt;
                    if(!Total_turret_groups[i].StartedFiring)
                    {
                        for (int j = 0; j < Total_turret_groups[i].Indi_Turret.Count; ++j)
                        {
                            if (GameUtilities.GetAlive(Total_turret_groups[i].Indi_Turret[j]))
                            {
                                GameUtilities.SetTurretFire(Total_turret_groups[i].Indi_Turret[j], true, Total_turret_groups[i].Turret_type);
                            }
                        }
                        Total_turret_groups[i].StartedFiring = true;
                    }
                    if(Total_turret_groups[i].activated_time < Total_turret_groups[i].accumulated_dt)
                    {
                        for (int j = 0; j < Total_turret_groups[i].Indi_Turret.Count; ++j)
                        {
                            if(GameUtilities.GetAlive(Total_turret_groups[i].Indi_Turret[j]))
                            {
                               GameUtilities.SetTurretFire(Total_turret_groups[i].Indi_Turret[j], false, Total_turret_groups[i].Turret_type);
                            }
                        }
                        Total_turret_groups[i].ActivateTurrets = false;
                    }
                }
            }

        }
        public override void LateUpdate(float dt)
        {
        }

        public override void OnTriggerEnter(uint id)
        {
            if(id == playerID)
            {
                ActivateTurretGroup(entityID);
            }
        }
        public override void OnTriggerExit(uint id)
        {

        }


        void ActivateTurretGroup(uint ParentID)
        {
            for (int i = 0; i < Total_turret_groups.Count; ++i)
            {
                if (Total_turret_groups[i].ParentID == ParentID)
                {
                    Total_turret_groups[i].ActivateTurrets = true;
                }
            }
        }

        void AddTurretGroup(string group_name)
        {
            TurretGroup tempTurretGroup = new TurretGroup();
            tempTurretGroup.ParentID = ECS.FindEntityWithName(group_name);

            tempTurretGroup.Turret_type = ECS.GetValue<int>(tempTurretGroup.ParentID, 0, "Type");

            if (tempTurretGroup.Turret_type == 0)
            {
                Console.WriteLine("No TurretType stated");
                return;
            }
            tempTurretGroup.activated_time = ECS.GetValue<float>(tempTurretGroup.ParentID, 5, "ActivationTime");

            int counter = ECS.GetChildCount(tempTurretGroup.ParentID);

            string turret_name = "Fake";
            if (tempTurretGroup.Turret_type == 1)
                turret_name = "Small_Turret";
            if (tempTurretGroup.Turret_type == 2)
                turret_name = "Gatling_Turret";
            string loop_name;

            for (int j = 1; j < counter + 1; ++j)
            {
                loop_name = turret_name + j;
                uint TurretNo = ECS.FindChildEntityWithName(tempTurretGroup.ParentID, loop_name);
                tempTurretGroup.Indi_Turret.Add(TurretNo);
            }

            Total_turret_groups.Add(tempTurretGroup);
        }
    }
}
