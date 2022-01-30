/******************************************************************************/
/*!
\file	TimeStone.cpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details

	This file contains the implementations of the delta time manipulator

\copyright	Copyright (c) 2022 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#include "TimeStone.h"
#include "../AudioEngine.h"

std::unique_ptr<TimeStone> TimeStone::m_instance = nullptr;
std::once_flag TimeStone::m_onceFlag;

float TimeStone::GetUpdatedDelta(float c_dt)
{
	auto& c_instance = Instance();
	
	c_instance.m_currentTimeScale = 1.f;
	float newAudioPitchAll = 1.f;
	float newAudioPitchAllExceptBGM = 1.f;

	for (int i = 0; i < c_instance.m_timeControlStack.size(); i++)
	{
		c_instance.m_timeControlStack[i].m_timer -= c_dt;

		c_instance.m_currentTimeScale *= c_instance.m_timeControlStack[i].m_multiplier;

		switch (c_instance.m_timeControlStack[i].m_audioAffectStat)
		{
		case 1:
			newAudioPitchAllExceptBGM *= c_instance.m_timeControlStack[i].m_multiplier;
			break;
		case 2:
			newAudioPitchAll *= c_instance.m_timeControlStack[i].m_multiplier;
			break;
		default:
			break;
		}

		if (c_instance.m_timeControlStack[i].m_timer <= 0)
		{
			c_instance.m_timeControlStack.erase(c_instance.m_timeControlStack.begin() + i);
			i--;
		}
	}

	PPA::AudioEngine::SetChannelGroupPitch("BGM", newAudioPitchAll);
	PPA::AudioEngine::SetChannelGroupPitch("SFX", newAudioPitchAll * newAudioPitchAllExceptBGM);
	PPA::AudioEngine::SetChannelGroupPitch("VO", newAudioPitchAll * newAudioPitchAllExceptBGM);

	return c_instance.m_currentTimeScale * c_dt;
}

/******************************************************************************/
/*!
\brief
	Triggers a time alteration event
\param timeMultiplier
	The timeScale to set to, 0.5 for 0.5x slow motion
\param durationInRealTime
	How long the slow motion should last, in real time seconds

\param audioAffectStat
	The setting to affect audio effects

	if == 0, will not affect the audio
	if == 1, will affect all except BGM
	if == 2, will affect all

\param overrideAll
	Whether it should clear all the other slow motion effects that
	are still active
*/
/******************************************************************************/
void TimeStone::TriggerTimeAlter(float timeMultiplier, float durationInRealTime, int audioAffectStat, bool overrideAll)
{
	auto& c_instance = Instance();

	if (overrideAll)
		c_instance.m_timeControlStack.clear();

	c_instance.m_timeControlStack.push_back(TimeControlInfo{ durationInRealTime, timeMultiplier, audioAffectStat });
}


void TimeStone::ForceNormalTime()
{
	Instance().m_timeControlStack.clear();
}

TimeStone& TimeStone::Instance()
{
	std::call_once(m_onceFlag, [] {
		m_instance.reset(new TimeStone);
		});

	return *m_instance.get();
}
