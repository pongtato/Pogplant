/*****************************************************************************/
/*!
\file	FileSystem.cpp
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	FileSystem class that calls the necessary compiler

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

#include "FileSystem.h"

ModelCompiler FileSystem::m_modelCompiler;

FileSystem::FileSystem(Mode mode) :
	m_mode{ mode }
{
}

FileSystem::~FileSystem()
{
}

void FileSystem::GenericToBinary(std::string filePath)
{
	if (!ProcessInput(filePath))
	{
		return;
	}
	
	// Read
	ReadRawBin(filePath);
	// Write 
	WriteToBin(m_file.m_name);
	m_buffer.clear();
}

bool FileSystem::isRunning()
{
	return m_running;
}

bool FileSystem::ProcessInput(std::string& input)
{
	// Check if input is empty
	if (input.empty())
	{
		Logger::Log({ "PP::COMPILER", LogEntry::LOGTYPE::ERROR, "Input is empty" });
		return false;
	}

	// Have to further process the input if it's CONT mode
	if (m_mode == Mode::CONT)
	{
		input = input.substr(1, input.length() - 2);
	}

	// Check if file exists
	if (!Exists(input))
	{
		return false;
	}

	// Set the file size
	m_file.m_header.m_payloadSize = std::filesystem::file_size(input);

	// Get the file extension and file name
	if (!GetFileNameExt(input))
	{
		return false;
	}

	/* Classify file type based on extension, call the right compiler */
	if (m_file.m_ext.find("obj") || m_file.m_ext.find("fbx"))
	{
		m_file.m_header.m_type = FileType::MODEL;
	}
	else
	{
		m_file.m_header.m_type = FileType::TOTAL;
	}

	return true;
}

bool FileSystem::GetFileNameExt(std::string& filePath)
{
	bool result;
	size_t foundSlash = filePath.find_last_of("/\\");
	size_t foundDot = filePath.find('.');

	if (foundDot != std::string::npos)
	{
		m_file.m_name = filePath.substr(foundSlash+1, foundDot-foundSlash-1);
		m_file.m_ext = filePath.substr(foundDot+1);
		result = true;
	}
	else
	{
		Logger::Log({ "PP::COMPILER", LogEntry::LOGTYPE::ERROR, "Unable to extract file extension" });
		result = false;
	}

	return result;
}

void FileSystem::WriteToBin(std::string& fileName)
{
	// File writing
	std::string outPath = "Resources/KekFiles";
	std::string outFile = fileName + m_Ext;
	std::string filePath = outPath + '/' + outFile;
	std::filesystem::create_directories(outPath);
	std::ofstream outBuffer(filePath.c_str(), std::ios::binary | std::ios::trunc);

	if (outBuffer.fail())
	{
		Logger::Log({ "PP::COMPILER", LogEntry::LOGTYPE::ERROR, "Unable to write file"});
		return;
	}

	// If file type is model, call the model compiler to write
	if (m_file.m_header.m_type == FileType::MODEL)
	{
		m_modelCompiler.Write(outBuffer);
	}
	else
	{
		 outBuffer.write(m_buffer.data(), m_file.m_header.m_payloadSize);
	}
	
	// Close the buffer
	outBuffer.close();
	std::filesystem::rename(filePath.c_str(), filePath.c_str());
	std::string succ = "Finished compiling " + m_file.m_name + '.' + m_file.m_ext + "to " + m_Ext + " format";
	Logger::Log({ "PP::COMPILER", LogEntry::LOGTYPE::SUCCESS, succ });
}

void FileSystem::ReadRawBin(std::string& filePath)
{
	// Check if file exists & read to buffer
	std::ifstream inBuffer(filePath, std::ios::binary);

	if (inBuffer.fail())
	{
		Logger::Log({ "PP::COMPILER", LogEntry::LOGTYPE::ERROR, "Unable to read file" });
		return;
	}

	// Call the right compiler to read
	if (m_file.m_header.m_type == FileType::MODEL)
	{
		m_modelCompiler.Read(filePath);
	}
	else
	{
		m_buffer.reserve(m_file.m_header.m_payloadSize);
		inBuffer.seekg(0, std::ios::beg);
		inBuffer.read(m_buffer.data(), m_file.m_header.m_payloadSize);
	}

	inBuffer.close();
}

bool FileSystem::Exists(std::string& filePath)
{
	std::ifstream file{ filePath };
	if (file.good())
	{
		return true;
	}

	Logger::Log({ "PP::COMPILER", LogEntry::LOGTYPE::ERROR, "Unable to find file" });
	return false;
}
