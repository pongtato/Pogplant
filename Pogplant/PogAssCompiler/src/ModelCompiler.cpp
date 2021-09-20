#include "ModelCompiler.h"

ModelCompiler::ModelCompiler()
{
}

ModelCompiler::~ModelCompiler()
{
}

void ModelCompiler::LoadModel(std::string _Path, unsigned int _PrimitiveType)
{
	Model model(_Path, _PrimitiveType);
	m_modelVec.push_back(model);
}
void ModelCompiler::UnloadModel()
{
	m_modelVec.pop_back();
}

void ModelCompiler::Read(std::string& filePath)
{
	LoadModel(filePath);
}

void ModelCompiler::Write(std::ofstream& outBuffer)
{
	// Write the meshes vertices/indices/textures in
	for (auto& meshes : m_modelVec.back().m_Meshes)
	{
		std::stringstream ss;

		// Save the vertices
		for (auto& vtx : meshes.m_Vertices)
		{
			// Position
			ss << vtx.m_Position.x << ' ' << vtx.m_Position.y << ' ' << vtx.m_Position.z << ' ';
			// Normal
			ss << vtx.m_Normal.x << ' ' << vtx.m_Normal.y << ' ' << vtx.m_Normal.z << ' ';
			// Tangent
			ss << vtx.m_Tangent.x << ' ' << vtx.m_Tangent.y << ' ' << vtx.m_Tangent.z << ' ';
			// Bi-Tangent
			ss << vtx.m_BiTangent.x << ' ' << vtx.m_BiTangent.y << ' ' << vtx.m_BiTangent.z << ' ';
			// Tex-Coords
			ss << vtx.m_TexCoords.x << ' ' << vtx.m_TexCoords.y << '\n';
		}
		ss << "\r\n\r\n";

		// Save the indices
		for (auto& idx : meshes.m_Indices)
		{
			// Indices (unsigned int)
			ss << idx << '\n';
		}
		ss << "\r\n\r\n";

		// Save the textures
		for (auto& texture : meshes.m_Textures)
		{
			// Type of texture
			ss << texture.m_Type << ' ' << texture.m_Path << '\n';
		}
		ss << "\r\n\r\n";
		// Write to the file
		outBuffer.write(ss.str().c_str(), ss.str().length());
	}

	std::stringstream ss1;
	for (auto& texloaded : m_modelVec.back().m_TexturesLoaded)
	{
		ss1 << texloaded.m_Type << ' ' << texloaded.m_Path << '\n';
	}
	ss1 << "\t\r\n";
	ss1 << m_modelVec.back().m_Directory << std::endl;
	ss1 << "\t\r\n";
	outBuffer.write(ss1.str().c_str(), ss1.str().length());

	UnloadModel();
}