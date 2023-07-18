using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityTraits
{
	[SerializeField]
	private AbilityTrait[] _traits;

	// Match functions are O(m x n) which may warrant a dictionary implementation
	// if the number of traits becomes large enough to impact performance

	public bool ContainsTrait(AbilityTrait trait)
	{
		foreach (AbilityTrait traitToCheck in _traits)
		{
			if (trait == traitToCheck)
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


	
}

public enum AbilityTrait
{
	None,

	Damage,

	// Damage class
	Melee,
	Range,
	Magic,

	// Tool traits
	Mining,
	Logging,
}