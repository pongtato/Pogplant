using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class L1BossCloudMover : MonoBehaviour
    {
        List<uint> cloud_list;
        List<uint> moving_cloud_list;
        const float cloud_move_speed = 30.0f;
        const float reset_distance = 150.0f;
        float[] arr_cloud_spawn_Z;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            cloud_list = new List<uint>();
            moving_cloud_list = new List<uint>();

            uint cloud_1a = ECS.FindEntityWithName("Cloud_Prefab_01a");
            uint cloud_2a = ECS.FindEntityWithName("Cloud_Prefab_02a");
            uint cloud_3a = ECS.FindEntityWithName("Cloud_Prefab_03a");
            uint cloud_1b = ECS.FindEntityWithName("Cloud_Prefab_01b");
            uint cloud_2b = ECS.FindEntityWithName("Cloud_Prefab_02b");
            uint cloud_3b = ECS.FindEntityWithName("Cloud_Prefab_03b");

            cloud_list.Add(cloud_1a);
            cloud_list.Add(cloud_2a);
            cloud_list.Add(cloud_3a);
            cloud_list.Add(cloud_1b);
            cloud_list.Add(cloud_2b);
            cloud_list.Add(cloud_3b);

            arr_cloud_spawn_Z = new float[3] { -65.0f, -220.0f, -420.0f };

            PickRandomCloud(3);

            for (int i = 0; i < moving_cloud_list.Count; ++i)
            {
                ECS.SetGlobalPosition(moving_cloud_list[i], new Vector3(0, -50, arr_cloud_spawn_Z[i]));
            }
        }
        public override void Start()
        {

        }
        public override void Update(float dt)
        {
            //Move the current 3 clouds
            for (int i = 0; i < moving_cloud_list.Count; ++i)
            {
                ECS.SetGlobalPosition(moving_cloud_list[i], new Vector3(0, -50, ECS.GetGlobalPosition(moving_cloud_list[i]).Z + cloud_move_speed * dt));

                //Pick another random cloud each time a cloud hits the reset range 
                if (ECS.GetGlobalPosition(moving_cloud_list[i]).Z >= reset_distance)
                {
                    cloud_list.Add(moving_cloud_list[i]);
                    moving_cloud_list.Remove(moving_cloud_list[i]);
                    PickRandomCloud(1);
                    ResetClouds(moving_cloud_list.Count -1 );
                    break;
                }
            }
        }

        void PickRandomCloud(int cloud_count)
        {
            if (cloud_count < cloud_list.Count - 1)
            {
                //Pick 3 random clouds and set them near the boss
                for (int i = 0; i < cloud_count; ++i)
                {
                    Random rand = new Random();
                    int index_shift = rand.Next(0, cloud_list.Count - 1);

                    moving_cloud_list.Add(cloud_list[index_shift]);
                    cloud_list.Remove(cloud_list[index_shift]);
                }
            }
        }

        void ResetClouds(int index)
        {
            ECS.SetGlobalPosition(moving_cloud_list[index], new Vector3(0, -50, arr_cloud_spawn_Z[arr_cloud_spawn_Z.Length - 1]));
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
    }
}
