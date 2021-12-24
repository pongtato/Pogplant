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
        public extern static uint FindChildEntityWithName(uint parentID, string name);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void GetTransformECS(uint entityID, ref Vector3 pos, ref Vector3 rot, ref Vector3 scale);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetTransformECS(uint entityID, Vector3 pos, Vector3 rot, Vector3 scale);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint SetTransformParent(uint childID, uint parentID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint GetTransformParent(uint childID);
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint RemoveParentFrom(uint childID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void GetBoxColliderECS(uint entityID, ref string collisionLayer, ref bool isTrigger, ref Vector3 centre, ref Vector3 extends);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetColliderBox(uint entityID, ref bool isTrigger, ref Vector3 centre, ref Vector3 extends);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static int GetChildCount(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static string GetTagECS(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint PlayAudio(uint entityID, uint index);

        // Set to True to enable, false to disable entity
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetActive(uint entityID, bool isEnabled);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void RigidbodyAddForce(uint entityID, Vector3 dir);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static bool CheckValidEntity(uint entityID);

        // Temp to test stuff
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static Vector3 GetVelocity(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetVelocity(uint entityID, Vector3 velocity);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetPosition(uint entityID, Vector3 position);
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetRotation(uint entityID, Vector3 rotation);
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetScale(uint entityID, Vector3 scale);

        public static GameObject CreateEntity(string name, Transform transform)
        {
            uint entityID = CreateEntity(name, transform.Position, transform.Rotation, transform.Scale);
            GameObject GO = new GameObject(entityID, transform, name);
            return GO;
        }

        public static GameObject CreateChild(uint parentID, string name, Transform transform)
        {
            uint entityID = CreateChild(parentID, name, transform.Position, transform.Rotation, transform.Scale);
            GameObject GO = new GameObject(entityID, transform, name);
            return GO;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static Vector3 GetGlobalPosition(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static Vector3 GetGlobalRotation(uint entityID);

        public static T GetComponent<T>(uint entityID)
        {
            if(typeof(T) == typeof(Transform))
            {
                Transform transform = new Transform();
                GetTransformECS(entityID, ref transform.Position, ref transform.Rotation, ref transform.Scale);
                return (T)(object)transform;
            }
            else if (typeof(T) == typeof(BoxCollider))
            {
                BoxCollider boxCollider = new BoxCollider();
                GetBoxColliderECS(entityID, ref boxCollider.collisionLayer, ref boxCollider.isTrigger, ref boxCollider.centre, ref boxCollider.extends);
                return (T)(object)boxCollider;
            }
            else if (typeof(T) == typeof(Tag))
            {
                Tag tagged = new Tag();
                tagged.tag = GetTagECS(entityID);
                return (T)(object)tagged;
            }
            else 
            {
                Console.WriteLine("Component not supported.");
            }

            return default(T);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint[] GetChildren(uint entityID);
    }
}
