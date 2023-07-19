using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	public class Stats : NetworkBehaviour
	{
		[SerializeField]
		private List<StatInitialValue> _statInitialValues;


		private Dictionary<AbilityTrait, Stat> _stats;


		private void Initialize()
		{
			foreach (StatInitialValue value in _statInitialValues)
			{
				if (_stats.ContainsKey(value.StatTrait))
				{
					Debug.LogWarning($"{gameObject} Initial stats contains multiple entries for trait {value.StatTrait}");
				}
				else
				{
					Stat stat = new Stat();
					stat.SetBounds(value.MinValue, value.MaxValue);
					stat.SetStatClamped(value.InitialValue);

					_stats[value.StatTrait] = stat;
				}
			}
		}


		[System.Serializable]
		private struct StatInitialValue
		{
			public AbilityTrait StatTrait;

			public float MinValue;

			public float MaxValue;

			public float InitialValue;
		}
	}

	
}
