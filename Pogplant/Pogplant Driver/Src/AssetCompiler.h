#ifndef _ASSET_COMPILER_H_
#define _ASSET_COMPILER_H_

#include <unordered_map>
#include <thread>
#include <string>
#include <iostream>
#include <sstream>

// Singleton asset compiler class
class AssetCompiler
{
private:
	static AssetCompiler* m_Instance;
	AssetCompiler();
	AssetCompiler(const AssetCompiler&) = delete;

public:

	static AssetCompiler* GetInstance();
	void RunExecutable(std::string appName, std::string param = "");
	~AssetCompiler();

};

#endif //_ASSET_COMPILER_H_
