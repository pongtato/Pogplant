#pragma once

#include <glm.hpp>

namespace Pogplant
{
	struct CameraLerp
	{
		float m_LerpSpeed;
		float m_StartYaw;
		float m_StartPitch;
		float m_TargetYaw;
		float m_TargetPitch;
		float m_LerpTimer;
		float m_LerpTarget;
		bool m_Lerping;

		static float Lerp(float _A, float _B, float _T);
	};

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
		void UpdateView(const glm::vec3& _Eye, const glm::vec3& _Center, const glm::vec3& _Up);
		void UpdateProjection();
		void UpdateProjection(const glm::vec2& _WindowSize);
		void UpdateVec();
		void UpdateZoom(double _ScrollAmount);
		void UpdateYawPitch(double _XPos, double _YPos);
		void UpdateCameraPan(double _XPos, double _YPos);
		const CameraConfig& mCameraConfig() const;

		const glm::mat4& GetOrtho() const;
		const glm::mat4& GetPerspective() const;
		const glm::mat4& GetView() const;

	private:

		void KeyUpdate(float _Dt);
		void MouseUpdate(float _Dt);
		void LerpUpdate(float _Dt);

		CameraConfig m_CameraConfig;
		CameraLerp m_CameraLerp;
		glm::mat4 m_Ortho;
		glm::mat4 m_Perspective;
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
		bool m_MMBDown; // Pan
	};
}

