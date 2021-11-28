using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class MissileController : MonoBehaviour
    {
        //The child's must be the same name while only keep the difference of main parent group
        //if this does not work go back Movestuff script
        public class MissleGroups
        {
            public uint ParentID = 0;
            public bool ActivateMissiles = false;
            public bool FiredAlready = false;
            public uint Missle1 = 0;
            public uint Missle2 = 0;
            public uint Missle3 = 0;
            public uint Missle4 = 0;
            public uint Missle5 = 0;
            public uint Missle6 = 0;
            public float accumulated_dt = 0.0f;
            public int counter = 0;

           public List<float> Timers = new List<float>();
           public List<uint> Indi_Missiles = new List<uint>();
        }

        MissleGroups first_group = new MissleGroups();
        List<MissleGroups> Total_Missile_groups = new List<MissleGroups>();
        public MissileController()
        {
            // initialize private variables here



        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;


            // Initialise the The individual controller with thier timer
            if (_entityID == ECS.FindEntityWithName("MissileTrigger"))
            {
                first_group.Timers = new List<float> {0.34f, 0.66f, 1.0f, 1.5f, 2.0f };
                first_group.ParentID = _entityID;
                first_group.ActivateMissiles = false;
            }
            // Add new controllers here
            //if (_entityID == ECS.FindEntityWithName("Parentxxx"))
            //    Timers = new List<float> { 0.34f, 0.66f };


            Total_Missile_groups.Add(first_group);

            UpdateMissleGroup(_entityID);
        }

        public override void Start()
        {

        }
        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            for (int i = 0; i < Total_Missile_groups.Count; ++i)
            {
                //If fired already we can skip
                if (Total_Missile_groups[i].FiredAlready)
                {
                    continue;
                }
                //If missile enable
                if (Total_Missile_groups[i].ActivateMissiles)
                {
                    Total_Missile_groups[i].accumulated_dt += dt;
                    if (Total_Missile_groups[i].Timers[Total_Missile_groups[i].counter] <= Total_Missile_groups[i].accumulated_dt)
                    {
                        GameUtilities.StartMissile(Total_Missile_groups[i].Indi_Missiles[Total_Missile_groups[i].counter]);
                        Total_Missile_groups[i].counter++;
                    }
                    if (Total_Missile_groups[i].Indi_Missiles.Count <= Total_Missile_groups[i].counter)
                    {
                        Total_Missile_groups[i].FiredAlready = true;
                    }
                }
            }
        }
        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

        }
        public override void OnTriggerEnter(uint id)
        {
            MatchMissleGroupParentIDandActivate(entityID);
        }
        public override void OnTriggerExit(uint id)
        {

        }

        void UpdateMissleGroup(uint _entityID)
        {
            //Call update on all missle groups
            for (int i = 0; i < Total_Missile_groups.Count; ++i)
            {
                int counter = ECS.GetChildCount(_entityID);
                string missile_name = "Missile";
                string loop_name;
                for (int j = 1; j < counter + 1; ++j)
                {
                    loop_name = missile_name + j;
                    uint MissileNo = ECS.FindChildEntityWithName(entityID, loop_name);
                    Total_Missile_groups[i].Indi_Missiles.Add(MissileNo);
                }
            }
        }

        void MatchMissleGroupParentIDandActivate(uint _ID)
        {
            for (int i = 0; i < Total_Missile_groups.Count; ++i)
            {
                if (Total_Missile_groups[i].ParentID == _ID)
                {
                    Total_Missile_groups[i].ActivateMissiles = true;
                }
            }
        }
    }
}
