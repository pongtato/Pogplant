using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class L1BossShield : MonoBehaviour
    {
        /// <summary>
        /// Sequence of how the boss shield works:
        /// 1. Player damages the shield while fending off enemies
        /// 2. When shield is depleted, play/instantiate object with particle
        /// 3. When the particle hits the player (via estimated time), the black screen appears and plays audio
        /// 4. After another delay, transit to another scene.
        /// </summary>

       

        public L1BossShield()
        {

        }

        public override void Start()
        {
           
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        public override void Update(float dt)
        {
        }

        public override void LateUpdate(float dt)
        {

        }

        public override void OnTriggerEnter(uint id)
        {

        }

        public override void OnTriggerExit(uint id)
        {

        }
    }
}
