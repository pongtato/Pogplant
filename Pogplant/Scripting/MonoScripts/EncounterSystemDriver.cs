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
        float Enemy1_fire_rate = 5;
        float Enemy2_fire_rate = 2f;

        EncounterManager encounterManager = new EncounterManager();

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
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
                //case "SegmentFour_A":
                //    SegmentFour_A();
                //    break;
                //case "SegmentFour_B":
                //    SegmentFour_B();
                //    break;
                //case "SegmentFour_C":
                //    SegmentFour_C();
                //    break;
                default:
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
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R1_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R1_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R2_Start"), 8.0f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_Start"), enemyManager.GetWaypoint("Near_R1_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_Start"), enemyManager.GetWaypoint("Near_R1_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_Start"), enemyManager.GetWaypoint("Near_R2_MidM"), 1.0f));

            //enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("10,1,5"), 1.0f));
            //enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("8,1,5"), 1.0f));
            //enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("7,2,5"), 1.0f));

            //enemy1.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy3.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("10,1,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("8,1,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("7,2,5"), 1.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.5f);
                enemy3.AddCommand(new CompositeAction(action3));
            }

            //enemy1.AddCommand(new WaitAction(3.0f));
            //enemy2.AddCommand(new WaitAction(3.0f));
            //enemy3.AddCommand(new WaitAction(3.0f));

            // Wait and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new WaitAction(3.0f);
                action1[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.5f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new WaitAction(3.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new WaitAction(3.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f);
                enemy3.AddCommand(new CompositeAction(action3));
            }

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,1,5"), enemyManager.GetWaypoint("Far_T10"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("8,1,5"), enemyManager.GetWaypoint("Far_T9"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,2,5"), enemyManager.GetWaypoint("Far_T7"), 1.0f));

            //
            // Part B
            //
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 8.0f, 50.0f);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 7.5f, 50.0f);

            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("Near_L6_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("Near_L6_MidM"), 1.0f));

            //enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("7,5,5"), 1.0f));
            //enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_MidM"), enemyManager.GetWaypoint("6,6,5"), 1.0f));
            //enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_MidM"), enemyManager.GetWaypoint("4,6,5"), 1.0f));

            //enemy4.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));
            //enemy5.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy6.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("7,5,5"), 1.0f);
                action1[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 3.0f);
                enemy4.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_L6_MidM"), enemyManager.GetWaypoint("6,6,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy5.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_L6_MidM"), enemyManager.GetWaypoint("4,6,5"), 1.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy6.AddCommand(new CompositeAction(action3));
            }

            //enemy4.AddCommand(new WaitAction(3.0f));
            //enemy5.AddCommand(new WaitAction(3.0f));
            //enemy6.AddCommand(new WaitAction(3.0f));

            // Wait and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new WaitAction(3.0f);
                action1[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.5f);
                enemy4.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new WaitAction(3.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f);
                enemy5.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new WaitAction(3.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f);
                enemy6.AddCommand(new CompositeAction(action3));
            }

            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,5,5"), enemyManager.GetWaypoint("Far_T7"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,6,5"), enemyManager.GetWaypoint("Far_T6"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,6,5"), enemyManager.GetWaypoint("Far_T4"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentTwo_A.AddSpawnAction(new SpawnAction(3.0f, enemyManager.GetWaypoint("Near_R1_Start"), enemy1, "Enemy_01"));
            segmentTwo_A.AddSpawnAction(new SpawnAction(2.75f, enemyManager.GetWaypoint("Near_R1_Start"), enemy2, "Enemy_01"));
            segmentTwo_A.AddSpawnAction(new SpawnAction(2.5f, enemyManager.GetWaypoint("Near_R2_Start"), enemy3, "Enemy_01"));

            segmentTwo_A.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_L5_Start"), enemy4, "Enemy_01"));
            segmentTwo_A.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_L6_Start"), enemy5, "Enemy_01"));
            segmentTwo_A.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_L6_Start"), enemy6, "Enemy_01"));

            encounterManager.AddEncounter(segmentTwo_A);
        }

        void SegmentTwo_B()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentTwo_B = new Encounter(0.0f, 11.0f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R0_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R0_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R0_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R0_Start"), 7.5f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_Start"), enemyManager.GetWaypoint("Near_R0_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_Start"), enemyManager.GetWaypoint("Near_R0_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_Start"), enemyManager.GetWaypoint("Near_R0_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_Start"), enemyManager.GetWaypoint("Near_R0_MidM"), 1.0f));

            //enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_MidM"), enemyManager.GetWaypoint("3,0,5"), 1.0f));
            //enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_MidM"), enemyManager.GetWaypoint("5,0,5"), 1.0f));
            //enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_MidM"), enemyManager.GetWaypoint("7,0,5"), 1.0f));
            //enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_MidM"), enemyManager.GetWaypoint("9,0,5"), 1.0f));

            //enemy1.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy3.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy4.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_R0_MidM"), enemyManager.GetWaypoint("3,0,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_R0_MidM"), enemyManager.GetWaypoint("5,0,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_R0_MidM"), enemyManager.GetWaypoint("7,0,5"), 1.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.5f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_R0_MidM"), enemyManager.GetWaypoint("9,0,5"), 1.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.5f);
                enemy4.AddCommand(new CompositeAction(action4));
            }

            //enemy1.AddCommand(new WaitAction(3.0f));
            //enemy2.AddCommand(new WaitAction(3.0f));
            //enemy3.AddCommand(new WaitAction(3.0f));
            //enemy4.AddCommand(new WaitAction(3.0f));

            // Wait and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new WaitAction(3.0f);
                action1[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.5f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new WaitAction(3.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f);
                enemy2.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new WaitAction(3.0f);
                action3[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f);
                enemy3.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new WaitAction(3.0f);
                action4[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f);
                enemy4.AddCommand(new CompositeAction(action4));
            }

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,0,5"), enemyManager.GetWaypoint("Far_T3_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,0,5"), enemyManager.GetWaypoint("Far_T5_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,0,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,0,5"), enemyManager.GetWaypoint("Far_T9_MidM"), 1.0f));

            //
            // Part B
            //
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L4_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L4_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L4_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L4_Start"), 7.5f, 50.0f);

            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L4_Start"), enemyManager.GetWaypoint("Near_L4_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L4_Start"), enemyManager.GetWaypoint("Near_L4_MidM"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L4_Start"), enemyManager.GetWaypoint("Near_L4_MidM"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L4_Start"), enemyManager.GetWaypoint("Near_L4_MidM"), 1.0f));

            //enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L4_MidM"), enemyManager.GetWaypoint("11,4,5"), 1.0f));
            //enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L4_MidM"), enemyManager.GetWaypoint("9,4,5"), 1.0f));
            //enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L4_MidM"), enemyManager.GetWaypoint("7,4,5"), 1.0f));
            //enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L4_MidM"), enemyManager.GetWaypoint("5,4,5"), 1.0f));

            //enemy5.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy6.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy7.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy8.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_L4_MidM"), enemyManager.GetWaypoint("11,4,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy5.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_L4_MidM"), enemyManager.GetWaypoint("9,4,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy6.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_L4_MidM"), enemyManager.GetWaypoint("7,4,5"), 1.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.5f);
                enemy7.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_L4_MidM"), enemyManager.GetWaypoint("5,4,5"), 1.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.5f);
                enemy8.AddCommand(new CompositeAction(action4));
            }

            //enemy5.AddCommand(new WaitAction(3.0f));
            //enemy6.AddCommand(new WaitAction(3.0f));
            //enemy7.AddCommand(new WaitAction(3.0f));
            //enemy8.AddCommand(new WaitAction(3.0f));

            // Wait and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new WaitAction(3.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f);
                enemy5.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new WaitAction(3.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f);
                enemy6.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new WaitAction(3.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.5f);
                enemy7.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new WaitAction(3.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 1.5f);
                enemy8.AddCommand(new CompositeAction(action4));
            }

            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("11,4,5"), enemyManager.GetWaypoint("Far_T11_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,4,5"), enemyManager.GetWaypoint("Far_T9_MidM"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,4,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,4,5"), enemyManager.GetWaypoint("Far_T5_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentTwo_B.AddSpawnAction(new SpawnAction(2.75f, enemyManager.GetWaypoint("Near_R0_Start"), enemy1, "Enemy_01"));
            segmentTwo_B.AddSpawnAction(new SpawnAction(3.0f, enemyManager.GetWaypoint("Near_R0_Start"), enemy2, "Enemy_01"));
            segmentTwo_B.AddSpawnAction(new SpawnAction(3.25f, enemyManager.GetWaypoint("Near_R0_Start"), enemy3, "Enemy_01"));
            segmentTwo_B.AddSpawnAction(new SpawnAction(3.5f, enemyManager.GetWaypoint("Near_R0_Start"), enemy4, "Enemy_01"));

            segmentTwo_B.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_L4_Start"), enemy5, "Enemy_01"));
            segmentTwo_B.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_L4_Start"), enemy6, "Enemy_01"));
            segmentTwo_B.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_L4_Start"), enemy7, "Enemy_01"));
            segmentTwo_B.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Near_L4_Start"), enemy8, "Enemy_01"));

            encounterManager.AddEncounter(segmentTwo_B);
        }

        void SegmentFive_A()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentFive_A = new Encounter(0.0f, 8.75f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R1_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R1_Start"), 7.5f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("Near_R3_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("Near_R3_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_Start"), enemyManager.GetWaypoint("Near_R1_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_Start"), enemyManager.GetWaypoint("Near_R1_MidM"), 1.0f));

            //enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("8,5,5"), 1.0f));
            //enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("11,5,5"), 1.0f));
            //enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_MidM"), enemyManager.GetWaypoint("9,3,5"), 1.0f));
            //enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_MidM"), enemyManager.GetWaypoint("12,3,5"), 1.0f));
            //enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("10,1,5"), 1.0f));
            //enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("13,1,5"), 1.0f));

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
            Encounter segmentFive_B = new Encounter(0.0f, 8.75f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 7.5f, 50.0f);

            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 1.0f));

            //enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("6,5,5"), 1.0f));
            //enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("3,5,5"), 1.0f));
            //enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("5,3,5"), 1.0f));
            //enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("2,3,5"), 1.0f));
            //enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("4,1,5"), 1.0f));
            //enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("1,1,5"), 1.0f));

            //enemy7.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy8.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy9.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy10.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy11.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy12.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("6,5,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy7.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("3,5,5"), 1.0f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy8.AddCommand(new CompositeAction(action2));

                BaseAction[] action3 = new BaseAction[2];
                action3[0] = new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("5,3,5"), 1.0f);
                action3[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.5f);
                enemy9.AddCommand(new CompositeAction(action3));

                BaseAction[] action4 = new BaseAction[2];
                action4[0] = new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("2,3,5"), 1.0f);
                action4[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.5f);
                enemy10.AddCommand(new CompositeAction(action4));

                BaseAction[] action5 = new BaseAction[2];
                action5[0] = new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("4,1,5"), 1.0f);
                action5[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.5f);
                enemy11.AddCommand(new CompositeAction(action5));

                BaseAction[] action6 = new BaseAction[2];
                action6[0] = new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("1,1,5"), 1.0f);
                action6[1] = new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.5f);
                enemy12.AddCommand(new CompositeAction(action6));
            }

            enemy7.AddCommand(new WaitAction(3.0f));
            enemy8.AddCommand(new WaitAction(3.0f));
            enemy9.AddCommand(new WaitAction(3.0f));
            enemy10.AddCommand(new WaitAction(3.0f));
            enemy11.AddCommand(new WaitAction(3.0f));
            enemy12.AddCommand(new WaitAction(3.0f));

            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,5,5"), enemyManager.GetWaypoint("Far_T6_MidM"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,5,5"), enemyManager.GetWaypoint("Far_T3_MidM"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,3,5"), enemyManager.GetWaypoint("Far_T5_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,3,5"), enemyManager.GetWaypoint("Far_T2_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,1,5"), enemyManager.GetWaypoint("Far_T4_MidM"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,1,5"), enemyManager.GetWaypoint("Far_T1_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentFive_B.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_L5_Start"), enemy7, "Enemy_01"));
            segmentFive_B.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_L5_Start"), enemy8, "Enemy_01"));
            segmentFive_B.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_L3_Start"), enemy9, "Enemy_01"));
            segmentFive_B.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Near_L3_Start"), enemy10, "Enemy_01"));
            segmentFive_B.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Near_L1_Start"), enemy11, "Enemy_01"));
            segmentFive_B.AddSpawnAction(new SpawnAction(1.25f, enemyManager.GetWaypoint("Near_L1_Start"), enemy12, "Enemy_01"));

            encounterManager.AddEncounter(segmentFive_B);
        }

        void SegmentSix()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentSix = new Encounter(0.0f, 9.0f, enemyManager);

            //
            //Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T7_MidM"), 9.0f, 100.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_MidM"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.5f));

            //enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T7_MidM"), enemyManager.GetWaypoint("7,4,5"), 1.5f));

            //enemy1.AddCommand(new AttackAction("Shoot_D_1", Enemy2_fire_rate, 2, 1.5f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("Far_T7_MidM"), enemyManager.GetWaypoint("7,4,5"), 1.5f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 3.0f);
                enemy1.AddCommand(new CompositeAction(action1));
            }

            enemy1.AddCommand(new WaitAction(3.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,4,5"), enemyManager.GetWaypoint("Near_T7_MidM"), 1.5f));

            //
            //GET SPAWN POINTS
            //
            segmentSix.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_T7_MidM"), enemy1, "Enemy_02"));

            encounterManager.AddEncounter(segmentSix);
        }

        void SegmentSeven_A()
        {
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentSeven_A = new Encounter(0.0f, 7.5f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 7.5f, 100.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R6_Start"), 7.5f, 100.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 1.5f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R6_Start"), enemyManager.GetWaypoint("Near_R6_MidM"), 1.5f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("7,1,5"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R6_MidM"), enemyManager.GetWaypoint("7,6,5"), 1.0f));


            //enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,1,5"), enemyManager.GetWaypoint("10,3,5"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,6,5"), enemyManager.GetWaypoint("4,4,5"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("7,1,5"), enemyManager.GetWaypoint("10,3,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy1.AddCommand(new CompositeAction(action1));
            }

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,3,5"), enemyManager.GetWaypoint("7,6,5"), 1.0f));
            //enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,4,5"), enemyManager.GetWaypoint("7,1,5"), 1.0f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("4,4,5"), enemyManager.GetWaypoint("7,1,5"), 1.0f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 2.5f);
                enemy2.AddCommand(new CompositeAction(action1));
            }

            //enemy1.AddCommand(new AttackAction("Shoot_D_1", Enemy1_fire_rate, 2, 1.5f));
            //enemy2.AddCommand(new AttackAction("Shoot_D_1", Enemy1_fire_rate, 2, 1.5f));

            //enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,6,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.5f));
            //enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,1,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.5f));

            // Move and shoot
            {
                BaseAction[] action1 = new BaseAction[2];
                action1[0] = new MoveAction(enemyManager.GetWaypoint("7,6,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.5f);
                action1[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f);
                enemy1.AddCommand(new CompositeAction(action1));

                BaseAction[] action2 = new BaseAction[2];
                action2[0] = new MoveAction(enemyManager.GetWaypoint("7,1,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.5f);
                action2[1] = new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f);
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
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R4_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R2_Start"), 7.5f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R4_Start"), enemyManager.GetWaypoint("Near_R4_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("Near_R3_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_Start"), enemyManager.GetWaypoint("Near_R2_MidM"), 1.0f));

            //enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("5,5,5"), 1.0f));
            //enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("9,5,5"), 1.0f));
            //enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R4_MidM"), enemyManager.GetWaypoint("11,4,5"), 1.0f));
            //enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_MidM"), enemyManager.GetWaypoint("12,3,5"), 1.0f));
            //enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("13,2,5"), 1.0f));

            //enemy1.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy3.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy4.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy5.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

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
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T10_MidM"), 7.5f, 50.0f);
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T7_MidM"), 7.5f, 50.0f);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T4_MidM"), 7.5f, 50.0f);
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T6_MidM"), 7.5f, 50.0f);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T8_MidM"), 7.5f, 50.0f);

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T10_MidM"), enemyManager.GetWaypoint("Near_T10_MidM"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T7_MidM"), enemyManager.GetWaypoint("Near_T7_MidM"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T4_MidM"), enemyManager.GetWaypoint("Near_T4_MidM"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T6_MidM"), enemyManager.GetWaypoint("Near_T6_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T8_MidM"), enemyManager.GetWaypoint("Near_T8_MidM"), 1.0f));

            //enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T10_MidM"), enemyManager.GetWaypoint("10,2,5"), 1.0f));
            //enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_MidM"), enemyManager.GetWaypoint("7,2,5"), 1.0f));
            //enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T4_MidM"), enemyManager.GetWaypoint("4,2,5"), 1.0f));
            //enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T6_MidM"), enemyManager.GetWaypoint("6,4,5"), 1.0f));
            //enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T8_MidM"), enemyManager.GetWaypoint("8,4,5"), 1.0f));

            //enemy6.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy7.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy8.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy9.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy10.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

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
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T6_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T8_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T11_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T12_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T13_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T14_Start"), 7.5f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T6_Start"), enemyManager.GetWaypoint("Near_T6_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T8_Start"), enemyManager.GetWaypoint("Near_T8_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T11_Start"), enemyManager.GetWaypoint("Near_T11_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T12_Start"), enemyManager.GetWaypoint("Near_T12_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T13_Start"), enemyManager.GetWaypoint("Near_T13_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T14_Start"), enemyManager.GetWaypoint("Near_T14_MidM"), 1.0f));

            //enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T6_MidM"), enemyManager.GetWaypoint("6,5,5"), 1.0f));
            //enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T8_MidM"), enemyManager.GetWaypoint("8,5,5"), 1.0f));
            //enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T11_MidM"), enemyManager.GetWaypoint("11,5,5"), 1.0f));
            //enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T12_MidM"), enemyManager.GetWaypoint("12,4,5"), 1.0f));
            //enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T13_MidM"), enemyManager.GetWaypoint("13,3,5"), 1.0f));
            //enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T14_MidM"), enemyManager.GetWaypoint("14,2,5"), 1.0f));

            //enemy1.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy3.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy4.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy5.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy6.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

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
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T5"), 7.5f, 50.0f);
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T9"), 7.5f, 50.0f);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T4"), 7.5f, 50.0f);
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T10"), 7.5f, 50.0f);

            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T5"), enemyManager.GetWaypoint("Near_T5_MidM"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T9"), enemyManager.GetWaypoint("Near_T9_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T4"), enemyManager.GetWaypoint("Near_T4_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T10"), enemyManager.GetWaypoint("Near_T10_MidM"), 1.0f));

            //enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T5_MidM"), enemyManager.GetWaypoint("5,6,5"), 1.0f));
            //enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T9_MidM"), enemyManager.GetWaypoint("9,6,5"), 1.0f));
            //enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T4_MidM"), enemyManager.GetWaypoint("4,5,5"), 1.0f));
            //enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T10_MidM"), enemyManager.GetWaypoint("10,5,5"), 1.0f));

            //enemy8.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy9.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy10.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy11.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

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
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R4_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R4_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R6_Start"), 5.5f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R4_Start"), enemyManager.GetWaypoint("Near_R4_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R4_Start"), enemyManager.GetWaypoint("Near_R4_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R6_Start"), enemyManager.GetWaypoint("Near_R6_MidM"), 1.0f));

            //enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R4_MidM"), enemyManager.GetWaypoint("5,4,5"), 1.0f));
            //enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R4_MidM"), enemyManager.GetWaypoint("7,4,5"), 1.0f));
            //enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("9,5,5"), 1.0f));
            //enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R6_MidM"), enemyManager.GetWaypoint("11,6,5"), 1.0f));

            //enemy1.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy3.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy4.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

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
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L2_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 5.5f, 50.0f);

            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("Near_L2_MidM"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 1.0f));

            //enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("10,3,5"), 1.0f));
            //enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("8,3,5"), 1.0f));
            //enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("6,2,5"), 1.0f));
            //enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("4,1,5"), 1.0f));

            //enemy5.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy6.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy7.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy8.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

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
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T5_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T7_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T9_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T11_Start"), 5.5f, 50.0f);

            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T5_Start"), enemyManager.GetWaypoint("Near_T5_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_Start"), enemyManager.GetWaypoint("Near_T7_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T9_Start"), enemyManager.GetWaypoint("Near_T9_MidM"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T11_Start"), enemyManager.GetWaypoint("Near_T11_MidM"), 1.0f));

            //enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T5_MidM"), enemyManager.GetWaypoint("5,3,5"), 1.0f));
            //enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_MidM"), enemyManager.GetWaypoint("7,3,5"), 1.0f));
            //enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T9_MidM"), enemyManager.GetWaypoint("9,3,5"), 1.0f));
            //enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T11_MidM"), enemyManager.GetWaypoint("11,3,5"), 1.0f));

            //enemy9.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy10.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy11.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy12.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

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
            EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy14 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy15 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R6_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy16 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 5.5f, 50.0f);

            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R6_Start"), enemyManager.GetWaypoint("Near_R6_MidM"), 1.0f));
            enemy16.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("Near_L6_MidM"), 1.0f));

            //enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("10,5,5"), 1.0f));
            //enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("4,5,5"), 1.0f));
            //enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R6_MidM"), enemyManager.GetWaypoint("8,6,5"), 1.0f));
            //enemy16.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_MidM"), enemyManager.GetWaypoint("6,6,5"), 1.0f));

            //enemy13.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy14.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy15.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy16.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

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
            EnemyTemplate enemy17 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy18 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L2_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy19 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy20 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 5.5f, 50.0f);

            enemy17.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("Near_L6_MidM"), 1.0f));
            enemy18.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("Near_L2_MidM"), 1.0f));
            enemy19.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy20.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 1.0f));

            //enemy17.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_MidM"), enemyManager.GetWaypoint("7,6,5"), 1.0f));
            //enemy18.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("7,2,5"), 1.0f));
            //enemy19.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("5,5,5"), 1.0f));
            //enemy20.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("5,3,5"), 1.0f));

            //enemy17.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy18.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy19.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy20.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

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
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 6.5f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 6.5f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 6.5f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 6.5f, 50.0f);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 6.5f, 50.0f);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 6.5f, 50.0f);

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
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 6.5f, 50.0f);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 6.5f, 50.0f);
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R2_Start"), 6.5f, 50.0f);
            EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L2_Start"), 6.5f, 50.0f);
            EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R4_Start"), 8f, 50.0f);

            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_Start"), enemyManager.GetWaypoint("Near_R2_MidM"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("Near_L2_MidM"), 1.0f));
            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_Start"), enemyManager.GetWaypoint("Near_T7_MidM"), 1.5f));

            //enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("9,5,5"), 1.0f));
            //enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("5,5,5"), 1.0f));
            //enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("10,2,5"), 1.0f));
            //enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("4,2,5"), 1.0f));
            //enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_MidM"), enemyManager.GetWaypoint("7,3,5"), 1.5f));

            //enemy9.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy10.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy11.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy12.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            //enemy13.AddCommand(new AttackAction("Shoot_D_1", Enemy2_fire_rate, 2, 1.5f));

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
        public void AddScore()
        {
            //encounterManager.enemyManager.AddScore(true);
            EnemyManager.AddScore(true);
        }

        // This function exposes EnemyManager.GetAlive method,
        // Provided an ID, the function will deal given damage to the enemy.
        public void TakeDamage(uint id, float damage)
        {
            encounterManager.enemyManager.TakeDamage(id, damage);
        }
    }

}
