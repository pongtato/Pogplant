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

        public float movement_speed = 500.0f;
        private float horizontal_input = 0;
        private float vertical_input = 0;
        private float slowForce = 8.0f;
        //private float maxSpeed = 10.0f;


        //Player Rotation movement (Camera duplicates this effect)
        //float camera_roll = -1.5f;

        public float max_rotate_angle = 12.5f;
        public float rotation_speed_horizontal = 1.0f;
        public float rotation_speed_vertical = 1.0f;
        public float revert_speed = 2.0f; // changed from 20
        public float max_dampening_speed = 100.0f;

        public float dampening_reduction = 1.0f;
        public float dampening_modifier = 4.0f;

        //private float current_vertical_dampening;
        //private float current_horizontal_dampening;

        //private float timeCount;
        public float ship_follow_rot_speed;
        static public float maxHealth = 300.0f;
        public float health = maxHealth;

        uint shipCameraEntity;
        uint boxEntityID;

        //Used to calculate ship tilt
        private Vector3 lastPosition;
        private Vector3 calculatedVelocity;
        private Vector3 targetRotation;

        private float shipYawFollowSpeed = 8f;
        private float shipRollFollowSpeed = 8f;
        private float shipPitchFollowSpeed = 8f;

        //Change this to make the ship tilt more according to movement
        private float shipYawMultiplier = 2.5f;
        private float shipPitchMultiplier = 3.5f;
        private float shipRollMultiplier = 3.5f;

        //How much ship should roll based off rotation from the spline
        private float shipRollMultiplierRotation = 0.25f;
        

        //// THIS IS ONLY FOR TESTING + EXAMPLE PURPOSES
        //public List<uint> entityIDList = new List<uint>();
        //// parentID first, childID second
        //public List<Tuple<uint, uint>> childIDList = new List<Tuple<uint, uint>>();

        public List<GameObject> entityList = new List<GameObject>();
        public List<Tuple<GameObject, GameObject>> childList = new List<Tuple<GameObject, GameObject>>();
        //EnemyManager enemyManager = new EnemyManager();

        bool isAlive = true;

        public PlayerScript()
        {

        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            shipCameraEntity = ECS.FindEntityWithName("PlayerCam");
            boxEntityID = ECS.FindEntityWithName("PlayerBox");
            ECS.PlayAudio(shipCameraEntity, 0);

            lastPosition = ECS.GetGlobalPosition(entityID);
            targetRotation = ECS.GetComponent<Transform>(entityID).Rotation;
        }

        public override void Start()
        {
        }

        public void Awake()
        {
        }

        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            /*//if(transform.Position.Y >= 10.0f)
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
                SpawnWave();
            }

            if (InputUtility.onKeyTriggered(KEY_ID.KEY_D))
            {
                Console.WriteLine("D key is triggered");
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
                if (entityList.Count > 0)
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
            //*/

            // Debug log example
            //if (InputUtility.onKeyTriggered(KEY_ID.KEY_D))
            //{
            //    DebugUtilities.LogToEditor("PlayerScript", "Pressing D");
            //}

            bool rightPushed = InputUtility.onKeyHeld("RIGHT");
            bool leftPushed = InputUtility.onKeyHeld("LEFT");
            bool upPushed = InputUtility.onKeyHeld("UP");
            bool downPushed = InputUtility.onKeyHeld("DOWN");

            if (rightPushed || leftPushed)
                horizontal_input = (rightPushed ? 1.0f : 0.0f) + (leftPushed ? -1.0f : 0.0f);
            else
                horizontal_input = InputUtility.GetAxis("MOVEX");

            if (upPushed || downPushed)
                vertical_input = (upPushed ? 1.0f : 0.0f) + (downPushed ? -1.0f : 0.0f);
            else
                vertical_input = -InputUtility.GetAxis("MOVEY");

            Vector3 up_vec = Transform.GetUpVector(entityID);
            Vector3 forward_vec = Transform.GetForwardVector(entityID);
            Vector3 right_vec = Vector3.CrossProduct(forward_vec, up_vec);
            Vector3 direc_vector = (right_vec * horizontal_input) + (up_vec * vertical_input);

            BoxCollider boxCollider =  ECS.GetComponent<BoxCollider>(boxEntityID);
            Transform boxTransform = ECS.GetComponent<Transform>(boxEntityID);
            float length = transform.Position.magnitude();

            Vector3 playerGlobalPos = ECS.GetGlobalPosition(entityID);
            Vector3 boxGlobalPos = ECS.GetGlobalPosition(boxEntityID);

            //Console.WriteLine("playerGlobalPos Vector: " + playerGlobalPos.X + " | " + playerGlobalPos.Y + " | " + playerGlobalPos.Z);
            //Console.WriteLine("boxGlobalPos Vector: " + boxGlobalPos.X + " | " + boxGlobalPos.Y + " | " + boxGlobalPos.Z);

            if (length > boxCollider.extends.X)
            {
                Vector3 newPosNoY = new Vector3(playerGlobalPos.X, 0.0f, playerGlobalPos.Z);
                Vector3 newForwardNoY = new Vector3(forward_vec.X, 0.0f, forward_vec.Z);
                Vector3 newBoxPosNoY = new Vector3(boxGlobalPos.X, 0.0f, boxGlobalPos.Z);
                Vector3 boxtoplayer = Vector3.Normalise(newPosNoY - newBoxPosNoY);

                float dotproduct = Vector3.Dot(right_vec, boxtoplayer);

                //Console.WriteLine("BoxToPlayer Vector: " + boxtoplayer.X + " | " + boxtoplayer.Y + " | " + boxtoplayer.Z);
                //Console.WriteLine("Forward Vector: " + newForwardNoY.X + " | " + newForwardNoY.Y + " | " + newForwardNoY.Z);
                //Console.WriteLine("Right Vector: " + right_vec.X + " | " + right_vec.Y + " | " + right_vec.Z);
                //Console.WriteLine("Dot Product: " + dotproduct);

                if (dotproduct > 0.0f)
                {
                    rigidbody.AddForce(right_vec * -dotproduct * 700f);
                    //Console.WriteLine("Exceed +X bounds");
                }
                else
                {
                    rigidbody.AddForce(right_vec * -dotproduct * 700f);
                    //Console.WriteLine("Exceed -X bounds");
                }
            }

            if(transform.Position.Y > boxCollider.extends.Y)
            {
                rigidbody.AddForce(up_vec * (boxCollider.extends.Y - transform.Position.Y) * 200f);
                //Console.WriteLine("Exceed +Y bounds");
            }

            if(transform.Position.Y < -boxCollider.extends.Y)
            {
                rigidbody.AddForce(up_vec * (-boxCollider.extends.Y - transform.Position.Y) * 500f);
                //Console.WriteLine("Exceed -Y bounds");
            }

            //if(forward_vec.Z > boxCollider.extends.Z)
            //{
            //    Console.WriteLine("Exceed +Z bounds");
            //}

            //if(forward_vec.Z < -boxCollider.extends.Z)
            //{
            //    Console.WriteLine("Exceed -Z bounds");
            //}

            //int result = GameUtilities.CheckBounds(ECS.GetGlobalPosition(entityID), Vector3.Normalise(rigidbody.velocity) + direc_vector);

            //if ((result & 1) != 0)
            //{
            //    rigidbody.velocity.X = 0f;
            //    direc_vector.X = 0;
            //}

            //if ((result & 2) != 0)
            //{
            //    rigidbody.velocity.Y = 0f;
            //    direc_vector.Y = 0;
            //}

            //if ((result & 4) != 0)
            //{
            //    rigidbody.velocity.Z = 0f;
            //    direc_vector.Z = 0;
            //}

            float directionalMag = direc_vector.magnitude();

            if (directionalMag > 1.0f)
                direc_vector *= 1 / directionalMag;

            Vector3 force_dir = direc_vector * movement_speed;
            rigidbody.AddForce(force_dir);

            float maxslowforce = rigidbody.velocity.magnitude();
            if (Math.Abs(maxslowforce) <= float.Epsilon)
                maxslowforce = 0.0f;
            else
            {
                Vector3 SlowDownVec = -rigidbody.velocity * (1/ maxslowforce);
                rigidbody.velocity += SlowDownVec * Math.Min(maxslowforce, maxslowforce * slowForce * dt);
            }

            if (dt > 0)
            {
                calculatedVelocity = lastPosition - playerGlobalPos;
                calculatedVelocity = calculatedVelocity * (1 / dt);
            }

            lastPosition = playerGlobalPos;

            //Ship tilter
            float relativeVelX = Vector3.Dot(right_vec, calculatedVelocity);
            
            //Left right tilt
            targetRotation.Y = relativeVelX * shipYawMultiplier;

            //Roll tilt
            targetRotation.Z = -relativeVelX * shipRollMultiplier;

            //Up down tilt
            targetRotation.X = Vector3.Dot(up_vec, calculatedVelocity) * shipPitchMultiplier;

            transform.Rotation.Y += (targetRotation.Y - transform.Rotation.Y) * shipYawFollowSpeed * dt;
            transform.Rotation.Z += (targetRotation.Z - transform.Rotation.Z) * shipRollFollowSpeed * dt;
            transform.Rotation.X += (targetRotation.X - transform.Rotation.X) * shipPitchFollowSpeed * dt;
        }

        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            //Vector3 Rotation_Concat = Vector3.Zero();

            //float adjustedAngle = transform.Rotation.X > 180.0f ? transform.Rotation.X - 360.0f : transform.Rotation.X;
            //current_vertical_dampening = max_rotate_angle / (Math.Abs(adjustedAngle) + (dampening_reduction * dampening_modifier)) * 2f;
            //if ((adjustedAngle > -max_rotate_angle && vertical_input > 0) || (adjustedAngle < max_rotate_angle && vertical_input < 0))
            //{
            //    Vector3 Rotatation_Vertical = new Vector3((max_rotate_angle * vertical_input * -current_vertical_dampening * rotation_speed_vertical * dt), 0.0f, 0.0f);
            //    Rotation_Concat += Rotatation_Vertical;
            //    timeCount = 0;
            //}

            //adjustedAngle = transform.Rotation.Y > 180.0f ? transform.Rotation.Y - 360.0f : transform.Rotation.Y;
            //current_horizontal_dampening = max_rotate_angle / (Math.Abs(adjustedAngle) + (dampening_reduction * dampening_modifier));

            //if ((adjustedAngle > -max_rotate_angle && horizontal_input < 0) || (adjustedAngle < max_rotate_angle && horizontal_input > 0))
            //{
            //    Vector3 Rotatation_Horizontal = new Vector3(0.0f, (max_rotate_angle * horizontal_input * current_horizontal_dampening * rotation_speed_horizontal * dt), 0.0f);
            //    Rotation_Concat += Rotatation_Horizontal;
            //    timeCount = 0;
            //}

            //transform.Rotation += Rotation_Concat;
            //float adjustedRoll = (transform.Rotation.Y > 180.0f ? transform.Rotation.Y - 360.0f : transform.Rotation.Y) * camera_roll;
            //transform.Rotation = new Vector3(transform.Rotation.X, transform.Rotation.Y, adjustedRoll);


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

            GameUtilities.FollowPlayerCam(transform.Position, transform.Rotation, dt);

            //p_fire_timer += dt;
            //if ((InputUtility.onKeyHeld("SHOOT")))
            //{
            //    if (p_fire_timer >= p_fireRate)s
            //    {
            //        // Call C++ side bullet firing
            //        GameUtilities.FirePlayerBullet(entityID, transform.Position, transform.Rotation);
            //        ECS.PlayAudio(shipCameraEntity, 1);
            //        p_fire_timer = 0.0f;
            //    }
            //}

        }

        public void SpawnWave()
        {
            //var rand = new Random();
            //Transform location = new Transform(Vector3.Zero(), Vector3.Zero(), Vector3.One());
            //location.Position.X += (((float)rand.Next() / int.MaxValue) * 10.0f - 5.0f);
            //location.Position.Y += (((float)rand.Next() / int.MaxValue) * 10.0f - 5.0f);
            //location.Position.Z = 30.0f;
            //location.Rotation.Y = 180.0f;

            Console.WriteLine("Spawn Wave called!");

            float z_value = 15.0f;

            int max_spawns = 5;

            for (int x = -10; x <= 10; x+=5)
            {
                for (int y = -10; y <= 10; y+=5)
                {
                    if (x != 0 && y != 0 && GetRandFloat() > 0.80f && max_spawns-- >= 0)
                    {
                        Transform location = new Transform();
                        location.Rotation.Y = 180.0f;
                        location.Position.X = x;
                        location.Position.Y = y;
                        location.Position.Z = z_value;
                        //enemyManager.InstantiateTempEnemy(location, "Enemy", "PlayerBox");
                    }
                }
                z_value -= 0.2f;
            }

        }

        // return float between zero and one
        float GetRandFloat()
        {
            var rand = new Random();
            return (float)rand.Next() / int.MaxValue;
        }

        public void FixedUpdate()
        {

        }

        public override void OnTriggerEnter(uint id)
        {
            //Console.WriteLine(" Other ID" + id);
            //string tag =  ECS.GetComponent<Tag>(id);

        }

        public override void OnTriggerExit(uint id)
        {

        }

        public float GetPlayerHealth()
        {
            return health;
        }

        public float GetPlayerMaxHealth()
        {
            return maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (health > 0)
            {
                health -= damage;
            }

            GameUtilities.UpdatePlayerHealth_UI();

            if (health <= 0)
            {
                HandleDeath();
            }
            Console.WriteLine("Player took damage, health is now: " + health + " Entity ID: " + entityID);
        }

        void HandleDeath()
        {

            // This is a hardcoded way of destroying this instance, need to be replaced!
            if (isAlive)
            {
                Console.WriteLine("Player (id: " + entityID + ") has died");
                ECS.DestroyEntity(entityID);
                isAlive = false;
            }
        }
    }
}
