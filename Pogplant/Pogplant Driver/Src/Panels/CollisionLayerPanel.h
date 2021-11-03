#ifndef COLLISIONLAYERPANEL_H_
#define COLLISIONLAYERPANEL_H_

#include "../Panels.h"

namespace PPP
{
	class CollisionLayerPanel : public PPD::Panels
	{
	public:
		CollisionLayerPanel() = default;
		~CollisionLayerPanel() = default;

		virtual void Init(ECS* ecs, entt::entity& current_entity) override;
		virtual void Render(entt::entity& current_entity) override;

	private:
	};
}

#endif // !COLLISIONLAYERPANEL_H_
