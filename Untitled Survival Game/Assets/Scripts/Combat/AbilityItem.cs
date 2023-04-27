using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityItem
{
	public string ItemName;

	public int ItemID;

	public Ability[] Abilities;


	public AbilityItem(AbilityItemSO abilityItemSO)
	{
		if (abilityItemSO != null)
		{
			ItemName = abilityItemSO.ItemName;

			ItemID = abilityItemSO.ItemID;

			Abilities = abilityItemSO.GetAbilities();
		}
	}
}
