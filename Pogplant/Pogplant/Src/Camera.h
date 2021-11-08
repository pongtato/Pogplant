#pragma once
#include "Ray.h"

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
		static void GetUpdatedView(const glm::vec3& _Eye, const glm::vec3& _Center, const glm::vec3& _Up, glm::mat4& _View);
		void UpdateProjection();
		void UpdateProjection(const glm::vec2& _WindowSize);
		static void GetUpdatedProjection(const glm::vec2& _WindowSize, float _Zoom, float _Near, float _Far, glm::mat4& _Projection);
		void UpdateVec();
		static void GetUpdatedVec(float _Yaw, float _Pitch, glm::vec3& _Up, glm::vec3& _Right, glm::vec3& _Front);
		void UpdateZoom(double _ScrollAmount);
		void UpdateYawPitch(double _XPos, double _YPos);
		void UpdateCameraPan(double _XPos, double _YPos);
		void UpdateFront(float* _Front);
		void UpdateRayConfig(RayConfig _Rayconfig);

		const CameraConfig& GetCameraConfig() const;
		const glm::mat4& GetOrtho() const;
		const glm::mat4& GetPerspective() const;
		const glm::mat4& GetView() const;

		glm::mat4& Perspective();
		glm::mat4& View();

		const Ray& GetRay() const;
		void RayCast();

		const glm::vec3& GetPosition() const;

		glm::vec3 m_Position;
		glm::vec3 m_Front;
		glm::vec3 m_Up;
		glm::vec3 m_Right;
		CameraConfig m_CameraConfig;

	private:

		void KeyUpdate(float _Dt);
		void MouseUpdate(float _Dt);
		void LerpUpdate(float _Dt);

		CameraLerp m_CameraLerp;
		Ray m_Ray;
		glm::mat4 m_Ortho;
		glm::mat4 m_Perspective;
		glm::mat4 m_View;
		glm::vec3 m_Target;
		double m_ScrollAmount;
		double m_LastXPos;
		double m_LastYPos;
		bool m_RMBDown; // Camera look
		bool m_MMBDown; // Pan
	};
}

