using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

namespace Scripting
{
    class ECS
    {
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint CreateEntity(string str, Vector3 pos, Vector3 rot, Vector3 scale);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void DestroyEntity(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint CreateChild(uint parentID, string str, Vector3 pos, Vector3 rot, Vector3 scale);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint FindEntityWithTag(string str);
    }
}
