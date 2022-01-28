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
        private Dictionary<string, GameObject> m_LettersMap = new Dictionary<string, GameObject>();
        float m_Timer = 0.0f;
        float m_AnimationSpeed = 5.0f;

        Vector3 m_pos = new Vector3();
        Vector3 m_rot = new Vector3();
        Vector3 m_scale = new Vector3();

        private enum GRADE
        {
            A = 20,
            B = 10,
            C = 0,
            COUNT
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();

            // End Score Text ID
            m_EndScoreID = ECS.FindEntityWithName("EndScoreText");

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

            ECS.GetTransformECS(entityID, ref pos, ref rot, ref scale);
            m_LettersMap.Add("EndMenuControllerStart", new GameObject(entityID, new Transform(pos, rot, scale), "EndMenuController"));
            m_LettersMap.Add("EndMenuControllerMid", new GameObject(entityID, new Transform(new Vector3(pos.X, pos.Y + 2.0f, pos.Z), rot, scale), "EndMenuController"));
            m_LettersMap.Add("EndMenuControllerEnd", new GameObject(entityID, new Transform(new Vector3(pos.X, pos.Y + 1.7f, pos.Z), rot, scale), "EndMenuController"));

            ECS.SetActive(entityID, false);
        }

        public override void Start()
        {
        }

        public override void Update(float dt)
        {
            if(EndGameMenuTrigger.m_EnableEndGameMenu)
            {
                m_Timer += dt;
                ECS.GetTransformECS(entityID, ref m_pos, ref m_rot, ref m_scale);
                
                if(m_Timer < 1.0f)
                {
                    ECS.SetPosition(entityID, Vector3.Lerp(m_pos, m_LettersMap["EndMenuControllerMid"].transform.Position, m_AnimationSpeed * dt));
                }
                else if(m_Timer >= 1.0f && m_Timer < 2.0f)
                {
                    ECS.SetPosition(entityID, Vector3.Lerp(m_pos, m_LettersMap["EndMenuControllerEnd"].transform.Position, m_AnimationSpeed * dt));
                }
                else if(m_Timer >= 2.0f && m_Timer < 3.0f)
                {
                    UpdateDestroyGrade();
                }
                else if(m_Timer >= 3.0f && m_Timer < 4.0f)
                {
                    UpdateEvasiveGrade();
                }
                else if(m_Timer >= 4.0f && m_Timer < 5.0f)
                {
                    UpdateCollectGrade();
                }
                else
                {
                    GameUtilities.UpdateScore(m_EndScoreID, EnemyManager.score);
                }
            }
        }

        public override void OnTriggerEnter(uint id)
        {
        }

        public override void OnTriggerExit(uint id)
        {
        }

        private void UpdateDestroyGrade()
        {
            // Grade C for Destruction
            if (PlayerScript.m_EnemyDestroyedCount >= (uint)GRADE.C && PlayerScript.m_EnemyDestroyedCount < (uint)GRADE.B)
            {
                SwapGrade(m_LettersMap["Destruction_C"].id, m_LettersMap["Destruction_B"].id, m_LettersMap["Destruction_A"].id);
            }
            // Grade B for Destruction
            else if (PlayerScript.m_EnemyDestroyedCount >= (uint)GRADE.B && PlayerScript.m_EnemyDestroyedCount < (uint)GRADE.A)
            {
                SwapGrade(m_LettersMap["Destruction_B"].id, m_LettersMap["Destruction_C"].id, m_LettersMap["Destruction_A"].id);
            }
            // Grade A for Destruction
            else
            {
                SwapGrade(m_LettersMap["Destruction_A"].id, m_LettersMap["Destruction_B"].id, m_LettersMap["Destruction_C"].id);
            }
        }

        private void UpdateEvasiveGrade()
        {
            // Grade C for Evasiveness
            if (PlayerScript.m_PlayerHitCount >= (uint)GRADE.C && PlayerScript.m_PlayerHitCount < (uint)GRADE.B)
            {
                SwapGrade(m_LettersMap["Evasiveness_C"].id, m_LettersMap["Evasiveness_B"].id, m_LettersMap["Evasiveness_A"].id);
            }
            // Grade B for Evasiveness
            else if (PlayerScript.m_PlayerHitCount >= (uint)GRADE.B && PlayerScript.m_PlayerHitCount < (uint)GRADE.A)
            {
                SwapGrade(m_LettersMap["Evasiveness_B"].id, m_LettersMap["Evasiveness_C"].id, m_LettersMap["Evasiveness_A"].id);
            }
            // Grade A for Evasiveness
            else
            {
                SwapGrade(m_LettersMap["Evasiveness_A"].id, m_LettersMap["Evasiveness_B"].id, m_LettersMap["Evasiveness_C"].id);
            }
        }
        private void UpdateCollectGrade()
        {
            // Grade C for Collectibles
            if (PlayerScript.m_CollectiblesCount >= (uint)GRADE.C && PlayerScript.m_CollectiblesCount < (uint)GRADE.B)
            {
                SwapGrade(m_LettersMap["Collectibles_C"].id, m_LettersMap["Collectibles_B"].id, m_LettersMap["Collectibles_A"].id);
            }
            // Grade B for Collectibles
            else if (PlayerScript.m_CollectiblesCount >= (uint)GRADE.B && PlayerScript.m_CollectiblesCount < (uint)GRADE.A)
            {
                SwapGrade(m_LettersMap["Collectibles_B"].id, m_LettersMap["Collectibles_C"].id, m_LettersMap["Collectibles_A"].id);
            }
            // Grade A for Collectibles
            else
            {
                SwapGrade(m_LettersMap["Collectibles_A"].id, m_LettersMap["Collectibles_B"].id, m_LettersMap["Collectibles_C"].id);
            }
        }

        private void SwapGrade(uint activeID, uint inactiveID1, uint inactiveID2)
        {
            ECS.SetActive(activeID, true);
            ECS.SetActive(inactiveID1, false);
            ECS.SetActive(inactiveID2, false);
        }
    }
}
