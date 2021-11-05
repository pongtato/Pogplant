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

    [StructLayout(LayoutKind.Sequential)]
    public struct Renderer
    {
        public Vector3 colorTint;
        public Vector3 emissiveTint;
        public int useLight; // true
        public bool editorDrawOnly; // false
        public string modelName;

        public Renderer(string _modelName = "", Vector3 _colorTint = new Vector3(), Vector3 _emissiveTint = new Vector3(), int _useLight = 1, bool _editorDrawOnly = false)
        {
            colorTint = _colorTint;
            emissiveTint = _emissiveTint;
            useLight = _useLight;
            editorDrawOnly = _editorDrawOnly;

            // I can't use .Contains here for some reason, not sure why so do this.
            bool found = false;
            for(int i = 0; i < ComponentHelper.ModelKeys.Length; ++i)
            {
                if(ComponentHelper.ModelKeys[i] == _modelName)
                {
                    found = true;
                    break;
                }
            }

            if(found)
            {
                modelName = _modelName;
            }
            else
            {
                Console.WriteLine("Model: '" + _modelName + "' not found in modelpool, using default: '" + ComponentHelper.ModelKeys[0] +"'");
                modelName = ComponentHelper.ModelKeys[0];
            }
        }

        public Renderer(string _modelName)
        {
            colorTint = Vector3.One();
            emissiveTint = Vector3.One();
            useLight = 1;
            editorDrawOnly = false;

            // I can't use .Contains here for some reason, not sure why so do this.
            bool found = false;
            for (int i = 0; i < ComponentHelper.ModelKeys.Length; ++i)
            {
                if (ComponentHelper.ModelKeys[i] == _modelName)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                modelName = _modelName;
            }
            else
            {
                Console.WriteLine("Model: '" + _modelName + "' not found in modelpool, using default: '" + ComponentHelper.ModelKeys[0] + "'");
                modelName = ComponentHelper.ModelKeys[0];
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Collider
    {
        public bool isTrigger = false;
        public int collisionLayer = 0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class BoxCollider : Collider
    {
        public Vector3 extends;
        public Vector3 centre;

        public BoxCollider()
        {
            extends = Vector3.One();
            centre = Vector3.Zero();
        }

        public BoxCollider(Vector3 _extends, Vector3 _centre)
        {
            extends = _extends;
            centre = _centre;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SphereCollider : Collider
    {
        public float radius;
        public Vector3 centre;

        public SphereCollider()
        {
            radius = 1.0f;
            centre = Vector3.Zero();
        }

        public SphereCollider(float _radius, Vector3 _centre)
        {
            radius = _radius;
            centre = _centre;
        }
    }
}
