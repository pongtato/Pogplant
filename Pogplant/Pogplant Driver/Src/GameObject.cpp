#include "GameObject.h"
#include <gtc/type_ptr.hpp>

GameObject::GameObject(glm::vec3 _Pos, glm::vec3 _Rot, glm::vec3 _Scale)
{
	// Set translate to array
	memcpy(m_Position, glm::value_ptr(_Pos), sizeof(_Pos));

	// Set rotation to array
	memcpy(m_Rotation, glm::value_ptr(_Rot), sizeof(_Rot));

	// Set scale to array
	memcpy(m_Scale, glm::value_ptr(_Scale), sizeof(_Scale));

	// Calculate matrix
	glm::mat4 model = glm::mat4{ 1 };
	model = glm::translate(model, _Pos);
	model = glm::rotate(model, m_Rotation[0], { 1,0,0 });
	model = glm::rotate(model, m_Rotation[1], { 0,1,0 });
	model = glm::rotate(model, m_Rotation[2], { 0,0,1 });
	model = glm::scale(model, _Scale);

	// Convert back to float
	memcpy(m_ModelMtx, glm::value_ptr(model), sizeof(model));
}
