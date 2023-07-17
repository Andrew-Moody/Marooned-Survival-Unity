using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	public class Ability : ScriptableObject
	{
		// Override these functions to define a new ability behavior
		public virtual bool CanActivate(AbilityInstanceData data) { return true; }


		public virtual void Activate(AbilityInstanceData data) { }


		public virtual void Cancel(AbilityInstanceData data) { }


		public virtual void End(AbilityInstanceData data) { }


		public virtual AbilityInstanceData CreateInstanceData(AbilityActor user)
		{
			// Derive from AbilityInstanceData for more complex abilities
			return new AbilityInstanceData() { User = user };
		}
	}
}
