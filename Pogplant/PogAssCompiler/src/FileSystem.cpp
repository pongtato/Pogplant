#include "FileSystem.h"

FileSystem::FileSystem()
{
	
}

FileSystem::~FileSystem()
{

}

void FileSystem::GenericToBinary(std::string fullpath)
{
	std::string fileExt, fileName;
	if (fullpath.compare("quit") == 0)
	{
		m_running = false;
		return;
	}
	fullpath = fullpath.substr(1, fullpath.length()-2);

	// Check if file exists
	m_File.m_buffer.open(fullpath, std::ios::binary);
	if (!m_File.m_buffer.good())
	{
		std::cout << "Unable to read file." << std::endl;
		return;
	}

	// Setting the header data
	m_File.m_fileHeader.m_fileSize = std::filesystem::file_size(fullpath);

	if (!getFileNameExt(fullpath, fileExt, fileName))
	{
		std::cout << "Failed to extract file extension." << std::endl;
		return;
	}

	if (fileExt.find("obj") || fileExt.find("fbx"))
	{
		m_File.m_fileHeader.m_fileType = FileType::MODEL;
	}
	else
	{
		m_File.m_fileHeader.m_fileType = FileType::TOTAL;
	}

	// Not sure what to add to the subheader
	m_File.m_fileHeader.m_subHeaderLen = 0;

	// File reading
	m_File.m_buffer.seekg(0, std::ios::beg);
	std::vector<char> buffer(m_File.m_fileHeader.m_fileSize);
	m_File.m_buffer.read(buffer.data(), m_File.m_fileHeader.m_fileSize);
	m_File.m_buffer.close();
	
	// File writing
	std::string outFileName = fileName + '.' + m_Ext;
	std::stringstream ss;
	ss << static_cast<size_t>(m_File.m_fileHeader.m_fileType) << '\n' 
	   << m_File.m_fileHeader.m_fileSize << '\n'
	   << m_File.m_fileHeader.m_subHeaderLen << "\r\n\r\n";

	m_File.m_outBuffer.open(outFileName, std::ios::binary);
	if (!m_File.m_outBuffer.good())
	{
		std::cout << "Unable to write file." << std::endl;
		return;
	}
	m_File.m_outBuffer.write(ss.str().c_str(), ss.str().length()+1);
	m_File.m_outBuffer.write(buffer.data(), m_File.m_fileHeader.m_fileSize);
	m_File.m_outBuffer.close();

	std::cout << "Finished compiling file to kek format." << std::endl;
}

bool FileSystem::isRunning()
{
	return m_running;
}

bool FileSystem::getFileNameExt(std::string file, std::string& fileExt, std::string& fileName)
{
	bool result;
	size_t foundSlash = file.find_last_of("/\\");
	size_t foundDot = file.find('.');
	if (foundDot != std::string::npos)
	{
		fileName = file.substr(foundSlash+1, foundDot-foundSlash-1);
		fileExt = file.substr(foundDot+1);
		result = true;
	}
	else
	{
		result = false;
	}

	return result;
}
