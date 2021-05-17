#pragma once
#include <unordered_map>

namespace PogplantDriver
{
	typedef unsigned int ImGuiID;

	struct ImguiHelper
	{
		static bool InitImgui();
		static void DrawImgui();
		static void DrawDock();
		static void CleanUpImgui();

		// Window handles
		static void SceneWindow();
		static void GameWindow();

		static bool m_FirstRun;
	};
}

namespace PPD = PogplantDriver;