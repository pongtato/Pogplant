#include "Application.h"

#define NOMINMAX
#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>

int main()
{
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);

	PogplantDriver::Application& instance = PogplantDriver::Application::GetInstance();
	instance.Init();
	instance.Run();
	instance.Exit();
}
