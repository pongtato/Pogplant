#pragma once
#include <unordered_map>
#include <string>

namespace Pogplant
{
	struct ShaderProfile
	{
		ShaderProfile() = default;
		ShaderProfile(std::string _ProgramID, std::string _Directory, std::string _VertexPath, std::string _FragmentPath, std::string _GeometryPath = "");

		std::string m_ProgramID;
		std::string m_VertexPath;
		std::string m_FragmentPath;
		std::string m_GeometryPath;
	};

	struct ShaderResource
	{
		static void AddShaderProfile(const ShaderProfile& _ShaderProfile);

		/// Container
		static std::unordered_map < std::string, unsigned int > m_ShaderPrograms;
		static std::unordered_map < std::string, ShaderProfile > m_ShaderProfiles;
	};
}