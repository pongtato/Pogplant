#ifndef _ASSET_COMPILER_H_
#define _ASSET_COMPILER_H_

#include <unordered_map>
#include <thread>
#include <string>
#include <iostream>
#include <sstream>
#include <memory>
#include <mutex>

// Singleton asset compiler class
class AssetCompiler
{
private:

	static std::unique_ptr<AssetCompiler> m_instance;
	static std::once_flag m_onceFlag;
	AssetCompiler() = default;
	AssetCompiler(const AssetCompiler&) = delete;
	AssetCompiler& operator=(const AssetCompiler& rhs) = delete;
	int m_data;

public:

	virtual ~AssetCompiler() = default;
	static AssetCompiler& GetInstance();
	void RunExecutable(std::string appName, std::string param = "");

	// Test to check if it's actually singleton
	void SetData(int data);
	int GetData();
};

#endif //_ASSET_COMPILER_H_
