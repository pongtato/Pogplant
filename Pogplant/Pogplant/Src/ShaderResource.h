#pragma once
#include <unordered_map>

namespace Pogplant
{
	struct ShaderProfile
	{
		ShaderProfile() = default;
		ShaderProfile(const char* const _ProgramID, const char* const _VertexPath, const char* const _FragmentPath);

		const char* m_ProgramID;
		const char* m_VertexPath;
		const char* m_FragmentPath;
	};

	struct ShaderResource
	{
		static void AddShaderProfile(const ShaderProfile& _ShaderProfile);

		/// Container
		static std::unordered_map < const char*, unsigned int > m_ShaderPrograms;
		static std::unordered_map < const char*, ShaderProfile > m_ShaderProfiles;
	};
}