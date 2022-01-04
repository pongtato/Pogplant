#pragma once
#include <glm.hpp>

namespace Pogplant
{
	class SpriteAnimation
	{
	public:
		// Derived
		glm::vec2 m_Tiling;
		glm::vec2 m_UV_Offset;
		// Accumulated
		float m_FrameCounter;
		// Set
		float m_PlaySpeed;
		int m_MaxFrames;
		int m_Rows;
		int m_Columns;
		bool m_Repeat;
		
		SpriteAnimation();
		SpriteAnimation(int _MaxFrames, int _Rows, int _Columns, bool _Repeat, bool _Playing, float _PlaySpeed = 1.0f);
		void Update(float _Dt);
		void Reset();
		void Play();
		void Pause();

	private:
		bool m_Playing;
		bool m_Paused;
	};
}