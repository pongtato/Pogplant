#ifndef GENERICCOMPONENTS_H_
#define GENERICCOMPONENTS_H_

#include <rttr/registration>

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

	struct Imaginary_object
	{
		std::string str;
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

		glm::mat4 m_ModelMtx;

		Transform() = default;
		Transform(const Transform&) = default;
		Transform(glm::vec3 pos, glm::vec3 rot, glm::vec3 scale)
			: m_position(pos), m_rotation(rot), m_scale(scale)
		{
			init();
		}

		void init(void)
		{
			m_ModelMtx = glm::mat4{ 1 };
			m_ModelMtx = glm::translate(m_ModelMtx, m_position);
			m_ModelMtx = glm::rotate(m_ModelMtx, m_rotation[0], { 1,0,0 });
			m_ModelMtx = glm::rotate(m_ModelMtx, m_rotation[1], { 0,1,0 });
			m_ModelMtx = glm::rotate(m_ModelMtx, m_rotation[2], { 0,0,1 });
			m_ModelMtx = glm::scale(m_ModelMtx, m_scale);
		}

		void updateModelMtx(void)
		{
			m_ModelMtx = glm::mat4{ 1 };
			m_ModelMtx = glm::translate(m_ModelMtx, m_position);
			m_ModelMtx = glm::rotate(m_ModelMtx, m_rotation[0], { 1,0,0 });
			m_ModelMtx = glm::rotate(m_ModelMtx, m_rotation[1], { 0,1,0 });
			m_ModelMtx = glm::rotate(m_ModelMtx, m_rotation[2], { 0,0,1 });
			m_ModelMtx = glm::scale(m_ModelMtx, m_scale);
		}
	};


	struct Renderer
	{
		Renderer() {};
		Renderer(glm::mat4 Model, glm::vec3 ColorTint, Pogplant::Model* RenderModel, int UseLight = 1, bool EditorDrawOnly = false) :
			m_Model(Model), m_ColorTint(ColorTint), m_RenderModel(RenderModel), m_UseLight(UseLight), m_EditorDrawOnly(EditorDrawOnly){}

		glm::mat4 m_Model;
		glm::vec3 m_ColorTint;
		Pogplant::Model* m_RenderModel;
		int m_UseLight = true;
		bool m_EditorDrawOnly = false;
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

	struct AudioSource
	{
		struct AudioClip
		{
			std::string m_name;
			float m_volume = 1.f;
			bool m_is3D;
			bool m_isLooping;
			bool m_isStreamed;
		};

		std::vector<AudioClip> m_audioSources;

		inline void PlayAudio(size_t id = 0)
		{
			if (m_audioSources.size() > id)
			{
				//PPA::AudioEngine::Instance();
			}
		}
	};

	//Temporary
	struct CharacterController
	{
		float force = 2.f;
	};

}

#endif // !GENERICCOMPONENTS_H_
