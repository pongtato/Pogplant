using System;
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
            IMG5,
            EXIT
        }

        private Dictionary<string, GameObject> mEntities = new Dictionary<string, GameObject>();
        private MENUSTATE m_MenuState = MENUSTATE.IMG1;
        private MENUSTATE m_PrevMenuState = MENUSTATE.IMG1;
        static public bool m_EnableHTP = false;
        static public bool m_IsResumeScene = false;
        private bool m_isActive = false;
        uint m_MenuGroupID;

        public enum ANIM_STATE
        {
            OPENING,
            CLOSING,
            INPUT_READY,
            INACTIVE
        }

        public static ANIM_STATE current_state;
        public static float anim_lerp_step;
        const float anim_lerp_speed = 1.5f;
        const float lerp_min = 0.0f;
        const float lerp_max = 1.0f;
        Vector3 lerp_initial_position;
        Vector3 lerp_final_position;

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
            for (int i = 1; i < 6; ++i)
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
            ECS.GetTransformECS(conA, ref pos, ref rot, ref scale);
            mEntities.Add("HTP_ConA", new GameObject(conA, new Transform(pos, rot, scale), "HTP_ConA"));

            uint conD = ECS.FindEntityWithName("HTP_ConD");
            ECS.GetTransformECS(conD, ref pos, ref rot, ref scale);
            mEntities.Add("HTP_ConD", new GameObject(conD, new Transform(pos, rot, scale), "HTP_ConD"));

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

            //Back
            uint kb_ct = ECS.FindEntityWithName("HTP_KBControlText");
            ECS.GetTransformECS(kb_ct, ref pos, ref rot, ref scale);
            mEntities.Add("HTP_KBControlText", new GameObject(kb_ct, new Transform(pos, rot, scale), "HTP_KBControlText"));

            uint con_ct = ECS.FindEntityWithName("HTP_CtrlerControlText");
            ECS.GetTransformECS(con_ct, ref pos, ref rot, ref scale);
            mEntities.Add("HTP_CtrlerControlText", new GameObject(con_ct, new Transform(pos, rot, scale), "HTP_CtrlerControlText"));

            m_MenuGroupID = ECS.FindEntityWithName("HowToPlayMenuGroup");

            current_state = ANIM_STATE.INACTIVE;
            anim_lerp_step = 0.0f;
            lerp_initial_position = new Vector3(0.0f, -1.1f, ECS.GetGlobalPosition(entityID).Z);
            lerp_final_position = new Vector3(0.0f, 0.0f, ECS.GetGlobalPosition(entityID).Z);
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
                    //foreach (var entity in mEntities)
                    //{
                    //    ECS.SetActive(entity.Value.id, false);
                    //}
                    ResetMenuState();

                    ECS.SetActive(mEntities["HTP_BG"].id, true);
                    ECS.SetActive(mEntities["HTP_Image1"].id, true);

                    if (InputUtility.IsControlledBeingUsed())
                    {
                        ECS.SetActive(mEntities["HTP_ConD"].id, true);
                        ECS.SetActive(mEntities["HTP_CtrlerControlText"].id, true);
                    }
                    else
                    {
                        ECS.SetActive(mEntities["HTP_KeyD"].id, true);
                        ECS.SetActive(mEntities["HTP_KBControlText"].id, true);
                    }

                    ECS.SetActive(mEntities["HTP_ArrowR"].id, true);
                    
                    m_MenuState = MENUSTATE.IMG1;
                    m_PrevMenuState = MENUSTATE.IMG1;
                    m_isActive = true;
                }
                else
                {
                    switch (current_state)
                    {
                        case ANIM_STATE.INPUT_READY:
                            UpdateMenuInput();
                            break;
                        case ANIM_STATE.OPENING:
                            UpdateSettingsMenuAnimation(true, dt);
                            break;
                        case ANIM_STATE.CLOSING:
                            UpdateSettingsMenuAnimation(false, dt);
                            break;
                    }
                }
            }
        }

        void ResetMenuState()
        {
            ECS.SetActive(mEntities["HTP_KeyA"].id, false);
            ECS.SetActive(mEntities["HTP_ConA"].id, false);
            ECS.SetActive(mEntities["HTP_ArrowL"].id, false);
            ECS.SetActive(mEntities["HTP_Image2"].id, false);
            ECS.SetActive(mEntities["HTP_Image3"].id, false);
            ECS.SetActive(mEntities["HTP_Image4"].id, false);
            ECS.SetActive(mEntities["HTP_Image5"].id, false);
        }

        void UpdateSettingsMenuAnimation(bool opening, float dt)
        {
            anim_lerp_step += dt * anim_lerp_speed;

            //Menu slide in from below
            if (opening)
            {
                if (anim_lerp_step > lerp_max)
                {
                    anim_lerp_step = lerp_max;
                    current_state = ANIM_STATE.INPUT_READY;
                }

                ECS.SetGlobalPosition(m_MenuGroupID, Vector3.Lerp(lerp_initial_position, lerp_final_position, anim_lerp_step));
            }
            //Menu slide out to below
            else
            {
                if (anim_lerp_step > lerp_max)
                {
                    anim_lerp_step = lerp_max;
                    m_isActive = false;
                    m_EnableHTP = false;

                    current_state = ANIM_STATE.INACTIVE;

                    if (GameUtilities.GetSceneName() == "MainMenu")
                        MainMenuController.menu_state = MainMenuController.MENU_STATE.INPUT_READY;
                    else
                        PauseMenu.menu_state = PauseMenu.MENU_STATE.INPUT_READY;

                    if (m_IsResumeScene)
                        GameUtilities.ResumeScene();
                }

                ECS.SetGlobalPosition(m_MenuGroupID, Vector3.Lerp(lerp_final_position, lerp_initial_position, anim_lerp_step));
                
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
            anim_lerp_step = 0.0f;
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
                if (m_MenuState < MENUSTATE.IMG5)
                    m_MenuState++;
            }
            else if (InputUtility.onKeyTriggered("MENUBACK") || InputUtility.onKeyTriggered("ESCAPE"))
            {
                ECS.PlayAudio(entityID, 3, "SFX");
                m_PrevMenuState = m_MenuState;
                m_MenuState = MENUSTATE.EXIT;
                current_state = ANIM_STATE.CLOSING;
                anim_lerp_step = lerp_min;
            }

            if (m_PrevMenuState != m_MenuState)
            {
                switch (m_MenuState)
                {
                    case MENUSTATE.IMG1:
                        {
                            if (InputUtility.IsControlledBeingUsed())
                            {
                                ECS.SetActive(mEntities["HTP_ConD"].id, true);
                                ECS.SetActive(mEntities["HTP_CtrlerControlText"].id, true);
                            }
                            else
                            {
                                ECS.SetActive(mEntities["HTP_KeyD"].id, true);
                                ECS.SetActive(mEntities["HTP_KBControlText"].id, true);
                            }
                            ECS.SetActive(mEntities["HTP_Image1"].id, true);

                            ECS.SetActive(mEntities["HTP_KeyA"].id, false);
                            ECS.SetActive(mEntities["HTP_ConA"].id, false);
                            ECS.SetActive(mEntities["HTP_ArrowL"].id, false);
                            ECS.SetActive(mEntities["HTP_Image2"].id, false);
                        }
                        break;
                    case MENUSTATE.IMG2:
                        {
                            if (InputUtility.IsControlledBeingUsed())
                            {
                                ECS.SetActive(mEntities["HTP_ConA"].id, true);
                                ECS.SetActive(mEntities["HTP_ConD"].id, true);
                                ECS.SetActive(mEntities["HTP_CtrlerControlText"].id, true);
                            }
                            else
                            {
                                ECS.SetActive(mEntities["HTP_KeyA"].id, true);
                                ECS.SetActive(mEntities["HTP_KeyD"].id, true);
                                ECS.SetActive(mEntities["HTP_KBControlText"].id, true);
                            }
                            
                            ECS.SetActive(mEntities["HTP_Image2"].id, true);
                            ECS.SetActive(mEntities["HTP_ArrowL"].id, true);

                            ECS.SetActive(mEntities["HTP_Image1"].id, false);
                            ECS.SetActive(mEntities["HTP_Image3"].id, false);
                        }
                        break;
                    case MENUSTATE.IMG3:
                        {
                            if (InputUtility.IsControlledBeingUsed())
                            {
                                ECS.SetActive(mEntities["HTP_ConA"].id, true);
                                ECS.SetActive(mEntities["HTP_ConD"].id, true);
                                ECS.SetActive(mEntities["HTP_CtrlerControlText"].id, true);
                            }
                            else
                            {
                                ECS.SetActive(mEntities["HTP_KeyA"].id, true);
                                ECS.SetActive(mEntities["HTP_KeyD"].id, true);
                                ECS.SetActive(mEntities["HTP_KBControlText"].id, true);
                            }

                            ECS.SetActive(mEntities["HTP_Image3"].id, true);
                            ECS.SetActive(mEntities["HTP_ArrowR"].id, true);

                            ECS.SetActive(mEntities["HTP_Image2"].id, false);
                            ECS.SetActive(mEntities["HTP_Image4"].id, false);
                        }
                        break;
                    case MENUSTATE.IMG4:
                        {
                            
                            if (InputUtility.IsControlledBeingUsed())
                            {
                                ECS.SetActive(mEntities["HTP_ConA"].id, true);
                                ECS.SetActive(mEntities["HTP_ConD"].id, true);
                                ECS.SetActive(mEntities["HTP_CtrlerControlText"].id, true);
                            }
                            else
                            {
                                ECS.SetActive(mEntities["HTP_KeyA"].id, true);
                                ECS.SetActive(mEntities["HTP_KeyD"].id, true);
                                ECS.SetActive(mEntities["HTP_KBControlText"].id, true);
                            }

                            ECS.SetActive(mEntities["HTP_Image4"].id, true);
                            ECS.SetActive(mEntities["HTP_Image3"].id, false);
                            ECS.SetActive(mEntities["HTP_Image5"].id, false);
                            ECS.SetActive(mEntities["HTP_ArrowR"].id, true);
                        }
                        break;
                    case MENUSTATE.IMG5:
                        {
                            if (InputUtility.IsControlledBeingUsed())
                            {
                                ECS.SetActive(mEntities["HTP_ConA"].id, true);
                                ECS.SetActive(mEntities["HTP_CtrlerControlText"].id, true);
                            }
                            else
                            {
                                ECS.SetActive(mEntities["HTP_KeyA"].id, true);
                                ECS.SetActive(mEntities["HTP_KBControlText"].id, true);
                            }
                            
                            ECS.SetActive(mEntities["HTP_Image5"].id, true);

                            ECS.SetActive(mEntities["HTP_KeyD"].id, false);
                            ECS.SetActive(mEntities["HTP_ConD"].id, false);
                            ECS.SetActive(mEntities["HTP_Image4"].id, false);
                            ECS.SetActive(mEntities["HTP_ArrowR"].id, false);
                        }
                        break;
                    case MENUSTATE.EXIT:
                        {

                        }
                        break;
                }

                m_PrevMenuState = m_MenuState;
            }
        }
    }
}
