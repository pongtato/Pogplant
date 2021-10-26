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

        // THIS IS ONLY FOR TESTING + EXAMPLE PURPOSES
        public List<uint> entityIDList = new List<uint>();
        // parentID first, childID second
        public List<Tuple<uint, uint>> childIDList = new List<Tuple<uint, uint>>();

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
            //if(transform.Position.Y >= 10.0f)
            //{
            //    rigidbody.AddForce(new Vector3(0, -3.0f, 0));
            //}

            //if(transform.Position.Y < 10.0f)
            //{
            //    rigidbody.AddForce(new Vector3(0, 3.0f, 0));
            //}

            if(InputUtility.onKeyHeld(KEY_ID.KEY_A))
            {
                //Console.WriteLine("A key is held down");
            }

            if (InputUtility.onKeyTriggered(KEY_ID.KEY_D))
            {
                //Console.WriteLine("D key is triggered");
            }

            // THIS IS ONLY FOR TESTING + EXAMPLE PURPOSES
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_Z))
            {
                uint entityID = ECS.CreateEntity("Testing123", Vector3.Zero(), Vector3.Zero(), Vector3.Zero());
                entityIDList.Add(entityID);
                Console.WriteLine("Entity ID created: " + entityID);
            }

            // THIS IS ONLY FOR TESTING + EXAMPLE PURPOSES
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_X))
            {
                if (entityIDList.Count > 0)
                {
                    uint entity = entityIDList[0];
                    ECS.DestroyEntity(entity);
                    entityIDList.RemoveAt(0);
                    Console.WriteLine("Entity ID destroyed: " + entity);
                }
                else
                {
                    Console.WriteLine("EntityID list is empty");
                }
            }

            // THIS IS ONLY FOR TESTING + EXAMPLE PURPOSES
            if (InputUtility.onKeyTriggered(KEY_ID.KEY_C))
            {
                if (entityIDList.Count > 0)
                {
                    uint parentID = entityIDList[0];
                    uint entityID = ECS.CreateChild(parentID, "Child123", Vector3.Zero(), Vector3.Zero(), Vector3.Zero());
                    childIDList.Add(new Tuple<uint, uint>(parentID, entityID));
                    Console.WriteLine("Child ID created: " + entityID + " Parent ID: " + parentID);
                }
                else
                {
                    Console.WriteLine("EntityID list is empty");
                }
            }

            if (InputUtility.onKeyTriggered(KEY_ID.KEY_V))
            {
                string exampleName = "Testing123";
                uint entityID = ECS.FindEntityWithTag(exampleName);
                Console.WriteLine("EntityID with name " + exampleName + ": " + entityID);
            }

            if (InputUtility.onKeyReleased(KEY_ID.KEY_S))
            {
                //Console.WriteLine("S key is released");
            }

            float horizontal_input = (InputUtility.onKeyHeld("LEFT") ? -1.0f : 0.0f) + (InputUtility.onKeyHeld("RIGHT") ? 1.0f : 0.0f);
            float vertical_input = (InputUtility.onKeyHeld(KEY_ID.KEY_W) ? 1.0f : 0.0f) + (InputUtility.onKeyHeld(KEY_ID.KEY_S) ? -1.0f : 0.0f);

            Vector3 force_dir = new Vector3(horizontal_input, vertical_input, 0) * movement_speed * dt; // 32 is magic number
            transform.Position += force_dir;

            switch(GameUtilities.CheckBounds(transform.Position))
            {
                //Out of X bounds
                case 1:
                    {
                        Console.WriteLine("OUTOFBOUNDDDDxxxx");
                        float newX = transform.Position.X - force_dir.X;
                        transform.Position = new Vector3(newX, transform.Position.Y, transform.Position.Z);
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("OUTOFBOUNDDDDyyyy");
                        float newY = transform.Position.Y - force_dir.Y;
                        transform.Position = new Vector3(transform.Position.X, newY, transform.Position.Z);
                        break;
                    }
                case 3:
                    {
                        float newX = transform.Position.X - force_dir.X;
                        float newY = transform.Position.Y - force_dir.Y;
                        transform.Position = new Vector3(newX, newY, transform.Position.Z);
                        break;
                    }
                default:
                    {
                        //Console.WriteLine("Nothing");
                        break;
                    }
            }
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
