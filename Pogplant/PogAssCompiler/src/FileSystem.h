#ifndef _FILE_SYSTEM_
#define _FILE_SYSTEM_

#include <iostream>
#include <string>
#include <sstream>
#include <fstream>
#include <filesystem>
#include <unordered_map>

enum class FileType
{
	MODEL,
	TEXTURE,
	ETC,
	TOTAL
};

struct FileHeader
{
	FileType m_fileType;
	size_t m_fileSize;
	size_t m_subHeaderLen;
};

struct File
{	
	std::ifstream m_buffer;
	std::ofstream m_outBuffer;
	FileHeader m_fileHeader;
	static constexpr size_t m_maxHeaderLen {40};
	size_t m_currPos;
};

class FileSystem
{
	using Filename = std::string;

public:

	FileSystem();
	~FileSystem();

	void GenericToBinary(std::string fullpath);
	bool isRunning();

private:
	
	// Helper function
	bool getFileNameExt(std::string file, std::string& fileExt, std::string& fileName);

	// Private data members
	//std::unordered_map<Filename, File> m_Files;
	const std::string m_Ext{"kek"};
	File m_File;
	bool m_running = true;
};

#endif // _FILE_SYSTEM
