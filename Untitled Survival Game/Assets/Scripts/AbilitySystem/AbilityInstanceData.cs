using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	// Used to store runtime data for specific instance of an ability
	// Will need to be derived from for new ability types
	public class AbilityInstanceData
	{
		public AbilityActor User { get; set; }

		public float CooldownRemaining { get; set; }

		public AsyncTasks.IAsyncTask Task { get; set; }
	}
}
