#pragma once

namespace Pogplant
{
	class Renderer
	{
	public:

		static void StartEditorBuffer();
		static void StartGameBuffer();
		static void EndBuffer();
		static void PostProcess();
		static void ClearBuffer(float _R = 0.235f, float _G = 0.117f, float _B = 0.251f);
		static void SwapBuffer();
		static void Draw(const char* _CameraID);
		static void Draw(const float(&_View)[16], const float(&_Projection)[16]);
		static void DrawScreen();
		static void BindTexture(int _Location, unsigned _TexID);
	private:
	};
}