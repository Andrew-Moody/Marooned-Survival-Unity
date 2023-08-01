using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	// Used by abilites to pass custom data to targeting calculations

	public class TargetingArgs { }

	public class BasicTargetingArgs : TargetingArgs
	{
		public Vector3 Position { get; set; }

		public float Range { get; set; }
	}


	public class RaycastTargetArgs : TargetingArgs
	{
		public float Range { get; set; }
	}
}
