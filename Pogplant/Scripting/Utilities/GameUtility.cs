﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

namespace Scripting
{
    class GameUtilities
    {
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void GetMousePos(ref float _X, ref float _Y);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static int CheckBounds(Vector3 _Position, Vector3 _Velocity);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint Instantiate(string name, Vector3 _Position, Vector3 _Rotation);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void FollowPlayerCam(uint playerCam, uint playerBox, uint playerShip, Vector3 _Position, Vector3 _Rotation, float _deltaTime);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        //Get Global _Position for player bullet and Global _Rotation, use forwardvector to shoot
        public extern static void FirePlayerBullet(Vector3 _Position, Vector3 _ForwardVector, Vector3 _Rotation);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SpawnStaticExplosion(Vector3 _Position, int type);


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void FireEnemyBullet(uint entityID, Vector3 _Position, Vector3 _Rotation, float _Speed, float _Lifetime, bool isTrue = false);
        //[MethodImplAttribute(MethodImplOptions.InternalCall)]
        //public extern static void DebugSphere(float pos_x, float pos_y, float pos_z, float camdir_x, float camdir_y, float camdit_z, float _Radius);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void UpdatePlayerHealth_UI();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void PlayerTakeDamage(uint Player_ID, float _Damage);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static bool GetAlive(uint entityID);
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void PlayEnemyDeathAnimation(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void StartMissile(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetTurretFire(uint entityID, bool isActivated, int TurretIdentifier);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void UpdateScore(uint text_object, uint score);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void IncreaseScorefromEnv(uint entityID);


        public static GameObject InstantiateObject(string prefabName, Vector3 position, Vector3 rotation, string tag = "")
        {
            var id = Instantiate(prefabName, position, rotation);
            Transform t = ECS.GetComponent<Transform>(id);

            return new GameObject(id, t, tag);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void LoadScene(string levelToLoad);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void ExitScene();
    }
}
