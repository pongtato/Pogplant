#ifndef _ASSET_COMPILER_H_
#define _ASSET_COMPILER_H_

#include <unordered_map>
#include <thread>
#include <string>
#include <iostream>
#include <sstream>
#include <memory>
#include <mutex>
#include <windows.h>
#include <filesystem>

// Singleton asset compiler class
class AssetCompiler
{
private:

	static std::unique_ptr<AssetCompiler> m_instance;
	static std::once_flag m_onceFlag;
	AssetCompiler() = default;
	AssetCompiler(const AssetCompiler&) = delete;
	AssetCompiler& operator=(const AssetCompiler& rhs) = delete;
	// Key = String, Value = Process Handle
	std::unordered_map<std::string, HANDLE> m_processHandle;

public:

	virtual ~AssetCompiler() = default;
	static AssetCompiler& GetInstance();
	void RunExecutable(std::string appName, std::string param = "");
	void WaitForSingleProcess(std::string key);
	void WaitForAllProcess();
	void Update();
	bool CheckProcessDone(std::string key);
	bool Exists(std::string filePath);
	static std::string GetFileName(const std::string& fullpath);
};

#endif //_ASSET_COMPILER_H_
