#include "FileSystem.h"

//int main()
//{
//	FileSystem fileSystem(Mode::CONT);
//	while (fileSystem.isRunning())
//	{
//		std::cout << "Drag and drop files here." << std::endl;
//
//		char input[256];
//		std::cin.getline(input, 256);
//		fileSystem.GenericToBinary(input);
//	}
//}

int main(int argc, char* argv[])
{
	std::stringstream input;
	if (argc < 1)
	{
		return 1;
	}
	else
	{
		// In case of spaces
		for (int i = 0; i < argc; ++i)
		{
			input << argv[i] << ' ';
		}
	}

	FileSystem fileSystem(Mode::ONCE);
	if (fileSystem.isRunning())
	{
		fileSystem.GenericToBinary(input.str());
	}
}