using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	public class Targeter : ScriptableObject
	{
		public virtual List<TargetResult> FindTargets(AbilityActor user, TargetingArgs args)
		{
			return new List<TargetResult>();
		}
	}
}