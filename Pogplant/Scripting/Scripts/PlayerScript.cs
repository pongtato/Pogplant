/*****************************************************************************/
/*!
\file	PlayerScript.cs
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	PlayerScript class for scripting c# project

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
    // Player script class
    public class PlayerScript : MonoBehaviour
    {

        public float movement_speed = 20000.0f;
        private float horizontal_input = 0;
        private float vertical_input = 0;
        private float slowForce = 2.0f;
        private float maxSpeed = 10.0f;

        //Player Firing 
        float p_fireRate;
        float p_fire_timer = 0.0f;
        //Player Rotation movement (Camera duplicates this effect)
        float camera_roll = -1.5f;

        public float max_rotate_angle = 12.5f;
        public float rotation_speed_horizontal = 1.0f;
        public float rotation_speed_vertical = 1.0f; 
        public float revert_speed = 2.0f; // changed from 20
        public float max_dampening_speed = 100.0f;

        public float dampening_reduction = 1.0f;
        public float dampening_modifier = 4.0f;

        private float current_vertical_dampening;
        private float current_horizontal_dampening;

        private float timeCount;
        public float ship_follow_rot_speed;


        //// THIS IS ONLY FOR TESTING + EXAMPLE PURPOSES
        //public List<uint> entityIDList = new List<uint>();
        //// parentID first, childID second
        //public List<Tuple<uint, uint>> childIDList = new List<Tuple<uint, uint>>();

        public List<GameObject> entityList = new List<GameObject>();
        public List<Tuple<GameObject, GameObject>> childList = new List<Tuple<GameObject, GameObject>>();
        EnemyManager enemyManager = new EnemyManager();

        public PlayerScript()
        {
            p_fireRate = 1 / 5.0f;


        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        public override void Start()
        {
        }

        public void Awake()
        {
        }

        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            //if(transform.Position.Y >= 10.0f)
            //{
            //    rigidbody.AddForce(new Vector3(0, -3.0f, 0));
            //}

            //if(transform.Position.Y < 10.0f)
            //{
            //    rigidbody.AddForce(new Vector3(0, 3.0f, 0));
            //}

            // Fake enemy encounter press Y to spawn random enemy infront of player
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_Y))
            {
                var rand = new Random();
                Transform location = new Transform(Vector3.Zero(), Vector3.Zero(), Vector3.One());
                //location.Position = ECS.GetGlobalPosition(entityID);
                //location.Position.X += (((float)rand.Next() / int.MaxValue) * 10.0f - 5.0f);
                //location.Position.Y += (((float)rand.Next() / int.MaxValue) * 10.0f - 5.0f);
                location.Position.Z = 30.0f;
                location.Rotation.Y = 180.0f;

                enemyManager.InstantiateTempEnemy(location, "Enemy", "PlayerBox");
            }

            if (InputUtility.onKeyTriggered(KEY_ID.KEY_D))
            {
                //Console.WriteLine("D key is triggered");
            }

            if (InputUtility.onKeyTriggered(KEY_ID.KEY_D))
            {
                //Console.WriteLine("D key is triggered");
            }

            // Create Entity Example
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_Z))
            {
                //uint entityID = ECS.CreateEntity("Testing123", Vector3.Zero(), Vector3.Zero(), Vector3.Zero());
                //entityIDList.Add(entityID);

                GameObject GO = ECS.CreateEntity("RandomNamexD", new Transform(Vector3.One(), Vector3.One(), Vector3.One()));
                GO.AddComponent<Renderer>(new Renderer("Player_Ship"));

                entityList.Add(GO);

                Console.WriteLine("Entity ID created: " + GO.id);
                //Console.WriteLine("Test Mass Is: " + GO.GetComponent<Rigidbody>().mass);
            }

            // Destroy Entity Example
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_X))
            {
                if (entityList.Count > 0)
                {
                    uint entity = entityList[0].id;
                    Console.WriteLine("Trying Entity ID destroyed: " + entity);
                    ECS.DestroyEntity(entity);
                    entityList.RemoveAt(0);
                    Console.WriteLine("Entity ID destroyed: " + entity);
                }
                else
                {
                    Console.WriteLine("Entity list is empty");
                }
            }

            // Create Child Example
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_C))
            {
                if (entityList.Count > 0)
                {
                    GameObject GOParent = entityList[0];
                    GameObject GOChild = ECS.CreateChild(GOParent.id, "Child123", new Transform(Vector3.Zero(), Vector3.Zero(), Vector3.Zero()));
                    childList.Add(new Tuple<GameObject, GameObject>(GOParent, GOChild));
                    Console.WriteLine("Child ID created: " + GOChild.id + " Parent ID: " + GOParent.id);
                }
                else
                {
                    Console.WriteLine("Entity list is empty");
                }
            }
            
            // Get EntityID example
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_V))
            {
                string exampleName = "RandomNamexD";
                uint entityID = ECS.FindEntityWithName(exampleName);
                Console.WriteLine("EntityID with name " + exampleName + ": " + entityID);
            }

            // Get component example
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_B))
            {
                if (entityList.Count > 0)
                {
                    //Console.WriteLine("Test Mass Is: " + entityList[0].GetComponent<Rigidbody>().mass);
                    //Console.WriteLine("Test Model Name Is: " + entityList[0].GetComponent<Renderer>().modelName);
                    Transform transform1 = ECS.GetComponent<Transform>(entityList[0].id);
                    Console.WriteLine("Test Transforms are: " + transform1.Position.X + transform1.Position.Y + transform1.Position.Z);
                }
                else 
                {
                    Console.WriteLine("Entity list is empty");
                }
            }

            // Add component example
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_N))
            {
                if(entityList.Count > 0)
                {
                    entityList[0].AddComponent<Rigidbody>(new Rigidbody(new Vector3()));
                    entityList[0].AddComponent<Renderer>(new Renderer("Player_Ship"));
                    entityList[0].AddComponent<SphereCollider>(new SphereCollider());
                    entityList[0].AddComponent<BoxCollider>(new BoxCollider());
                }
                else
                {
                    Console.WriteLine("Entity list is empty");
                }
            }

            if (InputUtility.onKeyTriggered(KEY_ID.KEY_M))
            {
                GameObject GO = ECS.CreateEntity("Player2", new Transform(Vector3.One(), Vector3.One(), Vector3.One()));
                GO.AddComponent<Rigidbody>(new Rigidbody(new Vector3()));
                GO.AddComponent<Renderer>(new Renderer("Player_Ship"));
                GO.AddComponent<PlayerScript>(new PlayerScript());

                //Console.WriteLine("S key is released");
            }


            horizontal_input = (InputUtility.onKeyHeld("RIGHT") ? -1.0f : 0.0f) + (InputUtility.onKeyHeld("LEFT") ? 1.0f : 0.0f);
            vertical_input = (InputUtility.onKeyHeld("UP") ? 1.0f : 0.0f) + (InputUtility.onKeyHeld("DOWN") ? -1.0f : 0.0f);

            //if(rigidbody.velocity.X < maxSpeed && horizontal_input > 0.0f || rigidbody.velocity.X > -maxSpeed && horizontal_input < 0.0f)
            //{
            //    rigidbody.AddForce(new Vector3(horizontal_input, 0.0f, 0.0f));
            //}
            //transform.Position += force_dir;
            Vector3 direc_vector = new Vector3(-horizontal_input, vertical_input, 0);
            direc_vector = Vector3.Normalise(direc_vector);

            //Vector3 force_dir = direc_vector * movement_speed * dt; // 32 is magic number
            //rigidbody.AddForce(force_dir);

            switch(GameUtilities.CheckBounds(transform.Position, Vector3.Normalise(rigidbody.velocity) + direc_vector))
            {
                //Out of X bounds
                case 1:
                    {
                        //float newX = transform.Position.X - force_dir.X;
                        //transform.Position = new Vector3(newX, transform.Position.Y, transform.Position.Z);
                        rigidbody.velocity.X = 0f;
                        direc_vector.X = 0;
                        Vector3 y_force_dir = direc_vector * movement_speed * dt; // 32 is magic number
                        rigidbody.AddForce(y_force_dir);
                        break;
                    }
                case 2:
                    {
                        //float newY = transform.Position.Y - force_dir.Y;
                        //transform.Position = new Vector3(transform.Position.X, newY, transform.Position.Z);
                        rigidbody.velocity.Y = 0f;
                        direc_vector.Y = 0;
                        Vector3 x_force_dir = direc_vector * movement_speed * dt; // 32 is magic number
                        rigidbody.AddForce(x_force_dir);
                        break;
                    }
                case 3:
                    {
                        //float newX = transform.Position.X - force_dir.X;
                        //float newY = transform.Position.Y - force_dir.Y;
                        //transform.Position = new Vector3(newX, newY, transform.Position.Z);
                        rigidbody.velocity.X = 0f;
                        rigidbody.velocity.Y = 0f;
                        direc_vector.X = 0;
                        direc_vector.Y = 0;
                        break;
                    }
                default:
                    {
                        Vector3 force_dir = direc_vector * movement_speed * dt; // 32 is magic number
                        rigidbody.AddForce(force_dir);
                        break;
                    }
            }


            float maxslowforce = rigidbody.velocity.magnitude();
            if (Math.Abs(maxslowforce) <= float.Epsilon)
                maxslowforce = 0.0f;
            else
            {
                maxslowforce = 1 / maxslowforce;

                Vector3 SlowDownVec = rigidbody.velocity * maxslowforce;
                rigidbody.velocity -= SlowDownVec * Math.Min(maxslowforce, (1 / maxslowforce) * slowForce * dt);
            }
        }

        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            Vector3 Rotation_Concat = Vector3.Zero();

            float adjustedAngle = transform.Rotation.X > 180.0f ? transform.Rotation.X - 360.0f : transform.Rotation.X;
            current_vertical_dampening = max_rotate_angle / (Math.Abs(adjustedAngle) + (dampening_reduction * dampening_modifier)) * 2f;
            if ((adjustedAngle > -max_rotate_angle && vertical_input > 0) || (adjustedAngle < max_rotate_angle && vertical_input < 0))
            {
                Vector3 Rotatation_Vertical = new Vector3((max_rotate_angle * vertical_input * -current_vertical_dampening * rotation_speed_vertical * dt), 0.0f, 0.0f);
                Rotation_Concat += Rotatation_Vertical;
                timeCount = 0;
            }

            adjustedAngle = transform.Rotation.Y > 180.0f ? transform.Rotation.Y - 360.0f : transform.Rotation.Y;
            current_horizontal_dampening = max_rotate_angle / (Math.Abs(adjustedAngle) + (dampening_reduction * dampening_modifier));

            if ((adjustedAngle > -max_rotate_angle && horizontal_input < 0) || (adjustedAngle < max_rotate_angle && horizontal_input > 0))
            {
                Vector3 Rotatation_Horizontal = new Vector3(0.0f, (max_rotate_angle * horizontal_input * current_horizontal_dampening * rotation_speed_horizontal * dt), 0.0f);
                Rotation_Concat += Rotatation_Horizontal;
                timeCount = 0;
            }

            transform.Rotation += Rotation_Concat;
            float adjustedRoll = (transform.Rotation.Y > 180.0f ? transform.Rotation.Y - 360.0f : transform.Rotation.Y) * camera_roll;
            transform.Rotation = new Vector3(transform.Rotation.X, transform.Rotation.Y, adjustedRoll);


            //timeCount += dt * ship_follow_rot_speed;
            //ship_transform.localRotation = Vector3.Lerp(ship_transform.localRotation, transform.localRotation, timeCount);

            //if (vertical_input == 0.0f && horizontal_input == 0.0f)
            //{
            //    //Harcoded for now, if the box changes the rotation then i will edit this
            //    transform.Rotation = Vector3.RotateTowards(transform.Rotation, new Vector3(0.0f,0.0f,0.01f), revert_speed * dt);
            //}


            //m_Distance = Vector3.Distance(FollowTarget.transform.position, transform.position);
            //float step = m_MoveSpeed * Time.deltaTime;

            //transform.position = Vector3.Lerp(transform.position, FollowTarget.transform.position, step);


            //if (m_TrackRotation == true)
            //{
            //    transform.rotation = FollowTarget.transform.rotation;
            //}

            GameUtilities.FollowPlayerCam(transform.Position, transform.Rotation);

            p_fire_timer += dt;
            if ((InputUtility.onKeyTriggered("SHOOT") || (InputUtility.onKeyHeld("SHOOT"))))
            {
                if (p_fire_timer >= p_fireRate)
                {
                    // Call C++ side bullet firing
                    GameUtilities.FirePlayerBullet(entityID, transform.Position, transform.Rotation);
                    p_fire_timer = 0.0f;
                }
            }
        }

        public void FixedUpdate()
        {

        }

        public override void OnTriggerEnter(uint id)
        {
            //Console.WriteLine(" Other ID" + id);
        }

        public override void OnTriggerExit(uint id)
        {
            
        }
    }
}
