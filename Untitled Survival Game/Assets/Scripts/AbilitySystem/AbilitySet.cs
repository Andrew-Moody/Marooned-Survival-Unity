using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	public class AbilitySet
	{
		private AbilityActor _user;


		// Instantiated ability instances
		private Dictionary<AbilityInput, AbilityHandle> _abilityDefaults;

		// Overide ability input binding
		private Dictionary<AbilityInput, AbilityHandle> _abilityOverrides;


		public AbilitySet(AbilityActor user, List<AbilityInputBinding> defaultAbilities)
		{
			_user = user;

			SetDefaultAbilities(defaultAbilities);
		}


		public void SetDefaultAbilities(List<AbilityInputBinding> abilities)
		{
			_abilityDefaults = new Dictionary<AbilityInput, AbilityHandle>();

			foreach (AbilityInputBinding binding in abilities)
			{
				AbilityHandle handle = new AbilityHandle(binding.Ability, _user, binding.Input);

				_abilityDefaults.Add(binding.Input, handle);
			}

			_abilityOverrides = new Dictionary<AbilityInput, AbilityHandle>();
		}


		public bool TryFindAbility(AbilityInput abilityInput, out AbilityHandle abilityHandle)
		{
			if (_abilityOverrides.TryGetValue(abilityInput, out abilityHandle))
			{
				return true;
			}

			return _abilityDefaults.TryGetValue(abilityInput, out abilityHandle);
		}


		public void SetAbilityOverride(AbilityHandle handle)
		{
			_abilityOverrides[handle.InputBinding] = handle;
		}


		public void RemoveAbilityOverride(AbilityHandle handle)
		{
			_abilityOverrides.Remove(handle.InputBinding);
		}


		public void SetAbilityOverrides(List<AbilityHandle> handles)
		{
			foreach (AbilityHandle handle in handles)
			{
				_abilityOverrides[handle.InputBinding] = handle;
			}
		}


		public void RemoveAbilityOverrides(List<AbilityHandle> handles)
		{
			foreach (AbilityHandle handle in handles)
			{
				_abilityOverrides.Remove(handle.InputBinding);
			}
		}
	}
}
