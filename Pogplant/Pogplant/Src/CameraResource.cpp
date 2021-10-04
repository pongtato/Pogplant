#include "CameraResource.h"

namespace Pogplant
{
	std::unordered_map<const char*, size_t> CameraResource::m_CameraKeys;
	std::vector<Camera*> CameraResource::m_CameraPool;
	Camera* CameraResource::m_ActiveCamera = nullptr;

	void CameraResource::InitBaseCameras(const glm::vec3& _EditorPosition, CameraConfig _CameraConfig)
	{
		// Editor
		AddCamera("EDITOR");
		SetActiveCam("EDITOR");
		m_ActiveCamera->InitCamera(_EditorPosition, _CameraConfig);
		m_ActiveCamera->UpdateProjection();
		m_ActiveCamera->UpdateView();

		// Back to editor since game should be offloaded to application camera instead of Pogplant camera
		SetActiveCam("EDITOR");
	}

	void CameraResource::AddCamera(const char* _ID)
	{
		m_CameraKeys[_ID] = m_CameraPool.size();
		m_CameraPool.push_back(new Camera());
	}

	void CameraResource::RemoveCamera(size_t _Index)
	{
		m_CameraPool.erase(m_CameraPool.begin() + _Index);
	}

	Camera* CameraResource::GetCamera(const char* _ID)
	{
		return m_CameraPool[m_CameraKeys[_ID]];
	}

	Camera* CameraResource::GetCamera(size_t _Index)
	{
		return m_CameraPool[_Index];
	}

	void CameraResource::SetActiveCam(const char* _ID)
	{
		m_ActiveCamera = m_CameraPool[m_CameraKeys[_ID]];
	}

	void CameraResource::SetActiveCam(size_t _Index)
	{
		m_ActiveCamera = m_CameraPool[_Index];
	}

	void CameraResource::DeselectCam()
	{
		m_ActiveCamera = nullptr;
	}

	Camera* CameraResource::GetActiveCam()
	{
		return m_ActiveCamera;
	}

	void CameraResource::CleanUpCameras()
	{
		for (auto& it : m_CameraPool)
		{
			delete it;
		}
	}

	void CameraResource::UpdateAllProjection()
	{
		for (auto& it : m_CameraPool)
		{
			it->UpdateProjection();
		}
	}
	void CameraResource::UpdateActiveCamera(float _Dt)
	{
		if (m_ActiveCamera)
		{
			m_ActiveCamera->Update(_Dt);
		}
	}

	void CameraResource::ImGUIWindowSizeOverride(const glm::vec2& _ImGuiWinSize)
	{
		if (m_ActiveCamera)
		{
			m_ActiveCamera->UpdateProjection(_ImGuiWinSize);
		}
	}
}