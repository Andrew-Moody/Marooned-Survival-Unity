using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Actors;

namespace AbilitySystem
{
	[System.Serializable]
	public class StatModifier
	{
		[SerializeField] private StatKind _statKind;

		[SerializeField] private ModifierOperation _operation;

		[SerializeField] private float _magnitude;

		[SerializeField] private StatOperation _statOperation;

		
		public void ApplyModifier(Actor source, Actor target)
		{
			if (_operation == ModifierOperation.Custom)
			{
				OperationData data = new BasicOpData() { Value = _magnitude };

				_statOperation.Apply(source, target, data);
			}
			else
			{
				float value = target.Stats.GetStatValue(_statKind);

				value = ApplyModifier(value);

				target.Stats.SetStatValue(_statKind, value);
			}
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
			Override,
			Custom
		}
	}
}
