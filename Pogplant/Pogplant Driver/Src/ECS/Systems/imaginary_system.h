#ifndef  IMAGINARY_H
#define IMAGINARY_H

#include "../ECS.h"

class ECS;

class Imaginary_system
{
public:
	Imaginary_system();
	~Imaginary_system();

	void Init(ECS* ecs);
	void Update();

private:
	ECS* m_registry;

};


#endif // ! IMAGINARY_H
