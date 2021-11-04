using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Scripting
{
    // All the components needed for the scripts
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public Single X;
        public Single Y;
        public Single Z;

        public Vector3(Single x = 0.0f, Single y = 0.0f, Single z = 0.0f)
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

        public static Vector3 operator -(Vector3 a)
        {
            return a = a * -1.0f;
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }

        public static Vector3 operator *(float b, Vector3 a)
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            Vector3 result = a - b;
            return result.magnitude();
        }

        public static Vector3 Lerp(Vector3 start, Vector3 end, float t)
        {
            return (1 - t) * start + t * end;
        }

        public float magnitude()
        {
            return (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }

        public static Vector3 One()
        {
            return new Vector3(1.0f, 1.0f, 1.0f);
        }
        public static Vector3 Zero()
        {
            return new Vector3(0.0f, 0.0f, 0.0f);
        }

        //Dot product for vector3
        public static float Dot(Vector3 a, Vector3 b)
        {
            return (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);
        }

        public static float Angle(Vector3 a, Vector3 b)
        {
            float dot  = Dot(a, b);
            //Account for Divide by 0
            float mag_a = a.magnitude();
            float mag_b = b.magnitude();
            if (mag_a == 0.0f || mag_b == 0.0f)
                return 0.0f;
            return (float)Math.Acos(dot / (a.magnitude() * b.magnitude()));

        }

        public static Vector3 RotateTowards (Vector3 from, Vector3 to, float maxDegreesDelta)
        {
            float angle = Angle(from, to);
            if (angle == 0.0f)
            {
                return to;
            }
            float  t = Math.Min(1f, maxDegreesDelta / angle);
            return Lerp(from, to, t);
        }
    }
}
