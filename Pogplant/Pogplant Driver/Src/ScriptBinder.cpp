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
