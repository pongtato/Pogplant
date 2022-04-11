using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class TutPopup : MonoBehaviour
    {
        uint tut_id;
        uint player_id;
        public float deactivate_duration;
        float deactivate_timer;
        public string entity_to_find;
        bool move_in;
        bool triggered;

        Vector3 initial_position;
        Vector3 final_position;
        public float offset_Y;

        const float lerp_speed = 1.5f;
        float lerp_step;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

        }
        public override void Start()
        {
            player_id = ECS.FindEntityWithName("PlayerShip");
            deactivate_duration = ECS.GetValue<float>(entityID, 3.0f, "DeactivateDuration");
            entity_to_find = ECS.GetValue<string>(entityID, "", "EntityToFind");
            offset_Y = ECS.GetValue<float>(entityID, -0.3f, "OffsetY");
            deactivate_timer = 0;
            tut_id = ECS.FindEntityWithName(entity_to_find);
            move_in = true;

            lerp_step = 0.0f;
            initial_position = ECS.GetGlobalPosition(tut_id);
            final_position = new Vector3(ECS.GetGlobalPosition(tut_id).X, offset_Y, ECS.GetGlobalPosition(tut_id).Z);
        }

        public override void Update(float dt)
        {
            if (triggered)
            {
                //Move UI in
                if (move_in)
                {
                    lerp_step += lerp_speed * dt;

                    //Lerp cap
                    if (lerp_step >= 1.0f)
                    {
                        lerp_step = 1.0f;
                        move_in = false;
                    }

                    ECS.SetGlobalPosition(tut_id, Vector3.Lerp(initial_position, final_position, lerp_step));

                    if (lerp_step >= 1.0f)
                    {
                        lerp_step = 0.0f;
                    }
                }
                //Move UI out
                else
                {
                    deactivate_timer += dt;
                    if (deactivate_timer > deactivate_duration)
                    {
                        lerp_step += lerp_speed * dt;

                        //Lerp cap
                        if (lerp_step >= 1.0f)
                        {
                            lerp_step = 1.0f;
                            move_in = false;
                            triggered = false;
                        }

                        ECS.SetGlobalPosition(tut_id, Vector3.Lerp(final_position, initial_position, lerp_step));
                    }
                }
            }
        }

        public override void LateUpdate(float dt)
        {

        }

        public override void OnTriggerEnter(uint id)
        {
            if (id == player_id)
            {
                move_in = true;
                triggered = true;
            }
        }

        public override void OnTriggerExit(uint id)
        {

        }
    }
}
