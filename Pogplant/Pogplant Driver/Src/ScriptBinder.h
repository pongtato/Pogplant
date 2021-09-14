#ifndef _SCRIPT_BINDER_
#define	_SCRIPT_BINDER_

#include <mono/metadata/assembly.h>
#include <mono/metadata/object.h>
#include <mono/metadata/appdomain.h>
#include <mono/metadata/debug-helpers.h>

// Class to bind to C# functions
class ScriptBinder
{
public:

	// Example function to generate string from cpp and bind to c#
	static MonoString* CS_getRandomWord();
};

#endif //_SCRIPT_BINDER_