#ifndef _FILE_SYSTEM_
#define _FILE_SYSTEM_

#include <iostream>
#include <string>
#include <sstream>
#include <fstream>
#include <filesystem>
#include <unordered_map>

#include "ModelCompiler.h"
#include "Logger.h"

enum class FileType
{
	MODEL,
	TEXTURE,
	ETC,
	TOTAL
};

struct FileHeader
{
	FileType m_type;
	size_t m_payloadSize;
	size_t m_subHeaderLen;
};

struct File
{	
	std::ifstream m_inBuffer;
	std::ofstream m_outBuffer;
	FileHeader m_header;
	size_t m_currPos;
	std::string m_name;
	std::string m_ext;
};

class FileSystem
{

public:

	FileSystem();
	~FileSystem();

	void GenericToBinary(std::string filePath);
	bool isRunning();

private:
	// Helper function
	bool ProcessInput(std::string& input);
	bool GetFileNameExt(std::string& filePath);
	void WriteToBin(std::string& fileName);
	void ReadRawBin(std::string& filePath);
	bool Exists(std::string& filePath);

	// Private data members
	static ModelCompiler m_modelCompiler;

	const std::string m_Ext{"kek"};
	File m_file;
	bool m_running = true;
	std::vector<char> m_buffer;
};

#endif // _FILE_SYSTEM
