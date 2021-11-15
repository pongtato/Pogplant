#pragma once
//#include "Camera.h"
#include "Camera4D.h"

#include <vector>
#include <unordered_map>
#include <glm.hpp>

namespace Pogplant
{
	class CameraResource
	{
	public:
		static void InitBaseCameras(const CameraConfig& _CameraConfig);
		static void AddCamera(const char* _ID, const CameraConfig& _CameraConfig);
		static void RemoveCamera(size_t _Index);
		static Camera4D* GetCamera(const char* _ID);
		static Camera4D* GetCamera(size_t _Index);
		static void SetActiveCam(const char* _ID);
		static void SetActiveCam(size_t _Index);
		static void DeselectCam();
		static Camera4D* GetActiveCam();
		static void CleanUpCameras();
		static void UpdateAllProjection();
		static void UpdateActiveCamera(float _Dt);
		static void ImGUIWindowSizeOverride(const glm::vec2& _ImGuiWinSize);

		//static Camera4D m_QuatCam;

	private:
		static std::unordered_map<const char*, size_t> m_CameraKeys;
		//static std::vector<Camera*> m_CameraPool;
		//static Camera* m_ActiveCamera;
		static std::vector<Camera4D*> m_CameraPool;
		static Camera4D* m_ActiveCamera;
	};
}