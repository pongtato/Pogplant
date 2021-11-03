﻿/******************************************************************************/
/*!
\file	EncounterManager.cs
\author Ng Tian Kiat
\par	email: tiankiat.ng\@digipen.edu
\details
    This script provides a way to spawn enemies in a scripted encounter.

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
            or disclosure of this file or its contents without the prior
            written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{

    // This class defines a enemy spawning action
    public class SpawnAction
    {
        public float spawn_time { get; private set; } // At what time this spawn action will activate
        public Transform spawn_location { get; private set; } // Location of the spawn
        public EnemyTemplate enemy_to_spawn { get; private set; } // the template of the enemy to spawn
        public string enemy_prefab_name { get; private set; } // the prefab name of the enemy to spawn
        public bool is_executed { get; set; } // whether this action has been executed

        public SpawnAction(float spawnTime, Transform spawnLocation, EnemyTemplate enemyTemplate, string prefab_name)
        {
            spawn_time = spawnTime;
            spawn_location = spawnLocation;
            enemy_to_spawn = enemyTemplate;
            enemy_prefab_name = prefab_name;

            is_executed = false;
        }
    }

    // This class defines a encounter which is basically a collection of spawn actions
    public class Encounter
    {
        public float start_time { get; private set; } // At what time this encounter will activate
        private float duration; // total duration for this encounter
        private List<SpawnAction> spawn_actions; // a collection of spawn actions for this encounter.

        private float current_time;
        public bool is_executed { get; private set; } // whether this encounter has activated
        private bool is_active;

        private EnemyManager enemyManager;

        public Encounter(float startTime, float totalDuration, EnemyManager manager, List<SpawnAction> actions = null)
        {
            start_time = startTime;
            duration = totalDuration;
            enemyManager = manager;
            spawn_actions = actions;

            if (spawn_actions == null)
                spawn_actions = new List<SpawnAction>();

            is_executed = false;
            is_active = false;
            current_time = 0.0f;
        }

        // This adds a spawn action into the encounter
        public void AddSpawnAction(SpawnAction action)
        {
            spawn_actions.Add(action);
        }

        // initializes the encounter, called when encounter is activated
        public void ExecuteEncounter()
        {
            is_executed = true;
            is_active = true;
            current_time = 0.0f;

            Console.WriteLine("Spawned encounter");
        }

        // updates the encounter
        public void Update(float dt)
        {
            // Update the encounter behaviour if it is active
            if (is_active)
            {
                current_time += dt;

                // Execute spawn actions at the correct time
                foreach (var action in spawn_actions)
                {
                    if (current_time >= action.spawn_time && !action.is_executed)
                    {
                        action.is_executed = true;
                        // Spawn enemy at spawn location here
                        enemyManager.InstantiateEnemy(action.spawn_location, action.enemy_to_spawn, action.enemy_prefab_name);
                    }

                }

                // If encounter is finished deactivate it
                if (current_time >= duration)
                    is_active = false;
            }
        }

        public static int EncounterComparer(Encounter x, Encounter y)
        {
            if (x.start_time == y.start_time)
                return 0;
            else if (x.start_time > y.start_time)
                return 1;
            else
                return -1;
        }
    }

    // This class manages a collection of encounter it handles activating and updating encounters.
    public class EncounterManager : MonoBehaviour
    {
        private List<Encounter> encounters = null;
        public EnemyManager enemyManager { get; private set; }
        private float current_time = 0.0f;
        // Start is called before the first frame update
        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        public override void Start()
        {
            //enemyManager = GetComponent<EnemyManager>();
            encounters = new List<Encounter>();
        }

        // Update is called once per frame
        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            current_time += dt;
            UpdateEncounterTimeline();
            UpdateSpawnedEncounters(dt);
        }

        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

        }

        public void AddEncounter(Encounter encounter)
        {
            encounters.Add(encounter); // Add the client created encounter to list of encounters to manage.
            encounters.Sort(Encounter.EncounterComparer); // Sort the list by start_time so it is in chronological order.
        }

        void UpdateEncounterTimeline()
        {
            foreach (Encounter encounter in encounters)
            {
                // Spawn encounter if it is time and it has not spawned yet
                if (!encounter.is_executed && current_time >= encounter.start_time)
                {
                    encounter.ExecuteEncounter();
                    break;
                }
            }
        }

        void UpdateSpawnedEncounters(float dt)
        {
            foreach (Encounter encounter in encounters)
            {
                encounter.Update(dt);
            }
        }



    }

}