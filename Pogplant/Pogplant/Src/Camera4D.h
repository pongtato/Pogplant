#pragma once
#include "Ray.h"

#include <glm.hpp>
#include <gtc/quaternion.hpp>

namespace Pogplant
{
	struct CameraConfig
	{
		glm::vec3 m_Position;
		float m_Yaw;
		float m_Pitch;
		float m_Fov;
		float m_Speed;
		float m_Near;
		float m_Far;
		float m_LookSens;
		float m_PanSens;
	};

	class Camera4D
	{
	public:
		Camera4D();
		Camera4D(CameraConfig _CamConfig);
		~Camera4D();
		void Update(float _Dt);
		void UpdateVectors();
		void UpdateProjection();
		void UpdateProjection(glm::vec2 _WindowSize);
		void KeyInput(float _Dt);
		void MouseInput(float _X, float _Y);
		glm::mat4 GetView() const;

		void UpdateRayConfig(RayConfig _Rayconfig);
		void RayCast();

		void UpdateFront(float* _Front);
		void UpdateZoom(double _ScrollAmount);

		// To update componentss
		static void UpdateVectors(float _Yaw, float _Pitch, glm::vec3& _Front, glm::vec3& _Right, glm::vec3& _Up, glm::quat& _Orientation);
		static void UpdateProjection(glm::vec2 _WindowSize, float _Near, float _Far, float _Fov, glm::mat4& _Projection);
		static void GetView(const glm::vec3& _Position,const glm::quat& _Orientation, glm::mat4& _View);

		glm::quat m_Orientation;
		glm::mat4 m_Projection;
		glm::vec3 m_Position;
		glm::vec3 m_Front;
		glm::vec3 m_Right;
		glm::vec3 m_Up;
		glm::vec2 m_MousePosition;

		float m_Pitch;
		float m_Yaw;
		float m_Speed;
		float m_LookSpeed;
		float m_Fov;
		float m_Near;
		float m_Far;
		float m_Flip;

		bool m_RMB;

		Ray m_Ray;
	private:
		void YawPitchFlip();
	};
}