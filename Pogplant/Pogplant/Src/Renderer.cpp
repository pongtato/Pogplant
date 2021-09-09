#include "Renderer.h"
#include "Window.h"
#include "FrameBufferResource.h"
#include "FrameBuffer.h"
#include "ShaderLinker.h"
#include "CameraResource.h"
#include "MeshResource.h"
#include "ModelResource.h"
#include "Model.h"

#include <glew.h>
#include <glfw3.h>
#include <string>

namespace Pogplant
{
	void Renderer::StartEditorBuffer()
	{
		FrameBuffer::BindFrameBuffer(BufferType::EDITOR_BUFFER);
		glEnable(GL_DEPTH_TEST);
	}

	void Renderer::StartGameBuffer()
	{
		FrameBuffer::BindFrameBuffer(BufferType::GAME_BUFFER);
		glEnable(GL_DEPTH_TEST);
	}

	void Renderer::StartGizmoBuffer()
	{
		FrameBuffer::BindFrameBuffer(BufferType::GIZMO_BUFFER);
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
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	}

	void Renderer::SwapBuffer()
	{
		glfwSwapBuffers(Window::GetWindow());
		glfwPollEvents();
	}

	void Renderer::Draw(const char* _CameraID)
	{
		Camera* currCam = CameraResource::GetCamera(_CameraID);
		ShaderLinker::Use("BASIC");
		ShaderLinker::SetUniform("m4_Projection", currCam->GetPerspective());
		ShaderLinker::SetUniform("m4_View", currCam->GetView());
		MeshResource::DrawInstanced(MeshResource::MESH_TYPE::QUAD);
		ShaderLinker::UnUse();

		ShaderLinker::Use("MODEL");
		ShaderLinker::SetUniform("m4_Model", glm::mat4{ 1 });
		ShaderLinker::SetUniform("m4_Projection", currCam->GetPerspective());
		ShaderLinker::SetUniform("m4_View", currCam->GetView());
		ModelResource::m_ModelPool["BAG"]->Draw();
		ShaderLinker::UnUse();
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