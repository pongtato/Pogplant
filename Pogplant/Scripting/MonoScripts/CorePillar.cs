using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class CorePillar : MonoBehaviour
    {
        uint m_Explosion;
        uint m_EmissiveEntity;
        uint m_null_entity;
        bool m_Destroyed = false;

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
            m_null_entity = ECS.GetNull();
            FindAndCheck(ref m_Explosion, "ExplosionEffect", true);
            FindAndCheck(ref m_EmissiveEntity, "Energy_Capsule_Emissive", true);
        }

        public override void Start()
        {
        }

        public override void Update(float dt)
        {
        }

        public override void LateUpdate(float dt)
        {
        }

        public override void OnTriggerEnter(uint id)
        {
            if (!m_Destroyed)
            {
                m_Destroyed = true;
                //ECS.SetGlobalPosition(m_Explosion, ECS.GetGlobalPosition(id));
                ECS.SetActive(m_Explosion, true);
                Vector3 noEmi = new Vector3(0);
                ECS.SetActive(m_EmissiveEntity, false);
            }
        }

        public override void OnTriggerExit(uint id)
        {
        }
    }
}
