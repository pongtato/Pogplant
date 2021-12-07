/*****************************************************************************/
/*!
\file	MainMenuController.cs
\author Lee Wei Liang
\par	email: weiliang.lee\@digipen.edu
\details
	Main Menu script for keyboard and controller to navigate to game scene.

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class MainMenuController : MonoBehaviour
    {
        public enum BUTTONS
        {
            START_GAME = 0,
            QUIT_GAME = 1 
        }

        private int active_index = 0;
        private Dictionary<string, GameObject> buttonMap = new Dictionary<string, GameObject>();

        public MainMenuController()
        {
            
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            ECS.PlayAudio(entityID, 0);

            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();
            
            uint sb = ECS.FindChildEntityWithName(entityID, "Start Button");
            ECS.GetTransformECS(sb, ref pos, ref rot, ref scale);
            buttonMap.Add("Start Button", new GameObject(sb, new Transform(pos, rot, scale), "Start Button"));

            uint qb = ECS.FindChildEntityWithName(entityID, "Quit Button");
            ECS.GetTransformECS(qb, ref pos, ref rot, ref scale);
            buttonMap.Add("Quit Button", new GameObject(qb, new Transform(pos, rot, scale), "Quit Button"));

            uint ar = ECS.FindChildEntityWithName(entityID, "Arrow");
            ECS.GetTransformECS(ar, ref pos, ref rot, ref scale);
            buttonMap.Add("Arrow", new GameObject(ar, new Transform(pos, rot, scale), "Arrow"));
        }

        public override void Start()
        {
            //active_index = 0;
            //Console.WriteLine("MMC init");
        }

        public override void Update(float dt)
        {
            //// Example of how to disable entity
            //if (InputUtility.onKeyTriggered(KEY_ID.KEY_D))
            //{
            //    ECS.SetActive(buttonMap["Arrow"].id, false);
            //}

            //// Example of how to enable entity (Do not activate an entity that is already active or it'll crash)
            //if (InputUtility.onKeyTriggered(KEY_ID.KEY_F))
            //{
            //    ECS.SetActive(buttonMap["Arrow"].id, true);
            //}

            UpdateInputs();
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

        void UpdateInputs()
        {
            //Key input
            if (InputUtility.onKeyTriggered("MENUDOWN"))
            //if (InputUtility.onKeyTriggered(KEY_ID.KEY_DOWN))
            {
                --active_index;
                
                if (active_index < (int)BUTTONS.START_GAME)
                {
                    active_index = (int)BUTTONS.QUIT_GAME;

                }
                //Console.WriteLine("Active index is: " + active_index);
            }
            else if (InputUtility.onKeyTriggered("MENUUP"))
            //else if (InputUtility.onKeyTriggered(KEY_ID.KEY_UP))
            {
                ++active_index;

                if (active_index > (int)BUTTONS.QUIT_GAME)
                {
                    active_index = (int)BUTTONS.START_GAME;

                }
                //Console.WriteLine("Active index is: "+ active_index);
            }
            
            switch (active_index)
            {
                case 0:
                    ECS.SetTransformECS(buttonMap["Arrow"].id,
                        new Vector3((buttonMap["Start Button"].transform.Value.Position.X - 0.161f), buttonMap["Start Button"].transform.Value.Position.Y, buttonMap["Start Button"].transform.Value.Position.Z),
                        buttonMap["Arrow"].transform.Value.Rotation,
                        buttonMap["Arrow"].transform.Value.Scale);
                    break;
                case 1:
                    ECS.SetTransformECS(buttonMap["Arrow"].id,
                        new Vector3((buttonMap["Quit Button"].transform.Value.Position.X - 0.161f), buttonMap["Quit Button"].transform.Value.Position.Y, buttonMap["Quit Button"].transform.Value.Position.Z),
                        buttonMap["Arrow"].transform.Value.Rotation,
                        buttonMap["Arrow"].transform.Value.Scale);
                    break;
            }

            if (InputUtility.onKeyTriggered("MENUSELECT"))
            //if (InputUtility.onKeyTriggered(KEY_ID.KEY_ENTER))
            {
                //Start or quit game based on index
                switch (active_index)
                {
                    case 0:
                        GameUtilities.LoadScene("Level01_M3_Blockout");
                        break;
                    case 1:
                        GameUtilities.ExitScene();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
