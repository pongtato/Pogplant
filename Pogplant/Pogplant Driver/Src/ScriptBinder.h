/*****************************************************************************/
/*!
\file	ScriptBinder.h
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	Class to Bind C++ functions with C# functions, unused for now.

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

#ifndef _SCRIPT_BINDER_H_
#define	_SCRIPT_BINDER_H_

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

#endif //_SCRIPT_BINDER_H_