using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "PlacementTargeter", menuName = "AbilitySystem/Targeter/PlacementTargeter")]
	public class PlacementTargeter : Targeter
	{
		public override List<TargetResult> FindTargets(AbilityActor user, TargetingArgs args)
		{
			List<TargetResult> targets = new List<TargetResult>();

			Transform view = user.Actor.ViewTransform.transform;

			if (view == null)
			{
				Debug.LogError("Failed to find interact targets: Actor is missing a viewTransform");
				return targets;
			}

			float range = 2f;

			if (args is RaycastTargetArgs raycastArgs)
			{
				range = raycastArgs.Range;
			}

			if (Physics.Raycast(view.position, view.forward, out RaycastHit hit, range, LayerMask.GetMask("Ground")))
			{
				TargetResult result = new PointNormalTargetResult()
				{
					Point = hit.point,
					Normal = hit.normal
				};

				targets.Add(result);
			}

			return targets;
		}
	}
}
