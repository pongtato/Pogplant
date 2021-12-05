using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

namespace Scripting
{
    class DebugDraw
    {
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void DebugSphere(Vector3 _Position, Vector3 _CamDirection, float _Radius, uint _Segments);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void DebugLine(Vector3 _Position1, Vector3 _Position2);

        //[MethodImplAttribute(MethodImplOptions.InternalCall)]
        //public extern static void DebugSphere(float pos_x, float pos_y, float pos_z, float camdir_x, float camdir_y, float camdit_z, float _Radius);
    }
}
