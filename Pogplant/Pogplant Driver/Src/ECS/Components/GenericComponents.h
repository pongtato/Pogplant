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

	struct Relationship
	{
		entt::entity m_parent = entt::null;
		std::set<entt::entity> m_children;

	};

	struct Transform
	{
		glm::vec3 m_position;
		glm::vec3 m_rotation;
		glm::vec3 m_scale;

		glm::mat4 m_ModelMtx;// has been multiplied by parent matrix
		glm::mat4 actual_m_ModelMtx;

		Transform() = default;
		Transform(const Transform&) = default;
		Transform(glm::vec3 pos, glm::vec3 rot, glm::vec3 scale)
			: m_position(pos), m_rotation(rot), m_scale(scale)
		{
			init();
		}

		void init(void)
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

		void updateModelMtx(void)
		{

			ImGuizmo::RecomposeMatrixFromComponents(glm::value_ptr(m_position),
													glm::value_ptr(m_rotation),
													glm::value_ptr(m_scale),
													glm::value_ptr(m_ModelMtx));
		}

		void updateModelMtx(Transform _transform)
		{
			updateModelMtx();
			actual_m_ModelMtx = m_ModelMtx;
			m_ModelMtx = _transform.m_ModelMtx * m_ModelMtx;
		}
	};


	struct Renderer
	{
		Renderer() {};
		Renderer(glm::vec3 ColorTint, Pogplant::Model* RenderModel, Pogplant::Mesh3D* Mesh, int UseLight = 1, bool EditorDrawOnly = false)
			: m_ColorTint(ColorTint)
			, m_RenderModel(RenderModel)
			, m_Mesh(Mesh)
			, m_UseLight(UseLight)
			, m_EditorDrawOnly(EditorDrawOnly)
		{
		}

		glm::vec3 m_ColorTint;
		int m_UseLight = true;
		bool m_EditorDrawOnly = false;
		Pogplant::Model* m_RenderModel;
		Pogplant::Mesh3D* m_Mesh;
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
		}

		std::vector<std::string> m_DiffTex;
		std::vector<std::string> m_BumpTex;
		std::vector<std::string> m_NormTex;
		std::vector<std::string> m_SpecTex;
		Pogplant::Mesh* m_Mesh;
		float m_Blend;
		bool m_Heightmap;
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
		glm::vec4 m_Color;
		glm::vec3 m_Position;
		glm::vec3 m_Velocity;
		glm::vec3 m_MinVelocity;
		glm::vec3 m_BaseScale;
		glm::vec3 m_Scale;
		int m_TexID;
		float m_BaseLife;
		float m_Life;
		bool m_Gravity;
		bool m_LerpSpeed;
	};

	struct ParticleSystem
	{
		ParticleSystem
		(
			glm::vec4 _Color,
			glm::vec3 _SpawnDir,
			float _Delay,
			float _MinLife,
			float _MaxLife,
			float _MinScale,
			float _MaxScale,
			float _MinSpeed,
			float _MaxSpeed,
			int _SpawnCount,
			bool _Loop,
			bool _Gravity,
			bool _Burst,
			bool _LerpSpeed
		) 
			: m_Color{ _Color }
			, m_SpawnDirection{ _SpawnDir }
			, m_Delay{ _Delay }
			, m_Timer{ 0 }
			, m_MinLife{ _MinLife }
			, m_MaxLife{ _MaxLife }
			, m_MinScale{ _MinScale }
			, m_MaxScale{ _MaxScale }
			, m_MinSpeed{ _MinSpeed }
			, m_MaxSpeed{ _MaxSpeed }
			, m_ActiveCount{ 0 }
			, m_SpawnCount{ _SpawnCount }
			, m_Gravity{ _Gravity }
			, m_Loop{ _Loop }
			, m_Done{ false }
			, m_Burst{ _Burst }
			, m_LerpSpeed{ _LerpSpeed }
		{
		}

		void Spawn(glm::vec3 _Position, glm::vec3 _Direction)
		{
			// Scale up if need more
			if (m_ActiveCount >= m_ParticlePool.size())
			{
				m_ParticlePool.resize(m_ParticlePool.size() + 1 * 2);
			}

			float speed = glm::linearRand(m_MinSpeed, m_MaxSpeed);
			float life = glm::linearRand(m_MinLife, m_MaxLife);
			float scale = glm::linearRand(m_MinScale, m_MaxScale);

			// Update at end of pool
			m_ParticlePool[m_ActiveCount] =
				Particle
			{
				m_Color,
				_Position,
				_Direction * speed,
				glm::vec3{0},
				glm::vec3{scale},
				glm::vec3{scale},
				-1,
				life,
				life,
				m_Gravity,
				m_LerpSpeed,
			};

			m_ActiveCount++;
		}
		
		std::vector<Particle> m_ParticlePool;
		glm::vec4 m_Color;
		glm::vec3 m_SpawnDirection;
		float m_Delay;
		float m_Timer;
		float m_MinLife;
		float m_MaxLife;
		float m_MinScale;
		float m_MaxScale;
		float m_MinSpeed;
		float m_MaxSpeed;
		int m_ActiveCount;
		int m_SpawnCount;
		bool m_Gravity;
		bool m_Loop;
		bool m_Done;
		bool m_Burst;
		bool m_LerpSpeed;
	};

	struct Canvas
	{
		glm::vec4 m_Color;
		int m_TexID;
	};

	// For reference for Gab 
	struct HeightMapDebugger
	{
		float currHeight = 0.0f;
	};
}

#endif // !GENERICCOMPONENTS_H_
