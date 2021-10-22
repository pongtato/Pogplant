#pragma warning(disable : 4005)

#ifndef APPLICATION_H_
#define APPLICATION_H_

#define PPD_EDITOR_BUILD //If defined, will mean this is in editor mode, remove for final game
#define PPD_UPDATE_EDITOR_AS_GAME //Defines if stuff should update as a game even in editor state
#define PPD_DEBUG_OBJECTS //Whether we should spawn those random debug stuff we using

#include "ImguiHelper.h"
#include "AudioEngine.h"
#include "ECS/Entity.h"
#include "ECS/Systems/GeneralSystem.h"
#include "ECS/Systems/PhysicsSystem.h"
#include "ECS/Systems/ScriptSystem.h"

#include "Input/InputSystem.h"
#include "ResourceAllocator.hpp"
#include "AssetCompiler.h"
#include "FileHandler.h"

#include "Utils/ChronoTimer.h"
#include "Events/Events.h"

#include <iostream>
#include <Pogplant.h>
#include <glm.hpp>
#include <gtc/matrix_transform.hpp>
#include <gtc/type_ptr.hpp>
#include <gtc/random.hpp>
#include <imgui.h>

namespace PogplantDriver
{
	class Application
	{
	public:
		virtual ~Application() = default;

		void Init();
		void Run();
		void Exit();

	private:
		/**************************
		*
		* Enums
		*
		**************************/
		enum class APPLICATIONSTATE
		{
			UNDEFINED,
			EDITOR,
			PLAY
		};

		/**************************
		*
		* Functions
		*
		**************************/
		Application() = default;
		Application(const Application&) = delete;
		Application(Application&&) = delete;
		Application& operator=(const Application&) = delete;
		Application& operator=(Application&&) = delete;

#ifdef PPD_DEBUG_OBJECTS
		void InitialiseDebugObjects();
#endif
		void BindEvents();

		void UpdateTransform(entt::entity _id, Components::Transform& parent_transform);
		void UpdateTransforms(float _Dt);

		//Drawing
		void DrawCommon();
		void DrawEditor();
		void DrawGame();
		void DrawScreen();
		void DrawImGUI();


		//Editor/play
		void EnterEditorState();
		void UpdateEditorState(float);
		void RenderEditorState();
		void LeaveEditorState();

		void EnterPlayState();
		void UpdatePlayState(float);
		void RenderPlayState();
		void LeavePlayState();

		//Other
		void TransitionApplicationState(APPLICATIONSTATE c_newState);

		/**************************
		* 
		* ECS + SYSTEMS
		* s tag being system
		* 
		**************************/
		ECS m_ecs;
		GeneralSystem m_sGeneralSystem;
		PhysicsSystem m_sPhysicsSystem;
		ScriptSystem m_sScriptSystem;
		std::shared_ptr<PPE::EventBus> m_eventBus;

		/**************************
		*
		* Variables
		*
		**************************/

		APPLICATIONSTATE m_appState = APPLICATIONSTATE::UNDEFINED;
		APPLICATIONSTATE m_nextAppState = APPLICATIONSTATE::EDITOR;
		float m_accumulatedFixedTime = 0.f;
		static constexpr float m_minFixedUpdateTime = 1 / 30.f;


		/**************************
		*
		* Singleton
		*
		**************************/
		static std::unique_ptr<Application> m_instance;
		static std::once_flag m_onceFlag;

	public:
		static Application& GetInstance();

	private:

		/**************************
		*
		* TO BE REMOVED
		*
		**************************/
		void ConstructModel(Entity& _Entity, PP::Model* _Model, PP::Mesh3D* _Mesh3D, const glm::vec3& _Color = glm::vec3{ 1 }, bool _UseLight = true, bool _EditorOnly = false, bool _FirstIt = true);
		void OnTriggerEnterEventTest(std::shared_ptr<PPE::OnTriggerEnterEvent> onTriggerEnterEvent);
		void OnTriggerExitEventTest(std::shared_ptr<PPE::OnTriggerExitEvent> onTriggerExitEvent);
	};
}

#endif // !APPLICATION_H_
