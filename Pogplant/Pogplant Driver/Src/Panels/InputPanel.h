#ifndef INPUTPANEL_H_
#define INPUTPANEL_H_

#include "../Panels.h"

namespace PPP
{
	class InputPanel : public PPD::Panels
	{
	public:
		InputPanel() = default;
		~InputPanel() = default;

		virtual void Init(ECS* ecs, entt::entity& current_entity) override;
		virtual void Render(entt::entity& current_entity) override;

	private:
		std::string GetKeyString(int glfwKeyCode);
		bool IsValidKey(int glfwKeyCode);

		std::string GetControllerString(int glfwControllerCode);
		bool IsValidControllerKey(int glfwKeyCode);
	};
}


#endif // !INPUTPANEL_H_
