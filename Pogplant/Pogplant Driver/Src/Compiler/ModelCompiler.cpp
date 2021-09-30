#include "ModelCompiler.h"

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

	for (auto& meshes : model.m_Meshes)
	{
		std::stringstream sizes;
		sizes << meshes.m_Vertices.size() << ' ' << meshes.m_Indices.size() << ' ' << meshes.m_Textures.size() << '\n';
		outBuffer.write(sizes.str().c_str(), sizes.str().length());
		outBuffer.write(reinterpret_cast<char*>(&meshes.m_Vertices[0]), meshes.m_Vertices.size() * sizeof(Vertex));
		outBuffer.write(reinterpret_cast<char*>(&meshes.m_Indices[0]), meshes.m_Indices.size() * sizeof(uint));

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

	// Write the meshes directory/vertices/indices/textures in
	//std::stringstream dir;
	//dir << m_modelVec.back().m_Directory << std::endl;
	//dir << "\r\n\r\n";
	//outBuffer.write(dir.str().c_str(), dir.str().length());

	//for (auto& meshes : m_modelVec.back().m_Meshes)
	//{
	//	std::stringstream ss;
	//	// Save the vertices
	//	for (auto& vtx : meshes.m_Vertices)
	//	{
	//		// Position
	//		ss << vtx.m_Position.x << ' ' << vtx.m_Position.y << ' ' << vtx.m_Position.z << ' ';
	//		// Normal
	//		ss << vtx.m_Normal.x << ' ' << vtx.m_Normal.y << ' ' << vtx.m_Normal.z << ' ';
	//		// Tangent
	//		ss << vtx.m_Tangent.x << ' ' << vtx.m_Tangent.y << ' ' << vtx.m_Tangent.z << ' ';
	//		// Bi-Tangent
	//		ss << vtx.m_BiTangent.x << ' ' << vtx.m_BiTangent.y << ' ' << vtx.m_BiTangent.z << ' ';
	//		// Tex-Coords
	//		ss << vtx.m_TexCoords.x << ' ' << vtx.m_TexCoords.y << '\n';
	//	}
	//	ss << "\r\n\r\n";
	//	// Save the indices
	//	for (auto& idx : meshes.m_Indices)
	//	{
	//		// Indices (unsigned int)
	//		ss << idx << '\n';
	//	}
	//	ss << "\r\n\r\n";
	//	// Save the textures
	//	for (auto& texture : meshes.m_Textures)
	//	{
	//		// Type of texture
	//		ss << texture.m_Type << ' ' << texture.m_Path << '\n';
	//	}
	//	ss << "\r\n\r\n";
	//		//Write to the file
	//	outBuffer.write(ss.str().c_str(), ss.str().length());
	//}
	UnloadModel();
}
	