/******************************************************************************/
/*!
\file	AudioEngine.cpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details

	This file contains the implementations of the audio engine handling FMOD

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#include "AudioEngine.h"

PPA::AudioEngine* PPA::AudioEngine::m_instance = nullptr;

namespace PPA
{
	AudioEngine* AudioEngine::Instance()
	{
		if (!m_instance)
			m_instance = new AudioEngine();

		return m_instance;
	}

	void AudioEngine::Update()
	{
		if (m_instance)
			m_instance->xFmod.Update();
	}

	void AudioEngine::Destroy()
	{
		if (m_instance)
		{
			delete m_instance;
			m_instance = nullptr;
		}
	}

	/***************************************************************************/
	/*!
	\brief
		Loads a sound into FMOD to be played
	\param fileName
		The name of the file
	\param is3D
		If the sound should be in 3D space
	\param isStreamed
		If the file should be streamed
	*/
	/***************************************************************************/
	void AudioEngine::LoadAudio(const std::string& fileName, bool is3D, bool isLooping, bool isStreamed)
	{
		auto instance = AudioEngine::Instance();

		auto soundItr = instance->xFmod.m_soundMap.find(fileName);
		if (soundItr != instance->xFmod.m_soundMap.end())
			return;

		FMOD_MODE mode = FMOD_DEFAULT;
		mode |= FMOD_NONBLOCKING;
		mode |= is3D ? FMOD_3D : FMOD_2D;
		mode |= isLooping ? FMOD_LOOP_NORMAL : FMOD_LOOP_OFF;
		mode |= isStreamed ? FMOD_CREATESTREAM : FMOD_CREATECOMPRESSEDSAMPLE;

		FMOD::Sound* sound = nullptr;
		instance->xFmod.m_system->createSound(fileName.c_str(), mode, nullptr, &sound);

		if (sound)
			instance->xFmod.m_soundMap[fileName] = sound;
	}

	AudioEngine::xFMOD::xFMOD()
		:
		m_nextChannelID{ 0 },
		m_studioSystem{ nullptr },
		m_system{ nullptr }
	{
		FMOD::Studio::System::create(&m_studioSystem);
		m_studioSystem->initialize(32, FMOD_STUDIO_INIT_LIVEUPDATE, FMOD_INIT_PROFILE_ENABLE, nullptr);
		m_studioSystem->getCoreSystem(&m_system);
	}

	AudioEngine::xFMOD::~xFMOD()
	{
		m_studioSystem->unloadAll();
		m_studioSystem->release();
		m_system->release();
	}

	void AudioEngine::xFMOD::Update()
	{
		static std::vector<std::map<int, FMOD::Channel*>::iterator> c_stoppedChannels;

		c_stoppedChannels.clear();

		for (auto it = m_channelMap.begin(); it != m_channelMap.end(); ++it)
		{
			bool isPlaying = false;
			it->second->isPlaying(&isPlaying);

			if (!isPlaying)
				c_stoppedChannels.push_back(it);
		}

		for (auto& it : c_stoppedChannels)
		{
			m_channelMap.erase(it);
		}

		m_studioSystem->update();
	}
}
