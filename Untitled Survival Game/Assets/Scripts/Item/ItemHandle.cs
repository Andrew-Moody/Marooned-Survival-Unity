using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;
using AbilitySystem;


public class ItemHandle
{
	public ItemSO ItemSO => _itemSO;
	private ItemSO _itemSO;

	public int ItemID => _itemSO.ItemID;

	public List<AbilityHandle> AbilityHandles => _abilityHandles;
	private List<AbilityHandle> _abilityHandles;

	public Inventory Inventory { get => _inventory; set => _inventory = value; }
	private Inventory _inventory;

	public int Slot { get => _slot; set => _slot = value; }
	private int _slot;


	public ItemHandle(ItemSO itemSO, Inventory inventory, int slot)
	{
		_itemSO = itemSO;

		Inventory = inventory;

		Slot = slot;
	}


	public void CreateAbilityHandles(AbilityActor user)
	{
		_abilityHandles = AbilitySystemUtility.CreateAbilityHandles(ItemSO.Abilities, user);
	}


	public bool TryRemoveItem(int quantity)
	{
		return _inventory.RemoveItemsAtSlot(_slot, ItemID, quantity);
	}
}
