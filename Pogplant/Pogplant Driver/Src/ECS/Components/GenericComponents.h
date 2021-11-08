#ifndef GENERICCOMPONENTS_H_
#define GENERICCOMPONENTS_H_

#include <rttr/registration>
#include <../IMGUI/ImGuiExtraStyles.h>
//#include <ImGuizmo.h>
#include <../IMGUIZMO/ImGuizmo.h>
#include <gtc/type_ptr.hpp>
#include <gtc/random.hpp>

namespace Components
{
	struct Tag
	{
		std::string m_tag;
	};

	struct Name
	{
		Name() = default;
		Name(std::string name) : m_name(name) {}
		std::string m_name;
	};

	struct TheUnkillable6969xx
	{
		//anything with this won't be deleted
	};

	struct PositionList
	{
		short m_index;
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
			//ImGuizmo::RecomposeMatrixFromComponents(glm::value_ptr(m_position),
			//										glm::value_ptr(m_rotation),
			//										glm::value_ptr(m_scale),
			//										glm::value_ptr(m_ModelMtx));

			m_ModelMtx = glm::mat4{ 1 };
			m_ModelMtx = glm::translate(m_ModelMtx, m_position);
			m_ModelMtx = glm::rotate(m_ModelMtx, m_rotation[0], { 1,0,0 });
			m_ModelMtx = glm::rotate(m_ModelMtx, m_rotation[1], { 0,1,0 });
			m_ModelMtx = glm::rotate(m_ModelMtx, m_rotation[2], { 0,0,1 });
			m_ModelMtx = glm::scale(m_ModelMtx, m_scale);
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

		inline void updateModelMtx(Transform _transform)
		{
			m_localToWorld = _transform.m_ModelMtx;

			updateModelMtx();
			m_ModelMtx = _transform.m_ModelMtx * m_ModelMtx;
		}

		void SetGlobalPosition(const glm::vec3& globalPos);
		void SetGlobalRotation(const glm::vec3& globalRot);
		void SetGlobalScale(const glm::vec3& globalScale);

		glm::vec3 GetGlobalPosition();
		glm::vec3 GetGlobalRotation();
		glm::vec3 GetGlobalScale();

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

		glm::vec3 m_ColorTint;		// Save
		glm::vec3 m_EmissiveTint;	// Save
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
		glm::mat4 m_Projection;
		glm::mat4 m_View;
		glm::vec3 m_Up;
		glm::vec3 m_Right;
		glm::vec3 m_Front;
		float m_Yaw;
		float m_Pitch;
		float m_Zoom;
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
		glm::vec3 m_Position;
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

	struct ParticleSystem
	{
		struct CurveVariable
		{
			CurveVariable() = default;
			CurveVariable(float _CurveMin, float _CurveMax, float _MultiMin, float _MultiMax);

			enum { m_MaxPoints = 9, m_DataPoints = 128 };
			std::vector<float> m_CurveData;
			// For editor
			ImVec2 m_CurvePoints[m_MaxPoints];

			float m_MultiplierMin;
			float m_MultiplierMax;

			float m_CurveMin;
			float m_CurveMax;
		};

		ParticleSystem() = default;

		ParticleSystem
		(
			glm::vec4 _Color,
			glm::vec3 _SpawnDir,
			glm::vec3 _Force,
			float _SpawnRadius,
			float _Delay,
			float _MinLife,
			float _MaxLife,
			CurveVariable _Speed,
			CurveVariable _Scale,
			std::string _TexID,
			int _SpawnCount,
			bool _Loop,
			bool _Burst,
			bool _RandomRotate
		);
		void Spawn(glm::vec3 _Position, glm::vec3 _Direction);
		void Clear();
		void UpdateInstance(Particle& _Particle, float _Dt, const glm::vec3& _CamPos);

		std::vector<Particle> m_ParticlePool;
		glm::vec4 m_Color;
		glm::vec3 m_SpawnDirection;
		glm::vec3 m_Force;
		CurveVariable m_Speed;
		CurveVariable m_Scale;
		float m_SpawnRadius;
		float m_Delay;
		float m_Timer;
		float m_MinLife;
		float m_MaxLife;
		std::string m_TexName;
		int m_TexID;
		int m_ActiveCount;
		int m_SpawnCount;
		bool m_Loop;
		bool m_Done;
		bool m_Burst;
		bool m_RandomRotate;
		bool m_Play;
		bool m_Pause;
	};

	struct Canvas
	{
		Canvas(const glm::vec4& _Color, std::string _TexName);

		glm::vec4 m_Color;
		std::string m_TexName;
		int m_TexID;
	};

	// For reference for Gab 
	struct HeightMapDebugger
	{
		float currHeight = 0.0f;
	};


	struct GUID
	{

	};

	struct Prefab
	{
		//lol entt don't like to create empty structs
		char i = 0;
	};

	struct PrefabInstance
	{
		
	};

}

#endif // !GENERICCOMPONENTS_H_
