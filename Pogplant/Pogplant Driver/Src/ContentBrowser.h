#pragma once
#include <filesystem>

namespace PogplantDriver
{

	class Directory
	{
	public:
		Directory();
		void RenderDirectory();
	private:
		std::filesystem::path m_AssetPath;
		std::filesystem::path m_CurrentPath;

	};
}
namespace PPD = PogplantDriver;
