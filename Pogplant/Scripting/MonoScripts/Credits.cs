using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
	public class Credits : MonoBehaviour
	{
		uint m_credits_id;
		float m_scroll_speed;
		float m_y_cutoff;
		bool move_bottom_bar = true;


		//Press any key flashing
		const float any_key_scale_speed = 5.0f;
		const float any_key_expanded_scale = 0.8f;
		const float any_key_default_scale = 0.75f;
		const float any_key_spin_speed = 2.0f;
		const float any_key_move_out_speed = 4.0f;
		bool any_key_expand;
		bool begin_any_key_spin;
		uint any_key_to_continue_id;


		public override void Init(ref uint _entityID)
		{
			entityID = _entityID;
		}

		public override void Start()
		{
			BindData();
			ECS.PlayAudio(entityID, 1, "BGM");
			ECS.PlayAudio(entityID, 2, "VO");
		}

		//mainly for binding of datas and entity id
		void BindData()
		{
			m_scroll_speed = ECS.GetValue<float>(entityID, 0.5f, "m_scroll_speed");
			m_y_cutoff = ECS.GetValue<float>(entityID, 2.3f, "m_y_cutoff");

			m_credits_id = ECS.FindChildEntityWithName(entityID, "Credits");
			any_key_to_continue_id = ECS.FindChildEntityWithName(entityID, "Any Key Continue");

			move_bottom_bar = true;

			ECS.SetActive(any_key_to_continue_id, false);
		}

		public override void Update(float dt)
		{
			Vector3 temp = new Vector3();
			Vector3 pos = new Vector3();


			if (move_bottom_bar)
            {
				ECS.GetTransformECS(m_credits_id, ref pos, ref temp, ref temp);

				if (pos.Y < m_y_cutoff)
					ECS.SetPosition(m_credits_id, new Vector3(pos.X, pos.Y + m_scroll_speed * dt, pos.Z));
				else
				{
					move_bottom_bar = false;
					ECS.SetActive(any_key_to_continue_id, true);
					begin_any_key_spin = false;
				}
			}
			else
            {
				if (!begin_any_key_spin)
				{
					if (any_key_expand)
					{
						ECS.SetGlobalScale(any_key_to_continue_id, Vector3.Lerp(new Vector3(ECS.GetGlobalScale(any_key_to_continue_id).X, ECS.GetGlobalScale(any_key_to_continue_id).Y, 1.0f), new Vector3(any_key_expanded_scale, any_key_expanded_scale * 0.5f, 1.0f), any_key_scale_speed * dt));
						if (ECS.GetGlobalScale(any_key_to_continue_id).X >= any_key_expanded_scale - 0.01f)
						{
							any_key_expand = false;
						}
					}
					else
					{
						ECS.SetGlobalScale(any_key_to_continue_id, Vector3.Lerp(new Vector3(ECS.GetGlobalScale(any_key_to_continue_id).X, ECS.GetGlobalScale(any_key_to_continue_id).Y, 1.0f), new Vector3(any_key_default_scale, any_key_default_scale * 0.5f, 1.0f), any_key_scale_speed * dt));
						if (ECS.GetGlobalScale(any_key_to_continue_id).X <= any_key_default_scale + 0.01f)
						{
							any_key_expand = true;
						}
					}
				}
            }

			UpdateInput();
		}


		private void UpdateInput()
		{
			//used to skip watching the credit scene :(
            if (InputUtility.onKeyTriggered("ESCAPE") ||
				(!move_bottom_bar && InputUtility.onAnyKey()))
            {
				ECS.PlayAudio(entityID, 0, "SFX");
				GameUtilities.LoadScene("MainMenu");
            }
        }

		public override void LateUpdate(float dt)
		{

		}

		public override void OnTriggerEnter(uint id)
		{
		}

		public override void OnTriggerExit(uint id)
		{
		}
	}
}
