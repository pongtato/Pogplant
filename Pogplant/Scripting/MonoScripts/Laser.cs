using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class Laser : MonoBehaviour
    {
        bool Activate_laser = false;
        uint Laser_Particle;
        uint Laser_object;
        public float charge_window;
        float acc_charge_window = 0.0f;

        bool Once_add_particle = false;
        bool Once_True = false;
        bool Once_remove_particle = false;

        public Laser() 
        {
        }

        public override void Start()
        {

        }

        public override void Init(ref uint _entityID)
        {
            //If do not want the charging stuff, add bool to laser component to enable/disable it.
            //There will be a check with ECS at start for the bool here to skip the charge particle stuff
            //Self-laser ID
            entityID = _entityID;
            Laser_Particle = ECS.FindChildEntityWithName(entityID, "LaserParticle");
            Laser_object = ECS.FindChildEntityWithName(entityID, "LaserObject");

            //Get the charge time from editor
            charge_window = ECS.GetValue<float>(entityID, 3.0f, "ChargeWindow");

            //Disable the laser to be used
            ECS.SetActive(Laser_Particle, false);
            //Contains the laser component
            ECS.SetActive(Laser_object, false);
        }

        public override void Update(float dt)
        {
            //if (InputUtility.onKeyTriggered(KEY_ID.KEY_J))
            //{       
            //    ActivateLaser();
            //}


            //Sequenctial update of laser
            if (Activate_laser)
            {
                if(!Once_add_particle)
                {
                    ECS.SetActive(Laser_Particle, true);
                    Once_add_particle = true;
                }
                acc_charge_window += dt;
                if(acc_charge_window >= charge_window && !Once_remove_particle)
                {
                    ECS.SetActive(Laser_Particle, false);
                    Once_True = true;
                    Once_remove_particle = true;
                }
                //Run the laser component
                if(Once_True)
                {
                    ECS.SetLaserStart(Laser_object, true);
                    Once_True = false;
                }
            }
            if (ECS.IsLaserComplete(Laser_object))
            {
                ResetLaser();
            }

        }

        public override void LateUpdate(float dt)
        {

        }

        public override void OnTriggerEnter(uint id)
        {
        }

        public override void OnTriggerExit(uint id)
        {

        }

        public void ActivateLaser()
        {
            if(!Activate_laser)
                Activate_laser = true;
        }
        public void ResetLaser()
        {
            Once_True = false;
            Once_add_particle = false;
            Once_remove_particle = false;
            acc_charge_window = 0.0f;
            Activate_laser = false;
            ECS.ResetLaser(Laser_object);
        }
    }
}
