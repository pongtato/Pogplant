using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;


namespace Scripting
{
    public class GameObject
    {
        public GameObject()
        {
        }

        public GameObject(uint _id, Transform _transform, string _tag)
        {
            id = _id;
            transform = _transform;
            tag = _tag;
        }

        public GameObject(uint _id, string _name, Transform _transform, Rigidbody _rigidbody, string _tag)
        {
            id = _id;
            name = _name;
            transform = _transform;
            rigidbody = _rigidbody;
            tag = _tag;
        }

        public uint id;
        public string name;
        public string tag;
        //public Transform transform;
        public Nullable<Transform> transform;
        //public Rigidbody rigidbody;
        public Nullable<Rigidbody> rigidbody;

        // For now let's just seperate each component 
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void AddComponentTransform(uint id, Transform transform);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void AddComponentRigidbody(uint id, Rigidbody rigidbody);

        public T AddComponent<T>(T component)
        {
            if(typeof(T) == typeof(Transform))
            {
                if(transform.HasValue == false)
                {
                    transform = (Transform)(object)component;
                    AddComponentTransform(id, transform.Value);
                }
                else 
                {
                    Console.WriteLine("Transform component already exists.");
                }
            }
            else if(typeof(T) == typeof(Rigidbody))
            {
                if(rigidbody.HasValue == false)
                {
                    rigidbody = (Rigidbody)(object)component;
                    AddComponentRigidbody(id, rigidbody.Value);
                }
                else
                {
                    Console.WriteLine("Rigidbody component already exists.");
                }
            }
            else 
            {
                Console.WriteLine("Creating Unknown Component");
            }
            return component;
        }

        public T GetComponent<T>()
        {
            if(typeof(T) ==  typeof(Transform))
            {
                if (transform.HasValue == true)
                {
                    return (T)(object)transform.Value;
                }
                else
                {
                    Console.WriteLine("Transform component doesn't exist.");
                }
            }
            else if(typeof(T) == typeof(Rigidbody))
            {
                if (rigidbody.HasValue == true)
                {
                    return (T)(object)rigidbody.Value;
                }
                else
                {
                    Console.WriteLine("Rigidbody component doesn't exist.");
                }
            }
            else
            {
                Console.WriteLine("Getting Unknown Component");
            }
            return default(T);
        }
    }
}
