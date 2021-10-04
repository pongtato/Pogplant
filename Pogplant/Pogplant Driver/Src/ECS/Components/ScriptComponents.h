#ifndef _SCRIPT_COMPONENTS_H_
#define _SCRIPT_COMPONENTS_H_

#include <iostream>
#include <vector>
#include <string>
#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>

namespace Components
{
	struct Scriptable
	{
		// Names of the functions in C# to call the method
		std::vector<std::string> m_Functions;
	};
}


#endif // _SCRIPT_COMPONENTS_H_
