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

INT WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, PSTR lpCmdLine, INT nCmdShow)
{
	UNREFERENCED_PARAMETER(hInstance);
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);
	UNREFERENCED_PARAMETER(nCmdShow);

	PogplantDriver::Application& instance = PogplantDriver::Application::GetInstance();
	instance.Init();
	instance.Run();
	instance.Exit();

	return 0;
}