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
		InitPostProcessBuffer();
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

		//// Depth & Stencil
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
}