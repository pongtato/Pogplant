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

        private const float shipYawFollowSpeed = 8f;
        private const float shipRollFollowSpeed = 8f;
        private const float shipPitchFollowSpeed = 8f;

        //Change this to make the ship tilt more according to movement
        private const float shipYawMultiplier = 2.5f;
        private const float shipPitchMultiplier = 3.5f;
        private const float shipRollMultiplier = 3.5f;

        private const float cameraShakeInitMultiplier = 0.007f;
        private const float cameraShakeMagMultiplier = 5f;

        //How long the player should be invulnerable if damage is taken
        private const float damageInvulPeriod = 0.1f;

        private bool damageInvul = false;
        private float damageInvulTimer = 0.0f;


        //How much ship should roll based off rotation from the spline
        //private float shipRollMultiplierRotation = 0.25f;
        

        //// THIS IS ONLY FOR TESTING + EXAMPLE PURPOSES
        //public List<uint> entityIDList = new List<uint>();
        //// parentID first, childID second
        //public List<Tuple<uint, uint>> childIDList = new List<Tuple<uint, uint>>();

        public List<GameObject> entityList = new List<GameObject>();
        public List<Tuple<GameObject, GameObject>> childList = new List<Tuple<GameObject, GameObject>>();
        BoxCollider boxCollider;
        Camera camera;
        Transform playerTrans;
        //EnemyManager enemyManager = new EnemyManager();

        bool isAlive = true;
        uint DashboardScreenID;
        uint VOEntityID;
        //uint m_TrailRenderer;

        //PlayerSpline Speed 
        float m_Maxspeed = 12.0f;
        float m_Minspeed = 9.0f;
        float m_SpeedChange = 0.5f;
        float m_IncrementInterval = 0.1f;
        float m_IncrementTimer = 0.0f;

        // Bonus Item / Bonus Effects
        static public int m_BonusItem = 0;
        const int m_BonusItemMax = 10;
        static public bool m_EnableBonusScreen = false;
        uint m_BobHeadMenuID;
        static public uint m_ScoreMultiplierBobbleCount = 1;
        static public uint m_ShieldBobbleCount = 1;
        static public uint m_ComboDecayBobbleCount = 1;
        static public float m_ComboDecayTimer = 0.0f;
        const float m_ComboDecayTimeLimit = 5.0f;
        static public bool m_ComboActive = true;

        // Bonus Effects

        //Pause menu
        static public bool m_EnablePauseMenu = false;

         
        public PlayerScript()
        {

        }

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
            shipCameraEntity = ECS.FindEntityWithName("PlayerCam");
            boxEntityID = ECS.FindEntityWithName("PlayerBox");
            DashboardScreenID = ECS.FindEntityWithName("DashboardScreenFace");
            ECS.PlayAudio(shipCameraEntity, 0, "BGM");
            boxCollider = ECS.GetComponent<BoxCollider>(boxEntityID);
            lastPosition = ECS.GetGlobalPosition(entityID);
            targetRotation = ECS.GetComponent<Transform>(entityID).Rotation;
            m_initialCameraPosition = ECS.GetComponent<Transform>(shipCameraEntity).Position;
            VOEntityID = ECS.FindEntityWithName("VO_AudioSource");
            //m_TrailRenderer = ECS.FindEntityWithName("TrailRender");

            // For the bobble things
            m_BobHeadMenuID = ECS.FindEntityWithName("BobbleHeadMenu");
            m_ScoreMultiplierBobbleCount = 1;
            m_ShieldBobbleCount = 1;
            m_ComboDecayBobbleCount = 1;
            m_ComboDecayTimer = 0.0f;
            m_ComboActive = true;

            float yaw, pitch, roll;
            yaw = pitch = roll = 0;
            Camera.GetCamera(shipCameraEntity, ref yaw, ref pitch, ref roll);
            camera = new Camera(yaw, pitch, roll);

            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();
            ECS.GetTransformECS(entityID, ref pos, ref rot, ref scale);
            playerTrans = new Transform(pos, rot, scale);

            ECS.PlayAudio(VOEntityID, 0, "VO");
        }

        public override void Start()
        {
        }

        public void Awake()
        {
        }

        public override void Update(float dt)
        {
            //Return to main menu key
            //if (InputUtility.onKeyTriggered(KEY_ID.KEY_ESCAPE))
            //{
            //    GameUtilities.LoadScene("MainMenu");
            //}

            //// Example usage, pause particle
            //if (InputUtility.onKeyTriggered(KEY_ID.KEY_0))
            //{
            //    ECS.SetParticlePause(m_TrailRenderer, true);
            //}

            //// Example usage, play particle
            //if (InputUtility.onKeyTriggered(KEY_ID.KEY_1))
            //{
            //    ECS.SetParticlePause(m_TrailRenderer, false);
            //}

            //Updates Follow Spline
            UpdateFollowSplineSpeed(dt);

            // Updates bonus item 
            UpdateBonusItem();

            // Updates the combo bonus
            UpdateCombo(dt);

            // Updates Combo bonus

            ECS.GetTransformECS(entityID, ref playerTrans.Position, ref playerTrans.Rotation, ref playerTrans.Scale);
            Camera.GetCamera(shipCameraEntity, ref camera.m_Yaw, ref camera.m_Pitch, ref camera.m_Roll);

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
            float length = playerTrans.Position.magnitude();

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
                    ECS.RigidbodyAddForce(entityID, right_vec * -dotproduct * 700f);
                    //rigidbody.AddForce(right_vec * -dotproduct * 700f);
                    //Console.WriteLine("Exceed +X bounds");
                }
                else
                {
                    ECS.RigidbodyAddForce(entityID, right_vec * -dotproduct * 700f);
                    //rigidbody.AddForce(right_vec * -dotproduct * 700f);
                    //Console.WriteLine("Exceed -X bounds");
                }
            }

            if(playerTrans.Position.Y > boxCollider.extends.Y)
            {
                ECS.RigidbodyAddForce(entityID, up_vec * (boxCollider.extends.Y - playerTrans.Position.Y) * 150f);
                //rigidbody.AddForce(up_vec * (boxCollider.extends.Y - transform.Position.Y) * 150f);
                //Console.WriteLine("Exceed +Y bounds");
            }

            if(playerTrans.Position.Y < -boxCollider.extends.Y)
            {
                ECS.RigidbodyAddForce(entityID, up_vec * (-boxCollider.extends.Y - playerTrans.Position.Y) * 450f);
                //rigidbody.AddForce(up_vec * (-boxCollider.extends.Y - transform.Position.Y) * 450f);
                //Console.WriteLine("Exceed -Y bounds");
            }

            float directionalMag = direc_vector.magnitude();

            if (directionalMag > 1.0f)
                direc_vector *= 1 / directionalMag;

            Vector3 force_dir = direc_vector * movement_speed;
            ECS.RigidbodyAddForce(entityID, force_dir);
            //rigidbody.AddForce(force_dir);

            Vector3 playerVel = ECS.GetVelocity(entityID);
            float maxslowforce = playerVel.magnitude();
            if (Math.Abs(maxslowforce) <= float.Epsilon)
                maxslowforce = 0.0f;
            else
            {
                Vector3 SlowDownVec = -playerVel * (1 / maxslowforce);
                playerVel += SlowDownVec * Math.Min(maxslowforce, maxslowforce * slowForce * dt);
            }

            if (dt > 0)
            {
                calculatedVelocity = lastPosition - playerGlobalPos;
                calculatedVelocity = calculatedVelocity * (1 / dt);
            }

            //float maxslowforce = rigidbody.velocity.magnitude();
            //if (Math.Abs(maxslowforce) <= float.Epsilon)
            //    maxslowforce = 0.0f;
            //else
            //{
            //    Vector3 SlowDownVec = -rigidbody.velocity * (1 / maxslowforce);
            //    rigidbody.velocity += SlowDownVec * Math.Min(maxslowforce, maxslowforce * slowForce * dt);
            //}

            //if (dt > 0)
            //{
            //    calculatedVelocity = lastPosition - playerGlobalPos;
            //    calculatedVelocity = calculatedVelocity * (1 / dt);
            //}

            lastPosition = playerGlobalPos;

            //Ship tilter
            float relativeVelX = Vector3.Dot(right_vec, calculatedVelocity);
            
            //Left right tilt
            targetRotation.Y = relativeVelX * shipYawMultiplier;

            //Roll tilt
            targetRotation.Z = -relativeVelX * shipRollMultiplier;

            //Up down tilt
            targetRotation.X = Vector3.Dot(up_vec, calculatedVelocity) * shipPitchMultiplier;

            playerTrans.Rotation.Y += (targetRotation.Y - playerTrans.Rotation.Y) * shipYawFollowSpeed * dt;
            playerTrans.Rotation.Z += (targetRotation.Z - playerTrans.Rotation.Z) * shipRollFollowSpeed * dt;
            playerTrans.Rotation.X += (targetRotation.X - playerTrans.Rotation.X) * shipPitchFollowSpeed * dt;

            playerTrans.Position.Z = 0.0f;

            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();
            ECS.GetTransformECS(boxEntityID, ref pos, ref rot, ref scale);
            Transform box_pos = new Transform(pos, rot, scale);

            Vector3 rotationTarget = new Vector3(
                (-box_pos.Rotation.X - playerTrans.Rotation.X),
                (box_pos.Rotation.Y + playerTrans.Rotation.Y) + 180.0f,
                -playerTrans.Rotation.Z);

            if (camera.m_Yaw - rotationTarget.Y > 180.0f)
                camera.m_Yaw -= 360.0f;
            else if (camera.m_Yaw - rotationTarget.Y < -180.0f)
                camera.m_Yaw += 360.0f;

            if (camera.m_Pitch - rotationTarget.X > 180.0f)
                camera.m_Pitch -= 360.0f;
            else if (camera.m_Pitch - rotationTarget.X < -180.0f)
                camera.m_Pitch += 360.0f;

            if (camera.m_Roll - rotationTarget.Z > 180.0f)
                camera.m_Roll -= 360.0f;
            else if (camera.m_Roll - rotationTarget.Z < -180.0f)
                camera.m_Roll += 360.0f;

            //Lerps yaw and pitch over time
            camera.m_Yaw += (rotationTarget.Y - camera.m_Yaw) * dt * 20.0f;
            camera.m_Pitch += (rotationTarget.X - camera.m_Pitch) * dt * 20.0f;
            camera.m_Roll += (rotationTarget.Z - camera.m_Roll) * dt * 20.0f;

            Camera.SetCamera(shipCameraEntity, camera.m_Yaw, camera.m_Pitch, camera.m_Roll);

            //Console.WriteLine("Position: " + playerTrans.Position.X + '|' + playerTrans.Position.Y + '|' + playerTrans.Position.Z);
            //Console.WriteLine("Rotation: " + playerTrans.Rotation.X + '|' + playerTrans.Rotation.Y + '|' + playerTrans.Rotation.Z);
            //Console.WriteLine("Scale: " + playerTrans.Scale.X + '|' + playerTrans.Scale.Y + '|' + playerTrans.Scale.Z);

            ECS.SetVelocity(entityID, playerVel);
            ECS.SetTransformECS(entityID, playerTrans.Position, playerTrans.Rotation, playerTrans.Scale);


            //Pause menu
            if (InputUtility.onKeyTriggered("ESCAPE"))
            {
                Console.WriteLine("pausing");
                m_EnablePauseMenu = true;
                GameUtilities.PauseScene();
            }
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

        public override void LateUpdate(float dt)
        {

            if (m_isShaking)
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

            if(damageInvul)
			{
                damageInvulTimer += dt;

                if (damageInvulTimer > (damageInvulPeriod * m_ShieldBobbleCount))
                    damageInvul = false;

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
            if (damageInvul)
                return;

            damageInvul = true;
            damageInvulTimer = 0.0f;

            if (health > 0)
            {
                health -= damage;
            }

            //GameUtilities.UpdatePlayerHealth_UI();

            if (health <= 0)
            {
                HandleDeath();
            }

            Console.WriteLine("Player took damage, health is now: " + health + " Entity ID: " + entityID);
            GameUtilities.UpdateDashboardFace(DashboardScreenID, 2);
            ECS.PlayAudio(shipCameraEntity, 2, "SFX");
            EnemyManager.AddScore(false);

            //Triggers a random camera shake upon taking damage, scales with damage taken
            TriggerCameraShake(new Vector3(GetRandFloat() * cameraShakeInitMultiplier * damage, GetRandFloat() * cameraShakeInitMultiplier * damage, GetRandFloat() * cameraShakeInitMultiplier * damage),
                new Vector3(GetRandFloat() * cameraShakeMagMultiplier, GetRandFloat() * cameraShakeMagMultiplier, GetRandFloat() * cameraShakeMagMultiplier), 1f);

            //Add controller vibration
            InputUtility.VibrateControllerHeavyMotor(0.8f, 0.2f);
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

        void UpdateFollowSplineSpeed(float dt)
        {
            //Add speed
            if (m_IncrementTimer >= 0)
            {
                m_IncrementTimer -= dt;
            }
            else
            {
                if (InputUtility.onKeyTriggered(KEY_ID.KEY_Q) || InputUtility.onKeyHeld(KEY_ID.KEY_Q))
                {
                    if ((FollowSpline.follow_speed += m_SpeedChange) > m_Maxspeed)
                        FollowSpline.follow_speed = m_Maxspeed;
                    m_IncrementTimer = m_IncrementInterval;
                }
                if (InputUtility.onKeyTriggered(KEY_ID.KEY_E) || InputUtility.onKeyHeld(KEY_ID.KEY_E))
                {
                    if ((FollowSpline.follow_speed -= m_SpeedChange) < m_Minspeed)
                        FollowSpline.follow_speed = m_Minspeed;

                    //DebugUtilities.LogToEditor("PlayerScript", "Current FollowSpline speed is: " + FollowSpline.follow_speed);
                    //Console.WriteLine("Current FollowSpline speed is: " + FollowSpline.follow_speed);
                    m_IncrementTimer = m_IncrementInterval;
                }
            }
        }

        void UpdateBonusItem()
        {
            if(m_BonusItem >= m_BonusItemMax)
            {
                m_EnableBonusScreen = true;
                ECS.SetActive(m_BobHeadMenuID, true);
                GameUtilities.PauseScene();
            }
        }

        void UpdateCombo(float dt)
        {
            m_ComboDecayTimer += dt;
            
            if(m_ComboDecayTimer >= (m_ComboDecayTimeLimit * m_ComboDecayBobbleCount))
            {
                m_ComboDecayTimer = 0.0f;
                m_ComboActive = false;
            }
            else
            {
                m_ComboActive = true;
            }
        }
    }
}
