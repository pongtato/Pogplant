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
    public class PlayerScript : MonoBehaviour
    {

        public float movement_speed = 50.0f;

        public PlayerScript()
        {
        }

        public override void Start()
        {
        }

        public void Awake()
        {
        }

        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
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

            float horizontal_input = (InputUtility.onKeyHeld("LEFT") ? -1.0f : 0.0f) + (InputUtility.onKeyHeld("RIGHT") ? 1.0f : 0.0f);
            float vertical_input = (InputUtility.onKeyHeld(KEY_ID.KEY_W) ? 1.0f : 0.0f) + (InputUtility.onKeyHeld(KEY_ID.KEY_S) ? -1.0f : 0.0f);

            Vector3 force_dir = new Vector3(horizontal_input, vertical_input, 0) * movement_speed * dt; // 32 is magic number

            transform.Position += force_dir;

            //if (rigidbody.velocity <= movement_speed)

        }

        public void LateUpdate(ref Transform transform, ref Rigidbody rigidbody)
        {
        }

        public void FixedUpdate()
        {
        }

        public void OnTriggerEnter()
        {
            Console.WriteLine("OnTriggerEnter");
        }

        public void OnTriggerExit()
        {
            Console.WriteLine("OnTriggerExit");
        }
    }
}
