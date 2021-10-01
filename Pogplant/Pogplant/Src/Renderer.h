#pragma once
#include <vector>
#include <string>

#include "../../Pogplant Driver/Src/ECS/entt_ecs/entt.hpp"

namespace Components 
{
	struct Render;
}

namespace Pogplant
{

	class Renderer
	{
	public:
		static void StartEditorBuffer();
		static void StartGameBuffer();
		static void StartGBuffer();
		static void EndBuffer();
		static void PostProcess();
		static void GLightPass(const char* _CameraID, const entt::registry& registry);
		static void ShadowPass(const entt::registry& registry);
		static void ClearBuffer(float _R = 0.0f, float _G = 0.0f, float _B = 0.0f, float _A = 1.0f);
		static void SwapBuffer();
		// Draw mesh in world space
		static void Draw(const char* _CameraID, const entt::registry& registry, Components::Render* _Selected);
		static void DrawNoLight(const char* _CameraID, const entt::registry& registry, Components::Render* _Selected);
		static void Draw(const float(&_View)[16], const float(&_Ortho)[16], const float(&_Perspective)[16]);
		// Draw screen buffer
		static void DrawScreen();
		static void BindTexture(int _Location, unsigned _TexID);
	private:
		static void DrawText(const char* _CameraID, const entt::registry& registry);
	
	};
}