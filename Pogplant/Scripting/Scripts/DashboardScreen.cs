using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class DashboardScreen : MonoBehaviour
    {
        uint UI_happy;
        uint UI_hurt;
        uint UI_neutral;

        private const float seconds_until_revert = 3.0f;
        private float revert_timer;
        private bool start_revert_timer;

        public enum FACES
        {
            NEUTRAL = 0,
            HAPPY = 1,
            HURT = 2
        }

        FACES current_face;

        public DashboardScreen()
        {

        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            UI_happy = ECS.FindChildEntityWithName(entityID, "Happy Face");
            UI_hurt = ECS.FindChildEntityWithName(entityID, "Hurt Face");
            UI_neutral = ECS.FindChildEntityWithName(entityID, "Neutral Face");
        }

        public override void Start()
        {
            current_face = FACES.NEUTRAL;

            ECS.SetActive(UI_happy, false);
            ECS.SetActive(UI_hurt, false);
        }

        public override void Update(float dt)
        {
            if (start_revert_timer)
            {
                revert_timer += dt;

                if (revert_timer >= seconds_until_revert)
                {
                    revert_timer = 0.0f;
                    start_revert_timer = false;
                    ResetFace();
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

        public void SwapFace(int faceType)
        {
            start_revert_timer = true;
            revert_timer = 0.0f;

            //Enable the face type to swap to
            switch (faceType)
            {
                //Neutral face (Revert after x seconds)
                case 0:
                    //Console.WriteLine("enabling neutral");
                    ECS.SetActive(UI_neutral, true);
                    ECS.SetActive(UI_happy, false);
                    ECS.SetActive(UI_hurt, false);
                    current_face = FACES.NEUTRAL;
                    break;
                //Happy face (When getting bonus)
                case 1:
                    //Console.WriteLine("enabling happy");
                    ECS.SetActive(UI_neutral, false);
                    ECS.SetActive(UI_happy, true);
                    ECS.SetActive(UI_hurt, false);
                    current_face = FACES.HAPPY;
                    break;
                //Hurt Face (When taking damage)
                case 2:
                    //Console.WriteLine("enabling hurt");
                    ECS.SetActive(UI_neutral, false);
                    ECS.SetActive(UI_happy, false);
                    ECS.SetActive(UI_hurt, true);
                    current_face = FACES.HURT;
                    break;
            }
        }

        void ResetFace()
        {
            ECS.SetActive(UI_happy, false);
            ECS.SetActive(UI_hurt, false);
            ECS.SetActive(UI_neutral, true);
            current_face = FACES.NEUTRAL;
        }
    }
}
