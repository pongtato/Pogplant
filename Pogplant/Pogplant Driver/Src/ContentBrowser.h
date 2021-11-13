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
		float pad;
		float thumbsize;
		float cellsize;
		float panelsize;
		int col_count;

		std::string current_file;

		std::filesystem::path m_AssetPath;
		std::filesystem::path m_CurrentPath;

		void DrawIndividual(const std::filesystem::directory_entry& _entry);

	};
}
namespace PPD = PogplantDriver;
