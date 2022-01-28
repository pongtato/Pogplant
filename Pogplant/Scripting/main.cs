/*****************************************************************************/
/*!
\file	main.cs
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	Main class for scripting c# project

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    // My main class for scripting
    public class Scripting
    {
        // Add Script Types here
        public static ScriptsContainer ScriptsContainer()
        {
            return new ScriptsContainer();
        }

        public static MainMenuController MainMenuController()
        {
            return new MainMenuController();
        }

        public static GameOverScreen GameOverScreen()
        {
            return new GameOverScreen();
        }

        public static GameEndingTrigger GameEndingTrigger()
        {
            return new GameEndingTrigger();
        }

        public static DashboardScreen DashboardScreen()
        {
            return new DashboardScreen();
        }

        public static LaserWeapon LaserWeapon()
        {
            return new LaserWeapon();
        }
        public static PlayerScript PlayerScript()
        {
            return new PlayerScript();
        }

        public static BaseTurret BaseTurret()
        {
            return new BaseTurret();
        }

        public static FollowSpline FollowSpline()
        {
            return new FollowSpline();
        }

        //public static EnemyManager EnemyManager()
        //{
        //    return new EnemyManager();
        //}

        //public static BaseEnemy BaseEnemy()
        //{
        //    return new BaseEnemy();
        //}

        //public static EncounterManager EncounterManager()
        //{
        //    return new EncounterManager();
        //}

        public static EncounterSystemDriver EncounterSystemDriver()
        {
            return new EncounterSystemDriver();
        }

        public static FlockController FlockController()
        {
            return new FlockController();
        }

        public static FirstPersonFiringSystem FirstPersonFiringSystem()
        {
            return new FirstPersonFiringSystem();
        }

        //public static EnvDamage EnvDamage()
        //{
        //    return new EnvDamage();
        //}

        //public static Missile Missile()
        //{
        //    return new Missile();
        //}

        public static MissileController MissileController()
        {
            return new MissileController();
        }

        public static TurretController TurretController()
        {
            return new TurretController();
        }

        public static BaseGattling BaseGattling()
        {
            return new BaseGattling();
        }

        public static BaseFlock BaseFlock()
        {
            return new BaseFlock();
        }

        public static Laser Laser()
        {
            return new Laser();
        }

        public static LaserTriggerController LaserTriggerController()
        {
            return new LaserTriggerController();
        }

        public static L1BossShield L1BossShield()
        {
            return new L1BossShield();
        }

        public static TestPauseUpdate TestPauseUpdate()
        {
            return new TestPauseUpdate();
        }

        public static BonusItem BonusItem()
        {
            return new BonusItem();
        }

        public static BobbleHeadMenu BobbleHeadMenu()
        {
            return new BobbleHeadMenu();
        }

        public static SettingsMenu SettingsMenu()
        {
            return new SettingsMenu();
        }

        public static PauseSettingsMenu PauseSettingsMenu()
        {
            return new PauseSettingsMenu();
        }

        public static PauseMenu PauseMenu()
        {
            return new PauseMenu();
        }

        public static L1Boss L1Boss()
        {
            return new L1Boss();
        }
        public static EndGameMenu EndGameMenu()
        {
            return new EndGameMenu();
        }

        public static EndGameMenuTrigger EndGameMenuTrigger()
        {
            return new EndGameMenuTrigger();
        }
        
        public static TetherEnemy TetherEnemy()
        {
            return new TetherEnemy();
        }

        public static ExplosiveEnemy ExplosiveEnemy()
        {
            return new ExplosiveEnemy();
        }
    }
}
