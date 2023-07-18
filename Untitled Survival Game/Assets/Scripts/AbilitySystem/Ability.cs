using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(menuName = "AbilitySystem/Ability")]
	public class Ability : ScriptableObject
	{
		// Override these functions to define a new ability behavior

		public virtual bool CanActivate(AbilityInstanceData data) { return true; }


		public virtual void Activate(AbilityInstanceData data)
		{
			Debug.Log("Ability Activate");

			End(data);
		}


		public virtual void Cancel(AbilityInstanceData data)
		{
			Debug.Log("Ability Cancel");

			End(data);
		}


		// Perform any cleanup needed after the ability finishes or is canceled
		protected virtual void End(AbilityInstanceData data)
		{
			Debug.Log("Ability End");
		}


		public virtual AbilityInstanceData CreateInstanceData(AbilityActor user)
		{
			// Derive from AbilityInstanceData for more complex abilities
			return new AbilityInstanceData() { User = user };
		}


		public virtual void Tick(AbilityInstanceData data, float deltaTime)
		{
			data.CooldownRemaining -= deltaTime;

			if (data.CooldownRemaining < 0f)
			{
				data.CooldownRemaining = 0f;
			}
		}
	}
}
