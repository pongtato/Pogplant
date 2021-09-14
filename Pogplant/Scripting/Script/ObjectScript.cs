using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Scripting
{
    public class ObjectScript
    {
        // Fetch something from CPP, in this example I'll fetch a string
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static string getRandomWord();
    }
}
