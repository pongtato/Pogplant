#ifndef _MODEL_COMPILER_H_
#define _MODEL_COMPILER_H_

#include "Model.h"
#include <sstream>
#include <fstream>

class ModelCompiler
{
public:
	
	ModelCompiler();
	~ModelCompiler();

	void Read(std::string& filePath);
	void Write(std::ofstream& outBuffer);

private:
	
	void LoadModel(std::string _Path, unsigned int _PrimitiveType = 0x0004);
	void UnloadModel();
	// This model contains all the data needed
	std::vector<Model> m_modelVec;
};

#endif // _MODEL_COMPILER_H_