using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actors
{
	public class ActorStat
	{
		public StatKind StatKind => _statKind;

		public float Value => _currentValue;

		public float MinValue => _minValue;

		public float MaxValue => _maxValue;


		private StatKind _statKind;

		private float _minValue;

		private float _maxValue;

		private float _currentValue;


		public ActorStat() { }

		public ActorStat(ActorStat stat)
		{
			_statKind = stat._statKind;

			_currentValue = stat._currentValue;

			_minValue = stat._minValue;

			_maxValue = stat._maxValue;
		}


		/// <summary>
		/// Sets the new value of stat clamped between the min and max bounds
		/// </summary>
		public void SetStatClamped(float value)
		{
			_currentValue = Clamp(value);
		}


		/// <summary>
		/// Sets the bounds of the stat and clamps the current value to the new bounds
		/// </summary>
		public float SetBounds(float min, float max)
		{
			_minValue = min;
			_maxValue = max;

			_currentValue = Clamp(_currentValue);

			return _currentValue;
		}


		public UIEventData GetUIData()
		{
			return new UIFloatChangeEventData()
			{
				TagString = _statKind.ToString(),
				Value = _currentValue,
				MinValue = _minValue,
				MaxValue = _maxValue
			};
		}


		private float Clamp(float value)
		{
			return Mathf.Clamp(value, _minValue, _maxValue);
		}
	}
}
