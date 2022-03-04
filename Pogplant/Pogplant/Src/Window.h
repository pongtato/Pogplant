#pragma once

//#define PPD_EDITOR_BUILD

struct GLFWwindow;

namespace Pogplant
{
	struct Window
	{
		/// Functions
		static int InitWindow(int _Width, int _Height, const char* _Window_Name = "Pogplant");
		static void CleanUpWindow();
		static void CloseWindow();
		static void CheckForceClose();
		static bool ShouldCloseWindow();
		static GLFWwindow* GetWindow();
		static void SetWindowSize(int _Width, int _Height);
		static void SetVSync(bool _State);
		static void HideCursor();
		static void ShowCursor();

		static void GameSetWindowSize(int _Width, int _Height);
		static void GameSetFullscreen(bool _Fullscreen);

		/// Variables
		static GLFWwindow* m_Window;
		static int m_Width;
		static int m_Height;
		static int m_xPos;
		static int m_yPos;
		static bool m_VSync;
		static float m_Aspect;
		static float m_TargetAspect;
		static bool m_Hide;
		static bool m_IsUnhiding; // To check for key release state so that the thing does not unfocus and close on 1 key press
	};
}