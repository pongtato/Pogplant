using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
	public class PlayerNuke : MonoBehaviour
	{
		//Variables

		List<uint> m_enemy_in_range = new List<uint>();

		// Transition time between states
		List<float> m_state_time = new List<float>();


		List<Vector3> m_Overall_scale = new List<Vector3>();
		List<Vector3> m_Shockwaves_scale = new List<Vector3>();
		List<Vector3> m_Nuke_Outer_scale = new List<Vector3>();
		List<Vector3> m_Nuke_Swirl_scale = new List<Vector3>();
		List<Vector3> m_Nuke_Outer_Spikes_scale = new List<Vector3>();
		List<Vector3> m_Nuke_Outer_Outline_scale = new List<Vector3>();

		//used for doing scaling transition
		//or = original
		bool set_transition = true;

		Vector3 or_Overall_scale;
		Vector3 or_Shockwaves_scale;
		Vector3 or_Nuke_Outer_scale;
		Vector3 or_Nuke_Swirl_scale;
		Vector3 or_Nuke_Outer_Spikes_scale;
		Vector3 or_Nuke_Outer_Outline_scale;

		//(new scale - old scale)/time taken
		float or_Overall_scale_rate;
		float or_Shockwaves_scale_rate;
		float or_Nuke_Outer_scale_rate;
		float or_Nuke_Swirl_scale_rate;
		float or_Nuke_Outer_Spikes_scale_rate;
		float or_Nuke_Outer_Outline_scale_rate;

		//entity id for the parts
		uint m_Shockwaves;
		uint m_Nuke_Outer;
		uint m_Nuke_Swirl;
		uint m_Nuke_Outer_Spikes;
		uint m_Nuke_Outer_Outline;

		uint m_null_entity;

		int m_state_counter = 0;
		float m_timer = 0.0f;

		//used to stop it from shooting 
		public static bool m_has_fired = false;

		public override void Init(ref uint _entityID)
		{
			entityID = _entityID;
			m_null_entity = ECS.GetNull();
		}

		public override void Start()
		{
			BindData();
		}

		void FindAndCheck(ref uint destination, string entity_name, bool is_child)
		{
			uint result = is_child ? ECS.FindChildEntityWithName(entityID, entity_name) : ECS.FindEntityWithName(entity_name);
            if (result == m_null_entity)
                Console.WriteLine(entity_name + " is null entity");
            else
                destination = result;
		}

		//mainly for binding of datas and entity id
		void BindData()
		{
			FindAndCheck(ref m_Shockwaves, "Shockwaves", true);
			FindAndCheck(ref m_Nuke_Outer, "Nuke_Outer", true);
			FindAndCheck(ref m_Nuke_Swirl, "Nuke_Swirl", true);
			FindAndCheck(ref m_Nuke_Outer_Spikes, "Nuke_Outer_Spikes", true);
			FindAndCheck(ref m_Nuke_Outer_Outline, "Nuke_Outer_Outline", true);


			//state 1
			//m_delay.Add(ECS.GetValue<float>(entityID, 2.0f, "s1_delay"));
			m_Overall_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s1_overall_scale"));
			m_Shockwaves_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s1_Shockwaves_scale"));
			m_Nuke_Outer_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s1_Nuke_Outer"));
			m_Nuke_Swirl_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s1_Nuke_Swirl"));
			m_Nuke_Outer_Spikes_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s1_Nuke_Outer_Spikes"));
			m_Nuke_Outer_Outline_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s1_Nuke_Outer_Outline"));

			//state 2
			m_state_time.Add(ECS.GetValue<float>(entityID, 2.0f, "s1_s2_transition"));
			m_Overall_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s2_overall_scale"));
			m_Shockwaves_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s2_Shockwaves_scale"));
			m_Nuke_Outer_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s2_Nuke_Outer"));
			m_Nuke_Swirl_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1.1f, 1.1f, 1.1f), "s2_Nuke_Swirl"));
			m_Nuke_Outer_Spikes_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s2_Nuke_Outer_Spikes"));
			m_Nuke_Outer_Outline_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1.1f, 1.1f, 1.1f), "s2_Nuke_Outer_Outline"));

			//state 3
			m_state_time.Add(ECS.GetValue<float>(entityID, 2.0f, "s2_s3_transition"));
			m_Overall_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s3_overall_scale"));
			m_Shockwaves_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s3_Shockwaves_scale"));
			m_Nuke_Outer_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(0.75f, 0.75f, 0.75f), "s3_Nuke_Outer"));
			m_Nuke_Swirl_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(2, 2, 2), "s3_Nuke_Swirl"));
			m_Nuke_Outer_Spikes_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s3_Nuke_Outer_Spikes"));
			m_Nuke_Outer_Outline_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1.1f, 1.1f, 1.1f), "s3_Nuke_Outer_Outline"));

			//state 4
			m_state_time.Add(ECS.GetValue<float>(entityID, 2.0f, "s3_s4_transition"));
			m_Overall_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(0.1f, 0.1f, 0.1f), "s4_overall_scale"));
			m_Shockwaves_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s4_Shockwaves_scale"));
			m_Nuke_Outer_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(0.75f, 0.75f, 0.75f), "s4_Nuke_Outer"));
			m_Nuke_Swirl_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(0, 0, 0), "s4_Nuke_Swirl"));
			m_Nuke_Outer_Spikes_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s4_Nuke_Outer_Spikes"));
			m_Nuke_Outer_Outline_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1.1f, 1.1f, 1.1f), "s4_Nuke_Outer_Outline"));

			//state 5
			m_state_time.Add(ECS.GetValue<float>(entityID, 1.0f, "s4_s5_transition"));
			m_Overall_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(3, 3, 3), "s5_overall_scale"));
			m_Shockwaves_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s5_Shockwaves_scale"));
			m_Nuke_Outer_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s5_Nuke_Outer"));
			m_Nuke_Swirl_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(0, 0, 0), "s5_Nuke_Swirl"));
			m_Nuke_Outer_Spikes_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1, 1, 1), "s5_Nuke_Outer_Spikes"));
			m_Nuke_Outer_Outline_scale.Add(ECS.GetValue<Vector3>(entityID, new Vector3(1.1f, 1.1f, 1.1f), "s5_Nuke_Outer_Outline"));
		}

		void SetActiveAndScale(uint entity, bool state, Vector3 scale)
		{
			ECS.SetActive(entity, state);
			ECS.SetScale(entity, scale);
		}

		void CalcScaleRate(int index)
        {
			int prev = index - 1;
			or_Overall_scale_rate = (m_Overall_scale[index] - m_Overall_scale[prev]).X / m_state_time[prev];
			or_Shockwaves_scale_rate = (m_Shockwaves_scale[index] - m_Shockwaves_scale[prev]).X / m_state_time[prev];
			or_Nuke_Outer_scale_rate = (m_Nuke_Outer_scale[index] - m_Nuke_Outer_scale[prev]).X / m_state_time[prev];
			or_Nuke_Swirl_scale_rate = (m_Nuke_Swirl_scale[index] - m_Nuke_Swirl_scale[prev]).X / m_state_time[prev];
			or_Nuke_Outer_Spikes_scale_rate = (m_Nuke_Outer_Spikes_scale[index] - m_Nuke_Outer_Spikes_scale[prev]).X / m_state_time[prev];
			or_Nuke_Outer_Outline_scale_rate = (m_Nuke_Outer_Outline_scale[index] - m_Nuke_Outer_Outline_scale[prev]).X / m_state_time[prev];
		}

		void AddToScale(ref Vector3 vec3, float amount)
        {
			vec3.X += amount;
			vec3.Y += amount;
			vec3.Z += amount;
		}

		void GrowShrink(float dt)
        {
			AddToScale(ref or_Overall_scale, or_Overall_scale_rate * dt);
			ECS.SetScale(entityID, or_Overall_scale);

			AddToScale(ref or_Shockwaves_scale, or_Shockwaves_scale_rate * dt);
			ECS.SetScale(m_Shockwaves, or_Shockwaves_scale);

			AddToScale(ref or_Nuke_Outer_scale, or_Nuke_Outer_scale_rate * dt);
			ECS.SetScale(m_Nuke_Outer, or_Nuke_Outer_scale);

			AddToScale(ref or_Nuke_Swirl_scale, or_Nuke_Swirl_scale_rate * dt);
			ECS.SetScale(m_Nuke_Swirl, or_Nuke_Swirl_scale);

			AddToScale(ref or_Nuke_Outer_Spikes_scale, or_Nuke_Outer_Spikes_scale_rate * dt);
			ECS.SetScale(m_Nuke_Outer_Spikes, or_Nuke_Outer_Spikes_scale);

			AddToScale(ref or_Nuke_Outer_Outline_scale, or_Nuke_Outer_Outline_scale_rate * dt);
			ECS.SetScale(m_Nuke_Outer_Outline, or_Nuke_Outer_Outline_scale);
		}

		void Animation_State(int state, float dt)
		{
			switch(state)
			{
				case 1:
					{
						if (set_transition)
						{
							ECS.PlayAudio(entityID, 0, "SFX");

							CalcScaleRate(1);

							ECS.SetActive(m_Shockwaves, false);
							ECS.SetActive(m_Nuke_Outer, true);
							ECS.SetActive(m_Nuke_Swirl, true);
							ECS.SetActive(m_Nuke_Outer_Spikes, true);
							ECS.SetActive(m_Nuke_Outer_Outline, true);

							set_transition = false;

							or_Overall_scale = m_Overall_scale[0];
							or_Shockwaves_scale = m_Shockwaves_scale[0];
							or_Nuke_Outer_scale = m_Nuke_Outer_scale[0];
							or_Nuke_Swirl_scale = m_Nuke_Swirl_scale[0];
							or_Nuke_Outer_Spikes_scale = m_Nuke_Outer_Spikes_scale[0];
							or_Nuke_Outer_Outline_scale = m_Nuke_Outer_Outline_scale[0];


						}

						//grow/shrink
						GrowShrink(dt);
					}
					break;
				case 2:
					{
						if (set_transition)
						{
							CalcScaleRate(2);

							ECS.SetActive(m_Shockwaves, false);
							ECS.SetActive(m_Nuke_Outer, true);
							ECS.SetActive(m_Nuke_Swirl, true);
							ECS.SetActive(m_Nuke_Outer_Spikes, true);
							ECS.SetActive(m_Nuke_Outer_Outline, true);

							set_transition = false;

							or_Overall_scale = m_Overall_scale[1];
							or_Shockwaves_scale = m_Shockwaves_scale[1];
							or_Nuke_Outer_scale = m_Nuke_Outer_scale[1];
							or_Nuke_Swirl_scale = m_Nuke_Swirl_scale[1];
							or_Nuke_Outer_Spikes_scale = m_Nuke_Outer_Spikes_scale[1];
							or_Nuke_Outer_Outline_scale = m_Nuke_Outer_Outline_scale[1];
						}

						//grow/shrink
						GrowShrink(dt);

					}
					break;
				case 3:
					{
						if (set_transition)
						{
							CalcScaleRate(3);

							ECS.SetActive(m_Shockwaves, false);
							ECS.SetActive(m_Nuke_Outer, true);
							ECS.SetActive(m_Nuke_Swirl, true);
							ECS.SetActive(m_Nuke_Outer_Spikes, true);
							ECS.SetActive(m_Nuke_Outer_Outline, true);

							set_transition = false;

							or_Overall_scale = m_Overall_scale[2];
							or_Shockwaves_scale = m_Shockwaves_scale[2];
							or_Nuke_Outer_scale = m_Nuke_Outer_scale[2];
							or_Nuke_Swirl_scale = m_Nuke_Swirl_scale[2];
							or_Nuke_Outer_Spikes_scale = m_Nuke_Outer_Spikes_scale[2];
							or_Nuke_Outer_Outline_scale = m_Nuke_Outer_Outline_scale[2];

							GameUtilities.StopMoving(entityID);
						}

						GrowShrink(dt);
					}
					break;
				case 4:
					{
						if (set_transition)
						{
							CalcScaleRate(4);

							ECS.SetActive(m_Shockwaves, true);
							ECS.SetActive(m_Nuke_Outer, true);
							ECS.SetActive(m_Nuke_Swirl, false);
							ECS.SetActive(m_Nuke_Outer_Spikes, true);
							ECS.SetActive(m_Nuke_Outer_Outline, true);

							set_transition = false;

							or_Overall_scale = m_Overall_scale[3];
							or_Shockwaves_scale = m_Shockwaves_scale[3];
							or_Nuke_Outer_scale = m_Nuke_Outer_scale[3];
							or_Nuke_Swirl_scale = m_Nuke_Swirl_scale[3];
							or_Nuke_Outer_Spikes_scale = m_Nuke_Outer_Spikes_scale[3];
							or_Nuke_Outer_Outline_scale = m_Nuke_Outer_Outline_scale[3];

							TimeManager.TriggerTimeAlter(0.5f, 2f);
						}

						GrowShrink(dt);
					}
					break;
				default:
					break;
			}
		}

		public override void Update(float dt)
		{
			if (m_has_fired)
            {
				if (m_state_counter < m_state_time.Count())
				{
					m_timer += dt;
					//Console.WriteLine(m_state_counter + 1);
					Animation_State(m_state_counter + 1, dt);

					if (m_timer > m_state_time[m_state_counter])
					{
						m_timer = 0;
						m_state_counter++;
						set_transition = true;
					}
				}
				else
				{
					//if exceed, hit everyone

					//var NoDuplicate = m_enemy_in_range.Distinct();
					////Console.WriteLine( "in range of nuke: " + NoDuplicate.Count());
					//foreach (uint entity in NoDuplicate)
					//{
					//	//SSH::InvokeFunction("EncounterSystemDriver", "TakeDamage", GameScript::GetPlayerBox(), static_cast<std::uint32_t>(other), player_projectile_script->m_Damage);
					//	GameUtilities.EnemyTakeDamageFromID(entity, 9999);
					//	EncounterSystemDriver.TakeDamage(entity, 9999);

					//}

					//m_enemy_in_range.Clear();

					//disable self
					ECS.SetActive(entityID, false);

					//reset variables 
					m_timer = 0;
					m_state_counter = 0;
					set_transition = true;
					m_has_fired = false;
				}
			}
		}

		void SpinObjectEndless(uint id, Vector3 axis, float spin_speed, float dt)
		{
			//if (m_spin_propeller)
			//{
			//	m_propeller_rot += axis * spin_speed * dt;

			//	ClampRotationValue(ref m_propeller_rot.X, -360.0f, 360.0f);
			//	ClampRotationValue(ref m_propeller_rot.Y, -360.0f, 360.0f);
			//	ClampRotationValue(ref m_propeller_rot.Z, -360.0f, 360.0f);

			//	ECS.SetRotation(id, m_propeller_rot);
			//}
		}

		void ClampRotationValue(ref float value, float min, float max)
		{
			if (value <= min)
				value = 0.0f;
			else if (value >= max)
				value = 0.0f;
		}

		public override void LateUpdate(float dt)
		{

		}

		public override void OnTriggerEnter(uint id)
		{
			//if (id == m_player_id)
			//{
			//	if (!m_armed)
			//		m_armed = true;
			//}
			//m_enemy_in_range.Add(id);

			//foreach (uint entity in NoDuplicate)
			//{
			//	//SSH::InvokeFunction("EncounterSystemDriver", "TakeDamage", GameScript::GetPlayerBox(), static_cast<std::uint32_t>(other), player_projectile_script->m_Damage);
			//	GameUtilities.EnemyTakeDamageFromID(entity, 9999);
			//	EncounterSystemDriver.TakeDamage(entity, 9999);

			//}

			if (m_has_fired)
            {
				GameUtilities.EnemyTakeDamageFromID(id, 9999);
				EncounterSystemDriver.TakeDamage(id, 9999);
			}


		}

		public override void OnTriggerExit(uint id)
		{
			m_enemy_in_range.Remove(id);
		}

		public void TakeDamage(float damage)
		{

		}

		public void Explode()
		{
			//var NoDuplicate = m_enemy_in_range.Distinct();

			//foreach (uint entity in NoDuplicate)
   //         {
			//	GameUtilities.EnemyTakeDamageFromID(entity, m_damage);
   //         }

			////disable self
			//ECS.SetActive(entityID, false);

		}
	}
}
