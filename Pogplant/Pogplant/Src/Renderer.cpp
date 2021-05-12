#include "Renderer.h"
#include "Window.h"

#include <glfw3.h>

namespace Pogplant
{
	void Renderer::ClearBuffer(float _R, float _G, float _B)
	{
		//glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
		glClearColor(_R, _G, _B, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT);
	}

	void Renderer::SwapBuffer()
	{
		glfwSwapBuffers(Window::GetWindow());
		glfwPollEvents();
	}

	void Renderer::Draw()
	{
	}
}