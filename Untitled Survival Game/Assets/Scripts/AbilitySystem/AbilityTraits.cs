using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[System.Serializable]
	public class AbilityTraits
	{
		[SerializeField]
		private List<AbilityTrait> _traits;

		// Match functions are O(m x n) which may warrant a dictionary implementation
		// if the number of traits becomes large enough to impact performance

		public bool ContainsTrait(AbilityTrait trait)
		{
			foreach (AbilityTrait traitToCheck in _traits)
			{
				if (trait.Matches(traitToCheck))
				{
					return true;
				}
			}

			return false;
		}


		public bool MatchAny(AbilityTraits traits)
		{
			foreach (AbilityTrait trait in _traits)
			{
				if (traits.ContainsTrait(trait))
				{
					return true;
				}
			}

			return false;
		}


		public bool MatchAll(AbilityTraits traits)
		{
			foreach (AbilityTrait trait in _traits)
			{
				if (traits.ContainsTrait(trait))
				{
					return false;
				}
			}

			return true;
		}


		public bool MatchAny(AbilityTrait[] traits)
		{
			foreach (AbilityTrait trait in traits)
			{
				if (ContainsTrait(trait))
				{
					return true;
				}
			}

			return false;
		}


		public bool MeetsAllRequirements(AbilityTrait[] traits)
		{
			foreach (AbilityTrait reqTrait in _traits)
			{
				if (!ContainsTrait(traits, reqTrait))
				{
					return false;
				}
			}

			return true;
		}


		public bool ContainsTrait(AbilityTrait[] traits, AbilityTrait traitToMatch)
		{
			foreach (AbilityTrait trait in traits)
			{
				if (traitToMatch == trait)
				{
					return true;
				}
			}

			return false;
		}


		public void AddTraits(AbilityTraits traits)
		{
			foreach (AbilityTrait trait in traits._traits)
			{
				_traits.Add(trait);
			}
		}
	}
}
