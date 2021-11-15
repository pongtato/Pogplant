#include "Camera4D.h"
#include "Window.h"

#include <gtc/matrix_transform.hpp>
#include <gtc/quaternion.hpp>
#include <gtx/quaternion.hpp>
#include <glfw3.h>

#include <iostream>

namespace Pogplant
{
	Camera4D::Camera4D()
		: m_Orientation{ glm::quat{0,0,0,-1} }
		, m_Projection{ glm::mat4{1} }
		, m_Position{ glm::vec3{0,0,0} }
		, m_Front{ glm::vec3{0,0,-1} }
		, m_Right{ glm::vec3{1,0,0} }
		, m_Up{ glm::vec3{0,1,0} }
		, m_Pitch{ 0.0f }
		, m_Yaw{ 0.0f }
		, m_Speed{ 16.9f }
		, m_LookSpeed{ 0.21f }
		, m_Fov{ 45.0f }
		, m_Near{ 0.1f }
		, m_Far{ 420.0f }
		, m_Flip{ 1.0f }
		, m_RMB{ false }
	{
		UpdateVectors();
		UpdateProjection();
	}

	Camera4D::Camera4D(CameraConfig _CamConfig)
		: m_Orientation{ glm::quat{0,0,0,-1} }
		, m_Projection{ glm::mat4{1} }
		, m_Position{ _CamConfig.m_Position }
		, m_Front{ glm::vec3{0,0,-1} }
		, m_Right{ glm::vec3{1,0,0} }
		, m_Up{ glm::vec3{0,1,0} }
		, m_Pitch{ _CamConfig.m_Pitch }
		, m_Yaw{ _CamConfig.m_Yaw }
		, m_Speed{ _CamConfig.m_Speed }
		, m_LookSpeed{ _CamConfig.m_LookSens }
		, m_Fov{ _CamConfig.m_Fov }
		, m_Near{ _CamConfig.m_Near }
		, m_Far{ _CamConfig.m_Far }
		, m_Flip{ 1.0f }
		, m_RMB{ false }
	{
		UpdateVectors();
		UpdateProjection();
	}

	Camera4D::~Camera4D()
	{
	}

	void Camera4D::Update(float _Dt)
	{
		auto quatFront = m_Orientation * glm::quat(0, 0, 0, -1) * glm::conjugate(m_Orientation);
		m_Front = glm::vec3{ quatFront.x, quatFront.y, quatFront.z };
		if (m_Front.length() > 0)
		{
			m_Front = glm::normalize(m_Front);
		}
		m_Right = glm::cross(m_Front, m_Up);
		if (m_Right.length() > 0)
		{
			m_Right = glm::normalize(m_Right);
		}

		KeyInput(_Dt);
		YawPitchFlip();
	}

	void Camera4D::UpdateVectors()
	{
		glm::quat yRotate = glm::angleAxis(glm::radians(m_Yaw), glm::vec3{ 0,1,0 });
		glm::quat xRotate = glm::angleAxis(glm::radians(m_Pitch), glm::vec3{ 1,0,0 });
		m_Orientation = yRotate * xRotate;
	}

	void Camera4D::UpdateProjection()
	{
		UpdateProjection(glm::vec2{ static_cast<float>(Window::m_Width),static_cast<float>(Window::m_Height) });
	}

	void Camera4D::UpdateProjection(glm::vec2 _WindowSize)
	{
		if (_WindowSize.y > 0.0f)
		{
			const float aspectR = _WindowSize.x / _WindowSize.y;
			m_Projection = glm::perspective(glm::radians(m_Fov), aspectR, m_Near, m_Far);
		}
		else
		{
			const float aspectR = 1.0f;
			m_Projection = glm::perspective(glm::radians(m_Fov), aspectR, m_Near, m_Far);
		}
	}

	void Camera4D::KeyInput(float _Dt)
	{
		// Forward
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_W) == GLFW_PRESS)
		{
			m_Position += m_Front * m_Speed * _Dt;
		}

		// Back
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_S) == GLFW_PRESS)
		{
			m_Position -= m_Front * m_Speed * _Dt;
		}

		// Left
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_A) == GLFW_PRESS)
		{
			m_Position -= m_Right * m_Speed * _Dt * m_Flip;
		}

		// Right
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_D) == GLFW_PRESS)
		{
			m_Position += m_Right * m_Speed * _Dt * m_Flip;
		}

		// Up
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_E) == GLFW_PRESS)
		{
			m_Position += m_Up * m_Speed * _Dt;
		}

		// Down
		if (glfwGetKey(Window::GetWindow(), GLFW_KEY_Q) == GLFW_PRESS)
		{
			m_Position -= m_Up * m_Speed * _Dt;
		}
	}

	void Camera4D::MouseInput(float _X, float _Y)
	{
		// Right mouse to trigger
		if (glfwGetMouseButton(Window::GetWindow(), GLFW_MOUSE_BUTTON_RIGHT) == GLFW_PRESS)
		{
			if (!m_RMB)
			{
				m_MousePosition = { _X,_Y };
				m_RMB = true;
			}

			glm::vec2 diff = m_MousePosition - glm::vec2{ _X,_Y };
			m_MousePosition = { _X,_Y };

			m_Yaw += diff.x * m_LookSpeed;
			m_Pitch += diff.y * m_LookSpeed;

			UpdateVectors();
		}

		if (glfwGetMouseButton(Window::GetWindow(), GLFW_MOUSE_BUTTON_RIGHT) == GLFW_RELEASE)
		{
			m_RMB = false;
		}
	}

	glm::mat4 Camera4D::GetView() const
	{
		glm::quat reverseOrient = glm::conjugate(m_Orientation);
		//::mat4 rot = glm::mat4_cast(reverseOrient);
		//glm::mat4 translate = glm::translate(glm::mat4(1.0), -m_Position);

		glm::mat4 rot = glm::mat4_cast(reverseOrient);
		rot[3][0] = -(rot[0][0] * m_Position.x + rot[1][0] * m_Position.y + rot[2][0] * m_Position.z);
		rot[3][1] = -(rot[0][1] * m_Position.x + rot[1][1] * m_Position.y + rot[2][1] * m_Position.z);
		rot[3][2] = -(rot[0][2] * m_Position.x + rot[1][2] * m_Position.y + rot[2][2] * m_Position.z);
		rot[3][3] = 1;


		return rot;
	}

	void Camera4D::UpdateRayConfig(RayConfig _Rayconfig)
	{
		m_Ray = { _Rayconfig,m_Position,glm::vec3{0} };
	}

	void Camera4D::RayCast()
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
		glm::vec4 eye = glm::inverse(m_Projection) * clip;
		// Unproject XY portion only
		eye = { eye.x,eye.y,-1.0f,0.0f };
		// World space from view space
		const glm::vec4 world = glm::inverse(GetView()) * eye;
		// Normalise the ray 
		m_Ray.m_Direction = glm::normalize(glm::vec3(world));
	}

	void Camera4D::UpdateFront(float* _Front)
	{
		m_Front = { _Front[0],_Front[1],_Front[2] };
		m_Yaw = glm::degrees(atan2f(m_Front.x, m_Front.z));
		m_Pitch = glm::degrees(-asinf(m_Front.y));
		UpdateVectors();
	}

	void Camera4D::UpdateZoom(double _ScrollAmount)
	{
		m_Position += m_Front * static_cast<float>(_ScrollAmount);
	}

	void Camera4D::UpdateVectors(float _Yaw, float _Pitch, glm::vec3& _Front, glm::vec3& _Right, glm::vec3& _Up, glm::quat& _Orientation)
	{
		glm::quat yRotate = glm::angleAxis(glm::radians(_Yaw), glm::vec3{ 0,1,0 });
		glm::quat xRotate = glm::angleAxis(glm::radians(_Pitch), glm::vec3{ 1,0,0 });
		_Orientation = yRotate * xRotate;

		auto quatFront = _Orientation * glm::quat(0, 0, 0, -1) * glm::conjugate(_Orientation);
		_Front = glm::vec3{ quatFront.x, quatFront.y, quatFront.z };
		if (_Front.length() > 0)
		{
			_Front = glm::normalize(_Front);
		}
		_Right = glm::cross(_Front, glm::vec3{ 0,1,0 });
		if (_Right.length() > 0)
		{
			_Right = glm::normalize(_Right);
		}
		_Up = glm::cross(_Front, _Right);
	}

	void Camera4D::UpdateProjection(glm::vec2 _WindowSize, float _Near, float _Far, float _Fov, glm::mat4& _Projection)
	{
		if (_WindowSize.y > 0.0f)
		{
			const float aspectR = _WindowSize.x / _WindowSize.y;
			_Projection = glm::perspective(glm::radians(_Fov), aspectR, _Near, _Far);
		}
		else
		{
			const float aspectR = 1.0f;
			_Projection = glm::perspective(glm::radians(_Fov), aspectR, _Near, _Far);
		}
	}

	void Camera4D::GetView(const glm::vec3& _Position, const glm::quat& _Orientation, glm::mat4& _View)
	{
		glm::quat reverseOrient = glm::conjugate(_Orientation);
		_View = glm::mat4_cast(reverseOrient);
		_View[3][0] = -(_View[0][0] * _Position.x + _View[1][0] * _Position.y + _View[2][0] * _Position.z);
		_View[3][1] = -(_View[0][1] * _Position.x + _View[1][1] * _Position.y + _View[2][1] * _Position.z);
		_View[3][2] = -(_View[0][2] * _Position.x + _View[1][2] * _Position.y + _View[2][2] * _Position.z);
		_View[3][3] = 1;
	}

	void Camera4D::YawPitchFlip()
	{
		//std::cout << m_Pitch << "|" << m_Yaw << std::endl;

		if (m_Pitch < 0)
		{
			m_Pitch += 360.0f;
		}
		else if(m_Pitch > 360.0f)
		{
			m_Pitch -= 360.0f;
		}

		if (m_Pitch >= 90.0f && m_Pitch <= 270.0f)
		{
			m_Flip = -1.0f;
		}
		else
		{
			m_Flip = 1.0f;
		}
	}

}
