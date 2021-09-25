#include "AssetCompiler.h"
#include <windows.h>

AssetCompiler* AssetCompiler::m_Instance;

AssetCompiler::AssetCompiler()
{
}

AssetCompiler* AssetCompiler::GetInstance()
{
	if (!m_Instance)
	{
		m_Instance = new AssetCompiler();
	}
	return m_Instance;
}

void AssetCompiler::RunExecutable(std::string appName, std::string param)
{
	std::cout << "Starting Process: " << appName << std::endl;

	STARTUPINFOA si;
	PROCESS_INFORMATION pi;

	ZeroMemory(&si, sizeof(si));
	si.cb = sizeof(si);
	ZeroMemory(&pi, sizeof(pi));

	bool flag = CreateProcessA(
		appName.c_str(),					// Application Name
		const_cast<char *>(param.c_str()),	// Command Line Parameters
		NULL,
		NULL,
		FALSE,
		0,
		NULL,
		NULL,
		&si,
		&pi
	);

	if (!flag)
	{
		
		DWORD error = GetLastError();
		std::stringstream ss;
		ss << "CreateProcess Failed, Error: " << error;
		//PP::Logger::Log({"PP::SUCCESS::PROCESS", PP::LogEntry::TYPE::SUCCESS, ss.str()});
		std::cout << ss.str() << std::endl;
		return;
	}

	CloseHandle(pi.hThread);
	CloseHandle(pi.hProcess);
}

AssetCompiler::~AssetCompiler()
{
}
