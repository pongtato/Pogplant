#pragma once

#include <glm.hpp>

namespace Pogplant
{
	enum class BufferType;

	class FrameBuffer
	{
	public:
		static void InitFrameBuffer();
		static void ResizeFrameBuffer();
		static void ResizeFrameBuffer(BufferType _BufferType, const float _Scale);
		static void BindFrameBuffer(BufferType _BufferType);
		static void UnbindFrameBuffer();
		static void CleanUpFrameBuffer();

	private:
		static bool InitEditorBuffer();
		static bool InitGameBuffer();
		static bool InitDebugBuffer();
		static bool InitPostProcessBuffer();
		static bool InitGBuffer();
		static bool InitShadowBuffer();
		static bool InitBlurBuffer();
		static bool InitAOBuffer();
	};
}

