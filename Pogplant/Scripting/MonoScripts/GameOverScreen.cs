using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class GameOverScreen : MonoBehaviour
    {
        public enum BUTTONS
        {
            RESTART = 0,
            RETURN_TO_MENU = 1
        }

        private int active_index = 0;
        private static Dictionary<string, GameObject> buttonMap = new Dictionary<string, GameObject>();
        //private static bool enable_menu_control;

        public GameOverScreen()
        {
            //enable_menu_control = true;
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            //ECS.PlayAudio(entityID, 0);

            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();

            uint sb = ECS.FindChildEntityWithName(entityID, "Restart Button");
            ECS.GetTransformECS(sb, ref pos, ref rot, ref scale);
            buttonMap.Add("Restart Button", new GameObject(sb, new Transform(pos, rot, scale), "Restart Button"));

            uint qb = ECS.FindChildEntityWithName(entityID, "Return To Menu Button");
            ECS.GetTransformECS(qb, ref pos, ref rot, ref scale);
            buttonMap.Add("Return To Menu Button", new GameObject(qb, new Transform(pos, rot, scale), "Return To Menu Button"));

            uint ar = ECS.FindChildEntityWithName(entityID, "Arrow");
            ECS.GetTransformECS(ar, ref pos, ref rot, ref scale);
            buttonMap.Add("Arrow", new GameObject(ar, new Transform(pos, rot, scale), "Arrow"));

            //uint bg = ECS.FindChildEntityWithName(entityID, "Background");
            //ECS.GetTransformECS(ar, ref pos, ref rot, ref scale);
            //buttonMap.Add("Background", new GameObject(bg, new Transform(pos, rot, scale), "Background"));

            //Disable the object
            //foreach (KeyValuePair<string, GameObject> button in buttonMap)
            //{
            //    ECS.SetActive(button.Value.id, false);
            //}

            //enable_menu_control = false;
        }

        public override void Start()
        {

        }

        public override void Update(float dt)
        {
            //if (enable_menu_control)
            //{
                UpdateInputs();
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

        //public void ShowGameOverScreen()
        //{
        //    //Disable the object
        //    //foreach (KeyValuePair<string, GameObject> button in buttonMap)
        //    //{
        //    //    ECS.SetActive(button.Value.id, true);
        //    //}

        //    //enable_menu_control = true;
        //}

        void UpdateInputs()
        {
            //Key input
            if (InputUtility.onKeyTriggered("MENUDOWN"))
            {
                --active_index;

                if (active_index < (int)BUTTONS.RESTART)
                {
                    active_index = (int)BUTTONS.RETURN_TO_MENU;
                }
            }
            else if (InputUtility.onKeyTriggered("MENUUP"))
            {
                ++active_index;

                if (active_index > (int)BUTTONS.RETURN_TO_MENU)
                {
                    active_index = (int)BUTTONS.RESTART;
                }
            }

            switch (active_index)
            {
                case 0:
                    ECS.SetTransformECS(buttonMap["Arrow"].id,
                        new Vector3((buttonMap["Restart Button"].transform.Position.X - 0.161f), buttonMap["Restart Button"].transform.Position.Y, buttonMap["Restart Button"].transform.Position.Z),
                        buttonMap["Arrow"].transform.Rotation,
                        buttonMap["Arrow"].transform.Scale);
                    break;
                case 1:
                    ECS.SetTransformECS(buttonMap["Arrow"].id,
                        new Vector3((buttonMap["Return To Menu Button"].transform.Position.X - 0.161f), buttonMap["Return To Menu Button"].transform.Position.Y, buttonMap["Return To Menu Button"].transform.Position.Z),
                        buttonMap["Arrow"].transform.Rotation,
                        buttonMap["Arrow"].transform.Scale);
                    break;
            }

            if (InputUtility.onKeyTriggered("MENUSELECT"))
            {
                //Restart or return to menu based on index
                switch (active_index)
                {
                    //Restart
                    case 0:
                        GameUtilities.LoadScene("Level01_M3_Blockout");
                        break;
                    //Return to menu
                    case 1:
                        GameUtilities.LoadScene("MainMenu");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
