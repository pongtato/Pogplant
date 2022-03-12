#include "Renderer.h"
#include "Window.h"
#include "FrameBuffer.h"
#include "ShaderLinker.h"
#include "Mesh.h"
#include "Model.h"
#include "Skybox.h"
#include "ShadowConfig.h"
#include "Font.h"
#include "MeshBuilder.h"
#include "DebugDraw.h"
#include "Logger.h"
#include "MeshInstance.h"
#include "FontResource.h"
#include "CameraResource.h"
#include "MeshResource.h"
#include "ModelResource.h"
#include "FrameBufferResource.h"
#include "TextureResource.h"
#include "../../Pogplant Driver/Src/ECS/Components/Components.h"

#include <gtc/matrix_transform.hpp>
#include <glew.h>
#include <glfw3.h>
#include <string>
#include <random>
#include <iostream>
namespace Pogplant
{
	bool Renderer::m_RenderGrid = false;
	bool Renderer::m_EnableShadows = true;
	float Renderer::m_BloomDamp = 20.f;
	float Renderer::m_Exposure = 1.0f;
	float Renderer::m_Gamma = 2.2f;
	//float Renderer::m_LightShaftDecay = 0.9f;
	//float Renderer::m_LightShaftExposure = 0.2f;
	//float Renderer::m_LightShaftDensity = 0.69f;
	//float Renderer::m_LightShaftWeight = 0.3f;
	//glm::vec3 Renderer::m_LightShaftPos = { -500.0f,500.0f,0.0f };
	//float Renderer::m_LightShaftScale = 10.0f;
	std::vector<glm::vec3> Renderer::m_AOKernel;
	float Renderer::m_AO_Radius = 1.5f;
	float Renderer::m_AO_Bias = 0.0125f;
	//Renderer::RenderMode Renderer::m_DebugRenderMode = Renderer::RenderMode::EDITOR;
	bool Renderer::m_EditorCamDebug = false;
	int Renderer::m_Layer = 0;

	// Hard fix for shadows, good angle for minimal artifacts
	const glm::vec3 lightDir = glm::normalize(glm::vec3(10.0f, 20.0f, 10.0f));

	/// QUAT TEST
	glm::vec3 Renderer::m_QuatTestPos = glm::vec3{ 0 };
	glm::vec3 Renderer::m_QuatTestRot = glm::vec3{ 0 };
	glm::vec3 Renderer::m_QuatTestScale = glm::vec3{ 1 };

	struct CameraReturnData
	{
		glm::mat4 m_Orthographic;
		glm::mat4 m_Projection;
		glm::mat4 m_View;
		glm::vec3 m_Position;
		float m_Near;
		float m_Far;
		float m_Fov;
	};

	/// Helper
	CameraReturnData GetCurrentCamera(entt::registry& registry, bool _EditorMode)
	{
		// Try to get game camera
		auto cam_results = registry.view<Components::Transform, Components::Camera>();
		if (!_EditorMode)
		{
			// If game cam exists
			if (cam_results.size_hint() > 0)
			{
				CameraReturnData ret = {};
				bool failFlag = true;
				// Use the game camera
				for (auto& e : cam_results)
				{
					auto& it_Trans = cam_results.get<Components::Transform>(e);
					auto& it_Camera = cam_results.get<Components::Camera>(e);
					if (it_Camera.m_Active)
					{
						ret =
						{
							it_Camera.m_Orthographic,
							it_Camera.m_Projection,
							it_Camera.m_View,
							//it_Trans.GetGlobalPosition(),
							it_Trans.m_position,
							it_Camera.m_Near,
							it_Camera.m_Far,
							it_Camera.m_Fov
						};
						
						failFlag = false;
					}

					if (failFlag)
					{
						Logger::Log({ "PP::RENDERER", LogEntry::LOGTYPE::WARNING, "No active game cameras!" });
					}
				}

				// Found a camera
				if (!failFlag)
				{
					return ret;
				}
			}
			else
			{
				Logger::Log({ "PP::RENDERER", LogEntry::LOGTYPE::ERROR, "No game camera found, default to editor camera" });
			}
		}

		// Default to editor cam
		Camera4D* currCam = CameraResource::GetCamera("EDITOR");

		return CameraReturnData
		{
			currCam->m_Orthographic,
			currCam->m_Projection,
			currCam->GetView(),
			currCam->m_Position,
			currCam->m_Near,
			currCam->m_Far,
			currCam->m_Fov
		};
	}

	glm::mat4 LightSpaceMtx(CameraReturnData _Cam, float _CascadeNear, float _CascadeFar, glm::vec3 _LightDir)
	{
		const auto proj = glm::perspective( glm::radians(_Cam.m_Fov), (float)Window::m_Aspect, _Cam.m_Near / _CascadeNear, _Cam.m_Far / _CascadeFar);
		const auto corners = Camera4D::GetFrustumCorner(_Cam.m_Projection, _Cam.m_View);

		glm::vec3 center = glm::vec3(0, 0, 0);
		for (const auto& v : corners)
		{
			center += glm::vec3(v);
		}
		center /= corners.size();

		const auto lightView = glm::lookAt(center + _LightDir, center, glm::vec3(0.0f, 1.0f, 0.0f));

		float minX = std::numeric_limits<float>::max();
		float maxX = std::numeric_limits<float>::min();
		float minY = std::numeric_limits<float>::max();
		float maxY = std::numeric_limits<float>::min();
		float minZ = std::numeric_limits<float>::max();
		float maxZ = std::numeric_limits<float>::min();

		for (const auto& v : corners)
		{
			const auto trf = lightView * v;
			minX = std::min(minX, trf.x);
			maxX = std::max(maxX, trf.x);
			minY = std::min(minY, trf.y);
			maxY = std::max(maxY, trf.y);
			minZ = std::min(minZ, trf.z);
			maxZ = std::max(maxZ, trf.z);
		}

		// Tune this parameter according to the scene
		constexpr float zMult = 10.0f;
		if (minZ < 0)
		{
			minZ *= zMult;
		}
		else
		{
			minZ /= zMult;
		}
		if (maxZ < 0)
		{
			maxZ /= zMult;
		}
		else
		{
			maxZ *= zMult;
		}

		const glm::mat4 lightProjection = glm::ortho(minX, maxX, minY, maxY, minZ, maxZ);

		return lightProjection * lightView;
	}

	std::vector<glm::mat4> CalcAllLightSpaceMtx(CameraReturnData _Cam, glm::vec3 _LightDir)
	{
		std::vector<glm::mat4> ret;
		for (size_t i = 0; i < ShadowCFG::m_CascadeIntervals.size() + 1; ++i)
		{
			if (i == 0)
			{
				ret.push_back(LightSpaceMtx(_Cam, 1.0f, ShadowCFG::m_CascadeIntervals[i], _LightDir));
			}
			else if (i < ShadowCFG::m_CascadeIntervals.size())
			{
				ret.push_back(LightSpaceMtx(_Cam, ShadowCFG::m_CascadeIntervals[i - 1], ShadowCFG::m_CascadeIntervals[i], _LightDir));
			}
			else
			{
				ret.push_back(LightSpaceMtx(_Cam, ShadowCFG::m_CascadeIntervals[i - 1], 1.0f, _LightDir));
			}
		}
		return ret;
	}

	float Lerp(float _A, float _B, float _F)
	{
		return _A + _F * (_B - _A);
	}

	void Renderer::InitAOKernel()
	{
		std::uniform_real_distribution<float> floatRand(0.0, 1.0);
		std::default_random_engine gen = {};
		for (unsigned int i = 0; i < 64; ++i)
		{
			glm::vec3 sample(floatRand(gen) * 2.0 - 1.0, floatRand(gen) * 2.0 - 1.0, floatRand(gen));
			sample = glm::normalize(sample);
			sample *= floatRand(gen);
			float scale = float(i) / 64.0f;

			// scale samples s.t. they're more aligned to center of kernel
			scale = Lerp(0.1f, 1.0f, scale * scale);
			sample *= scale;
			m_AOKernel.push_back(sample);
		}

		ShaderLinker::Use("SSAO");
		// Send kernel + rotation 
		for (unsigned int i = 0; i < m_AOKernel.size(); ++i)
		{
			ShaderLinker::SetUniform(("v3_Samples[" + std::to_string(i) + "]").c_str(), m_AOKernel[i]);
		}
		ShaderLinker::UnUse();
	}

	void Renderer::StartEditorBuffer()
	{
		FrameBuffer::BindFrameBuffer(BufferType::EDITOR_BUFFER);
		glEnable(GL_DEPTH_TEST);
		//glEnable(GL_STENCIL_TEST);
		//glStencilOp(GL_KEEP, GL_KEEP, GL_REPLACE);
	}

	void Renderer::StartGameBuffer()
	{
		FrameBuffer::BindFrameBuffer(BufferType::GAME_BUFFER);
		glEnable(GL_DEPTH_TEST);
	}

	void Renderer::StartGBuffer()
	{
		FrameBuffer::BindFrameBuffer(BufferType::G_BUFFER);
		glEnable(GL_DEPTH_TEST);
	}

	void Renderer::AOPass(entt::registry& registry, bool _EditorMode)
	{
		ShaderLinker::Use("SSAO");
		FrameBuffer::BindFrameBuffer(BufferType::SSAO_BUFFER);
		glClear(GL_COLOR_BUFFER_BIT);
		CameraReturnData ret = GetCurrentCamera(registry, _EditorMode);
		ShaderLinker::SetUniform("v2_Noise", { static_cast<float>(Window::m_Width) * 0.25f, static_cast<float>(Window::m_Height) * 0.25f });
		ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
		ShaderLinker::SetUniform("m4_View", glm::transpose(glm::inverse(ret.m_View)));
		ShaderLinker::SetUniform("Radius", m_AO_Radius);
		ShaderLinker::SetUniform("Bias", m_AO_Bias);
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_POS_BUFFER]);
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_NORMAL_BUFFER]);
		glActiveTexture(GL_TEXTURE2);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_COLOR_BUFFER]);
		MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN);
		ShaderLinker::UnUse();
	}

	void Renderer::AOBlurPass()
	{
		FrameBuffer::BindFrameBuffer(BufferType::SSAO_BLUR_BUFFER);
		glClear(GL_COLOR_BUFFER_BIT);
		ShaderLinker::Use("SSAO_BLUR");
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::SSAO_COLOR_BUFFER]);
		MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN);
		ShaderLinker::UnUse();
	}

	void Renderer::DebugPass(entt::registry& registry)
	{
		FrameBuffer::BindFrameBuffer(BufferType::DEBUG_BUFFER);
		glEnable(GL_DEPTH_TEST);
		ClearBuffer();
		DrawDebug(registry, nullptr);
		EndBuffer();
	}

	void Renderer::EndBuffer()
	{
		FrameBuffer::UnbindFrameBuffer();
		glDisable(GL_DEPTH_TEST);
	}

	void Renderer::PostProcess()
	{
		FrameBuffer::BindFrameBuffer(BufferType::PP_BUFFER);
	}

	void Renderer::GLightPass(entt::registry& registry, bool _EditorMode)
	{
		ShaderLinker::Use("GPASS");
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_POS_BUFFER]);
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_NORMAL_BUFFER]);
		glActiveTexture(GL_TEXTURE2);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_COLOR_BUFFER]);
		glActiveTexture(GL_TEXTURE3);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_NOLIGHT_BUFFER]);
		glActiveTexture(GL_TEXTURE4);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_EMISSIVE_BUFFER]);
		glActiveTexture(GL_TEXTURE5);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_CANVAS_BUFFER]);
		glActiveTexture(GL_TEXTURE6);
		glBindTexture(GL_TEXTURE_2D_ARRAY, FBR::m_FrameBuffers[BufferType::LIGHT_DEPTH_MAP]);
		//glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::SHADOW_DEPTH]);
		glActiveTexture(GL_TEXTURE7);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::SSAO_BLUR_COLOR_BUFFER]);

		ShaderLinker::SetUniform("BloomDamp", m_BloomDamp);
		ShaderLinker::SetUniform("Exposure", m_Exposure);
		ShaderLinker::SetUniform("Gamma", m_Gamma);
		ShaderLinker::SetUniform("Shadows", m_EnableShadows);

		//// Editor cam by default;
		CameraReturnData ret = GetCurrentCamera(registry, _EditorMode);
		ShaderLinker::SetUniform("ViewPos", ret.m_Position);

		/// Shaft pos screen
		//auto clipSpace = ret.m_Projection * (ret.m_View * glm::vec4(m_LightShaftPos,1.0f));
		//auto ndc = glm::vec3{ clipSpace.x,clipSpace.y,clipSpace.z } / clipSpace.w;
		//auto windowSpace = glm::vec2{ ndc.x + 1.0f, ndc.y + 1.0f } / 2.0f;
		//ShaderLinker::SetUniform("ScreenSize", glm::vec2{ Window::m_Width, Window::m_Height });
		//ShaderLinker::SetUniform("LightScreenPos", windowSpace);
		//ShaderLinker::SetUniform("Decay", m_LightShaftDecay);
		//ShaderLinker::SetUniform("Exposure", m_LightShaftExposure);
		//ShaderLinker::SetUniform("Density", m_LightShaftDensity);
		//ShaderLinker::SetUniform("Weight", m_LightShaftWeight);
		//ShaderLinker::SetUniform("m4_View", ret.m_View);

		/// Lights
		// Directional
		// ShaderLinker::SetUniform("m4_LightProjection", ShadowCFG::m_LightProj);
		// Default position in case directional light not in scene
		
		//glm::vec3 lightDir = glm::normalize(glm::vec3(20.0f, 50.0f, 20.0f));
		auto dResults = registry.view<Components::Directional_Light, Components::Transform>();
		auto dLight_it = dResults.begin();
		if (dLight_it != dResults.end())
		{
			const auto& dLight = dResults.get<Components::Directional_Light>(*dLight_it);
			const std::string currLight = "directLight.";
			ShaderLinker::SetUniform((currLight + "Direction").c_str(), dLight.m_Direction);
			ShaderLinker::SetUniform((currLight + "Color").c_str(), dLight.m_Color * dLight.m_Intensity);
			ShaderLinker::SetUniform((currLight + "Diffuse").c_str(), dLight.m_Diffuse);
			ShaderLinker::SetUniform((currLight + "Specular").c_str(), dLight.m_Specular);
			ShaderLinker::SetUniform((currLight + "Direction").c_str(), dLight.m_Direction);
			// const auto& dLight_trans = dResults.get<const Components::Transform>(*dLight_it);
			// lightDir = glm::normalize(dLight_trans.m_position);
		}

		// Point lights
		auto results = registry.view<Components::Point_Light, Components::Transform>();
		ShaderLinker::SetUniform("activeLights", static_cast<int>(results.size_hint()));
		int light_it = 0;
		for (auto& e : results)
		{
			auto& it_light = results.get<Components::Point_Light>(e);
			auto& it_trans = results.get<Components::Transform>(e);

			const std::string currLight = "lights[" + std::to_string(light_it) + "].";
			//ShaderLinker::SetUniform((currLight + "Position").c_str(), it_trans.GetGlobalPosition());
			ShaderLinker::SetUniform((currLight + "Position").c_str(), it_trans.m_position);
			ShaderLinker::SetUniform((currLight + "Color").c_str(), it_light.m_Color * it_light.m_Intensity);

			// Attenuation
			const float k = 1.0f; // Constant
			ShaderLinker::SetUniform((currLight + "Linear").c_str(), it_light.m_Linear);
			ShaderLinker::SetUniform((currLight + "Quadratic").c_str(), it_light.m_Quadratic);
			const float& linear = it_light.m_Linear;
			const float& quad = it_light.m_Linear;
			const float maxBrightness = std::fmaxf(std::fmaxf(it_light.m_Color.r, it_light.m_Color.g), it_light.m_Color.b);
			float radius = (-linear + std::sqrtf(linear * linear - 4 * quad * (k - (256.0f / 5.0f) * maxBrightness))) / (2.0f * quad);
			ShaderLinker::SetUniform((currLight + "Radius").c_str(), radius);
			// Iteration count
			light_it++;
		}

		/// Shadow stuff
		ShaderLinker::SetUniform("m4_InverseView", glm::inverse(ret.m_View));
		ShaderLinker::SetUniform("lightDir", lightDir);
		ShaderLinker::SetUniform("farPlane", ret.m_Far);
		ShaderLinker::SetUniform("cascadeCount", static_cast<int>(ShadowCFG::m_CascadeIntervals.size()));
		for (size_t i = 0; i < ShadowCFG::m_CascadeIntervals.size(); ++i)
		{
			auto str = "cascadePlaneDistances[" + std::to_string(i) + "]";
			//ShaderLinker::SetUniform(str.c_str(), shadowCascadeLevels[i]);
			ShaderLinker::SetUniform(str.c_str(), ret.m_Far/ShadowCFG::m_CascadeIntervals[i]);
		}

		//ShaderLinker::SetUniform("viewPos", ret.m_Position);
		MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN, FBR::m_FrameBuffers[BufferType::G_POS_BUFFER]);
		ShaderLinker::UnUse();

		// Copy back
		glBindFramebuffer(GL_READ_FRAMEBUFFER, FBR::m_FrameBuffers[BufferType::G_BUFFER]);
		glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0); 
		glBlitFramebuffer(0, 0, Window::m_Width, Window::m_Height, 0, 0, Window::m_Width, Window::m_Height, GL_DEPTH_BUFFER_BIT, GL_NEAREST);
		glBindFramebuffer(GL_FRAMEBUFFER, 0);
	}

	void Renderer::ShadowPass(entt::registry& registry)
	{
		//// Default position in case directional light not in scene
		//glm::vec3 lightDir = glm::normalize(glm::vec3(20.0f, 50.0f, 20.0f));
		//// Directional light
		//auto dResults = registry.view<Components::Directional_Light, Components::Transform>();
		//auto dLight_it = dResults.begin();
		//if (dLight_it != dResults.end())
		//{
		//	const auto& dLight = dResults.get<const Components::Transform>(*dLight_it);
		//	lightDir = glm::normalize(dLight.m_position);
		//}

		// Editor cam by default;
		CameraReturnData ret = GetCurrentCamera(registry, false);

		// Light matrix config
		const auto lightMatrices = CalcAllLightSpaceMtx(ret, lightDir);
		//const auto lightMatrices = getLightSpaceMatrices(ret);
		glBindBuffer(GL_UNIFORM_BUFFER, FBR::m_FrameBuffers[BufferType::MATRICES_BUFFER]);
		for (size_t i = 0; i < lightMatrices.size(); ++i)
		{
			glBufferSubData(GL_UNIFORM_BUFFER, i * sizeof(glm::mat4x4), sizeof(glm::mat4x4), &lightMatrices[i]);
		}
		glBindBuffer(GL_UNIFORM_BUFFER, 0);

		/// Depth
		glBindFramebuffer(GL_FRAMEBUFFER, FBR::m_FrameBuffers[BufferType::LIGHT_BUFFER]);
		glFramebufferTexture(GL_FRAMEBUFFER, GL_TEXTURE_2D_ARRAY, FBR::m_FrameBuffers[BufferType::LIGHT_DEPTH_MAP], 0);
		glEnable(GL_DEPTH_TEST);
		glViewport(0, 0, ShadowCFG::m_ShadowMapW, ShadowCFG::m_ShadowMapH);
		glClear(GL_DEPTH_BUFFER_BIT);
		glCullFace(GL_FRONT);  // peter panning
		
		/// Scene
		ShaderLinker::Use("CSM");
		for (auto& model : ModelResource::m_ModelPool)
		{
			for (auto& mesh : model.second->m_Meshes)
			{
				mesh.second.DrawInstanced(false);
			}
		}
		ShaderLinker::UnUse();
		
		glCullFace(GL_BACK);
		glDisable(GL_DEPTH_TEST);
		glBindFramebuffer(GL_FRAMEBUFFER, 0);

		// reset viewport
		glViewport(0, 0, Window::m_Width, Window::m_Height);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		///Debug
		//glBindFramebuffer(GL_FRAMEBUFFER, FBR::m_FrameBuffers[BufferType::DEBUG_BUFFER]);
		//ShaderLinker::Use("CSM_D");
		//ShaderLinker::SetUniform("depthMap", 0);
		//ShaderLinker::SetUniform("layer", m_Layer);
		////ShaderLinker::SetUniform("near_plane", ret.m_Near);
		////ShaderLinker::SetUniform("far_plane", ret.m_Far);
		//glActiveTexture(GL_TEXTURE0);
		//glBindTexture(GL_TEXTURE_2D_ARRAY, FBR::m_FrameBuffers[BufferType::LIGHT_DEPTH_MAP]);
		//MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN);
		//ShaderLinker::UnUse();
		//glBindFramebuffer(GL_FRAMEBUFFER, 0);
	}

	void Renderer::BlurPass()
	{
		ShaderLinker::Use("BLUR");
		// Pass this first as first iteration
		ShaderLinker::SetUniform("horizontal", true);
		glBindFramebuffer(GL_FRAMEBUFFER, FBR::m_FrameBuffers[BufferType::BLUR_BUFFER_0]);
		MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN, FBR::m_FrameBuffers[BufferType::PP_COLOR_BUFFER_BRIGHT]);

		bool first_it = true;
		const float blur_increment = 0.10f;
		for (int i = 1; i <= 5; i++)
		{
			ShaderLinker::SetUniform("targetWidth", Window::m_Width * blur_increment * i);
			ShaderLinker::SetUniform("targetHeight", Window::m_Height * blur_increment * i);

			// Skip this one as we use the bright color as first pass
			if (!first_it)
			{
				ShaderLinker::SetUniform("horizontal", true);
				glBindFramebuffer(GL_FRAMEBUFFER, FBR::m_FrameBuffers[BufferType::BLUR_BUFFER_0]);
				MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN, FBR::m_FrameBuffers[BufferType::PP_COLOR_BUFFER_BRIGHT]);
			}

			ShaderLinker::SetUniform("horizontal", false);
			glBindFramebuffer(GL_FRAMEBUFFER, FBR::m_FrameBuffers[BufferType::BLUR_BUFFER_1]);
			MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN, FBR::m_FrameBuffers[BufferType::BLUR_COLOR_BUFFER_0]);

			// First pass complete
			first_it = false;
		}
		
		ShaderLinker::UnUse();
		glBindFramebuffer(GL_FRAMEBUFFER, 0);
	}

	void Renderer::HDRPass(bool _Bloom)
	{
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		ShaderLinker::Use("BLOOM");
		ShaderLinker::SetUniform("scene", 0);
		ShaderLinker::SetUniform("bloomBlur", 1);
		ShaderLinker::SetUniform("debug", 2);
		ShaderLinker::SetUniform("bloom", _Bloom);

		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::PP_COLOR_BUFFER_NORMAL]);
		glActiveTexture(GL_TEXTURE1);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::BLUR_COLOR_BUFFER_1]);
		glActiveTexture(GL_TEXTURE2);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::DEBUG_COLOR_BUFFER]);
		MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN);
		ShaderLinker::UnUse();
	}

	void Renderer::ClearBuffer(float _R, float _G, float _B, float _A)
	{
		// glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
		glClearColor(_R, _G, _B, _A);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	}

	void Renderer::SwapBuffer()
	{
		glfwSwapBuffers(Window::GetWindow());
		glfwPollEvents();
	}

	void Renderer::Draw(entt::registry& registry, Components::Renderer* _Selected, bool _EditorMode)
	{
		glEnable(GL_CULL_FACE);

		// Outline edge shit will be solved later
		(void)_Selected;

		// Editor cam by default;
		CameraReturnData ret = GetCurrentCamera(registry, _EditorMode);

		/*ShaderLinker::Use("SHAFT");
		ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
		ShaderLinker::SetUniform("m4_View", ret.m_View);
		glm::mat4 model = glm::mat4{ 1 };
		model = glm::translate(model, m_LightShaftPos);
		model = glm::rotate(model, glm::radians(180.0f), { 0,1,0 });
		model = glm::scale(model, { m_LightShaftScale,m_LightShaftScale,m_LightShaftScale });
		ShaderLinker::SetUniform("m4_Model", model);
		ShaderLinker::SetUniform("S2D_Texture", 0);
		ShaderLinker::SetUniform("colorTint", glm::vec3{ 0.9f,0.82f,0.565f });
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool["ParticleTest.dds"]);
		MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN);
		ShaderLinker::UnUse();*/

		// 3D models
		ShaderLinker::Use("MODEL_I");
		ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
		ShaderLinker::SetUniform("m4_View", ret.m_View);
		ShaderLinker::SetUniform("i_EditorMode", static_cast<int>(_EditorMode));
		for (auto& model : ModelResource::m_ModelPool)
		{
			for (auto& mesh : model.second->m_Meshes)
			{
				ShaderLinker::SetUniform("material.emissive_count", 0);
				mesh.second.DrawInstanced(true);
			}
		}
		ShaderLinker::UnUse();

		/// Skybox
		glDepthFunc(GL_LEQUAL);
		ShaderLinker::Use("SKYBOX");
		// Remove translate
		glm::mat4 skyboxView = glm::mat4(glm::mat3(ret.m_View));
		ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
		ShaderLinker::SetUniform("m4_View", skyboxView);
		Skybox::Draw(TextureResource::m_TexturePool["Skybox"]);
		ShaderLinker::UnUse();
		glDepthFunc(GL_LESS);

		/// Text
		DrawText(registry, _EditorMode);

		/// Canvas
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
		// Render G pass objects first
		ShaderLinker::Use("BASIC");
		MeshBuilder::RebindQuad();

		// Bind textures
		for (const auto& it : TextureResource::m_UsedTextures)
		{
			//printf("%s | %d\n", it.first.c_str(), it.second.m_MappedID);
			std::string uniformStr = "Textures[" + std::to_string(it.second.m_MappedID) + "]";
			ShaderLinker::SetUniform(uniformStr.c_str(), static_cast<int>(it.second.m_MappedID));
			glActiveTexture(GL_TEXTURE0 + it.second.m_MappedID);
			glBindTexture(GL_TEXTURE_2D, it.second.m_ID);
		}

		ShaderLinker::SetUniform("m4_Ortho", ret.m_Orthographic);
		ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
		ShaderLinker::SetUniform("m4_View", ret.m_View);
		ShaderLinker::SetUniform("b_Editor", _EditorMode);
		MeshResource::DrawInstanced(MeshResource::MESH_TYPE::QUAD);
		ShaderLinker::UnUse();
		glDisable(GL_BLEND);

		if (_EditorMode)
		{
			DrawDebug(registry, nullptr);
		}

		glDisable(GL_CULL_FACE);
	}

	void Renderer::DrawDebug(entt::registry& registry, Components::Renderer* _Selected)
	{
		// Outline edge thing will be solved later
		(void)_Selected;

		/// Editor cam by default, dont need to search for gam cam since this only appears in debug
		Camera4D* currCam = CameraResource::GetCamera("EDITOR");
		// Try to get game camera
		auto cam_results = registry.view<Components::Camera>();
		glm::mat4 projection = currCam->m_Projection;
		glm::mat4 view = currCam->GetView();


		/// Debug
		// Debug line size
		glLineWidth(DebugDraw::m_LineWidth);

		// Update verts to mesh
		MeshBuilder::RebindLines(DebugDraw::m_DebugVerts);

		ShaderLinker::Use("LINE");
		ShaderLinker::SetUniform("m4_Projection", projection);
		ShaderLinker::SetUniform("m4_View", view);
		ShaderLinker::SetUniform("m4_Model", glm::mat4{ 1 });

		// Debug color default to green for now
		ShaderLinker::SetUniform("colorTint", glm::vec3{ 0.0f, 1.0f, 0.0f });

		Mesh* lineMesh = MeshResource::m_MeshPool[MeshResource::MESH_TYPE::LINE];
		glBindVertexArray(lineMesh->m_VAO);
		glDrawArrays(GL_LINES, 0, lineMesh->m_IndicesCount);
		glBindVertexArray(0);

		if (m_RenderGrid)
		{
			/// Grid - thin
			// Clear for grid
			DebugDraw::NewFrame();

			// Debug grid size
			glLineWidth(DebugDraw::m_GridWidth);

			//int camFar = static_cast<int>(currCam->m_Far);
			const int camFar = 420;

			for (int i = -camFar; i <= camFar; i++)
			{
				if (i % DebugDraw::m_GridInterval != 0)
				{
					DebugDraw::DebugLine(glm::vec3{ i,0,-camFar }, glm::vec3{ i,0,camFar });
				}
			}
			for (int i = -camFar; i <= camFar; i++)
			{
				if (i % DebugDraw::m_GridInterval != 0)
				{
					DebugDraw::DebugLine(glm::vec3{ -camFar,0,i }, glm::vec3{ camFar,0,i });
				}
			}

			// Update verts to mesh
			MeshBuilder::RebindLines(DebugDraw::m_DebugVerts);

			ShaderLinker::SetUniform("colorTint", glm::vec3{ 0.42f, 0.42f, 0.42f });

			glBindVertexArray(lineMesh->m_VAO);
			glDrawArrays(GL_LINES, 0, lineMesh->m_IndicesCount);
			glBindVertexArray(0);

			/// Grid - thicc
			// Clear for grid
			DebugDraw::NewFrame();

			// Debug grid size
			glLineWidth(DebugDraw::m_GridIntervalWidth);

			for (int i = -camFar; i <= camFar; i += DebugDraw::m_GridInterval)
			{
				DebugDraw::DebugLine(glm::vec3{ i,0,-camFar }, glm::vec3{ i,0,camFar });
			}
			for (int i = -camFar; i <= camFar; i += DebugDraw::m_GridInterval)
			{
				DebugDraw::DebugLine(glm::vec3{ -camFar,0,i }, glm::vec3{ camFar,0,i });
			}

			// Update verts to mesh
			MeshBuilder::RebindLines(DebugDraw::m_DebugVerts);

			ShaderLinker::SetUniform("colorTint", glm::vec3{ 0.69f, 0.69f, 0.69f });

			glBindVertexArray(lineMesh->m_VAO);
			glDrawArrays(GL_LINES, 0, lineMesh->m_IndicesCount);
			glBindVertexArray(0);

			ShaderLinker::UnUse();
		}

		// Call this after or all points will be cleared
		DebugDraw::NewFrame();
	}

	void Renderer::DrawScreen()
	{
		// Unused atm, for final game
		ShaderLinker::Use("SCREEN");
		MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN, FBR::m_FrameBuffers[BufferType::GAME_COLOR_BUFFER]);
		ShaderLinker::UnUse();
	}

	void Renderer::DrawText(entt::registry& registry, bool _EditorMode)
	{
		//glEnable(GL_BLEND);
		//glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		// Editor cam by default;
		CameraReturnData ret = GetCurrentCamera(registry, _EditorMode);
		
		// Use shader
		ShaderLinker::Use("TEXT");

		auto results = registry.view<Components::Transform, Components::Text>(entt::exclude_t<Components::Prefab, Components::Disabled>());
		for (const auto& e : results)
		{
			auto& it_Text = results.get<Components::Text>(e);
			auto& it_Trans = results.get<Components::Transform>(e);

			Font* currFont = FontResource::m_FontPool[it_Text.m_FontID];

			/// Scale with window size
			glm::mat4 model = glm::mat4{ 1 };
			// Ortho or not
			if (!it_Text.m_Ortho)
			{
				ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
				ShaderLinker::SetUniform("m4_View", ret.m_View);	
				ShaderLinker::SetUniform("b_Editor", false);
				model = it_Trans.m_ModelMtx;
			}
			else
			{
				ShaderLinker::SetUniform("m4_Projection", ret.m_Orthographic);
				ShaderLinker::SetUniform("m4_View", glm::mat4{ 1 });

				// Don't allow ortho in editor mode
				ShaderLinker::SetUniform("b_Editor", _EditorMode);

				glm::vec3 pos = {};
				glm::vec3 rot = {};
				glm::vec3 scale = {};

				// To maintain parent transform
				ImGuizmo::DecomposeMatrixToComponents
				(
					glm::value_ptr(it_Trans.m_ModelMtx),
					glm::value_ptr(pos),
					glm::value_ptr(rot),
					glm::value_ptr(scale)
				);

				// Decompose to apply mapping
				model = glm::translate(model, { pos.x * Window::m_Width, pos.y * Window::m_Height, pos.z });
				model = glm::rotate(model, glm::radians(rot.x), { 1,0,0 });
				model = glm::rotate(model, glm::radians(rot.y), { 0,1,0 });
				model = glm::rotate(model, glm::radians(rot.z), { 0,0,1 });
				// Disregard aspect ratio so width for x and y
				model = glm::scale(model, { scale.x * Window::m_Width, scale.y * Window::m_Height * Window::m_TargetAspect, 1.0f });
			}

			ShaderLinker::SetUniform("m4_Model", glm::mat4{ model });
			ShaderLinker::SetUniform("textColor", it_Text.m_Color);
			glBindTexture(GL_TEXTURE_2D, currFont->m_TextureID);

			//glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
			float xAccumulate = 0;
			// Use this as the basis for all offsets
			const  auto& refChar = currFont->m_Font['A'];
			const float yOffset = -(refChar.m_Size.y - refChar.m_Offsets.y);
			for (const auto& it : it_Text.m_Text)
			{
				const auto& currChar = currFont->m_Font[it];
				ShaderLinker::SetUniform("offset", currChar.m_TexCoords);

				const float xPos = xAccumulate + currChar.m_Offsets.x;
				float yPos = -(currChar.m_Size.y - currChar.m_Offsets.y) - yOffset;
				//const float yPos = 0.0f; // By line

				const float width = currChar.m_Size.x;
				const float height = currChar.m_Size.y;

				MeshBuilder::RebindTextQuad(xPos, yPos, width, height, currChar.m_Size.x, currChar.m_Size.y);

				/// Draw
				glBindVertexArray(MeshResource::m_MeshPool[MeshResource::MESH_TYPE::TEXT_QUAD]->m_VAO);
				glDrawArrays(GL_TRIANGLES, 0, 6);
				glBindVertexArray(0);

				xAccumulate += currChar.m_Advance;
			}

			glBindTexture(GL_TEXTURE_2D, 0);
			//glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
		}
		ShaderLinker::UnUse();

		//glDisable(GL_BLEND);
	}
}