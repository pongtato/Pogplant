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

#include <iostream>

namespace Pogplant
{
	bool Renderer::m_RenderGrid = false;
	float Renderer::m_Exposure = 1.0f;
	float Renderer::m_Gamma = 2.2f;
	float Renderer::m_LightShaftDecay = 0.9f;
	float Renderer::m_LightShaftExposure = 0.2f;
	float Renderer::m_LightShaftDensity = 0.69f;
	float Renderer::m_LightShaftWeight = 0.3f;
	glm::vec3 Renderer::m_LightShaftPos = { -500.0f,500.0f,0.0f };
	float Renderer::m_LightShaftScale = 10.0f;

	/// QUAT TEST
	glm::vec3 Renderer::m_QuatTestPos = glm::vec3{ 0 };
	glm::vec3 Renderer::m_QuatTestRot = glm::vec3{ 0 };
	glm::vec3 Renderer::m_QuatTestScale = glm::vec3{ 1 };

	struct CameraReturnData
	{
		glm::mat4 m_Projection;
		glm::mat4 m_View;
		glm::vec3 m_Position;
		float m_Near;
		float m_Far;
	};

	/// Helper
	CameraReturnData GetCurrentCamera(const entt::registry& registry, bool _EditorMode)
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
				for (const auto& e : cam_results)
				{
					const auto& it_Trans = cam_results.get<const Components::Transform>(e);
					const auto& it_Camera = cam_results.get<const Components::Camera>(e);
					if (it_Camera.m_Active)
					{
						ret =
						{
							it_Camera.m_Projection,
							it_Camera.m_View,
							it_Trans.m_position,
							it_Camera.m_Near,
							it_Camera.m_Far,
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

		//Camera4D* currQCam = &CameraResource::m_QuatCam;
		//std::cout << "@" << currQCam->m_Projection.length() << std::endl;
		//std::cout << "@" << currQCam->m_View.length() << std::endl;
		//std::cout << "@" << currQCam->m_Position.x << "|" << currQCam->m_Position.y << "|" << currQCam->m_Position.z << std::endl;
		//std::cout << "@" << currQCam->m_Near << std::endl;
		//std::cout << "@" << currQCam->m_Far << std::endl;
		//std::cout << currQCam->m_Pitch << "|" << currQCam->m_Heading << std::endl;

		return CameraReturnData
		{
			/*currCam->GetPerspective(),
			currCam->GetView(),
			currCam->GetPosition(),
			currCam->GetCameraConfig().m_Near,
			currCam->GetCameraConfig().m_Far,*/
			currCam->m_Projection,
			currCam->GetView(),
			currCam->m_Position,
			currCam->m_Near,
			currCam->m_Far,
		};
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

	void Renderer::DebugPass(const entt::registry& registry)
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

	void Renderer::GLightPass(const entt::registry& registry, bool _EditorMode)
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
		//glActiveTexture(GL_TEXTURE5);
		//glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_SHAFT_BUFFER]);
		glActiveTexture(GL_TEXTURE6);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::SHADOW_DEPTH]);

		ShaderLinker::SetUniform("Exposure", m_Exposure);
		ShaderLinker::SetUniform("Gamma", m_Gamma);

		///// Shaft pos screen
		//// Editor cam by default;
		CameraReturnData ret = GetCurrentCamera(registry, _EditorMode);
		//auto clipSpace = ret.m_Projection * (ret.m_View * glm::vec4(m_LightShaftPos,1.0f));
		//auto ndc = glm::vec3{ clipSpace.x,clipSpace.y,clipSpace.z } / clipSpace.w;
		//auto windowSpace = glm::vec2{ ndc.x + 1.0f, ndc.y + 1.0f } / 2.0f;
		//ShaderLinker::SetUniform("ScreenSize", glm::vec2{ Window::m_Width, Window::m_Height });
		//ShaderLinker::SetUniform("LightScreenPos", windowSpace);
		//ShaderLinker::SetUniform("Decay", m_LightShaftDecay);
		//ShaderLinker::SetUniform("Exposure", m_LightShaftExposure);
		//ShaderLinker::SetUniform("Density", m_LightShaftDensity);
		//ShaderLinker::SetUniform("Weight", m_LightShaftWeight);

		/// Lights
		// Directional
		ShaderLinker::SetUniform("m4_LightProjection", ShadowCFG::m_LightProj);
		auto dResults = registry.view<Components::Directional_Light, Components::Transform>();
		auto dLight_it = dResults.begin();
		if (dLight_it != dResults.end())
		{
			const auto& dLight = dResults.get<const Components::Directional_Light>(*dLight_it);
			const std::string currLight = "directLight.";
			ShaderLinker::SetUniform((currLight + "Direction").c_str(), dLight.m_Direction);
			ShaderLinker::SetUniform((currLight + "Color").c_str(), dLight.m_Color * dLight.m_Intensity);
			ShaderLinker::SetUniform((currLight + "Diffuse").c_str(), dLight.m_Diffuse);
			ShaderLinker::SetUniform((currLight + "Specular").c_str(), dLight.m_Specular);
			ShaderLinker::SetUniform((currLight + "Direction").c_str(), dLight.m_Direction);
		}

		// Point lights
		auto results = registry.view<Components::Point_Light, Components::Transform>();
		ShaderLinker::SetUniform("activeLights", static_cast<int>(results.size_hint()));
		int light_it = 0;
		for (const auto& e : results)
		{
			const auto& it_light = results.get<const Components::Point_Light>(e);
			const auto& it_trans = results.get<const Components::Transform>(e);

			const std::string currLight = "lights[" + std::to_string(light_it) + "].";
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

		ShaderLinker::SetUniform("viewPos", ret.m_Position);
		MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN, FBR::m_FrameBuffers[BufferType::G_POS_BUFFER]);
		ShaderLinker::UnUse();

		// Copy back
		glBindFramebuffer(GL_READ_FRAMEBUFFER, FBR::m_FrameBuffers[BufferType::G_BUFFER]);
		glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0); 
		glBlitFramebuffer(0, 0, Window::m_Width, Window::m_Height, 0, 0, Window::m_Width, Window::m_Height, GL_DEPTH_BUFFER_BIT, GL_NEAREST);
		glBindFramebuffer(GL_FRAMEBUFFER, 0);
	}

	void Renderer::ShadowPass(const entt::registry& registry)
	{
		// Default position in case directional light not in scene
		glm::vec3 lightPos = { 0,0,0 };
		// Directional light
		auto dResults = registry.view<Components::Directional_Light, Components::Transform>();
		auto dLight_it = dResults.begin();
		if (dLight_it != dResults.end())
		{
			const auto& dLight = dResults.get<const Components::Transform>(*dLight_it);
			lightPos = dLight.m_position;
		}

		// Editor cam by default;
		CameraReturnData ret = GetCurrentCamera(registry, false);

		//ret.m_Far = 200.0f;
		glm::mat4 orthogalProj = glm::ortho(-ret.m_Far, ret.m_Far, -ret.m_Far, ret.m_Far, ret.m_Near, ret.m_Far);
		glm::mat4 lightView = glm::lookAt(lightPos, glm::vec3{ 0 }, glm::vec3{ 0.0f, 1.0f, 0.0f });
		ShadowCFG::m_LightProj = orthogalProj * lightView;

		glEnable(GL_DEPTH_TEST);
		glViewport(0, 0, ShadowCFG::m_ShadowMapW, ShadowCFG::m_ShadowMapH);
		glBindFramebuffer(GL_FRAMEBUFFER, FrameBufferResource::m_FrameBuffers[BufferType::SHADOW_BUFFER]);
		glClear(GL_DEPTH_BUFFER_BIT);

		/// Draw
		ShaderLinker::Use("SHADOW_I");
		ShaderLinker::SetUniform("m4_LightProjection", ShadowCFG::m_LightProj);
		// 3D Instanced
		for (auto& model : ModelResource::m_ModelPool)
		{
			for (auto& mesh : model.second->m_Meshes)
			{
				mesh.second.DrawInstanced(false);
			}
		}
		ShaderLinker::UnUse();

		ShaderLinker::Use("SHADOW");
		ShaderLinker::SetUniform("m4_LightProjection", ShadowCFG::m_LightProj);
		auto p_results = registry.view<Components::PrimitiveRender, Components::Transform>();
		for (const auto& e : p_results)
		{
			const auto& it = p_results.get<const Components::PrimitiveRender>(e);
			const auto& it_trans = p_results.get<const Components::Transform>(e);
			ShaderLinker::SetUniform("m4_Model", it_trans.m_ModelMtx);
			it.m_Mesh->Draw();
		}
		ShaderLinker::UnUse();
		///

		glDisable(GL_DEPTH_TEST);

		glBindFramebuffer(GL_FRAMEBUFFER, 0);

		glViewport(0, 0, Window::m_Width, Window::m_Height);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		/// Debug draw
		//glBindFramebuffer(GL_FRAMEBUFFER, FrameBufferResource::m_FrameBuffers[BufferType::PP_BUFFER]);
		//ShaderLinker::Use("DEPTH");
		//MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN, FBR::m_FrameBuffers[BufferType::SHADOW_DEPTH]);
		//ShaderLinker::UnUse();
		//glBindFramebuffer(GL_FRAMEBUFFER,0);
	}

	void Renderer::BlurPass()
	{
		ShaderLinker::Use("BLUR");
		// Pass this first as first iteration
		ShaderLinker::SetUniform("horizontal", true);
		glBindFramebuffer(GL_FRAMEBUFFER, FBR::m_FrameBuffers[BufferType::BLUR_BUFFER_0]);
		MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN, FBR::m_FrameBuffers[BufferType::PP_COLOR_BUFFER_BRIGHT]);

		bool first_it = true;
		const float blur_increment = 0.15f;
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

	void Renderer::Draw(const entt::registry& registry, Components::Renderer* _Selected, bool _EditorMode)
	{
		glEnable(GL_CULL_FACE);

		// Outline edge shit will be solved later
		(void)_Selected;

		// Editor cam by default;
		CameraReturnData ret = GetCurrentCamera(registry, _EditorMode);

		// Render G pass objects first
		//glEnable(GL_BLEND);
		//glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
		ShaderLinker::Use("BASIC");
		MeshBuilder::RebindQuad();
		// Bind textures
		for (const auto& it : TextureResource::m_TexturePool)
		{
			auto mapped_id = TextureResource::m_UsedTextures[it.second];
			std::string uniformStr = "Textures[" + std::to_string(mapped_id) + "]";
			ShaderLinker::SetUniform(uniformStr.c_str(), static_cast<int>(mapped_id));
			glActiveTexture(GL_TEXTURE0 + mapped_id);
			glBindTexture(GL_TEXTURE_2D, it.second);
		}

		ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
		ShaderLinker::SetUniform("m4_View", ret.m_View);
		ShaderLinker::SetUniform("f_Aspect", Pogplant::Window::m_Aspect);
		ShaderLinker::SetUniform("b_Editor", _EditorMode);
		MeshResource::DrawInstanced(MeshResource::MESH_TYPE::QUAD);
		ShaderLinker::UnUse();
		//glDisable(GL_BLEND);

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

		// lol xd
		//glDisable(GL_CULL_FACE);
		
		/// QUAT TEST
		//ShaderLinker::Use("MODEL");
		//ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
		//ShaderLinker::SetUniform("m4_View", ret.m_View);
		//ShaderLinker::SetUniform("noTex", true);
		//ShaderLinker::SetUniform("useLight", true);
		//ShaderLinker::SetUniform("colorTint", glm::vec3{ 1,1,1 });

		//glm::mat4 t = glm::translate( glm::mat4{1}, m_QuatTestPos );

		//glm::quat xR = glm::angleAxis(glm::radians(m_QuatTestRot.x), glm::vec3{ 1,0,0 });
		//glm::quat yR = glm::angleAxis(glm::radians(m_QuatTestRot.y), glm::vec3{ 0,1,0 });
		//glm::quat zR = glm::angleAxis(glm::radians(m_QuatTestRot.z), glm::vec3{ 0,0,1 });
		//glm::mat4 r = glm::mat4_cast(yR * xR * zR);
		//glm::mat4 s = glm::scale(glm::mat4{ 1 }, m_QuatTestScale);

		//ShaderLinker::SetUniform("m4_Model", t * r * s);
		//auto it = std::next(ModelResource::m_ModelPool.begin(),1);
		//it->second->Draw(false);
		//ShaderLinker::UnUse();
		///

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
		//glEnable(GL_CULL_FACE);

		// Primitive shapes
		ShaderLinker::Use("PRIMITIVE");
		ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
		ShaderLinker::SetUniform("m4_View", ret.m_View);
		auto p_results = registry.view<Components::PrimitiveRender, Components::Transform>();
		for (const auto& e : p_results)
		{
			const auto& it_pRender = p_results.get<const Components::PrimitiveRender>(e);
			const auto& it_trans = p_results.get<const Components::Transform>(e);

			//ShaderLinker::SetUniform("activeTextures", static_cast<int>(it.m_DiffTex.size()));
			ShaderLinker::SetUniform("tile", it_pRender.m_Blend);
			ShaderLinker::SetUniform("m4_Model", it_trans.m_ModelMtx);
			ShaderLinker::SetUniform("v3_ViewPosition", ret.m_Position);

			ShaderLinker::SetUniform("texture_diffuse", 0);
			ShaderLinker::SetUniform("texture_normal", 1);
			ShaderLinker::SetUniform("texture_disp", 2);
			ShaderLinker::SetUniform("texture_specular", 3);

			ShaderLinker::SetUniform("texture_diffuse2", 4);
			ShaderLinker::SetUniform("texture_normal2", 5);
			ShaderLinker::SetUniform("texture_disp2", 6);
			ShaderLinker::SetUniform("texture_specular2", 7);

			/// WIP, to be updated
			// Tex 1
			glActiveTexture(GL_TEXTURE0);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it_pRender.m_DiffTex[0]]);
			glActiveTexture(GL_TEXTURE1);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it_pRender.m_NormTex[0]]);
			glActiveTexture(GL_TEXTURE2);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it_pRender.m_BumpTex[0]]);
			glActiveTexture(GL_TEXTURE3);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it_pRender.m_SpecTex[0]]);

			// Tex 2
			glActiveTexture(GL_TEXTURE4);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it_pRender.m_DiffTex[1]]);
			glActiveTexture(GL_TEXTURE5);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it_pRender.m_NormTex[1]]);
			glActiveTexture(GL_TEXTURE6);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it_pRender.m_BumpTex[1]]);
			glActiveTexture(GL_TEXTURE7);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it_pRender.m_SpecTex[1]]);

			it_pRender.m_Mesh->Draw();
		}
		ShaderLinker::UnUse();

		// Skybox
		glDepthFunc(GL_LEQUAL);
		ShaderLinker::Use("SKYBOX");
		// Remove translate
		ret.m_View = glm::mat4(glm::mat3(ret.m_View));
		ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
		ShaderLinker::SetUniform("m4_View", ret.m_View);
		Skybox::Draw(TextureResource::m_TexturePool["Skybox"]);
		ShaderLinker::UnUse();
		glDepthFunc(GL_LESS);

		DrawText(registry, _EditorMode);

		if (_EditorMode)
		{
			DrawDebug(registry, nullptr);
		}

		glDisable(GL_CULL_FACE);
	}

	void Renderer::DrawDebug(const entt::registry& registry, Components::Renderer* _Selected)
	{
		// Outline edge thing will be solved later
		(void)_Selected;

		/// Editor cam by default, dont need to search for gam cam since this only appears in debug
		Camera4D* currCam = CameraResource::GetCamera("EDITOR");
		//Camera4D* currCam = &CameraResource::m_QuatCam;
		// Try to get game camera
		auto cam_results = registry.view<Components::Camera>();
		//glm::mat4 projection = currCam->GetPerspective();
		//glm::mat4 view = currCam->GetView();
		glm::mat4 projection = currCam->m_Projection;
		glm::mat4 view = currCam->GetView();

		/// What does this do again i cant rmb lmao
		/*ShaderLinker::Use("DEBUG");
		ShaderLinker::SetUniform("m4_Projection", projection);
		ShaderLinker::SetUniform("m4_View", view);

		auto results = registry.view<Components::DebugRender, Components::Transform>();

		for (const auto& e : results)
		{
			const auto& it = results.get<const Components::DebugRender>(e);
			const auto& it_trans = results.get<const Components::Transform>(e);
			ShaderLinker::SetUniform("m4_Model", it_trans.m_ModelMtx);
			ShaderLinker::SetUniform("colorTint", it.m_ColorTint);
			it.m_Mesh->Draw(false);
		}
		ShaderLinker::UnUse();*/

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

			int camFar = static_cast<int>(currCam->m_Far);

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

	void Renderer::Draw(const float(&_View)[16], const float(&_Ortho)[16], const float(&_Perspective)[16])
	{
		(void)_Ortho;
		(void)_View;
		(void)_Perspective;

		// Use the other draw function after casting to glm type
		// ... to do
		// do we even need this?
	}

	void Renderer::DrawScreen()
	{
		// Unused atm, for final game
		ShaderLinker::Use("SCREEN");
		MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN, FBR::m_FrameBuffers[BufferType::GAME_COLOR_BUFFER]);
		ShaderLinker::UnUse();
	}

	void Renderer::BindTexture(int _Location, unsigned _TexID)
	{
		/*ShaderLinker::Use("BASIC");
		std::string append = "Textures[";
		append = append + std::to_string(_Location) + "]";
		ShaderLinker::SetUniform(append.c_str(), _Location);
		glBindTextureUnit(static_cast<GLint>(_Location), _TexID);
		ShaderLinker::UnUse();*/
	}

	void Renderer::DrawText(const entt::registry& registry, bool _EditorMode)
	{
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		// Editor cam by default;
		CameraReturnData ret = GetCurrentCamera(registry, _EditorMode);
		
		// Use shader
		ShaderLinker::Use("TEXT");

		auto results = registry.view<Components::Text, Components::Transform>();
		for (const auto& e : results)
		{
			const auto& it_Text = results.get<const Components::Text>(e);
			const auto& it_Trans = results.get<const Components::Transform>(e);

			Font* currFont = FontResource::m_FontPool[it_Text.m_FontID];

			// Ortho or not
			if (!it_Text.m_Ortho)
			{
				ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
				ShaderLinker::SetUniform("m4_View", ret.m_View);
				ShaderLinker::SetUniform("f_Aspect", 1.0f);
			}
			else
			{
				ShaderLinker::SetUniform("m4_Projection", glm::mat4{ 1 });
				ShaderLinker::SetUniform("m4_View", glm::mat4{ 1 });
				ShaderLinker::SetUniform("f_Aspect", Window::m_Aspect);
			}

			glm::mat4 model = glm::mat4{ 1 };
			model = glm::translate(model, { it_Trans.m_position.x,it_Trans.m_position.y, it_Trans.m_position.z });
			glm::quat rot_z = glm::angleAxis(glm::radians(it_Trans.m_rotation.z), glm::vec3{ 0.0f,0.0f,1.0f });
			glm::quat rot_y = glm::angleAxis(glm::radians(it_Trans.m_rotation.y), glm::vec3{ 0.0f,1.0f,0.0f });
			glm::quat rot_x = glm::angleAxis(glm::radians(it_Trans.m_rotation.x), glm::vec3{ 1.0f,0.0f,0.0f });
			glm::mat4 m4_rot = glm::mat4_cast(rot_y * rot_x * rot_z);
			glm::mat4 m4_scale = glm::scale(glm::mat4{ 1 }, it_Trans.m_scale);

			ShaderLinker::SetUniform("m4_Model", glm::mat4{ model * m4_rot * m4_scale });
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

		glDisable(GL_BLEND);
	}
}