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
            
        }

        uint Player;
        uint Game_Over_Screen_ID;

        public override void Start()
        {
            Player = ECS.FindEntityWithName("PlayerShip");
            Game_Over_Screen_ID = ECS.FindEntityWithName("Game Over Menu");
        }

        public override void Init(ref uint _entityID)
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
            if (id == Player)
            {
                
                GameUtilities.LoadScene("MainMenu"); //<- Uncomment this line to load the game over screen
                //Console.WriteLine("Player hit game over box");
            }
        }

        public override void OnTriggerExit(uint id)
        {

        }
    }
}
