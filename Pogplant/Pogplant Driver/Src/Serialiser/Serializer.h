#ifndef SERIALIZER_H
#define SERIALIZER_H

#include "../ECS/Entity.h"
#include "../../Libs/json/include/json.h"
#include "../ImguiHelper.h"
#include <string>
#include <stack>
#include <filesystem>

namespace PogplantDriver
{
	class Serializer
	{
	public:
		Serializer();
		// Save scene
		void Save(const std::string& File);
		// Load scene
		bool Load(const std::string& File);
		//
		void SavePrefab(const std::string& File, entt::entity id);
		void LoadPrefab(const std::string& File);
	private:
		std::stack<int> m_child_counter;
		std::stack<entt::entity> m_parent_id;

		void SaveObjects(const std::string& File);
		Json::Value SaveComponents(entt::entity id);
		void LoadObjects(const std::string& File);
		void LoadComponents(const Json::Value& root, entt::entity id);
		int RecurSaveChild(Json::Value& _classroot, entt::entity id, int counter);


		//helper functions
		void AddVec3To(Json::Value& _classroot, std::string _string, glm::vec3& _vec3);
		glm::vec3 CreateVec3(const Json::Value& _data);
		//reflection based function

		Json::Value Reflect_Serialization(Json::Value& _root, rttr::instance _obj);
		void Reflect_Deserialization(rttr::instance _obj, const Json::Value& _data);

	};


}



#endif