using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AbilitySO")]
public class AbilitySO : ScriptableObject
{
	public Ability Ability;


	private void OnValidate()
	{
		if (Ability == null)
		{
			Ability = new Ability();
		}
		else
		{
			// This is begging for a factory
			if (Ability.GetAbilityType() == AbilityType.Melee && !(Ability.GetType() == typeof(MeleeAbility)))
			{
				Ability = new MeleeAbility(Ability);
			}
			else if (Ability.GetAbilityType() == AbilityType.None && !(Ability.GetType() == typeof(Ability)))
			{
				Ability = new Ability(Ability);
			}
		}

		Ability.OnValidate();
	}
}