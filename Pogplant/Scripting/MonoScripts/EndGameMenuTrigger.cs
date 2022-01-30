using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class EndGameMenuTrigger : MonoBehaviour
    {
        uint m_PlayerID;
        uint m_EndMenuControllerID;
        // End Game Menu
        static public bool m_EnableEndGameMenu = false;

        public EndGameMenuTrigger()
        {
        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            m_PlayerID = ECS.FindEntityWithName("PlayerShip");
            m_EndMenuControllerID = ECS.FindEntityWithName("EndGameMenuController");
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
            // On Player Collision
            if (id == m_PlayerID)
            {
                Console.WriteLine("Level Clear");
                m_EnableEndGameMenu = true;
                ECS.SetActive(m_EndMenuControllerID, true);
                GameUtilities.PauseScene();
            }
        }
        public override void OnTriggerExit(uint id)
        {

        }
    }
}
