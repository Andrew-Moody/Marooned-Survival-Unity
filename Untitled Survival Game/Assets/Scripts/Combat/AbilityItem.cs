using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityItem
{
	public string ItemName;

	public int ItemID;

	public Ability[] Abilities;


	public AbilityItem(string itemName, int itemID, Ability[] abilities)
	{
		ItemName = itemName;

		ItemID = itemID;

		Abilities = abilities;
	}
}
