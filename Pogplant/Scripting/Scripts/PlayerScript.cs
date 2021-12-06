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

using Scripting.Utilities;

namespace Scripting
{
    // Player script class
    public class PlayerScript : MonoBehaviour
    {

        public float movement_speed = 200.0f;
        private float horizontal_input = 0;
        private float vertical_input = 0;
        private float slowForce = 4.0f;
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
        static public float maxHealth = 30000.0f;
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

        private float cameraShakeInitMultiplier = 0.005f;
        private float cameraShakeMagMultiplier = 5f;

        //How much ship should roll based off rotation from the spline
        //private float shipRollMultiplierRotation = 0.25f;
        

        //// THIS IS ONLY FOR TESTING + EXAMPLE PURPOSES
        //public List<uint> entityIDList = new List<uint>();
        //// parentID first, childID second
        //public List<Tuple<uint, uint>> childIDList = new List<Tuple<uint, uint>>();

        public List<GameObject> entityList = new List<GameObject>();
        public List<Tuple<GameObject, GameObject>> childList = new List<Tuple<GameObject, GameObject>>();
        BoxCollider boxCollider;
        //EnemyManager enemyManager = new EnemyManager();

        bool isAlive = true;
        uint DashboardScreenID;
        uint VOEntityID;

        public PlayerScript()
        {

        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            shipCameraEntity = ECS.FindEntityWithName("PlayerCam");
            boxEntityID = ECS.FindEntityWithName("PlayerBox");
            DashboardScreenID = ECS.FindEntityWithName("DashboardScreenFace");
            ECS.PlayAudio(shipCameraEntity, 0);
            boxCollider = ECS.GetComponent<BoxCollider>(boxEntityID);
            lastPosition = ECS.GetGlobalPosition(entityID);
            targetRotation = ECS.GetComponent<Transform>(entityID).Rotation;
            m_initialCameraPosition = ECS.GetComponent<Transform>(shipCameraEntity).Position;
            VOEntityID = ECS.FindEntityWithName("VO_AudioSource");
            ECS.PlayAudio(VOEntityID, 0);
        }

        public override void Start()
        {
        }

        public void Awake()
        {
        }

        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
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

            //BoxCollider boxCollider =  ECS.GetComponent<BoxCollider>(boxEntityID);
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
                rigidbody.AddForce(up_vec * (boxCollider.extends.Y - transform.Position.Y) * 150f);
                //Console.WriteLine("Exceed +Y bounds");
            }

            if(transform.Position.Y < -boxCollider.extends.Y)
            {
                rigidbody.AddForce(up_vec * (-boxCollider.extends.Y - transform.Position.Y) * 450f);
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

            transform.Position.Z = 0.0f;
        }

        private Vector3 m_initialCameraPosition;
        private Vector3 m_cameraPosition;
        private Vector3 m_cameraRotation;
        private Vector3 m_cameraScale;

        bool m_isShaking = false;
        float m_shakeTimer = 0f;
        float m_shakeAddUnit = 1f;
        Vector3 m_shakeMagnitude;
        Vector3 m_shakeInit;

        

        public void TriggerCameraShake(Vector3 initialShake, Vector3 shakeAmount, float duration)
		{
            if (duration < float.Epsilon)
                return;

            m_isShaking = true;
            m_shakeInit = initialShake;
            m_shakeMagnitude = shakeAmount;
            m_shakeAddUnit = 1 / duration;
            m_shakeTimer = 0f;
        }

        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            //Camera shake movement
            GameUtilities.FollowPlayerCam(shipCameraEntity, boxEntityID, entityID, transform.Position, transform.Rotation, dt);

            if(m_isShaking)
			{
                ECS.GetTransformECS(shipCameraEntity, ref m_cameraPosition, ref m_cameraRotation, ref m_cameraScale);

                m_shakeTimer += dt * m_shakeAddUnit;


                m_cameraPosition.X = m_initialCameraPosition.X + m_shakeInit.X * (1f - Ease.EaseOutElastic(m_shakeTimer, m_shakeMagnitude.X));
                m_cameraPosition.Y = m_initialCameraPosition.Y + m_shakeInit.Y * (1f - Ease.EaseOutElastic(m_shakeTimer, m_shakeMagnitude.Y));
                m_cameraPosition.Z = m_initialCameraPosition.Z + m_shakeInit.Z * (1f - Ease.EaseOutElastic(m_shakeTimer, m_shakeMagnitude.Z));

                if (m_shakeTimer >= 1f)
                {
                    m_isShaking = false;
                    m_cameraPosition = m_initialCameraPosition;
                }

                ECS.SetTransformECS(shipCameraEntity, m_cameraPosition, m_cameraRotation, m_cameraScale);
            }
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
            GameUtilities.UpdateDashboardFace(DashboardScreenID, 2);
            ECS.PlayAudio(shipCameraEntity, 2);

            //Triggers a random camera shake upon taking damage, scales with damage taken
            TriggerCameraShake(new Vector3(GetRandFloat() * cameraShakeInitMultiplier * damage, GetRandFloat() * cameraShakeInitMultiplier * damage, GetRandFloat() * cameraShakeInitMultiplier * damage),
                new Vector3(GetRandFloat() * cameraShakeMagMultiplier, GetRandFloat() * cameraShakeMagMultiplier, GetRandFloat() * cameraShakeMagMultiplier), 1f);
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
