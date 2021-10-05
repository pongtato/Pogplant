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

#include <vector>
#include <string>
#include <map>

namespace PPA
{
	class AudioEngine
	{
	public:
		
		static AudioEngine* Instance();
		static void Update();
		static void Destroy();
		
		static void LoadAudio(const std::string& fileName, bool is3D, bool isLooping, bool isStreamed);
	private:
		AudioEngine() = default;
		~AudioEngine() = default;

		struct xFMOD
		{
			xFMOD();
			~xFMOD();

			void Update();

			int m_nextChannelID;

			std::map<int, FMOD::Channel*> m_channelMap;
			std::map<std::string, FMOD::Sound*> m_soundMap;

			FMOD::Studio::System* m_studioSystem;
			FMOD::System* m_system;
		};

		xFMOD xFmod;

		static AudioEngine* m_instance;
	};
}

#endif