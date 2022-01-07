/******************************************************************************/
/*!
\file	AudioEngine.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	
	This file contains the definitions of the audio engine handling FMOD

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef AUDIOENGINE_H_
#define AUDIOENGINE_H_

#include <fmod_studio.hpp>
#include <fmod.hpp>
#include <glm.hpp>

#include <mutex>
#include <vector>
#include <string>
#include <unordered_map>
#include <map>

#define PPA_NULL_STRING "NULL"

namespace PPA
{
	class AudioEngine
	{
	public:
		~AudioEngine() = default;

		static AudioEngine& Instance();
		static void Update();
		
		static void CreateChannelGroup(const std::string& channelGroupName);
		static void SetChannelGroupVolume(const std::string& channelGroupName, float volume);
		static float GetChannelGroupVolume(const std::string& channelGroupName);
		static void PauseChannelGroup(const std::string& channelGroupName);
		static void ResumeChannelGroup(const std::string& channelGroupName);
		static void StopChannelGroup(const std::string& channelGroupName);

		static bool LoadAudio(const std::string& fileName, bool is3D, bool isLooping, bool isStreamed);
		static void UnloadAudio(const std::string& fileName);


		static void UpdateAudio(const std::string& fileName, bool is3D, bool isLooping, bool isStreamed);
		static int PlaySound(const std::string& fileName, float volume, const glm::vec3& position, const std::string& channelGroupName = PPA_NULL_STRING);
		static void StopPlayingChannel(int channelID);
		static void StopPlayingAll();

		static bool UpdateChannel3DPosition(int channelID, const glm::vec3& position);
		static void UpdateChannelVolume(int channelID, float volume);

		static void UpdateListenerPosition(const glm::vec3& position, const glm::vec3& forwardVec, const glm::vec3& upVec, const glm::vec3& velocity);
	private:
		AudioEngine() = default;
		
		struct xFMOD
		{
			xFMOD();
			~xFMOD();

			struct ChannelGroupInfo
			{
				bool m_setPaused = false;
			};

			void Update();

			int m_nextChannelID;

			std::unordered_map<std::string, std::pair<FMOD::ChannelGroup*, ChannelGroupInfo>> m_channelGroupMap;
			std::map<int, std::pair<FMOD::Channel*, std::string>> m_channelMap;
			std::map<std::string, FMOD::Sound*> m_soundMap;

			FMOD::Studio::System* m_studioSystem;
			FMOD::System* m_system;
		};

		xFMOD xFmod;

		FMOD::ChannelGroup* GetChannelGroup(const std::string& channelGroupName);

		static FMOD_VECTOR GLMToFMODVec3(const glm::vec3& vec);

		static std::unique_ptr<AudioEngine> m_instance;
		static std::once_flag m_onceFlag;
	};

	class AudioResource
	{
	public:
		~AudioResource();

		struct AudioClip
		{
			inline AudioClip(
				bool is3D = true,
				bool isLooping = false,
				bool isStreamed = false,
				bool enableDoppler = false)
				:
				m_is3D{ is3D },
				m_isLooping{ isLooping },
				m_isStreamed{ isStreamed },
				m_enableDopplerEffect{ enableDoppler }
			{

			}

			~AudioClip() = default;

			bool m_is3D = true;
			bool m_isLooping = false;
			bool m_isStreamed = false;
			bool m_enableDopplerEffect = false;
		};

		static bool LoadAudio(const std::string& fileName);

		static AudioResource& Instance();
		static std::map<std::string, AudioClip>& AudioPool();

	private:
		AudioResource() = default;

		std::map<std::string, AudioClip> m_audioPool;

		static std::unique_ptr<AudioResource> m_instance;
		static std::once_flag m_onceFlag;
	};
}

#endif