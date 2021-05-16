#include "ShaderResource.h"
// Printing error to console - To be replaced by IMGUI
#ifdef _DEBUG
#include <iostream>
#endif

namespace Pogplant
{
	std::unordered_map < const char*, unsigned int > ShaderResource::m_ShaderPrograms;
	std::unordered_map<const char*, ShaderProfile> ShaderResource::m_ShaderProfiles;

	ShaderProfile::ShaderProfile(const char* const _ProgramID, const char* const _VertexPath, const char* const _FragmentPath)
		: m_ProgramID{ _ProgramID }
		, m_VertexPath{ _VertexPath }
		, m_FragmentPath{ _FragmentPath }
	{
	};

	void ShaderResource::AddShaderProfile(const ShaderProfile& _ShaderProfile)
	{
#ifdef _DEBUG
		if(m_ShaderProfiles.find(_ShaderProfile.m_ProgramID) != m_ShaderProfiles.end())
		{
			std::cout << "[PP::SHADER] Duplicate shader profile ID" << std::endl;
		}
#endif
		m_ShaderProfiles[_ShaderProfile.m_ProgramID] = _ShaderProfile;
	}
}