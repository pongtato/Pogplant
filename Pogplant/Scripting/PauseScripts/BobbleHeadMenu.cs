using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class BobbleHeadMenu : PauseBehaviour
    {
        public enum BUTTONS
        {
            FIRST = 0,
            SECOND = 1,
            THIRD = 2,
        }

        private int m_activeIndex = 0;
        private Dictionary<string, GameObject> buttonMap = new Dictionary<string, GameObject>();
        private bool m_menuActive = false;
        private float m_arrowOffsetY = 0.15f;

        public BobbleHeadMenu()
        {

        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();

            uint arrowButton = ECS.FindChildEntityWithName(entityID, "ArrowButton");
            ECS.GetTransformECS(arrowButton, ref pos, ref rot, ref scale);
            buttonMap.Add("ArrowButton", new GameObject(arrowButton, new Transform(pos, rot, scale), "ArrowButton"));

            uint first = ECS.FindChildEntityWithName(entityID, "FirstButton");
            ECS.GetTransformECS(first, ref pos, ref rot, ref scale);
            buttonMap.Add("FirstButton", new GameObject(first, new Transform(pos, rot, scale), "FirstButton"));

            uint second = ECS.FindChildEntityWithName(entityID, "SecondButton");
            ECS.GetTransformECS(second, ref pos, ref rot, ref scale);
            buttonMap.Add("SecondButton", new GameObject(second, new Transform(pos, rot, scale), "SecondButton"));

            uint third = ECS.FindChildEntityWithName(entityID, "ThirdButton");
            ECS.GetTransformECS(third, ref pos, ref rot, ref scale);
            buttonMap.Add("ThirdButton", new GameObject(third, new Transform(pos, rot, scale), "ThirdButton"));

            ECS.SetActive(entityID, false);
            m_menuActive = false;
        }

        public override void Start()
        {
        }

        public override void Update(float dt)
        {
            if (PlayerScript.m_EnableBonusScreen)
            {
                if(!m_menuActive)
                {
                    m_menuActive = true;
                }
                UpdateInputs();
            }
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
            if (InputUtility.onKeyTriggered("MENULEFT"))
            {
                --m_activeIndex;

                if (m_activeIndex < (int)BUTTONS.FIRST)
                {
                    m_activeIndex = (int)BUTTONS.THIRD;

                }
                //Console.WriteLine("Active index is: " + active_index);
            }
            else if (InputUtility.onKeyTriggered("MENURIGHT"))
            {
                ++m_activeIndex;

                if (m_activeIndex > (int)BUTTONS.THIRD)
                {
                    m_activeIndex = (int)BUTTONS.FIRST;

                }
            }

            bool selected = InputUtility.onKeyHeld("MENUSELECT");

            if(selected)
            {
                switch (m_activeIndex)
                {
                    case 0:
                        Console.WriteLine("Score Multiplier Boost Selected.");
                        ++PlayerScript.m_ScoreMultiplierBobbleCount;
                        break;
                    case 1:
                        Console.WriteLine("Shield Invulnerable Duration Boost Selected.");
                        ++PlayerScript.m_ShieldBobbleCount;
                        break;
                    case 2:
                        Console.WriteLine("Combo Decay Timer Boost Selected");
                        ++PlayerScript.m_ComboDecayBobbleCount;
                        break;
                }
                PlayerScript.m_EnableBonusScreen = false;
                PlayerScript.m_BonusItem = 0;
                m_menuActive = false;
                ECS.SetActive(entityID, false);
                GameUtilities.ResumeScene();
            }
            else 
            {
                switch (m_activeIndex)
                {
                    case 0:
                        ECS.SetPosition(buttonMap["ArrowButton"].id, 
                            new Vector3(buttonMap["FirstButton"].transform.Position.X, buttonMap["FirstButton"].transform.Position.Y + m_arrowOffsetY, buttonMap["FirstButton"].transform.Position.Z));
                        break;
                    case 1:
                        ECS.SetPosition(buttonMap["ArrowButton"].id,
                            new Vector3(buttonMap["SecondButton"].transform.Position.X, buttonMap["SecondButton"].transform.Position.Y + m_arrowOffsetY, buttonMap["SecondButton"].transform.Position.Z));
                        break;
                    case 2:
                        ECS.SetPosition(buttonMap["ArrowButton"].id,
                            new Vector3(buttonMap["ThirdButton"].transform.Position.X, buttonMap["ThirdButton"].transform.Position.Y + m_arrowOffsetY, buttonMap["ThirdButton"].transform.Position.Z));
                        break;
                }
            }
        }
    }
}
