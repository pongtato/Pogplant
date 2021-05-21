#pragma once
#include "Camera.h"

#include <vector>
#include <unordered_map>
#include <glm.hpp>

namespace Pogplant
{
	class CameraResource
	{
	public:
		static void InitBaseCameras(const glm::vec3& _EditorPosition, const glm::vec3& _GamePosition, CameraConfig _CameraConfig);
		static void AddCamera(const char* _ID);
		static void RemoveCamera(size_t _Index);
		static Camera* GetCamera(const char* _ID);
		static Camera* GetCamera(size_t _Index);
		static void SetActiveCam(const char* _ID);
		static void SetActiveCam(size_t _Index);
		static void DeselectCam();
		static Camera* GetActiveCam();
		static void CleanUpCameras();
		static void UpdateAllProjection();
		static void UpdateActiveCamera(float _Dt);
		static void ImGUIWindowSizeOverride(const glm::vec2& _ImGuiWinSize);

	private:
		static std::unordered_map<const char*, size_t> m_CameraKeys;
		static std::vector<Camera*> m_CameraPool;
		static Camera* m_ActiveCamera;
	};
}