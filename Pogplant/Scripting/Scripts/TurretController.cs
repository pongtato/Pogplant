using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class TurretController : MonoBehaviour
    {
        //The child's must be the same name while only keep the difference of main parent group
        //if this does not work go back Movestuff script
        public class TurretGroup
        {
            public uint ParentID = 0;
            public bool ActivateTurrets = false;
            public bool StartedFiring = false;
            public uint Turret1 = 0;
            public uint Turret2 = 0;
            public uint Turret3 = 0;
            public uint Turret4 = 0;
            public uint Turret5 = 0;
            public uint Turret6 = 0;
            public float accumulated_dt = 0.0f;

            public float activated_time = 0.0f;
            public List<uint> Indi_Turret = new List<uint>();
        }

        TurretGroup first_group = new TurretGroup();
        List<TurretGroup> Total_turret_groups = new List<TurretGroup>();
        public TurretController()
        {
            // initialize private variables here



        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            // Initialise the The individual controller with thier timer
            if (_entityID == ECS.FindEntityWithName("TurretGroup1"))
            {
                first_group.ParentID = _entityID;
                first_group.activated_time = 20.0f;

            }
            // Add new controllers here
            //if (_entityID == ECS.FindEntityWithName("Parentxxx"))
            //    Timers = new List<float> { 0.34f, 0.66f };


            Total_turret_groups.Add(first_group);

            UpdateTurretGroup(_entityID);
        }

        public override void Start()
        {

        }
        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
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
                            GameUtilities.SetTurretFire(Total_turret_groups[i].Indi_Turret[j], true);
                        }
                        Total_turret_groups[i].StartedFiring = true;
                    }
                    if(Total_turret_groups[i].activated_time < Total_turret_groups[i].accumulated_dt)
                    {
                        for (int j = 0; j < Total_turret_groups[i].Indi_Turret.Count; ++j)
                        {
                            GameUtilities.SetTurretFire(Total_turret_groups[i].Indi_Turret[j], false);
                        }
                        Total_turret_groups[i].ActivateTurrets = false;
                    }
                }
            }

        }
        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

        }
        public override void OnTriggerEnter(uint id)
        {
            ActivateTurretGroup(entityID);
        }
        public override void OnTriggerExit(uint id)
        {

        }

        void UpdateTurretGroup(uint Parent_ID)
        {
            //Call update on all missle groups
            for (int i = 0; i < Total_turret_groups.Count; ++i)
            {
                int counter = ECS.GetChildCount(Parent_ID);
                string turret_name = "Enemy_Turret";
                string loop_name;
                for (int j = 1; j < counter + 1; ++j)
                {
                    loop_name = turret_name + j;
                    uint TurretNo = ECS.FindChildEntityWithName(entityID, loop_name);
                    Total_turret_groups[i].Indi_Turret.Add(TurretNo);
                }
            }
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
    }
}
