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

		// Debug
		DEBUG_BUFFER,
		DEBUG_COLOR_BUFFER,
		DEBUG_DEPTH_STENCIL,

		// GPass
		G_BUFFER,
		G_POS_BUFFER,
		G_NORMAL_BUFFER,
		G_COLOR_BUFFER,
		G_NOLIGHT_BUFFER,
		G_EMISSIVE_BUFFER,
		//G_SHAFT_BUFFER,
		G_CANVAS_BUFFER,
		G_AO_BUFFER,
		G_DEPTH,

		// Shadow pass
		//SHADOW_BUFFER,	// old
		//SHADOW_DEPTH,	// old
		LIGHT_BUFFER,
		LIGHT_DEPTH_MAP,
		MATRICES_BUFFER,

		// Post process
		PP_BUFFER,
		PP_COLOR_BUFFER_NORMAL,
		PP_COLOR_BUFFER_BRIGHT,

		// Blur pass
		BLUR_BUFFER_0,
		BLUR_BUFFER_1,
		BLUR_COLOR_BUFFER_0,
		BLUR_COLOR_BUFFER_1,

		// SSAO
		SSAO_BUFFER,
		SSAO_BLUR_BUFFER,
		SSAO_COLOR_BUFFER,
		SSAO_BLUR_COLOR_BUFFER
	};

	struct FrameBufferResource
	{
		static std::unordered_map<BufferType, unsigned int> m_FrameBuffers;
	};

	typedef FrameBufferResource FBR;
}