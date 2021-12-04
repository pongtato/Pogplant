#pragma warning(disable : 4005)

#ifndef APPLICATION_H_
#define APPLICATION_H_

#define PPD_EDITOR_BUILD //If defined, will mean this is in editor mode, remove for final game
//#define PPD_UPDATE_EDITOR_AS_GAME //Defines if stuff should update as a game even in editor state
#define PPD_DEBUG_OBJECTS //Whether we should spawn those random debug stuff we using

#define NOMINMAX
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

namespace PogplantDriver
{
	class Application
	{
	public:
		virtual ~Application() = default;

		void Init();
		void Run();
		void Exit();

		/**************************
		*
		* Enums
		*
		**************************/
		enum class APPLICATIONSTATE
		{
			UNDEFINED,
			EDITOR,
			PLAY,
			PREFAB_EDITOR,
			NEWSCENETRANSITION
		};

		enum class PLAYSTATE
		{
			PLAY,
			PAUSE,
			STEPNEXT
		};

		void StartPrefabEditing(const std::string& filePath);
		void ExitPrefabEditing();

		/**> Loads a scene while in play mode, can be called anywhere*/
		void LoadScene(const std::string& newScene);

		/**> Calls to transition between play/pause editor */
		void TransitionApplicationState(APPLICATIONSTATE c_newState);
		
		inline void SetPlayState(PLAYSTATE c_playState)
		{
			m_playState = c_playState;
		}

		inline APPLICATIONSTATE GetState() const
		{
			return m_appState;
		}

		inline PLAYSTATE GetPlayState() const
		{
			return m_playState;
		}

	private:
		/**************************
		*
		* Enums
		*
		**************************/
		

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

		void UpdateTransforms(float _Dt);
		void UpdateModelRef(std::vector<std::string>& _EditedModels);

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

		void EnterPrefabState();
		void UpdatePrefabState(float);
		void RenderPrefabState();
		void LeavePrefabState();

		void EnterPlayState(const std::string& sceneToLoad);
		void UpdatePlayState(float);
		void RenderPlayState();
		void LeavePlayState();

		
	public:
		/**************************
		* 
		* ECS + SYSTEMS
		* s tag being system
		* 
		**************************/
		ECS m_editorECS;
		ECS m_playECS;
		ECS* m_activeECS;

		GeneralSystem m_sGeneralSystem;
		PhysicsSystem m_sPhysicsSystem;
		ScriptSystem m_sScriptSystem;
		std::shared_ptr<PPE::EventBus> m_eventBus;

	private:
		/**************************
		*
		* Variables
		*
		**************************/

		APPLICATIONSTATE m_appState = APPLICATIONSTATE::UNDEFINED;
		APPLICATIONSTATE m_nextAppState = APPLICATIONSTATE::EDITOR;
		PLAYSTATE m_playState = PLAYSTATE::PLAY;
		std::string m_genericFilePath;
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
		/**************************
		*
		* TO BE REMOVED
		*
		**************************/
		void ConstructModel(Entity& _Entity, PP::Model* _Model, PP::Mesh3D* _Mesh3D, const glm::vec3& _Color = glm::vec3{ 1 }, const glm::vec3& _Emissive = glm::vec3{ 1 }, bool _UseLight = true, bool _EditorOnly = false, bool _FirstIt = true);
	private:


	
	};
}

#endif // !APPLICATION_H_
