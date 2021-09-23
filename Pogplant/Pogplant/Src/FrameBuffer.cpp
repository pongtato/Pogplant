#include "FrameBuffer.h"
#include "FrameBufferResource.h"
#include "Window.h"
#include "ShaderLinker.h"
#include "Logger.h"

#include <glew.h>

namespace Pogplant
{
	void FrameBuffer::InitFrameBuffer()
	{
		InitEditorBuffer();
		InitGameBuffer();
		//InitGizmoBuffer();
		InitPostProcessBuffer();
		InitGBuffer();
	}

	void FrameBuffer::ResizeFrameBuffer()
	{
		// Editor
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::EDITOR_COLOR_BUFFER]);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);

		glBindRenderbuffer(GL_RENDERBUFFER, FBR::m_FrameBuffers[BufferType::EDITOR_BUFFER]);
		glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, Window::m_Width, Window::m_Height);

		// Game
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::GAME_COLOR_BUFFER]);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);

		glBindRenderbuffer(GL_RENDERBUFFER, FBR::m_FrameBuffers[BufferType::GAME_BUFFER]);
		glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, Window::m_Width, Window::m_Height);

		// Gizmo
		//glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::GIZMO_COLOR_BUFFER]);
		//glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);
		//glBindRenderbuffer(GL_RENDERBUFFER, FBR::m_FrameBuffers[BufferType::GIZMO_BUFFER]);
		//glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, Window::m_Width, Window::m_Height);
		
		// Gpass
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_POS_BUFFER]);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA16F, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_NORMAL_BUFFER]);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA16F, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_COLOR_BUFFER]);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);
		glBindTexture(GL_TEXTURE_2D, FBR::m_FrameBuffers[BufferType::G_NOLIGHT_BUFFER]);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);
		glBindRenderbuffer(GL_RENDERBUFFER, FBR::m_FrameBuffers[BufferType::G_DEPTH]);
		glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT, Window::m_Width, Window::m_Height);

	}

	void FrameBuffer::BindFrameBuffer(BufferType _BufferType)
	{
		glBindFramebuffer(GL_FRAMEBUFFER, FBR::m_FrameBuffers[_BufferType]);
	}

	void FrameBuffer::UnbindFrameBuffer()
	{
		glBindFramebuffer(GL_FRAMEBUFFER, 0);
	}

	void FrameBuffer::CleanUpFrameBuffer()
	{
		glDeleteFramebuffers(1, &FBR::m_FrameBuffers[BufferType::EDITOR_BUFFER]);
		glDeleteTextures(1, &FBR::m_FrameBuffers[BufferType::EDITOR_COLOR_BUFFER]);
		glDeleteRenderbuffers(1, &FBR::m_FrameBuffers[BufferType::EDITOR_DEPTH_STENCIL]);

		glDeleteFramebuffers(1, &FBR::m_FrameBuffers[BufferType::GAME_BUFFER]);
		glDeleteTextures(1, &FBR::m_FrameBuffers[BufferType::GAME_COLOR_BUFFER]);
		glDeleteRenderbuffers(1, &FBR::m_FrameBuffers[BufferType::GAME_DEPTH_STENCIL]);

		//glDeleteFramebuffers(1, &FBR::m_FrameBuffers[BufferType::GIZMO_BUFFER]);
		//glDeleteTextures(1, &FBR::m_FrameBuffers[BufferType::GIZMO_COLOR_BUFFER]);
		//glDeleteRenderbuffers(1, &FBR::m_FrameBuffers[BufferType::GIZMO_DEPTH_STENCIL]);

		glDeleteFramebuffers(1, &FBR::m_FrameBuffers[BufferType::G_BUFFER]);
		glDeleteTextures(1, &FBR::m_FrameBuffers[BufferType::G_POS_BUFFER]);
		glDeleteTextures(1, &FBR::m_FrameBuffers[BufferType::G_NORMAL_BUFFER]);
		glDeleteTextures(1, &FBR::m_FrameBuffers[BufferType::G_COLOR_BUFFER]);
		glDeleteTextures(1, &FBR::m_FrameBuffers[BufferType::G_NOLIGHT_BUFFER]);
		glDeleteRenderbuffers(1, &FBR::m_FrameBuffers[BufferType::G_DEPTH]);

		glDeleteFramebuffers(1, &FBR::m_FrameBuffers[BufferType::PP_BUFFER]);
		glDeleteTextures(1, &FBR::m_FrameBuffers[BufferType::PP_COLOR_BUFFER_NORMAL]);
		glDeleteTextures(1, &FBR::m_FrameBuffers[BufferType::PP_COLOR_BUFFER_BRIGHT]);
	}

	void FrameBuffer::InitEditorBuffer()
	{
		// Set this texture to be at location 0
		ShaderLinker::Use("SCREEN");
		ShaderLinker::SetUniform("s2D_ScreenTexture", 0);
		ShaderLinker::UnUse();

		// FB gen
		unsigned int* frameBuffer = &FBR::m_FrameBuffers[BufferType::EDITOR_BUFFER];
		glGenFramebuffers(1, frameBuffer);
		glBindFramebuffer(GL_FRAMEBUFFER, *frameBuffer);

		// Color texture gen
		unsigned int* colorBuffer = &FBR::m_FrameBuffers[BufferType::EDITOR_COLOR_BUFFER];
		glGenTextures(1, colorBuffer);
		glBindTexture(GL_TEXTURE_2D, *colorBuffer);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
		glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, *colorBuffer, 0);

		// Depth & Stencil
		unsigned int* depthStencil = &FBR::m_FrameBuffers[BufferType::EDITOR_DEPTH_STENCIL];
		glGenRenderbuffers(1, depthStencil);
		glBindRenderbuffer(GL_RENDERBUFFER, *depthStencil);
		glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, Window::m_Width, Window::m_Height);
		glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, *depthStencil);

		// Assert
		if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
		{
			Logger::Log({ "PP::FRAMEBUFFER",LogEntry::ERROR,"Editor Framebuffer init failed" });
		}
		else
		{
			Logger::Log({ "PP::FRAMEBUFFER",LogEntry::SUCCESS,"Editor Framebuffer init complete" });
		}
		glBindFramebuffer(GL_FRAMEBUFFER, 0);
	}

	void FrameBuffer::InitGameBuffer()
	{
		// Set this texture to be at location 0
		ShaderLinker::Use("SCREEN");
		ShaderLinker::SetUniform("s2D_ScreenTexture", 0);
		ShaderLinker::UnUse();

		// FB gen
		unsigned int* frameBuffer = &FBR::m_FrameBuffers[BufferType::GAME_BUFFER];
		glGenFramebuffers(1, frameBuffer);
		glBindFramebuffer(GL_FRAMEBUFFER, *frameBuffer);

		// Color texture gen
		unsigned int* colorBuffer = &FBR::m_FrameBuffers[BufferType::GAME_COLOR_BUFFER];
		glGenTextures(1, colorBuffer);
		glBindTexture(GL_TEXTURE_2D, *colorBuffer);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
		glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, *colorBuffer, 0);

		//// Depth & Stencil
		unsigned int* depthStencil = &FBR::m_FrameBuffers[BufferType::GAME_DEPTH_STENCIL];
		glGenRenderbuffers(1, depthStencil);
		glBindRenderbuffer(GL_RENDERBUFFER, *depthStencil);
		glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, Window::m_Width, Window::m_Height);
		glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, *depthStencil);

		// Assert
		if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
		{
			Logger::Log({ "PP::FRAMEBUFFER",LogEntry::ERROR,"Game Framebuffer init failed" });
		}
		else
		{
			Logger::Log({ "PP::FRAMEBUFFER",LogEntry::SUCCESS,"Game Framebuffer init complete" });
		}

		glBindFramebuffer(GL_FRAMEBUFFER, 0);
	}

	void FrameBuffer::InitGizmoBuffer()
	{
		// Set this texture to be at location 0
		ShaderLinker::Use("SCREEN");
		ShaderLinker::SetUniform("s2D_ScreenTexture", 0);
		ShaderLinker::UnUse();

		// FB gen
		unsigned int* frameBuffer = &FBR::m_FrameBuffers[BufferType::GIZMO_BUFFER];
		glGenFramebuffers(1, frameBuffer);
		glBindFramebuffer(GL_FRAMEBUFFER, *frameBuffer);

		// Color texture gen
		unsigned int* colorBuffer = &FBR::m_FrameBuffers[BufferType::GIZMO_COLOR_BUFFER];
		glGenTextures(1, colorBuffer);
		glBindTexture(GL_TEXTURE_2D, *colorBuffer);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
		glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, *colorBuffer, 0);

		//// Depth & Stencil
		unsigned int* depthStencil = &FBR::m_FrameBuffers[BufferType::GIZMO_DEPTH_STENCIL];
		glGenRenderbuffers(1, depthStencil);
		glBindRenderbuffer(GL_RENDERBUFFER, *depthStencil);
		glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, Window::m_Width, Window::m_Height);
		glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, *depthStencil);

		// Assert
		if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
		{
			Logger::Log({ "PP::FRAMEBUFFER",LogEntry::ERROR,"Gizmo Framebuffer init failed" });
		}
		else
		{
			Logger::Log({ "PP::FRAMEBUFFER",LogEntry::SUCCESS,"Gizmo Framebuffer init complete" });
		}

		glBindFramebuffer(GL_FRAMEBUFFER, 0);
	}

	void FrameBuffer::InitPostProcessBuffer()
	{
		// FB gen
		unsigned int* frameBuffer = &FBR::m_FrameBuffers[BufferType::PP_BUFFER];
		glGenFramebuffers(1, frameBuffer);
		glBindFramebuffer(GL_FRAMEBUFFER, *frameBuffer);

		// Color texture gen
		unsigned int* colorBuffer = &FBR::m_FrameBuffers[BufferType::PP_COLOR_BUFFER_NORMAL];
		glGenTextures(1, colorBuffer);
		glBindTexture(GL_TEXTURE_2D, *colorBuffer);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
		glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, *colorBuffer, 0);

		colorBuffer = &FBR::m_FrameBuffers[BufferType::PP_COLOR_BUFFER_BRIGHT];
		glGenTextures(1, colorBuffer);
		glBindTexture(GL_TEXTURE_2D, *colorBuffer);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
		glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT1, GL_TEXTURE_2D, *colorBuffer, 0);

		unsigned int attachments[2] = { GL_COLOR_ATTACHMENT0, GL_COLOR_ATTACHMENT1 };
		glDrawBuffers(2, attachments);

		// Assert
		if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
		{
			Logger::Log({ "PP::FRAMEBUFFER",LogEntry::ERROR,"Post Processing Framebuffer init failed" });
		}
		else
		{
			Logger::Log({ "PP::FRAMEBUFFER",LogEntry::SUCCESS,"Post Processing Framebuffer init complete" });
		}

		glBindFramebuffer(GL_FRAMEBUFFER, 0);
	}

	void FrameBuffer::InitGBuffer()
	{
		ShaderLinker::Use("GPASS");
		ShaderLinker::SetUniform("gPosition", 0);
		ShaderLinker::SetUniform("gNormal", 1);
		ShaderLinker::SetUniform("gAlbedoSpec", 2);
		ShaderLinker::SetUniform("gNoLight", 3);
		ShaderLinker::UnUse();

		unsigned int* frameBuffer = &FBR::m_FrameBuffers[BufferType::G_BUFFER];
		glGenFramebuffers(1, frameBuffer);
		glBindFramebuffer(GL_FRAMEBUFFER, *frameBuffer);

		// position color buffer
		unsigned int* gPosition = &FBR::m_FrameBuffers[BufferType::G_POS_BUFFER];
		glGenTextures(1, gPosition);
		glBindTexture(GL_TEXTURE_2D, *gPosition);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA16F, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
		glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, *gPosition, 0);

		// normal color buffer
		unsigned int* gNormal = &FBR::m_FrameBuffers[BufferType::G_NORMAL_BUFFER];
		glGenTextures(1, gNormal);
		glBindTexture(GL_TEXTURE_2D, *gNormal);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA16F, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_FLOAT, NULL);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
		glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT1, GL_TEXTURE_2D, *gNormal, 0);

		// color + specular color buffer
		unsigned int* gAlbedoSpec = &FBR::m_FrameBuffers[BufferType::G_COLOR_BUFFER];
		glGenTextures(1, gAlbedoSpec);
		glBindTexture(GL_TEXTURE_2D, *gAlbedoSpec);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, NULL);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
		glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT2, GL_TEXTURE_2D, *gAlbedoSpec, 0);

		unsigned int* gNoLight = &FBR::m_FrameBuffers[BufferType::G_NOLIGHT_BUFFER];
		glGenTextures(1, gNoLight);
		glBindTexture(GL_TEXTURE_2D, *gNoLight);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Window::m_Width, Window::m_Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, NULL);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
		glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT3, GL_TEXTURE_2D, *gNoLight, 0);

		// tell OpenGL which color attachments we'll use (of this framebuffer) for rendering 
		unsigned int attachments[4] = { GL_COLOR_ATTACHMENT0, GL_COLOR_ATTACHMENT1, GL_COLOR_ATTACHMENT2, GL_COLOR_ATTACHMENT3 };
		glDrawBuffers(4, attachments);

		// create and attach depth buffer (renderbuffer)
		unsigned int* rboDepth = &FBR::m_FrameBuffers[BufferType::G_DEPTH];
		glGenRenderbuffers(1, rboDepth);
		glBindRenderbuffer(GL_RENDERBUFFER, *rboDepth);
		glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT, Window::m_Width, Window::m_Height);
		glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, *rboDepth);

		// Assert
		if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
		{
			Logger::Log({ "PP::FRAMEBUFFER",LogEntry::ERROR,"G Framebuffer init failed" });
		}
		else
		{
			Logger::Log({ "PP::FRAMEBUFFER",LogEntry::SUCCESS,"G Framebuffer init complete" });
		}

		glBindFramebuffer(GL_FRAMEBUFFER, 0);
	}
}