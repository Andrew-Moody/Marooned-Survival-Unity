using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ITaskUser = AsyncTasks.ITaskUser;

namespace AbilitySystem
{
	// The runtime representation of an ability
	// Allows the ability scriptable object to define default values and behavior.
	// Per instance runtime data (cooldown remaining for example) is held separatly so that
	// a new instance of ability SO does not need to be instantiated
	public class AbilityHandle : ITaskUser
	{
		private Ability _ability;

		public AbilityInstanceData AbilityData => _abilityData;
		private AbilityInstanceData _abilityData;

		public AbilityHandle(Ability ability, AbilityActor user)
		{
			_ability = ability;

			_abilityData = _ability.CreateInstanceData(user);
		}

		public AbilityHandle(Ability ability, AbilityActor user, AbilityEventData data)
		{
			_ability = ability;

			_abilityData = _ability.CreateInstanceData(user);

			_abilityData.AbilityEventData = data;
		}

		public bool CanActivate()
		{
			return _ability.CanActivate(this);
		}


		public void Activate()
		{
			_ability.Activate(this);
		}


		public void Cancel()
		{
			_ability.Cancel(this);
		}


		public void Tick(float deltaTime)
		{
			_ability.Tick(this, deltaTime);
		}
	}
}
