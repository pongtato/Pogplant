using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

namespace Scripting
{
    public class ComponentHelper
    {
        // Array of Keys used for ModelPool
        public static string[] ModelKeys = GetModelKeysIC();

        public ComponentHelper()
        {   
        }

        public static void PrintOut()
        {
            for (int i = 0; i < ModelKeys.Length; ++i)
            {
                Console.WriteLine(ModelKeys[i]);
            }
        }

        // Updates ModelKeys
        public static void UpdateModelKeys()
        {
            ModelKeys = GetModelKeysIC();
            Console.WriteLine("ModelKeys updated.");
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static string[] GetModelKeysIC();
    }

}
