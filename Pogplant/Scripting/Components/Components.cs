﻿/*****************************************************************************/
/*!
\file	Components.cs
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\author Ng Tian Kiat
\par	email: tiankiat.ng\@digipen.edu
\details
	Components class for scripting c# project

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

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
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Transform
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public void SetPositionAndRotatation(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }
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

    [StructLayout(LayoutKind.Sequential)]
    public class Time
    {
        public static double deltaTime;
    }
}
