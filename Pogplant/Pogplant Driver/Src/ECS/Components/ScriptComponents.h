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
#include <unordered_map>
#include <string>

namespace Components
{
	struct Scriptable
	{
		// Type of scripts the entity should have
		//std::vector<std::string> m_ScriptTypes;
		// key is name of script, bool is if it has started
		std::unordered_map<std::string, bool> m_ScriptTypes;
	};

	struct PauseScriptable
	{
		// key is name of script, bool is if it has started
		std::unordered_map<std::string, bool> m_ScriptTypes;
	};
}

#endif // _SCRIPT_COMPONENTS_H_
