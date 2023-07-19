using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	public class Stat
	{
		public float Value => _currentValue;


		private float _minValue;

		private float _maxValue;

		private float _currentValue;


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


		private float Clamp(float value)
		{
			return Mathf.Clamp(value, _minValue, _maxValue);
		}
	}
}
