/*****************************************************************************/
/*!
\file	PlayerScript.cs
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	PlayerScript class for scripting c# project

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
    // Player script class
    public class PlayerScript
    {
        public PlayerScript()
        {

        }

        public void Start()
        {
            //Console.WriteLine("C# PRINTOUT: Player Script Started.");
        }

        public void Awake()
        {

        }

        public void Update(ref Transform transform, ref Rigidbody rigidbody)
        {
            if(transform.Position.Y >= 10.0f)
            {
                rigidbody.AddForce(new Vector3(0, -3.0f, 0));
            }

            if(transform.Position.Y < 10.0f)
            {
                rigidbody.AddForce(new Vector3(0, 3.0f, 0));
            }

            if(InputUtility.onKeyHeld(KEY_ID.KEY_A))
            {
                //Console.WriteLine("A key is held down");
            }

            if (InputUtility.onKeyTriggered(KEY_ID.KEY_D))
            {
                //Console.WriteLine("D key is triggered");
            }

            if (InputUtility.onKeyReleased(KEY_ID.KEY_S))
            {
                //Console.WriteLine("S key is released");
            }

            //if (InputUtility.onKeyHeld("RIGHT"))
            //{
            //    Console.WriteLine("D key is held down");
            //}

            //if (InputUtility.onKeyTriggered("RIGHT"))
            //{
            //    Console.WriteLine("D key is triggered");
            //}

            //if (InputUtility.onKeyReleased("MENUUP"))
            //{
            //    Console.WriteLine("W key is released");
            //}
        }

        public void LateUpdate(ref Transform transform, ref Rigidbody rigidbody)
        {

        }

        public void FixedUpdate()
        {
        }
    }
}
