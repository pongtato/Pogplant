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
        public Scripts(string word)
        {
            m_word = word;
        }

        // test function with a bool return
        public bool testfuncwithreturn()
        {
            Console.WriteLine("C# PRINTOUT: The testfuncwithreturn printout word is: {0}", m_word);
            return false;
        }

        // test function with a param input
        public void testfuncwithparam(char key)
        {
            Console.WriteLine("C# PRINTOUT: The testfuncwithparam printout key is: {0}", key);
        }

        // Private data members
        private string m_word;
    }
}
