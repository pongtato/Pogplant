using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class GameEndingTrigger : MonoBehaviour
    {
        public GameEndingTrigger() 
        {
            Player = ECS.FindEntityWithName("PlayerShip");
            Game_Over_Screen_ID = ECS.FindEntityWithName("Game Over Menu");
        }

        uint Player;
        uint Game_Over_Screen_ID;

        public override void Start()
        {

        }

        public override void Init(ref uint _entityID)
        {
            Console.WriteLine("Game Over ID" + Game_Over_Screen_ID);
        }

        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

        }

        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

        }

        public override void OnTriggerEnter(uint id)
        {
            if (id == Player)
            {
                //ECS.GetComponent<GameOverScreen>(Game_Over_Screen_ID).ShowGameOverScreen();
                Console.WriteLine("PLayer hit game over box");
            }
        }

        public override void OnTriggerExit(uint id)
        {

        }
    }
}
