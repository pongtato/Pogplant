#include "Window.h"
#include "FrameBuffer.h"
#include "CameraResource.h"
#include "TextureResource.h"

#include <glfw3.h>
#include <iostream>

namespace Pogplant
{
	/// Static
	GLFWwindow* Window::m_Window = nullptr;
	int Window::m_Width = 0;
	int Window::m_Height = 0;
	float Window::m_Aspect = 0.0f;
	bool Window::m_VSync = false;

	void WindowSizeCallback(GLFWwindow*, int _Width, int _Height)
	{
		Window::SetWindowSize(_Width, _Height);
		glViewport(0, 0, _Width, _Height);
		FrameBuffer::ResizeFrameBuffer();
		CameraResource::UpdateAllProjection();
		//TextureResource::ReloadTextures();
	}

	void WindowScrollCallback(GLFWwindow*, double _XOffset, double _YOffset)
	{
		(void)_XOffset; // Unused unless your mouse scrolls sideways

		Camera* activeCam = CameraResource::GetActiveCam();
		if (activeCam)
		{
			activeCam->UpdateZoom(_YOffset);
		}
	}

	void WindowMouseCallback(GLFWwindow*, double _XPos, double _YPos)
	{
		Camera* activeCam = CameraResource::GetActiveCam();
		if (activeCam)
		{
			activeCam->UpdateYawPitch(_XPos, _YPos);
			activeCam->UpdateCameraPan(_XPos, _YPos);
		}
	}

	int Window::InitWindow(int _Width, int _Height, const char* _Window_Name)
	{
		m_Width = _Width;
		m_Height = _Height;
		m_Aspect = static_cast<float>(m_Width) / m_Height;

		glfwInit();
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 5);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

		//m_Window = glfwCreateWindow(m_Width, m_Height, _Window_Name, glfwGetPrimaryMonitor(), NULL);
		m_Window = glfwCreateWindow(m_Width, m_Height, _Window_Name, NULL, NULL);
		
		if (m_Window == NULL)
		{
			std::cout << "[PP::WINDOW] Failed to create GLFW window" << std::endl;
			glfwTerminate();
			return -1;
		}

		glfwMakeContextCurrent(m_Window);
		glfwSetFramebufferSizeCallback(m_Window, WindowSizeCallback);
		glfwSetScrollCallback(m_Window, WindowScrollCallback);
		glfwSetCursorPosCallback(m_Window, WindowMouseCallback);

		glfwSetInputMode(m_Window, GLFW_CURSOR, GLFW_CURSOR_NORMAL);

		// VSync
		if (m_VSync)
		{
			glfwSwapInterval(1);
		}
		else
		{
			glfwSwapInterval(0);
		}

#ifdef _DEBUG
		std::cout << "[PP::WINDOW] Window Created " << std::endl;
#endif
		return 0;
	}

	void Window::CleanUpWindow()
	{
#ifdef _DEBUG
		std::cout << "[PP::WINDOW] Window Cleaned Up " << std::endl;
#endif
		glfwTerminate();
	}

	void Window::CloseWindow()
	{
#ifdef _DEBUG
		std::cout << "[PP::WINDOW] Window Closed " << std::endl;
#endif
		glfwSetWindowShouldClose(m_Window, true);
	}

	void Window::CheckForceClose()
	{
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_ESCAPE) == GLFW_PRESS)
		{
			Window::CloseWindow();
		}
	}

	bool Window::ShouldCloseWindow()
	{
		if (!m_Window)
		{
			return false;
		}

		return glfwWindowShouldClose(m_Window);
	}

	GLFWwindow* Window::GetWindow()
	{
		return m_Window;
	}

	void Window::SetWindowSize(int _Width, int _Height)
	{
		m_Width = _Width;
		m_Height = _Height;
		m_Aspect = static_cast<float>(m_Width) / m_Height;
	}

	void Window::SetVSync(bool _State)
	{
		m_VSync = _State;
	}
}
