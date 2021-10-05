#pragma once
#include <unordered_map>
#include "ECS/entt_ecs/entt.hpp"
#include "ContentBrowser.h"

class ECS;
class Entity;

struct ImVec2;

namespace Pogplant
{
	class Camera;
}

namespace rttr
{
	class instance;
}

namespace PogplantDriver
{
	typedef unsigned int ImGuiID;


	struct ImguiHelper
	{
		static bool InitImgui(ECS* ecs);
		static void DrawImgui();
		static void DrawDock();
		static void CleanUpImgui();

		// Window handles
		static void SceneWindow();
		static void GameWindow();

		static bool m_FirstRun;
		//static int m_CurrentGOIdx;

		//ecs stuffs
		static void DrawEntityNode(Entity entity);
		static entt::entity m_CurrentEntity;
		static ECS* m_ecs;
		static int m_CurrentGOIdx;

		//Scene saving
		static void SaveSceneAs();
		static void NewScene();
		static void OpenScene();
		static void SavePrefab(entt::entity _object);
		static void LoadPrefab();
		
		static Directory m_Directory;
	private:
		//Recopy the Main-menu bar but change the ID stack
		static bool ImGui_BeginMainStatusBar();
		static void ImGui_EndMainStatusBar();
		static void Scene_GOPick(Pogplant::Camera* _CurrCam, ImVec2 _VMin, ImVec2 _VMax);
		static void Scene_GOEdit(Pogplant::Camera* _CurrCam, ImVec2 _VMin, ImVec2 _VMax);
		static void Scene_ViewEdit(Pogplant::Camera* _CurrCam, ImVec2 _VMin, ImVec2 _VMax);

		static bool DrawEntityNode(entt::entity entity, bool draw_childen = false);
		//Helper for opening Scene
		static void OpenScene(const std::filesystem::path& path);

		//simple helper imgui function
		//less copy pasta lol.
		static std::string CreateStringInputField(std::string& _label, std::string _target);
		static void CreateDragFloat3(std::string& _label, float* _value, float increment_speed = 1.0f, float min_val = 0.0f, float max_val = 0.f);
		//imgui reflection functions
		static void Reflect_ImGui(rttr::instance _obj);


	};

}

namespace PPD = PogplantDriver;
