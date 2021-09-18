#pragma once
#include <vector>
#include <string>

#include "../../Pogplant Driver/Src/ECS/entt_ecs/entt.hpp"

namespace Components 
{
	struct RenderObject;
}


namespace Pogplant
{

	class Renderer
	{
	public:
		static void StartEditorBuffer();
		static void StartGameBuffer();
		static void EndBuffer();
		static void PostProcess();
		static void ClearBuffer(float _R = 0.235f, float _G = 0.117f, float _B = 0.251f, float _A = 1.0f);
		static void SwapBuffer();
		// Draw mesh in world space
		static void Draw(const char* _CameraID, const entt::registry& registry, Components::RenderObject* _Selected);
		static void Draw(const float(&_View)[16], const float(&_Ortho)[16], const float(&_Perspective)[16]);
		// Draw screen buffer
		static void DrawScreen();
		static void BindTexture(int _Location, unsigned _TexID);
	private:
	};
}