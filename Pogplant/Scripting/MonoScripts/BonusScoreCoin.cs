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

        public BonusScoreCoin()
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
            PlayerScript.AddScore(true, true, m_BonusScoreAmt);
            ECS.DestroyEntity(entityID);
        }

        public override void OnTriggerExit(uint id)
        {
        }
    }
}
