using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class EndGameMenu : PauseBehaviour
    {
        uint m_EndScoreID;
        uint m_DestructParentID;
        uint m_EvasiveParentID;
        uint m_CollectParentID;
        uint m_MedalsParentID;
        uint m_EndContinueID;

        private Dictionary<string, GameObject> m_LettersMap = new Dictionary<string, GameObject>();
        private Dictionary<uint, Vector3> m_ScaleAnimation = new Dictionary<uint, Vector3>();
        
        float m_Timer = 0.0f;
        float m_AnimationSpeed = 5.0f;
        float m_AnimationLetterSpeed = 5.0f;
        bool m_CallOnce = true;
        bool m_CallOnceDes = true;
        bool m_CallOnceEva = true;
        bool m_CallOnceCol = true;
        bool m_CallOnceMedal = true;
        bool m_CallOnceCont = true;
        uint m_OverallGrade = 0;

        Vector3 m_pos = new Vector3();
        Vector3 m_rot = new Vector3();
        Vector3 m_scale = new Vector3();
        Vector3 m_contScaleSmall;
        Vector3 m_contScaleBig;

        bool m_EnablePlayBig = true;

        // Number of enemies destroyed
        private enum GRADE_DES
        {
            C = 0,
            B = 20,
            A = 40
        }

        // Number of times hit
        private enum GRADE_EVA
        {
            A = 0,
            B = 10,
            C = 20
        }

        // Number of coins collected
        private enum GRADE_COL
        {
            C = 0,
            B = 1,
            A = 2
        }

        private enum MEDALGRADE
        {
            BRONZE = 1,
            SILVER = 2,
            GOLD = 3,
            OVERALLBRONZE = 0,
            OVERALLSILVER = 4,
            OVERALLGOLD = 8,
            COUNT
        }

        /// <summary>
        /// 0. Screen open
        /// 1. Screen close
        /// 2. Gold
        /// 3. Silver
        /// 4. Bronze
        /// 5. Tick
        /// </summary>

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();

            // End Score Text ID
            m_EndScoreID = ECS.FindEntityWithName("EndScoreText");
            m_MedalsParentID = ECS.FindEntityWithName("Medals");

            // Collectibles
            uint collectA = ECS.FindEntityWithName("Collectibles_A");
            ECS.GetTransformECS(collectA, ref pos, ref rot, ref scale);
            m_LettersMap.Add("Collectibles_A", new GameObject(collectA, new Transform(pos, rot, scale), "Collectibles_A"));

            uint collectB = ECS.FindEntityWithName("Collectibles_B");
            ECS.GetTransformECS(collectB, ref pos, ref rot, ref scale);
            m_LettersMap.Add("Collectibles_B", new GameObject(collectB, new Transform(pos, rot, scale), "Collectibles_B"));

            uint collectC = ECS.FindEntityWithName("Collectibles_C");
            ECS.GetTransformECS(collectC, ref pos, ref rot, ref scale);
            m_LettersMap.Add("Collectibles_C", new GameObject(collectC, new Transform(pos, rot, scale), "Collectibles_C"));

            m_CollectParentID = ECS.FindEntityWithName("Collectibles");
            m_ScaleAnimation.Add(m_CollectParentID, new Vector3(1.0f, 1.0f, 1.0f));

            // Destruction
            uint destructA = ECS.FindEntityWithName("Destruction_A");
            ECS.GetTransformECS(destructA, ref pos, ref rot, ref scale);
            m_LettersMap.Add("Destruction_A", new GameObject(destructA, new Transform(pos, rot, scale), "Destruction_A"));

            uint destructB = ECS.FindEntityWithName("Destruction_B");
            ECS.GetTransformECS(destructB, ref pos, ref rot, ref scale);
            m_LettersMap.Add("Destruction_B", new GameObject(destructB, new Transform(pos, rot, scale), "Destruction_B"));

            uint destructC = ECS.FindEntityWithName("Destruction_C");
            ECS.GetTransformECS(destructC, ref pos, ref rot, ref scale);
            m_LettersMap.Add("Destruction_C", new GameObject(destructC, new Transform(pos, rot, scale), "Destruction_C"));

            m_DestructParentID = ECS.FindEntityWithName("Destruction");
            m_ScaleAnimation.Add(m_DestructParentID, new Vector3(1.0f, 1.0f, 1.0f));

            // Evasiveness
            uint evasiveA = ECS.FindEntityWithName("Evasiveness_A");
            ECS.GetTransformECS(evasiveA, ref pos, ref rot, ref scale);
            m_LettersMap.Add("Evasiveness_A", new GameObject(evasiveA, new Transform(pos, rot, scale), "Evasiveness_A"));

            uint evasiveB = ECS.FindEntityWithName("Evasiveness_B");
            ECS.GetTransformECS(evasiveB, ref pos, ref rot, ref scale);
            m_LettersMap.Add("Evasiveness_B", new GameObject(evasiveB, new Transform(pos, rot, scale), "Evasiveness_B"));

            uint evasiveC = ECS.FindEntityWithName("Evasiveness_C");
            ECS.GetTransformECS(evasiveC, ref pos, ref rot, ref scale);
            m_LettersMap.Add("Evasiveness_C", new GameObject(evasiveC, new Transform(pos, rot, scale), "Evasiveness_C"));

            m_EvasiveParentID = ECS.FindEntityWithName("Evasiveness");
            m_ScaleAnimation.Add(m_EvasiveParentID, new Vector3(1.0f, 1.0f, 1.0f));

            // Medals
            uint medalGold = ECS.FindEntityWithName("Medal_Gold");
            ECS.GetTransformECS(medalGold, ref pos, ref rot, ref scale);
            m_LettersMap.Add("Medal_Gold", new GameObject(medalGold, new Transform(pos, rot, scale), "Medal_Gold"));

            uint medalSilver = ECS.FindEntityWithName("Medal_Silver");
            ECS.GetTransformECS(medalSilver, ref pos, ref rot, ref scale);
            m_LettersMap.Add("Medal_Silver", new GameObject(medalSilver, new Transform(pos, rot, scale), "Medal_Silver"));

            uint medalBronze = ECS.FindEntityWithName("Medal_Bronze");
            ECS.GetTransformECS(medalBronze, ref pos, ref rot, ref scale);
            m_LettersMap.Add("Medal_Bronze", new GameObject(medalBronze, new Transform(pos, rot, scale), "Medal_Bronze"));

            // Continue Button
            m_EndContinueID = ECS.FindEntityWithName("EndContinue");

            ECS.GetTransformECS(entityID, ref pos, ref rot, ref scale);
            m_LettersMap.Add("EndMenuControllerStart", new GameObject(entityID, new Transform(pos, rot, scale), "EndMenuController"));
            m_LettersMap.Add("EndMenuControllerMid", new GameObject(entityID, new Transform(new Vector3(pos.X, pos.Y + 1.0f, pos.Z), rot, scale), "EndMenuController"));
            m_LettersMap.Add("EndMenuControllerEnd", new GameObject(entityID, new Transform(new Vector3(pos.X, pos.Y + 0.85f, pos.Z), rot, scale), "EndMenuController"));

            m_contScaleSmall = new Vector3(0.35f, 0.35f, 1.0f);
            m_contScaleBig = new Vector3(0.4f, 0.4f, 1.0f);

            ECS.SetActive(entityID, false);
        }

        public override void Start()
        {
        }

        public override void Update(float dt)
        {
            if(EndGameMenuTrigger.m_EnableEndGameMenu)
            {
                if (m_CallOnce)
                {
                    ECS.PlayAudio(entityID, 0, "SFX");
                    DisableGrades(m_LettersMap["Destruction_C"].id, m_LettersMap["Destruction_B"].id, m_LettersMap["Destruction_A"].id);
                    DisableGrades(m_LettersMap["Evasiveness_C"].id, m_LettersMap["Evasiveness_B"].id, m_LettersMap["Evasiveness_A"].id);
                    DisableGrades(m_LettersMap["Collectibles_C"].id, m_LettersMap["Collectibles_B"].id, m_LettersMap["Collectibles_A"].id);
                    DisableGrades(m_LettersMap["Medal_Bronze"].id, m_LettersMap["Medal_Silver"].id, m_LettersMap["Medal_Gold"].id);
                    ECS.SetActive(m_EndScoreID, false);
                    ECS.SetActive(m_EndContinueID, false);
                    m_CallOnce = false;
                }

                m_Timer += dt;
                ECS.GetTransformECS(entityID, ref m_pos, ref m_rot, ref m_scale);
                
                if(m_Timer < 0.5f)
                {
                    ECS.SetPosition(entityID, Vector3.Lerp(m_pos, m_LettersMap["EndMenuControllerMid"].transform.Position, m_AnimationSpeed * dt));
                }
                else if(m_Timer >= 0.5f && m_Timer < 1.0f)
                {
                    ECS.SetPosition(entityID, Vector3.Lerp(m_pos, m_LettersMap["EndMenuControllerEnd"].transform.Position, m_AnimationSpeed * dt));
                }
                else if(m_Timer >= 1.0f && m_Timer < 2.0f)
                {
                    UpdateDestroyGrade(dt);
                }
                else if(m_Timer >= 2.0f && m_Timer < 3.0f)
                {
                    UpdateEvasiveGrade(dt);
                }
                else if(m_Timer >= 3.0f && m_Timer < 4.0f)
                {
                    UpdateCollectGrade(dt);
                }
                else if(m_Timer >= 4.0f && m_Timer < 5.0f)
                {
                    ECS.SetActive(m_EndScoreID, true);
                    GameUtilities.UpdateScore(m_EndScoreID, PlayerScript.score);
                }
                else if(m_Timer >= 5.0f && m_Timer < 6.0f)
                {
                    UpdateMedals();
                }
                else
                {
                    UpdateContinue(dt);
                }
            }
        }

        public override void OnTriggerEnter(uint id)
        {
        }

        public override void OnTriggerExit(uint id)
        {
        }

        private void UpdateDestroyGrade(float dt)
        {
            if (m_CallOnceDes)
            {
                // Grade C for Destruction
                if (PlayerScript.m_EnemyDestroyedCount >= (uint)GRADE_DES.C && PlayerScript.m_EnemyDestroyedCount < (uint)GRADE_DES.B)
                {
                    ECS.PlayAudio(entityID, 4, "SFX");
                    SwapGrade(m_LettersMap["Destruction_C"].id, m_LettersMap["Destruction_B"].id, m_LettersMap["Destruction_A"].id);
                    m_OverallGrade += (uint)MEDALGRADE.BRONZE;
                }
                // Grade B for Destruction
                else if (PlayerScript.m_EnemyDestroyedCount >= (uint)GRADE_DES.B && PlayerScript.m_EnemyDestroyedCount < (uint)GRADE_DES.A)
                {
                    ECS.PlayAudio(entityID, 3, "SFX");
                    SwapGrade(m_LettersMap["Destruction_B"].id, m_LettersMap["Destruction_C"].id, m_LettersMap["Destruction_A"].id);
                    m_OverallGrade += (uint)MEDALGRADE.SILVER;
                }
                // Grade A for Destruction
                else
                {
                    ECS.PlayAudio(entityID, 2, "SFX");
                    SwapGrade(m_LettersMap["Destruction_A"].id, m_LettersMap["Destruction_B"].id, m_LettersMap["Destruction_C"].id);
                    m_OverallGrade += (uint)MEDALGRADE.GOLD;
                }
                m_CallOnceDes = false;
            }
            else
            {
                ECS.GetTransformECS(m_DestructParentID,ref m_pos,ref m_rot,ref m_scale);
                ECS.SetScale(m_DestructParentID, Vector3.Lerp(m_scale, m_ScaleAnimation[m_DestructParentID], m_AnimationLetterSpeed * dt));
            }
        }

        private void UpdateEvasiveGrade(float dt)
        {
            if (m_CallOnceEva)
            {
                // Grade C for Evasiveness
                if (PlayerScript.m_PlayerHitCount >= (uint)GRADE_EVA.C)
                {
                    ECS.PlayAudio(entityID, 4, "SFX");
                    SwapGrade(m_LettersMap["Evasiveness_C"].id, m_LettersMap["Evasiveness_B"].id, m_LettersMap["Evasiveness_A"].id);
                    m_OverallGrade += (uint)MEDALGRADE.BRONZE;
                }
                // Grade B for Evasiveness
                else if (PlayerScript.m_PlayerHitCount >= (uint)GRADE_EVA.B && PlayerScript.m_PlayerHitCount < (uint)GRADE_EVA.C)
                {
                    ECS.PlayAudio(entityID, 3, "SFX");
                    SwapGrade(m_LettersMap["Evasiveness_B"].id, m_LettersMap["Evasiveness_C"].id, m_LettersMap["Evasiveness_A"].id);
                    m_OverallGrade += (uint)MEDALGRADE.SILVER;
                }
                // Grade A for Evasiveness
                else
                {
                    ECS.PlayAudio(entityID, 2, "SFX");
                    SwapGrade(m_LettersMap["Evasiveness_A"].id, m_LettersMap["Evasiveness_B"].id, m_LettersMap["Evasiveness_C"].id);
                    m_OverallGrade += (uint)MEDALGRADE.GOLD;
                }
                m_CallOnceEva = false;
            }
            else
            {
                ECS.GetTransformECS(m_EvasiveParentID, ref m_pos, ref m_rot, ref m_scale);
                ECS.SetScale(m_EvasiveParentID, Vector3.Lerp(m_scale, m_ScaleAnimation[m_EvasiveParentID], m_AnimationLetterSpeed * dt));
            }
        }
        private void UpdateCollectGrade(float dt)
        {
            if(m_CallOnceCol)
            {
                // Grade C for Collectibles
                if (PlayerScript.m_CollectiblesCount >= (uint)GRADE_COL.C && PlayerScript.m_CollectiblesCount < (uint)GRADE_COL.B)
                {
                    ECS.PlayAudio(entityID, 4, "SFX");
                    SwapGrade(m_LettersMap["Collectibles_C"].id, m_LettersMap["Collectibles_B"].id, m_LettersMap["Collectibles_A"].id);
                    m_OverallGrade += (uint)MEDALGRADE.BRONZE;
                }
                // Grade B for Collectibles
                else if (PlayerScript.m_CollectiblesCount >= (uint)GRADE_COL.B && PlayerScript.m_CollectiblesCount < (uint)GRADE_COL.A)
                {
                    ECS.PlayAudio(entityID, 3, "SFX");
                    SwapGrade(m_LettersMap["Collectibles_B"].id, m_LettersMap["Collectibles_C"].id, m_LettersMap["Collectibles_A"].id);
                    m_OverallGrade += (uint)MEDALGRADE.SILVER;
                }
                // Grade A for Collectibles
                else
                {
                    ECS.PlayAudio(entityID, 2, "SFX");
                    SwapGrade(m_LettersMap["Collectibles_A"].id, m_LettersMap["Collectibles_B"].id, m_LettersMap["Collectibles_C"].id);
                    m_OverallGrade += (uint)MEDALGRADE.GOLD;
                }
                m_CallOnceCol = false;
            }
            else
            {
                ECS.GetTransformECS(m_CollectParentID, ref m_pos, ref m_rot, ref m_scale);
                ECS.SetScale(m_CollectParentID, Vector3.Lerp(m_scale, m_ScaleAnimation[m_CollectParentID], m_AnimationLetterSpeed * dt));
            }
        }
        private void UpdateMedals()
        {
            if(m_CallOnceMedal)
            {
                if (m_OverallGrade >= (uint)MEDALGRADE.OVERALLBRONZE && m_OverallGrade < (uint)MEDALGRADE.OVERALLSILVER)
                {
                    ECS.PlayAudio(entityID, 4, "SFX");
                    SwapGrade(m_LettersMap["Medal_Bronze"].id, m_LettersMap["Medal_Silver"].id, m_LettersMap["Medal_Gold"].id);
                }
                else if(m_OverallGrade >= (uint)MEDALGRADE.OVERALLSILVER && m_OverallGrade < (uint)MEDALGRADE.OVERALLGOLD)
                {
                    ECS.PlayAudio(entityID, 3, "SFX");
                    SwapGrade(m_LettersMap["Medal_Silver"].id, m_LettersMap["Medal_Bronze"].id, m_LettersMap["Medal_Gold"].id);
                }
                else
                {
                    ECS.PlayAudio(entityID, 2, "SFX");
                    SwapGrade(m_LettersMap["Medal_Gold"].id, m_LettersMap["Medal_Silver"].id, m_LettersMap["Medal_Bronze"].id);
                }
                m_CallOnceMedal = false;
            }
        }

        private void UpdateContinue(float dt)
        {
            if (m_CallOnceCont)
            {
                ECS.PlayAudio(entityID, 5, "SFX");
                ECS.SetActive(m_EndContinueID, true);
                m_CallOnceCont = false;
            }
            else
            {
                if(InputUtility.onAnyKey())
                {
                    ECS.PlayAudio(entityID, 1, "SFX");
                    GameUtilities.ResumeScene();
                    GameUtilities.LoadScene("Level01_Boss");
                }

                ECS.GetTransformECS(m_EndContinueID, ref m_pos, ref m_rot, ref m_scale);

                if (m_scale.X >= m_contScaleBig.X - 0.01f)
                    m_EnablePlayBig = false;

                if (m_scale.X <= m_contScaleSmall.X + 0.01f)
                    m_EnablePlayBig = true;

                if (m_EnablePlayBig)
                {
                    ECS.SetScale(m_EndContinueID, Vector3.Lerp(m_scale, m_contScaleBig, (m_AnimationLetterSpeed - 4.0f) * dt));
                }
                else
                {
                    ECS.SetScale(m_EndContinueID, Vector3.Lerp(m_scale, m_contScaleSmall, (m_AnimationLetterSpeed - 4.0f) * dt));
                }
            }
        }

        private void SwapGrade(uint activeID, uint inactiveID1, uint inactiveID2)
        {
            ECS.SetActive(activeID, true);
            ECS.SetActive(inactiveID1, false);
            ECS.SetActive(inactiveID2, false);
        }

        private void DisableGrades(uint inactiveID1, uint inactiveID2, uint inactiveID3)
        {
            ECS.SetActive(inactiveID1, false);
            ECS.SetActive(inactiveID2, false);
            ECS.SetActive(inactiveID3, false);
        }
    }
}
