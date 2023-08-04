using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "InteractAbility", menuName = "AbilitySystem/Ability/InteractAbility")]
	public class InteractAbility : Ability
	{
		[SerializeField] private InteractTargeter targeter;

		public override void Activate(AbilityHandle handle)
		{

			if (!handle.User.IsServer)
			{
				End(handle);
				return;
			}

			Interactable interactable = FindInteractable(handle);

			if (interactable != null)
			{
				interactable.Interact(handle.User.Owner);
				End(handle);
				return;
			}

			// if no interactable is found attempt to use an item instead

			// Haven't worked out best way to chain abilities but this will do
			End(handle);

			handle.User.ActivateAbility(AbilityInput.UseItem);
		}


		private Interactable FindInteractable(AbilityHandle handle)
		{
			TargetingArgs args = new RaycastTargetArgs() { Range = 2f };

			List<TargetResult> results = targeter.FindTargets(handle.User, args);

			if (results.Count > 0 && results[0] is InteractTargetResult result)
			{
				return result.Interactable;
			}

			return null;
		}
	}
}

