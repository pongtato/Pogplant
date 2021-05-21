#pragma once

namespace Pogplant
{
	enum class BufferType;

	class FrameBuffer
	{
	public:
		static void InitFrameBuffer();
		static void ResizeFrameBuffer();
		static void BindFrameBuffer(BufferType _BufferType);
		static void UnbindFrameBuffer();
		static void CleanUpFrameBuffer();

	private:
		static void InitEditorBuffer();
		static void InitGameBuffer();
		static void InitGizmoBuffer();
		static void InitPostProcessBuffer();
	};
}

