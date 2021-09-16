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
			ss << vtx.m_Position.x << ' ' << vtx.m_Position.y << ' ' << vtx.m_Position.z << '\n';
			// Normal
			ss << vtx.m_Normal.x << ' ' << vtx.m_Normal.y << ' ' << vtx.m_Normal.z << '\n';
			// Tangent
			ss << vtx.m_Tangent.x << ' ' << vtx.m_Tangent.y << ' ' << vtx.m_Tangent.z << '\n';
			// Bi-Tangent
			ss << vtx.m_BiTangent.x << ' ' << vtx.m_BiTangent.y << ' ' << vtx.m_BiTangent.z << '\n';
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
			// ID
			ss << texture.m_Id << '\n';
			// Type of texture
			ss << texture.m_Type << '\n';
			// Path of texture
			ss << texture.m_Path << '\n';
		}
		ss << "\r\n\r\n";

		// Write to the file
		outBuffer.write(ss.str().c_str(), ss.str().length());
	}

	UnloadModel();
}