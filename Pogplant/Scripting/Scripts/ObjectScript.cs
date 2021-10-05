/*****************************************************************************/
/*!
\file	ObjectScript.cs
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	ObjectScript class for scripting c# project

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
using System.Runtime.CompilerServices;

namespace Scripting
{
    // This is mainly for testing purposes
    public class ObjectScript
    {
        // Fetch something from CPP, in this example I'll fetch a string
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static string getRandomWord();
    }
}
