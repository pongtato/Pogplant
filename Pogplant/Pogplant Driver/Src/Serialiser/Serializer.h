#ifndef SERIALIZER_H
#define SERIALIZER_H

#include "../ECS/Entity.h"
#include "../../Libs/json/include/json.h"
#include <string>
#include <stack>
#include <filesystem>
#include <fstream>

namespace PogplantDriver
{
	class Serializer
	{
	public:

		Serializer(ECS& ecs);
		// Save scene
		void Save(const std::string& File);
		// Load scene
		bool Load(const std::string& File);
		//
		void SavePrefab(const std::string& File, entt::entity id);
		void LoadPrefab(const std::string& File);
	private:
		ECS& m_ecs;
		
		std::set<entt::entity> m_saved;
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

		void AddMat4To(Json::Value& _classroot, std::string _string, glm::mat4& _mat4);
		glm::mat4 CreateMat4(const Json::Value& _data);

		//reflection based function
		Json::Value Reflect_Serialization(Json::Value& _root, rttr::instance _obj);
		void Reflect_Deserialization(rttr::instance _obj, const Json::Value& _data);
		bool Save_arithmetic(Json::Value& _root, const std::string& _name, rttr::type _type,rttr::variant& _value);
		bool Load_arithmetic(rttr::type _type, rttr::property& _prop, rttr::instance& _obj, const Json::Value& _data);

		template <typename T>
		void Try_Load_Component(const Json::Value& _root, std::string _name, entt::entity _id)
		{
			if (_root[_name])
			{
				T _component;
				Reflect_Deserialization(_component, _root[_name]);
				m_ecs.GetReg().emplace<T>(_id, _component);
			}
		}

		template <typename T>
		void Try_Save_Component(Json::Value& _root, entt::entity _id)
		{
			T* _component = m_ecs.GetReg().try_get<T>(_id);

			if (_component)
				Reflect_Serialization(_root, _component);
		}
	};


}



#endif