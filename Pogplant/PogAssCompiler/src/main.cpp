#include "FileSystem.h"

int main()
{
	FileSystem fileSystem;
	while (fileSystem.isRunning())
	{
		std::cout << "Drag and drop files here." << std::endl;

		char input[256];
		std::cin.getline(input, 256);
		fileSystem.GenericToBinary(input);
	}
}