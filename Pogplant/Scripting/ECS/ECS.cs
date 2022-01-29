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
        public extern static uint PlayAudio(uint entityID, uint index, string channelGroupName = "NULL");

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

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static Vector3 GetGlobalScale(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetGlobalPosition(uint entityID, Vector3 _Position);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetGlobalRotation(uint entityID, Vector3 _Rotation);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetGlobalScale(uint entityID, Vector3 _Scale);

        // Set to true to pause particle, set to false to unpause particle
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetParticlePause(uint entityID, bool isPause);

        // Set to true to pause particle, set to false to unpause particle

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetLaserStart(uint entityID, bool isActivated);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static bool IsLaserComplete(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void ResetLaser(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetFrames(uint entityID, int frameValue);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetColorTint(uint entityID, ref Vector3 Color);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static Vector3 GetColorTint(uint entityID, ref Vector3 Color);

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



        public static T GetValue<T>(uint entityID, T defaultValue, string name)
        {
            if (typeof(T) == typeof(float))
            {
                return (T)(object)GetValueFloat(entityID, (float)(object)defaultValue, name);
            }
            else if (typeof(T) == typeof(int))
            {
                return (T)(object)GetValueInt(entityID, (int)(object)defaultValue, name);
            }
            else if (typeof(T) == typeof(bool))
            {
                return (T)(object)GetValueBool(entityID, (bool)(object)defaultValue, name);
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)(object)GetValueString(entityID, (string)(object)defaultValue, name);
            }
            else if (typeof(T) == typeof(Vector3))
            {
                return (T)(object)GetValueVector3(entityID, (Vector3)(object)defaultValue, name);
            }
            else
            {
                Console.WriteLine("Value not supported.");
            }

            return (T)(object)0;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern static float GetValueFloat(uint entityID, float defaultValue, string name);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern static int GetValueInt(uint entityID, int defaultValue, string name);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern static bool GetValueBool(uint entityID, bool defaultValue, string name);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern static string GetValueString(uint entityID, string defaultValue, string name);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern static Vector3 GetValueVector3(uint entityID, Vector3 defaultValue, string name);
    }
}
