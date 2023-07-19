using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	// Used to return the result of a targeting action
	// Additional data may be added or may just have to derive custom result classes

	public class TargetResult
	{
		public AbilityActor Target { get; set; }
	}
}
