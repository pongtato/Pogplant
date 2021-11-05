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
        public extern static int CheckBounds(Vector3 _Position);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void FollowPlayerCam(Vector3 _Position, Vector3 _Rotation);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void FirePlayerBullet(uint entityID, Vector3 _Position, Vector3 _Rotation);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void FireEnemyBullet(Vector3 _Position, Vector3 _Rotation);
        //[MethodImplAttribute(MethodImplOptions.InternalCall)]
        //public extern static void DebugSphere(float pos_x, float pos_y, float pos_z, float camdir_x, float camdir_y, float camdit_z, float _Radius);
    }
}
