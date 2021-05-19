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
		float m_LookSens;
		float m_KeyLookSens;
		float m_PanSens;
	};

	class Camera
	{
	public:
		Camera();
		
		void InitCamera(const glm::vec3& _Position, CameraConfig _CameraConfig);
		void Update(float _Dt);
		void UpdateView();
		void UpdateProjection();
		void UpdateVec();
		void UpdateZoom(double _ScrollAmount);
		void UpdateYawPitch(double _XPos, double _YPos);
		void UpdateCameraPan(double _XPos, double _YPos);

		const glm::mat4& GetProjection() const;
		const glm::mat4& GetView() const;

	private:

		void KeyUpdate(float _Dt);
		void MouseUpdate(float _Dt);

		CameraConfig m_CameraConfig;
		glm::mat4 m_Projection;
		glm::mat4 m_View;
		glm::vec3 m_Position;
		glm::vec3 m_Front;
		glm::vec3 m_Up;
		glm::vec3 m_Right;
		glm::vec3 m_Target;
		double m_ScrollAmount;
		double m_LastXPos;
		double m_LastYPos;
		bool m_RMBDown; // Camera look
		bool m_MMBDown; //Pan
	};
}

