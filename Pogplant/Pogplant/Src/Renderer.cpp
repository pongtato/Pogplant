#include "Renderer.h"
#include "Window.h"
#include "FrameBufferResource.h"
#include "FrameBuffer.h"
#include "ShaderLinker.h"
#include "CameraResource.h"
#include "MeshResource.h"
#include "ModelResource.h"
#include "Model.h"
#include "Skybox.h"
#include "TextureResource.h"

#include <gtc/matrix_transform.hpp>
#include <glew.h>
#include <glfw3.h>
#include <string>

#include <iostream>

#include "../../Pogplant Driver/Src/ECS/Components/Components.h"

namespace Pogplant
{
	void Renderer::StartEditorBuffer()
	{
		FrameBuffer::BindFrameBuffer(BufferType::EDITOR_BUFFER);
		glEnable(GL_DEPTH_TEST);
		glEnable(GL_STENCIL_TEST);
		glStencilOp(GL_KEEP, GL_KEEP, GL_REPLACE);
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

	void Renderer::EndBuffer()
	{
		FrameBuffer::UnbindFrameBuffer();
		glDisable(GL_DEPTH_TEST);
	}

	void Renderer::PostProcess()
	{
		FrameBuffer::BindFrameBuffer(BufferType::PP_BUFFER);
	}

	void Renderer::GLightPass(const char* _CameraID, const entt::registry& registry)
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

		// Lights

		// Directional
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

			// update attenuation parameters and calculate radius
			const float k = 1.0f; // Constant
			ShaderLinker::SetUniform((currLight + "Linear").c_str(), it_light.m_Linear);
			ShaderLinker::SetUniform((currLight + "Quadratic").c_str(), it_light.m_Quadratic);
			const float& linear = it_light.m_Linear;
			const float& quad = it_light.m_Linear;
			// then calculate radius of light volume/sphere
			const float maxBrightness = std::fmaxf(std::fmaxf(it_light.m_Color.r, it_light.m_Color.g), it_light.m_Color.b);
			float radius = (-linear + std::sqrt(linear * linear - 4 * quad * (k - (256.0f / 5.0f) * maxBrightness))) / (2.0f * quad);
			ShaderLinker::SetUniform((currLight + "Radius").c_str(), radius);
			// Iteration count
			light_it++;
		}

		Camera* currCam = CameraResource::GetCamera(_CameraID);
		ShaderLinker::SetUniform("viewPos", currCam->GetPosition());

		MeshResource::Draw(MeshResource::MESH_TYPE::SCREEN, FBR::m_FrameBuffers[BufferType::G_POS_BUFFER]);
		ShaderLinker::UnUse();

		// Copy back
		glBindFramebuffer(GL_READ_FRAMEBUFFER, FBR::m_FrameBuffers[BufferType::G_BUFFER]);
		glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0); 
		glBlitFramebuffer(0, 0, Window::m_Width, Window::m_Height, 0, 0, Window::m_Width, Window::m_Height, GL_DEPTH_BUFFER_BIT, GL_NEAREST);
		glBindFramebuffer(GL_FRAMEBUFFER, 0);
	}

	void Renderer::ClearBuffer(float _R, float _G, float _B, float _A)
	{
		//glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
		glClearColor(_R, _G, _B, _A);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);
	}

	void Renderer::SwapBuffer()
	{
		glfwSwapBuffers(Window::GetWindow());
		glfwPollEvents();
	}

	void Renderer::Draw(const char* _CameraID, const entt::registry& registry, Components::RenderObject* _Selected)
	{
		// Outline edge shit will be solved later
		(void)_Selected;

		Camera* currCam = CameraResource::GetCamera(_CameraID);

		// Render G pass objects first
		ShaderLinker::Use("MODEL");
		ShaderLinker::SetUniform("m4_Projection", currCam->GetPerspective());
		ShaderLinker::SetUniform("m4_View", currCam->GetView());

		auto results = registry.view<Components::RenderObject>();

		for (const auto& e : results)
		{
			// Draw selected objects seperately for edge detection
			const auto& it = results.get<const Components::RenderObject>(e);

			ShaderLinker::SetUniform("m4_Model", it.m_Model);
			ShaderLinker::SetUniform("useLight", it.m_UseLight);
			ShaderLinker::SetUniform("colorTint", it.m_ColorTint);
			it.m_RenderModel->Draw();
		}
		ShaderLinker::UnUse();

		//ShaderLinker::Use("BASIC");
		//ShaderLinker::SetUniform("m4_Projection", currCam->GetPerspective());
		//ShaderLinker::SetUniform("m4_View", currCam->GetView());
		//MeshResource::DrawInstanced(MeshResource::MESH_TYPE::QUAD);
		//ShaderLinker::UnUse();

		// Skybox
		glDepthFunc(GL_LEQUAL);
		ShaderLinker::Use("SKYBOX");
		// Remove translate
		glm::mat4 view = glm::mat4(glm::mat3(currCam->GetView()));
		ShaderLinker::SetUniform("m4_Projection", currCam->GetPerspective());
		ShaderLinker::SetUniform("m4_View", view);
		Skybox::Draw(TextureResource::m_TexturePool["SKYBOX4"]);
		ShaderLinker::UnUse();
		glDepthFunc(GL_LESS);

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
	}

	void Renderer::Draw(const float(&_View)[16], const float(&_Ortho)[16], const float(&_Perspective)[16])
	{
		(void)_Ortho;

		// Scene
		ShaderLinker::Use("BASIC");
		ShaderLinker::SetUniform("m4_Projection", _Perspective);
		ShaderLinker::SetUniform("m4_View", _View);
		MeshResource::DrawInstanced(MeshResource::MESH_TYPE::QUAD);
		ShaderLinker::UnUse();
	}

	void Renderer::DrawScreen()
	{
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
}