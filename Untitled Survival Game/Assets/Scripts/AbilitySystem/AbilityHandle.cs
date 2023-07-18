using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	// The runtime representation of an ability
	// Allows the ability scriptable object to define default values and behavior.
	// Per instance runtime data (cooldown remaining for example) is held separatly so that
	// a new instance of ability SO does not need to be instantiated
	public class AbilityHandle
	{
		private Ability _ability;

		private AbilityInstanceData _abilityData;

		public AbilityHandle(Ability ability, AbilityActor user)
		{
			_ability = ability;

			_abilityData = _ability.CreateInstanceData(user);
		}

		public bool CanActivate()
		{
			return _ability.CanActivate(_abilityData);
		}


		public void Activate()
		{
			_ability.Activate(_abilityData);
		}


		public void Cancel()
		{
			_ability.Cancel(_abilityData);
		}


		public void Tick(float deltaTime)
		{
			_ability.Tick(_abilityData, deltaTime);
		}
	}
}
