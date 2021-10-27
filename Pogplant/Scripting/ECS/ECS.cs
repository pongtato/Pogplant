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
        public extern static uint CreateEntity(string name, Vector3 pos, Vector3 rot, Vector3 scale, string tag = "Untagged");

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void DestroyEntity(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint CreateChild(uint parentID, string name, Vector3 pos, Vector3 rot, Vector3 scale, string tag = "Untagged");

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint FindEntityWithName(string name);
    }
}
