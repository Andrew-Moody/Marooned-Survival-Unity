using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/AbilityItemSO")]
public class AbilityItemSO : ScriptableObject
{
	public string ItemName;

	public int itemID;

	public Ability[] Abilities;

	private void OnValidate()
	{
		for (int i = 0; i < Abilities.Length; i++)
		{
			if (Abilities[i] == null)
			{
				Abilities[i] = new Ability();
			}
			else
			{
				// This is begging for a factory
				if (Abilities[i].GetAbilityType() == AbilityType.Melee && !(Abilities[i] is MeleeAbility))
				{
					Abilities[i] = new MeleeAbility(Abilities[i]);
				}
			}

			Abilities[i].OnValidate();
		}
	}
}
