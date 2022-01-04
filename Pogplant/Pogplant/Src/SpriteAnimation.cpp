#include "SpriteAnimation.h"

namespace Pogplant
{
	SpriteAnimation::SpriteAnimation()
		: m_Tiling{ 1,1 }
		, m_UV_Offset{ 0,0 }
		, m_FrameCounter{ 0 }
		, m_PlaySpeed{ 1 }
		, m_MaxFrames{ 0 }
		, m_Rows{ 0 }
		, m_Columns{ 0 }
		, m_Repeat{ false }
		, m_Playing{ false }
		, m_Paused{ false }
	{
	}

	SpriteAnimation::SpriteAnimation(int _MaxFrames, int _Rows, int _Columns, bool _Repeat, bool _Playing, float _PlaySpeed)
		: m_Tiling{ 1.0f / _Columns, 1.0f / _Rows }
		, m_UV_Offset{ 0,0 }
		, m_FrameCounter{ 0 }
		, m_PlaySpeed{ _PlaySpeed }
		, m_MaxFrames{ _MaxFrames }
		, m_Rows{ _Rows }
		, m_Columns{ _Columns }
		, m_Repeat{ _Repeat }
		, m_Playing{ _Playing }
		, m_Paused{ false }
	{
	}

	void SpriteAnimation::Update(float _Dt)
	{
		// If paused or stop just do not update
		if (!m_Playing || m_Paused)
		{
			return;
		}

		// Current frame
		m_FrameCounter += _Dt * m_PlaySpeed;
		// Cap at maximum frame/restart, -1 since start from 0
		if (m_FrameCounter = m_FrameCounter >= m_MaxFrames)
		{
			if (m_Repeat)
			{
				m_FrameCounter = 0.0f;
			}
			else
			{
				m_FrameCounter = m_MaxFrames - 1;
			}
		}

		// Calculate the UV offset
		const int currFrame = static_cast<int>(m_FrameCounter);
		// Rows = y, Columns = x
		// Assuming we have 4 rows and 4 columns, tiling is {0.25,0.25}  
		// Frame 6 will result in an offset of {0.5,0.25}
		int yOffset = currFrame / m_Columns;
		int xOffset = currFrame % m_Columns;
		m_UV_Offset = { xOffset * m_Tiling.x, yOffset * m_Tiling.y };
	}

	void SpriteAnimation::Reset()
	{
		m_FrameCounter = 0.0f;
		m_Playing = false;
		m_Paused = false;
	}

	void SpriteAnimation::Play()
	{
		m_Paused = false;
		m_Playing = true;
	}

	void SpriteAnimation::Pause()
	{
		m_Playing = false;
		m_Paused = true;
	}
}
