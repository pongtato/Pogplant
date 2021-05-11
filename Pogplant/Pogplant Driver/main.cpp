#include <iostream>
#include <Pogplant.h>


void Init()
{
	PP::Window::InitWindow(1600, 900, "KEKW");
}

void Run()
{
	//while (!PP::Window::ShouldCloseWindow())
	//{
	//	PP::Window::CheckForceClose(); // Temp exit using Esc
	//}
}

void Exit()
{
	PP::Window::CleanUpWindow();
}

int main()
{
	Init();
	Run();
	Exit();
}