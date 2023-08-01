using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "PlaceItemAbility", menuName = "AbilitySystem/Ability/PlaceItemAbility")]
	public class PlaceItemAbility : Ability
	{
		[SerializeField] private PlacementTargeter targeter;

		public override void Activate(AbilityHandle handle)
		{

			if (!handle.AbilityData.User.IsServer)
			{
				End(handle);
				return;
			}

			PointNormalTargetResult pointNormal = FindSpotOnGround(handle);

			if (pointNormal != null)
			{
				//interactable.Interact(handle.AbilityData.User.Owner);
			}

			End(handle);
		}


		private PointNormalTargetResult FindSpotOnGround(AbilityHandle handle)
		{
			TargetingArgs args = new RaycastTargetArgs() { Range = 2f };

			List<TargetResult> results = targeter.FindTargets(handle.AbilityData.User, args);

			if (results.Count > 0 && results[0] is PointNormalTargetResult result)
			{
				return result;
			}

			return null;
		}
	}
}

