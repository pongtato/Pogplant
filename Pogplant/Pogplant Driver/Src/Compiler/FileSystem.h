/*****************************************************************************/
/*!
\file	FileSystem.h
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	FileSystem class that calls the necessary compiler

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

#ifndef _FILE_SYSTEM_
#define _FILE_SYSTEM_

#include "ModelCompiler.h"
#include "Logger.h"

enum class Mode
{
	CONT,
	ONCE,
	TOTAL
};

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
};

struct File
{	
	FileHeader m_header;
	size_t m_currPos;
	std::string m_name;
	std::string m_ext;
};

class FileSystem
{

public:

	FileSystem(Mode mode);
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
	const std::string m_Ext{".kek"};
	Mode m_mode;
	File m_file;
	bool m_running = true;
	std::vector<char> m_buffer;
};

#endif // _FILE_SYSTEM
