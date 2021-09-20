#include "FileSystem.h"

ModelCompiler FileSystem::m_modelCompiler;

FileSystem::FileSystem()
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

	m_buffer.reserve(m_file.m_header.m_payloadSize);
	// Read the raw binary dump
	ReadRawBin(filePath);

	if (m_file.m_ext.find("obj") || m_file.m_ext.find("fbx"))
	{
		m_file.m_header.m_type = FileType::MODEL;
		m_modelCompiler.Read(filePath);
	}
	else
	{
		m_file.m_header.m_type = FileType::TOTAL;
	}

	// Not sure what to add to the subheader
	m_file.m_header.m_subHeaderLen = 0;

	// Write the raw binary dump
	WriteToBin(m_file.m_name);
	m_buffer.clear();
}

bool FileSystem::isRunning()
{
	return m_running;
}

bool FileSystem::ProcessInput(std::string& input)
{
	if (input.compare("quit") == 0)
	{
		m_running = false;
		return false;
	}

	if (input.empty())
	{
		std::cout << "Input is empty, try again." << std::endl;
		return false;
	}

	input = input.substr(1, input.length() - 2);
	if (!Exists(input))
	{
		return false;
	}

	// Set the file size
	m_file.m_header.m_payloadSize = std::filesystem::file_size(input);

	// Get the file extension and file name
	if (!GetFileNameExt(input))
	{
		std::cout << "Failed to extract file extension." << std::endl;
		return false;
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
		result = false;
	}

	return result;
}

void FileSystem::WriteToBin(std::string& fileName)
{
	// File writing
	std::string outFileName = fileName + '.' + m_Ext;
	m_file.m_outBuffer.open(outFileName, std::ios::binary);
	if (!m_file.m_outBuffer.good())
	{
		std::cout << "Unable to write file." << std::endl;
		return;
	}

	//// Header Stuff
	//std::stringstream ss;
	//ss << static_cast<size_t>(m_file.m_header.m_type) << '\n'
	//	<< m_file.m_header.m_payloadSize << '\n'
	//	<< m_file.m_header.m_subHeaderLen << "\r\n\r\n";
	//m_file.m_outBuffer.write(ss.str().c_str(), ss.str().length() + 1);

	// Header Stuff
	std::stringstream ss;
	ss << static_cast<size_t>(m_file.m_header.m_type) << '\n'
	   << m_file.m_header.m_payloadSize << "\r\n\r\n";
	m_file.m_outBuffer.write(ss.str().c_str(), ss.str().length());

	m_modelCompiler.Write(m_file.m_outBuffer);

	// Writing the raw binary dump (Payload)
	//m_file.m_outBuffer.write(m_buffer.data(), m_file.m_header.m_payloadSize);

	// Close the buffer
	m_file.m_outBuffer.close();
	
	
	std::cout << "Finished compiling file to kek format." << std::endl;
}

void FileSystem::ReadRawBin(std::string& filePath)
{
	// Check if file exists & read to buffer
	m_file.m_inBuffer.open(filePath, std::ios::binary);
	if (!m_file.m_inBuffer.good())
	{
		std::cout << "Unable to read file." << std::endl;
		return;
	}
	m_file.m_inBuffer.seekg(0, std::ios::beg);
	m_file.m_inBuffer.read(m_buffer.data(), m_file.m_header.m_payloadSize);
	m_file.m_inBuffer.close();
}

bool FileSystem::Exists(std::string& filePath)
{
	std::ifstream file{ filePath };
	if (file.good())
	{
		return true;
	}

	std::cout << "Unable to find file." << std::endl;
	return false;
}
