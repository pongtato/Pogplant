using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class BonusItem : MonoBehaviour
    {
        private Transform m_Trans;
        private float m_rotationSpeed = 100.0f;

        public BonusItem()
        {
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
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
        }

        public override void OnTriggerEnter(uint id)
        {
            ++PlayerScript.m_BonusItem;
            ++PlayerScript.m_CollectiblesCount;
            //PlayerScript.m_ActivateBobble = true;
            ECS.DestroyEntity(entityID);
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
