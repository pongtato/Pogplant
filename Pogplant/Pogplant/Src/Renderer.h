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
		static void AOPass(entt::registry& registry, bool _EditorMode);
		static void AOBlurPass();
		static void DebugPass(entt::registry& registry);
		static void EndBuffer();
		static void PostProcess();
		static void GLightPass(entt::registry& registry, bool _EditorMode);
		static void ShadowPass(entt::registry& registry);
		static void BlurPass();
		static void HDRPass(bool _Bloom);
		static void ClearBuffer(float _R = 0.0f, float _G = 0.0f, float _B = 0.0f, float _A = 1.0f);
		static void SwapBuffer();
		// Draw mesh in world space
		static void Draw(entt::registry& registry, Components::Renderer* _Selected, bool _EditorMode);
		static void DrawDebug(entt::registry& registry, Components::Renderer* _Selected);
		// Draw screen buffer
		static void DrawScreen();
		static bool m_RenderGrid;
		static bool m_EnableShadows;
		static float m_BloomDamp;
		static float m_Exposure;
		static float m_Gamma;
		static int m_Layer;
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

		static std::string m_CurrentSkybox;

	private:
		static void DrawText(entt::registry& registry, bool _EditorMode);
	};
}