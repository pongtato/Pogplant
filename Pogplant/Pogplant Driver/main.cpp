#include <iostream>
#include <Pogplant.h>


void Init()
{
	PP::Window::InitWindow(1600, 900, "KEKW");
	PP::MeshBuilder::InitMesh();
}

void Run()
{
	while (!PP::Window::ShouldCloseWindow())
	{
		PP::Window::CheckForceClose(); // Temp exit using Esc
		PP::Renderer::ClearBuffer();

		PP::MeshBuilder::RebindQuad();
		PP::Renderer::Draw();

		PP::Renderer::SwapBuffer();
	}
}

void Exit()
{
	PP::MeshBuilder::CleanUpMesh();
	PP::Window::CleanUpWindow();
}

int main()
{
	Init();
	Run();
	Exit();
}