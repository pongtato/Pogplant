#include "ShaderLinker.h"
#include "ShaderResource.h"
#include "Logger.h"

#include <fstream>
#include <sstream>
#include <glew.h>

namespace Pogplant
{
	unsigned int ShaderLinker::m_ProgramHandle = 0;

	void ShaderLinker::InitShader()
	{
		/// Serialize?
		ShaderResource::AddShaderProfile(ShaderProfile("BASIC", "Shaders/Basic.vert", "Shaders/Basic.frag"));
		ShaderResource::AddShaderProfile(ShaderProfile("SCREEN", "Shaders/Screen.vert", "Shaders/Screen.frag"));
		ShaderResource::AddShaderProfile(ShaderProfile("MODEL", "Shaders/Model.vert", "Shaders/Model.frag"));
		ShaderResource::AddShaderProfile(ShaderProfile("EDGE", "Shaders/Edge.vert", "Shaders/Edge.frag"));
		ShaderResource::AddShaderProfile(ShaderProfile("SKYBOX", "Shaders/Skybox.vert", "Shaders/Skybox.frag"));

		for (const auto& it : ShaderResource::m_ShaderProfiles)
		{
			LoadShader(it.second.m_ProgramID, it.second.m_VertexPath, it.second.m_FragmentPath);
		}
	}

	void ShaderLinker::Use(const char* _ProgramID)
	{
		m_ProgramHandle = ShaderResource::m_ShaderPrograms[_ProgramID];
		glUseProgram(m_ProgramHandle);
	}

	void ShaderLinker::UnUse()
	{
		glUseProgram(0);
	}

	void ShaderLinker::SetUniform(char const* _Name, bool _Val)
	{
		GLint loc = glGetUniformLocation(m_ProgramHandle, _Name);
		if (loc >= 0)
		{
			glUniform1i(loc, _Val);
		}
		else
		{
			std::stringstream logEntry;
			logEntry << "Uniform variable " << _Name << " doesn't exist";
			Logger::Log({ "PP::SHADER",LogEntry::WARNING, logEntry.str() });
		}
	}

	void ShaderLinker::SetUniform(char const* _Name, int _Val)
	{
		GLint loc = glGetUniformLocation(m_ProgramHandle, _Name);
		if (loc >= 0)
		{
			glUniform1i(loc, _Val);
		}
		else
		{
			std::stringstream logEntry;
			logEntry << "Uniform variable " << _Name << " doesn't exist";
			Logger::Log({ "PP::SHADER",LogEntry::WARNING, logEntry.str() });
		}
	}

	void ShaderLinker::SetUniform(char const* _Name, float _Val)
	{
		GLint loc = glGetUniformLocation(m_ProgramHandle, _Name);
		if (loc >= 0)
		{
			glUniform1f(loc, _Val);
		}
		else
		{
			std::stringstream logEntry;
			logEntry << "Uniform variable " << _Name << " doesn't exist";
			Logger::Log({ "PP::SHADER",LogEntry::WARNING, logEntry.str() });
		}
	}

	void ShaderLinker::SetUniform(char const* _Name, float _X, float _Y)
	{
		GLint loc = glGetUniformLocation(m_ProgramHandle, _Name);
		if (loc >= 0)
		{
			glUniform2f(loc, _X, _Y);
		}
		else
		{
			std::stringstream logEntry;
			logEntry << "Uniform variable " << _Name << " doesn't exist";
			Logger::Log({ "PP::SHADER",LogEntry::WARNING, logEntry.str() });
		}
	}

	void ShaderLinker::SetUniform(char const* _Name, float _X, float _Y, float _Z)
	{
		GLint loc = glGetUniformLocation(m_ProgramHandle, _Name);
		if (loc >= 0)
		{
			glUniform3f(loc, _X, _Y, _Z);
		}
		else
		{
			std::stringstream logEntry;
			logEntry << "Uniform variable " << _Name << " doesn't exist";
			Logger::Log({ "PP::SHADER",LogEntry::WARNING, logEntry.str() });
		}
	}

	void ShaderLinker::SetUniform(char const* _Name, float _X, float _Y, float _Z, float _W)
	{
		GLint loc = glGetUniformLocation(m_ProgramHandle, _Name);
		if (loc >= 0)
		{
			glUniform4f(loc, _X, _Y, _Z, _W);
		}
		else
		{
			std::stringstream logEntry;
			logEntry << "Uniform variable " << _Name << " doesn't exist";
			Logger::Log({ "PP::SHADER",LogEntry::WARNING, logEntry.str() });
		}
	}

	void ShaderLinker::SetUniform(char const* _Name, const glm::vec2& _Val)
	{
		GLint loc = glGetUniformLocation(m_ProgramHandle, _Name);
		if (loc >= 0)
		{
			glUniform2f(loc, _Val.x, _Val.y);
		}
		else
		{
			std::stringstream logEntry;
			logEntry << "Uniform variable " << _Name << " doesn't exist";
			Logger::Log({ "PP::SHADER",LogEntry::WARNING, logEntry.str() });
		}
	}

	void ShaderLinker::SetUniform(char const* _Name, const glm::vec3& _Val)
	{
		GLint loc = glGetUniformLocation(m_ProgramHandle, _Name);
		if (loc >= 0)
		{
			glUniform3f(loc, _Val.x, _Val.y, _Val.z);
		}
		else
		{
			std::stringstream logEntry;
			logEntry << "Uniform variable " << _Name << " doesn't exist";
			Logger::Log({ "PP::SHADER",LogEntry::WARNING, logEntry.str() });
		}
	}

	void ShaderLinker::SetUniform(char const* _Name, const glm::vec4& _Val)
	{
		GLint loc = glGetUniformLocation(m_ProgramHandle, _Name);
		if (loc >= 0)
		{
			glUniform4f(loc, _Val.x, _Val.y, _Val.z, _Val.w);
		}
		else
		{
			std::stringstream logEntry;
			logEntry << "Uniform variable " << _Name << " doesn't exist";
			Logger::Log({ "PP::SHADER",LogEntry::WARNING, logEntry.str() });
		}
	}

	void ShaderLinker::SetUniform(char const* _Name, const glm::mat3& _Val)
	{
		GLint loc = glGetUniformLocation(m_ProgramHandle, _Name);
		if (loc >= 0)
		{
			glUniformMatrix3fv(loc, 1, GL_FALSE, &_Val[0][0]);
		}
		else
		{
			std::stringstream logEntry;
			logEntry << "Uniform variable " << _Name << " doesn't exist";
			Logger::Log({ "PP::SHADER",LogEntry::WARNING, logEntry.str() });
		}
	}

	void ShaderLinker::SetUniform(char const* _Name, const glm::mat4& _Val)
	{
		GLint loc = glGetUniformLocation(m_ProgramHandle, _Name);
		if (loc >= 0)
		{
			glUniformMatrix4fv(loc, 1, GL_FALSE, &_Val[0][0]);
		}
		else
		{
			std::stringstream logEntry;
			logEntry << "Uniform variable " << _Name << " doesn't exist";
			Logger::Log({ "PP::SHADER",LogEntry::WARNING, logEntry.str() });
		}
	}

	unsigned int ShaderLinker::GetHandle()
	{
		return m_ProgramHandle;
	}

	void ShaderLinker::LoadShader(const char* const _ProgramID, const char* const _VertexPath, const char* const _FragmentPath)
	{
		// Get vertex/fragment shader code from file path
		std::string vertexCode;
		std::string fragmentCode;
		std::ifstream vShaderFile;
		std::ifstream fShaderFile;

		// For throwing exceptions
		vShaderFile.exceptions(std::ifstream::badbit | std::ifstream::failbit);
		fShaderFile.exceptions(std::ifstream::badbit | std::ifstream::failbit);

		try
		{
			std::stringstream VShaderStream, FShaderStream, GShaderStream;

			// Open files
			vShaderFile.open(_VertexPath);
			fShaderFile.open(_FragmentPath);

			// Read file data
			VShaderStream << vShaderFile.rdbuf();
			FShaderStream << fShaderFile.rdbuf();

			// Parse to string
			vertexCode = VShaderStream.str();
			fragmentCode = FShaderStream.str();

			// Close when done
			vShaderFile.close();
			fShaderFile.close();
		}
		catch (std::ifstream::failure e)
		{
			std::stringstream logEntry;
			logEntry << _ProgramID << " Failed to read shader file";
			Logger::Log({ "PP::SHADER",LogEntry::ERROR, logEntry.str() });
		}

		///Compiling of shaders
		//To pass into func
		const char* vShaderCode = vertexCode.c_str();
		const char* fShaderCode = fragmentCode.c_str();

		//Shader objects
		unsigned int vertex;
		unsigned int fragment;

		int success;
		char infoLog[512];

		/// Vertex Shader
		vertex = glCreateShader(GL_VERTEX_SHADER);
		glShaderSource(vertex, 1, &vShaderCode, NULL);
		glCompileShader(vertex);

		// Print compile errors if any
		glGetShaderiv(vertex, GL_COMPILE_STATUS, &success);
		if (!success)
		{
			glGetShaderInfoLog(vertex, 512, NULL, infoLog);
			std::stringstream logEntry;
			logEntry << _ProgramID << " Failed to compile vertex shader "  << " | " << infoLog;
			Logger::Log({ "PP::SHADER",LogEntry::ERROR, logEntry.str() });
		}

		/// Fragment Shader
		fragment = glCreateShader(GL_FRAGMENT_SHADER);
		glShaderSource(fragment, 1, &fShaderCode, NULL);
		glCompileShader(fragment);

		// Print compile errors if any
		glGetShaderiv(fragment, GL_COMPILE_STATUS, &success);
		if (!success)
		{
			glGetShaderInfoLog(fragment, 512, NULL, infoLog);
			std::stringstream logEntry;
			logEntry << _ProgramID << " Failed to compile fragment shader " << " | " << infoLog;
			Logger::Log({ "PP::SHADER",LogEntry::ERROR, logEntry.str() });
		}

		// Shader Program
		ShaderResource::m_ShaderPrograms[_ProgramID] = glCreateProgram();
		glAttachShader(ShaderResource::m_ShaderPrograms[_ProgramID], vertex);
		glAttachShader(ShaderResource::m_ShaderPrograms[_ProgramID], fragment);
		glLinkProgram(ShaderResource::m_ShaderPrograms[_ProgramID]);

		// Print linking errors if any
		glGetProgramiv(ShaderResource::m_ShaderPrograms[_ProgramID], GL_LINK_STATUS, &success);
		if (!success)
		{
			glGetProgramInfoLog(ShaderResource::m_ShaderPrograms[_ProgramID], 512, NULL, infoLog);
			std::stringstream logEntry;
			logEntry << _ProgramID << " Shaders failed to link " << " | " << infoLog;
			Logger::Log({ "PP::SHADER",LogEntry::ERROR, logEntry.str() });
		}
		else
		{
			std::stringstream logEntry;
			logEntry << _ProgramID << " Shaders linked";
			Logger::Log({ "PP::SHADER",LogEntry::SUCCESS, logEntry.str() });
		}

		// Delete the shaders as they're linked into our program now and no longer necessary
		glDeleteShader(vertex);
		glDeleteShader(fragment);
	}
}
