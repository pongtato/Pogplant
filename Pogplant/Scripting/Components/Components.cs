/*****************************************************************************/
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
    [StructLayout(LayoutKind.Sequential)]
    public struct Transform
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
        public Transform(Vector3 pos, Vector3 rot, Vector3 scale)
        {
            Position = pos;
            Rotation = rot;
            Scale = scale;
        }

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

        // Inputting 0 parameters doesn't seem to work even with default
        public Rigidbody(Vector3 _velocity = new Vector3(),
                         Vector3 _acceleration = new Vector3(),
                         Vector3 _impulseAcceleration= new Vector3(),
                         Vector3 _newPosition = new Vector3(),
                         bool _isKinematic = false,
                         bool _useGravity = false,
                         float _mass = 1.0f,
                         float _drag = 0.0f)
        {
            velocity = _velocity;
            acceleration = _acceleration;
            impulseAcceleration = _impulseAcceleration;
            newPosition = _newPosition;
            isKinematic = _isKinematic;
            useGravity = _useGravity;
            mass = _mass;
            drag = _drag;
        }

        public Rigidbody(Vector3 _velocity)
        {
            velocity = _velocity;
            acceleration = new Vector3();
            impulseAcceleration = new Vector3();
            newPosition = new Vector3();
            isKinematic = false;
            useGravity = false;
            mass = 1.0f;            
            drag = 0.0f;
        }

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
