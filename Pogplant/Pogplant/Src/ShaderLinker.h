#pragma once
#include <glm.hpp>

namespace Pogplant
{


	class ShaderLinker
	{
	public:
		/// Functions
		static void InitShader();
		static void Use(char const* _ProgramID);
		static void UnUse();

		/// Setters
		static void SetUniform(char const* _Name, bool _Val);
		static void SetUniform(char const* _Name, int _Val);
		static void SetUniform(char const* _Name, float _Val);
		static void SetUniform(char const* _Name, float _X, float _Y);
		static void SetUniform(char const* _Name, float _X, float _Y, float _Z);
		static void SetUniform(char const* _Name, float _X, float _Y, float _Z, float _W);
		static void SetUniform(char const* _Name, const glm::vec2& _Val);
		static void SetUniform(char const* _Name, const glm::vec3& _Val);
		static void SetUniform(char const* _Name, const glm::vec4& _Val);
		static void SetUniform(char const* _Name, const glm::mat3& _Val);
		static void SetUniform(char const* _Name, const glm::mat4& _Val);

		/// Getters
		static unsigned int GetHandle();

	private:
		static bool LoadShader(const char* const _ProgramID, const char* const _VertexPath, const char* const _FragmentPath, const char* const _GeometryPath);

		/// Var
		static unsigned int m_ProgramHandle;
	};
}

