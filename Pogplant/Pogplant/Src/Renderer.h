#pragma once
#include <vector>
#include <string>

#include "../../Pogplant Driver/Src/ECS/entt_ecs/entt.hpp"

namespace Components 
{
	struct Renderer;
}

namespace Pogplant
{

	class Renderer
	{
	public:
		static void StartEditorBuffer();
		static void StartGameBuffer();
		static void StartGBuffer();
		static void DebugPass(const entt::registry& registry);
		static void EndBuffer();
		static void PostProcess();
		static void GLightPass(const entt::registry& registry, bool _EditorMode);
		static void ShadowPass(const entt::registry& registry);
		static void BlurPass();
		static void HDRPass(bool _Bloom);
		static void ClearBuffer(float _R = 0.0f, float _G = 0.0f, float _B = 0.0f, float _A = 1.0f);
		static void SwapBuffer();
		// Draw mesh in world space
		static void Draw(const entt::registry& registry, Components::Renderer* _Selected, bool _EditorMode);
		static void DrawDebug(const entt::registry& registry, Components::Renderer* _Selected);
		static void Draw(const float(&_View)[16], const float(&_Ortho)[16], const float(&_Perspective)[16]);
		// Draw screen buffer
		static void DrawScreen();
		static void BindTexture(int _Location, unsigned _TexID);
		static bool m_RenderGrid;
		static float m_Exposure;
		static float m_Gamma;
	private:
		static void DrawText(const entt::registry& registry, bool _EditorMode);
	};
}