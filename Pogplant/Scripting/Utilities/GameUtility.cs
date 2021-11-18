using System;
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
        public extern static int CheckBounds(Vector3 _Position, Vector3 _Velocity);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint Instantiate(string name, Vector3 _Position, Vector3 _Rotation);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void FollowPlayerCam(Vector3 _Position, Vector3 _Rotation, float _deltaTime);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        //Get Global _Position for player bullet and Global _Rotation, use forwardvector to shoot
        public extern static void FirePlayerBullet(Vector3 _Position, Vector3 _ForwardVector, Vector3 _Rotation);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void FireEnemyBullet(uint entityID, Vector3 _Position, Vector3 _Rotation, bool isTrue = false);
        //[MethodImplAttribute(MethodImplOptions.InternalCall)]
        //public extern static void DebugSphere(float pos_x, float pos_y, float pos_z, float camdir_x, float camdir_y, float camdit_z, float _Radius);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void UpdatePlayerHealth_UI();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static bool GetTurretAlive(uint entityID);

        public static GameObject InstantiateObject(string prefabName, Vector3 position, Vector3 rotation, string tag = "")
        {
            var id = Instantiate(prefabName, position, rotation);
            Transform t = ECS.GetComponent<Transform>(id);

            return new GameObject(id, t, tag);
        }
    }
}
