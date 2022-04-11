using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class BobbleHeadMenu : PauseBehaviour
    {
        public enum IMAGENUM
        {
            FIRST = 1,
            SECOND = 2,
            THIRD = 3,
        }

        private IMAGENUM m_currImg = IMAGENUM.FIRST;
        private IMAGENUM m_prevImg = IMAGENUM.FIRST;
        private Dictionary<string, GameObject> m_Entities = new Dictionary<string, GameObject>();
        private bool m_menuActive = false;
        private bool m_CallOnce = false;
        //uint m_BobbleTimmy;
        //uint m_BobbleProf;
        //uint m_BobbleBoobas;
        uint m_MenuGroupID;

        float m_Timer = 0.0f;
        float m_AnimationSpeed = 5.0f;
        //private Dictionary<string, GameObject> m_EntitiesMap = new Dictionary<string, GameObject>();
        Transform m_Trans = new Transform();

        public BobbleHeadMenu()
        {
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();

            // Boxes
            uint bg = ECS.FindEntityWithName("TS_BG");
            ECS.GetTransformECS(bg, ref pos, ref rot, ref scale);
            m_Entities.Add("TS_BG", new GameObject(bg, new Transform(pos, rot, scale), "TS_BG"));

            uint mBox = ECS.FindEntityWithName("TS_MBox");
            ECS.GetTransformECS(mBox, ref pos, ref rot, ref scale);
            m_Entities.Add("TS_MBox", new GameObject(mBox, new Transform(pos, rot, scale), "TS_MBox"));

            uint lBox = ECS.FindEntityWithName("TS_LBox");
            ECS.GetTransformECS(lBox, ref pos, ref rot, ref scale);
            m_Entities.Add("TS_LBox", new GameObject(lBox, new Transform(pos, rot, scale), "TS_LBox"));

            uint rBox = ECS.FindEntityWithName("TS_RBox");
            ECS.GetTransformECS(rBox, ref pos, ref rot, ref scale);
            m_Entities.Add("TS_RBox", new GameObject(rBox, new Transform(pos, rot, scale), "TS_RBox"));

            uint img1 = ECS.FindEntityWithName("TS_IMG1");
            ECS.GetTransformECS(img1, ref pos, ref rot, ref scale);
            m_Entities.Add("TS_IMG1", new GameObject(img1, new Transform(pos, rot, scale), "TS_IMG1"));

            uint img2 = ECS.FindEntityWithName("TS_IMG2");
            ECS.GetTransformECS(img2, ref pos, ref rot, ref scale);
            m_Entities.Add("TS_IMG2", new GameObject(img2, new Transform(pos, rot, scale), "TS_IMG2"));

            uint img3 = ECS.FindEntityWithName("TS_IMG3");
            ECS.GetTransformECS(img3, ref pos, ref rot, ref scale);
            m_Entities.Add("TS_IMG3", new GameObject(img3, new Transform(pos, rot, scale), "TS_IMG3"));

            uint txt1 = ECS.FindEntityWithName("TS_TXT1");
            ECS.GetTransformECS(txt1, ref pos, ref rot, ref scale);
            m_Entities.Add("TS_TXT1", new GameObject(txt1, new Transform(pos, rot, scale), "TS_TXT1"));

            uint txt2 = ECS.FindEntityWithName("TS_TXT2");
            ECS.GetTransformECS(txt2, ref pos, ref rot, ref scale);
            m_Entities.Add("TS_TXT2", new GameObject(txt2, new Transform(pos, rot, scale), "TS_TXT2"));

            uint txt3 = ECS.FindEntityWithName("TS_TXT3");
            ECS.GetTransformECS(txt3, ref pos, ref rot, ref scale);
            m_Entities.Add("TS_TXT3", new GameObject(txt3, new Transform(pos, rot, scale), "TS_TXT3"));

            uint ct = ECS.FindEntityWithName("TS_ControlText");
            ECS.GetTransformECS(ct, ref pos, ref rot, ref scale);
            m_Entities.Add("TS_ControlText", new GameObject(ct, new Transform(pos, rot, scale), "TS_ControlText"));

            uint ct_r = ECS.FindEntityWithName("TS_ControlText_Remote");
            ECS.GetTransformECS(ct_r, ref pos, ref rot, ref scale);
            m_Entities.Add("TS_ControlText_Remote", new GameObject(ct_r, new Transform(pos, rot, scale), "TS_ControlText_Remote"));

            uint bt1 = ECS.FindEntityWithName("BobbleTimmy1");
            ECS.GetTransformECS(bt1, ref pos, ref rot, ref scale);
            m_Entities.Add("BobbleTimmy1", new GameObject(bt1, new Transform(pos, rot, scale), "BobbleTimmy1"));

            bt1 = ECS.FindEntityWithName("BobbleTimmy2");
            ECS.GetTransformECS(bt1, ref pos, ref rot, ref scale);
            m_Entities.Add("BobbleTimmy2", new GameObject(bt1, new Transform(pos, rot, scale), "BobbleTimmy2"));

            bt1 = ECS.FindEntityWithName("BobbleTimmy3");
            ECS.GetTransformECS(bt1, ref pos, ref rot, ref scale);
            m_Entities.Add("BobbleTimmy3", new GameObject(bt1, new Transform(pos, rot, scale), "BobbleTimmy3"));

            uint bp1 = ECS.FindEntityWithName("BobbleProf1");
            ECS.GetTransformECS(bp1, ref pos, ref rot, ref scale);
            m_Entities.Add("BobbleProf1", new GameObject(bp1, new Transform(pos, rot, scale), "BobbleProf1"));

            bp1 = ECS.FindEntityWithName("BobbleProf2");
            ECS.GetTransformECS(bp1, ref pos, ref rot, ref scale);
            m_Entities.Add("BobbleProf2", new GameObject(bp1, new Transform(pos, rot, scale), "BobbleProf2"));

            bp1 = ECS.FindEntityWithName("BobbleProf3");
            ECS.GetTransformECS(bp1, ref pos, ref rot, ref scale);
            m_Entities.Add("BobbleProf3", new GameObject(bp1, new Transform(pos, rot, scale), "BobbleProf3"));

            uint bb1 = ECS.FindEntityWithName("BobbleBoobas1");
            ECS.GetTransformECS(bb1, ref pos, ref rot, ref scale);
            m_Entities.Add("BobbleBoobas1", new GameObject(bb1, new Transform(pos, rot, scale), "BobbleBoobas1"));

            bb1 = ECS.FindEntityWithName("BobbleBoobas2");
            ECS.GetTransformECS(bb1, ref pos, ref rot, ref scale);
            m_Entities.Add("BobbleBoobas2", new GameObject(bb1, new Transform(pos, rot, scale), "BobbleBoobas2"));

            bb1 = ECS.FindEntityWithName("BobbleBoobas3");
            ECS.GetTransformECS(bb1, ref pos, ref rot, ref scale);
            m_Entities.Add("BobbleBoobas3", new GameObject(bb1, new Transform(pos, rot, scale), "BobbleBoobas3"));

            //m_BobbleTimmy = ECS.FindEntityWithName("BobbleTimmy");
            //m_BobbleProf = ECS.FindEntityWithName("BobbleProf");
            //m_BobbleBoobas = ECS.FindEntityWithName("BobbleBoobas");

            m_MenuGroupID = ECS.FindEntityWithName("TrinketShopMenuGroup");
            ECS.GetTransformECS(m_MenuGroupID, ref m_Trans.Position, ref m_Trans.Rotation, ref m_Trans.Scale);
            m_Entities.Add("MenuMid", new GameObject(m_MenuGroupID, new Transform(new Vector3(m_Trans.Position.X, m_Trans.Position.Y + 1.0f, m_Trans.Position.Z), m_Trans.Rotation, m_Trans.Scale), "MenuMid"));
            m_Entities.Add("MenuEnd", new GameObject(m_MenuGroupID, new Transform(new Vector3(m_Trans.Position.X, m_Trans.Position.Y + 0.85f, m_Trans.Position.Z), m_Trans.Rotation, m_Trans.Scale), "MenuEnd"));

            m_currImg = IMAGENUM.FIRST;
            m_prevImg = IMAGENUM.FIRST;
            m_menuActive = false;
            m_CallOnce = false;
        }

        public override void Start()
        {
        }

        public override void Update(float dt)
        {
            if (InputUtility.IsControlledBeingUsed())
            {
                ECS.SetActive(m_Entities["TS_ControlText_Remote"].id, true);
                ECS.SetActive(m_Entities["TS_ControlText"].id, false);
            }
            else
            {
                ECS.SetActive(m_Entities["TS_ControlText"].id, true);
                ECS.SetActive(m_Entities["TS_ControlText_Remote"].id, false);
            }

            if (PlayerScript.m_EnableBonusScreen)
            {
                if (!m_menuActive)
                {
                    ECS.PlayAudio(entityID, 3, "SFX");
                    m_menuActive = true;
                    m_Timer = 0.0f;
                    ECS.SetActive(m_Entities["TS_BG"].id, true);
                    ECS.SetActive(m_Entities["TS_TXT1"].id, true);
                    ECS.SetActive(m_Entities["TS_IMG1"].id, true);
                    ECS.SetActive(m_Entities["TS_IMG2"].id, true);
                    ECS.SetActive(m_Entities["TS_IMG3"].id, true);
                    //ECS.SetActive(m_Entities["TS_ControlText"].id, false);
                    ECS.SetPosition(m_Entities["TS_IMG1"].id,
                        new Vector3(m_Entities["TS_MBox"].transform.Position.X, m_Entities["TS_MBox"].transform.Position.Y, m_Entities["TS_IMG1"].transform.Position.Z));
                    ECS.SetPosition(m_Entities["TS_IMG2"].id,
                        new Vector3(m_Entities["TS_LBox"].transform.Position.X, m_Entities["TS_LBox"].transform.Position.Y, m_Entities["TS_IMG2"].transform.Position.Z));
                    ECS.SetPosition(m_Entities["TS_IMG3"].id,
                        new Vector3(m_Entities["TS_RBox"].transform.Position.X, m_Entities["TS_RBox"].transform.Position.Y, m_Entities["TS_IMG3"].transform.Position.Z));
                    m_currImg = IMAGENUM.FIRST;
                    m_prevImg = IMAGENUM.FIRST;
                }
                else
                {
                    m_Timer += dt;
                    ECS.GetTransformECS(m_MenuGroupID, ref m_Trans.Position, ref m_Trans.Rotation, ref m_Trans.Scale);

                    if(m_Timer <= 0.5f)
                    {
                        ECS.SetPosition(m_MenuGroupID, Vector3.Lerp(m_Trans.Position, m_Entities["MenuMid"].transform.Position, m_AnimationSpeed * dt));
                    }
                    else if(m_Timer > 0.5f && m_Timer <= 1.0f)
                    {
                        ECS.SetPosition(m_MenuGroupID, Vector3.Lerp(m_Trans.Position, m_Entities["MenuEnd"].transform.Position, m_AnimationSpeed * dt));
                    }
                    else
                    {
                        if(!m_CallOnce)
                        {
                            //ECS.SetActive(m_Entities["TS_ControlText"].id, true);
                            m_CallOnce = false;
                        }
                        UpdateInputs();
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

        void UpdateInputs()
        {
            //Key input
            if (InputUtility.onKeyTriggered("MENULEFT"))
            {
                m_prevImg = m_currImg;
                if (m_currImg >= IMAGENUM.THIRD)
                    m_currImg = IMAGENUM.FIRST;
                else
                    ++m_currImg;
                ECS.PlayAudio(entityID, 0, "SFX");
            }
            else if (InputUtility.onKeyTriggered("MENURIGHT"))
            {
                m_prevImg = m_currImg;
                if (m_currImg <= IMAGENUM.FIRST)
                    m_currImg = IMAGENUM.THIRD;
                else
                    --m_currImg;
                ECS.PlayAudio(entityID, 1, "SFX");
            }

            //Console.WriteLine(m_currImg);

            bool selected = InputUtility.onKeyHeld("MENUSELECT");

            if(selected)
            {
                switch (m_currImg)
                {
                    case IMAGENUM.FIRST:
                        ECS.SetActive(m_Entities["BobbleBoobas" + PlayerScript.m_CollectiblesCount.ToString()].id, true);
                        //Console.WriteLine("Score Multiplier Boost Selected.");
                        ++PlayerScript.m_ScoreMultiplierBobbleCount;
                        break;
                    case IMAGENUM.SECOND:
                        ECS.SetActive(m_Entities["BobbleProf" + PlayerScript.m_CollectiblesCount.ToString()].id, true);
                        //ECS.SetActive(m_BobbleProf, true);
                        //Console.WriteLine("Combo Decay Timer Boost Selected");
                        ++PlayerScript.m_ComboBonusBobbleCount;
                        break;
                    case IMAGENUM.THIRD:
                        ECS.SetActive(m_Entities["BobbleTimmy" + PlayerScript.m_CollectiblesCount.ToString()].id, true);
                        //ECS.SetActive(m_BobbleTimmy, true);
                        //Console.WriteLine("Shield Invulnerable Duration Boost Selected.");
                        //PlayerScript.m_ShieldHitCountMax = PlayerScript.m_ShieldHitCountMax * PlayerScript.m_ShieldBobbleCount;
                        //PlayerScript.m_ShieldHitCount = PlayerScript.m_ShieldHitCountMax;
                        PlayerScript.m_ShieldHitCount += 3;
                        //Console.WriteLine("Max Shield Count:" + PlayerScript.m_ShieldHitCountMax);
                        //Console.WriteLine("Shield Count:" + PlayerScript.m_ShieldHitCount);
                        //GameUtilities.UpdateShieldUI();
                        ECS.SetActive(PlayerScript.m_shieldParent, true);
                        GameUtilities.UpdateShieldUI(PlayerScript.m_shieldNumberID, PlayerScript.m_ShieldHitCount);
                        //ECS.SetScale(PlayerScript.m_ComboBarID, PlayerScript.m_FullComboBarScale);
                        ++PlayerScript.m_ShieldBobbleCount;
                        break;
                }
                PlayerScript.m_EnableBonusScreen = false;
                PlayerScript.m_BonusItem = 0;
                m_menuActive = false;
                m_currImg = IMAGENUM.FIRST;
                m_prevImg = IMAGENUM.FIRST;
                ECS.SetActive(m_Entities["TS_BG"].id, false);
                ECS.SetActive(m_Entities["TS_IMG1"].id, false);
                ECS.SetActive(m_Entities["TS_IMG2"].id, false);
                ECS.SetActive(m_Entities["TS_IMG3"].id, false);
                ECS.SetActive(m_Entities["TS_TXT1"].id, false);
                ECS.SetActive(m_Entities["TS_TXT2"].id, false);
                ECS.SetActive(m_Entities["TS_TXT3"].id, false);
                ECS.PlayAudio(entityID, 2, "SFX");
                GameUtilities.ResumeScene();
            }
            else 
            {
                if(m_prevImg != m_currImg)
                {
                    switch (m_currImg)
                    {
                        case IMAGENUM.FIRST:
                            ECS.SetPosition(m_Entities["TS_IMG1"].id,
                                new Vector3(m_Entities["TS_MBox"].transform.Position.X, m_Entities["TS_MBox"].transform.Position.Y, m_Entities["TS_IMG1"].transform.Position.Z));
                            ECS.SetPosition(m_Entities["TS_IMG2"].id,
                                new Vector3(m_Entities["TS_LBox"].transform.Position.X, m_Entities["TS_LBox"].transform.Position.Y, m_Entities["TS_IMG2"].transform.Position.Z));
                            ECS.SetPosition(m_Entities["TS_IMG3"].id,
                                new Vector3(m_Entities["TS_RBox"].transform.Position.X, m_Entities["TS_RBox"].transform.Position.Y, m_Entities["TS_IMG3"].transform.Position.Z));
                            ECS.SetActive(m_Entities["TS_TXT1"].id, true);
                            ECS.SetActive(m_Entities["TS_TXT2"].id, false);
                            ECS.SetActive(m_Entities["TS_TXT3"].id, false);
                            break;
                        case IMAGENUM.SECOND:
                            ECS.SetPosition(m_Entities["TS_IMG2"].id,
                                new Vector3(m_Entities["TS_MBox"].transform.Position.X, m_Entities["TS_MBox"].transform.Position.Y, m_Entities["TS_IMG1"].transform.Position.Z));
                            ECS.SetPosition(m_Entities["TS_IMG3"].id,
                                new Vector3(m_Entities["TS_LBox"].transform.Position.X, m_Entities["TS_LBox"].transform.Position.Y, m_Entities["TS_IMG3"].transform.Position.Z));
                            ECS.SetPosition(m_Entities["TS_IMG1"].id,
                                new Vector3(m_Entities["TS_RBox"].transform.Position.X, m_Entities["TS_RBox"].transform.Position.Y, m_Entities["TS_IMG1"].transform.Position.Z));
                            ECS.SetActive(m_Entities["TS_TXT1"].id, false);
                            ECS.SetActive(m_Entities["TS_TXT2"].id, true);
                            ECS.SetActive(m_Entities["TS_TXT3"].id, false);
                            break;
                        case IMAGENUM.THIRD:
                            ECS.SetPosition(m_Entities["TS_IMG3"].id,
                                new Vector3(m_Entities["TS_MBox"].transform.Position.X, m_Entities["TS_MBox"].transform.Position.Y, m_Entities["TS_IMG3"].transform.Position.Z));
                            ECS.SetPosition(m_Entities["TS_IMG1"].id,
                                new Vector3(m_Entities["TS_LBox"].transform.Position.X, m_Entities["TS_LBox"].transform.Position.Y, m_Entities["TS_IMG1"].transform.Position.Z));
                            ECS.SetPosition(m_Entities["TS_IMG2"].id,
                                new Vector3(m_Entities["TS_RBox"].transform.Position.X, m_Entities["TS_RBox"].transform.Position.Y, m_Entities["TS_IMG2"].transform.Position.Z));
                            ECS.SetActive(m_Entities["TS_TXT1"].id, false);
                            ECS.SetActive(m_Entities["TS_TXT2"].id, false);
                            ECS.SetActive(m_Entities["TS_TXT3"].id, true);
                            break;
                    }
                    m_prevImg = m_currImg;
                }
            }
        }
    }
}
