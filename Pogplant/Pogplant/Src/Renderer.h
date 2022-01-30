#pragma once
#include <vector>
#include <string>
#include <glm.hpp>
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

		//enum RenderMode
		//{
		//	EDITOR,
		//	POSITION,
		//	NORMAL,
		//	ALBEDO,
		//	AO,
		//	RENDER_MODE_COUNT,
		//};

		static void InitAOKernel();
		static void StartEditorBuffer();
		static void StartGameBuffer();
		static void StartGBuffer();
		static void AOPass(const entt::registry& registry, bool _EditorMode);
		static void AOBlurPass();
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
		static bool m_EnableShadows;
		static float m_BloomDamp;
		static float m_Exposure;
		static float m_Gamma;
		//static float m_LightShaftDecay;
		//static float m_LightShaftExposure;
		//static float m_LightShaftDensity;
		//static float m_LightShaftWeight;
		//static glm::vec3 m_LightShaftPos;
		//static float m_LightShaftScale;

		static glm::vec3 m_QuatTestPos;
		static glm::vec3 m_QuatTestRot;
		static glm::vec3 m_QuatTestScale;

		static std::vector<glm::vec3> m_AOKernel;
		static float m_AO_Radius;
		static float m_AO_Bias;
		
		//static RenderMode m_DebugRenderMode;
		static bool m_EditorCamDebug;

	private:
		static void DrawText(const entt::registry& registry, bool _EditorMode);
	};
}