using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(menuName = "AbilitySystem/Ability")]
	public class Ability : ScriptableObject
	{
		// Override these functions to define a new ability behavior

		public virtual bool CanActivate(AbilityHandle handle) { return true; }


		public virtual void Activate(AbilityHandle handle)
		{
			Debug.Log("Ability Activate");

			End(handle);
		}


		public virtual void Cancel(AbilityHandle handle)
		{
			Debug.Log("Ability Cancel");

			End(handle);
		}


		// Perform any cleanup needed after the ability finishes or is canceled
		protected virtual void End(AbilityHandle handle)
		{
			Debug.Log("Ability End");

			handle.AbilityData.User.HandleAbilityEnd();
		}


		public virtual AbilityInstanceData CreateInstanceData(AbilityActor user)
		{
			// Derive from AbilityInstanceData for more complex abilities
			return new AbilityInstanceData() { User = user };
		}


		public virtual void Tick(AbilityHandle handle, float deltaTime)
		{
			handle.AbilityData.CooldownRemaining -= deltaTime;

			if (handle.AbilityData.CooldownRemaining < 0f)
			{
				handle.AbilityData.CooldownRemaining = 0f;
			}
		}
	}
}
