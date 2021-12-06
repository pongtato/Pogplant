using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class EnvDamage : MonoBehaviour
    {
        uint Player;
        float damage = 10.0f;
        public EnvDamage()
        {
        }

        public override void Init(ref uint _entityID)
        {
            Player = ECS.FindEntityWithName("PlayerShip");
        }

        public override void Start()
        {
        }

        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
        }

        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
        }

        public override void OnTriggerEnter(uint id)
        {
            // On Player Collision
            if (id == Player)
            {
                Console.WriteLine("Take Env Dmg");
                GameUtilities.PlayerTakeDamage(Player, damage);
            }
        }
        public override void OnTriggerExit(uint id)
        {

        }
    }
}
