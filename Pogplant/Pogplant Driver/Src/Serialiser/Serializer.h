#ifndef SERIALIZER_H
#define SERIALIZER_H

#include "../ECS/Entity.h"
#include "../../Libs/json/include/json.h"
#include <string>
#include <filesystem>

namespace Pogplant
{
	class Serializer
	{
	public:
		// Save scene
		void Save(const std::string& File);
		// Load scene
		void Load(const std::string& File);
	private:
		void SaveObjects(const std::string& File);
		Json::Value SaveComponents(entt::entity id);

	};


}



#endif