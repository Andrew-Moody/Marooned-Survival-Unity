using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[System.Serializable]
	public class StatModifier
	{
		public LegacyAbility.StatType StatType => _statType;
		[SerializeField] private LegacyAbility.StatType _statType;

		//public ModifierOperation Operation => _operation;
		[SerializeField] private ModifierOperation _operation;

		//public float Magnitude => _magnitude;
		[SerializeField] private float _magnitude;

		public void ApplyModifier(LegacyAbility.Stats stats)
		{
			float value = stats.GetStatValue(_statType);

			switch (_operation)
			{
				case ModifierOperation.Add:
				{
					value += _magnitude;
					break;
				}
				case ModifierOperation.Multiply:
				{
					value *= _magnitude;
					break;
				}
				case ModifierOperation.Divide:
				{
					value /= _magnitude;
					break;
				}
				case ModifierOperation.Override:
				{
					value = _magnitude;
					break;
				}
			}


			stats.SetStat(_statType, value);
		}

		public enum ModifierOperation
		{
			None,
			Add,
			Multiply,
			Divide,
			Override
		}
	}
}
