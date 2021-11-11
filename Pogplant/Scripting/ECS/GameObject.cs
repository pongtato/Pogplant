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

        public GameObject(uint _id, Transform _transform, string _name)
        {
            id = _id;
            transform = _transform;
            name = _name;
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
        
        // Components here
        public Nullable<Transform> transform;
        public Nullable<Rigidbody> rigidbody;
        public Nullable<Renderer> renderer;
        public BoxCollider boxCollider;
        public SphereCollider sphereCollider;
        public BaseEnemy baseEnemy;
        public FollowSpline followSpline;
        public PlayerScript playerScript;

        // For now let's just seperate each component 
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void AddComponentTransform(uint id, Transform transform);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void AddComponentRigidbody(uint id, Rigidbody rigidbody);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void AddComponentRenderer(uint id, Vector3 colorTint, Vector3 emissiveTint, int uselight, bool editordrawonly, string modelName);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void AddComponentBoxCollider(uint id, bool isTrigger, string collisionLayer, Vector3 extends, Vector3 centre);

        //[MethodImplAttribute(MethodImplOptions.InternalCall)]
        //public extern static void AddComponentBoxCollider(uint id, BoxCollider boxCollider);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void AddComponentSphereCollider(uint id, bool isTrigger, string collisionLayer, float radius, Vector3 centre);

        //[MethodImplAttribute(MethodImplOptions.InternalCall)]
        //public extern static void AddComponentSphereCollider(uint id, SphereCollider sphereCollider);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void AddScript(uint id, string name);

        public T AddComponent<T>(T component)
        {
            if (typeof(T) == typeof(Transform))
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
            else if(typeof(T) == typeof(Renderer))
            {
                if (renderer.HasValue == false)
                {
                    renderer = (Renderer)(object)component;
                    AddComponentRenderer(id, renderer.Value.colorTint, renderer.Value.emissiveTint, renderer.Value.useLight, renderer.Value.editorDrawOnly, renderer.Value.modelName);
                    //AddComponentRenderer(id, renderer.Value.colorTint, renderer.Value.useLight, renderer.Value.editorDrawOnly);
                }
                else
                {
                    Console.WriteLine("Renderer component already exists.");
                }
            }
            else if(typeof(T) == typeof(BaseEnemy))
            {
                if (baseEnemy == null)
                {
                    baseEnemy = (BaseEnemy)(object)component;
                    AddScript(id, "BaseEnemy");
                }
                else
                {
                    Console.WriteLine("Follow spline script already exists.");
                }
            }
            else if(typeof(T) == typeof(FollowSpline))
            {
                if (followSpline == null)
                {
                    followSpline = (FollowSpline)(object)component;
                    AddScript(id, "FollowSpline");
                }
                else
                {
                    Console.WriteLine("Follow spline script already exists.");
                }
            }
            else if (typeof(T) == typeof(PlayerScript))
            {
                if (playerScript == null)
                {
                    playerScript = (PlayerScript)(object)component;
                    AddScript(id, "PlayerScript");
                }
                else
                {
                    Console.WriteLine("Player script already exists.");
                }
            }
            else if(typeof(T) == typeof(BoxCollider))
            {
                if (boxCollider == null)
                {
                    boxCollider = (BoxCollider)(object)component;
                    AddComponentBoxCollider(id, boxCollider.isTrigger, boxCollider.collisionLayer, boxCollider.extends, boxCollider.centre);
                    //AddComponentBoxCollider(id, boxCollider);
                }
                else
                {
                    Console.WriteLine("Box collider already exists.");
                }
            }
            else if (typeof(T) == typeof(SphereCollider))
            {
                if (sphereCollider == null)
                {
                    sphereCollider = (SphereCollider)(object)component;
                    AddComponentSphereCollider(id, sphereCollider.isTrigger, sphereCollider.collisionLayer, sphereCollider.radius, sphereCollider.centre);
                    //AddComponentSphereCollider(id, sphereCollider);
                }
                else
                {
                    Console.WriteLine("Box collider already exists.");
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
            else if(typeof(T) == typeof(Renderer))
            {
                if (renderer.HasValue == true)
                {
                    return (T)(object)renderer.Value;
                }
                else
                {
                    Console.WriteLine("Renderer component doesn't exist.");
                }
            }
            else if(typeof(T) == typeof(BaseEnemy))
            {
                if(baseEnemy != null)
                {
                    return (T)(object)baseEnemy;
                }
                else
                {
                    Console.WriteLine("Base Enemy component doesn't exist.");
                }
            }
            else if (typeof(T) == typeof(FollowSpline))
            {
                if (followSpline != null)
                {
                    return (T)(object)followSpline;
                }
                else
                {
                    Console.WriteLine("Follow Spline script doesn't exist.");
                }
            }
            else if (typeof(T) == typeof(PlayerScript))
            {
                if (playerScript != null)
                {
                    return (T)(object)playerScript;
                }
                else
                {
                    Console.WriteLine("Player script doesn't exist.");
                }
            }
            else if(typeof(T) == typeof(BoxCollider))
            {
                if(boxCollider != null)
                {
                    return (T)(object)boxCollider;
                }
                else
                {
                    Console.WriteLine("Box Collider doesn't exist.");
                }
            }
            else if(typeof(T) == typeof(SphereCollider))
            {
                if(sphereCollider != null)
                {
                    return (T)(object)sphereCollider;
                }
                else 
                {
                    Console.WriteLine("Sphere Collider doesn't exist.");
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
