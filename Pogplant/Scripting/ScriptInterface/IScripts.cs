using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    // Interface class for scripts
    public interface IScripts
    {
        bool testfuncwithreturn();

        void testfuncwithparam(char key);
    }
}
