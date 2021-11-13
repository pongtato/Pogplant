#include "PP_Entry.h"
#include "MeshBuilder.h"
#include "Window.h"
#include "TextureResource.h"
#include "FontResource.h"
#include "ShaderLinker.h"
#include "FrameBuffer.h"
#include "CameraResource.h"

namespace Pogplant
{
	void Entry::Init()
	{
		Window::InitWindow
		(
			1920,					// Width
			1080,					// Height
			"BogosBinted"			// Window name
		);

		MeshBuilder::InitMesh();
		TextureResource::InitResource();
		FontResource::InitResource();
		ShaderLinker::InitShader();
		FrameBuffer::InitFrameBuffer();
		CameraResource::InitBaseCameras
		(
			glm::vec3{ 0,0,5.0f }, // Editor cam pos
			CameraConfig
			{
				-90.0f, // Yaw
				0.0f,	// Pitch
				45.0f,	// Zoom 
				16.9f,	// Speed 
				0.1f,	// Near
				2000.0f,// Far
				0.21f,	// Mouse look sens
				20.0f,	// Key input look sens
				0.1f	// Pan speed
			}
		);

		// Ghetto init
		auto& quatCam = CameraResource::m_QuatCam;
		quatCam.m_Position = glm::vec3{ 0,0,5.0f };
		quatCam.m_Near = 0.1f;
		quatCam.m_Far = 2000.0f;
		quatCam.m_Fov = 45.0f;
	}

	void Entry::Cleanup()
	{
		CameraResource::CleanUpCameras();
		FrameBuffer::CleanUpFrameBuffer();
		FontResource::CleanUpResource();
		MeshBuilder::CleanUpMesh();
		Window::CleanUpWindow();
	}
}
