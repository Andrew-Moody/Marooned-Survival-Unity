using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityActor = LegacyAbility.AbilityActor;

[CreateAssetMenu(menuName = "ScriptableObjects/Item/PlaceItemSO")]
public class PlaceItemSO : UseItemSO
{
	public override void UseItem(AbilityActor user, ItemSO itemSO)
	{
		Debug.LogError($"Placing item: {itemSO.ItemName}");

		DestructibleManager.Instance.PlaceItem(user, itemSO.ItemID);
	}
}
