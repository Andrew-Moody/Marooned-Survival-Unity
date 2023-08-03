using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	public delegate void AbilityEventHandler(AbilityHandle handle, AbilityEventData data);

	public class AbilityEventData
	{
		public AbilityActor Target { get; set; }
	}


	public class ItemActivateEventData
	{
		public ItemHandle Item { get; set; }
	}
}

