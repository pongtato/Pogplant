#pragma once
#include <unordered_map>

struct ImVec2;

namespace Pogplant
{
	class Camera;
}

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
		static int m_CurrentGOIdx;

	private:
		static void Scene_GOPick(Pogplant::Camera* _CurrCam, ImVec2 _VMin, ImVec2 _VMax);
		static void Scene_GOEdit(Pogplant::Camera* _CurrCam, ImVec2 _VMin, ImVec2 _VMax);
		static void Scene_ViewEdit(Pogplant::Camera* _CurrCam, ImVec2 _VMin, ImVec2 _VMax);
	};
}

namespace PPD = PogplantDriver;