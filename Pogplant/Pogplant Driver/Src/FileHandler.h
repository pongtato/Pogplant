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

enum class FileStatus
{
	CREATED,
	MODIFIED,
	ERASED,
	TOTAL
};

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
	std::atomic<bool> m_running = true;
	std::unordered_map<std::string, std::thread> m_threads;
	void Start();

public:
	virtual ~FileHandler() = default;
	static FileHandler& GetInstance();
	void AddNewWatchPath(std::string path);
	void Stop();
};

#endif // _FILE_HANDLER_H_