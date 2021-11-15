#include "CameraResource.h"

namespace Pogplant
{
	std::unordered_map<const char*, size_t> CameraResource::m_CameraKeys;
	//std::vector<Camera*> CameraResource::m_CameraPool;
	//Camera* CameraResource::m_ActiveCamera = nullptr;
	//Camera4D CameraResource::m_QuatCam = Camera4D();
	std::vector<Camera4D*> CameraResource::m_CameraPool;
	Camera4D* CameraResource::m_ActiveCamera = nullptr;

	void CameraResource::InitBaseCameras(const CameraConfig& _CameraConfig)
	{
		// Editor
		AddCamera("EDITOR", _CameraConfig);
		SetActiveCam("EDITOR");
		m_ActiveCamera->UpdateProjection();
		m_ActiveCamera->UpdateVectors();
	}

	void CameraResource::AddCamera(const char* _ID, const CameraConfig& _CameraConfig)
	{
		m_CameraKeys[_ID] = m_CameraPool.size();
		m_CameraPool.push_back(new Camera4D(_CameraConfig));
	}

	void CameraResource::RemoveCamera(size_t _Index)
	{
		m_CameraPool.erase(m_CameraPool.begin() + _Index);
	}

	Camera4D* CameraResource::GetCamera(const char* _ID)
	{
		return m_CameraPool[m_CameraKeys[_ID]];
	}

	Camera4D* CameraResource::GetCamera(size_t _Index)
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

	Camera4D* CameraResource::GetActiveCam()
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

		/// QUAT
		//CameraResource::m_QuatCam.UpdateProjection();
	}
	void CameraResource::UpdateActiveCamera(float _Dt)
	{
		if (m_ActiveCamera)
		{
			m_ActiveCamera->Update(_Dt);
		}

		/// QUAT
		//CameraResource::m_QuatCam.Update(_Dt);
	}

	void CameraResource::ImGUIWindowSizeOverride(const glm::vec2& _ImGuiWinSize)
	{
		if (m_ActiveCamera)
		{
			m_ActiveCamera->UpdateProjection(_ImGuiWinSize);
		}

		/// QUAT
		//CameraResource::m_QuatCam.UpdateProjection(_ImGuiWinSize);
	}
}