using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "InteractTargeter", menuName = "AbilitySystem/Targeter/InteractTargeter")]
	public class InteractTargeter : Targeter
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

			if (Physics.Raycast(view.position, view.forward, out RaycastHit hit, range, LayerMask.GetMask("Mob")))
			{
				if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out Interactable interactible))
				{
					TargetResult result = new InteractTargetResult()
					{
						Interactable = interactible
					};

					targets.Add(result);
				}
			}

			return targets;
		}
	}
}
