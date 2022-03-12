using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class BonusItem : MonoBehaviour
    {
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
    }
}
