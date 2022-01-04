#ifndef GENERICCOMPONENTS_H_
#define GENERICCOMPONENTS_H_

#include <rttr/registration>
#include <../IMGUI/ImGuiExtraStyles.h>
//#include <ImGuizmo.h>
#include <../IMGUIZMO/ImGuizmo.h>
#include <gtc/type_ptr.hpp>
#include <gtc/random.hpp>
#include "../../Pogplant/Src/Logger.h"

namespace Components
{
	struct Tag
	{
		Tag() = default;
		Tag(std::string tag) : m_tag(tag) {}
		std::string m_tag;
	};

	struct Name
	{
		Name() = default;
		Name(std::string name) : m_name(name), status(true) {}
		std::string m_name;
		bool status;
	};

	struct Disabled
	{
		//cheap method to disable an entity
		bool m_disabled;
	};

	struct PositionList
	{
		short m_index;
	};

	//Just a component to debug transforms
	struct TransformDebugger
	{
		float m_forwardVectorLength = 1.f;
		bool m_drawForwardVector = true;
	};

	struct Transform
	{
		glm::vec3 m_position;
		glm::vec3 m_rotation;
		glm::vec3 m_scale;

		glm::mat4 m_ModelMtx = glm::mat4{ 1 };// has been multiplied by parent matrix

		/**> Matrix to get local from world */
		glm::mat4 m_ModelMtxLocal = glm::mat4{ 1 };
		glm::mat4 m_localToWorld = glm::mat4{ 1 };

		entt::entity m_parent = entt::null;
		std::set<entt::entity> m_children;

		Transform(const Transform&) = default;
		Transform(glm::vec3 pos = { 0,0,0 }, glm::vec3 rot = { 0,0,0 }, glm::vec3 scale = { 0,0,0 }, entt::entity parent = entt::null)
			: m_position(pos), m_rotation(rot), m_scale(scale), m_parent{ parent }
		{
			init();
		}

		inline Transform& operator=(const Transform& rhs)
		{
			if (this != &rhs)
			{
				m_position = rhs.m_position;
				m_rotation = rhs.m_rotation;
				m_scale = rhs.m_scale;
				m_parent = rhs.m_parent;
				m_children = rhs.m_children;
			}

			return *this;
		}

		inline Transform& operator=(Transform&& rhs)
		{
			if (this != &rhs)
			{
				m_position = std::move(rhs.m_position);
				m_rotation = std::move(rhs.m_rotation);
				m_scale = std::move(rhs.m_scale);
				m_parent = std::move(rhs.m_parent);
				m_children = std::move(rhs.m_children);
			}

			return *this;
		}

		inline void init(void)
		{
			updateModelMtx();
		}

		inline void updateModelMtx(void)
		{
			//if (m_parent == entt::null)
			{
				ImGuizmo::RecomposeMatrixFromComponents(
					glm::value_ptr(m_position),
					glm::value_ptr(m_rotation),
					glm::value_ptr(m_scale),
					glm::value_ptr(m_ModelMtx));

				m_ModelMtxLocal = m_ModelMtx;

				computedLocal = false;
			}
		}

		inline void updateModelMtx(const Transform& _transform)
		{
			m_localToWorld = _transform.m_ModelMtx;

			updateModelMtx();
			m_ModelMtx = _transform.m_ModelMtx * m_ModelMtx;
		}

		inline void LookAt(const glm::vec3& target)
		{

			glm::vec3 directionalVector = target - GetGlobalPosition();

			float angleX = glm::atan(
				glm::sqrt(directionalVector.x * directionalVector.x + directionalVector.z * directionalVector.z),
				directionalVector.y);

			SetGlobalRotation(
				glm::vec3{
					glm::degrees(angleX) - 90.f,
					glm::degrees(glm::atan(directionalVector.z, -directionalVector.x)) - 90.f,
					0.f//Ignore roll for now
				}
			);
	

			// bandaid
			//float threshold = 1.0f;
			//float angle_from_up = glm::degrees(glm::acos(glm::dot(directionalVector, { 0,1,0 }) / glm::length(directionalVector)));
			//if (angle_from_up > threshold
			//	&& angle_from_up < 180.0f - threshold)
			//{
			//	float angleX = glm::atan(
			//			directionalVector.y,
			//			glm::sqrt(directionalVector.x * directionalVector.x + directionalVector.z * directionalVector.z));

			//	m_rotation.x = glm::degrees(-angleX);
			//	m_rotation.y = glm::degrees(glm::atan(directionalVector.x, directionalVector.z));
			//	m_rotation.z = 0.0f;
			//}
			//else
			//{
			//	Pogplant::Logger::Log({ "PPD::GenericComponents::Transform",Pogplant::LogEntry::LOGTYPE::WARNING, "Angle between look direction and up vector is to small" }, true);
			//}

		}

		inline void LookAtClamped(const glm::vec3& target)
		{
			// bandaid
			glm::vec3 directionalVector = target - GetGlobalPosition();
			float threshold = 8.0f;
			float angle_from_up = glm::degrees(glm::acos(glm::dot(directionalVector, { 0,1,0 }) / glm::length(directionalVector)));
			if (angle_from_up > threshold
				&& angle_from_up < 180.0f - threshold)
			{
				float angleX = glm::atan(
						directionalVector.y,
						glm::sqrt(directionalVector.x * directionalVector.x + directionalVector.z * directionalVector.z));

				m_rotation.x = glm::degrees(-angleX);
				m_rotation.y = glm::degrees(glm::atan(directionalVector.x, directionalVector.z));
				m_rotation.z = 0.0f;
			}
			else
			{
				Pogplant::Logger::Log({ "PPD::GenericComponents::Transform",Pogplant::LogEntry::LOGTYPE::WARNING, "Angle between look direction and up vector is to small" });
			}
		}

		void SetGlobalPosition(const glm::vec3& globalPos);
		void SetGlobalRotation(const glm::vec3& globalRot);
		void SetGlobalScale(const glm::vec3& globalScale);

		glm::vec3 GetGlobalPosition();
		glm::vec3 GetGlobalRotation();
		glm::vec3 GetGlobalScale();
		glm::vec3 GetForwardVector();
		glm::vec3 GetUpVector();

	private:
		bool computedLocal = false;

		glm::mat4 m_midddleMatrix;

		void ComputeLocalMtxes();
	};


	struct Renderer
	{
		Renderer() {};
		Renderer
		(
			glm::vec3 _ColorTint,
			glm::vec3 _EmissiveTint,
			Pogplant::Model* _RenderModel,
			Pogplant::Mesh3D* _Mesh,
			int _UseLight = 1,
			bool _EditorDrawOnly = false
		)
			: m_ColorTint(_ColorTint)
			, m_EmissiveTint(_EmissiveTint)
			, m_RenderModel(_RenderModel)
			, m_Mesh(_Mesh)
			, m_UseLight(_UseLight)
			, m_EditorDrawOnly(_EditorDrawOnly)
		{
		}

		glm::vec3 m_ColorTint;
		glm::vec3 m_EmissiveTint;
		Pogplant::Model* m_RenderModel;
		Pogplant::Mesh3D* m_Mesh;
		int m_UseLight = true;
		int m_EditorDrawOnly = false;
	};

	struct PrimitiveRender : public Renderer
	{
		PrimitiveRender(std::vector<std::string> _DiffTex, std::vector<std::string> _BumpTex, std::vector<std::string> _NormTex, std::vector<std::string> _SpecTex, Pogplant::Mesh* _Mesh, float _Blend, bool _Heightmap = false)
			: m_DiffTex{ _DiffTex }
			, m_BumpTex{ _BumpTex }
			, m_NormTex{ _NormTex }
			, m_SpecTex{ _SpecTex }
			, m_Mesh{ _Mesh }
			, m_Blend{ _Blend }
			, m_Heightmap{ _Heightmap }
		{
			m_ColorTint = glm::vec4{ 1.0f };
		}

		std::vector<std::string> m_DiffTex;		// Save
		std::vector<std::string> m_BumpTex;		// Save
		std::vector<std::string> m_NormTex;		// Save
		std::vector<std::string> m_SpecTex;		// Save
		Pogplant::Mesh* m_Mesh;					// Save
		float m_Blend;							// Save
		bool m_Heightmap;						// Save
	};

	struct DebugRender : public Renderer
	{
		// Some info to add for special render like edge detection
	};

	struct Light
	{
		Light() {};
		Light(glm::vec3 _color, float _intensity)
			: m_Color(_color), m_Intensity(_intensity) {}

		glm::vec3 m_Color;
		float m_Intensity;

		RTTR_ENABLE();
	};

	struct Point_Light : public Light
	{
		Point_Light() {};
		Point_Light(glm::vec3 _color, float _intensity, float _Linear, float _Quadratic)
			: Light{ _color, _intensity }, m_Linear(_Linear), m_Quadratic(_Quadratic){}

		float m_Linear;
		float m_Quadratic;

		RTTR_ENABLE(Light);
	};

	struct Directional_Light : public Light
	{
		Directional_Light() {};
		Directional_Light(glm::vec3 _color, float _intensity, glm::vec3 _direction, float _diffuse, float _specular)
			: Light{ _color, _intensity }, m_Direction(_direction), m_Diffuse(_diffuse), m_Specular(_specular) {}

		glm::vec3 m_Direction;
		float m_Diffuse;
		float m_Specular;

		RTTR_ENABLE(Light);
	};

	struct Text
	{
		Text() = default;
		Text(glm::vec3 Color, std::string FontID, std::string Text = "", bool Ortho = false) :
			m_Color(Color), m_FontID(FontID), m_Text(Text), m_Ortho(Ortho) {};
		glm::vec3 m_Color;
		std::string m_FontID;
		std::string m_Text;
		bool m_Ortho;
	};

	struct Camera
	{
		Camera();
		Camera(bool _Active);

		glm::quat m_Orientation;
		glm::mat4 m_Projection;
		glm::mat4 m_View;
		glm::vec3 m_Up;
		glm::vec3 m_Right;
		glm::vec3 m_Front;
		float m_Yaw;
		float m_Pitch;
		float m_Roll;
		float m_Fov;
		float m_Near;
		float m_Far;
		bool m_Active;
	};

	struct Particle
	{
		struct CurveVariable
		{
			float m_Min;
			float m_Max;
			float m_Multiplier;
		};

		// Reference to particle system to determine curve
		std::vector <float>* m_SpeedCurve;
		std::vector <float>* m_ScaleCurve;
		glm::vec4 m_Color;
		glm::vec3 m_BasePosition;
		glm::vec3 m_LocalPosition;
		glm::vec3 m_Velocity;
		glm::vec3 m_MinVelocity;
		glm::vec3 m_Force;
		CurveVariable m_Speed;
		CurveVariable m_Scale;
		int m_TexID;
		float m_BaseLife;
		float m_Life;
		float m_IndexCalc;
		float m_Rotation;
	};

	struct SpriteAnimation
	{
		// Derived
		glm::vec2 m_Tiling;
		glm::vec2 m_UV_Offset;
		// Accumulated
		float m_FrameCounter;
		// Set
		float m_PlaySpeed;
		int m_MaxFrames;
		int m_Rows;
		int m_Columns;
		bool m_Repeat;
		bool m_Play;
		bool m_Pause;

		SpriteAnimation();
		SpriteAnimation(int _MaxFrames, int _Rows, int _Columns, bool _Repeat, bool _Playing, float _PlaySpeed = 1.0f);
		void Update(float _Dt);
		void UpdateTiling();
		void NextFrame();
		void PrevFrame();
		void init();

	private:
		void CalcUV();
	};

	struct ParticleSystem
	{
		enum class EMITTER_TYPE
		{
			GENERAL,
			BURST,
			CONE,
			COUNT
		};

		// No need to serialize
		struct SubEmitter
		{
			SubEmitter() = default;
			SubEmitter(const glm::vec3& _Position, const glm::vec3& _Direction, int _Count);
			bool Update(float _Dt, float _Delay);

			glm::vec3 m_Position;
			glm::vec3 m_Direction;
			int m_Count;
			float m_Timer;
		};

		struct CurveVariable
		{
			CurveVariable();
			CurveVariable(const std::vector<ImVec2>& _CurvePoints, float _CurveMin, float _CurveMax, float _MultiMin, float _MultiMax);
			float CurveValueSmooth(float _P, int _Maxpoints, const ImVec2* _Points);
			void Spline(const float* _Key, int _Num, int _Dim, float _T, float* _V);

			//enum { m_MaxPoints = 9, m_DataPoints = 128 };

			static size_t m_MaxPoints;
			static size_t m_DataPoints;

			std::vector<float> m_CurveData;
			// For editor
			//ImVec2 m_CurvePoints[m_MaxPoints];
			std::vector<ImVec2> m_CurvePoints;

			float m_MultiplierMin;
			float m_MultiplierMax;

			float m_CurveMin;
			float m_CurveMax;

			inline void init(void)
			{
				m_CurveData.resize(m_MaxPoints);

				int smoothness = static_cast<int>(m_CurveData.size());
				for (int i = 0; i <= (smoothness - 1); ++i)
				{
					float qx = (i + 1) / float(smoothness);
					float qy = 1 - CurveValueSmooth(qx, static_cast<int>(m_CurvePoints.size() + 1), m_CurvePoints.data());
					m_CurveData[i] = qy;
				}
			}
		};

		ParticleSystem();

		ParticleSystem
		(
			glm::vec4 _Color,
			glm::vec3 _SpawnDir,
			glm::vec3 _Force,
			glm::vec3 _BillboardAxis,
			float _SpawnRadius,
			float _ConeRadiusMin,
			float _ConeRadiusMax,
			float _TargetConeScale,
			float _Delay,
			float _SubDelay, // Delay for each point to form a trail
			float _MinLife,
			float _MaxLife,
			CurveVariable _Speed,
			CurveVariable _Scale,
			std::string _TexName,
			int _SpawnCount,
			int _SubSpawnCount,
			int _EmitterType,
			bool _Loop,
			bool _RandomRotate,
			bool _FollowParent,
			bool _Trail
		);

		void Update(float _Dt, const Transform& _Transform, const glm::vec3& _CamPos);
		void Spawn(glm::vec3 _BasePos, glm::vec3 _RandPos, glm::vec3 _Direction);
		void Clear();
		void UpdateInstance(Particle& _Particle, float _Dt, const glm::vec3& _CamPos, const glm::mat4& _ParentMtx, bool _Parented);
		void init();

		std::vector<Particle> m_ParticlePool;
		std::vector<SubEmitter> m_SubEmitters;
		glm::vec4 m_Color;
		glm::vec3 m_SpawnDirection;
		glm::vec3 m_Force;
		glm::vec3 m_BillboardAxis;

		CurveVariable m_Speed;
		CurveVariable m_Scale;

		/// General emitter var
		float m_SpawnRadius; 
		/// Cone emitter var
		float m_ConeRadiusMin;
		float m_ConeRadiusMax;
		float m_TargetConeScale;
		///
		float m_Delay;
		float m_SubDelay;
		float m_Timer;
		float m_MinLife;
		float m_MaxLife;
		std::string m_TexName;
		int m_TexID;
		int m_ActiveCount;
		int m_SpawnCount;
		int m_SubSpawnCount;
		int m_SubActiveCount;
		EMITTER_TYPE m_EmitterType;
		bool m_Loop;
		bool m_Done;
		bool m_RandomRotate;
		bool m_Play;
		bool m_Pause;
		bool m_FollowParent;
		bool m_Trail; 
		
		// extra float timer to keep track of current_lifetime
		float m_CurrentLifetime;
	};

	struct Canvas
	{
		Canvas() = default;
		Canvas(const glm::vec4& _Color, std::string _TexName, bool _Ortho);
		void init(void);
		glm::vec4 m_Color;
		std::string m_TexName;
		int m_TexID;
		bool m_Ortho;
		SpriteAnimation m_SpriteAnimation;
	};

	// For reference for Gab 
	struct HeightMapDebugger
	{
		float currHeight = 0.0f;
	};


	struct Guid
	{
		Guid() = default;
		Guid(std::string _str) : m_guid(_str) {};
		std::string m_guid;
	};

	struct Prefab
	{
		//lol entt don't like to create empty structs
		Prefab() = default;
		Prefab(std::string _str) : file_path(_str) {};
		std::string file_path;
	};

	struct PrefabInstance
	{
		PrefabInstance() = default;
		PrefabInstance(std::string _str, std::string _path);
		std::string prefab_GUID;
		std::string prefab_path;
	};

}

#endif // !GENERICCOMPONENTS_H_
