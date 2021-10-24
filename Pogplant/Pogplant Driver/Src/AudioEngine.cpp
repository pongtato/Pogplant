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
#include <fstream>
#include <filesystem>

std::unique_ptr<PPA::AudioEngine> PPA::AudioEngine::m_instance = nullptr;
std::once_flag PPA::AudioEngine::m_onceFlag;

std::unique_ptr<PPA::AudioResource> PPA::AudioResource::m_instance = nullptr;
std::once_flag PPA::AudioResource::m_onceFlag;

namespace PPA
{
	/***************************************************************************/
	/*!
	\brief
		Gets the AudioEngine singleton instance
	*/
	/***************************************************************************/
	AudioEngine& AudioEngine::Instance()
	{
		std::call_once(m_onceFlag, [] {
			m_instance.reset(new AudioEngine);
			});

		return *m_instance.get();
	}

	/***************************************************************************/
	/*!
	\brief
		Updates the AudioEngine
	*/
	/***************************************************************************/
	void AudioEngine::Update()
	{
		AudioEngine::Instance().xFmod.Update();
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
	bool AudioEngine::LoadAudio(const std::string& fileName, bool is3D, bool isLooping, bool isStreamed)
	{
		auto c_instance = &AudioEngine::Instance();

		auto soundItr = c_instance->xFmod.m_soundMap.find(fileName);
		if (soundItr != c_instance->xFmod.m_soundMap.end())
			return true;

		FMOD_MODE mode = FMOD_DEFAULT;
		//mode |= FMOD_NONBLOCKING;
		mode |= is3D ? FMOD_3D : FMOD_2D;
		mode |= isLooping ? FMOD_LOOP_NORMAL : FMOD_LOOP_OFF;
		mode |= isStreamed ? FMOD_CREATESTREAM : FMOD_CREATECOMPRESSEDSAMPLE;

		FMOD::Sound* sound = nullptr;
		c_instance->xFmod.m_system->createSound(fileName.c_str(), mode, nullptr, &sound);

		if (sound)
			c_instance->xFmod.m_soundMap[fileName] = sound;
		else
		{
			std::stringstream ss;

			ss << "Unable to load audio file \"" << fileName << "\"";
			PP::Logger::Log(
				PP::LogEntry{ "AudioEngine::LoadAudio", PP::LogEntry::LOGTYPE::ERROR, ss.str() }, true);

			return false;
		}

		std::stringstream ss;
		ss << "Successfully loaded: \"" << fileName << "\"";
		PP::Logger::Log(
			PP::LogEntry{ "AudioEngine::LoadAudio", PP::LogEntry::LOGTYPE::SUCCESS, ss.str() }, true);

		return true;
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
		auto c_instance = &AudioEngine::Instance();

		auto soundItr = c_instance->xFmod.m_soundMap.find(fileName);
		if (soundItr == c_instance->xFmod.m_soundMap.end())
			return;

		soundItr->second->release();
		c_instance->xFmod.m_soundMap.erase(soundItr);

		/*std::stringstream ss;

		ss << "Successfully unloaded: \"" << fileName << "\"";
		PP::Logger::Log(
			PP::LogEntry{ "AudioEngine::UnloadAudio", PP::LogEntry::TYPE::SUCCESS, ss.str() }, true);*/
	}

	void AudioEngine::UpdateAudio(const std::string& fileName, bool is3D, bool isLooping, bool isStreamed)
	{
		auto c_instance = &AudioEngine::Instance();

		auto soundItr = c_instance->xFmod.m_soundMap.find(fileName);
		if (soundItr == c_instance->xFmod.m_soundMap.end())
		{
			PP::Logger::Log(
				PP::LogEntry{
					"AudioEngine::UpdateAudio",
					PP::LogEntry::LOGTYPE::ERROR,
					"Unable to update sound, might not have loaded properly" }, true);//*/
			return;
		}

		FMOD_MODE mode = FMOD_DEFAULT;
		//mode |= FMOD_NONBLOCKING;
		mode |= is3D ? FMOD_3D : FMOD_2D;
		mode |= isLooping ? FMOD_LOOP_NORMAL : FMOD_LOOP_OFF;
		mode |= isStreamed ? FMOD_CREATESTREAM : FMOD_CREATECOMPRESSEDSAMPLE;
		soundItr->second->setMode(mode);
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
		auto c_instance = &AudioEngine::Instance();

		auto soundItr = c_instance->xFmod.m_soundMap.find(fileName);
		if (soundItr == c_instance->xFmod.m_soundMap.end())
		{
			std::stringstream ss;
			ss << "Unable to play sound \"" << fileName << "\"";
			PP::Logger::Log(
				PP::LogEntry{
					"AudioEngine::PlaySound",
					PP::LogEntry::LOGTYPE::ERROR,
					ss.str() }, true);//*/
			return -1;
		}

		int channelID{ c_instance->xFmod.m_nextChannelID++ };
		FMOD::Channel* channel{ nullptr };

		c_instance->xFmod.m_system->playSound(soundItr->second, nullptr, true, &channel);

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

			c_instance->xFmod.m_channelMap[channelID] = channel;
		}

		return channelID;
	}

	/***************************************************************************/
	/*!
	\brief
		Stops a sound from playing
	*/
	/***************************************************************************/
	void AudioEngine::StopPlayingChannel(int channelID)
	{
		auto c_instance = &AudioEngine::Instance();

		auto channelItr = c_instance->xFmod.m_channelMap.find(channelID);

		if (channelItr != c_instance->xFmod.m_channelMap.end())
		{
			channelItr->second->stop();
		}
	}

	void AudioEngine::StopPlayingAll()
	{
		auto c_instance = &AudioEngine::Instance();

		for (auto& channel : c_instance->xFmod.m_channelMap)
		{
			channel.second->stop();
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
		auto c_instance = &AudioEngine::Instance();

		auto channelItr = c_instance->xFmod.m_channelMap.find(channelID);

		if (channelItr != c_instance->xFmod.m_channelMap.end())
		{
			FMOD_VECTOR fvPosition = GLMToFMODVec3(position);
			channelItr->second->set3DAttributes(&fvPosition, nullptr);

			return true;
		}

		return false;
	}

	void AudioEngine::UpdateChannelVolume(int channelID, float volume)
	{
		auto c_instance = &AudioEngine::Instance();

		auto channelItr = c_instance->xFmod.m_channelMap.find(channelID);

		if (channelItr != c_instance->xFmod.m_channelMap.end())
		{
			channelItr->second->setVolume(volume);
		}
	}

	void AudioEngine::UpdateListenerPosition(const glm::vec3& position, const glm::vec3& forwardVec, const glm::vec3& upVec, const glm::vec3& velocity)
	{
		auto fvPosition = GLMToFMODVec3(position);
		auto fvForward = GLMToFMODVec3(forwardVec);
		auto fvUp = GLMToFMODVec3(upVec);
		auto fvVelo = GLMToFMODVec3(velocity);

		AudioEngine::Instance().xFmod.m_system->set3DListenerAttributes(0, &fvPosition, nullptr, &fvForward, &fvUp);
	}

	FMOD_VECTOR AudioEngine::GLMToFMODVec3(const glm::vec3& vec)
	{
		FMOD_VECTOR fv;
		fv.x = vec.x;
		fv.y = vec.y;
		fv.z = -vec.z;
		return fv;
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

		assert(result == FMOD_OK && m_system != nullptr);

		if (result != FMOD_OK || m_system == nullptr)
		{
			PP::Logger::Log(
				PP::LogEntry{
					"AudioEngine::xFMOD",
					PP::LogEntry::LOGTYPE::ERROR,
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

	AudioResource::~AudioResource()
	{
		for (auto audioClip : m_audioPool)
		{
			std::ofstream fileStream{ audioClip.first + ".mamamia",
					std::ios::out | std::ios::binary | std::ios::trunc };

			if (fileStream.is_open())
			{
				fileStream.write(reinterpret_cast<const char*>(&audioClip.second), sizeof(AudioClip));
			}
			else
			{
				assert(false);
			}
		}
	}

	bool AudioResource::LoadAudio(const std::string& fileName)
	{
		AudioClip newAudioClip;

		if (std::filesystem::exists(fileName + ".mamamia"))
		{
			std::ifstream fileStream{ fileName + ".mamamia", std::ios::binary };

			if (fileStream.is_open())
				fileStream.read(reinterpret_cast<char*>(&newAudioClip), sizeof(AudioClip));
		}

		auto ret = AudioEngine::LoadAudio(fileName, true, false, false);
		
		if (ret)
		{
			AudioResource::Instance().m_audioPool[fileName] = newAudioClip;
			AudioEngine::UpdateAudio(fileName, newAudioClip.m_is3D, newAudioClip.m_isLooping, newAudioClip.m_isStreamed);
		}

		return ret;
	}

	AudioResource& AudioResource::Instance()
	{
		std::call_once(m_onceFlag, [] {
			m_instance.reset(new AudioResource);
			});

		return *m_instance.get();
	}

	std::map<std::string, AudioResource::AudioClip>& AudioResource::AudioPool()
	{
		return Instance().m_audioPool;
	}
}
