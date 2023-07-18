using UnityEngine;

namespace AbilitySystem
{
	/// <summary>
	/// Used to pass data required to process effects. Can be subclassed to add specific info
	/// </summary>
	public class EffectEventData
	{
		public AbilityActor Source { get; set; }

		public AbilityActor Target { get; set; }
	}
}

