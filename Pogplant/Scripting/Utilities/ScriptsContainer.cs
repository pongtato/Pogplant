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
            scriptNames.Add("BaseTurret");
            scriptNames.Add("FollowSpline");
            scriptNames.Add("EnemyManager");
            scriptNames.Add("BaseEnemy");
            scriptNames.Add("EncounterManager");
            scriptNames.Add("MoveStuff");
            scriptNames.Add("FirstPersonFiringSystem");
            scriptNames.Add("DummyScript");
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
