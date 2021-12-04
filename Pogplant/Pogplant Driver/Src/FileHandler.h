/*****************************************************************************/
/*!
\file	FileHandler.h
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	Singleton FileHandler class used for file tracking and updates

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

#ifndef _FILE_HANDLER_H_
#define _FILE_HANDLER_H_

#include <filesystem>
#include <chrono>
#include <thread>
#include <unordered_map>
#include <string>
#include <mutex>
#include <memory>
#include <Pogplant.h>
#include <stack>

namespace PPF
{
	enum class FileStatus
	{
		CREATED,
		MODIFIED,
		ERASED,
		TOTAL
	};

	namespace FileStuff
	{
		struct ModelUpdate
		{
			std::string m_key;
			std::string m_filepath;
		};

		struct ModelNew
		{
			std::string m_key;
			std::string m_filepath;
		};
	}

	class FileHandler
	{
	private:
		using ftt = std::filesystem::file_time_type;

		static std::unique_ptr<FileHandler> m_instance;
		static std::once_flag m_onceFlag;
		FileHandler() = default;
		FileHandler(const FileHandler&) = delete;
		FileHandler& operator=(const FileHandler& rhs) = delete;
		// Key is Directory, Value->first is name of file, ftt is the time of creation
		std::unordered_map<std::string, std::unordered_map<std::string, ftt>> m_path;
		// Time interval at which we check the base folder for changes
		const std::chrono::duration<int, std::milli> m_delay{ 3000 };
		// The bool to terminate the running thread of Start()
		std::atomic<bool> m_running = true;
		void Start();
		static std::filesystem::path m_defaultpath;
		static std::thread m_thread;
		const std::string kekFileFolder{"Resources/KekFiles/"};

		// Stacks to update particular thing, add own type here
		std::stack<FileStuff::ModelUpdate> m_modelUpdate;
		std::stack<FileStuff::ModelNew> m_modelNew;

	public:
		virtual ~FileHandler() = default;
		static FileHandler& GetInstance();
		// Add path to folder to watch for
		void AddNewWatchPath(const std::string& path);
		// Update the model pool whenever needed
		void UpdateModels();
		// Call for cleanup of thread
		void Stop();

		// Flag for when hot reloaded
		static bool m_Modified;
		// Flag for when the ecs has cleared references
		static bool m_ShouldUpdate;
		// Current id
		static std::vector<std::string> m_UpdatedName;
	};
}

#endif // _FILE_HANDLER_H_