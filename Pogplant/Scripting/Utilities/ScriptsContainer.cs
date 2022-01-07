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
        static public List<string> monoScriptNames = new List<string>();
        static public List<string> pauseScriptNames = new List<string>();

        public ScriptsContainer()
        {
            AddMonoScripts();
            AddPauseScripts();
        }

        private void AddMonoScripts()
        {
            // For Mono Scripts
            monoScriptNames.Add("PlayerScript");
            monoScriptNames.Add("FirstPersonFiringSystem");
            monoScriptNames.Add("EncounterSystemDriver");
            monoScriptNames.Add("FollowSpline");
            monoScriptNames.Add("L1BossShield");
            //monoScriptNames.Add("EncounterManager");
            //monoScriptNames.Add("EnemyManager");
            //monoScriptNames.Add("BaseEnemy");
            monoScriptNames.Add("BaseTurret");
            monoScriptNames.Add("BaseFlock");
            monoScriptNames.Add("BaseGattling");
            monoScriptNames.Add("TurretController");
            monoScriptNames.Add("MissileController");
            //monoScriptNames.Add("Missile");
            monoScriptNames.Add("FlockController");
            //monoScriptNames.Add("EnvDamage");
            monoScriptNames.Add("MainMenuController");
            monoScriptNames.Add("DummyScript");
            monoScriptNames.Add("GameOverScreen");
            monoScriptNames.Add("GameEndingTrigger");
            monoScriptNames.Add("DashboardScreen");
        }
        
        private void AddPauseScripts()
        {
            // For Pause State Scripts
            pauseScriptNames.Add("TestPauseUpdate");
        }

        public string GetScriptNameElement(int index)
        {
            return monoScriptNames[index];
        }

        public int GetScriptNamesSize()
        {
            return monoScriptNames.Count;
        }

        public string GetPScriptNameElement(int index)
        {
            return pauseScriptNames[index];
        }

        public int GetPScriptNamesSize()
        {
            return pauseScriptNames.Count;
        }
    }
}
