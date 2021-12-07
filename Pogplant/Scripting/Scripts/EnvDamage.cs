//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Scripting
//{
//    public class EnvDamage : MonoBehaviour
//    {
//        uint Player;
//        float damage = 20.0f;
//        uint DashboardScreenID;

//        public EnvDamage()
//        {
//        }

//        public override void Init(ref uint _entityID)
//        {
//            Player = ECS.FindEntityWithName("PlayerShip");
//            DashboardScreenID = ECS.FindEntityWithName("DashboardScreenFace");
//        }

//        public override void Start()
//        {
//        }

//        public override void Update(float dt)
//        {
//        }

//        public override void LateUpdate(float dt)
//        {
//        }

//        public override void OnTriggerEnter(uint id)
//        {
//            // On Player Collision
//            if (id == Player)
//            {
//                Console.WriteLine("Take Env Dmg");
//                GameUtilities.PlayerTakeDamage(Player, damage, DashboardScreenID, 2);
//            }
//        }
//        public override void OnTriggerExit(uint id)
//        {

//        }
//    }
//}
