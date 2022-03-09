﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class HowToPlayMenu : PauseBehaviour
    {
        enum MENUSTATE
        {
            IMG1,
            IMG2,
            IMG3,
            IMG4,
            EXIT
        }

        private Dictionary<string, GameObject> mEntities = new Dictionary<string, GameObject>();
        private MENUSTATE m_MenuState = MENUSTATE.IMG1;
        private MENUSTATE m_PrevMenuState = MENUSTATE.IMG1;
        static public bool m_EnableHTP = false;
        static public bool m_IsResumeScene = false;
        private bool m_isActive = false;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            m_EnableHTP = false;
            m_isActive = false;
            m_MenuState = MENUSTATE.IMG1;
            m_PrevMenuState = MENUSTATE.IMG1;

            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();
            for (int i = 1; i < 5; ++i)
            {
                uint htpImg = ECS.FindEntityWithName("HTP_Image" + i);
                ECS.GetTransformECS(htpImg, ref pos, ref rot, ref scale);
                mEntities.Add("HTP_Image" + i, new GameObject(htpImg, new Transform(pos, rot, scale), "HTP_Image" + i));
            }

            // Add keys
            uint keyA = ECS.FindEntityWithName("HTP_KeyA");
            ECS.GetTransformECS(keyA, ref pos, ref rot, ref scale);
            mEntities.Add("HTP_KeyA", new GameObject(keyA, new Transform(pos, rot, scale), "Key_A"));

            uint keyD = ECS.FindEntityWithName("HTP_KeyD");
            ECS.GetTransformECS(keyD, ref pos, ref rot, ref scale);
            mEntities.Add("HTP_KeyD", new GameObject(keyD, new Transform(pos, rot, scale), "Key_D"));

            uint conA = ECS.FindEntityWithName("HTP_ConA");
            ECS.GetTransformECS(keyA, ref pos, ref rot, ref scale);
            mEntities.Add("HTP_ConA", new GameObject(keyA, new Transform(pos, rot, scale), "HTP_ConA"));

            uint conD = ECS.FindEntityWithName("HTP_ConD");
            ECS.GetTransformECS(keyD, ref pos, ref rot, ref scale);
            mEntities.Add("HTP_ConD", new GameObject(keyD, new Transform(pos, rot, scale), "HTP_ConD"));

            // Add Arrows
            uint arrowR = ECS.FindEntityWithName("HTP_ArrowR");
            ECS.GetTransformECS(arrowR, ref pos, ref rot, ref scale);
            mEntities.Add("HTP_ArrowR", new GameObject(arrowR, new Transform(pos, rot, scale), "HTP_ArrowR"));
            
            uint arrowL = ECS.FindEntityWithName("HTP_ArrowL");
            ECS.GetTransformECS(arrowL, ref pos, ref rot, ref scale);
            mEntities.Add("HTP_ArrowL", new GameObject(arrowL, new Transform(pos, rot, scale), "HTP_ArrowL"));

            // Add BG
            uint bg = ECS.FindEntityWithName("HTP_BG");
            ECS.GetTransformECS(bg, ref pos, ref rot, ref scale);
            mEntities.Add("HTP_BG", new GameObject(bg, new Transform(pos, rot, scale), "HTP_BG"));

            uint ct = ECS.FindEntityWithName("HTP_ControlText");
            ECS.GetTransformECS(ct, ref pos, ref rot, ref scale);
            mEntities.Add("HTP_ControlText", new GameObject(ct, new Transform(pos, rot, scale), "HTP_ControlText"));
            
        }

        public override void Start()
        {
        }

        public override void Update(float dt)
        {
            if (m_EnableHTP)
            {
                if (!m_isActive)
                {
                    ECS.SetActive(mEntities["HTP_BG"].id, true);
                    ECS.SetActive(mEntities["HTP_Image1"].id, true);
                    ECS.SetActive(mEntities["HTP_KeyD"].id, true);
                    ECS.SetActive(mEntities["HTP_ArrowR"].id, true);
                    ECS.SetActive(mEntities["HTP_ControlText"].id, true);
                    m_MenuState = MENUSTATE.IMG1;
                    m_PrevMenuState = MENUSTATE.IMG1;
                    m_isActive = true;
                }
                else
                {
                    UpdateMenuInput();

                    if(m_PrevMenuState != m_MenuState)
                    {
                        switch (m_MenuState)
                        {
                        case MENUSTATE.IMG1:
                            {
                                ECS.SetActive(mEntities["HTP_Image1"].id, true);
                                ECS.SetActive(mEntities["HTP_KeyD"].id, true);

                                ECS.SetActive(mEntities["HTP_KeyA"].id, false);
                                ECS.SetActive(mEntities["HTP_ArrowL"].id, false);
                                ECS.SetActive(mEntities["HTP_Image2"].id, false);
                            }
                            break;
                        case MENUSTATE.IMG2:
                            {
                                ECS.SetActive(mEntities["HTP_KeyD"].id, true);
                                ECS.SetActive(mEntities["HTP_KeyA"].id, true);
                                ECS.SetActive(mEntities["HTP_Image2"].id, true);
                                ECS.SetActive(mEntities["HTP_ArrowL"].id, true);

                                ECS.SetActive(mEntities["HTP_Image1"].id, false);
                                ECS.SetActive(mEntities["HTP_Image3"].id, false);
                            }
                            break;
                        case MENUSTATE.IMG3:
                            {
                                ECS.SetActive(mEntities["HTP_KeyD"].id, true);
                                ECS.SetActive(mEntities["HTP_KeyA"].id, true);
                                ECS.SetActive(mEntities["HTP_Image3"].id, true);
                                ECS.SetActive(mEntities["HTP_ArrowR"].id, true);

                                ECS.SetActive(mEntities["HTP_Image2"].id, false);
                                ECS.SetActive(mEntities["HTP_Image4"].id, false);
                            }
                            break;
                        case MENUSTATE.IMG4:
                            {
                                ECS.SetActive(mEntities["HTP_Image4"].id, true);
                                ECS.SetActive(mEntities["HTP_KeyA"].id, true);

                                ECS.SetActive(mEntities["HTP_KeyD"].id, false);
                                ECS.SetActive(mEntities["HTP_Image3"].id, false);
                                ECS.SetActive(mEntities["HTP_ArrowR"].id, false);
                            }
                            break;
                        case MENUSTATE.EXIT:
                            {
                                m_isActive = false;
                                m_EnableHTP = false;
                                foreach (var entity in mEntities)
                                {
                                    ECS.SetActive(entity.Value.id, false);
                                }
                                if(m_IsResumeScene)
                                    GameUtilities.ResumeScene();
                            }
                            break;
                        }

                        m_PrevMenuState = m_MenuState;
                    }
                }
            }
        }

        public override void OnTriggerEnter(uint id)
        {
        }

        public override void OnTriggerExit(uint id)
        {
        }

        public static void EnableHowToPlayMenu(bool isResumeScene)
        {
            m_EnableHTP = true;
            m_IsResumeScene = isResumeScene;
        }

        private void UpdateMenuInput()
        {
            if (InputUtility.onKeyTriggered("MENULEFT"))
            {
                ECS.PlayAudio(entityID, 0, "SFX");
                m_PrevMenuState = m_MenuState;
                if (m_MenuState > MENUSTATE.IMG1)
                    m_MenuState--;
            }
            else if (InputUtility.onKeyTriggered("MENURIGHT"))
            {
                ECS.PlayAudio(entityID, 1, "SFX");
                m_PrevMenuState = m_MenuState;
                if (m_MenuState < MENUSTATE.IMG4)
                    m_MenuState++;
            }
            else if (InputUtility.onKeyTriggered("ESCAPE"))
            {
                ECS.PlayAudio(entityID, 3, "SFX");
                m_PrevMenuState = m_MenuState;
                m_MenuState = MENUSTATE.EXIT;
            }
        }
    }
}