#pragma once

namespace PogplantDriver
{
	struct ImguiHelper
	{
		static bool InitImgui();
		static void DrawImgui();
		static void CleanUpImgui();
	};
}

namespace PPD = PogplantDriver;