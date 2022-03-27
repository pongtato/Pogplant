using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class ExplosionEffect : MonoBehaviour
    {
        Random m_Rand = new Random();
        Vector3 m_ShockwaveScale = new Vector3(1);
        float m_Timer = 0;
        float m_LerpDuration = 0;
        float m_LerpTimer = 0;
        float m_PersistDuration = 0;
        float m_PersistTimer = 0;
        float m_GeneratedSize = 0;
        float m_Spin = 0;
        float m_SpinSpeed = 0;
        float m_ShockwaveExtraScale = 0;
        float m_ScaleSpeed = 0;
        float m_DelayRand = 0;
        int m_DelayMin = 0;
        int m_DelayMax = 0;
        int m_TargetSizeMin = 0;
        int m_TargetSizeMax = 0;
        uint m_Shockwave;
        uint m_SmokeEffect;
        uint m_null_entity;
        bool m_Done = false;
        bool m_Repeat = true;
        bool m_Playing = false;

        void FindAndCheck(ref uint destination, string entity_name, bool is_child)
        {
            uint result = is_child ? ECS.FindChildEntityWithName(entityID, entity_name) : ECS.FindEntityWithName(entity_name);
            if (result == m_null_entity)
                Console.WriteLine(entity_name + " is null entity");
            else
                destination = result;
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            m_LerpDuration =  ECS.GetValue<float>(entityID, 1.0f, "LerpDuration");
            m_PersistDuration = ECS.GetValue<float>(entityID, 1.0f, "PersistDuration");
            m_SpinSpeed = ECS.GetValue<float>(entityID, 1.0f, "SpinSpeed");
            m_ScaleSpeed = ECS.GetValue<float>(entityID, 1.0f, "ScaleSpeed");
            m_DelayMin = ECS.GetValue<int>(entityID, 1, "DelayMin");
            m_DelayMax = ECS.GetValue<int>(entityID, 1, "DelayMax");
            m_TargetSizeMin = ECS.GetValue<int>(entityID, 1, "TargetSizeMin");
            m_TargetSizeMax = ECS.GetValue<int>(entityID, 1, "TargetSizeMax");
            m_Repeat = ECS.GetValue<bool>(entityID, true, "Repeat");
            m_null_entity = ECS.GetNull();

            FindAndCheck(ref m_Shockwave, "Shockwave", true);
            FindAndCheck(ref m_SmokeEffect, "SmokeEffect", true);
            //m_ShockwaveScale = ECS.GetGlobalScale(m_Shockwave);
        }

        public override void Start()
        {
            m_GeneratedSize = m_Rand.Next(m_TargetSizeMin, m_TargetSizeMax);
            m_DelayRand = (float)m_Rand.Next(m_DelayMin, m_DelayMax);
        }

        public override void Update(float dt)
        {
        }

        public override void LateUpdate(float dt)
        {
            // Waiting for next interval to play
            if (m_Timer < m_DelayRand)
            {
                m_Timer += dt;
            }
            // Animation playing
            else
            {
                if(m_Done && !m_Repeat)
                {
                    return;
                }

                if (!m_Playing)
                {
                    m_Done = false;
                    m_Playing = true;
                    ECS.SetParticlePlay(entityID, true);
                    ECS.SetParticlePlay(m_SmokeEffect, true);
                }

                // Lerp
                m_LerpTimer += dt;
                float ratio = m_LerpTimer / m_LerpDuration;
                float curve = (float)Math.Sin(ratio * Math.PI * 0.5f);

                // When finish lerping determine next size & reset timer for delay
                if (ratio >= 1)
                {
                    if (m_PersistTimer < m_PersistDuration)
                    {
                        m_PersistTimer += dt;
                    }
                    else
                    {
                        m_GeneratedSize = m_Rand.Next(m_TargetSizeMin, m_TargetSizeMax);
                        m_DelayRand = (float)m_Rand.Next(m_DelayMin, m_DelayMax);
                        m_Timer = 0;
                        m_LerpTimer = 0;
                        m_PersistTimer = 0;
                        m_ShockwaveExtraScale = 0;
                        // Set random rot so its not so obvious
                        m_Spin = m_Rand.Next(0, 360);
                        ECS.SetScale(entityID, new Vector3(0));
                        m_Playing = false;
                        m_Done = true;
                    }
                }
                else
                {
                    // Ball
                    Vector3 sizeCalc = new Vector3(m_GeneratedSize, m_GeneratedSize, m_GeneratedSize) * curve;
                    ECS.SetScale(entityID, sizeCalc);
                }

                // Shockwave rotation - Updates regardless
                m_Spin += dt * m_SpinSpeed;
                m_ShockwaveExtraScale += dt * m_ScaleSpeed; 
                if (m_Spin >= 360.0f)
                {
                    m_Spin -= 360.0f;
                }
                ECS.SetRotation(m_Shockwave, new Vector3(0, m_Spin, 0));
                ECS.SetScale(m_Shockwave, m_ShockwaveScale + new Vector3(m_ShockwaveExtraScale, m_ShockwaveExtraScale, m_ShockwaveExtraScale));
            }
        }

        public override void OnTriggerEnter(uint id)
        {
        }

        public override void OnTriggerExit(uint id)
        {
        }
    }
}
