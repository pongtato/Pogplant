#ifndef _FILE_HANDLER_H_
#define _FILE_HANDLER_H_

#include <filesystem>
#include <chrono>
#include <thread>
#include <unordered_map>
#include <string>
#include <functional>
#include <mutex>
#include <memory>
#include <iostream>
#include <Pogplant.h>
#include <stack>

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
	const std::chrono::duration<int, std::milli> m_delay{4000};
	// The bool to terminate the running thread of Start()
	std::atomic<bool> m_running = true;
	void Start();
	static std::filesystem::path m_defaultpath;
	static std::thread m_thread;

	// Stacks to update particular thing, add own type here
	std::stack<FileStuff::ModelUpdate> m_modelUpdate;
	std::stack<FileStuff::ModelNew> m_modelNew;

	// Helper functions
	std::string GetFileName(const std::string& fullpath);

public:
	virtual ~FileHandler() = default;
	static FileHandler& GetInstance();
	// Add path to folder to watch for, [IMPORTANT]: FOLDER CANNOT CONTAIN OTHER FOLDERS
	void AddNewWatchPath(const std::string& path);
	// Call for cleanup of thread
	void Stop();
	// Return the ref of the stack for usage
	std::stack<FileStuff::ModelUpdate>& GetModelUpdate();
	std::stack<FileStuff::ModelNew>& GetModelNew();
};

#endif // _FILE_HANDLER_H_