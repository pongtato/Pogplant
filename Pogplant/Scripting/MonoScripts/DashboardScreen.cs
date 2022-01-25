using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class DashboardScreen : MonoBehaviour
    {
        static uint UI_happy;
        static uint UI_hurt;
        static uint UI_neutral;

        private const float seconds_until_revert = 1.0f;
        private const float seconds_until_changeable = seconds_until_revert - 0.5f;
        private static float revert_timer = 0.0f;
        private static float changeable_timer = seconds_until_changeable;
        //private bool start_revert_timer;
        private static bool swap_face_lock = false;
        private static FACES currentFace = FACES.NEUTRAL;
        //private static uint currentFace = 0;

        //FACES current_face;
        public enum FACES
        {
            NEUTRAL = 0,
            HAPPY = 1,
            HURT = 2
        }

        public DashboardScreen()
        {

        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            UI_happy = ECS.FindChildEntityWithName(entityID, "Happy Face");
            UI_hurt = ECS.FindChildEntityWithName(entityID, "Hurt Face");
            UI_neutral = ECS.FindChildEntityWithName(entityID, "Neutral Face");
            Console.WriteLine(UI_happy + " | " + UI_hurt + " | " + UI_neutral);
        }

        public override void Start()
        {
            //current_face = FACES.NEUTRAL;

            ECS.SetActive(UI_happy, false);
            ECS.SetActive(UI_hurt, false);
        }

        public override void Update(float dt)
        {
            if (swap_face_lock)
            {
                changeable_timer += dt;
                if (changeable_timer >= seconds_until_changeable)
                    swap_face_lock = false;
            }
            else if (currentFace != FACES.NEUTRAL)
            {
                revert_timer += dt;

                if (revert_timer >= seconds_until_revert)
                {
                    revert_timer = 0.0f;
                    ResetFace();
                    currentFace = FACES.NEUTRAL;
                }
            }

            //if (start_revert_timer)
            //{
            //    revert_timer += dt;

            //    if (revert_timer >= seconds_until_revert)
            //    {
            //        revert_timer = 0.0f;
            //        start_revert_timer = false;
            //        ResetFace();
            //    }
            //}
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

        public static void SwapFace(FACES faceType)
        {
            //Enable the face type to swap to
            if (currentFace != faceType && !swap_face_lock)
            {
                switch (faceType)
                {
                    //Neutral face (Revert after x seconds)
                    case FACES.NEUTRAL:
                        //Console.WriteLine("enabling neutral");
                        ECS.SetActive(UI_neutral, true);
                        ECS.SetActive(UI_happy, false);
                        ECS.SetActive(UI_hurt, false);
                        currentFace = FACES.NEUTRAL;
                        //current_face = FACES.NEUTRAL;
                        break;
                    //Happy face (When getting bonus)
                    case FACES.HAPPY:
                        //Console.WriteLine("enabling happy");
                        ECS.SetActive(UI_neutral, false);
                        ECS.SetActive(UI_happy, true);
                        ECS.SetActive(UI_hurt, false);
                        currentFace = FACES.HAPPY;
                        //current_face = FACES.HAPPY;
                        break;
                    //Hurt Face (When taking damage)
                    case FACES.HURT:
                        //Console.WriteLine("enabling hurt");
                        ECS.SetActive(UI_neutral, false);
                        ECS.SetActive(UI_happy, false);
                        ECS.SetActive(UI_hurt, true);
                        currentFace = FACES.HURT;
                        //current_face = FACES.HURT;
                        break;
                }
                swap_face_lock = true;
                changeable_timer = 0.0f;
                revert_timer = 0.0f;
            }
        }

        void ResetFace()
        {
            ECS.SetActive(UI_happy, false);
            ECS.SetActive(UI_hurt, false);
            ECS.SetActive(UI_neutral, true);
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Scripting
//{
//    public class DashboardScreen : MonoBehaviour
//    {
//        uint UI_happy;
//        uint UI_hurt;
//        uint UI_neutral;

//        private const float seconds_until_revert = 2.0f;
//        private const float seconds_until_changeable = seconds_until_revert - 0.5f;
//        private float revert_timer = 0.0f;
//        private float changeable_timer = seconds_until_changeable;
//        //private bool start_revert_timer;
//        //private FACES currentFace = FACES.NEUTRAL;
//        private uint currentFace = 0;

//        public enum FACES
//        {
//            NEUTRAL = 0,
//            HAPPY = 1,
//            HURT = 2
//        }

//        //FACES current_face;

//        public DashboardScreen()
//        {

//        }

//        public override void Init(ref uint _entityID)
//        {
//            entityID = _entityID;
//            UI_happy = ECS.FindChildEntityWithName(entityID, "Happy Face");
//            UI_hurt = ECS.FindChildEntityWithName(entityID, "Hurt Face");
//            UI_neutral = ECS.FindChildEntityWithName(entityID, "Neutral Face");
//        }

//        public override void Start()
//        {
//            //current_face = FACES.NEUTRAL;

//            ECS.SetActive(UI_happy, false);
//            ECS.SetActive(UI_hurt, false);
//        }

//        public override void Update(float dt)
//        {
//            if(currentFace != 0)
//            {
//                revert_timer += dt;
//                changeable_timer += dt;

//                if (revert_timer >= seconds_until_revert)
//                {
//                    revert_timer = 0.0f;
//                    ResetFace();
//                    currentFace = 0;
//                }
//            }

//            //if (start_revert_timer)
//            //{
//            //    revert_timer += dt;

//            //    if (revert_timer >= seconds_until_revert)
//            //    {
//            //        revert_timer = 0.0f;
//            //        start_revert_timer = false;
//            //        ResetFace();
//            //    }
//            //}
//        }

//        public override void LateUpdate(float dt)
//        {

//        }

//        public override void OnTriggerEnter(uint id)
//        {

//        }

//        public override void OnTriggerExit(uint id)
//        {

//        }

//        public void SwapFace(uint faceType)
//        {
//            revert_timer = 0.0f;
//            //Enable the face type to swap to
//            if (currentFace != faceType &&
//                changeable_timer >= seconds_until_changeable)
//            {
//                switch (faceType)
//                {
//                    //Neutral face (Revert after x seconds)
//                    case 0:
//                        //Console.WriteLine("enabling neutral");
//                        ECS.SetActive(UI_neutral, true);
//                        ECS.SetActive(UI_happy, false);
//                        ECS.SetActive(UI_hurt, false);
//                        currentFace = 0;
//                        //current_face = FACES.NEUTRAL;
//                        break;
//                    //Happy face (When getting bonus)
//                    case 1:
//                        //Console.WriteLine("enabling happy");
//                        ECS.SetActive(UI_neutral, false);
//                        ECS.SetActive(UI_happy, true);
//                        ECS.SetActive(UI_hurt, false);
//                        currentFace = 1;
//                        //current_face = FACES.HAPPY;
//                        break;
//                    //Hurt Face (When taking damage)
//                    case 2:
//                        //Console.WriteLine("enabling hurt");
//                        ECS.SetActive(UI_neutral, false);
//                        ECS.SetActive(UI_happy, false);
//                        ECS.SetActive(UI_hurt, true);
//                        currentFace = 2;
//                        //current_face = FACES.HURT;
//                        break;
//                }
//                changeable_timer = 0.0f;
//            }
//        }

//        void ResetFace()
//        {
//            ECS.SetActive(UI_happy, false);
//            ECS.SetActive(UI_hurt, false);
//            ECS.SetActive(UI_neutral, true);
//            //current_face = FACES.NEUTRAL;
//        }
//    }
//}
