﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows;

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

        public static float DistanceSquared(Vector3 a, Vector3 b)
        {
            Vector3 result = a - b;
            return result.magnitudeSquared();
        }

        public static Vector3 Lerp(Vector3 start, Vector3 end, float t)
        {
            return (1 - t) * start + t * end;
        }

        public float magnitude()
        {
            return (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }

        public float magnitudeSquared()
        {
            return (float)(X * X) + (Y * Y) + (Z * Z);
        }

        public static Vector3 Normalise(Vector3 a)
        {
            float mag = a.magnitude();
            if (Math.Abs(mag) <= float.Epsilon)
                return a;
            a.X /= mag;
            a.Y /= mag;
            a.Z /= mag;

            return a;
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
            //Console.WriteLine("dot value:" + dot);
            //Account for Divide by 0
            float mag_a = a.magnitude();
            float mag_b = b.magnitude();

           // Console.WriteLine("mag a:" + mag_a);
            //Console.WriteLine("mag b:" + mag_b);
            
            if (Math.Abs(mag_a) < float.Epsilon || Math.Abs(mag_b) < float.Epsilon)
                return 0.0f;
            //RETURNS IN RADIANS
            float input = dot / (mag_a * mag_b);
            if (input > 1.0f || input < -1.0f || float.IsNaN(input))
                return 0.0f;
            //Console.WriteLine("Input:" + input);
            float test = (float)Math.Acos(input);
            //Console.WriteLine("Test:" + test);
            return test;

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

        public static Vector3 CrossProduct(Vector3 v1, Vector3 v2)
        {
            Single x, y, z;
            x = v1.Y * v2.Z - v2.Y * v1.Z;
            y = (v1.X * v2.Z - v2.X * v1.Z) * -1;
            z = v1.X * v2.Y - v2.X * v1.Y;

            var rtnvector = new Vector3(x, y, z);
            return rtnvector;
        }

        public Vector3 Normalised()
		{
            float length = 1f / magnitude();
            return new Vector3(X * length, Y * length, Z * length);
		}

        public static Vector3 Abs(Vector3 v1)
        {
            Single x, y, z;
            x = Math.Abs(v1.X);
            y = Math.Abs(v1.Y);
            z = Math.Abs(v1.Z);

            return new Vector3(x, y, z);
        }

        //temp for Y only
        public static Vector3 GetRotationFromVector(Vector3 v1)
		{
            return new Vector3(0, -(float)Math.Atan2(v1.Z, v1.X) * 180f/(float)Math.PI + 90f, 0);
		}
    }
}
