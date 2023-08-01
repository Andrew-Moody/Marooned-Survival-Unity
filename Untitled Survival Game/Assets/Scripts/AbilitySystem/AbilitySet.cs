using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	public class AbilitySet
	{
		private AbilityActor _user;


		// Instantiated ability instances
		private Dictionary<AbilityInput, AbilityHandle> _abilities;

		// Overide ability input binding
		private Dictionary<AbilityInput, AbilityHandle> _abilityOverrides;


		public AbilitySet(AbilityActor user, List<AbilityInputBinding> startingAbilities)
		{
			_user = user;

			SetStartingAbilities(startingAbilities);
		}


		public void SetStartingAbilities(List<AbilityInputBinding> abilities)
		{
			_abilities = new Dictionary<AbilityInput, AbilityHandle>();

			foreach (AbilityInputBinding binding in abilities)
			{
				AbilityHandle handle = new AbilityHandle(binding.Ability, _user, binding.Input);

				_abilities.Add(binding.Input, handle);
			}

			_abilityOverrides = new Dictionary<AbilityInput, AbilityHandle>();
		}


		public bool TryFindAbility(AbilityInput abilityInput, out AbilityHandle abilityHandle)
		{
			if (_abilityOverrides.TryGetValue(abilityInput, out abilityHandle))
			{
				return true;
			}

			return _abilities.TryGetValue(abilityInput, out abilityHandle);
		}


		public void SetAbilityOverride(AbilityHandle abilityHandle)
		{
			_abilityOverrides[abilityHandle.InputBinding] = abilityHandle;
		}


		public void RemoveAbilityOverride(AbilityHandle abilityHandle)
		{
			_abilityOverrides.Remove(abilityHandle.InputBinding);
		}
	}
}
