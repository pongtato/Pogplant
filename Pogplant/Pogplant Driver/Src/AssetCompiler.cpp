#include "AssetCompiler.h"
#include <windows.h>

std::unique_ptr<AssetCompiler> AssetCompiler::m_instance = nullptr;
std::once_flag AssetCompiler::m_onceFlag;

AssetCompiler& AssetCompiler::GetInstance()
{
	std::call_once(m_onceFlag, [] {
		m_instance.reset(new AssetCompiler);
		});

	return *m_instance.get();
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

// just for testing
void AssetCompiler::SetData(int data)
{
	m_data = data;
}

int AssetCompiler::GetData()
{
	return m_data;
}
