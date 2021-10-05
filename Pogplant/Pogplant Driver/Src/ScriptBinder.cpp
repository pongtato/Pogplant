/*****************************************************************************/
/*!
\file	ScriptBinder.cpp
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
    Class to Bind C++ functions with C# functions, unused for now.

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
            or disclosure of this file or its contents without the prior
            written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

#include "ScriptBinder.h"

MonoString* ScriptBinder::CS_getRandomWord()
{
    MonoString* outStr = nullptr;
    const char* randWord = "ABC";
    if (randWord)
    {
        outStr = mono_string_new(mono_domain_get(), randWord);
    }
    return outStr;
}
