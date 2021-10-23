/*****************************************************************************/
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
    public class EnemyScript : MonoBehaviour
    {
        public EnemyScript()
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
            if (transform.Position.X >= 10.0f)
            {
                rigidbody.AddForce(new Vector3(-3.0f, 0, 0));
            }

            if (transform.Position.X < 10.0f)
            {
                rigidbody.AddForce(new Vector3(3.0f, 0, 0));
            }
        }

        public void LateUpdate(ref Transform transform, ref Rigidbody rigidbody)
        {
        }

        public void FixedUpdate()
        {
        }
    }
}
