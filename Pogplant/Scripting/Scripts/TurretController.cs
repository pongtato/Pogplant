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
            public int Turret_type = 0;

            public float activated_time = 0.0f;
            public List<uint> Indi_Turret = new List<uint>();
        }

        TurretGroup first_group = new TurretGroup();
        TurretGroup second_group = new TurretGroup();
        TurretGroup third_group = new TurretGroup();
        TurretGroup fourth_group = new TurretGroup();
        TurretGroup fifth_group = new TurretGroup();
        TurretGroup six_group = new TurretGroup();
        TurretGroup seven_group = new TurretGroup();

        TurretGroup first_small_group = new TurretGroup();
        TurretGroup second_small_group = new TurretGroup();
        TurretGroup third_small_group = new TurretGroup();
        TurretGroup fourth_small_group = new TurretGroup();
        TurretGroup fifth_small_group = new TurretGroup();
        TurretGroup six_small_group = new TurretGroup();
        TurretGroup seven_small_group = new TurretGroup();
        TurretGroup eight_small_group = new TurretGroup();
        TurretGroup nine_small_group = new TurretGroup();
        TurretGroup ten_small_group = new TurretGroup();
        TurretGroup eleven_small_group = new TurretGroup();
        TurretGroup twelve_small_group = new TurretGroup();
        TurretGroup thirteen_small_group = new TurretGroup();
        TurretGroup fourteen_small_group = new TurretGroup();
        TurretGroup fifteen_small_group = new TurretGroup();
        TurretGroup sixteen_small_group = new TurretGroup();
        TurretGroup seventeen_small_group = new TurretGroup();

        List<TurretGroup> Total_turret_groups = new List<TurretGroup>();

        uint playerID;
        public TurretController()
        {
            // initialize private variables here



        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            playerID = ECS.FindEntityWithName("PlayerShip");
            // Initialise the The individual controller with thier timer
            if (_entityID == ECS.FindEntityWithName("GatlingTurretGroup1"))
            {
                first_group.ParentID = _entityID;
                first_group.activated_time = 5.0f;
                first_group.Turret_type = 2;
            }

            if (_entityID == ECS.FindEntityWithName("GatlingTurretGroup2"))
            {
                second_group.ParentID = _entityID;
                second_group.activated_time = 5.0f;
                second_group.Turret_type = 2;
            }
            // Add new controllers here
            //if (_entityID == ECS.FindEntityWithName("Parentxxx"))
            //    Timers = new List<float> { 0.34f, 0.66f };

            if (_entityID == ECS.FindEntityWithName("GatlingTurretGroup3"))
            {
                third_group.ParentID = _entityID;
                third_group.activated_time = 5.0f;
                third_group.Turret_type = 2;
            }


            if (_entityID == ECS.FindEntityWithName("GatlingTurretGroup4"))
            {
                fourth_group.ParentID = _entityID;
                fourth_group.activated_time = 5.0f;
                fourth_group.Turret_type = 2;
            }

            if (_entityID == ECS.FindEntityWithName("GatlingTurretGroup5"))
            {
                fifth_group.ParentID = _entityID;
                fifth_group.activated_time = 5.0f;
                fifth_group.Turret_type = 2;
            }

            if (_entityID == ECS.FindEntityWithName("GatlingTurretGroup6"))
            {
                six_group.ParentID = _entityID;
                six_group.activated_time = 5.0f;
                six_group.Turret_type = 2;
            }

            if (_entityID == ECS.FindEntityWithName("GatlingTurretGroup7"))
            {
                seven_group.ParentID = _entityID;
                seven_group.activated_time = 5.0f;
                seven_group.Turret_type = 2;
            }


            //small turrets bellow

            if (_entityID == ECS.FindEntityWithName("TurretGroup1"))
            {
                first_small_group.ParentID = _entityID;
                first_small_group.activated_time = 20.0f;
                first_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup2"))
            {
                second_small_group.ParentID = _entityID;
                second_small_group.activated_time = 20.0f;
                second_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup3"))
            {
                third_small_group.ParentID = _entityID;
                third_small_group.activated_time = 20.0f;
                third_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup4"))
            {
                fourth_small_group.ParentID = _entityID;
                fourth_small_group.activated_time = 20.0f;
                fourth_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup5"))
            {
                fifth_small_group.ParentID = _entityID;
                fifth_small_group.activated_time = 20.0f;
                fifth_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup6"))
            {
                six_small_group.ParentID = _entityID;
                six_small_group.activated_time = 20.0f;
                six_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup7"))
            {
                seven_small_group.ParentID = _entityID;
                seven_small_group.activated_time = 20.0f;
                seven_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup8"))
            {
                eight_small_group.ParentID = _entityID;
                eight_small_group.activated_time = 20.0f;
                eight_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup9"))
            {
                nine_small_group.ParentID = _entityID;
                nine_small_group.activated_time = 20.0f;
                nine_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup10"))
            {
                ten_small_group.ParentID = _entityID;
                ten_small_group.activated_time = 20.0f;
                ten_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup11"))
            {
                eleven_small_group.ParentID = _entityID;
                eleven_small_group.activated_time = 20.0f;
                eleven_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup12"))
            {
                twelve_small_group.ParentID = _entityID;
                twelve_small_group.activated_time = 20.0f;
                twelve_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup13"))
            {
                thirteen_small_group.ParentID = _entityID;
                thirteen_small_group.activated_time = 20.0f;
                thirteen_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup14"))
            {
                fourteen_small_group.ParentID = _entityID;
                fourteen_small_group.activated_time = 20.0f;
                fourteen_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup15"))
            {
                fifth_small_group.ParentID = _entityID;
                fifth_small_group.activated_time = 20.0f;
                fifth_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup16"))
            {
                sixteen_small_group.ParentID = _entityID;
                sixteen_small_group.activated_time = 20.0f;
                sixteen_small_group.Turret_type = 1;
            }

            if (_entityID == ECS.FindEntityWithName("TurretGroup17"))
            {
                seventeen_small_group.ParentID = _entityID;
                seventeen_small_group.activated_time = 20.0f;
                seventeen_small_group.Turret_type = 1;
            }


            Total_turret_groups.Add(first_group);
            Total_turret_groups.Add(second_group);
            Total_turret_groups.Add(third_group);
            Total_turret_groups.Add(fourth_group);
            Total_turret_groups.Add(fifth_group);
            Total_turret_groups.Add(six_group);
            Total_turret_groups.Add(seven_group);

            Total_turret_groups.Add(first_small_group);
            Total_turret_groups.Add(second_small_group);
            Total_turret_groups.Add(third_small_group);
            Total_turret_groups.Add(fourth_small_group);
            Total_turret_groups.Add(fifth_small_group);
            Total_turret_groups.Add(six_small_group);
            Total_turret_groups.Add(seven_small_group);
            Total_turret_groups.Add(eight_small_group);
            Total_turret_groups.Add(nine_small_group);
            Total_turret_groups.Add(ten_small_group);
            Total_turret_groups.Add(eleven_small_group);
            Total_turret_groups.Add(twelve_small_group);
            Total_turret_groups.Add(thirteen_small_group);
            Total_turret_groups.Add(fourth_small_group);
            Total_turret_groups.Add(fifteen_small_group);
            Total_turret_groups.Add(sixteen_small_group);
            Total_turret_groups.Add(seventeen_small_group);

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
        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
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
