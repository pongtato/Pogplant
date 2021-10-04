using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace Scripting
{
    // Scripting class
    class Scripts : IScripts
    {
        public Scripts()
        {
            i = 0;
        }

        public void testing123()
        {
            //Console.WriteLine("C# PRINTOUT: testing123 function bogos binted.");
        }

        public void Start()
        {
            Console.WriteLine("C# PRINTOUT: Start Scripts.");
        }

        public void Update()
        {
            ++i;
            //Console.WriteLine("C# PRINTOUT: Update i {0}", i);
            testing123();
        }

        public void Move(float dt, ref Transform transform)
        {
            transform.Rotation.Y += 10 * dt;
            Console.WriteLine("C# PRINTOUT: Transform{0}", transform.Rotation.Y);
        }

        public void RigidbodyMove(ref Rigidbody rigidbody)
        {
            rigidbody.AddForce(new Vector3(0, 2.0f, 0));
            //Console.WriteLine("C# PRINTOUT: Velocity{0}", rigidbody.velocity.Y);
        }

        // Private data members
        private int i;
    }
}
