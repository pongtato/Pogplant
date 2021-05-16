#pragma once
#include <unordered_map>

namespace Pogplant
{
	enum class BufferType
	{
		// Editor
		EDITOR_BUFFER,
		EDITOR_COLOR_BUFFER,
		EDITOR_DEPTH_STENCIL,

		// Game
		GAME_BUFFER,
		GAME_COLOR_BUFFER,
		GAME_DEPTH_STENCIL,

		// Post process
		PP_BUFFER,
		PP_COLOR_BUFFER_NORMAL,
		PP_COLOR_BUFFER_BRIGHT,

		// Blur pass
		BLUR_BUFFER_0,
		BLUR_COLOR_BUFFER_0,
		BLUR_BUFFER_1,
		BLUR_COLOR_BUFFER_1
	};

	struct FrameBufferResource
	{
		static std::unordered_map<BufferType, unsigned int> m_FrameBuffers;
	};

	typedef FrameBufferResource FBR;
}