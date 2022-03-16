using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class LaserWeapon : PauseBehaviour
    {
        List<uint> m_EnemiesInRange = new List<uint>();
        //float m_Timer = 0.0f;
        //Vector3 m_StartLaserScale;
        uint m_PlayerShipID;
        const float m_Damage = 2000.0f;

        public LaserWeapon()
        {
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            //m_Timer = 0.0f;
            //m_StartLaserScale = ECS.GetGlobalScale(entityID);
            m_PlayerShipID = ECS.FindEntityWithName("PlayerShip");
            //ECS.SetActive(entityID, false);
        }

        public override void Start()
        {
        }

        public override void Update(float dt)
        {
            // commented out by vinceen
            // replaced with nuke.cs
            
            //if(PlayerScript.m_EnableLaserAttack)
            //{
            //    m_Timer += dt;
            //    //Console.WriteLine("Timer: " + m_Timer);
            //    if(m_Timer >= 2.0f)
            //    {
            //        m_Timer = 0.0f;
            //        //Console.WriteLine("Enemies In Range: " + m_EnemiesInRange.Count);
            //        foreach (var id in m_EnemiesInRange)
            //        {    
            //            EncounterSystemDriver.TakeDamage(id, m_Damage);
            //            GameUtilities.EnemyTakeDamageFromID(id, m_Damage);
            //        }

            //        PlayerScript.m_EnableLaserAttack = false;
            //        //ECS.SetGlobalScale(entityID, m_StartLaserScale);
            //        m_EnemiesInRange.Clear();
            //        //ECS.SetActive(entityID, false);
            //        GameUtilities.ResumeScene();
            //    }
            //    else
            //    {
            //        //Vector3 currScale = ECS.GetGlobalScale(entityID);
            //        //ECS.SetGlobalScale(entityID, new Vector3(currScale.X * 1.02f, currScale.Y, currScale.Z * 1.02f));
            //    }
            //}
        }

        public override void OnTriggerEnter(uint id)
        {
            Tag other_tag = ECS.GetComponent<Tag>(id);
            if (other_tag.tag == "Targetable")
            {
                m_EnemiesInRange.Add(id);
            }

        }

        public override void OnTriggerExit(uint id)
        {
            Tag other_tag = ECS.GetComponent<Tag>(id);
            if (other_tag.tag == "Targetable")
            {
                m_EnemiesInRange.Remove(id);
            }

        }
    }
}
