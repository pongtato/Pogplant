#pragma once

#include <glm.hpp>

namespace Pogplant
{
	struct InstanceData
	{
		glm::mat4 m_Model;
		glm::vec4 m_Color;
		glm::vec2 m_Tiling;
		glm::vec2 m_UV_Offset;
		int m_TexID;
		// Cant be bool, GLSL does not have varying bool
		unsigned int m_Ortho;
		unsigned int m_GameOnly;
	};

	class MeshInstance
	{
	public:
		static void InitMeshInstance(size_t _Pool_Size);
		static void SetInstance(InstanceData _Instance_Data);
		static void ResetCount();
		static size_t GetInstanceCount();

	private:
		static size_t m_InstanceCount;
	};
}

