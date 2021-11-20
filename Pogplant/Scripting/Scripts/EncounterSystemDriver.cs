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
        //float Enemy2_fire_rate = 0.5f;

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
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    SegmentTwo_A();
            //}

            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    SegmentTwo_B();
            //}

            //if (Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    SegmentFour_A();
            //}

            //if (Input.GetKeyDown(KeyCode.Alpha4))
            //{
            //    SegmentFour_B();
            //}

            //if (Input.GetKeyDown(KeyCode.Alpha5))
            //{
            //    SegmentFour_C();
            //}

            //if (Input.GetKeyDown(KeyCode.Alpha6))
            //{
            //    SegmentFour_D();
            //}

            //if (Input.GetKeyDown(KeyCode.Alpha7))
            //{
            //    SegmentFour_E();
            //}

            //if (Input.GetKeyDown(KeyCode.Alpha0))
            //{
            //    TestWave();
            //}

        }

        // Update is called once per frame
        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            encounterManager.Update(dt);
            Update();
        }

        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
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
            Console.WriteLine("EncounterSystemDriver.SpawnWave(string)");
            switch (waveName)
            {
                case "SegmentTwo_A":
                    SegmentTwo_A();
                    Console.WriteLine("SegmentTwo_A called.");
                    break;
                case "SegmentTwo_B":
                    SegmentTwo_B();
                    Console.WriteLine("SegmentTwo_B called.");
                    break;
                case "SegmentFour_A":
                    SegmentFour_A();
                    break;
                case "SegmentFour_B":
                    SegmentFour_B();
                    break;
                case "SegmentFour_C":
                    SegmentFour_C();
                    break;
                default:
                    //Console.WriteLine("Unsupported wave name: \"" + waveName + "\"");
                    break;
            }
        }

        void TestWave()
        {

            //encounterManager = GetComponent<EncounterManager>();
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter test = new Encounter(1.0f, 10.0f, enemyManager);

            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T3"), 11.0f, 300.0f);

            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L0_Start"), 11.0f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 11.0f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L2_Start"), 11.0f, 50.0f);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 11.0f, 50.0f);

            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R0_Start"), 11.0f, 50.0f);
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R1_Start"), 11.0f, 50.0f);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R2_Start"), 11.0f, 50.0f);
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 11.0f, 50.0f);
            //

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T3"), enemyManager.GetWaypoint("3,1,0"), 1.25f));

            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L0_Start"), enemyManager.GetWaypoint("Near_L0_MidM"), 0.75f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 0.75f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("Near_L2_MidM"), 0.75f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 0.75f));

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_Start"), enemyManager.GetWaypoint("Near_R0_MidM"), 0.75f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_Start"), enemyManager.GetWaypoint("Near_R1_MidM"), 0.75f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_Start"), enemyManager.GetWaypoint("Near_R2_MidM"), 0.75f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("Near_R3_MidM"), 0.75f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,1,0"), enemyManager.GetWaypoint("3,1,2"), 1.25f));

            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L0_MidM"), enemyManager.GetWaypoint("1,0,5"), 0.75f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("1,1,5"), 0.75f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("1,2,5"), 0.75f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("1,3,5"), 0.75f));

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_MidM"), enemyManager.GetWaypoint("5,0,5"), 0.75f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("5,1,5"), 0.75f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("5,2,5"), 0.75f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_MidM"), enemyManager.GetWaypoint("5,3,5"), 0.75f));

            enemy1.AddCommand(new AttackAction("Shoot_D_1", Enemy1_fire_rate, 2, 4.5f));

            enemy2.AddCommand(new AttackAction("Shoot_D_1", Enemy1_fire_rate, 2, 4.0f));
            enemy3.AddCommand(new AttackAction("Shoot_D_1", Enemy1_fire_rate, 2, 4.0f));
            enemy4.AddCommand(new AttackAction("Shoot_D_1", Enemy1_fire_rate, 2, 4.0f));
            enemy5.AddCommand(new AttackAction("Shoot_D_1", Enemy1_fire_rate, 2, 4.0f));

            enemy6.AddCommand(new AttackAction("Shoot_D_1", Enemy1_fire_rate, 2, 4.0f));
            enemy7.AddCommand(new AttackAction("Shoot_D_1", Enemy1_fire_rate, 2, 4.0f));
            enemy8.AddCommand(new AttackAction("Shoot_D_1", Enemy1_fire_rate, 2, 4.0f));
            enemy9.AddCommand(new AttackAction("Shoot_D_1", Enemy1_fire_rate, 2, 4.0f));

            enemy1.AddCommand(new WaitAction(2.0f));

            enemy2.AddCommand(new WaitAction(2.0f));
            enemy3.AddCommand(new WaitAction(2.0f));
            enemy4.AddCommand(new WaitAction(2.0f));
            enemy5.AddCommand(new WaitAction(2.0f));

            enemy6.AddCommand(new WaitAction(2.0f));
            enemy7.AddCommand(new WaitAction(2.0f));
            enemy8.AddCommand(new WaitAction(2.0f));
            enemy9.AddCommand(new WaitAction(2.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,1,2"), enemyManager.GetWaypoint("Far_T3"), 1.25f));

            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,0,5"), enemyManager.GetWaypoint("Far_T1"), 0.75f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,1,5"), enemyManager.GetWaypoint("Far_T1"), 0.75f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,2,5"), enemyManager.GetWaypoint("Far_T1"), 0.75f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,3,5"), enemyManager.GetWaypoint("Far_T1"), 0.75f));

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,0,5"), enemyManager.GetWaypoint("Far_T5"), 0.75f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,1,5"), enemyManager.GetWaypoint("Far_T5"), 0.75f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,2,5"), enemyManager.GetWaypoint("Far_T5"), 0.75f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,3,5"), enemyManager.GetWaypoint("Far_T5"), 0.75f));

            //
            test.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_T3"), enemy1, "Enemy_02"));

            test.AddSpawnAction(new SpawnAction(2.0f, enemyManager.GetWaypoint("Near_L0_Start"), enemy2, "Enemy_03"));
            test.AddSpawnAction(new SpawnAction(2.25f, enemyManager.GetWaypoint("Near_L1_Start"), enemy3, "Enemy_03"));
            test.AddSpawnAction(new SpawnAction(2.5f, enemyManager.GetWaypoint("Near_L2_Start"), enemy4, "Enemy_03"));
            test.AddSpawnAction(new SpawnAction(2.75f, enemyManager.GetWaypoint("Near_L3_Start"), enemy5, "Enemy_03"));

            test.AddSpawnAction(new SpawnAction(2.0f, enemyManager.GetWaypoint("Near_R0_Start"), enemy6, "Enemy_03"));
            test.AddSpawnAction(new SpawnAction(2.25f, enemyManager.GetWaypoint("Near_R1_Start"), enemy7, "Enemy_03"));
            test.AddSpawnAction(new SpawnAction(2.5f, enemyManager.GetWaypoint("Near_R2_Start"), enemy8, "Enemy_03"));
            test.AddSpawnAction(new SpawnAction(2.75f, enemyManager.GetWaypoint("Near_R3_Start"), enemy9, "Enemy_03"));

            encounterManager.AddEncounter(test);
        }

        void WaveOne()
        {
            //encounterManager = GetComponent<EncounterManager>();
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter waveOne = new Encounter(0.5f, 12.5f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T1"), 5.0f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T2"), 5.0f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T3"), 5.0f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T4"), 5.0f, 50.0f);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T5"), 5.0f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T1"), enemyManager.GetWaypoint("1,0,0"), 0.75f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T2"), enemyManager.GetWaypoint("2,0,0"), 0.75f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T3"), enemyManager.GetWaypoint("3,0,0"), 0.75f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T4"), enemyManager.GetWaypoint("4,0,0"), 0.75f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T5"), enemyManager.GetWaypoint("5,0,0"), 0.75f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,0,0"), enemyManager.GetWaypoint("1,0,2"), 0.75f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,0,0"), enemyManager.GetWaypoint("2,0,2"), 0.75f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,0,0"), enemyManager.GetWaypoint("3,0,2"), 0.75f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,0,0"), enemyManager.GetWaypoint("4,0,2"), 0.75f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,0,0"), enemyManager.GetWaypoint("5,0,2"), 0.75f));

            enemy1.AddCommand(new AttackAction("Burst_Down_F", Enemy1_fire_rate, 2, 1.5f));
            enemy2.AddCommand(new AttackAction("Burst_Down_F", Enemy1_fire_rate, 2, 1.5f));
            enemy3.AddCommand(new AttackAction("Burst_Down_F", Enemy1_fire_rate, 2, 1.5f));
            enemy4.AddCommand(new AttackAction("Burst_Down_F", Enemy1_fire_rate, 2, 1.5f));
            enemy5.AddCommand(new AttackAction("Burst_Down_F", Enemy1_fire_rate, 2, 1.5f));

            enemy1.AddCommand(new WaitAction(1.0f));
            enemy2.AddCommand(new WaitAction(1.0f));
            enemy3.AddCommand(new WaitAction(1.0f));
            enemy4.AddCommand(new WaitAction(1.0f));
            enemy5.AddCommand(new WaitAction(1.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,0,2"), enemyManager.GetWaypoint("Far_T1"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,0,2"), enemyManager.GetWaypoint("Far_T2"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,0,2"), enemyManager.GetWaypoint("Far_T3"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,0,2"), enemyManager.GetWaypoint("Far_T4"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,0,2"), enemyManager.GetWaypoint("Far_T5"), 1.0f));

            //
            // Part B
            //
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R1_Start"), 5.0f, 50.0f);
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R1_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 5.0f, 50.0f);

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_Start"), enemyManager.GetWaypoint("Near_R1_MidM"), 0.75f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_Start"), enemyManager.GetWaypoint("Near_R1_MidM"), 0.75f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 0.75f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 0.75f));

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("6,1,5"), 0.75f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("4,1,5"), 0.75f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("2,1,5"), 0.75f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("0,1,5"), 0.75f));

            enemy6.AddCommand(new AttackAction("Burst_Up_M", Enemy1_fire_rate, 2, 1.5f));
            enemy7.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));
            enemy8.AddCommand(new AttackAction("Shoot_H_1", Enemy1_fire_rate, 2, 2.0f));
            enemy9.AddCommand(new AttackAction("Burst_Up_M", Enemy1_fire_rate, 2, 1.5f));

            enemy6.AddCommand(new WaitAction(1.0f));
            enemy7.AddCommand(new WaitAction(1.0f));
            enemy8.AddCommand(new WaitAction(1.0f));
            enemy9.AddCommand(new WaitAction(1.0f));

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,1,5"), enemyManager.GetWaypoint("Far_T6"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,1,5"), enemyManager.GetWaypoint("Far_T4"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,1,5"), enemyManager.GetWaypoint("Far_T2"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,1,5"), enemyManager.GetWaypoint("Far_T0"), 1.0f));

            //
            // Part C
            //
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L2_Start"), 5.0f, 50.0f);
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L2_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R2_Start"), 5.5f, 50.0f);
            EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R2_Start"), 5.0f, 50.0f);

            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_Start"), enemyManager.GetWaypoint("Near_R2_MidM"), 0.75f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_Start"), enemyManager.GetWaypoint("Near_R2_MidM"), 0.75f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("Near_L2_MidM"), 0.75f));
            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("Near_L2_MidM"), 0.75f));

            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("6,2,5"), 0.75f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("4,2,5"), 0.75f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("2,2,5"), 0.75f));
            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("0,2,5"), 0.75f));

            enemy10.AddCommand(new AttackAction("Burst_Down_M", Enemy1_fire_rate, 2, 1.5f));
            enemy11.AddCommand(new AttackAction("Shoot_H_1", Enemy1_fire_rate, 2, 2.0f));
            enemy12.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));
            enemy13.AddCommand(new AttackAction("Burst_Down_M", Enemy1_fire_rate, 2, 1.5f));

            enemy10.AddCommand(new WaitAction(1.0f));
            enemy11.AddCommand(new WaitAction(1.0f));
            enemy12.AddCommand(new WaitAction(1.0f));
            enemy13.AddCommand(new WaitAction(1.0f));

            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,1,5"), enemyManager.GetWaypoint("Far_T6"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,1,5"), enemyManager.GetWaypoint("Far_T4"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,1,5"), enemyManager.GetWaypoint("Far_T2"), 1.0f));
            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,1,5"), enemyManager.GetWaypoint("Far_T0"), 1.0f));

            //
            // Part D
            //
            EnemyTemplate enemy14 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T1"), 5.0f, 50.0f);
            EnemyTemplate enemy15 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T2"), 5.0f, 50.0f);
            EnemyTemplate enemy16 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T3"), 5.0f, 50.0f);
            EnemyTemplate enemy17 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T4"), 5.0f, 50.0f);
            EnemyTemplate enemy18 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T5"), 5.0f, 50.0f);

            enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T1"), enemyManager.GetWaypoint("1,3,0"), 0.75f));
            enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T2"), enemyManager.GetWaypoint("2,3,0"), 0.75f));
            enemy16.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T3"), enemyManager.GetWaypoint("3,3,0"), 0.75f));
            enemy17.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T4"), enemyManager.GetWaypoint("4,3,0"), 0.75f));
            enemy18.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T5"), enemyManager.GetWaypoint("4,3,0"), 0.75f));

            enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,3,0"), enemyManager.GetWaypoint("1,3,2"), 0.75f));
            enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,3,0"), enemyManager.GetWaypoint("2,3,2"), 0.75f));
            enemy16.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,3,0"), enemyManager.GetWaypoint("3,3,2"), 0.75f));
            enemy17.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,3,0"), enemyManager.GetWaypoint("4,3,2"), 0.75f));
            enemy18.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,3,0"), enemyManager.GetWaypoint("4,3,2"), 0.75f));

            enemy14.AddCommand(new AttackAction("Burst_Down_F", Enemy1_fire_rate, 2, 1.5f));
            enemy15.AddCommand(new AttackAction("Burst_Down_F", Enemy1_fire_rate, 2, 1.5f));
            enemy16.AddCommand(new AttackAction("Burst_Down_F", Enemy1_fire_rate, 2, 1.5f));
            enemy17.AddCommand(new AttackAction("Burst_Down_F", Enemy1_fire_rate, 2, 1.5f));
            enemy18.AddCommand(new AttackAction("Burst_Down_F", Enemy1_fire_rate, 2, 1.5f));

            enemy14.AddCommand(new WaitAction(1.0f));
            enemy15.AddCommand(new WaitAction(1.0f));
            enemy16.AddCommand(new WaitAction(1.0f));
            enemy17.AddCommand(new WaitAction(1.0f));
            enemy18.AddCommand(new WaitAction(1.0f));

            enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,3,2"), enemyManager.GetWaypoint("Far_T1"), 1.0f));
            enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,3,2"), enemyManager.GetWaypoint("Far_T2"), 1.0f));
            enemy16.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,3,2"), enemyManager.GetWaypoint("Far_T3"), 1.0f));
            enemy17.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,3,2"), enemyManager.GetWaypoint("Far_T4"), 1.0f));
            enemy18.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,3,2"), enemyManager.GetWaypoint("Far_T5"), 1.0f));

            // 
            //GET SPAWN POINTS
            //
            waveOne.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_T1"), enemy1, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Far_T2"), enemy2, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Far_T3"), enemy3, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Far_T4"), enemy4, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Far_T4"), enemy5, "Enemy_01"));

            waveOne.AddSpawnAction(new SpawnAction(2.5f, enemyManager.GetWaypoint("Near_R1_Start"), enemy6, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(2.5f, enemyManager.GetWaypoint("Near_R1_Start"), enemy7, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(3.0f, enemyManager.GetWaypoint("Near_L1_Start"), enemy8, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(3.0f, enemyManager.GetWaypoint("Near_L1_Start"), enemy9, "Enemy_01"));

            waveOne.AddSpawnAction(new SpawnAction(4.5f, enemyManager.GetWaypoint("Near_L2_Start"), enemy10, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(4.5f, enemyManager.GetWaypoint("Near_L2_Start"), enemy11, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(5.0f, enemyManager.GetWaypoint("Near_R2_Start"), enemy12, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(5.0f, enemyManager.GetWaypoint("Near_R2_Start"), enemy13, "Enemy_01"));

            waveOne.AddSpawnAction(new SpawnAction(6.5f, enemyManager.GetWaypoint("Far_T1"), enemy14, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(6.75f, enemyManager.GetWaypoint("Far_T2"), enemy15, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(7.0f, enemyManager.GetWaypoint("Far_T3"), enemy16, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(7.25f, enemyManager.GetWaypoint("Far_T4"), enemy17, "Enemy_01"));
            waveOne.AddSpawnAction(new SpawnAction(7.5f, enemyManager.GetWaypoint("Far_T4"), enemy18, "Enemy_01"));

            encounterManager.AddEncounter(waveOne);
        }

        void WaveTwo()
        {
            //calculate firerate in bullet/second
            //fire_rate = 1 / fire_rate;

            //encounterManager = GetComponent<EncounterManager>();
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter waveTwo = new Encounter(0.5f, 13.0f, enemyManager);


            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T6"), 5.0f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T5"), 5.0f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T4"), 5.0f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T4"), 5.5f, 50.0f);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T6"), 5.5f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T6"), enemyManager.GetWaypoint("6,0,0"), 0.75f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T5"), enemyManager.GetWaypoint("5,0,0"), 0.75f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T4"), enemyManager.GetWaypoint("4,0,0"), 0.75f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T4"), enemyManager.GetWaypoint("4,1,0"), 0.75f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T6"), enemyManager.GetWaypoint("6,1,0"), 0.75f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,0,0"), enemyManager.GetWaypoint("6,0,2"), 0.75f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,0,0"), enemyManager.GetWaypoint("5,0,2"), 0.75f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,0,0"), enemyManager.GetWaypoint("4,0,2"), 0.75f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,1,0"), enemyManager.GetWaypoint("4,1,5"), 0.75f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,1,0"), enemyManager.GetWaypoint("6,1,5"), 0.75f));

            enemy1.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy3.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy4.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));
            enemy5.AddCommand(new AttackAction("Shoot_H_1", Enemy1_fire_rate, 2, 2.0f));

            enemy1.AddCommand(new WaitAction(1.0f));
            enemy2.AddCommand(new WaitAction(1.0f));
            enemy3.AddCommand(new WaitAction(1.0f));
            enemy4.AddCommand(new WaitAction(1.0f));
            enemy5.AddCommand(new WaitAction(1.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,0,2"), enemyManager.GetWaypoint("Far_T6"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,0,2"), enemyManager.GetWaypoint("Far_T5"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,0,2"), enemyManager.GetWaypoint("Far_T4"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,1,5"), enemyManager.GetWaypoint("Far_T4"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,1,5"), enemyManager.GetWaypoint("Far_T6"), 1.0f));

            //
            // Part B
            //
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R2_Start"), 5.0f, 50.0f);
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R2_Start"), 5.0f, 50.0f);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T3_Start"), 5.0f, 50.0f);
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L2_Start"), 5.0f, 50.0f);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L2_Start"), 5.0f, 50.0f);

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_Start"), enemyManager.GetWaypoint("Near_R2_MidM"), 0.75f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_Start"), enemyManager.GetWaypoint("Near_R2_MidM"), 0.75f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T3_Start"), enemyManager.GetWaypoint("Near_T3_MidM"), 0.75f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("Near_L2_MidM"), 0.75f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("Near_L2_MidM"), 0.75f));

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("6,2,5"), 0.75f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("4,2,5"), 0.75f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T3_MidM"), enemyManager.GetWaypoint("3,2,5"), 0.75f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("2,2,5"), 0.75f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("0,2,5"), 0.75f));

            enemy6.AddCommand(new AttackAction("Burst_Left_M", Enemy1_fire_rate, 2, 1.5f));
            enemy7.AddCommand(new AttackAction("Burst_Left_M", Enemy1_fire_rate, 2, 1.5f));
            enemy8.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy9.AddCommand(new AttackAction("Burst_Right_M", Enemy1_fire_rate, 2, 1.5f));
            enemy10.AddCommand(new AttackAction("Burst_Right_M", Enemy1_fire_rate, 2, 1.5f));

            enemy6.AddCommand(new WaitAction(1.0f));
            enemy7.AddCommand(new WaitAction(1.0f));
            enemy8.AddCommand(new WaitAction(1.0f));
            enemy9.AddCommand(new WaitAction(1.0f));
            enemy10.AddCommand(new WaitAction(1.0f));

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,2,5"), enemyManager.GetWaypoint("Far_R2"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,2,5"), enemyManager.GetWaypoint("Far_R2"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,2,5"), enemyManager.GetWaypoint("Far_T3"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,2,5"), enemyManager.GetWaypoint("Far_L2"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,2,5"), enemyManager.GetWaypoint("Far_L2"), 1.0f));

            //
            // Part C
            //
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T0"), 5.5f, 50.0f);
            EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T2"), 5.5f, 50.0f);
            EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T2"), 5.0f, 50.0f);
            EnemyTemplate enemy14 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T1"), 5.0f, 50.0f);
            EnemyTemplate enemy15 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T0"), 5.0f, 50.0f);

            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T0"), enemyManager.GetWaypoint("0,1,0"), 0.75f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T2"), enemyManager.GetWaypoint("2,1,0"), 0.75f));
            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T2"), enemyManager.GetWaypoint("2,0,0"), 0.75f));
            enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T1"), enemyManager.GetWaypoint("1,0,0"), 0.75f));
            enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T0"), enemyManager.GetWaypoint("0,0,0"), 0.75f));

            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,1,0"), enemyManager.GetWaypoint("0,1,2"), 0.75f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,1,0"), enemyManager.GetWaypoint("2,1,2"), 0.75f));
            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,0,0"), enemyManager.GetWaypoint("2,0,2"), 0.75f));
            enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,0,0"), enemyManager.GetWaypoint("1,0,2"), 0.75f));
            enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,0,0"), enemyManager.GetWaypoint("0,0,2"), 0.75f));

            enemy11.AddCommand(new AttackAction("Shoot_H_1", Enemy1_fire_rate, 2, 2.0f));
            enemy12.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));
            enemy13.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy14.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy15.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            enemy11.AddCommand(new WaitAction(1.0f));
            enemy12.AddCommand(new WaitAction(1.0f));
            enemy13.AddCommand(new WaitAction(1.0f));
            enemy14.AddCommand(new WaitAction(1.0f));
            enemy15.AddCommand(new WaitAction(1.0f));

            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,1,2"), enemyManager.GetWaypoint("Far_T0"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,1,2"), enemyManager.GetWaypoint("Far_T2"), 1.0f));
            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,0,2"), enemyManager.GetWaypoint("Far_T2"), 1.0f));
            enemy14.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,0,2"), enemyManager.GetWaypoint("Far_T1"), 1.0f));
            enemy15.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,0,2"), enemyManager.GetWaypoint("Far_T0"), 1.0f));

            //
            // Part D
            //
            EnemyTemplate enemy16 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 5.0f, 50.0f);
            EnemyTemplate enemy17 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T1"), 5.5f, 50.0f);
            EnemyTemplate enemy18 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T2"), 5.0f, 50.0f);
            EnemyTemplate enemy19 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T3"), 5.0f, 50.0f);
            EnemyTemplate enemy20 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T4"), 5.0f, 50.0f);
            EnemyTemplate enemy21 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T5"), 5.5f, 50.0f);
            EnemyTemplate enemy22 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R3_Start"), 5.0f, 50.0f);

            enemy16.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 0.75f));
            enemy17.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T1"), enemyManager.GetWaypoint("1,2,0"), 0.75f));
            enemy18.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T2"), enemyManager.GetWaypoint("2,3,0"), 0.75f));
            enemy19.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T3"), enemyManager.GetWaypoint("3,3,0"), 0.75f));
            enemy20.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T4"), enemyManager.GetWaypoint("4,3,0"), 0.75f));
            enemy21.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T5"), enemyManager.GetWaypoint("5,2,0"), 0.75f));
            enemy22.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_Start"), enemyManager.GetWaypoint("Near_R3_MidM"), 0.75f));

            enemy16.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("0,3,5"), 0.75f));
            enemy17.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,2,0"), enemyManager.GetWaypoint("1,2,5"), 0.75f));
            enemy18.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,3,0"), enemyManager.GetWaypoint("2,3,5"), 0.75f));
            enemy19.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,3,0"), enemyManager.GetWaypoint("3,3,5"), 0.75f));
            enemy20.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,3,0"), enemyManager.GetWaypoint("4,3,5"), 0.75f));
            enemy21.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,2,0"), enemyManager.GetWaypoint("5,2,5"), 0.75f));
            enemy22.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R3_MidM"), enemyManager.GetWaypoint("6,3,5"), 0.75f));

            enemy16.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy17.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));
            enemy18.AddCommand(new AttackAction("Burst_Up_M", Enemy1_fire_rate, 2, 1.5f));
            enemy19.AddCommand(new AttackAction("Burst_Up_M", Enemy1_fire_rate, 2, 1.5f));
            enemy20.AddCommand(new AttackAction("Burst_Up_M", Enemy1_fire_rate, 2, 1.5f));
            enemy21.AddCommand(new AttackAction("Shoot_H_1", Enemy1_fire_rate, 2, 2.0f));
            enemy22.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            enemy16.AddCommand(new WaitAction(1.0f));
            enemy17.AddCommand(new WaitAction(1.0f));
            enemy18.AddCommand(new WaitAction(1.0f));
            enemy19.AddCommand(new WaitAction(1.0f));
            enemy20.AddCommand(new WaitAction(1.0f));
            enemy21.AddCommand(new WaitAction(1.0f));
            enemy22.AddCommand(new WaitAction(1.0f));

            enemy16.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,3,5"), enemyManager.GetWaypoint("Far_T0"), 1.0f));
            enemy17.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,2,5"), enemyManager.GetWaypoint("Far_T1"), 1.0f));
            enemy18.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,3,5"), enemyManager.GetWaypoint("Far_T2"), 1.0f));
            enemy19.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,3,5"), enemyManager.GetWaypoint("Far_T3"), 1.0f));
            enemy20.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,3,5"), enemyManager.GetWaypoint("Far_T4"), 1.0f));
            enemy21.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,2,5"), enemyManager.GetWaypoint("Far_T5"), 1.0f));
            enemy22.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,3,5"), enemyManager.GetWaypoint("Far_T6"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            waveTwo.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_T5"), enemy1, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Far_T4"), enemy2, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Far_T3"), enemy3, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Far_T3"), enemy4, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Far_T5"), enemy5, "Enemy_01"));

            waveTwo.AddSpawnAction(new SpawnAction(2.5f, enemyManager.GetWaypoint("Near_R1_Start"), enemy6, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(2.5f, enemyManager.GetWaypoint("Near_L1_Start"), enemy7, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(2.5f, enemyManager.GetWaypoint("Near_L1_Start"), enemy8, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(2.5f, enemyManager.GetWaypoint("Near_L1_Start"), enemy9, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(2.5f, enemyManager.GetWaypoint("Near_L1_Start"), enemy10, "Enemy_01"));

            waveTwo.AddSpawnAction(new SpawnAction(4.0f, enemyManager.GetWaypoint("Near_L2_Start"), enemy11, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(4.25f, enemyManager.GetWaypoint("Near_R2_Start"), enemy12, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(4.5f, enemyManager.GetWaypoint("Near_R2_Start"), enemy13, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(4.75f, enemyManager.GetWaypoint("Near_R2_Start"), enemy14, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(5.0f, enemyManager.GetWaypoint("Near_R2_Start"), enemy15, "Enemy_01"));

            waveTwo.AddSpawnAction(new SpawnAction(6.5f, enemyManager.GetWaypoint("Far_T3"), enemy16, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(6.75f, enemyManager.GetWaypoint("Far_T4"), enemy17, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(7.0f, enemyManager.GetWaypoint("Far_T3"), enemy18, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(7.0f, enemyManager.GetWaypoint("Far_T3"), enemy19, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(7.0f, enemyManager.GetWaypoint("Far_T4"), enemy20, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(6.75f, enemyManager.GetWaypoint("Far_T3"), enemy21, "Enemy_01"));
            waveTwo.AddSpawnAction(new SpawnAction(6.5f, enemyManager.GetWaypoint("Far_T4"), enemy22, "Enemy_01"));

            encounterManager.AddEncounter(waveTwo);
        }

        void WaveThree()
        {
            //calculate firerate in bullet/second
            //fire_rate = 1 / fire_rate;

            //encounterManager = GetComponent<EncounterManager>();
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter waveThree = new Encounter(0.5f, 13.0f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 5.0f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T1"), 5.0f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 5.0f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T1"), 5.5f, 50.0f);

            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R1_Start"), 5.0f, 50.0f);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T5"), 5.0f, 50.0f);
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R1_Start"), 5.0f, 50.0f);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T5"), 5.5f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidF"), 0.75f));
            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_MidF"), enemyManager.GetWaypoint("2,1,2"), 0.75f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T1"), enemyManager.GetWaypoint("1,1,0"), 0.75f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,1,0"), enemyManager.GetWaypoint("1,1,2"), 0.75f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L_Start"), enemyManager.GetWaypoint("Near_L1_MidF"), 0.75f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_MidF"), enemyManager.GetWaypoint("0,1,2"), 0.75f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T1"), enemyManager.GetWaypoint("1,0,0"), 0.75f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,0,0"), enemyManager.GetWaypoint("1,0,2"), 0.75f));

            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_Start"), enemyManager.GetWaypoint("Near_R1_MidF"), 0.75f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_MidF"), enemyManager.GetWaypoint("4,1,2"), 0.75f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T5"), enemyManager.GetWaypoint("5,1,0"), 0.75f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,1,0"), enemyManager.GetWaypoint("5,1,2"), 0.75f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_Start"), enemyManager.GetWaypoint("Near_R1_MidF"), 0.75f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_MidF"), enemyManager.GetWaypoint("6,1,2"), 0.75f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T5"), enemyManager.GetWaypoint("5,0,0"), 0.75f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,0,0"), enemyManager.GetWaypoint("5,0,2"), 0.75f));

            enemy1.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy3.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy4.AddCommand(new AttackAction("Shoot_H_1", Enemy1_fire_rate, 2, 2.0f));

            enemy5.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy6.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy7.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy8.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));

            enemy1.AddCommand(new WaitAction(1.0f));
            enemy2.AddCommand(new WaitAction(1.0f));
            enemy3.AddCommand(new WaitAction(1.0f));
            enemy4.AddCommand(new WaitAction(1.0f));

            enemy5.AddCommand(new WaitAction(1.0f));
            enemy6.AddCommand(new WaitAction(1.0f));
            enemy7.AddCommand(new WaitAction(1.0f));
            enemy8.AddCommand(new WaitAction(1.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("2,1,2"), enemyManager.GetWaypoint("Far_T2"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,1,2"), enemyManager.GetWaypoint("Far_T1"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,1,2"), enemyManager.GetWaypoint("Far_T0"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,0,2"), enemyManager.GetWaypoint("Far_T1"), 1.0f));

            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,1,2"), enemyManager.GetWaypoint("Far_T4"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,1,2"), enemyManager.GetWaypoint("Far_T5"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,1,2"), enemyManager.GetWaypoint("Far_T6"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,0,2"), enemyManager.GetWaypoint("Far_T5"), 1.0f));

            //
            // Part B
            //
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R2_Start"), 5.0f, 50.0f);

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_Start"), enemyManager.GetWaypoint("Near_R2_MidM"), 0.75f));

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("6,2,5"), 0.75f));

            enemy6.AddCommand(new AttackAction("Burst_Left_M", Enemy1_fire_rate, 2, 1.5f));

            enemy6.AddCommand(new WaitAction(1.0f));

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,2,5"), enemyManager.GetWaypoint("Far_R2"), 1.0f));

            //
            // Part C
            //
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T0"), 5.5f, 50.0f);

            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T0"), enemyManager.GetWaypoint("0,1,0"), 0.75f));

            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,1,0"), enemyManager.GetWaypoint("0,1,2"), 0.75f));

            enemy11.AddCommand(new AttackAction("Shoot_H_1", Enemy1_fire_rate, 2, 2.0f));

            enemy11.AddCommand(new WaitAction(1.0f));

            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,1,2"), enemyManager.GetWaypoint("Far_T0"), 1.0f));

            //
            // Part D
            //
            EnemyTemplate enemy16 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L3_Start"), 5.0f, 50.0f);

            enemy16.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 0.75f));

            enemy16.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("0,3,5"), 0.75f));

            enemy16.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            enemy16.AddCommand(new WaitAction(1.0f));

            enemy16.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,3,5"), enemyManager.GetWaypoint("Far_T0"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            waveThree.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_L1_Start"), enemy1, "Enemy_01"));
            waveThree.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_T1"), enemy2, "Enemy_01"));
            waveThree.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_L1_Start"), enemy3, "Enemy_01"));
            waveThree.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_T1"), enemy4, "Enemy_01"));

            waveThree.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_R1_Start"), enemy5, "Enemy_01"));
            waveThree.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_T5"), enemy6, "Enemy_01"));
            waveThree.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_R1_Start"), enemy7, "Enemy_01"));
            waveThree.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_T5"), enemy8, "Enemy_01"));

            waveThree.AddSpawnAction(new SpawnAction(2.5f, enemyManager.GetWaypoint("Near_R1_Start"), enemy6, "Enemy_01"));

            waveThree.AddSpawnAction(new SpawnAction(4.0f, enemyManager.GetWaypoint("Near_L2_Start"), enemy11, "Enemy_01"));

            waveThree.AddSpawnAction(new SpawnAction(6.5f, enemyManager.GetWaypoint("Far_T3"), enemy16, "Enemy_01"));

            encounterManager.AddEncounter(waveThree);
        }

        void SegmentTwo_A()
        {
            //calculate firerate in bullet/second
            //fire_rate = 1 / fire_rate;

            //encounterManager = GetComponent<EncounterManager>();
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

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("10,1,5"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("8,1,5"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("7,2,5"), 1.0f));

            enemy1.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy3.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));

            enemy1.AddCommand(new WaitAction(3.0f));
            enemy2.AddCommand(new WaitAction(3.0f));
            enemy3.AddCommand(new WaitAction(3.0f));

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

            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("7,5,5"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_MidM"), enemyManager.GetWaypoint("6,6,5"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_MidM"), enemyManager.GetWaypoint("4,6,5"), 1.0f));

            enemy4.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));
            enemy5.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy6.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            enemy4.AddCommand(new WaitAction(3.0f));
            enemy5.AddCommand(new WaitAction(3.0f));
            enemy6.AddCommand(new WaitAction(3.0f));

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
            //calculate firerate in bullet/second
            //fire_rate = 1 / fire_rate;

            //encounterManager = GetComponent<EncounterManager>();
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

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_MidM"), enemyManager.GetWaypoint("3,0,5"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_MidM"), enemyManager.GetWaypoint("5,0,5"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_MidM"), enemyManager.GetWaypoint("7,0,5"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R0_MidM"), enemyManager.GetWaypoint("9,0,5"), 1.0f));

            enemy1.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy3.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy4.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            enemy1.AddCommand(new WaitAction(3.0f));
            enemy2.AddCommand(new WaitAction(3.0f));
            enemy3.AddCommand(new WaitAction(3.0f));
            enemy4.AddCommand(new WaitAction(3.0f));

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

            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L4_MidM"), enemyManager.GetWaypoint("11,4,5"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L4_MidM"), enemyManager.GetWaypoint("9,4,5"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L4_MidM"), enemyManager.GetWaypoint("7,4,5"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L4_MidM"), enemyManager.GetWaypoint("5,4,5"), 1.0f));

            enemy5.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy6.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy7.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy8.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            enemy5.AddCommand(new WaitAction(3.0f));
            enemy6.AddCommand(new WaitAction(3.0f));
            enemy7.AddCommand(new WaitAction(3.0f));
            enemy8.AddCommand(new WaitAction(3.0f));

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

        void SegmentFour_A()
        {
            //calculate firerate in bullet/second
            //fire_rate = 1 / fire_rate;

            //encounterManager = GetComponent<EncounterManager>();
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentFour_A = new Encounter(0.0f, 12.0f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T12_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T11_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T9_Start"), 8.0f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T7_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T6_Start"), 7.5f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T12_Start"), enemyManager.GetWaypoint("Near_T12_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T11_Start"), enemyManager.GetWaypoint("Near_T11_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T9_Start"), enemyManager.GetWaypoint("Near_T9_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_Start"), enemyManager.GetWaypoint("Near_T7_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T6_Start"), enemyManager.GetWaypoint("Near_T6_MidM"), 1.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T12_MidM"), enemyManager.GetWaypoint("12,3,5"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T11_MidM"), enemyManager.GetWaypoint("11,4,5"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T9_MidM"), enemyManager.GetWaypoint("9,4,5"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_MidM"), enemyManager.GetWaypoint("7,4,5"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T6_MidM"), enemyManager.GetWaypoint("6,3,5"), 1.0f));

            enemy1.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy3.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));
            enemy4.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy5.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            enemy1.AddCommand(new WaitAction(3.0f));
            enemy2.AddCommand(new WaitAction(3.0f));
            enemy3.AddCommand(new WaitAction(3.0f));
            enemy4.AddCommand(new WaitAction(3.0f));
            enemy5.AddCommand(new WaitAction(3.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,3,5"), enemyManager.GetWaypoint("Far_T12_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("11,4,5"), enemyManager.GetWaypoint("Far_T11_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,4,5"), enemyManager.GetWaypoint("Far_T9_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,4,5"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,3,5"), enemyManager.GetWaypoint("Far_T6_MidM"), 1.0f));

            //
            // Part B
            //
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T6_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T5_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T3_Start"), 8.0f, 50.0f);
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T1_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T0_Start"), 7.5f, 50.0f);

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T6_Start"), enemyManager.GetWaypoint("Near_T6_MidM"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T5_Start"), enemyManager.GetWaypoint("Near_T5_MidM"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T3_Start"), enemyManager.GetWaypoint("Near_T3_MidM"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T1_Start"), enemyManager.GetWaypoint("Near_T1_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T0_Start"), enemyManager.GetWaypoint("Near_T0_MidM"), 1.0f));

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T6_MidM"), enemyManager.GetWaypoint("6,1,5"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T5_MidM"), enemyManager.GetWaypoint("5,0,5"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T3_MidM"), enemyManager.GetWaypoint("3,0,5"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T1_MidM"), enemyManager.GetWaypoint("1,0,5"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T0_MidM"), enemyManager.GetWaypoint("0,1,5"), 1.0f));

            enemy6.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy7.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy8.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));
            enemy9.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy10.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            enemy6.AddCommand(new WaitAction(3.0f));
            enemy7.AddCommand(new WaitAction(3.0f));
            enemy8.AddCommand(new WaitAction(3.0f));
            enemy9.AddCommand(new WaitAction(3.0f));
            enemy10.AddCommand(new WaitAction(3.0f));

            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,1,5"), enemyManager.GetWaypoint("Far_T6_MidM"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,0,5"), enemyManager.GetWaypoint("Far_T5_MidM"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,0,5"), enemyManager.GetWaypoint("Far_T3_MidM"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,0,5"), enemyManager.GetWaypoint("Far_T1_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,1,5"), enemyManager.GetWaypoint("Far_T0_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentFour_A.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_T12_Start"), enemy1, "Enemy_01"));
            segmentFour_A.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_T11_Start"), enemy2, "Enemy_01"));
            segmentFour_A.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_T9_Start"), enemy3, "Enemy_01"));
            segmentFour_A.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Near_T7_Start"), enemy4, "Enemy_01"));
            segmentFour_A.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Near_T6_Start"), enemy5, "Enemy_01"));

            segmentFour_A.AddSpawnAction(new SpawnAction(3.0f, enemyManager.GetWaypoint("Near_T6_Start"), enemy6, "Enemy_01"));
            segmentFour_A.AddSpawnAction(new SpawnAction(3.25f, enemyManager.GetWaypoint("Near_T5_Start"), enemy7, "Enemy_01"));
            segmentFour_A.AddSpawnAction(new SpawnAction(3.5f, enemyManager.GetWaypoint("Near_T3_Start"), enemy8, "Enemy_01"));
            segmentFour_A.AddSpawnAction(new SpawnAction(3.75f, enemyManager.GetWaypoint("Near_T1_Start"), enemy9, "Enemy_01"));
            segmentFour_A.AddSpawnAction(new SpawnAction(4.0f, enemyManager.GetWaypoint("Near_T0_Start"), enemy10, "Enemy_01"));

            encounterManager.AddEncounter(segmentFour_A);
        }

        void SegmentFour_B()
        {
            //calculate firerate in bullet/second
            //fire_rate = 1 / fire_rate;

            //encounterManager = GetComponent<EncounterManager>();
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentFour_B = new Encounter(0.0f, 11.75f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T11"), 7.5f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T9"), 8.0f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T8"), 7.5f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T7"), 7.5f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T11"), enemyManager.GetWaypoint("Far_T11_MidC"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T9"), enemyManager.GetWaypoint("Far_T9_MidC"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T8"), enemyManager.GetWaypoint("Far_T8_MidC"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T7"), enemyManager.GetWaypoint("Far_T7_MidC"), 1.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T11_MidC"), enemyManager.GetWaypoint("10,1,5"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T9_MidC"), enemyManager.GetWaypoint("13,1,5"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T8_MidC"), enemyManager.GetWaypoint("13,3,5"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T7_MidC"), enemyManager.GetWaypoint("13,4,5"), 1.0f));

            enemy1.AddCommand(new AttackAction("Shoot_H_1", Enemy1_fire_rate, 2, 2.0f));
            enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy3.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy4.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            enemy1.AddCommand(new WaitAction(3.0f));
            enemy2.AddCommand(new WaitAction(3.0f));
            enemy3.AddCommand(new WaitAction(3.0f));
            enemy4.AddCommand(new WaitAction(3.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,1,5"), enemyManager.GetWaypoint("Far_T11"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("13,1,5"), enemyManager.GetWaypoint("Far_T9"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("13,3,5"), enemyManager.GetWaypoint("Far_T8"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("13,4,5"), enemyManager.GetWaypoint("Far_T7"), 1.0f));

            //
            // Part B
            //
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T6"), 8.0f, 50.0f);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 8.0f, 50.0f);
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 8.0f, 50.0f);

            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T6"), enemyManager.GetWaypoint("Far_T6_MidC"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("Near_L6_MidC"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("Near_L6_MidC"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("Near_L6_MidC"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidC"), 1.0f));

            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("6,5,8"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T11_MidM"), enemyManager.GetWaypoint("5,6,8"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T9_MidM"), enemyManager.GetWaypoint("3,6,8"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T7_MidM"), enemyManager.GetWaypoint("1,6,8"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T7_MidM"), enemyManager.GetWaypoint("0,5,8"), 1.0f));

            enemy5.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));
            enemy6.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));
            enemy7.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy8.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));
            enemy9.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            enemy5.AddCommand(new WaitAction(3.0f));
            enemy6.AddCommand(new WaitAction(3.0f));
            enemy7.AddCommand(new WaitAction(3.0f));
            enemy8.AddCommand(new WaitAction(3.0f));
            enemy9.AddCommand(new WaitAction(3.0f));

            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,5,8"), enemyManager.GetWaypoint("Far_T6"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,6,8"), enemyManager.GetWaypoint("Far_T5"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("3,6,8"), enemyManager.GetWaypoint("Far_T3"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("1,6,8"), enemyManager.GetWaypoint("Far_T1"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("0,5,8"), enemyManager.GetWaypoint("Far_T0"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentFour_B.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_T11"), enemy1, "Enemy_01"));
            segmentFour_B.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Far_T9"), enemy2, "Enemy_01"));
            segmentFour_B.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Far_T8"), enemy3, "Enemy_01"));
            segmentFour_B.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Far_T7"), enemy4, "Enemy_01"));

            segmentFour_B.AddSpawnAction(new SpawnAction(2.75f, enemyManager.GetWaypoint("Far_T6"), enemy5, "Enemy_01"));
            segmentFour_B.AddSpawnAction(new SpawnAction(3.0f, enemyManager.GetWaypoint("Near_L6_Start"), enemy6, "Enemy_01"));
            segmentFour_B.AddSpawnAction(new SpawnAction(3.25f, enemyManager.GetWaypoint("Near_L6_Start"), enemy7, "Enemy_01"));
            segmentFour_B.AddSpawnAction(new SpawnAction(3.5f, enemyManager.GetWaypoint("Near_L6_Start"), enemy8, "Enemy_01"));
            segmentFour_B.AddSpawnAction(new SpawnAction(3.75f, enemyManager.GetWaypoint("Near_L5_Start"), enemy9, "Enemy_01"));
            encounterManager.AddEncounter(segmentFour_B);
        }

        void SegmentFour_C()
        {
            //calculate firerate in bullet/second
            //fire_rate = 1 / fire_rate;

            //encounterManager = GetComponent<EncounterManager>();
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentFour_C = new Encounter(0.0f, 11.25f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L1_Start"), 7.5f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_Start"), enemyManager.GetWaypoint("Near_L1_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("Near_L2_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_Start"), enemyManager.GetWaypoint("Near_L2_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_Start"), enemyManager.GetWaypoint("Near_L3_MidM"), 1.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("10,1,5"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("8,1,5"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("6,1,5"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("11,2,5"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("9,2,5"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("12,3,5"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L3_MidM"), enemyManager.GetWaypoint("10,3,5"), 1.0f));

            enemy1.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy3.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy4.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy5.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy6.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy7.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            enemy1.AddCommand(new WaitAction(3.0f));
            enemy2.AddCommand(new WaitAction(3.0f));
            enemy3.AddCommand(new WaitAction(3.0f));
            enemy4.AddCommand(new WaitAction(3.0f));
            enemy5.AddCommand(new WaitAction(3.0f));
            enemy6.AddCommand(new WaitAction(3.0f));
            enemy7.AddCommand(new WaitAction(3.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,1,5"), enemyManager.GetWaypoint("Far_T10_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("8,1,5"), enemyManager.GetWaypoint("Far_T8_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,1,5"), enemyManager.GetWaypoint("Far_T6_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("11,2,5"), enemyManager.GetWaypoint("Far_T11_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,2,5"), enemyManager.GetWaypoint("Far_T9_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,3,5"), enemyManager.GetWaypoint("Far_T12_MidM"), 1.0f));
            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,3,5"), enemyManager.GetWaypoint("Far_T10_MidM"), 1.0f));

            //
            // Part B
            //
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L6_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L7_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy13 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L7_Start"), 7.5f, 50.0f);

            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("Near_L6_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_Start"), enemyManager.GetWaypoint("Near_L6_MidM"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L7_Start"), enemyManager.GetWaypoint("Near_L7_MidM"), 1.0f));
            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L7_Start"), enemyManager.GetWaypoint("Near_L7_MidM"), 1.0f));

            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("12,5,5"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("10,5,5"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_MidM"), enemyManager.GetWaypoint("13,6,5"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_MidM"), enemyManager.GetWaypoint("11,6,5"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L7_MidM"), enemyManager.GetWaypoint("14,7,5"), 1.0f));
            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L7_MidM"), enemyManager.GetWaypoint("12,7,5"), 1.0f));

            enemy8.AddCommand(new AttackAction("Shoot_H_2", Enemy1_fire_rate, 2, 2.0f));
            enemy9.AddCommand(new AttackAction("Shoot_H_1", Enemy1_fire_rate, 2, 2.0f));
            enemy10.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy11.AddCommand(new AttackAction("Burst_Left_M", Enemy1_fire_rate, 2, 1.5f));
            enemy12.AddCommand(new AttackAction("Burst_Left_M", Enemy1_fire_rate, 2, 1.5f));
            enemy13.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));

            enemy8.AddCommand(new WaitAction(3.0f));
            enemy9.AddCommand(new WaitAction(3.0f));
            enemy10.AddCommand(new WaitAction(3.0f));
            enemy11.AddCommand(new WaitAction(3.0f));
            enemy12.AddCommand(new WaitAction(3.0f));
            enemy13.AddCommand(new WaitAction(3.0f));

            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,5,5"), enemyManager.GetWaypoint("Far_T12_MidM"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,5,5"), enemyManager.GetWaypoint("Far_T10_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("13,6,5"), enemyManager.GetWaypoint("Far_T13_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("11,6,5"), enemyManager.GetWaypoint("Far_T11_MidM"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("14,7,5"), enemyManager.GetWaypoint("Far_T14_MidM"), 1.0f));
            enemy13.AddCommand(new MoveAction(enemyManager.GetWaypoint("12,7,5"), enemyManager.GetWaypoint("Far_T12_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentFour_C.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_L1_Start"), enemy1, "Enemy_01"));
            segmentFour_C.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_L1_Start"), enemy2, "Enemy_01"));
            segmentFour_C.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_L1_Start"), enemy3, "Enemy_01"));
            segmentFour_C.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_L2_Start"), enemy4, "Enemy_01"));
            segmentFour_C.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Near_L2_Start"), enemy5, "Enemy_01"));
            segmentFour_C.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Near_L3_Start"), enemy6, "Enemy_01"));
            segmentFour_C.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Near_L3_Start"), enemy7, "Enemy_01"));

            segmentFour_C.AddSpawnAction(new SpawnAction(3.0f, enemyManager.GetWaypoint("Near_L5_Start"), enemy8, "Enemy_01"));
            segmentFour_C.AddSpawnAction(new SpawnAction(3.25f, enemyManager.GetWaypoint("Near_L5_Start"), enemy9, "Enemy_01"));
            segmentFour_C.AddSpawnAction(new SpawnAction(3.25f, enemyManager.GetWaypoint("Near_L6_Start"), enemy10, "Enemy_01"));
            segmentFour_C.AddSpawnAction(new SpawnAction(3.5f, enemyManager.GetWaypoint("Near_L6_Start"), enemy11, "Enemy_01"));
            segmentFour_C.AddSpawnAction(new SpawnAction(3.5f, enemyManager.GetWaypoint("Near_L7_Start"), enemy12, "Enemy_01"));
            segmentFour_C.AddSpawnAction(new SpawnAction(3.75f, enemyManager.GetWaypoint("Near_L7_Start"), enemy13, "Enemy_01"));

            encounterManager.AddEncounter(segmentFour_C);
        }

        void SegmentFour_D()
        {
            //calculate firerate in bullet/second
            //fire_rate = 1 / fire_rate;

            //encounterManager = GetComponent<EncounterManager>();
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentFour_D = new Encounter(0.0f, 11.25f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T5_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Near_L5_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Near_R5_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Near_T9_Start"), 7.5f, 50.0f);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T7"), 8.0f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T5_Start"), enemyManager.GetWaypoint("Near_T5_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_Start"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_Start"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T9_Start"), enemyManager.GetWaypoint("Near_T9_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T7"), enemyManager.GetWaypoint("Near_T7_MidM"), 1.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T5_MidM"), enemyManager.GetWaypoint("5,3,8"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("6,5,8"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("8,5,8"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T9_MidM"), enemyManager.GetWaypoint("9,3,8"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T7_MidM"), enemyManager.GetWaypoint("7,2,8"), 1.0f));

            enemy1.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy2.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy3.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy4.AddCommand(new AttackAction("Burst_Front", Enemy1_fire_rate, 2, 1.5f));
            enemy5.AddCommand(new AttackAction("Shoot_D_1", 0, 2, 4.0f));

            enemy1.AddCommand(new WaitAction(3.0f));
            enemy2.AddCommand(new WaitAction(3.0f));
            enemy3.AddCommand(new WaitAction(3.0f));
            enemy4.AddCommand(new WaitAction(3.0f));
            enemy5.AddCommand(new WaitAction(3.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,3,8"), enemyManager.GetWaypoint("Far_T5_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,5,8"), enemyManager.GetWaypoint("Far_T6_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("8,5,8"), enemyManager.GetWaypoint("Far_T8_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,3,8"), enemyManager.GetWaypoint("Far_T9_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("7,2,8"), enemyManager.GetWaypoint("Far_T7_MidM"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentFour_D.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Near_T5_Start"), enemy1, "Enemy_01"));
            segmentFour_D.AddSpawnAction(new SpawnAction(0.25f, enemyManager.GetWaypoint("Near_L5_Start"), enemy2, "Enemy_01"));
            segmentFour_D.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Near_R5_Start"), enemy3, "Enemy_01"));
            segmentFour_D.AddSpawnAction(new SpawnAction(0.75f, enemyManager.GetWaypoint("Near_T9_Start"), enemy4, "Enemy_01"));
            segmentFour_D.AddSpawnAction(new SpawnAction(1.25f, enemyManager.GetWaypoint("Far_T7"), enemy5, "Enemy_03"));

            encounterManager.AddEncounter(segmentFour_D);
        }

        void SegmentFour_E()
        {
            //calculate firerate in bullet/second
            //fire_rate = 1 / fire_rate;

            //encounterManager = GetComponent<EncounterManager>();
            EnemyManager enemyManager = encounterManager.enemyManager;
            Encounter segmentFour_E = new Encounter(0.0f, 11.0f, enemyManager);

            //
            // Part A
            //
            EnemyTemplate enemy1 = new EnemyTemplate(enemyManager.GetWaypoint("Far_L1"), 10.0f, 50.0f);
            EnemyTemplate enemy2 = new EnemyTemplate(enemyManager.GetWaypoint("Far_L6"), 10.0f, 50.0f);
            EnemyTemplate enemy3 = new EnemyTemplate(enemyManager.GetWaypoint("Far_R1"), 10.0f, 50.0f);
            EnemyTemplate enemy4 = new EnemyTemplate(enemyManager.GetWaypoint("Far_R6"), 10.0f, 50.0f);
            EnemyTemplate enemy5 = new EnemyTemplate(enemyManager.GetWaypoint("Far_L2"), 10.0f, 50.0f);
            EnemyTemplate enemy6 = new EnemyTemplate(enemyManager.GetWaypoint("Far_L5"), 10.0f, 50.0f);

            EnemyTemplate enemy7 = new EnemyTemplate(enemyManager.GetWaypoint("Far_R2"), 10.0f, 50.0f);
            EnemyTemplate enemy8 = new EnemyTemplate(enemyManager.GetWaypoint("Far_R5"), 10.0f, 50.0f);
            EnemyTemplate enemy9 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T6"), 10.0f, 50.0f);
            EnemyTemplate enemy10 = new EnemyTemplate(enemyManager.GetWaypoint("Far_L4"), 10.0f, 50.0f);
            EnemyTemplate enemy11 = new EnemyTemplate(enemyManager.GetWaypoint("Far_T8"), 10.0f, 50.0f);
            EnemyTemplate enemy12 = new EnemyTemplate(enemyManager.GetWaypoint("Far_R4"), 10.0f, 50.0f);

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_L1"), enemyManager.GetWaypoint("Near_L1_MidM"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_L6"), enemyManager.GetWaypoint("Near_L6_MidM"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_R1"), enemyManager.GetWaypoint("Near_R1_MidM"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_R6"), enemyManager.GetWaypoint("Near_R6_MidM"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_L2"), enemyManager.GetWaypoint("Near_L2_MidM"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_L5"), enemyManager.GetWaypoint("Near_L5_MidM"), 1.0f));

            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_R2"), enemyManager.GetWaypoint("Near_R2_MidM"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_R5"), enemyManager.GetWaypoint("Near_R5_MidM"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T6"), enemyManager.GetWaypoint("Near_T6_MidM"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_L4"), enemyManager.GetWaypoint("Near_L4_MidM"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_T8"), enemyManager.GetWaypoint("Near_T8_MidM"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Far_R4"), enemyManager.GetWaypoint("Near_R4_MidM"), 1.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L1_MidM"), enemyManager.GetWaypoint("4,1,5"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L6_MidM"), enemyManager.GetWaypoint("4,6,5"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R1_MidM"), enemyManager.GetWaypoint("10,1,5"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R6_MidM"), enemyManager.GetWaypoint("10,6,5"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L2_MidM"), enemyManager.GetWaypoint("5,2,5"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L5_MidM"), enemyManager.GetWaypoint("5,5,5"), 1.0f));

            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R2_MidM"), enemyManager.GetWaypoint("9,2,5"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R5_MidM"), enemyManager.GetWaypoint("9,5,5"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T6_MidM"), enemyManager.GetWaypoint("6,3,5"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_L4_MidM"), enemyManager.GetWaypoint("6,4,5"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_T8_MidM"), enemyManager.GetWaypoint("8,3,5"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("Near_R4_MidM"), enemyManager.GetWaypoint("8,4,5"), 1.0f));

            enemy1.AddCommand(new AttackAction("Shoot_D_1", 0, 2, 4.0f));
            enemy2.AddCommand(new AttackAction("Shoot_D_1", 0, 2, 4.0f));
            enemy3.AddCommand(new AttackAction("Shoot_D_1", 0, 2, 4.0f));
            enemy4.AddCommand(new AttackAction("Shoot_D_1", 0, 2, 4.0f));
            enemy5.AddCommand(new AttackAction("Shoot_D_1", 0, 2, 4.0f));
            enemy6.AddCommand(new AttackAction("Shoot_D_1", 0, 2, 4.0f));

            enemy7.AddCommand(new AttackAction("Shoot_D_1", 0, 2, 4.0f));
            enemy8.AddCommand(new AttackAction("Shoot_D_1", 0, 2, 4.0f));
            enemy9.AddCommand(new AttackAction("Shoot_D_1", 0, 2, 4.0f));
            enemy10.AddCommand(new AttackAction("Shoot_D_1", 0, 2, 4.0f));
            enemy11.AddCommand(new AttackAction("Shoot_D_1", 0, 2, 4.0f));
            enemy12.AddCommand(new AttackAction("Shoot_D_1", 0, 2, 4.0f));

            enemy1.AddCommand(new WaitAction(3.0f));
            enemy2.AddCommand(new WaitAction(3.0f));
            enemy3.AddCommand(new WaitAction(3.0f));
            enemy4.AddCommand(new WaitAction(3.0f));
            enemy5.AddCommand(new WaitAction(3.0f));
            enemy6.AddCommand(new WaitAction(3.0f));

            enemy7.AddCommand(new WaitAction(3.0f));
            enemy8.AddCommand(new WaitAction(3.0f));
            enemy9.AddCommand(new WaitAction(3.0f));
            enemy10.AddCommand(new WaitAction(3.0f));
            enemy11.AddCommand(new WaitAction(3.0f));
            enemy12.AddCommand(new WaitAction(3.0f));

            enemy1.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,1,5"), enemyManager.GetWaypoint("Far_T4"), 1.0f));
            enemy2.AddCommand(new MoveAction(enemyManager.GetWaypoint("4,6,5"), enemyManager.GetWaypoint("Far_T4"), 1.0f));
            enemy3.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,1,5"), enemyManager.GetWaypoint("Far_T10"), 1.0f));
            enemy4.AddCommand(new MoveAction(enemyManager.GetWaypoint("10,6,5"), enemyManager.GetWaypoint("Far_T10"), 1.0f));
            enemy5.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,2,5"), enemyManager.GetWaypoint("Far_T5"), 1.0f));
            enemy6.AddCommand(new MoveAction(enemyManager.GetWaypoint("5,5,5"), enemyManager.GetWaypoint("Far_T5"), 1.0f));

            enemy7.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,2,5"), enemyManager.GetWaypoint("Far_T9"), 1.0f));
            enemy8.AddCommand(new MoveAction(enemyManager.GetWaypoint("9,5,5"), enemyManager.GetWaypoint("Far_T9"), 1.0f));
            enemy9.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,3,5"), enemyManager.GetWaypoint("Far_T6"), 1.0f));
            enemy10.AddCommand(new MoveAction(enemyManager.GetWaypoint("6,4,5"), enemyManager.GetWaypoint("Far_T6"), 1.0f));
            enemy11.AddCommand(new MoveAction(enemyManager.GetWaypoint("8,3,5"), enemyManager.GetWaypoint("Far_T8"), 1.0f));
            enemy12.AddCommand(new MoveAction(enemyManager.GetWaypoint("8,4,5"), enemyManager.GetWaypoint("Far_T8"), 1.0f));

            //
            // GET SPAWN POINTS
            //
            segmentFour_E.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_L1"), enemy1, "Enemy_03"));
            segmentFour_E.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_L6"), enemy2, "Enemy_03"));
            segmentFour_E.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_R1"), enemy3, "Enemy_03"));
            segmentFour_E.AddSpawnAction(new SpawnAction(0.0f, enemyManager.GetWaypoint("Far_R6"), enemy4, "Enemy_03"));
            segmentFour_E.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Far_L2"), enemy5, "Enemy_03"));
            segmentFour_E.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Far_L5"), enemy6, "Enemy_03"));

            segmentFour_E.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Far_R2"), enemy7, "Enemy_03"));
            segmentFour_E.AddSpawnAction(new SpawnAction(0.5f, enemyManager.GetWaypoint("Far_R5"), enemy8, "Enemy_03"));
            segmentFour_E.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Far_T6"), enemy9, "Enemy_03"));
            segmentFour_E.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Far_L4"), enemy10, "Enemy_03"));
            segmentFour_E.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Far_T8"), enemy11, "Enemy_03"));
            segmentFour_E.AddSpawnAction(new SpawnAction(1.0f, enemyManager.GetWaypoint("Far_R4"), enemy12, "Enemy_03"));

            encounterManager.AddEncounter(segmentFour_E);
        }
    }

}
