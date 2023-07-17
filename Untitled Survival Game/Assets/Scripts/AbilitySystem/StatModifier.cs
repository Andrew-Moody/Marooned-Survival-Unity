using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[System.Serializable]
	public class StatModifier
	{
		public StatType StatType => _statType;
		[SerializeField] private StatType _statType;

		public ModifierOperation Operation => _operation;
		[SerializeField] private ModifierOperation _operation;

		public float Magnitude => _magnitude;
		[SerializeField] private float _magnitude;



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
