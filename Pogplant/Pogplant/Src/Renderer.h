#pragma once

namespace Pogplant
{
	class Renderer
	{
	public:
		static void ClearBuffer(float _R = 0.235f, float _G = 0.117f, float _B = 0.251f);
		static void SwapBuffer();
		static void Draw();
	private:
	};
}