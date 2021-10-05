/*****************************************************************************/
/*!
\file	ScriptComponents.h
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	Components that are needed for c# scripting

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

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
		// Type of scripts the entity should have
		std::vector<std::string> m_ScriptTypes;
	};
}

#endif // _SCRIPT_COMPONENTS_H_
