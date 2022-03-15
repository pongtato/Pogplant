/******************************************************************************/
/*!
\file	EncounterSystemDriver.cs
\author Ng Tian Kiat
\par	email: tiankiat.ng\@digipen.edu
\details
    This script serves as a API test for the EncounterSystem script.
    It provides an example of API usage of the EncounterSystem to create
    encounters, in the game.

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
            or disclosure of this file or its contents without the prior
            written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class EncounterSystemDriver : MonoBehaviour
    {
        public static EncounterSystemDriver m_singleton;

        float Enemy1_fire_rate = 5;
        float Enemy2_fire_rate = 24f;
        float Enemy3_fire_rate = 10f;

        float small_enemy_health = 10.0f;
        float big_enemy_health = 50.0f;

        static EncounterManager encounterManager = new EncounterManager();

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            m_singleton = this;
        }

        public override void Start()
        {
            encounterManager.Start();
        }

        // Update is called once per frame
        public override void Update(float dt)
        {
            encounterManager.Update(dt);
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

        //this function will be called by the wave trigger boxes
        public void SpawnWave(string waveName)
        {
            switch (waveName)
            {
                case "SegmentTwo_A":
                    SegmentTwo_A();
                    break;
                case "SegmentTwo_B":
                    SegmentTwo_B();
                    break;
                case "SegmentFour_A":
                    SegmentFour_A();
                    break;
                case "SegmentFive_A":
                    SegmentFive_A();
                    break;
                case "SegmentFive_B":
                    SegmentFive_B();
                    break;
                case "SegmentSix":
                    SegmentSix();
                    break;
                case "SegmentSeven_A":
                    SegmentSeven_A();
                    break;
                case "SegmentSeven_BA":
                    SegmentSeven_BA();
                    break;
                case "SegmentSeven_BB":
                    SegmentSeven_BB();
                    break;
                case "SegmentEight_A":
                    SegmentEight_A();
                    break;
                case "SegmentEight_B":
                    SegmentEight_B();
                    break;
                case "SegmentEight_C":
                    SegmentEight_C();
                    break;
                case "SegmentNine_AA":
                    SegmentNine_AA();
                    break;
                case "SegmentNine_AB":
                    SegmentNine_AB();
                    break;
                case "SegmentNine_AC":
                    SegmentNine_AC();
                    break;
                case "SegmentNine_AD":
                    SegmentNine_AD();
                    break;
                case "SegmentNine_AE":
                    SegmentNine_AE();
                    break;
                case "SegmentNine_BA":
                    SegmentNine_BA();
                    break;
                case "SegmentNine_BB":
                    SegmentNine_BB();
                    break;
                case "Boss_Low1":
                    Boss_Low1();
                    break;
                case "Boss_Low2":
                    Boss_Low2();
                    break;
                case "Boss_Med1":
                    Boss_Med1();
                    break;
                case "Boss_Med2":
                    Boss_Med2();
                    break;
                case "Boss_High1":
                    Boss_High1();
                    break;
                case "Boss_High2":
                    Boss_High2();
                    break;
                case "Level2_SegmentTwo":
                    Level2_SegmentTwo();
                    break;
                case "Level2_SegmentFour":
                    Level2_SegmentFour();
                    break;
                case "Level2_SegmentFive":
                    Level2_SegmentFive();
                    break;
                default:
                    Boss_Low1();
                    Console.WriteLine("Unsupported wave name: \"" + waveName + "\"");
                    break;
            }
        }
        void SegmentTwo_A()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentTwo_A = new Encounter(0.0f, 11.0f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 6.5f, small_enemy_health);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 0.5f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 0.5f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 0.5f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 0.5f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("12,1,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("9,1,5"), 0.8f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 0.8f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("5,1,5"), 0.6f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 0.6f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("2,1,5"), 0.4f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 0.4f);
                enemy4.AddCommand(new CompositeAction(action4));
            }

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("12,1,5"), enemyManager.GetWaypoint("14,1,8"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("9,1,5"), enemyManager.GetWaypoint("12,1,8"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("5,1,5"), enemyManager.GetWaypoint("10,1,8"), 1.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("2,1,5"), enemyManager.GetWaypoint("8,1,8"), 1.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy4.AddCommand(new CompositeAction(action4));
            }

            // Wait and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new WaitAction(1.0f);
                action1[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new WaitAction(1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new WaitAction(1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new WaitAction(1.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy4.AddCommand(new CompositeAction(action4));
            }

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("14,1,8"), enemyManager.GetWaypoint("12,7,2"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("12,1,8"), enemyManager.GetWaypoint("9,7,2"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("10,1,8"), enemyManager.GetWaypoint("5,7,2"), 1.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("8,1,8"), enemyManager.GetWaypoint("2,7,2"), 1.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy4.AddCommand(new CompositeAction(action4));
            }

            // Wait and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new WaitAction(2.0f);
                action1[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new WaitAction(2.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.0f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new WaitAction(2.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.0f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new WaitAction(2.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f);
                enemy4.AddCommand(new CompositeAction(action4));
            }

            //
            // GET SPAWN POINTS
            //
            segmentTwo_A.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_L1_Start"), enemy1, "Enemy_01"));
            segmentTwo_A.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_L1_Start"), enemy2, "Enemy_01"));
            segmentTwo_A.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Near_L1_Start"), enemy3, "Enemy_01"));
            segmentTwo_A.AddSpawnAction(new SpawnAction(1.5f, enemyManager.GetWaypoint("Near_L1_Start"), enemy4, "Enemy_01"));


            encounterManager.AddEncounter(segmentTwo_A);
        }

        void SegmentTwo_B()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter SegmentTwo_B = new Encounter(0.0f, 11.0f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 6.5f, small_enemy_health);

            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 5.0f, small_enemy_health);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 5.0f, small_enemy_health);
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 5.0f, small_enemy_health);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L7_Start"), 5.0f, small_enemy_health);

            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R1_Start"), 4.5f, small_enemy_health);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 4.5f, small_enemy_health);
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 4.5f, small_enemy_health);
            EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R7_Start"), 4.5f, small_enemy_health);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 0.5f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 0.5f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 0.5f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 0.5f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("12,1,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("9,1,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("5,1,5"), 1.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("2,1,5"), 1.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy4.AddCommand(new CompositeAction(action4));
            }

            // Wait and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new WaitAction(1.0f);
                action1[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new WaitAction(1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new WaitAction(1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new WaitAction(1.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy4.AddCommand(new CompositeAction(action4));
            }

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("12,1,5"), enemyManager.GetWaypoint("12,4,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("9,1,5"), enemyManager.GetWaypoint("9,4,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("5,1,5"), enemyManager.GetWaypoint("5,4,5"), 1.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("2,1,5"), enemyManager.GetWaypoint("2,4,5"), 1.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy4.AddCommand(new CompositeAction(action4));
            }

            // Wait and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new WaitAction(3.0f);
                action1[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 3.0f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new WaitAction(3.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new WaitAction(3.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new WaitAction(3.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 3.0f);
                enemy4.AddCommand(new CompositeAction(action4));
            }

            // PART B
            // Move and shoot enemy 5,6,7,8
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("0,1,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy5.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("0,3,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy6.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("0,5,5"), 1.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy7.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_L7_Start"), enemyManager.GetWaypoint("0,7,5"), 1.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy8.AddCommand(new CompositeAction(action4));
            }

            // Wait and shoot enemy 5,6,7,8
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new WaitAction(4.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 4.0f);
                enemy5.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new WaitAction(4.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 4.0f);
                enemy6.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new WaitAction(4.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 4.0f);
                enemy7.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new WaitAction(4.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 4.0f);
                enemy8.AddCommand(new CompositeAction(action4));
            }

            // Move and shoot enemy 9,10,11,12
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_R1_Start"), enemyManager.GetWaypoint("14,1,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy9.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("14,3,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.0f);
                enemy10.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("14,5,5"), 1.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy11.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_R7_Start"), enemyManager.GetWaypoint("14,7,5"), 1.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy12.AddCommand(new CompositeAction(action4));
            }

            // Wait and shoot enemy 9,10,11,12
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new WaitAction(3.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                enemy9.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new WaitAction(3.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                enemy10.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new WaitAction(3.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 3.0f);
                enemy11.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new WaitAction(3.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 3.0f);
                enemy12.AddCommand(new CompositeAction(action4));
            }

            //
            // GET SPAWN POINTS
            //
            SegmentTwo_B.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Near_L1_Start"), enemy1, "Enemy_01"));
            SegmentTwo_B.AddSpawnAction(new SpawnAction(1.2f, enemyManager.GetWaypoint("Near_L1_Start"), enemy2, "Enemy_01"));
            SegmentTwo_B.AddSpawnAction(new SpawnAction(1.4f, enemyManager.GetWaypoint("Near_L1_Start"), enemy3, "Enemy_01"));
            SegmentTwo_B.AddSpawnAction(new SpawnAction(1.6f, enemyManager.GetWaypoint("Near_L1_Start"), enemy4, "Enemy_01"));

            SegmentTwo_B.AddSpawnAction(new SpawnAction(6.2f, enemyManager.GetWaypoint("Near_L1_Start"), enemy5, "Enemy_01"));
            SegmentTwo_B.AddSpawnAction(new SpawnAction(6.4f, enemyManager.GetWaypoint("Near_L3_Start"), enemy6, "Enemy_01"));
            SegmentTwo_B.AddSpawnAction(new SpawnAction(6.6f, enemyManager.GetWaypoint("Near_L5_Start"), enemy7, "Enemy_01"));
            SegmentTwo_B.AddSpawnAction(new SpawnAction(6.8f, enemyManager.GetWaypoint("Near_L7_Start"), enemy8, "Enemy_01"));

            SegmentTwo_B.AddSpawnAction(new SpawnAction(6.2f, enemyManager.GetWaypoint("Near_R1_Start"), enemy9, "Enemy_01"));
            SegmentTwo_B.AddSpawnAction(new SpawnAction(6.4f, enemyManager.GetWaypoint("Near_R3_Start"), enemy10, "Enemy_01"));
            SegmentTwo_B.AddSpawnAction(new SpawnAction(6.6f, enemyManager.GetWaypoint("Near_R5_Start"), enemy11, "Enemy_01"));
            SegmentTwo_B.AddSpawnAction(new SpawnAction(6.8f, enemyManager.GetWaypoint("Near_R7_Start"), enemy12, "Enemy_01"));


            encounterManager.AddEncounter(SegmentTwo_B);
        }

        void SegmentFour_A()
        {
            // Not implemented yet
        }

        void SegmentFive_A()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentFive_A = new Encounter(0.0f, 8.75f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R1_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R1_Start"), 7.5f, small_enemy_health);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("Near_R3_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("Near_R3_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_Start"), enemyManager.GetWaypoint("Near_R1_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_Start"), enemyManager.GetWaypoint("Near_R1_MidM"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("8,5,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("11,5,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_R3_MidM"), enemyManager.GetWaypoint("9,3,5"), 1.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.5f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_R3_MidM"), enemyManager.GetWaypoint("12,3,5"), 1.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.5f);
                enemy4.AddCommand(new CompositeAction(action4));

                BaseAction[] action5 = new BaseAction[2];
                action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("10,1,5"), 1.0f);
                action5[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.5f);
                enemy5.AddCommand(new CompositeAction(action5));

                BaseAction[] action6 = new BaseAction[2];
                action6[0] = new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("13,1,5"), 1.0f);
                action6[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.5f);
                enemy6.AddCommand(new CompositeAction(action6));
            }

            enemy1.AddCommand(new WaitAction(3.0f));
            enemy2.AddCommand(new WaitAction(3.0f));
            enemy3.AddCommand(new WaitAction(3.0f));
            enemy4.AddCommand(new WaitAction(3.0f));
            enemy5.AddCommand(new WaitAction(3.0f));
            enemy6.AddCommand(new WaitAction(3.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("8,5,5"), enemyManager.GetWaypoint("Far_T8_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("11,5,5"), enemyManager.GetWaypoint("Far_T11_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,3,5"), enemyManager.GetWaypoint("Far_T9_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,3,5"), enemyManager.GetWaypoint("Far_T12_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,1,5"), enemyManager.GetWaypoint("Far_T10_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("13,1,5"), enemyManager.GetWaypoint("Far_T13_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentFive_A.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_R5_Start"), enemy1, "Enemy_01"));
            segmentFive_A.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_R5_Start"), enemy2, "Enemy_01"));
            segmentFive_A.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_R3_Start"), enemy3, "Enemy_01"));
            segmentFive_A.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Near_R3_Start"), enemy4, "Enemy_01"));
            segmentFive_A.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Near_R1_Start"), enemy5, "Enemy_01"));
            segmentFive_A.AddSpawnAction(new SpawnAction(1.25f, enemyManager.GetWaypoint("Near_R1_Start"), enemy6, "Enemy_01"));

            encounterManager.AddEncounter(segmentFive_A);
        }

        void SegmentFive_B()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentFive_B = new Encounter(0.0f, 8.25f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L2_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L2_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L0_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L0_Start"), 7.5f, small_enemy_health);

            // Move and shoot
            {
                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("5,2,5"), 1.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy9.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("2,2,5"), 1.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy10.AddCommand(new CompositeAction(action4));

                BaseAction[] action5 = new BaseAction[2];
                action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_L0_Start"), enemyManager.GetWaypoint("4,0,5"), 1.0f);
                action5[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy11.AddCommand(new CompositeAction(action5));

                BaseAction[] action6 = new BaseAction[2];
                action6[0] = new MoveAction(enemyManager.GetWaypoint("Near_L0_Start"), enemyManager.GetWaypoint("1,0,5"), 1.0f);
                action6[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.0f);
                enemy12.AddCommand(new CompositeAction(action6));
            }

            // Move and shoot
            {
                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("5,2,5"), enemyManager.GetWaypoint("6,2,5"), 0.5f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.5f);
                enemy9.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("2,2,5"), enemyManager.GetWaypoint("3,2,5"), 0.5f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.5f);
                enemy10.AddCommand(new CompositeAction(action4));

                BaseAction[] action5 = new BaseAction[2];
                action5[0] = new MoveAction(enemyManager.GetWaypoint("4,0,5"), enemyManager.GetWaypoint("6,0,5"), 0.5f);
                action5[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.5f);
                enemy11.AddCommand(new CompositeAction(action5));

                BaseAction[] action6 = new BaseAction[2];
                action6[0] = new MoveAction(enemyManager.GetWaypoint("1,0,5"), enemyManager.GetWaypoint("3,0,5"), 0.5f);
                action6[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.5f);
                enemy12.AddCommand(new CompositeAction(action6));
            }

            // Move and shoot
            {

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("6,2,5"), enemyManager.GetWaypoint("9,2,5"), 0.5f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f);
                enemy9.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("3,2,5"), enemyManager.GetWaypoint("5,2,5"), 0.5f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f);
                enemy10.AddCommand(new CompositeAction(action4));

                BaseAction[] action5 = new BaseAction[2];
                action5[0] = new MoveAction(enemyManager.GetWaypoint("6,0,5"), enemyManager.GetWaypoint("9,0,5"), 0.5f);
                action5[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f);
                enemy11.AddCommand(new CompositeAction(action5));

                BaseAction[] action6 = new BaseAction[2];
                action6[0] = new MoveAction(enemyManager.GetWaypoint("3,0,5"), enemyManager.GetWaypoint("5,0,5"), 0.5f);
                action6[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f);
                enemy12.AddCommand(new CompositeAction(action6));
            }

            // Move and shoot
            {
                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new WaitAction(2.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f);
                enemy9.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new WaitAction(2.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f);
                enemy10.AddCommand(new CompositeAction(action4));

                BaseAction[] action5 = new BaseAction[2];
                action5[0] = new WaitAction(2.0f);
                action5[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f);
                enemy11.AddCommand(new CompositeAction(action5));

                BaseAction[] action6 = new BaseAction[2];
                action6[0] = new WaitAction(2.0f);
                action6[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f);
                enemy12.AddCommand(new CompositeAction(action6));
            }

            //
            // GET SPAWN POINTS
            //
            segmentFive_B.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_L2_Start"), enemy9, "Enemy_01"));
            segmentFive_B.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Near_L2_Start"), enemy10, "Enemy_01"));
            segmentFive_B.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Near_L0_Start"), enemy11, "Enemy_01"));
            segmentFive_B.AddSpawnAction(new SpawnAction(1.25f, enemyManager.GetWaypoint("Near_L0_Start"), enemy12, "Enemy_01"));

            encounterManager.AddEncounter(segmentFive_B);
        }

        void SegmentSix()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentSix = new Encounter(0.0f, 7.0f, enemyManager);

            //
            //Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T7_Start"), 7.0f, big_enemy_health);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_Start"), enemyManager.GetWaypoint("7,0,2"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("7,4,2"), enemyManager.GetWaypoint("7,4,2"), 1.0f);
                action1[1] = new AttackAction("Spiral", Enemy2_fire_rate, 2, 5.0f, 0.2f);
                enemy1.AddCommand(new CompositeAction(action1));
            }

            //
            //GET SPAWN POINTS
            //
            segmentSix.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_T7_Start"), enemy1, "Enemy_02"));

            encounterManager.AddEncounter(segmentSix);
        }

        void SegmentSeven_A()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentSeven_A = new Encounter(0.0f, 6.5f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 6.5f, big_enemy_health);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R6_Start"), 6.5f, big_enemy_health);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("0,1,5"), 0.5f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R6_Start"), enemyManager.GetWaypoint("14,6,5"), 0.5f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,1,5"), enemyManager.GetWaypoint("7,0,2"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("14,6,5"), enemyManager.GetWaypoint("7,7,2"), 1.0f));

            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,7,2"), enemyManager.GetWaypoint("0,6,2"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("7,0,2"), enemyManager.GetWaypoint("14,1,2"), 2.0f);
                action1[1] = new AttackAction("Spiral", Enemy2_fire_rate, 2, 2.0f, 0.2f);
                enemy1.AddCommand(new CompositeAction(action1));
            }

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("14,1,2"), enemyManager.GetWaypoint("14,6,2"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("0,6,2"), enemyManager.GetWaypoint("0,1,2"), 2.0f);
                action1[1] = new AttackAction("Spiral", Enemy2_fire_rate, 2, 2.0f, 0.2f);
                enemy2.AddCommand(new CompositeAction(action1));
            }

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("14,6,2"), enemyManager.GetWaypoint("7,7,2"), 2.0f);
                action1[1] = new AttackAction("Spiral", Enemy2_fire_rate, 2, 2.0f, 0.2f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("0,1,2"), enemyManager.GetWaypoint("7,0,2"), 2.0f);
                action2[1] = new AttackAction("Spiral", Enemy2_fire_rate, 2, 2.0f, 0.2f);
                enemy2.AddCommand(new CompositeAction(action2));
            }

            //
            // GET SPAWN POINTS
            //
            segmentSeven_A.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_L1_Start"), enemy1, "Enemy_02"));
            segmentSeven_A.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_R6_Start"), enemy2, "Enemy_02"));

            encounterManager.AddEncounter(segmentSeven_A);
        }

        void SegmentSeven_BA()
        {

            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentSeven_BA = new Encounter(0.0f, 8.5f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R4_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R2_Start"), 7.5f, small_enemy_health);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R4_Start"), enemyManager.GetWaypoint("Near_R4_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("Near_R3_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_Start"), enemyManager.GetWaypoint("Near_R2_MidM"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("5,5,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("9,5,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_R4_MidM"), enemyManager.GetWaypoint("11,4,5"), 1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_R3_MidM"), enemyManager.GetWaypoint("12,3,5"), 1.0f);
                action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy4.AddCommand(new CompositeAction(action4));

                BaseAction[] action5 = new BaseAction[2];
                action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("13,2,5"), 1.0f);
                action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy5.AddCommand(new CompositeAction(action5));
            }

            enemy1.AddCommand(new WaitAction(3.0f));
            enemy2.AddCommand(new WaitAction(3.0f));
            enemy3.AddCommand(new WaitAction(3.0f));
            enemy4.AddCommand(new WaitAction(3.0f));
            enemy5.AddCommand(new WaitAction(3.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,5,5"), enemyManager.GetWaypoint("Far_T5_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,5,5"), enemyManager.GetWaypoint("Far_T9_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("11,4,5"), enemyManager.GetWaypoint("Far_T11_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,3,5"), enemyManager.GetWaypoint("Far_T12_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("13,2,5"), enemyManager.GetWaypoint("Far_T13_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentSeven_BA.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_R5_Start"), enemy1, "Enemy_01"));
            segmentSeven_BA.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_R5_Start"), enemy2, "Enemy_01"));
            segmentSeven_BA.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_R4_Start"), enemy3, "Enemy_01"));
            segmentSeven_BA.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Near_R3_Start"), enemy4, "Enemy_01"));
            segmentSeven_BA.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Near_R2_Start"), enemy5, "Enemy_01"));

            encounterManager.AddEncounter(segmentSeven_BA);
        }

        void SegmentSeven_BB()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentSeven_BB = new Encounter(0.0f, 8.75f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T10_MidM"), 7.5f, small_enemy_health);
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T7_MidM"), 7.5f, small_enemy_health);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T4_MidM"), 7.5f, small_enemy_health);
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T6_MidM"), 7.5f, small_enemy_health);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T8_MidM"), 7.5f, small_enemy_health);

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T10_MidM"), enemyManager.GetWaypoint("Near_T10_MidM"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T7_MidM"), enemyManager.GetWaypoint("Near_T7_MidM"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T4_MidM"), enemyManager.GetWaypoint("Near_T4_MidM"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T6_MidM"), enemyManager.GetWaypoint("Near_T6_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T8_MidM"), enemyManager.GetWaypoint("Near_T8_MidM"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_T10_MidM"), enemyManager.GetWaypoint("10,2,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy6.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_T7_MidM"), enemyManager.GetWaypoint("7,2,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy7.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_T4_MidM"), enemyManager.GetWaypoint("4,2,5"), 1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy8.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_T6_MidM"), enemyManager.GetWaypoint("6,4,5"), 1.0f);
                action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy9.AddCommand(new CompositeAction(action4));

                BaseAction[] action5 = new BaseAction[2];
                action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_T8_MidM"), enemyManager.GetWaypoint("8,4,5"), 1.0f);
                action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy10.AddCommand(new CompositeAction(action5));
            }

            enemy6.AddCommand(new WaitAction(3.0f));
            enemy7.AddCommand(new WaitAction(3.0f));
            enemy8.AddCommand(new WaitAction(3.0f));
            enemy9.AddCommand(new WaitAction(3.0f));
            enemy10.AddCommand(new WaitAction(3.0f));

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,2,5"), enemyManager.GetWaypoint("Far_T10_MidM"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,2,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,2,5"), enemyManager.GetWaypoint("Far_T4_MidM"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,4,5"), enemyManager.GetWaypoint("Far_T6_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("8,4,5"), enemyManager.GetWaypoint("Far_T8_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentSeven_BB.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_T10_MidM"), enemy6, "Enemy_01"));
            segmentSeven_BB.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_T7_MidM"), enemy7, "Enemy_01"));
            segmentSeven_BB.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_T4_MidM"), enemy8, "Enemy_01"));
            segmentSeven_BB.AddSpawnAction(new SpawnAction(1.5f, enemyManager.GetWaypoint("Far_T6_MidM"), enemy9, "Enemy_01"));
            segmentSeven_BB.AddSpawnAction(new SpawnAction(1.5f, enemyManager.GetWaypoint("Far_T8_MidM"), enemy10, "Enemy_01"));

            encounterManager.AddEncounter(segmentSeven_BB);
        }

        void SegmentEight_A()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentEight_A = new Encounter(0.0f, 8.75f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T6_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T8_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T11_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T12_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T13_Start"), 7.5f, small_enemy_health);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T14_Start"), 7.5f, small_enemy_health);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T6_Start"), enemyManager.GetWaypoint("Near_T6_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T8_Start"), enemyManager.GetWaypoint("Near_T8_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T11_Start"), enemyManager.GetWaypoint("Near_T11_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T12_Start"), enemyManager.GetWaypoint("Near_T12_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T13_Start"), enemyManager.GetWaypoint("Near_T13_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T14_Start"), enemyManager.GetWaypoint("Near_T14_MidM"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_T6_MidM"), enemyManager.GetWaypoint("6,5,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_T8_MidM"), enemyManager.GetWaypoint("8,5,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_T11_MidM"), enemyManager.GetWaypoint("11,5,5"), 1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_T12_MidM"), enemyManager.GetWaypoint("12,4,5"), 1.0f);
                action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy4.AddCommand(new CompositeAction(action4));

                BaseAction[] action5 = new BaseAction[2];
                action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_T13_MidM"), enemyManager.GetWaypoint("13,3,5"), 1.0f);
                action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy5.AddCommand(new CompositeAction(action5));

                BaseAction[] action6 = new BaseAction[2];
                action6[0] = new MoveAction(enemyManager.GetWaypoint("Near_T14_MidM"), enemyManager.GetWaypoint("14,2,5"), 1.0f);
                action6[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy6.AddCommand(new CompositeAction(action6));
            }

            enemy1.AddCommand(new WaitAction(3.0f));
            enemy2.AddCommand(new WaitAction(3.0f));
            enemy3.AddCommand(new WaitAction(3.0f));
            enemy4.AddCommand(new WaitAction(3.0f));
            enemy5.AddCommand(new WaitAction(3.0f));
            enemy6.AddCommand(new WaitAction(3.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,5,5"), enemyManager.GetWaypoint("Far_T6_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("8,5,5"), enemyManager.GetWaypoint("Far_T8_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("11,5,5"), enemyManager.GetWaypoint("Far_T11_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,4,5"), enemyManager.GetWaypoint("Far_T12_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("13,3,5"), enemyManager.GetWaypoint("Far_T13_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("14,2,5"), enemyManager.GetWaypoint("Far_T14_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentEight_A.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_T6_Start"), enemy1, "Enemy_01"));
            segmentEight_A.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_T8_Start"), enemy2, "Enemy_01"));
            segmentEight_A.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_T11_Start"), enemy3, "Enemy_01"));
            segmentEight_A.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Near_T12_Start"), enemy4, "Enemy_01"));
            segmentEight_A.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Near_T13_Start"), enemy5, "Enemy_01"));
            segmentEight_A.AddSpawnAction(new SpawnAction(1.25f, enemyManager.GetWaypoint("Near_T14_Start"), enemy6, "Enemy_01"));

            encounterManager.AddEncounter(segmentEight_A);
        }

        void SegmentEight_B()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentEight_B = new Encounter(0.0f, 9.25f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T5"), 7.5f, small_enemy_health);
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T9"), 7.5f, small_enemy_health);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T4"), 7.5f, small_enemy_health);
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T10"), 7.5f, small_enemy_health);

            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T5"), enemyManager.GetWaypoint("Near_T5_MidM"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T9"), enemyManager.GetWaypoint("Near_T9_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T4"), enemyManager.GetWaypoint("Near_T4_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T10"), enemyManager.GetWaypoint("Near_T10_MidM"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_T5_MidM"), enemyManager.GetWaypoint("5,6,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy8.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_T9_MidM"), enemyManager.GetWaypoint("9,6,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy9.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_T4_MidM"), enemyManager.GetWaypoint("4,5,5"), 1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy10.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_T10_MidM"), enemyManager.GetWaypoint("10,5,5"), 1.0f);
                action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy11.AddCommand(new CompositeAction(action4));
            }

            enemy8.AddCommand(new WaitAction(3.0f));
            enemy9.AddCommand(new WaitAction(3.0f));
            enemy10.AddCommand(new WaitAction(3.0f));
            enemy11.AddCommand(new WaitAction(3.0f));

            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,6,5"), enemyManager.GetWaypoint("Far_T5_MidM"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,6,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,5,5"), enemyManager.GetWaypoint("Far_T9_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,5,5"), enemyManager.GetWaypoint("Far_T10_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentEight_B.AddSpawnAction(new SpawnAction(1.25f, enemyManager.GetWaypoint("Far_T5"), enemy8, "Enemy_01"));
            segmentEight_B.AddSpawnAction(new SpawnAction(1.25f, enemyManager.GetWaypoint("Far_T9"), enemy9, "Enemy_01"));
            segmentEight_B.AddSpawnAction(new SpawnAction(1.75f, enemyManager.GetWaypoint("Far_T4"), enemy10, "Enemy_01"));
            segmentEight_B.AddSpawnAction(new SpawnAction(1.75f, enemyManager.GetWaypoint("Far_T10"), enemy11, "Enemy_01"));

            encounterManager.AddEncounter(segmentEight_B);
        }

        void SegmentEight_C()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentEight_C = new Encounter(0.0f, 7.5f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T7_Start"), 7.5f, 100.0f);

            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_Start"), enemyManager.GetWaypoint("Near_T7_MidM"), 1.5f));

            //enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_MidM"), enemyManager.GetWaypoint("7,4,5"), 1.5f));

            //enemy12.AddCommand(new AttackAction("Shoot_D_1", Enemy2_fire_rate, 2, 1.5f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_T7_MidM"), enemyManager.GetWaypoint("7,4,5"), 1.5f);
                action1[1] = new AttackAction("Shoot_D_1", Enemy1_fire_rate, 2, 3.0f);
                enemy12.AddCommand(new CompositeAction(action1));
            }

            enemy12.AddCommand(new WaitAction(2.0f));

            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,4,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentEight_C.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_T7_Start"), enemy12, "Enemy_02"));

            encounterManager.AddEncounter(segmentEight_C);
        }

        void SegmentNine_AA()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentNine_AA = new Encounter(0.0f, 6.25f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R4_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R4_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R6_Start"), 5.5f, small_enemy_health);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R4_Start"), enemyManager.GetWaypoint("Near_R4_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R4_Start"), enemyManager.GetWaypoint("Near_R4_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R6_Start"), enemyManager.GetWaypoint("Near_R6_MidM"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_R4_MidM"), enemyManager.GetWaypoint("5,4,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_T8_MidM"), enemyManager.GetWaypoint("8,5,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("9,5,5"), 1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_R6_MidM"), enemyManager.GetWaypoint("11,6,5"), 1.0f);
                action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy4.AddCommand(new CompositeAction(action4));
            }

            enemy1.AddCommand(new WaitAction(1.0f));
            enemy2.AddCommand(new WaitAction(1.0f));
            enemy3.AddCommand(new WaitAction(1.0f));
            enemy4.AddCommand(new WaitAction(1.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,4,5"), enemyManager.GetWaypoint("Far_T3_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,4,5"), enemyManager.GetWaypoint("Far_T5_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,5,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("11,6,5"), enemyManager.GetWaypoint("Far_T9_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentNine_AA.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_R4_Start"), enemy1, "Enemy_01"));
            segmentNine_AA.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_R4_Start"), enemy2, "Enemy_01"));
            segmentNine_AA.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_R5_Start"), enemy3, "Enemy_01"));
            segmentNine_AA.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Near_R6_Start"), enemy4, "Enemy_01"));

            encounterManager.AddEncounter(segmentNine_AA);
        }

        void SegmentNine_AB()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentNine_AB = new Encounter(0.0f, 6.25f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L2_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 5.5f, small_enemy_health);

            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("Near_L2_MidM"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy5.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("8,3,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy6.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("6,2,5"), 1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy7.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("4,1,5"), 1.0f);
                action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy8.AddCommand(new CompositeAction(action4));
            }

            enemy5.AddCommand(new WaitAction(1.0f));
            enemy6.AddCommand(new WaitAction(1.0f));
            enemy7.AddCommand(new WaitAction(1.0f));
            enemy8.AddCommand(new WaitAction(1.0f));

            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,3,5"), enemyManager.GetWaypoint("Far_T10_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("8,3,5"), enemyManager.GetWaypoint("Far_T8_MidM"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,2,5"), enemyManager.GetWaypoint("Far_T6_MidM"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,1,5"), enemyManager.GetWaypoint("Far_T4_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentNine_AB.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_L3_Start"), enemy5, "Enemy_01"));
            segmentNine_AB.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_L3_Start"), enemy6, "Enemy_01"));
            segmentNine_AB.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_L2_Start"), enemy7, "Enemy_01"));
            segmentNine_AB.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Near_L1_Start"), enemy8, "Enemy_01"));

            encounterManager.AddEncounter(segmentNine_AB);
        }

        void SegmentNine_AC()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentNine_AC = new Encounter(0.0f, 6.25f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T5_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T7_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T9_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T11_Start"), 5.5f, small_enemy_health);

            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T5_Start"), enemyManager.GetWaypoint("Near_T5_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_Start"), enemyManager.GetWaypoint("Near_T7_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T9_Start"), enemyManager.GetWaypoint("Near_T9_MidM"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T11_Start"), enemyManager.GetWaypoint("Near_T11_MidM"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_T5_MidM"), enemyManager.GetWaypoint("5,3,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy9.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_T7_MidM"), enemyManager.GetWaypoint("7,3,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy10.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_T9_MidM"), enemyManager.GetWaypoint("9,3,5"), 1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy11.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_T11_MidM"), enemyManager.GetWaypoint("11,3,5"), 1.0f);
                action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy12.AddCommand(new CompositeAction(action4));
            }

            enemy9.AddCommand(new WaitAction(1.0f));
            enemy10.AddCommand(new WaitAction(1.0f));
            enemy11.AddCommand(new WaitAction(1.0f));
            enemy12.AddCommand(new WaitAction(1.0f));

            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,3,5"), enemyManager.GetWaypoint("Far_T5_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,3,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,3,5"), enemyManager.GetWaypoint("Far_T9_MidM"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("11,3,5"), enemyManager.GetWaypoint("Far_T11_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentNine_AC.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_T5_Start"), enemy9, "Enemy_01"));
            segmentNine_AC.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_T7_Start"), enemy10, "Enemy_01"));
            segmentNine_AC.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_T9_Start"), enemy11, "Enemy_01"));
            segmentNine_AC.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Near_T11_Start"), enemy12, "Enemy_01"));
            encounterManager.AddEncounter(segmentNine_AC);
        }

        void SegmentNine_AD()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentNine_AD = new Encounter(0.0f, 5.75f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy14 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy15 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R6_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy16 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 5.5f, small_enemy_health);

            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R6_Start"), enemyManager.GetWaypoint("Near_R6_MidM"), 1.0f));
            enemy16.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("Near_L6_MidM"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("10,5,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy13.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("4,5,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy14.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_R6_MidM"), enemyManager.GetWaypoint("8,6,5"), 1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy15.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_L6_MidM"), enemyManager.GetWaypoint("6,6,5"), 1.0f);
                action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy16.AddCommand(new CompositeAction(action4));
            }

            enemy13.AddCommand(new WaitAction(1.0f));
            enemy14.AddCommand(new WaitAction(1.0f));
            enemy15.AddCommand(new WaitAction(1.0f));
            enemy16.AddCommand(new WaitAction(1.0f));

            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,5,5"), enemyManager.GetWaypoint("Far_T10_MidM"), 1.0f));
            enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,5,5"), enemyManager.GetWaypoint("Far_T4_MidM"), 1.0f));
            enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("8,6,5"), enemyManager.GetWaypoint("Far_T8_MidM"), 1.0f));
            enemy16.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,6,5"), enemyManager.GetWaypoint("Far_T6_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentNine_AD.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_R5_Start"), enemy13, "Enemy_01"));
            segmentNine_AD.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_L5_Start"), enemy14, "Enemy_01"));
            segmentNine_AD.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_R6_Start"), enemy15, "Enemy_01"));
            segmentNine_AD.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_L6_Start"), enemy16, "Enemy_01"));

            encounterManager.AddEncounter(segmentNine_AD);
        }

        void SegmentNine_AE()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentNine_AE = new Encounter(0.0f, 5.75f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy17 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy18 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L2_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy19 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 5.5f, small_enemy_health);
            EnemyTemplate enemy20 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 5.5f, small_enemy_health);

            enemy17.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("Near_L6_MidM"), 1.0f));
            enemy18.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("Near_L2_MidM"), 1.0f));
            enemy19.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy20.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_L6_MidM"), enemyManager.GetWaypoint("7,6,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy17.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("7,2,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy18.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("5,5,5"), 1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy19.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("5,3,5"), 1.0f);
                action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy20.AddCommand(new CompositeAction(action4));
            }

            enemy17.AddCommand(new WaitAction(1.0f));
            enemy18.AddCommand(new WaitAction(1.0f));
            enemy19.AddCommand(new WaitAction(1.0f));
            enemy20.AddCommand(new WaitAction(1.0f));

            enemy17.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,6,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.0f));
            enemy18.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,2,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.0f));
            enemy19.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,5,5"), enemyManager.GetWaypoint("Far_T5_MidM"), 1.0f));
            enemy20.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,3,5"), enemyManager.GetWaypoint("Far_T5_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentNine_AE.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_L6_Start"), enemy17, "Enemy_01"));
            segmentNine_AE.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_L2_Start"), enemy18, "Enemy_01"));
            segmentNine_AE.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_L5_Start"), enemy19, "Enemy_01"));
            segmentNine_AE.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_L3_Start"), enemy20, "Enemy_01"));

            encounterManager.AddEncounter(segmentNine_AE);
        }

        void SegmentNine_BA()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentNine_BA = new Encounter(0.0f, 9.25f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 6.5f, small_enemy_health);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("Near_R3_MidM"), 1.0f));

            //enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("8,5,5"), 1.0f));
            //enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("10,5,5"), 1.0f));
            //enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("6,5,5"), 1.0f));
            //enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("4,5,5"), 1.0f));
            //enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("3,3,5"), 1.0f));
            //enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_MidM"), enemyManager.GetWaypoint("11,3,5"), 1.0f));

            //enemy1.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy3.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy4.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy5.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy6.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("8,5,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("10,5,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("6,5,5"), 1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("4,5,5"), 1.0f);
                action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy4.AddCommand(new CompositeAction(action4));

                BaseAction[] action5 = new BaseAction[2];
                action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("3,3,5"), 1.0f);
                action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy5.AddCommand(new CompositeAction(action5));

                BaseAction[] action6 = new BaseAction[2];
                action6[0] = new MoveAction(enemyManager.GetWaypoint("Near_R3_MidM"), enemyManager.GetWaypoint("11,3,5"), 1.0f);
                action6[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy6.AddCommand(new CompositeAction(action6));
            }

            enemy1.AddCommand(new WaitAction(2.0f));
            enemy2.AddCommand(new WaitAction(2.0f));
            enemy3.AddCommand(new WaitAction(2.0f));
            enemy4.AddCommand(new WaitAction(2.0f));
            enemy5.AddCommand(new WaitAction(2.0f));
            enemy6.AddCommand(new WaitAction(2.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("8,5,5"), enemyManager.GetWaypoint("Far_T8_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,5,5"), enemyManager.GetWaypoint("Far_T10_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,5,5"), enemyManager.GetWaypoint("Far_T6_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,5,5"), enemyManager.GetWaypoint("Far_T4_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,3,5"), enemyManager.GetWaypoint("Far_T3_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("11,3,5"), enemyManager.GetWaypoint("Far_T11_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentNine_BA.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_R5_Start"), enemy1, "Enemy_01"));
            segmentNine_BA.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_R5_Start"), enemy2, "Enemy_01"));
            segmentNine_BA.AddSpawnAction(new SpawnAction(1.25f, enemyManager.GetWaypoint("Near_L5_Start"), enemy3, "Enemy_01"));
            segmentNine_BA.AddSpawnAction(new SpawnAction(1.5f, enemyManager.GetWaypoint("Near_L5_Start"), enemy4, "Enemy_01"));
            segmentNine_BA.AddSpawnAction(new SpawnAction(2.5f, enemyManager.GetWaypoint("Near_L3_Start"), enemy5, "Enemy_01"));
            segmentNine_BA.AddSpawnAction(new SpawnAction(2.75f, enemyManager.GetWaypoint("Near_R3_Start"), enemy6, "Enemy_01"));

            encounterManager.AddEncounter(segmentNine_BA);
        }

        void SegmentNine_BB()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentNine_BB = new Encounter(0.0f, 10.0f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R2_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L2_Start"), 6.5f, small_enemy_health);
            EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R4_Start"), 8f, big_enemy_health);

            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_Start"), enemyManager.GetWaypoint("Near_R2_MidM"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("Near_L2_MidM"), 1.0f));
            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_Start"), enemyManager.GetWaypoint("Near_T7_MidM"), 1.5f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("9,5,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy9.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("5,5,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy10.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("10,2,5"), 1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy11.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("4,2,5"), 1.0f);
                action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy12.AddCommand(new CompositeAction(action4));

                BaseAction[] action5 = new BaseAction[2];
                action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_T7_MidM"), enemyManager.GetWaypoint("7,3,5"), 1.5f);
                action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy13.AddCommand(new CompositeAction(action5));
            }

            enemy9.AddCommand(new WaitAction(2.0f));
            enemy10.AddCommand(new WaitAction(2.0f));
            enemy11.AddCommand(new WaitAction(2.0f));
            enemy12.AddCommand(new WaitAction(2.0f));
            enemy13.AddCommand(new WaitAction(2.0f));

            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,5,5"), enemyManager.GetWaypoint("Far_T9_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,5,5"), enemyManager.GetWaypoint("Far_T5_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,2,5"), enemyManager.GetWaypoint("Far_T10_MidM"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,2,5"), enemyManager.GetWaypoint("Far_T4_MidM"), 1.0f));
            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,3,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.5f));

            //
            // GET SPAWN POINTS
            //
            segmentNine_BB.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_R5_Start"), enemy9, "Enemy_01"));
            segmentNine_BB.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_L5_Start"), enemy10, "Enemy_01"));
            segmentNine_BB.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Near_R2_Start"), enemy11, "Enemy_01"));
            segmentNine_BB.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Near_L2_Start"), enemy12, "Enemy_01"));
            segmentNine_BB.AddSpawnAction(new SpawnAction(2.0f, enemyManager.GetWaypoint("Near_R4_Start"), enemy13, "Enemy_02"));

            encounterManager.AddEncounter(segmentNine_BB);
        }

        void Level2_SegmentTwo()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter SegmentTwo = new Encounter(0.0f, 40.0f, enemyManager);

            //
            // Part A
            //
            {
                EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 4.5f, small_enemy_health);
                EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 4.5f, small_enemy_health);
                EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 4.5f, small_enemy_health); // laser enemy
                EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 4.5f, small_enemy_health);
                EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 4.5f, small_enemy_health);

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("2,2,5"), 1.0f);
                    action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy9.AddCommand(new CompositeAction(action1));

                    BaseAction[] action2 = new BaseAction[2];
                    action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("4,2,5"), 1.0f);
                    action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy10.AddCommand(new CompositeAction(action2));

                    BaseAction[] action4 = new BaseAction[2];
                    action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("10,2,5"), 1.0f);
                    action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy12.AddCommand(new CompositeAction(action4));

                    BaseAction[] action5 = new BaseAction[2];
                    action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("12,2,5"), 1.0f);
                    action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy13.AddCommand(new CompositeAction(action5));
                }

                enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("7,2,5"), 1.0f));
                enemy11.AddCommand(new WaitAction(1.0f));
                enemy11.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 1.0f));

                enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,2,5"), enemyManager.GetWaypoint("Near_L5_Start"), 1.0f));
                enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,2,5"), enemyManager.GetWaypoint("Near_L5_Start"), 1.0f));
                enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,2,5"), enemyManager.GetWaypoint("Near_L5_Start"), 1.0f));
                enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,2,5"), enemyManager.GetWaypoint("Near_L5_Start"), 1.0f));
                enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,2,5"), enemyManager.GetWaypoint("Near_L5_Start"), 1.0f));

                //
                // GET SPAWN POINTS
                //
                SegmentTwo.AddSpawnAction(new SpawnAction(2.5f, enemyManager.GetWaypoint("Near_R5_Start"), enemy9, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(2.7f, enemyManager.GetWaypoint("Near_R5_Start"), enemy10, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(2.9f, enemyManager.GetWaypoint("Near_R5_Start"), enemy11, "Enemy_03"));
                SegmentTwo.AddSpawnAction(new SpawnAction(3.1f, enemyManager.GetWaypoint("Near_R5_Start"), enemy12, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(3.3f, enemyManager.GetWaypoint("Near_R5_Start"), enemy13, "Enemy_01"));
            }
            //
            // Part B
            //
            {
                EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L7_Start"), 5.0f, small_enemy_health);
                EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L7_Start"), 7.0f, small_enemy_health); // laser
                EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L7_Start"), 5.0f, small_enemy_health);
                EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L7_Start"), 7.0f, small_enemy_health); // laser
                EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L7_Start"), 5.0f, small_enemy_health);

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_L7_Start"), enemyManager.GetWaypoint("2,7,5"), 1.0f);
                    action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy9.AddCommand(new CompositeAction(action1));

                    BaseAction[] action3 = new BaseAction[2];
                    action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_L7_Start"), enemyManager.GetWaypoint("7,7,5"), 1.0f);
                    action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate * 2.0f, 2, 3.0f);
                    enemy11.AddCommand(new CompositeAction(action3));

                    BaseAction[] action5 = new BaseAction[2];
                    action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_L7_Start"), enemyManager.GetWaypoint("12,7,5"), 1.0f);
                    action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy13.AddCommand(new CompositeAction(action5));
                }

                enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L7_Start"), enemyManager.GetWaypoint("4,7,5"), 1.0f));
                enemy10.AddCommand(new WaitAction(1.0f));
                enemy10.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 1.0f));

                enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L7_Start"), enemyManager.GetWaypoint("10,7,5"), 1.0f));
                enemy12.AddCommand(new WaitAction(1.4f));
                enemy12.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 0.6f));

                // Move and shoot part 2b
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("2,7,5"), enemyManager.GetWaypoint("2,3,5"), 1.0f);
                    action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy9.AddCommand(new CompositeAction(action1));

                    BaseAction[] action3 = new BaseAction[2];
                    action3[0] = new MoveAction(enemyManager.GetWaypoint("7,7,5"), enemyManager.GetWaypoint("7,3,5"), 1.0f);
                    action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate * 2.0f, 2, 3.0f);
                    enemy11.AddCommand(new CompositeAction(action3));

                    BaseAction[] action5 = new BaseAction[2];
                    action5[0] = new MoveAction(enemyManager.GetWaypoint("12,7,5"), enemyManager.GetWaypoint("12,3,5"), 1.0f);
                    action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy13.AddCommand(new CompositeAction(action5));
                }

                enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,7,5"), enemyManager.GetWaypoint("4,3,5"), 3.0f));

                enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,7,5"), enemyManager.GetWaypoint("10,3,5"), 3.0f));


                enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,3,5"), enemyManager.GetWaypoint("Near_R7_Start"), 1.0f));
                enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,3,5"), enemyManager.GetWaypoint("Near_R7_Start"), 1.0f));
                enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,3,5"), enemyManager.GetWaypoint("Near_R7_Start"), 1.0f));
                enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,3,5"), enemyManager.GetWaypoint("Near_R7_Start"), 1.0f));
                enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,3,5"), enemyManager.GetWaypoint("Near_R7_Start"), 1.0f));

                //
                // GET SPAWN POINTS
                //
                SegmentTwo.AddSpawnAction(new SpawnAction(7.5f, enemyManager.GetWaypoint("Near_L7_Start"), enemy9, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(7.7f, enemyManager.GetWaypoint("Near_L7_Start"), enemy10, "Enemy_03"));
                SegmentTwo.AddSpawnAction(new SpawnAction(7.9f, enemyManager.GetWaypoint("Near_L7_Start"), enemy11, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(8.1f, enemyManager.GetWaypoint("Near_L7_Start"), enemy12, "Enemy_03"));
                SegmentTwo.AddSpawnAction(new SpawnAction(8.3f, enemyManager.GetWaypoint("Near_L7_Start"), enemy13, "Enemy_01"));
            }

            // Part C
            {
                EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T1_Start"), 7.0f, small_enemy_health);
                EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T1_Start"), 7.0f, small_enemy_health);
                EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T1_Start"), 7.0f, small_enemy_health);
                EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T13_Start"), 7.0f, small_enemy_health);
                EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T13_Start"), 7.0f, small_enemy_health);
                EnemyTemplate enemy14 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T13_Start"), 7.0f, small_enemy_health);

                EnemyTemplate enemy15 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T7_Start"), 7.0f, big_enemy_health);
                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_T1_Start"), enemyManager.GetWaypoint("1,2,5"), 1.0f);
                    action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 7.0f);
                    enemy9.AddCommand(new CompositeAction(action1));

                    BaseAction[] action3 = new BaseAction[2];
                    action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_T1_Start"), enemyManager.GetWaypoint("1,4,5"), 1.0f);
                    action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate * 2.0f, 2, 7.0f);
                    enemy10.AddCommand(new CompositeAction(action3));

                    BaseAction[] action5 = new BaseAction[2];
                    action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_T1_Start"), enemyManager.GetWaypoint("1,6,5"), 1.0f);
                    action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 7.0f);
                    enemy11.AddCommand(new CompositeAction(action5));

                    BaseAction[] action6 = new BaseAction[2];
                    action6[0] = new MoveAction(enemyManager.GetWaypoint("Near_T13_Start"), enemyManager.GetWaypoint("13,2,5"), 1.0f);
                    action6[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 7.0f);
                    enemy12.AddCommand(new CompositeAction(action6));

                    BaseAction[] action7 = new BaseAction[2];
                    action7[0] = new MoveAction(enemyManager.GetWaypoint("Near_T13_Start"), enemyManager.GetWaypoint("13,4,5"), 1.0f);
                    action7[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 7.0f);
                    enemy13.AddCommand(new CompositeAction(action7));

                    BaseAction[] action8 = new BaseAction[2];
                    action8[0] = new MoveAction(enemyManager.GetWaypoint("Near_T13_Start"), enemyManager.GetWaypoint("13,6,5"), 1.0f);
                    action8[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 7.0f);
                    enemy14.AddCommand(new CompositeAction(action8));

                    BaseAction[] action9 = new BaseAction[2];
                    action9[0] = new MoveAction(enemyManager.GetWaypoint("Near_T7_Start"), enemyManager.GetWaypoint("7,4,5"), 1.0f);
                    action9[1] = new AttackAction("Spiral", Enemy2_fire_rate, 2, 7.0f);
                    enemy15.AddCommand(new CompositeAction(action9));
                }

                //
                // GET SPAWN POINTS
                //
                SegmentTwo.AddSpawnAction(new SpawnAction(16.4f, enemyManager.GetWaypoint("Near_T1_Start"), enemy9, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(16.2f, enemyManager.GetWaypoint("Near_T1_Start"), enemy10, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(16.0f, enemyManager.GetWaypoint("Near_T1_Start"), enemy11, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(16.4f, enemyManager.GetWaypoint("Near_T13_Start"), enemy12, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(16.2f, enemyManager.GetWaypoint("Near_T13_Start"), enemy13, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(16.0f, enemyManager.GetWaypoint("Near_T13_Start"), enemy14, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(16.0f, enemyManager.GetWaypoint("Near_T7_Start"), enemy15, "Enemy_02"));
            }

            encounterManager.AddEncounter(SegmentTwo);
        }

        void Level2_SegmentFour()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter SegmentTwo = new Encounter(0.0f, 26.0f, enemyManager);

            //
            // Part A
            //

            for (int i = 0; i <= 1; ++i)
            {
                EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T10_Start"), 5.0f, small_enemy_health);
                EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T10_Start"), 5.0f, small_enemy_health);
                EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T6_Start"), 5.0f, small_enemy_health); // laser enemy
                EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T4_Start"), 5.0f, small_enemy_health);
                EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T4_Start"), 5.0f, small_enemy_health);

                // Move and shoot
                {
                    enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T10_Start"), enemyManager.GetWaypoint("10,6,5"), 1.0f));
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("10,6,5"), enemyManager.GetWaypoint("10,6,5"), 3.0f);
                    action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy9.AddCommand(new CompositeAction(action1));

                    enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T10_Start"), enemyManager.GetWaypoint("10,1,5"), 1.0f));
                    BaseAction[] action2 = new BaseAction[2];
                    action2[0] = new MoveAction(enemyManager.GetWaypoint("10,1,5"), enemyManager.GetWaypoint("10,1,5"), 3.0f);
                    action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy10.AddCommand(new CompositeAction(action2));

                    enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T6_Start"), enemyManager.GetWaypoint("6,3,5"), 1.0f));
                    enemy11.AddCommand(new WaitAction(1.0f));
                    enemy11.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 2.0f));

                    enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T4_Start"), enemyManager.GetWaypoint("2,6,5"), 1.0f));
                    BaseAction[] action4 = new BaseAction[2];
                    action4[0] = new MoveAction(enemyManager.GetWaypoint("2,6,5"), enemyManager.GetWaypoint("2,6,5"), 3.0f);
                    action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy12.AddCommand(new CompositeAction(action4));

                    enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T4_Start"), enemyManager.GetWaypoint("2,1,5"), 1.0f));
                    BaseAction[] action5 = new BaseAction[2];
                    action5[0] = new MoveAction(enemyManager.GetWaypoint("2,1,5"), enemyManager.GetWaypoint("2,1,5"), 3.0f);
                    action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy13.AddCommand(new CompositeAction(action5));
                }


                if (i == 1)
                {
                    enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,6,5"), enemyManager.GetWaypoint("Near_L6_Start"), 1.0f));
                    enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,1,5"), enemyManager.GetWaypoint("Near_L1_Start"), 1.0f));
                    enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,3,5"), enemyManager.GetWaypoint("Near_L3_Start"), 1.0f));
                    enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,6,5"), enemyManager.GetWaypoint("Near_L6_Start"), 1.0f));
                    enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,1,5"), enemyManager.GetWaypoint("Near_L1_Start"), 1.0f));
                }
                else
                {
                    enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,6,5"), enemyManager.GetWaypoint("Near_R6_Start"), 1.0f));
                    enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,1,5"), enemyManager.GetWaypoint("Near_R1_Start"), 1.0f));
                    enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,3,5"), enemyManager.GetWaypoint("Near_R3_Start"), 1.0f));
                    enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,6,5"), enemyManager.GetWaypoint("Near_R6_Start"), 1.0f));
                    enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,1,5"), enemyManager.GetWaypoint("Near_R1_Start"), 1.0f));
                }

                //
                // GET SPAWN POINTS
                //
                SegmentTwo.AddSpawnAction(new SpawnAction(0.0f + (i * 6.0f), enemyManager.GetWaypoint("Near_T10_Start"), enemy9, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(0.2f + (i * 6.0f), enemyManager.GetWaypoint("Near_T10_Start"), enemy10, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(0.0f + (i * 6.0f), enemyManager.GetWaypoint("Near_T6_Start"), enemy11, "Enemy_03"));
                SegmentTwo.AddSpawnAction(new SpawnAction(0.0f + (i * 6.0f), enemyManager.GetWaypoint("Near_T4_Start"), enemy12, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(0.2f + (i * 6.0f), enemyManager.GetWaypoint("Near_T4_Start"), enemy13, "Enemy_01"));
            }
            //
            // Part B
            //
            {
                EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 6.0f, small_enemy_health);
                EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L0_Start"), 6.0f, small_enemy_health); // laser
                EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 6.0f, small_enemy_health);
                EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L0_Start"), 6.0f, small_enemy_health); // laser
                EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 6.0f, small_enemy_health);
                EnemyTemplate enemy14 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 6.0f, small_enemy_health); // laser
                EnemyTemplate enemy15 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 6.0f, small_enemy_health); // laser

                // Move and shoot
                {
                    enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("2,3,5"), 1.0f));
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("2,3,5"), enemyManager.GetWaypoint("2,3,5"), 3.0f);
                    action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy9.AddCommand(new CompositeAction(action1));

                    enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("7,3,5"), 1.0f));
                    BaseAction[] action3 = new BaseAction[2];
                    action3[0] = new MoveAction(enemyManager.GetWaypoint("7,3,5"), enemyManager.GetWaypoint("7,3,5"), 3.0f);
                    action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy11.AddCommand(new CompositeAction(action3));

                    enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("12,3,5"), 1.0f));
                    BaseAction[] action5 = new BaseAction[2];
                    action5[0] = new MoveAction(enemyManager.GetWaypoint("12,3,5"), enemyManager.GetWaypoint("12,3,5"), 3.0f);
                    action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy13.AddCommand(new CompositeAction(action5));
                }

                enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L0_Start"), enemyManager.GetWaypoint("4,0,5"), 1.0f));
                enemy10.AddCommand(new WaitAction(1.0f));
                enemy10.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 3.0f));

                enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L0_Start"), enemyManager.GetWaypoint("10,0,5"), 1.0f));
                enemy12.AddCommand(new WaitAction(1.4f));
                enemy12.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 2.6f));

                enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("4,6,5"), 1.0f));
                enemy14.AddCommand(new WaitAction(1.8f));
                enemy14.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 1.2f));

                enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("10,6,5"), 1.0f));
                enemy15.AddCommand(new WaitAction(2.2f));
                enemy15.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 0.8f));


                enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,3,5"), enemyManager.GetWaypoint("Near_R3_Start"), 1.0f));
                enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,0,5"), enemyManager.GetWaypoint("Near_R0_Start"), 1.0f));
                enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,3,5"), enemyManager.GetWaypoint("Near_R3_Start"), 1.0f));
                enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,0,5"), enemyManager.GetWaypoint("Near_R0_Start"), 1.0f));
                enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,3,5"), enemyManager.GetWaypoint("Near_R3_Start"), 1.0f));
                enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,6,5"), enemyManager.GetWaypoint("Near_R6_Start"), 1.0f));
                enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,6,5"), enemyManager.GetWaypoint("Near_R6_Start"), 1.0f));

                //
                // GET SPAWN POINTS
                //
                SegmentTwo.AddSpawnAction(new SpawnAction(10.2f, enemyManager.GetWaypoint("Near_L0_Start"), enemy12, "Enemy_03"));
                SegmentTwo.AddSpawnAction(new SpawnAction(10.2f, enemyManager.GetWaypoint("Near_L3_Start"), enemy13, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(10.2f, enemyManager.GetWaypoint("Near_L6_Start"), enemy15, "Enemy_03"));
                SegmentTwo.AddSpawnAction(new SpawnAction(10.4f, enemyManager.GetWaypoint("Near_L0_Start"), enemy10, "Enemy_03"));
                SegmentTwo.AddSpawnAction(new SpawnAction(10.4f, enemyManager.GetWaypoint("Near_L3_Start"), enemy11, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(10.4f, enemyManager.GetWaypoint("Near_L6_Start"), enemy14, "Enemy_03"));
                SegmentTwo.AddSpawnAction(new SpawnAction(10.6f, enemyManager.GetWaypoint("Near_L3_Start"), enemy9, "Enemy_01"));
            }

            //
            // Part C
            //
            {
                EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 6.0f, small_enemy_health); // laser
                EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R0_Start"), 6.0f, small_enemy_health);
                EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 6.0f, small_enemy_health); // laser
                EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R0_Start"), 6.0f, small_enemy_health);
                EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 6.0f, small_enemy_health); // laser
                EnemyTemplate enemy14 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R6_Start"), 6.0f, small_enemy_health);
                EnemyTemplate enemy15 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R6_Start"), 6.0f, small_enemy_health);

                // Move and shoot
                {
                    enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("4,0,5"), 1.0f));
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("4,0,5"), enemyManager.GetWaypoint("4,0,5"), 3.0f);
                    action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy10.AddCommand(new CompositeAction(action1));

                    enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("10,0,5"), 1.0f));
                    BaseAction[] action3 = new BaseAction[2];
                    action3[0] = new MoveAction(enemyManager.GetWaypoint("10,0,5"), enemyManager.GetWaypoint("10,0,5"), 3.0f);
                    action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy12.AddCommand(new CompositeAction(action3));

                    enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("4,6,5"), 1.0f));
                    BaseAction[] action6 = new BaseAction[2];
                    action6[0] = new MoveAction(enemyManager.GetWaypoint("4,6,5"), enemyManager.GetWaypoint("4,6,5"), 3.0f);
                    action6[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy14.AddCommand(new CompositeAction(action6));

                    enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("10,6,5"), 1.0f));
                    BaseAction[] action7 = new BaseAction[2];
                    action7[0] = new MoveAction(enemyManager.GetWaypoint("10,6,5"), enemyManager.GetWaypoint("10,6,5"), 3.0f);
                    action7[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                    enemy15.AddCommand(new CompositeAction(action7));
                }

                enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("2,3,5"), 1.0f));
                enemy9.AddCommand(new WaitAction(1.0f));
                enemy9.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 3.0f));

                enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("7,3,5"), 1.0f));
                enemy11.AddCommand(new WaitAction(1.4f));
                enemy11.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 2.6f));

                enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("12,3,5"), 1.0f));
                enemy13.AddCommand(new WaitAction(1.8f));
                enemy13.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 1.8f));


                enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,3,5"), enemyManager.GetWaypoint("Near_L3_Start"), 1.0f));
                enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,0,5"), enemyManager.GetWaypoint("Near_L0_Start"), 1.0f));
                enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,3,5"), enemyManager.GetWaypoint("Near_L3_Start"), 1.0f));
                enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,0,5"), enemyManager.GetWaypoint("Near_L0_Start"), 1.0f));
                enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,3,5"), enemyManager.GetWaypoint("Near_L3_Start"), 1.0f));
                enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,6,5"), enemyManager.GetWaypoint("Near_L6_Start"), 1.0f));
                enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,6,5"), enemyManager.GetWaypoint("Near_L6_Start"), 1.0f));

                //
                // GET SPAWN POINTS
                //
                SegmentTwo.AddSpawnAction(new SpawnAction(18.2f, enemyManager.GetWaypoint("Near_R6_Start"), enemy15, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(18.4f, enemyManager.GetWaypoint("Near_R6_Start"), enemy14, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(18.6f, enemyManager.GetWaypoint("Near_R3_Start"), enemy13, "Enemy_03"));
                SegmentTwo.AddSpawnAction(new SpawnAction(18.8f, enemyManager.GetWaypoint("Near_R0_Start"), enemy12, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(19.0f, enemyManager.GetWaypoint("Near_R3_Start"), enemy11, "Enemy_03"));
                SegmentTwo.AddSpawnAction(new SpawnAction(19.2f, enemyManager.GetWaypoint("Near_R0_Start"), enemy10, "Enemy_01"));
                SegmentTwo.AddSpawnAction(new SpawnAction(19.4f, enemyManager.GetWaypoint("Near_R3_Start"), enemy9, "Enemy_03"));
            }

            encounterManager.AddEncounter(SegmentTwo);
        }

        void Level2_SegmentFive()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter SegmentTwo = new Encounter(0.0f, 26.0f, enemyManager);

            //
            // Part A
            //
            {
                EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 5.0f, small_enemy_health);
                EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 5.0f, small_enemy_health); // laser
                EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 5.0f, small_enemy_health); // laser
                EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 5.0f, small_enemy_health);

                EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 5.0f, small_enemy_health); // laser
                EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 5.0f, small_enemy_health);
                EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 5.0f, small_enemy_health);
                EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 5.0f, small_enemy_health); // laser

                EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L0_Start"), 5.0f, small_enemy_health);
                EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L0_Start"), 5.0f, small_enemy_health); // laser
                EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L0_Start"), 5.0f, small_enemy_health); // laser
                EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L0_Start"), 5.0f, small_enemy_health);

                {
                    // Move and shoot bottom line
                    {
                        BaseAction[] action1 = new BaseAction[2];
                        action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("12,6,5"), 1.0f);
                        action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 4.0f);
                        enemy1.AddCommand(new CompositeAction(action1));

                        enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("9,6,5"), 1.0f));
                        enemy2.AddCommand(new WaitAction(1.0f));
                        enemy2.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 2.0f));

                        enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("5,6,5"), 1.0f));
                        enemy3.AddCommand(new WaitAction(1.0f));
                        enemy3.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 2.0f));

                        BaseAction[] action5 = new BaseAction[2];
                        action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("2,6,5"), 1.0f);
                        action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 4.0f);
                        enemy4.AddCommand(new CompositeAction(action5));
                    }

                    // move off screen bottom line
                    enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,6,5"), enemyManager.GetWaypoint("Near_R6_Start"), 1.0f));
                    enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,6,5"), enemyManager.GetWaypoint("Near_R6_Start"), 1.0f));
                    enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,6,5"), enemyManager.GetWaypoint("Near_R6_Start"), 1.0f));
                    enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,6,5"), enemyManager.GetWaypoint("Near_R6_Start"), 1.0f));

                    SegmentTwo.AddSpawnAction(new SpawnAction(0.2f, enemyManager.GetWaypoint("Near_L6_Start"), enemy1, "Enemy_01"));
                    SegmentTwo.AddSpawnAction(new SpawnAction(0.4f, enemyManager.GetWaypoint("Near_L6_Start"), enemy2, "Enemy_03"));
                    SegmentTwo.AddSpawnAction(new SpawnAction(0.6f, enemyManager.GetWaypoint("Near_L6_Start"), enemy3, "Enemy_03"));
                    SegmentTwo.AddSpawnAction(new SpawnAction(0.8f, enemyManager.GetWaypoint("Near_L6_Start"), enemy4, "Enemy_01"));
                }

                {
                    // Move and shoot bottom line
                    {
                        enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("12,3,5"), 1.0f));
                        enemy5.AddCommand(new WaitAction(1.0f));
                        enemy5.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 2.0f));

                        BaseAction[] action1 = new BaseAction[2];
                        action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("9,3,5"), 1.0f);
                        action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 4.0f);
                        enemy6.AddCommand(new CompositeAction(action1));

                        BaseAction[] action5 = new BaseAction[2];
                        action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("5,3,5"), 1.0f);
                        action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 4.0f);
                        enemy7.AddCommand(new CompositeAction(action5));

                        enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("2,3,5"), 1.0f));
                        enemy8.AddCommand(new WaitAction(1.0f));
                        enemy8.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 2.0f));

                    }

                    // move off screen bottom line
                    enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,3,5"), enemyManager.GetWaypoint("Near_L3_Start"), 1.0f));
                    enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,3,5"), enemyManager.GetWaypoint("Near_L3_Start"), 1.0f));
                    enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,3,5"), enemyManager.GetWaypoint("Near_L3_Start"), 1.0f));
                    enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,3,5"), enemyManager.GetWaypoint("Near_L3_Start"), 1.0f));

                    SegmentTwo.AddSpawnAction(new SpawnAction(1.2f, enemyManager.GetWaypoint("Near_R3_Start"), enemy5, "Enemy_03"));
                    SegmentTwo.AddSpawnAction(new SpawnAction(1.4f, enemyManager.GetWaypoint("Near_R3_Start"), enemy6, "Enemy_01"));
                    SegmentTwo.AddSpawnAction(new SpawnAction(1.6f, enemyManager.GetWaypoint("Near_R3_Start"), enemy7, "Enemy_01"));
                    SegmentTwo.AddSpawnAction(new SpawnAction(1.8f, enemyManager.GetWaypoint("Near_R3_Start"), enemy8, "Enemy_03"));
                }

                {
                    // Move and shoot bottom line
                    {
                        BaseAction[] action1 = new BaseAction[2];
                        action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_L0_Start"), enemyManager.GetWaypoint("12,0,5"), 1.0f);
                        action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 4.0f);
                        enemy10.AddCommand(new CompositeAction(action1));

                        enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L0_Start"), enemyManager.GetWaypoint("9,0,5"), 1.0f));
                        enemy11.AddCommand(new WaitAction(1.0f));
                        enemy11.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 2.0f));

                        enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L0_Start"), enemyManager.GetWaypoint("5,0,5"), 1.0f));
                        enemy12.AddCommand(new WaitAction(1.0f));
                        enemy12.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 2.0f));

                        BaseAction[] action5 = new BaseAction[2];
                        action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_L0_Start"), enemyManager.GetWaypoint("2,0,5"), 1.0f);
                        action5[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 4.0f);
                        enemy13.AddCommand(new CompositeAction(action5));
                    }

                    // move off screen bottom line
                    enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,0,5"), enemyManager.GetWaypoint("Near_R0_Start"), 1.0f));
                    enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,0,5"), enemyManager.GetWaypoint("Near_R0_Start"), 1.0f));
                    enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,0,5"), enemyManager.GetWaypoint("Near_R0_Start"), 1.0f));
                    enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,0,5"), enemyManager.GetWaypoint("Near_R0_Start"), 1.0f));

                    SegmentTwo.AddSpawnAction(new SpawnAction(2.2f, enemyManager.GetWaypoint("Near_L0_Start"), enemy10, "Enemy_01"));
                    SegmentTwo.AddSpawnAction(new SpawnAction(2.4f, enemyManager.GetWaypoint("Near_L0_Start"), enemy11, "Enemy_03"));
                    SegmentTwo.AddSpawnAction(new SpawnAction(2.6f, enemyManager.GetWaypoint("Near_L0_Start"), enemy12, "Enemy_03"));
                    SegmentTwo.AddSpawnAction(new SpawnAction(2.8f, enemyManager.GetWaypoint("Near_L0_Start"), enemy13, "Enemy_01"));
                }
            }

            //
            // Part B
            //
            {
                EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T10_Start"), 4.0f, small_enemy_health); // laser
                EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T10_Start"), 4.0f, small_enemy_health); // laser
                EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T6_Start"), 4.0f, big_enemy_health); // big
                EnemyTemplate enemy14 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T4_Start"), 4.0f, small_enemy_health); // laser
                EnemyTemplate enemy15 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T4_Start"), 4.0f, small_enemy_health); // laser

                // Move and shoot
                {
                    enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T10_Start"), enemyManager.GetWaypoint("10,6,5"), 1.0f));
                    enemy11.AddCommand(new WaitAction(1.0f));
                    enemy11.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 3.0f));

                    enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T10_Start"), enemyManager.GetWaypoint("10,1,5"), 1.0f));
                    enemy11.AddCommand(new WaitAction(1.0f));
                    enemy12.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 3.0f));

                    enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T6_Start"), enemyManager.GetWaypoint("6,3,5"), 1.0f));
                    enemy13.AddCommand(new WaitAction(1.0f));
                    enemy13.AddCommand(new AttackAction("Spiral", Enemy2_fire_rate, 2, 1.0f));

                    enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T4_Start"), enemyManager.GetWaypoint("2,6,5"), 1.0f));
                    enemy11.AddCommand(new WaitAction(1.0f));
                    enemy14.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 3.0f));

                    enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T4_Start"), enemyManager.GetWaypoint("2,1,5"), 1.0f));
                    enemy11.AddCommand(new WaitAction(1.0f));
                    enemy15.AddCommand(new AttackAction("Laser", Enemy3_fire_rate, 2, 3.0f));
                }


                enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,6,5"), enemyManager.GetWaypoint("Near_R6_Start"), 1.0f));
                enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,1,5"), enemyManager.GetWaypoint("Near_R1_Start"), 1.0f));
                enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,3,5"), enemyManager.GetWaypoint("Near_R3_Start"), 1.0f));
                enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,6,5"), enemyManager.GetWaypoint("Near_R6_Start"), 1.0f));
                enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,1,5"), enemyManager.GetWaypoint("Near_R1_Start"), 1.0f));

                //
                // GET SPAWN POINTS
                //
                SegmentTwo.AddSpawnAction(new SpawnAction(6.0f, enemyManager.GetWaypoint("Near_T10_Start"), enemy11, "Enemy_03"));
                SegmentTwo.AddSpawnAction(new SpawnAction(6.2f, enemyManager.GetWaypoint("Near_T10_Start"), enemy12, "Enemy_03"));
                SegmentTwo.AddSpawnAction(new SpawnAction(6.0f, enemyManager.GetWaypoint("Near_T6_Start"), enemy13, "Enemy_02"));
                SegmentTwo.AddSpawnAction(new SpawnAction(6.0f, enemyManager.GetWaypoint("Near_T4_Start"), enemy14, "Enemy_03"));
                SegmentTwo.AddSpawnAction(new SpawnAction(6.2f, enemyManager.GetWaypoint("Near_T4_Start"), enemy15, "Enemy_03"));
            }

            encounterManager.AddEncounter(SegmentTwo);
        }

        void Boss_Low1()
        {
            Boss_LeftChain();
            Boss_RightChainAltSingle();
        }

        void Boss_Low2()
        {
            Boss_LeftChainAltSingle();
            Boss_RightChain();
        }

        void Boss_Med1()
        {
            Boss_LeftChain();
            Boss_RightChain();
        }
        void Boss_Med2()
        {
            Boss_LeftChainAltSingle();
            Boss_RightChainAltSingle();
        }
        void Boss_High1()
        {
            Boss_LeftChainAlt();
            Boss_RightChainAlt();
        }

        void Boss_High2()
        {
            Boss_LeftChain();
            Boss_RightChain();
        }

        void Boss_LeftChain(float encounter_offset_time = 0.0f)
        {

            /*  total encounter time = 14.0 seconds
                total enemy count = 20 + 16 = 36 small enemies
             
                brief description:
                    A vertical line of small enemies spawn and move down on x=1,3 from y=0 to y=7.
                    Taking 2 lanes of space. The enemies fire faster than normal denying the left side of the screen.
                    Critically they are spaced 1 lane apart and leave the center lane safe.
            */

            float new_firerate = Enemy1_fire_rate;
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter Boss_LeftChain = new Encounter(encounter_offset_time, 10.0f, enemyManager);

            for (int i = 0; i < 5; ++i)
            {
                //
                // Part A
                //
                EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T2_MidM"), 2.5f, small_enemy_health);
                EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T2_MidM"), 2.5f, small_enemy_health);
                EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T2_MidM"), 2.5f, small_enemy_health);
                EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T2_MidM"), 2.5f, small_enemy_health);

                enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T2_MidM"), enemyManager.GetWaypoint("2,0,5"), 1.0f));
                enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T2_MidM"), enemyManager.GetWaypoint("2,0,5"), 1.0f));
                enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T2_MidM"), enemyManager.GetWaypoint("2,0,5"), 1.0f));
                enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T2_MidM"), enemyManager.GetWaypoint("2,0,5"), 1.0f));

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("2,0,5"), enemyManager.GetWaypoint("2,7,5"), 1.5f);
                    action1[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy1.AddCommand(new CompositeAction(action1));

                    BaseAction[] action2 = new BaseAction[2];
                    action2[0] = new MoveAction(enemyManager.GetWaypoint("2,0,5"), enemyManager.GetWaypoint("2,7,5"), 1.5f);
                    action2[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy2.AddCommand(new CompositeAction(action2));

                    BaseAction[] action3 = new BaseAction[2];
                    action3[0] = new MoveAction(enemyManager.GetWaypoint("2,0,5"), enemyManager.GetWaypoint("2,7,5"), 1.5f);
                    action3[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy3.AddCommand(new CompositeAction(action3));

                    BaseAction[] action4 = new BaseAction[2];
                    action4[0] = new MoveAction(enemyManager.GetWaypoint("2,0,5"), enemyManager.GetWaypoint("2,7,5"), 1.5f);
                    action4[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy4.AddCommand(new CompositeAction(action4));
                }

                //
                // GET SPAWN POINTS
                //
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 0.0f + (i * 2), enemyManager.GetWaypoint("Far_T2_MidM"), enemy1, "Enemy_01"));
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 0.5f + (i * 2), enemyManager.GetWaypoint("Far_T2_MidM"), enemy2, "Enemy_01"));
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 1.0f + (i * 2), enemyManager.GetWaypoint("Far_T2_MidM"), enemy3, "Enemy_01"));
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 1.5f + (i * 2), enemyManager.GetWaypoint("Far_T2_MidM"), enemy4, "Enemy_01"));
            }

            for (int i = 0; i < 4; ++i)
            {
                //
                // Part B
                //
                EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T5_MidM"), 2.5f, small_enemy_health);
                EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T5_MidM"), 2.5f, small_enemy_health);
                EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T5_MidM"), 2.5f, small_enemy_health);
                EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T5_MidM"), 2.5f, small_enemy_health);

                enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T5_MidM"), enemyManager.GetWaypoint("5,0,5"), 1.0f));
                enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T5_MidM"), enemyManager.GetWaypoint("5,0,5"), 1.0f));
                enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T5_MidM"), enemyManager.GetWaypoint("5,0,5"), 1.0f));
                enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T5_MidM"), enemyManager.GetWaypoint("5,0,5"), 1.0f));

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("5,0,5"), enemyManager.GetWaypoint("5,7,5"), 1.5f);
                    action1[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy5.AddCommand(new CompositeAction(action1));

                    BaseAction[] action2 = new BaseAction[2];
                    action2[0] = new MoveAction(enemyManager.GetWaypoint("5,0,5"), enemyManager.GetWaypoint("5,7,5"), 1.5f);
                    action2[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy6.AddCommand(new CompositeAction(action2));

                    BaseAction[] action3 = new BaseAction[2];
                    action3[0] = new MoveAction(enemyManager.GetWaypoint("5,0,5"), enemyManager.GetWaypoint("5,7,5"), 1.5f);
                    action3[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy7.AddCommand(new CompositeAction(action3));

                    BaseAction[] action4 = new BaseAction[2];
                    action4[0] = new MoveAction(enemyManager.GetWaypoint("5,0,5"), enemyManager.GetWaypoint("5,7,5"), 1.5f);
                    action4[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy8.AddCommand(new CompositeAction(action4));
                }

                //
                // GET SPAWN POINTS
                //
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 3.0f + (i * 2), enemyManager.GetWaypoint("Far_T5_MidM"), enemy5, "Enemy_01"));
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 3.5f + (i * 2), enemyManager.GetWaypoint("Far_T5_MidM"), enemy6, "Enemy_01"));
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 4.0f + (i * 2), enemyManager.GetWaypoint("Far_T5_MidM"), enemy7, "Enemy_01"));
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 4.5f + (i * 2), enemyManager.GetWaypoint("Far_T5_MidM"), enemy8, "Enemy_01"));
            }

            encounterManager.AddEncounter(Boss_LeftChain);
        }

        void Boss_LeftChainAlt(float encounter_offset_time = 0.0f)
        {

            /*  total encounter time = 11.0 seconds
                total enemy count = 2 big enemies
             
                brief description:
                    A big enemies come down at x = 3, and shoots in a spiral pattern, this denies the left side of the screen and is harder to dodge.
            */

            float new_firerate = Enemy2_fire_rate;
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter Boss_LeftChainAlt = new Encounter(encounter_offset_time, 10.0f, enemyManager);
            {
                //
                // Part A
                //
                EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T3_MidM"), 10.0f, big_enemy_health);
                EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T3_MidM"), 10.0f, big_enemy_health);

                enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T3_MidM"), enemyManager.GetWaypoint("3,0,5"), 1.0f));
                enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T3_MidM"), enemyManager.GetWaypoint("3,0,5"), 1.0f));

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("3,0,5"), enemyManager.GetWaypoint("3,6,5"), 2.0f);
                    action1[1] = new AttackAction("Spiral", new_firerate, 2, 2.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));

                    BaseAction[] action2 = new BaseAction[2];
                    action2[0] = new MoveAction(enemyManager.GetWaypoint("3,0,5"), enemyManager.GetWaypoint("3,1,5"), 2.0f);
                    action2[1] = new AttackAction("Spiral", new_firerate, 2, 2.0f, 2.0f);
                    enemy2.AddCommand(new CompositeAction(action2));

                }

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new WaitAction(6);
                    action1[1] = new AttackAction("Spiral", new_firerate, 2, 6.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));

                    BaseAction[] action2 = new BaseAction[2];
                    action2[0] = new WaitAction(6);
                    action2[1] = new AttackAction("Spiral", new_firerate, 2, 6.0f, 2.0f);
                    enemy2.AddCommand(new CompositeAction(action2));

                }

                enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,6,5"), enemyManager.GetWaypoint("Near_L7_Start"), 1.0f));
                enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,1,5"), enemyManager.GetWaypoint("Near_L7_Start"), 1.0f));

                //
                // GET SPAWN POINTS
                //
                Boss_LeftChainAlt.AddSpawnAction(new SpawnAction(encounter_offset_time + 0.0f, enemyManager.GetWaypoint("Far_T3_MidM"), enemy1, "Enemy_02"));
                Boss_LeftChainAlt.AddSpawnAction(new SpawnAction(encounter_offset_time + 1.0f, enemyManager.GetWaypoint("Far_T3_MidM"), enemy2, "Enemy_02"));
            }

            encounterManager.AddEncounter(Boss_LeftChainAlt);
        }

        void Boss_LeftChainAltSingle(float encounter_offset_time = 0.0f)
        {

            /*  total encounter time = 10.0 seconds
                total enemy count = 1 big enemies
             
                brief description:
                    A big enemies come down at x = 3, and shoots in a spiral pattern, this denies the left side of the screen and is harder to dodge.
            */

            float new_firerate = Enemy2_fire_rate;
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter Boss_LeftChainAlt = new Encounter(encounter_offset_time, 10.0f, enemyManager);
            {
                //
                // Part A
                //
                EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T3_MidM"), 10.0f, big_enemy_health);

                enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T3_MidM"), enemyManager.GetWaypoint("3,0,5"), 1.0f));

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("3,0,5"), enemyManager.GetWaypoint("3,6,5"), 2.0f);
                    action1[1] = new AttackAction("Spiral", new_firerate, 2, 2.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));

                }

                // Wait and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new WaitAction(1);
                    action1[1] = new AttackAction("Spiral", new_firerate, 2, 1.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));


                }

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("3,6,5"), enemyManager.GetWaypoint("3,1,5"), 2.0f);
                    action1[1] = new AttackAction("Spiral", new_firerate, 2, 2.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));

                }

                // Wait and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new WaitAction(1);
                    action1[1] = new AttackAction("Spiral", new_firerate, 2, 1.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));


                }

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("3,1,5"), enemyManager.GetWaypoint("3,6,5"), 2.0f);
                    action1[1] = new AttackAction("Spiral", new_firerate, 2, 2.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));

                }

                enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,6,5"), enemyManager.GetWaypoint("Near_L7_Start"), 1.0f));

                //
                // GET SPAWN POINTS
                //
                Boss_LeftChainAlt.AddSpawnAction(new SpawnAction(encounter_offset_time + 0.0f, enemyManager.GetWaypoint("Far_T3_MidM"), enemy1, "Enemy_02"));
            }

            encounterManager.AddEncounter(Boss_LeftChainAlt);
        }

        void Boss_RightChain(float encounter_offset_time = 0.0f)
        {
            Console.WriteLine("right chain called");
            /*  total encounter time = 14.0 seconds
                total enemy count = 20 + 16 = 36 small enemies
             
                brief description:
                    A vertical line of small enemies spawn and move down on x=10,13 from y=0 to y=7.
                    Taking 2 lanes of space. The enemies fire faster than normal denying the right side of the screen.
                    Critically they are spaced 1 lane apart and leave the center lane safe.
            */

            float new_firerate = Enemy1_fire_rate;
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter Boss_LeftChain = new Encounter(encounter_offset_time, 10.0f, enemyManager);

            for (int i = 0; i < 5; ++i)
            {
                //
                // Part A
                //
                EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T12_MidM"), 2.5f, small_enemy_health);
                EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T12_MidM"), 2.5f, small_enemy_health);
                EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T12_MidM"), 2.5f, small_enemy_health);
                EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T12_MidM"), 2.5f, small_enemy_health);

                enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T12_MidM"), enemyManager.GetWaypoint("12,0,5"), 1.0f));
                enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T12_MidM"), enemyManager.GetWaypoint("12,0,5"), 1.0f));
                enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T12_MidM"), enemyManager.GetWaypoint("12,0,5"), 1.0f));
                enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T12_MidM"), enemyManager.GetWaypoint("12,0,5"), 1.0f));

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("12,0,5"), enemyManager.GetWaypoint("12,7,5"), 1.5f);
                    action1[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy1.AddCommand(new CompositeAction(action1));

                    BaseAction[] action2 = new BaseAction[2];
                    action2[0] = new MoveAction(enemyManager.GetWaypoint("12,0,5"), enemyManager.GetWaypoint("12,7,5"), 1.5f);
                    action2[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy2.AddCommand(new CompositeAction(action2));

                    BaseAction[] action3 = new BaseAction[2];
                    action3[0] = new MoveAction(enemyManager.GetWaypoint("12,0,5"), enemyManager.GetWaypoint("12,7,5"), 1.5f);
                    action3[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy3.AddCommand(new CompositeAction(action3));

                    BaseAction[] action4 = new BaseAction[2];
                    action4[0] = new MoveAction(enemyManager.GetWaypoint("12,0,5"), enemyManager.GetWaypoint("12,7,5"), 1.5f);
                    action4[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy4.AddCommand(new CompositeAction(action4));
                }

                //
                // GET SPAWN POINTS
                //
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 0.0f + (i * 2), enemyManager.GetWaypoint("Far_T12_MidM"), enemy1, "Enemy_01"));
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 0.5f + (i * 2), enemyManager.GetWaypoint("Far_T12_MidM"), enemy2, "Enemy_01"));
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 1.0f + (i * 2), enemyManager.GetWaypoint("Far_T12_MidM"), enemy3, "Enemy_01"));
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 1.5f + (i * 2), enemyManager.GetWaypoint("Far_T12_MidM"), enemy4, "Enemy_01"));
            }

            for (int i = 0; i < 4; ++i)
            {
                //
                // Part B
                //
                EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T9_MidM"), 2.5f, 10.0f);
                EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T9_MidM"), 2.5f, 10.0f);
                EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T9_MidM"), 2.5f, 10.0f);
                EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T9_MidM"), 2.5f, 10.0f);

                enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T9_MidM"), enemyManager.GetWaypoint("9,0,5"), 1.0f));
                enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T9_MidM"), enemyManager.GetWaypoint("9,0,5"), 1.0f));
                enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T9_MidM"), enemyManager.GetWaypoint("9,0,5"), 1.0f));
                enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T9_MidM"), enemyManager.GetWaypoint("9,0,5"), 1.0f));

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("9,0,5"), enemyManager.GetWaypoint("9,7,5"), 1.5f);
                    action1[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy5.AddCommand(new CompositeAction(action1));

                    BaseAction[] action2 = new BaseAction[2];
                    action2[0] = new MoveAction(enemyManager.GetWaypoint("9,0,5"), enemyManager.GetWaypoint("9,7,5"), 1.5f);
                    action2[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy6.AddCommand(new CompositeAction(action2));

                    BaseAction[] action3 = new BaseAction[2];
                    action3[0] = new MoveAction(enemyManager.GetWaypoint("9,0,5"), enemyManager.GetWaypoint("9,7,5"), 1.5f);
                    action3[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy7.AddCommand(new CompositeAction(action3));

                    BaseAction[] action4 = new BaseAction[2];
                    action4[0] = new MoveAction(enemyManager.GetWaypoint("9,0,5"), enemyManager.GetWaypoint("9,7,5"), 1.5f);
                    action4[1] = new AttackAction("Burst_Front", new_firerate, 2, 1.5f);
                    enemy8.AddCommand(new CompositeAction(action4));
                }

                //
                // GET SPAWN POINTS
                //
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 3.0f + (i * 2), enemyManager.GetWaypoint("Far_T9_MidM"), enemy5, "Enemy_01"));
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 3.5f + (i * 2), enemyManager.GetWaypoint("Far_T9_MidM"), enemy6, "Enemy_01"));
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 4.0f + (i * 2), enemyManager.GetWaypoint("Far_T9_MidM"), enemy7, "Enemy_01"));
                Boss_LeftChain.AddSpawnAction(new SpawnAction(encounter_offset_time + 4.5f + (i * 2), enemyManager.GetWaypoint("Far_T9_MidM"), enemy8, "Enemy_01"));
            }

            encounterManager.AddEncounter(Boss_LeftChain);
        }

        void Boss_RightChainAlt(float encounter_offset_time = 0.0f)
        {

            /*  total encounter time = 11.0 seconds
               total enemy count = 2 big enemies

               brief description:
                   A big enemies come down at x = 3, and shoots in a spiral pattern, this denies the left side of the screen and is harder to dodge.
           */

            float new_firerate = Enemy2_fire_rate;
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter Boss_RightChainAlt = new Encounter(encounter_offset_time, 10.0f, enemyManager);
            {
                //
                // Part A
                //
                EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T11_MidM"), 10.0f, big_enemy_health);
                EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T11_MidM"), 10.0f, big_enemy_health);

                enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T11_MidM"), enemyManager.GetWaypoint("11,0,5"), 1.0f));
                enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T11_MidM"), enemyManager.GetWaypoint("11,0,5"), 1.0f));

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("11,0,5"), enemyManager.GetWaypoint("11,6,5"), 2.0f);
                    action1[1] = new AttackAction("Spiral", new_firerate, 2, 2.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));

                    BaseAction[] action2 = new BaseAction[2];
                    action2[0] = new MoveAction(enemyManager.GetWaypoint("11,0,5"), enemyManager.GetWaypoint("11,1,5"), 2.0f);
                    action2[1] = new AttackAction("Spiral", new_firerate, 2, 2.0f, 2.0f);
                    enemy2.AddCommand(new CompositeAction(action2));

                }

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new WaitAction(6);
                    action1[1] = new AttackAction("Spiral", new_firerate, 2, 6.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));

                    BaseAction[] action2 = new BaseAction[2];
                    action2[0] = new WaitAction(6);
                    action2[1] = new AttackAction("Spiral", new_firerate, 2, 6.0f, 2.0f);
                    enemy2.AddCommand(new CompositeAction(action2));

                }

                enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("11,6,5"), enemyManager.GetWaypoint("Near_R7_Start"), 1.0f));
                enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("11,1,5"), enemyManager.GetWaypoint("Near_R7_Start"), 1.0f));

                //
                // GET SPAWN POINTS
                //
                Boss_RightChainAlt.AddSpawnAction(new SpawnAction(encounter_offset_time + 0.0f, enemyManager.GetWaypoint("Far_T3_MidM"), enemy1, "Enemy_02"));
                Boss_RightChainAlt.AddSpawnAction(new SpawnAction(encounter_offset_time + 1.0f, enemyManager.GetWaypoint("Far_T3_MidM"), enemy2, "Enemy_02"));
            }

            encounterManager.AddEncounter(Boss_RightChainAlt);
        }

        void Boss_RightChainAltSingle(float encounter_offset_time = 0.0f)
        {

            /*  total encounter time = 11.0 seconds
                total enemy count = 1 big enemies

                brief description:
                    A big enemies come down at x = 3, and shoots in a spiral pattern, this denies the left side of the screen and is harder to dodge.
            */

            float new_firerate = Enemy2_fire_rate;
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter Boss_RightChainAlt = new Encounter(encounter_offset_time, 10.0f, enemyManager);
            {
                //
                // Part A
                //
                EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T11_MidM"), 10.0f, big_enemy_health);

                enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T11_MidM"), enemyManager.GetWaypoint("11,0,5"), 1.0f));

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("11,0,5"), enemyManager.GetWaypoint("11,6,5"), 2.0f);
                    action1[1] = new AttackAction("Spiral", new_firerate, 2, 2.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));
                }

                // Wait and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new WaitAction(1);
                    action1[1] = new AttackAction("Spiral", new_firerate, 1, 1.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));
                }

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("11,6,5"), enemyManager.GetWaypoint("11,1,5"), 2.0f);
                    action1[1] = new AttackAction("Spiral", new_firerate, 2, 2.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));
                }

                // Wait and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new WaitAction(1);
                    action1[1] = new AttackAction("Spiral", new_firerate, 1, 1.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));
                }

                // Move and shoot
                {
                    BaseAction[] action1 = new BaseAction[2];
                    action1[0] = new MoveAction(enemyManager.GetWaypoint("11,1,5"), enemyManager.GetWaypoint("11,6,5"), 2.0f);
                    action1[1] = new AttackAction("Spiral", new_firerate, 2, 2.0f, 2.0f);
                    enemy1.AddCommand(new CompositeAction(action1));
                }

                enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("11,6,5"), enemyManager.GetWaypoint("Near_R7_Start"), 1.0f));

                //
                // GET SPAWN POINTS
                //
                Boss_RightChainAlt.AddSpawnAction(new SpawnAction(encounter_offset_time + 0.0f, enemyManager.GetWaypoint("Far_T3_MidM"), enemy1, "Enemy_02"));
            }

            encounterManager.AddEncounter(Boss_RightChainAlt);
        }

        // This function exposes EnemyManager.GetAlive method,
        // Provided an ID, the function will return the alive state of the enemy.
        // True if enemy is still alive,
        // False if enemy is dead or cannot be found.
        public bool GetAlive(uint id)
        {
            return encounterManager.enemyManager.GetAlive(id);
        }


        // This function exposes EnemyManager.AddScore method.
        // Increments the score.
        //public void AddScore()
        //{
        //    //encounterManager.enemyManager.AddScore(true);
        //    PlayerScript.AddScore(true);
        //}

        // This function exposes EnemyManager.GetAlive method,
        // Provided an ID, the function will deal given damage to the enemy.
        public static void TakeDamage(uint id, float damage)
        {
            encounterManager.enemyManager.TakeDamage(id, damage);
        }

        public void DestroyEnemyFromObstacle(uint id)
        {
            encounterManager.enemyManager.DestroyEnemyFromObstacle(id);
        }
    }

}
