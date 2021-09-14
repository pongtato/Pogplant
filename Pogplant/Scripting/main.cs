using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    // My main class for scripting
    public class Scripting
    {
         public static IScripts main()
        {
            string word = ObjectScript.getRandomWord();
            return new Scripts(word);
        }
    }
}
