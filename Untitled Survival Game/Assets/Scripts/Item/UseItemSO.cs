using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemSO : ScriptableObject
{
	public virtual void UseItem(AbilitySystem.AbilityActor user, ItemSO itemSO)
	{
		Debug.LogError($"Used item: {itemSO.ItemName}");
	}
}
