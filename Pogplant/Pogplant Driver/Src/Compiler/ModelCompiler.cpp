/*****************************************************************************/
/*!
\file	ModelCompiler.cpp
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	ModelCompiler class to compile models to own filetype

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

#include "ModelCompiler.h"
#include "Logger.h"

ModelCompiler::ModelCompiler()
{
}

ModelCompiler::~ModelCompiler()
{
}

void ModelCompiler::LoadModel(std::string _Path)
{
	Model model(_Path);
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
	std::stringstream dir;
	Model model = m_modelVec.back();
	dir << model.m_Directory << '\n';
	outBuffer.write(dir.str().c_str(), dir.str().length());
	outBuffer.write(reinterpret_cast<char*>(&model.m_Bounds), sizeof(Bounds));

	for (auto& it : model.m_Meshes)
	{
		auto& meshes = it.second;

		std::stringstream sizes;
		sizes << meshes.m_Vertices.size() << ' ' << meshes.m_Indices.size() << ' ' << meshes.m_Textures.size() << '\n';
		outBuffer.write(sizes.str().c_str(), sizes.str().length());
		outBuffer.write(reinterpret_cast<char*>(&meshes.m_Vertices[0]), meshes.m_Vertices.size() * sizeof(Vertex));
		outBuffer.write(reinterpret_cast<char*>(&meshes.m_Indices[0]), meshes.m_Indices.size() * sizeof(uint));
		outBuffer.write(reinterpret_cast<char*>(&meshes.m_Translate), meshes.m_Translate.length() * sizeof(float));
		outBuffer.write(reinterpret_cast<char*>(&meshes.m_Rotate), meshes.m_Rotate.length() * sizeof(float));
		outBuffer.write(reinterpret_cast<char*>(&meshes.m_Scale), meshes.m_Scale.length() * sizeof(float));

		size_t parentLen = meshes.m_ParentName.size();
		if (parentLen > 0)
		{
			outBuffer.write(reinterpret_cast<char*>(&parentLen), sizeof(size_t));
			outBuffer.write(meshes.m_ParentName.c_str(), parentLen);
		}

		size_t nameLen = meshes.m_Name.size();
		if (nameLen > 0)
		{
			outBuffer.write(reinterpret_cast<char*>(&nameLen), sizeof(size_t));
			outBuffer.write(meshes.m_Name.c_str(), nameLen);
		}

		for (auto& textures : meshes.m_Textures)
		{
			size_t len = textures.m_Type.size();
			outBuffer.write(reinterpret_cast<char*>(&len), sizeof(size_t));
			outBuffer.write(textures.m_Type.c_str(), len);

			len = textures.m_Path.size();
			outBuffer.write(reinterpret_cast<char*>(&len), sizeof(size_t));
			outBuffer.write(textures.m_Path.c_str(), len);
		}
	}
	UnloadModel();
}
	