/******************************************************************************/
/*!
\file	EnemyManager.cs
\author Ng Tian Kiat
\par	email: tiankiat.ng\@digipen.edu
\details
    This script is a factory for spawning enemies from a template.

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
            or disclosure of this file or its contents without the prior
            written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
using System;
using System.Collections.Generic;

namespace Scripting
{
    public class EnemyManager
    {

        public static int enemy_counter = 0;

        public Transform parent;

        public GameObject true_bullet_prefab;
        public GameObject false_bullet_prefab;

        private string waypoint_group_name = "enemy_waypoint_group_";

        public List<GameObject> waypoints;

        public GameObject[] turret_true_bullet_pool; // object pool for true bullets.
        public GameObject[] turret_false_bullet_pool; // object pool for false bullets.
        private static Dictionary<string, GameObject> waypoint_map; // a string to vector3 map for waypoints.

        private List<GameObject> enemy_instances = new List<GameObject>();

        private List<uint> enemies_to_delete = new List<uint>();

        public static uint score = 0;                                    // Current score
        private static uint kill_score = 100;                            // Addition to base score everytime an enemy is killed
        private static uint bonus_score = 50;                            // Bonus score
        private static int bonus_count;                                     
        private static int bonus_increment_requirement = 5;              // Increase bonus everytime this amount of enemies is killed
        static uint DashboardScreenID;

        // Start is called before the first frame update
        public void Start()
        {
            // Initialize waypoint dictionary
            waypoints = InitWaypointGroups();
            InitMap();
            DashboardScreenID = ECS.FindEntityWithName("DashboardScreenFace");
        }

        // This function creates waypoints based on the file and parents it to a enemy_waypoint_group object
        List<GameObject> CreateParentedWaypoints(string waypointFile, uint parent)
        {
            string[] data;
            try
            {
                data = System.IO.File.ReadAllLines(waypointFile);

            }
            catch (System.IO.FileNotFoundException e)
            {
                Console.WriteLine("Cannot read Waypoint files! Exception: " + e.Message);
                throw;
            }

            List<GameObject> waypointGroup = new List<GameObject>();

            foreach (string waypointData in data)
            {
                string[] tokens = waypointData.Split(' ');
                string name = tokens[0];

                Transform transform = new Transform();
                transform.Position = new Vector3(-float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
                transform.Rotation = new Vector3(0.0f, 180.0f, 0.0f);
                transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);

                GameObject waypoint_GO = ECS.CreateChild(parent, name, transform);
                //waypoint_GO.AddComponent<Renderer>(new Renderer("sphere"));
                waypointGroup.Add(waypoint_GO);
            }

            return waypointGroup;
        }

        // This function creates the waypoint groups from reading text files
        List<GameObject> InitWaypointGroups()
        {
            List<GameObject> waypointList = new List<GameObject>();
            int numberOfGroups = 5;
            // create filepaths to read the data from
            string directory = "Resources\\";
            string fileName = "GAM300_TakoFactory_Prototype_Enemy_WaypointGroup_";
            string[] fileNames = new string[numberOfGroups];

            for (int i = 0; i < fileNames.Length; ++i)
                fileNames[i] = directory + fileName + "(" + i + ").txt";

            // Find 5 waypoint group parents here
            uint[] parent_ids = new uint[numberOfGroups];
            for (int i = 0; i < numberOfGroups; ++i)
            {
                string parentName = waypoint_group_name + i;
                parent_ids[i] = ECS.FindEntityWithName(parentName);
                waypointList.AddRange(CreateParentedWaypoints(fileNames[i], parent_ids[i]));
            }

            return waypointList;
        }

        // This initializes the dictionary for string to vector3 conversion.
        void InitMap()
        {
            if (waypoints.Count == 0)
            {
                Console.WriteLine("No waypoints assigned!");
                return;
            }

            waypoint_map = new Dictionary<string, GameObject>();

            for (int i = 0; i < waypoints.Count; ++i)
            {
                string code = waypoints[i].name;
                code.Trim(',');
                waypoint_map.Add(code, waypoints[i]);
            }
        }

        // Given a string name of the waypoint this function returns the corresponding local position of the waypoint.
        public GameObject GetWaypoint(string name)
        {
            name.Trim(',');
            GameObject value;
            waypoint_map.TryGetValue(name, out value);
            return value;
        }

        // Given a "true bullet" boolean, this function returns a instance of a true/false bullet.
        public GameObject GetBullet(bool isTrueBullet)
        {
            // This function is deprecated, it is shifted to C++

            Console.WriteLine("[Deprecated] Cannot find anymore bullets in pool!");
            return null;
        }
        //get turret bullets that are not parented
        public GameObject GetTurretBullet(bool isTrueBullet)
        {
            // This function is deprecated, it is shifted to C++

            Console.WriteLine("[Deprecated] Cannot find anymore bullets in pool!");
            return null;
        }

        // Helper function to create an instance from an enemy prefab
        GameObject CreateEnemyInstance(string prefab_name, Transform location)
        {
            GameObject instance = GameUtilities.InstantiateObject(prefab_name, location.Position, location.Rotation);
            enemy_instances.Add(instance);
            return instance;
        }

        // Given a location, enemy template and name of the enemy prefab this function will spawn an instance of an enemy.
        public void InstantiateEnemy(GameObject spawnWaypoint, EnemyTemplate enemy_template, string prefab_object)
        {
            GameObject instance = CreateEnemyInstance(prefab_object, spawnWaypoint.transform);
            //// Set parent here
            ECS.SetTransformParent(instance.id, ECS.GetTransformParent(spawnWaypoint.id));

            Transform transform = instance.GetComponent<Transform>();
            transform.Rotation.Y = 180.0f;
            instance.AddComponent<BaseEnemy>(new BaseEnemy(enemy_template, instance, DashboardScreenID));
            instance.GetComponent<BaseEnemy>().Start();
            instance.GetComponent<BaseEnemy>().SetManager(this);
        }

        public void InstantiateTempEnemy(Transform location, string prefab_object, string parentName)
        {
            GameObject instance = CreateEnemyInstance(prefab_object, location);

            // Set parent here
            uint parent = ECS.FindEntityWithName(parentName);
            ECS.SetTransformParent(instance.id, parent);
        }

        // This function deletes the enemy instance from the managed list,
        // also adds score if the enemy died from player
        public void DeleteEnemyInstance(uint id, bool isDiedFromPlayer)
        {
            if (isDiedFromPlayer)
            {
                AddScore(true);
            }
            enemies_to_delete.Add(id);
        }

        public void Update(float dt)
        {
            foreach (var item in enemy_instances)
                item.GetComponent<BaseEnemy>().Update(dt);

            // Delete the enemies after all updated has been called,
            // To prevent iterator invalidation
            foreach (var go in enemies_to_delete)
                foreach (var item in enemy_instances)
                {
                    if (item.id == go)
                    {
                        enemy_instances.Remove(item);
                        ECS.DestroyEntity(item.id);
                        break;
                    }
                }

            enemies_to_delete.Clear();
        }

        public bool GetAlive(uint id)
        {
            foreach (var item in enemy_instances)
            {
                if (item.id == id)
                    return item.GetComponent<BaseEnemy>().GetAlive();
            }

            DebugUtilities.LogToEditor("EncounterManager", "Enemy with ID: " + id + " could not be found!");
            return false;
        }
        public void TakeDamage(uint id, float damage)
        {
            foreach (var item in enemy_instances)
            {
                if (item.id == id)
                {
                    item.GetComponent<BaseEnemy>().TakeDamage(damage);
                    break;
                }
            }
        }

        public static void AddScore(bool increment)
        {
            //Increase score
            if (increment)
            {
                ++bonus_count;
                if (bonus_count >= bonus_increment_requirement && PlayerScript.m_ComboActive)
                {
                    Console.WriteLine("Combo Activated");
                    bonus_count = 0;
                    score += bonus_score;
                    bonus_score += 10;
                }
                score += kill_score * PlayerScript.m_ScoreMultiplierBobbleCount;
                PlayerScript.m_ComboDecayTimer = 0.0f;
                //GameUtilities.UpdateDashboardFace(DashboardScreenID, 2);
            }
            //Decrease score
            else
            {
                bonus_count = 0;
                if (score > 10)
                {
                    score -= 10;
                }
            }

            GameUtilities.UpdateScore(ECS.FindEntityWithName("Score_Text"), score);
        }
    }
}
