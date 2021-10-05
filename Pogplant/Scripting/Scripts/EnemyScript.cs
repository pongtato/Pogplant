﻿/*****************************************************************************/
/*!
\file	EnemyScript.cs
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	EnemyScript class for scripting c# project

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

namespace Scripting
{
    // Enemy script class
    public class EnemyScript
    {
        public EnemyScript()
        {

        }

        public void Start()
        {
            //Console.WriteLine("C# PRINTOUT: Enemy Script Started.");
        }

        public void Awake()
        {

        }

        public void Update(ref Transform transform, ref Rigidbody rigidbody)
        {
            if (transform.Position.X >= 4.0f)
            {
                rigidbody.AddForce(new Vector3(-3.0f, 0, 0));
            }

            if (transform.Position.X < 4.0f)
            {
                rigidbody.AddForce(new Vector3(3.0f, 0, 0));
            }
        }

        public void FixedUpdate()
        {
        }
    }
}
