#include "ShaderResource.h"
#include "Logger.h"
#include <assert.h>

namespace Pogplant
{
	std::unordered_map <std::string, unsigned int> ShaderResource::m_ShaderPrograms;
	std::unordered_map <std::string, ShaderProfile> ShaderResource::m_ShaderProfiles;

	ShaderProfile::ShaderProfile(std::string _ProgramID, std::string _Directory, std::string _VertexPath, std::string _FragmentPath)
		: m_ProgramID{ _ProgramID }
		, m_VertexPath{ _Directory + _VertexPath }
		, m_FragmentPath{ _Directory + _FragmentPath }
	{
	};

	void ShaderResource::AddShaderProfile(const ShaderProfile& _ShaderProfile)
	{
		assert(m_ShaderProfiles.find(_ShaderProfile.m_ProgramID) == m_ShaderProfiles.end() && "Duplicate shader profile ID");
		/*if(m_ShaderProfiles.find(_ShaderProfile.m_ProgramID) != m_ShaderProfiles.end())
		{
			Logger::Log({ "PP::SHADER",LogEntry::LOGTYPE::ERROR,"Duplicate shader profile ID" });
		}*/
		m_ShaderProfiles[_ShaderProfile.m_ProgramID] = _ShaderProfile;
	}
}