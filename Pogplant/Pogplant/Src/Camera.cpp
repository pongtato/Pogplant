#include "Camera.h"
#include "Window.h"
#include "Logger.h"

#include <gtc/matrix_transform.hpp>
#include <glfw3.h>

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
		: m_CameraConfig{}
		, m_Ortho{ glm::mat4{1} }
		, m_Perspective{ glm::mat4{1} }
		, m_View{ glm::mat4{1} }
		, m_Position{ glm::vec3{0} }
		, m_Front{ glm::vec3{0} }
		, m_Up{ glm::vec3{0,1,0} }
		, m_Right{ glm::vec3{1,0,0} }
		, m_Target{ glm::vec3{0,0,-1} }
		, m_LastXPos{ 0 }
		, m_LastYPos{ 0 }
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

	void Camera::Update(float _Dt)
	{
		KeyUpdate(_Dt);
		MouseUpdate(_Dt);
	}

	void Camera::UpdateView()
	{
		UpdateView(m_Position, m_Position + m_Front, m_Up);
	}

	void Camera::UpdateView(const glm::vec3& _Eye, const glm::vec3& _Center, const glm::vec3& _Up)
	{
		m_View = glm::lookAt(_Eye, _Center, _Up);
	}

	void Camera::UpdateProjection()
	{
		UpdateProjection(glm::vec2{ static_cast<float>(Window::m_Width),static_cast<float>(Window::m_Height) });
	}

	void Camera::UpdateProjection(const glm::vec2& _WindowSize)
	{
		// Updates both ortho & perspective
		m_Ortho = glm::ortho(0.0f, _WindowSize.x, 0.0f, _WindowSize.y, m_CameraConfig.m_Near, m_CameraConfig.m_Far);

		if (_WindowSize.y != 0)
		{
			const float aspectR = _WindowSize.x / _WindowSize.y;
			m_Perspective = glm::perspective(glm::radians(m_CameraConfig.m_Zoom), aspectR, m_CameraConfig.m_Near, m_CameraConfig.m_Far);
		}
		else
		{
			const float aspectR = 1.0f;
			m_Perspective = glm::perspective(glm::radians(m_CameraConfig.m_Zoom), aspectR, m_CameraConfig.m_Near, m_CameraConfig.m_Far);
		}
	}

	void Camera::UpdateVec()
	{
		m_Front.x = cos(glm::radians(m_CameraConfig.m_Yaw)) * cos(glm::radians(m_CameraConfig.m_Pitch));
		m_Front.y = sin(glm::radians(m_CameraConfig.m_Pitch));
		m_Front.z = sin(glm::radians(m_CameraConfig.m_Yaw)) * cos(glm::radians(m_CameraConfig.m_Pitch));
		m_Front = glm::normalize(m_Front);
		m_Right = glm::cross(m_Front, glm::vec3(0, 1, 0));
		m_Right = glm::normalize(m_Right);
		m_Up = glm::cross(m_Right, m_Front);
	}

	void Camera::UpdateZoom(double _ScrollAmount)
	{
		/*m_CameraConfig.m_Zoom = (m_CameraConfig.m_Zoom - static_cast<float>(_ScrollAmount) ) > 0.0f 
			? m_CameraConfig.m_Zoom - static_cast<float>(_ScrollAmount) : m_CameraConfig.m_Zoom;
		UpdateProjection();*/

		// Just move the camera closer 4Head 
		m_Position += m_Front * static_cast<float>(_ScrollAmount);
		UpdateView();
	}

	void Camera::UpdateYawPitch(double _XPos, double _YPos)
	{
		// Right mouse to trigger
		if (glfwGetMouseButton(Window::GetWindow(), GLFW_MOUSE_BUTTON_RIGHT) == GLFW_PRESS)
		{
			// Prevent mashing of keys
			if (m_MMBDown)
			{
				return;
			}

			if (!m_RMBDown)
			{
				m_LastXPos = _XPos;
				m_LastYPos = _YPos;
				m_RMBDown = true;
			}

			float xDifference = static_cast<float>(_XPos - m_LastXPos);
			float yDifference = static_cast<float>(_YPos - m_LastYPos);
			m_LastXPos = _XPos;
			m_LastYPos = _YPos;

			m_CameraConfig.m_Yaw += xDifference * m_CameraConfig.m_LookSens;
			m_CameraConfig.m_Pitch -= yDifference * m_CameraConfig.m_LookSens;

			// Clamp to prevent gimbal lock
			m_CameraConfig.m_Pitch = m_CameraConfig.m_Pitch > 89.0f ? 89.0f : m_CameraConfig.m_Pitch;
			m_CameraConfig.m_Pitch = m_CameraConfig.m_Pitch < -89.0f ? -89.0f : m_CameraConfig.m_Pitch;

			UpdateVec();
			UpdateView();
		}

		if (glfwGetMouseButton(Window::GetWindow(), GLFW_MOUSE_BUTTON_RIGHT) == GLFW_RELEASE)
		{
			m_RMBDown = false;
		}
	}

	void Camera::UpdateCameraPan(double _XPos, double _YPos)
	{
		// Right mouse to trigger
		if (glfwGetMouseButton(Window::GetWindow(), GLFW_MOUSE_BUTTON_MIDDLE) == GLFW_PRESS)
		{
			// Prevent mashing of keys
			if (m_RMBDown)
			{
				return;
			}

			if (!m_MMBDown)
			{
				m_LastXPos = _XPos;
				m_LastYPos = _YPos;
				m_MMBDown = true;
			}

			float xDifference = static_cast<float>(_XPos - m_LastXPos);
			float yDifference = static_cast<float>(_YPos - m_LastYPos);
			m_LastXPos = _XPos;
			m_LastYPos = _YPos;

			m_Position -= xDifference * m_CameraConfig.m_PanSens * m_Right;
			m_Position += yDifference * m_CameraConfig.m_PanSens * m_Up;

			UpdateView();
		}

		if (glfwGetMouseButton(Window::GetWindow(), GLFW_MOUSE_BUTTON_MIDDLE) == GLFW_RELEASE)
		{
			m_MMBDown = false;
		}
	}

	const glm::mat4& Camera::GetOrtho() const
	{
		return m_Ortho;
	}

	const glm::mat4& Camera::GetPerspective() const
	{
		return m_Perspective;
	}

	const glm::mat4& Camera::GetView() const
	{
		return m_View;
	}

	const CameraConfig& Camera::mCameraConfig() const
	{
		return m_CameraConfig;
	}

	void Camera::KeyUpdate(float _Dt)
	{
		// Forward
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_W) == GLFW_PRESS)
		{
			m_Position += m_CameraConfig.m_Speed * _Dt * m_Front;
			UpdateView();
		}

		// Back
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_S) == GLFW_PRESS)
		{
			m_Position -= m_CameraConfig.m_Speed * _Dt * m_Front;
			UpdateView();
		}

		// Left
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_A) == GLFW_PRESS)
		{
			m_Position -= m_CameraConfig.m_Speed * _Dt * m_Right;
			UpdateView();
		}

		// Right
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_D) == GLFW_PRESS)
		{
			m_Position += m_CameraConfig.m_Speed * _Dt * m_Right;
			UpdateView();
		}

		// Up
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_E) == GLFW_PRESS)
		{
			m_Position += m_CameraConfig.m_Speed * _Dt * m_Up;
			UpdateView();
		}

		// Down
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_Q) == GLFW_PRESS)
		{
			m_Position -= m_CameraConfig.m_Speed * _Dt * m_Up;
			UpdateView();
		}

		// Tilt left
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_J) == GLFW_PRESS)
		{
			m_CameraConfig.m_Yaw -= m_CameraConfig.m_KeyLookSens * _Dt;
			UpdateVec();
			UpdateView();
		}

		// Tilt Right
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_L) == GLFW_PRESS)
		{
			m_CameraConfig.m_Yaw += m_CameraConfig.m_KeyLookSens * _Dt;
			UpdateVec();
			UpdateView();
		}

		// Tilt Up
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_I) == GLFW_PRESS)
		{
			const float nextVal = m_CameraConfig.m_Pitch + m_CameraConfig.m_KeyLookSens * _Dt;
			m_CameraConfig.m_Pitch = nextVal < 89.0f ? nextVal : m_CameraConfig.m_Pitch;
			UpdateVec();
			UpdateView();
		}

		// Tilt Down
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_K) == GLFW_PRESS)
		{
			const float nextVal = m_CameraConfig.m_Pitch - m_CameraConfig.m_KeyLookSens * _Dt;
			m_CameraConfig.m_Pitch = nextVal > -89.0f ? nextVal : m_CameraConfig.m_Pitch;
			UpdateVec();
			UpdateView();
		}

		// Reset rotation
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_R) == GLFW_PRESS)
		{
			m_CameraConfig.m_Yaw = -90.0f;
			m_CameraConfig.m_Pitch = 0.0f;
			UpdateVec();
			UpdateView();
		}
	}

	void Camera::MouseUpdate(float _Dt)
	{
		(void)_Dt;
	}
}