using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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

        // Private data members
        private int i;
    }
}
