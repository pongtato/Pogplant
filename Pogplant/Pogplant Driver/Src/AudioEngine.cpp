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
#include "Pogplant.h"

PPA::AudioEngine* PPA::AudioEngine::m_instance = nullptr;

namespace PPA
{
	/***************************************************************************/
	/*!
	\brief
		Gets the AudioEngine singleton instance
	*/
	/***************************************************************************/
	AudioEngine* AudioEngine::Instance()
	{
		if (!m_instance)
			m_instance = new AudioEngine();

		return m_instance;
	}

	/***************************************************************************/
	/*!
	\brief
		Updates the AudioEngine
	*/
	/***************************************************************************/
	void AudioEngine::Update()
	{
		if (m_instance)
			m_instance->xFmod.Update();
	}

	/***************************************************************************/
	/*!
	\brief
		Destroys the AudioEngine singleton instance
	*/
	/***************************************************************************/
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
		//mode |= FMOD_NONBLOCKING;
		mode |= is3D ? FMOD_3D : FMOD_2D;
		mode |= isLooping ? FMOD_LOOP_NORMAL : FMOD_LOOP_OFF;
		mode |= isStreamed ? FMOD_CREATESTREAM : FMOD_CREATECOMPRESSEDSAMPLE;

		FMOD::Sound* sound = nullptr;
		instance->xFmod.m_system->createSound(fileName.c_str(), mode, nullptr, &sound);

		if (sound)
			instance->xFmod.m_soundMap[fileName] = sound;
		else
		{
			std::stringstream ss;

			ss << "Unable to load audio file \"" << fileName << "\"";
			PP::Logger::Log(
				PP::LogEntry{ "AudioEngine::LoadAudio", PP::LogEntry::TYPE::ERROR, ss.str() }, true);
		}
	}

	/***************************************************************************/
	/*!
	\brief
		Releases the resources used by an audio clip from FMOD
	\param fileName
		The name of the file
	*/
	/***************************************************************************/
	void AudioEngine::UnloadAudio(const std::string& fileName)
	{
		if (m_instance)
		{
			auto soundItr = m_instance->xFmod.m_soundMap.find(fileName);
			if (soundItr == m_instance->xFmod.m_soundMap.end())
				return;

			soundItr->second->release();
			m_instance->xFmod.m_soundMap.erase(soundItr);
		}
	}

	/***************************************************************************/
	/*!
	\brief
		Plays a sound from memory
	\param fileName
		The name of the file
	\param volume
		The volume of the file
	\param position
		The position of the sound to play
	\return
		Returns the channel ID the sound is played in
	*/
	/***************************************************************************/
	int AudioEngine::PlaySound(const std::string& fileName, float volume, const glm::vec3& position)
	{
		if (m_instance)
		{
			auto soundItr = m_instance->xFmod.m_soundMap.find(fileName);
			if (soundItr == m_instance->xFmod.m_soundMap.end())
			{
				PP::Logger::Log(
					PP::LogEntry{
						"AudioEngine::PlaySound",
						PP::LogEntry::TYPE::ERROR,
						"Unable to play sound, might not have loaded properly" }, true);//*/
				return -1;
			}

			int channelID{ m_instance->xFmod.m_nextChannelID++ };
			FMOD::Channel* channel{ nullptr };

			m_instance->xFmod.m_system->playSound(soundItr->second, nullptr, true, &channel);

			if (channel)
			{
				FMOD_MODE mode;
				soundItr->second->getMode(&mode);

				if (mode & FMOD_3D)
				{
					FMOD_VECTOR fvPosition = GLMToFMODVec3(position);

					channel->set3DAttributes(&fvPosition, nullptr);
				}

				channel->setVolume(volume);
				channel->setPaused(false);

				m_instance->xFmod.m_channelMap[channelID] = channel;
			}

			return channelID;
		}

		return -1;
	}

	/***************************************************************************/
	/*!
	\brief
		Stops a sound from playing
	*/
	/***************************************************************************/
	void AudioEngine::StopPlayingChannel(int channelID)
	{
		if (m_instance)
		{
			auto channelItr = m_instance->xFmod.m_channelMap.find(channelID);

			if (channelItr != m_instance->xFmod.m_channelMap.end())
			{
				channelItr->second->stop();
			}
		}
	}

	/***************************************************************************/
	/*!
	\brief
		Updates the 3D position of the audio in the channel
	\param channelID
		The channel's ID
	\param volume
		The volume of the file
	\param position
		The position of the sound to play
	*/
	/***************************************************************************/
	bool AudioEngine::UpdateChannel3DPosition(int channelID, const glm::vec3& position)
	{
		if (m_instance)
		{
			auto channelItr = m_instance->xFmod.m_channelMap.find(channelID);

			if (channelItr != m_instance->xFmod.m_channelMap.end())
			{
				FMOD_VECTOR fvPosition = GLMToFMODVec3(position);
				channelItr->second->set3DAttributes(&fvPosition, nullptr);

				return true;
			}
		}

		return false;
	}

	void AudioEngine::UpdateChannelVolume(int channelID, float volume)
	{
		if (m_instance)
		{
			auto channelItr = m_instance->xFmod.m_channelMap.find(channelID);

			if (channelItr != m_instance->xFmod.m_channelMap.end())
			{
				channelItr->second->setVolume(volume);
			}
		}
	}

	void AudioEngine::UpdateListenerPosition(const glm::vec3& position, const glm::vec3& forwardVec, const glm::vec3& upVec, const glm::vec3& velocity)
	{
		auto fvPosition = GLMToFMODVec3(position);
		auto fvForward = GLMToFMODVec3(forwardVec);
		auto fvUp = GLMToFMODVec3(upVec);
		auto fvVelo = GLMToFMODVec3(velocity);

		m_instance->xFmod.m_system->set3DListenerAttributes(0, &fvPosition, nullptr, &fvForward, &fvUp);
	}

	FMOD_VECTOR&& AudioEngine::GLMToFMODVec3(const glm::vec3& vec)
	{
		FMOD_VECTOR fv;
		fv.x = vec.x;
		fv.y = vec.y;
		fv.z = -vec.z;
		return std::move(fv);
	}

	/***************************************************************************/
	/*!
	\brief
		Initialises the FMOD system
	*/
	/***************************************************************************/
	AudioEngine::xFMOD::xFMOD()
		:
		m_nextChannelID{ 0 },
		m_studioSystem{ nullptr },
		m_system{ nullptr }
	{
		FMOD::Studio::System::create(&m_studioSystem);
		auto result = m_studioSystem->initialize(32, FMOD_STUDIO_INIT_LIVEUPDATE, FMOD_INIT_PROFILE_ENABLE, nullptr);
		m_studioSystem->getCoreSystem(&m_system);

		if (result != FMOD_OK || m_system == nullptr)
		{
			PP::Logger::Log(
				PP::LogEntry{
					"AudioEngine::xFMOD",
					PP::LogEntry::TYPE::ERROR,
					"Error initialising FMOD" });
		}
	}

	/***************************************************************************/
	/*!
	\brief
		Cleans up the FMOD system
	*/
	/***************************************************************************/
	AudioEngine::xFMOD::~xFMOD()
	{
		m_studioSystem->unloadAll();
		m_studioSystem->release();
		m_system->release();
	}

	/***************************************************************************/
	/*!
	\brief
		Updates the FMOD system, and remove completed channels from the map
	*/
	/***************************************************************************/
	void AudioEngine::xFMOD::Update()
	{
		static std::vector<std::map<int, FMOD::Channel*>::iterator> c_stoppedChannels;

		c_stoppedChannels.clear();

		for (auto it = m_channelMap.begin(); it != m_channelMap.end(); ++it)
		{
			bool isPlaying = false;
			it->second->isPlaying(&isPlaying);
			//it->second->getPaused();

			if (!isPlaying)
				c_stoppedChannels.push_back(it);
		}

		for (auto& it : c_stoppedChannels)
			m_channelMap.erase(it);

		m_studioSystem->update();
	}
}
