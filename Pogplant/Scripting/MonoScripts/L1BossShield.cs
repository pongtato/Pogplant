using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class L1BossShield : MonoBehaviour
    {
        /// <summary>
        /// Sequence of how the boss shield works:
        /// 1. Player damages the shield while fending off enemies
        /// 2. When shield is depleted, play/instantiate object with particle
        /// 3. When the particle hits the player (via estimated time), the black screen appears and plays audio
        /// 4. After another delay, transit to another scene.
        /// </summary>

        //Shield
        public float health;
        bool is_shield_broken;
        bool start_shield_break_countdown;
        const float shield_break_delay_duration = 2.0f;
        float shield_break_delay_timer;
        uint shield_object_id;
        uint sparks_particle_id;

        //Eye
        uint eye_upper_lid_id;
        uint eye_lower_lid_id;
        Vector3 rotate_angle;
        const float rotate_speed = 10.0f;
        bool start_eye_countdown;
        const float eye_delay_duration = 3.0f;
        float eye_delay_timer;

        //Black screen & particle
        bool start_particle_countdown;
        const float particle_delay_duration = 2.5f;
        float particle_timer;
        uint particle_cannon_id;

        //Scene transition
        bool start_scene_change_countdown;
        const float scene_change_delay_duration = 5.0f;
        float scene_change_delay_timer;
        uint black_screen_id;

        public L1BossShield()
        {
            black_screen_id = ECS.FindEntityWithName("L1 Black Screen");
            shield_object_id = ECS.FindEntityWithName("Boss Shield");
            sparks_particle_id = ECS.FindEntityWithName("Shield Sparks");
            eye_upper_lid_id = ECS.FindEntityWithName("Boss Eye Upper Lid");
            eye_lower_lid_id = ECS.FindEntityWithName("Boss Eye Lower Lid");
            particle_cannon_id = ECS.FindEntityWithName("Boss Particle Cannon");
        }

        public override void Start()
        {
            health = 1;
            is_shield_broken = false;
            ECS.SetActive(black_screen_id, false);
            ECS.SetActive(particle_cannon_id, false);
            ECS.SetActive(sparks_particle_id, false);
            rotate_angle = new Vector3(0, 0, 40.0f);
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        public override void Update(float dt)
        {
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_G))
            {
                TakeDamage(1);
            }

            //Shield break animation
            if (start_shield_break_countdown)
            {
                if (shield_break_delay_timer < shield_break_delay_duration)
                {
                    shield_break_delay_timer += dt;
                }
                else
                {
                    start_shield_break_countdown = false;

                    //Stops the sparks particle and disable shields
                    ECS.SetParticlePause(sparks_particle_id, true);
                    ECS.SetActive(sparks_particle_id, false);
                    ECS.SetActive(shield_object_id, false);

                    start_eye_countdown = true;
                }
            }

            //Eye rotation and fire particle cannon animation
            if (start_eye_countdown)
            {
                //Rotate the eye lids
                ECS.SetRotation(eye_upper_lid_id, Vector3.Lerp(ECS.GetComponent<Transform>(eye_upper_lid_id).Rotation, rotate_angle, dt * rotate_speed));
                ECS.SetRotation(eye_lower_lid_id, Vector3.Lerp(ECS.GetComponent<Transform>(eye_upper_lid_id).Rotation, -rotate_angle, dt * rotate_speed));

                if (eye_delay_timer < eye_delay_duration)
                {
                    eye_delay_timer += dt;
                }
                else
                {
                    start_eye_countdown = false;
                    start_particle_countdown = true;

                    ECS.SetActive(particle_cannon_id, true);
                    ECS.SetParticlePause(particle_cannon_id, false);
                }
            }

            //Collision and black screen
            if (start_particle_countdown)
            {
                if (particle_timer < particle_delay_duration)
                {
                    particle_timer += dt;
                }
                else
                {
                    //Fire OHK cannon (activate large particle towards player and enable a black screen after being hit)
                    ECS.SetActive(black_screen_id, true);

                    //Play player take severe damage and crashing audio
                    //ECS.PlayAudio(entityID, 1);

                    //Transit to another scene after X seconds
                    start_scene_change_countdown = true;
                }   
            }

            //Countdown for scene changing
            if (start_scene_change_countdown)
            {
                if (scene_change_delay_timer < scene_change_delay_duration)
                {
                    scene_change_delay_timer += dt;
                }
                else
                {
                    start_scene_change_countdown = false;
                    //GameUtilities.LoadScene("Level2");
                }
            }
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            Console.WriteLine("Health: " + health);
            //Triggers the shield depleting and starts the sequence of the OHK
            if (health <= 0)
            {
                Console.WriteLine("Dead");
                if (!is_shield_broken)
                {
                    //Play shield destroyed audio
                    //ECS.PlayAudio(entityID, 0);
                    Console.WriteLine("shield broken");

                    //Play electric sparks particle
                    ECS.SetActive(sparks_particle_id, true);
                    ECS.SetParticlePause(sparks_particle_id, false);

                    //Start counting down the delay
                    start_shield_break_countdown = true;

                    //Only need to happen once
                    is_shield_broken = true;
                }
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
    }
}
