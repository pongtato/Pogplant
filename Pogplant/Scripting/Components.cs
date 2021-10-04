using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Scripting
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public Single X;
        public Single Y;
        public Single Z;

        public Vector3(Single x, Single y, Single z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(Single v)
        {
            X = v;
            Y = v;
            Z = v;
        }

        public static Vector3 operator *(Vector3 a, float b) 
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b); 
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Transform
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rigidbody
    {
        public bool isKinematic;
        public bool useGravity;
        public float mass;
        public float drag;

        public Vector3 velocity;
        public Vector3 acceleration;
        public Vector3 impulseAcceleration;
        public Vector3 newPosition;

        public void AddForce(Vector3 direction)
        {
            acceleration += direction * (1.0f / mass);
        }

        public void AddImpulseForce(Vector3 direction)
        {
            impulseAcceleration += direction * (1.0f / mass);
        }
    }
}
