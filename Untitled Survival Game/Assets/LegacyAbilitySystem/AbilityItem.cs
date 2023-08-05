using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyAbility
{
	public class AbilityItem
	{
		public string ItemName;

		public int ItemID;

		public LegacyAbility.Ability[] Abilities;

		public ItemSO ItemSO;


		public AbilityItem(ItemSO itemSO)
		{
			ItemSO = itemSO;

			ItemName = itemSO.ItemName;

			ItemID = itemSO.ItemID;

			//Abilities = itemSO.GetRuntimeAbilities();
		}
	}
}
