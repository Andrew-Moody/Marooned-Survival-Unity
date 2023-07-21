using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

namespace AbilitySystem
{
	[System.Serializable]
	public class StatModifier
	{
		[SerializeField] private StatKind _statKind;

		[SerializeField] private ModifierOperation _operation;

		[SerializeField] private float _magnitude;

		
		public void ApplyModifier(Stats stats)
		{
			float value = stats.GetStatValue(_statKind);

			value = ApplyModifier(value);

			stats.SetStatValue(_statKind, value);
		}


		private float ApplyModifier(float value)
		{
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

			return value;
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
