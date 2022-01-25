/******************************************************************************/
/*!
\file	TimeStone.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details

	This file contains the definitions of the delta time manipulator

\copyright	Copyright (c) 2022 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef TIMESTONE_H_
#define TIMESTONE_H_

#include <memory>
#include <mutex>
#include <vector>

class TimeStone
{
public:
	~TimeStone() = default;

	inline static float GetCurrentTimeScale()
	{
		return Instance().m_currentTimeScale;
	}

	static float GetUpdatedDelta(float c_dt);
	static void TriggerTimeAlter(float timeMultiplier, float durationInRealTime, int audioAffectStat = 2, bool overrideAll = false);
	static void ForceNormalTime();

	static TimeStone& Instance();
private:
	TimeStone() = default;

	struct TimeControlInfo
	{
		float m_timer = 0.f;
		float m_multiplier = 1.f;
		int m_audioAffectStat = 1;
	};

	float m_currentTimeScale;

	std::vector<TimeControlInfo> m_timeControlStack;

	/**> singleton instance*/
	static std::unique_ptr<TimeStone> m_instance;
	static std::once_flag m_onceFlag;
};

#endif