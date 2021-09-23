#include "Camera.h"
#include "Window.h"
#include "Logger.h"
#include "3DInspector.h"

#include <gtc/matrix_transform.hpp>
#include <glfw3.h>

#include <iostream>

namespace Pogplant
{
	Camera::Camera()
		: m_CameraConfig{}
		, m_CameraLerp{}
		, m_Ortho{ glm::mat4{1} }
		, m_Perspective{ glm::mat4{1} }
		, m_View{ glm::mat4{1} }
		, m_Position{ glm::vec3{0} }
		, m_Front{ glm::vec3{0} }
		, m_Up{ glm::vec3{0,1,0} }
		, m_Right{ glm::vec3{1,0,0} }
		, m_Target{ glm::vec3{0,0,-1} }
		, m_LastXPos{ 0.0 }
		, m_LastYPos{ 0.0 }
	{
	}

	void Camera::InitCamera(const glm::vec3& _Position, CameraConfig _CameraConfig)
	{
		m_Position = _Position;
		m_CameraConfig = _CameraConfig;
		m_CameraLerp = { 6.9f,0,0,0,0,0,0 }; // Lerp speed = 6.9f, rest = 0.0f

		UpdateVec();
		UpdateView();
		UpdateProjection();
	}

	void Camera::Update(float _Dt)
	{
		KeyUpdate(_Dt);
		MouseUpdate(_Dt);
		LerpUpdate(_Dt);
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

	void Camera::UpdateFront(float* _Front)
	{
		// Cartesian to spherical
		m_Front = { _Front[0],_Front[1],_Front[2] };
		m_CameraConfig.m_Yaw = 180.0f + glm::degrees(atan2f(m_Front.z, m_Front.x));
		m_CameraConfig.m_Pitch = glm::degrees(-asinf(m_Front.y));
		UpdateVec();
	}

	void Camera::UpdateRayConfig(RayConfig _Rayconfig)
	{
		m_Ray = { _Rayconfig,m_Position,glm::vec3{0} };

		//std::cout << _Rayconfig.m_VP_Min.x << "|" << _Rayconfig.m_VP_Min.y << std::endl;
		//std::cout << _Rayconfig.m_VP_Max.x << "|" << _Rayconfig.m_VP_Max.y << std::endl;
		//std::cout << _Rayconfig.m_VP_Size.x << "|" << _Rayconfig.m_VP_Size.y << std::endl;
		//std::cout << _Rayconfig.m_CursorPos.x << "|" << _Rayconfig.m_CursorPos.y << std::endl;
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

	glm::mat4& Camera::Perspective()
	{
		return m_Perspective;
	}

	glm::mat4& Camera::View()
	{
		return m_View;
	}

	const Ray& Camera::GetRay() const
	{
		return m_Ray;
	}

	void Camera::RayCast()
	{
		// Mouse position with offset
		const RayConfig& rayCfg = m_Ray.m_RayConfig;
		glm::vec2 mPos = { rayCfg.m_CursorPos.x - rayCfg.m_VP_Min.x , rayCfg.m_VP_Max.y - (rayCfg.m_VP_Max.y - rayCfg.m_CursorPos.y) - rayCfg.m_VP_Min.y };

		// NDC
		glm::vec3 screenToNDC = glm::vec3{ 0 };
		screenToNDC.x = (2.0f * mPos.x) / rayCfg.m_VP_Size.x - 1.0f;
		screenToNDC.y = 1.0f - (2.0f * mPos.y) / rayCfg.m_VP_Size.y;
		screenToNDC.z = 1.0f;
		const glm::vec4 clip = { screenToNDC.x,screenToNDC.y,-1.0f,1.0f };
		// View space from clip space
		glm::vec4 eye = glm::inverse(m_Perspective) * clip;
		// Unproject XY portion only
		eye = { eye.x,eye.y,-1.0f,0.0f };
		// World space from view space
		const glm::vec4 world = glm::inverse(m_View) * eye;
		// Normalise the ray 
		m_Ray.m_Direction = glm::normalize(glm::vec3(world));

		//// Normal of the plane is to camera
		//const glm::vec3 planeNormal = { 0,0,1.0f };
		//// Find the time of intersection
		//const float t = -m_Position.z / (glm::dot(m_Ray.m_Direction, planeNormal));
		//// Use time to offset direction to get the position we want
		//const glm::vec3 worldPos = m_Position + m_Ray.m_Direction * t;

		//std::stringstream logEntry;
		//logEntry << worldPos.x << "|" << worldPos.y << "|" << worldPos.z;
		//Logger::Log({ "PP::CAMERA",LogEntry::DEBUG_TEXT, logEntry.str() });
	}

	const glm::vec3& Camera::GetPosition() const
	{
		return m_Position;
	}

	const CameraConfig& Camera::GetCameraConfig() const
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
			//m_CameraConfig.m_Yaw = -90.0f;
			//m_CameraConfig.m_Pitch = 0.0f;
			//UpdateVec();
			//UpdateView();

			if (!m_CameraLerp.m_Lerping)
			{
				m_CameraLerp.m_Lerping = true;
				// Convert to positive range because lerp in negative is just pepega
				m_CameraLerp.m_StartYaw = m_CameraConfig.m_Yaw + 180.0f;
				m_CameraLerp.m_StartPitch = m_CameraConfig.m_Pitch + 90.0f;
				m_CameraLerp.m_TargetYaw = 90.0f;
				m_CameraLerp.m_TargetPitch = 90.0f;
				m_CameraLerp.m_LerpTarget = m_CameraLerp.m_LerpTimer + 1.0f; // Lerp in 1 sec
			}
		}
	}

	void Camera::MouseUpdate(float _Dt)
	{
		(void)_Dt;
	}

	void Camera::LerpUpdate(float _Dt)
	{
		m_CameraLerp.m_LerpTimer += m_CameraLerp.m_LerpSpeed * _Dt;

		if (m_CameraLerp.m_LerpTimer < m_CameraLerp.m_LerpTarget)
		{
			// Scale reference
			float timeCalc = 1.0f - (m_CameraLerp.m_LerpTarget - m_CameraLerp.m_LerpTimer);

			// Convert back to negative range because lerp in negative is just pepega :clown:
			m_CameraConfig.m_Yaw = CameraLerp::Lerp(m_CameraLerp.m_StartYaw, m_CameraLerp.m_TargetYaw, timeCalc) - 180.0f;
			m_CameraConfig.m_Pitch = CameraLerp::Lerp(m_CameraLerp.m_StartPitch, m_CameraLerp.m_TargetPitch, timeCalc) - 90.0f;

			UpdateVec();
			UpdateView();
		}
		else if(m_CameraLerp.m_Lerping)
		{
			m_CameraLerp.m_Lerping = false;
		}
	}

	float CameraLerp::Lerp(float _A, float _B, float _T)
	{
		return _A + _T * (_B - _A);
	}
}