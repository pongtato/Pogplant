#include "Camera.h"
#include "Window.h"

#include <gtc/matrix_transform.hpp>

namespace Pogplant
{
	//CameraConfig::CameraConfig()
	//	: m_Zoom{ 0 }
	//	, m_Speed{ 0 }
	//	, m_Yaw{ 0 }
	//	, m_Pitch{ 0 }
	//	, m_Near{ 0 }
	//	, m_Far{ 0 }
	//{
	//}

	Camera::Camera()
		: m_Projection{ glm::mat4{1} }
		, m_View{ glm::mat4{1} }
		, m_Position{ glm::vec3{0} }
		, m_Front{ glm::vec3{0} }
		, m_Up{ glm::vec3{0,1,0} }
		, m_Right{ glm::vec3{1,0,0} }
		, m_Target{ glm::vec3{0,0,-1} }
		, m_CameraConfig {}
	{
	}

	void Camera::InitCamera(const glm::vec3& _Position, CameraConfig _CameraConfig)
	{
		m_Position = _Position;
		m_CameraConfig = _CameraConfig;

		UpdateVec();
		UpdateView();
		UpdateProjection();
	}

	void Camera::Update()
	{
	}

	void Camera::UpdateView()
	{
		m_View = glm::lookAt(m_Position, m_Position + m_Front, m_Up);
	}

	void Camera::UpdateProjection()
	{
		if (Window::m_Height != 0)
		{
			const float aspectR = static_cast<float>(Window::m_Width) / Window::m_Height;
			m_Projection = glm::perspective(glm::radians(m_CameraConfig.m_Zoom), aspectR, m_CameraConfig.m_Near, m_CameraConfig.m_Far);
		}
		else
		{
			const float aspectR = 1.0f;
			m_Projection = glm::perspective(glm::radians(m_CameraConfig.m_Zoom), aspectR, m_CameraConfig.m_Near, m_CameraConfig.m_Far);
		}
	}

	void Camera::UpdateVec()
	{
		m_Front.x = cos(glm::radians(m_CameraConfig.m_Yaw)) * cos(glm::radians(m_CameraConfig.m_Pitch));
		m_Front.y = sin(glm::radians(m_CameraConfig.m_Pitch));
		m_Front.z = sin(glm::radians(m_CameraConfig.m_Yaw)) * cos(glm::radians(m_CameraConfig.m_Pitch));
		m_Front = glm::normalize(m_Front);
		m_Right = glm::cross(m_Front, glm::vec3(0, 1, 0));
		m_Up = glm::cross(m_Right, m_Front);
	}
	const glm::mat4& Camera::GetProjection() const
	{
		return m_Projection;
	}
	const glm::mat4& Camera::GetView() const
	{
		return m_View;
	}
}