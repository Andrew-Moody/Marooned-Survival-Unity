using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "NewBasicTargeter", menuName = "AbilitySystem/Targeter/BasicTargeter")]
	public class BasicTargeter : Targeter
	{
		public override List<TargetResult> FindTargets(AbilityActor user, TargetingArgs args)
		{
			// Need to get a layer mask from user, eventually based on traits
			// To exclude as much as possible
			int layerMask = LayerMask.GetMask("Mob");

			// user position is fine for now
			Vector3 position = user.transform.position;

			// Range could be a stat held by the actor set by the current weapon
			// or passed by the ability through TargetingArgs
			float range = 2f;


			Collider[] hits = Physics.OverlapSphere(position, range, layerMask);

			List<TargetResult> targets = new List<TargetResult>();

			// Check each hit and add a target result entry if it is a valid target
			for (int i = 0; i < hits.Length; i++)
			{
				if (hits[i].gameObject.TryGetComponent(out AbilityActor potentialTarget))
				{
					if (IsValidTarget(user, potentialTarget))
					{
						TargetResult result = new TargetResult()
						{ 
							Target = potentialTarget
						};

						targets.Add(result);
					}
				}
			}

			return targets;
		}


		private bool IsValidTarget(AbilityActor user, AbilityActor target)
		{
			// May or may not want to exclude self as a target
			// May need to double check Fishnet objects equality test isn't overridden in a way that this fails
			if (user == target)
			{
				return false;
			}


			// Potentially perform checks based on traits that layers were unable to rule out


			return true;
		}
	}
}
