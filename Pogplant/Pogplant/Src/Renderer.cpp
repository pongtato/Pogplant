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

#include "FontResource.h"
#include "CameraResource.h"
#include "MeshResource.h"
#include "ModelResource.h"
#include "FrameBufferResource.h"
#include "TextureResource.h"

#include <gtc/matrix_transform.hpp>
#include <glew.h>
#include <glfw3.h>
#include <string>

#include <iostream>

#include "../../Pogplant Driver/Src/ECS/Components/Components.h"

namespace Pogplant
{
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
						Logger::Log({ "PP::RENDERER", LogEntry::TYPE::WARNING, "No active game cameras!" });
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
				Logger::Log({ "PP::RENDERER", LogEntry::TYPE::ERROR, "No game camera found, default to editor camera" });
			}
		}

		// Default to editor cam
		Camera* currCam = CameraResource::GetCamera("EDITOR");
		return CameraReturnData
		{
			currCam->GetPerspective(),
			currCam->GetView(),
			currCam->GetPosition(),
			currCam->GetCameraConfig().m_Near,
			currCam->GetCameraConfig().m_Far,
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
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::SHADOW_DEPTH]);

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

		/// Editor cam by default;
		CameraReturnData ret = GetCurrentCamera(registry, _EditorMode);

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

		glm::mat4 orthogalProj = glm::ortho(-ret.m_Far, ret.m_Far, -ret.m_Far, ret.m_Far, ret.m_Near, ret.m_Far);
		glm::mat4 lightView = glm::lookAt(lightPos, glm::vec3{ 0 }, glm::vec3{ 0.0f, 1.0f, 0.0f });
		ShadowCFG::m_LightProj = orthogalProj * lightView;

		ShaderLinker::Use("SHADOW");
		ShaderLinker::SetUniform("m4_LightProjection", ShadowCFG::m_LightProj);

		glEnable(GL_DEPTH_TEST);
		glViewport(0, 0, ShadowCFG::m_ShadowMapW, ShadowCFG::m_ShadowMapH);
		glBindFramebuffer(GL_FRAMEBUFFER, FrameBufferResource::m_FrameBuffers[BufferType::SHADOW_BUFFER]);
		glClear(GL_DEPTH_BUFFER_BIT);

		/// Draw
		auto results = registry.view<Components::Renderer>();
		for (const auto& e : results)
		{
			const auto& it = results.get<const Components::Renderer>(e);
			ShaderLinker::SetUniform("m4_Model", it.m_Model);
			if (it.m_UseLight)
			{
				it.m_RenderModel->Draw();
			}
		}

		auto p_results = registry.view<Components::PrimitiveRender>();
		for (const auto& e : p_results)
		{
			const auto& it = p_results.get<const Components::PrimitiveRender>(e);
			ShaderLinker::SetUniform("m4_Model", it.m_Model);
			it.m_Mesh->Draw();
		}
		///

		glDisable(GL_DEPTH_TEST);

		glBindFramebuffer(GL_FRAMEBUFFER, 0);
		ShaderLinker::UnUse();

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
		bool horizontal = true;
		bool first_it = true;
		ShaderLinker::Use("BLUR");
		const size_t blur_amnt = 10;
		for (size_t i = 0; i < blur_amnt; i++)
		{
			BufferType currFBuff = horizontal == true ? BufferType::BLUR_BUFFER_1 : BufferType::BLUR_BUFFER_0;
			glBindFramebuffer(GL_FRAMEBUFFER, FBR::m_FrameBuffers[currFBuff]);
			ShaderLinker::SetUniform("horizontal", horizontal);
			BufferType targetCBuff = horizontal == true ? BufferType::BLUR_COLOR_BUFFER_0 : BufferType::BLUR_COLOR_BUFFER_1;
			BufferType currCBuff = first_it == true ? BufferType::PP_COLOR_BUFFER_BRIGHT : targetCBuff;
			MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN, FBR::m_FrameBuffers[currCBuff]);
			horizontal = !horizontal;
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
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::BLUR_COLOR_BUFFER_0]);
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
		ShaderLinker::Use("MODEL");
		ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
		ShaderLinker::SetUniform("m4_View", ret.m_View);

		auto results = registry.view<Components::Renderer>();
		for (const auto& e : results)
		{
			const auto& it = results.get<const Components::Renderer>(e);

			ShaderLinker::SetUniform("m4_Model", it.m_Model);
			ShaderLinker::SetUniform("colorTint", it.m_ColorTint);
			ShaderLinker::SetUniform("useLight", it.m_UseLight);
			if (!it.m_EditorDrawOnly || it.m_EditorDrawOnly && _EditorMode)
			{
				it.m_RenderModel->Draw();
			}
		}
		ShaderLinker::UnUse();

		// Primitive shapes
		ShaderLinker::Use("PRIMITIVE");
		ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
		ShaderLinker::SetUniform("m4_View", ret.m_View);
		auto p_results = registry.view<Components::PrimitiveRender>();
		for (const auto& e : p_results)
		{
			const auto& it = p_results.get<const Components::PrimitiveRender>(e);
			ShaderLinker::SetUniform("activeTextures", static_cast<int>(it.m_DiffTex.size()));
			ShaderLinker::SetUniform("blend", it.m_Blend);
			ShaderLinker::SetUniform("m4_Model", it.m_Model);
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
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it.m_DiffTex[0]]);
			glActiveTexture(GL_TEXTURE1);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it.m_NormTex[0]]);
			glActiveTexture(GL_TEXTURE2);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it.m_BumpTex[0]]);
			glActiveTexture(GL_TEXTURE3);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it.m_SpecTex[0]]);

			// Tex 2
			glActiveTexture(GL_TEXTURE4);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it.m_DiffTex[1]]);
			glActiveTexture(GL_TEXTURE5);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it.m_NormTex[1]]);
			glActiveTexture(GL_TEXTURE6);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it.m_BumpTex[1]]);
			glActiveTexture(GL_TEXTURE7);
			glBindTexture(GL_TEXTURE_2D, TextureResource::m_TexturePool[it.m_SpecTex[1]]);

			it.m_Mesh->Draw();
		}
		ShaderLinker::UnUse();

		// Debug boxes
		if (_EditorMode)
		{
			// Debug line size
			glLineWidth(DebugDraw::m_LineWidth);

			// Update verts to mesh
			MeshBuilder::RebindLines(DebugDraw::m_DebugVerts);

			ShaderLinker::Use("LINE");
			ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
			ShaderLinker::SetUniform("m4_View", ret.m_View);
			ShaderLinker::SetUniform("m4_Model", glm::mat4{ 1 });
			// Debug color default to gree for now
			ShaderLinker::SetUniform("colorTint", glm::vec3{ 0.0f,1.0f,0.0f });
		
			Mesh* lineMesh = MeshResource::m_MeshPool[MeshResource::MESH_TYPE::LINE];
			glBindVertexArray(lineMesh->m_VAO);
			glDrawArrays(GL_LINES, 0, lineMesh->m_IndicesCount);
			glBindVertexArray(0);

			ShaderLinker::UnUse();
			// Call this after or all points will be cleared
			DebugDraw::NewFrame();
		}

		// Skybox
		glDepthFunc(GL_LEQUAL);
		ShaderLinker::Use("SKYBOX");
		// Remove translate
		ret.m_View = glm::mat4(glm::mat3(ret.m_View));
		ShaderLinker::SetUniform("m4_Projection", ret.m_Projection);
		ShaderLinker::SetUniform("m4_View", ret.m_View);
		Skybox::Draw(TextureResource::m_TexturePool["SKYBOX"]);
		ShaderLinker::UnUse();
		glDepthFunc(GL_LESS);

		DrawText(registry, _EditorMode);

		//ShaderLinker::Use("BASIC");
		//ShaderLinker::SetUniform("m4_Projection", currCam->GetPerspective());
		//ShaderLinker::SetUniform("m4_View", currCam->GetView());
		//MeshResource::DrawInstanced(MeshResource::MESH_TYPE::QUAD);
		//ShaderLinker::UnUse();

		//// Edge
		//if (_Selected != nullptr)
		//{
		//	// Test against the selected object
		//	glStencilFunc(GL_ALWAYS, 1, 0xFF);
		//	glStencilMask(0xFF);

		//	// Drawn seperately 
		//	ShaderLinker::Use("MODEL");
		//	ShaderLinker::SetUniform("m4_Projection", currCam->GetPerspective());
		//	ShaderLinker::SetUniform("m4_View", currCam->GetView());
		//	ShaderLinker::SetUniform("m4_Model", _Selected->m_Model);
		//	_Selected->m_RenderModel->Draw();
		//	ShaderLinker::UnUse();

		//	// Get the edge
		//	glStencilFunc(GL_NOTEQUAL, 1, 0xFF);
		//	glStencilMask(0x00);
		//	glDisable(GL_DEPTH_TEST);

		//	ShaderLinker::Use("EDGE");
		//	ShaderLinker::SetUniform("f_Thicc", 0.05f);
		//	ShaderLinker::SetUniform("m4_Projection", currCam->GetPerspective());
		//	ShaderLinker::SetUniform("m4_View", currCam->GetView());
		//	ShaderLinker::SetUniform("m4_Model", _Selected->m_Model);
		//	_Selected->m_RenderModel->Draw();
		//	ShaderLinker::UnUse();

		//	glStencilMask(0xFF);
		//	glStencilFunc(GL_ALWAYS, 0, 0xFF);
		//	glEnable(GL_DEPTH_TEST);
		//}

		glDisable(GL_CULL_FACE);
	}

	void Renderer::DrawDebug(const entt::registry& registry, Components::Renderer* _Selected)
	{
		glEnable(GL_CULL_FACE);

		// Outline edge thing will be solved later
		(void)_Selected;

		/// Editor cam by default, dont need to search for gam cam since this only appears in debug
		Camera* currCam = CameraResource::GetCamera("EDITOR");
		// Try to get game camera
		auto cam_results = registry.view<Components::Camera>();
		glm::mat4 projection = currCam->GetPerspective();
		glm::mat4 view = currCam->GetView();

		// Render G pass objects first
		ShaderLinker::Use("DEBUG");
		ShaderLinker::SetUniform("m4_Projection", projection);
		ShaderLinker::SetUniform("m4_View", view);

		auto results = registry.view<Components::DebugRender>();

		for (const auto& e : results)
		{
			const auto& it = results.get<const Components::DebugRender>(e);
			ShaderLinker::SetUniform("m4_Model", it.m_Model);
			ShaderLinker::SetUniform("colorTint", it.m_ColorTint);
			it.m_RenderModel->Draw();
		}
		ShaderLinker::UnUse();

		glDisable(GL_CULL_FACE);
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
		ShaderLinker::Use("BASIC");
		std::string append = "Textures[";
		append = append + std::to_string(_Location) + "]";
		ShaderLinker::SetUniform(append.c_str(), _Location);
		glBindTextureUnit(static_cast<GLint>(_Location), _TexID);
		ShaderLinker::UnUse();
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
			}
			else
			{
				ShaderLinker::SetUniform("m4_Projection", glm::mat4{ 1 });
				ShaderLinker::SetUniform("m4_View", glm::mat4{ 1 });
			}

			glm::mat4 model = glm::mat4{ 1 };
			// Magic padding to shift font down lmao
			model = glm::translate(model, { it_Trans.m_position.x,it_Trans.m_position.y - it_Trans.m_scale.y * 0.85f, it_Trans.m_position.z });
			model = glm::rotate(model, glm::radians(it_Trans.m_rotation.x), glm::vec3{ 1.0f,0.0f,0.0f });
			model = glm::rotate(model, glm::radians(it_Trans.m_rotation.y), glm::vec3{ 0.0f,1.0f,0.0f });
			model = glm::rotate(model, glm::radians(it_Trans.m_rotation.z), glm::vec3{ 0.0f,0.0f,1.0f });
			model = glm::scale(model, it_Trans.m_scale);

			ShaderLinker::SetUniform("m4_Model", glm::mat4{ model });
			ShaderLinker::SetUniform("distanceRange", currFont->m_DistanceRange);
			ShaderLinker::SetUniform("textColor", it_Text.m_Color);
			glBindTexture(GL_TEXTURE_2D, currFont->m_TextureID);

			//glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
			float xAccumulate = 0;
			for (const auto& it : it_Text.m_Text)
			{
				const auto& currChar = currFont->m_Font[it];
				ShaderLinker::SetUniform("offset", currChar.m_TexCoords);

				const float xPos = xAccumulate + currChar.m_Offsets.x;
				const float yPos = -(currChar.m_Size.y - currChar.m_Offsets.y);
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