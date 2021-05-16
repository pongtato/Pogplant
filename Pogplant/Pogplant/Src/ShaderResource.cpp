#include "ShaderResource.h"
#include "Logger.h"

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
		if(m_ShaderProfiles.find(_ShaderProfile.m_ProgramID) != m_ShaderProfiles.end())
		{
			Logger::Log({ "PP::SHADER",LogEntry::ERROR,"Duplicate shader profile ID" });
		}
		m_ShaderProfiles[_ShaderProfile.m_ProgramID] = _ShaderProfile;
	}
}