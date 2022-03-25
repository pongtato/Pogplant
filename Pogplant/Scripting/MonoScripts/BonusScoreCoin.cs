using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class BonusScoreCoin : MonoBehaviour
    {
        uint m_BonusScoreAmt = 1000;
        bool m_isShrink = false;
        private Transform m_Trans;
        private float m_rotationSpeed = 100.0f;
        private float m_coinMoveSpeed = 0.5f;
        private float m_Timer;
        private float m_ScaleEndTime = 0.5f;
        uint m_playerID;
        private Transform m_playerTrans;
        public BonusScoreCoin()
        {
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            ECS.GetTransformECS(entityID, ref m_Trans.Position, ref m_Trans.Rotation, ref m_Trans.Scale);
            m_playerID = ECS.FindEntityWithName("PlayerShip");
            m_Timer = m_ScaleEndTime;
        }

        public override void Start()
        {
        }

        public override void Update(float dt)
        {
        }

        public override void LateUpdate(float dt)
        {
            ECS.GetTransformECS(entityID, ref m_Trans.Position, ref m_Trans.Rotation, ref m_Trans.Scale);
            // Spin
            ECS.SetRotation(entityID, new Vector3(m_Trans.Rotation.X, clampRotation(m_Trans.Rotation.Y + m_rotationSpeed * dt), m_Trans.Rotation.Z));

            // Shrink when collected
            if (!m_isShrink)
                return;

            m_Timer -= dt;

            // Move position
            ECS.GetTransformECS(m_playerID, ref m_playerTrans.Position, ref m_playerTrans.Rotation, ref m_playerTrans.Scale);
            ECS.SetPosition(entityID, m_Trans.Position + (m_playerTrans.Position - m_Trans.Position) * dt * m_coinMoveSpeed);

            // Scale down
            ECS.SetScale(entityID, m_Trans.Scale * (m_Timer / m_ScaleEndTime));
            
            // End timer
            if(m_Timer <= 0.0f)
            {
                PlayerScript.AddScore(true, true, m_BonusScoreAmt);
                ECS.DestroyEntity(entityID);
            }
        }

        public override void OnTriggerEnter(uint id)
        {
            m_isShrink = true;
        }

        public override void OnTriggerExit(uint id)
        {
        }

        float clampRotation(float angle)
        {
            if (angle >= 360.0f)
                return angle - 360.0f;
            else
                return angle;
        }
    }
}
