using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Scripting
{
    public class ScriptsContainer
    {
        static public List<string> scriptNames = new List<string>();

        public ScriptsContainer()
        {
            scriptNames.Add("PlayerScript");
            scriptNames.Add("FirstPersonFiringSystem");
            scriptNames.Add("EncounterSystemDriver");
            scriptNames.Add("FollowSpline");
            scriptNames.Add("EncounterManager");
            scriptNames.Add("EnemyManager");
            scriptNames.Add("BaseEnemy");
            scriptNames.Add("BaseTurret");
            scriptNames.Add("BaseGattling");
            scriptNames.Add("TurretController");
            scriptNames.Add("MissileController");
            scriptNames.Add("Missile");
            scriptNames.Add("FlockController");
            scriptNames.Add("EnvDamage");
            scriptNames.Add("MainMenuController");
            scriptNames.Add("DummyScript");
            scriptNames.Add("GameOverScreen");
            scriptNames.Add("GameEndingTrigger");
        }

        public string GetScriptNameElement(int index)
        {
            return scriptNames[index];
        }

        public int GetScriptNamesSize()
        {
            return scriptNames.Count;
        }
    }
}
