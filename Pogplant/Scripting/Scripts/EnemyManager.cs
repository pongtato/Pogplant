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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class EnemyManager : MonoBehaviour
    {
        //singleton
        public static EnemyManager Instance;

        public static int enemy_counter = 0;

        public Transform parent;

        public GameObject true_bullet_prefab;
        public GameObject false_bullet_prefab;

        public Transform[] waypoints;

        private GameObject[] true_bullet_pool; // object pool for true bullets.
        private GameObject[] false_bullet_pool; // object pool for false bullets.
        public GameObject[] turret_true_bullet_pool; // object pool for true bullets.
        public GameObject[] turret_false_bullet_pool; // object pool for false bullets.
        private Dictionary<string, Transform> waypoint_map; // a string to vector3 map for waypoints.

        private List<GameObject> enemy_instances = new List<GameObject>();

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        // Start is called before the first frame update
        public override void Start()
        {
            // Initialize waypoint dictionary
            InitMap();

            // Initialize bullet pools
            const int pool_size = 500;
            true_bullet_pool = new GameObject[pool_size];
            false_bullet_pool = new GameObject[pool_size];

            //GameObject folder = new GameObject("Enemy_Projectile_Folder");
            ////folder.transform.SetParent(parent);
            //for (int i = 0; i < pool_size; ++i)
            //{
            //    true_bullet_pool[i] = Instantiate(true_bullet_prefab, folder.transform);
            //    true_bullet_pool[i].SetActive(false);
            //}

            //for (int i = 0; i < pool_size; ++i)
            //{
            //    false_bullet_pool[i] = Instantiate(false_bullet_prefab, folder.transform);
            //    false_bullet_pool[i].SetActive(false);
            //}


            ////InitMap turret bullets
            //turret_true_bullet_pool = new GameObject[pool_size];
            //turret_false_bullet_pool = new GameObject[pool_size];

            //for (int i = 0; i < pool_size; ++i)
            //{
            //    turret_true_bullet_pool[i] = Instantiate(true_bullet_prefab);
            //    turret_true_bullet_pool[i].SetActive(false);
            //}

            //for (int i = 0; i < pool_size; ++i)
            //{
            //    turret_false_bullet_pool[i] = Instantiate(false_bullet_prefab);
            //    turret_false_bullet_pool[i].SetActive(false);
            //}

            Instance = this;
        }

        // This initializes the dictionary for string to vector3 conversion.
        void InitMap()
        {
            if (waypoints.Length == 0)
            {
                Console.WriteLine("No waypoints assigned!");
                return;
            }

            waypoint_map = new Dictionary<string, Transform>();

            for (int i = 0; i < waypoints.Length; ++i)
            {
                //string code = waypoints[i].name;
                //code.Trim(',');
                //waypoint_map.Add(code, waypoints[i]);
            }
        }

        // Given a string name of the waypoint this function returns the corresponding local position of the waypoint.
        public Transform GetWaypoint(string name)
        {
            name.Trim(',');
            Transform value;
            waypoint_map.TryGetValue(name, out value);
            return value;
        }

        // Given a "true bullet" boolean, this function returns a instance of a true/false bullet.
        public GameObject GetBullet(bool isTrueBullet)
        {
            if (isTrueBullet)
            {
                for (int i = 0; i < true_bullet_pool.Length; ++i)
                {
                    //find inactive bullet to shoot
                    //if (!true_bullet_pool[i].activeSelf)
                    //    return true_bullet_pool[i];
                }
            }
            else
            {
                for (int i = 0; i < false_bullet_pool.Length; ++i)
                {
                    //find inactive bullet to shoot
                    //if (!false_bullet_pool[i].activeSelf)
                    //    return false_bullet_pool[i];
                }
            }
            Console.WriteLine("Cannot find anymore bullets in pool!");
            return null;
        }
        //get turret bullets that are not parented
        public GameObject GetTurretBullet(bool isTrueBullet)
        {
            if (isTrueBullet)
            {
                for (int i = 0; i < turret_true_bullet_pool.Length; ++i)
                {
                    //find inactive bullet to shoot
                    //if (!turret_true_bullet_pool[i].activeSelf)
                    //    return turret_true_bullet_pool[i];
                }
            }
            else
            {
                for (int i = 0; i < turret_false_bullet_pool.Length; ++i)
                {
                    //find inactive bullet to shoot
                    //if (!turret_false_bullet_pool[i].activeSelf)
                    //    return turret_false_bullet_pool[i];
                }
            }
            Console.WriteLine("Cannot find anymore bullets in pool!");
            return null;
        }

        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

        }
        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        { }
        GameObject CreateEnemyInstance(Transform location)
        {
            GameObject instance = ECS.CreateEntity("Enemy (" + enemy_counter + ")", location);

            instance.AddComponent<Renderer>(new Renderer("Enemy_01"));
            instance.AddComponent<Rigidbody>(new Rigidbody());
            //instance.AddComponent<BaseEnemy>(new BaseEnemy());

            enemy_instances.Add(instance);
            return instance;
        }

        // Given a location, enemy template and name of the enemy prefab this function will spawn an instance of an enemy.
        public void InstantiateEnemy(Transform location, EnemyTemplate enemy_template, string prefab_object)
        {
            //GameObject prefab = Resources.Load(prefab_object) as GameObject;
            //GameObject instance = Instantiate(prefab, parent, false);
            GameObject instance = CreateEnemyInstance(location);
            Transform transform = instance.GetComponent<Transform>();
            transform.Position = location.Position;
            transform.Rotation = new Vector3(0, 180, 0);
            //BaseEnemy comp = instance.GetComponent<BaseEnemy>();
            //comp.SetTemplate(enemy_template);
            //comp.SetManager(this);
        }

        public void DeleteEnemyInstance(uint id)
        {
            foreach (var item in enemy_instances)
            {
                if (item.id == id)
                {
                    enemy_instances.Remove(item);
                    return;
                }
            }
        }

    }

}
