using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

namespace Scripting
{
    class DebugUtilities
    {
        // Log debug text to the editor, first param is the script name or anything, second param is the text you want to log.
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void LogToEditor(string scriptName, string textToLog);
    }
}
