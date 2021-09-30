#ifndef _MODEL_COMPILER_H_
#define _MODEL_COMPILER_H_

#include "Model.h"
#include <iostream>
#include <string>
#include <sstream>
#include <fstream>
#include <filesystem>
#include <unordered_map>

class ModelCompiler
{
public:
	
	ModelCompiler();
	~ModelCompiler();

	void Read(std::string& filePath);
	void Write(std::ofstream& outBuffer);

private:
	
	void LoadModel(std::string _Path);
	void UnloadModel();
	// This model contains all the data needed
	std::vector<Model> m_modelVec;
};

#endif // _MODEL_COMPILER_H_