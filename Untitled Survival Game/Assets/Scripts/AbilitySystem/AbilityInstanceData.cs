using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	// Used to store runtime data for a specific instance of an ability
	public class AbilityInstanceData { }


	public class ProjectileAbilityData : AbilityInstanceData
	{
		public ProjectileBase Projectile { get; set; }
	}


	public class SequenceAbilityData : AbilityInstanceData
	{
		public int SequenceIndex { get; set; }

		public List<AbilityHandle> AbilityHandles { get; set; }
	}
}
