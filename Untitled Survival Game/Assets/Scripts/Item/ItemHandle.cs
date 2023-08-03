using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;


public class ItemHandle
{
	public ItemSO ItemSO => _itemSO;
	private ItemSO _itemSO;

	public int ItemID => _itemID;
	private int _itemID;

	public List<AbilityHandle> AbilityHandles => _abilityHandles;
	private List<AbilityHandle> _abilityHandles;
}
