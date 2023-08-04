using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	public static class AbilitySystemUtility
	{
		public static List<AbilityHandle> CreateAbilityHandles(List<AbilityInputBinding> bindings, AbilityActor user)
		{
			List<AbilityHandle> handles = new List<AbilityHandle>();

			foreach (AbilityInputBinding binding in bindings)
			{
				AbilityHandle handle = new AbilityHandle(binding.Ability, user, binding.Input);

				handles.Add(handle);
			}

			return handles;
		}
	}
}
