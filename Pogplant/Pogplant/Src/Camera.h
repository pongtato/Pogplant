#pragma once

#include <glm.hpp>

namespace Pogplant
{
	struct CameraConfig
	{
		//CameraConfig();

		float m_Yaw;
		float m_Pitch;
		float m_Zoom;
		float m_Speed;
		float m_Near;
		float m_Far;
	};

	class Camera
	{
	public:
		Camera();
		
		void InitCamera(const glm::vec3& _Position, CameraConfig _CameraConfig);
		void Update();
		void UpdateView();
		void UpdateProjection();
		void UpdateVec();

		const glm::mat4& GetProjection() const;
		const glm::mat4& GetView() const;

	private:
		glm::mat4 m_Projection;
		glm::mat4 m_View;
		glm::vec3 m_Position;
		glm::vec3 m_Front;
		glm::vec3 m_Up;
		glm::vec3 m_Right;
		glm::vec3 m_Target;

		CameraConfig m_CameraConfig;
	};
}

