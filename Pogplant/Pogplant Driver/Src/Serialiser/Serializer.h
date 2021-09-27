#ifndef SERIALIZER_H
#define SERIALIZER_H

#include "../ECS/Entity.h"
#include "../../Libs/json/include/json.h"
#include <string>
#include <filesystem>

namespace PogplantDriver
{
	class Serializer
	{
	public:
		// Save scene
		void Save(const std::string& File);
		// Load scene
		bool Load(const std::string& File);
	private:
		void SaveObjects(const std::string& File);
		Json::Value SaveComponents(entt::entity id);

		void LoadObjects(const std::string& File);
		void LoadComponents(const Json::Value& root, entt::entity id);


		//helper functions for saving
		void AddVec3To(Json::Value& _classroot, std::string _string, glm::vec3& _vec3);
	
		//reflection based function

		Json::Value Reflect_This(Json::Value& _root, rttr::instance _obj);

	};


}



#endif