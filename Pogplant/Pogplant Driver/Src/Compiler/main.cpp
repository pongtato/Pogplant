/*****************************************************************************/
/*!
\file	main.cpp
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	Pogplant compiler used to compile just models for now.
	main file for Pogplant Compiler

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

#include "FileSystem.h"

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
	Logger::Print();
}