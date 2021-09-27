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
	/* Read the raw binary dump, not needed for models, not sure about font */
	ReadRawBin(filePath);
	// Not sure what to add to the subheader
	m_file.m_header.m_subHeaderLen = 0;

	// Write back the data
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

	// Have to further process the input if it's CONT mode
	if (m_mode == Mode::CONT)
	{
		input = input.substr(1, input.length() - 2);
	}

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
		result = false;
	}

	return result;
}

void FileSystem::WriteToBin(std::string& fileName)
{
	// File writing
	std::string outPath = "Resources/KekFiles";
	std::string outFile = fileName + '.' + m_Ext;
	std::string filePath = outPath + '/' + outFile;
	std::filesystem::create_directories(outPath);
	std::ofstream outBuffer(filePath.c_str(), std::ios::binary | std::ios::trunc);

	if (outBuffer.fail())
	{
		std::cout << "Unable to write file." << std::endl;
		return;
	}

	// Header Stuff
	//std::stringstream ss;
	//ss << static_cast<size_t>(m_file.m_header.m_type) << "\r\n\r\n";
	//outBuffer.write(ss.str().c_str(), ss.str().length());

	/* If file type is model, call the model compiler to write */
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
	std::cout << "[POGPLANT COMPILER] Finished compiling " << m_file.m_name << " to kek format." << std::endl;
}

void FileSystem::ReadRawBin(std::string& filePath)
{
	// Check if file exists & read to buffer
	std::ifstream inBuffer(filePath, std::ios::binary);

	if (inBuffer.fail())
	{
		std::cout << "Unable to read file." << std::endl;
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

	std::cout << "Unable to find file." << std::endl;
	return false;
}
