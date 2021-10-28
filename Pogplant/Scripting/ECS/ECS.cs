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

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void AddModel(uint _Entity, string _Model, Vector3 _Color, bool _UseLight, bool _EditorOnly, bool _FirstIt);

        public static GameObject CreateEntity(string name, Transform transform, string tag = "Untagged")
        {
            uint entityID = CreateEntity(name, transform.Position, transform.Rotation, transform.Scale);
            GameObject GO = new GameObject(entityID, transform, tag);
            return GO;
        }

        public static GameObject CreateChild(uint parentID, string name, Transform transform, string tag = "Untagged")
        {
            uint entityID = CreateChild(parentID, name, transform.Position, transform.Rotation, transform.Scale);
            GameObject GO = new GameObject(entityID, transform, tag);
            return GO;
        }
    }
}
