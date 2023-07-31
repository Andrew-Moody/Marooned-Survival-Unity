using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[System.Serializable]
	public class AbilityTrait
	{
		[SerializeField]
		private AbilityTraitValue _trait;

		public bool Matches(AbilityTrait trait)
		{
			return _trait == trait._trait;
		}

		public int GetTraitKey()
		{
			return _trait.GetHashCode();
		}


		public override string ToString()
		{
			return _trait.ToString();
		}
	}


	public enum AbilityTraitValue
	{
		None,
		Death,
		Damage,

		// Damage class
		Melee,
		Range,
		Magic,

		// Tool traits
		Mining,
		Logging,

		// Stat traits
		Health,
		MagicEnergy
	}
}

