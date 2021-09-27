#include "AssetCompiler.h"

std::unique_ptr<AssetCompiler> AssetCompiler::m_instance = nullptr;
std::once_flag AssetCompiler::m_onceFlag;

AssetCompiler& AssetCompiler::GetInstance()
{
	std::call_once(m_onceFlag, [] {m_instance.reset(new AssetCompiler);});

	return *m_instance.get();
}

void AssetCompiler::RunExecutable(std::string appName, std::string param)
{
	//std::cout << "[PP::ASSETCOMPILER] " << appName << ", Input: " << param << std::endl;

	STARTUPINFOA si;
	PROCESS_INFORMATION pi;

	ZeroMemory(&si, sizeof(si));
	si.cb = sizeof(si);
	ZeroMemory(&pi, sizeof(pi));

	bool flag = CreateProcessA
	(
		appName.c_str(),					// Application Name
		const_cast<char*>(param.c_str()),	// Command Line Parameters
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
	m_processHandle[GetFileName(param)] = pi.hProcess;
	CloseHandle(pi.hThread);
	//CloseHandle(pi.hProcess);
}

void AssetCompiler::WaitForSingleProcess(std::string key)
{
	// Continue until either timeout or process ends
	bool done = false;
	while (!done)
	{
		DWORD exit = WaitForSingleObject(m_processHandle[key], 0);
		if (exit == WAIT_OBJECT_0)
		{
			done = true;
		}
	}
}

void AssetCompiler::WaitForAllProcess()
{
	// Continue until either timeout or process ends
	while (!m_processHandle.empty())
	{
		Update();
	}
}

void AssetCompiler::Update()
{
	auto it = m_processHandle.begin();
	while (it != m_processHandle.end()) 
	{
		DWORD exit_code;
		GetExitCodeProcess(it->second, &exit_code);
		if (exit_code != STILL_ACTIVE)
		{
			// Delete the processHandle from the map
			it = m_processHandle.erase(it);
		}
		else
		{
			it++;
		}
	}
}

bool AssetCompiler::CheckProcessDone(std::string key)
{
	Update();
	if (m_processHandle.contains(key))
	{
		return true;
	}
	return false;
}

bool AssetCompiler::Exists(std::string filePath)
{
	if (std::filesystem::exists(filePath))
	{
		return true;
	}
	return false;
}

std::string AssetCompiler::GetFileName(const std::string& fullpath)
{
	std::string filename;
	size_t found = fullpath.find_last_of('/');
	filename = fullpath.substr(found + 1, fullpath.find_last_of('.') - found - 1);
	return filename;
}
