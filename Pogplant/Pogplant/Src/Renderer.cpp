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

	void Renderer::EndBuffer()
	{
		FrameBuffer::UnbindFrameBuffer();
		glDisable(GL_DEPTH_TEST);
	}

	void Renderer::PostProcess()
	{
		FrameBuffer::BindFrameBuffer(BufferType::PP_BUFFER);
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
		Camera* currCam = CameraResource::GetCamera(_CameraID);

		ShaderLinker::Use("BASIC");
		ShaderLinker::SetUniform("m4_Projection", currCam->GetPerspective());
		ShaderLinker::SetUniform("m4_View", currCam->GetView());
		MeshResource::DrawInstanced(MeshResource::MESH_TYPE::QUAD);
		ShaderLinker::UnUse();

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

		ShaderLinker::Use("MODEL");
		ShaderLinker::SetUniform("m4_Projection", currCam->GetPerspective());
		ShaderLinker::SetUniform("m4_View", currCam->GetView());

		auto results = registry.view<Components::RenderObject>();

		for (const auto& e : results)
		{
			// Draw selected objects seperately for edge detection
			const auto& it = results.get<const Components::RenderObject>(e);

			if(_Selected == nullptr || it.m_RenderModel != _Selected->m_RenderModel)
			{
				ShaderLinker::SetUniform("m4_Model", it.m_Model);
				it.m_RenderModel->Draw();
			}
		}
		ShaderLinker::UnUse();

		// Edge
		if (_Selected != nullptr)
		{
			// Test against the selected object
			glStencilFunc(GL_ALWAYS, 1, 0xFF);
			glStencilMask(0xFF);

			// Drawn seperately 
			ShaderLinker::Use("MODEL");
			ShaderLinker::SetUniform("m4_Projection", currCam->GetPerspective());
			ShaderLinker::SetUniform("m4_View", currCam->GetView());
			ShaderLinker::SetUniform("m4_Model", _Selected->m_Model);
			_Selected->m_RenderModel->Draw();
			ShaderLinker::UnUse();

			// Get the edge
			glStencilFunc(GL_NOTEQUAL, 1, 0xFF);
			glStencilMask(0x00);
			glDisable(GL_DEPTH_TEST);

			ShaderLinker::Use("EDGE");
			ShaderLinker::SetUniform("f_Thicc", 0.05f);
			ShaderLinker::SetUniform("m4_Projection", currCam->GetPerspective());
			ShaderLinker::SetUniform("m4_View", currCam->GetView());
			ShaderLinker::SetUniform("m4_Model", _Selected->m_Model);
			_Selected->m_RenderModel->Draw();
			ShaderLinker::UnUse();

			glStencilMask(0xFF);
			glStencilFunc(GL_ALWAYS, 0, 0xFF);
			glEnable(GL_DEPTH_TEST);
		}
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