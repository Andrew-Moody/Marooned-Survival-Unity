using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Runtime Representation of an Item
/// </summary>
public class InventoryItem
{
	public ItemSO ItemSO => _itemSO;
	private ItemSO _itemSO;

	public int ItemID => _itemSO.ItemID;

	public int Quantity { get => _quantity; set => _quantity = value; }
	private int _quantity;

	public EquipSlot EquipSlot =>_itemSO.equipSlot;

	public bool ConsumeOnUse => _consumeOnUse;
	private bool _consumeOnUse = true;

	public Sprite Sprite => ItemSO.Sprite;


	public int StackSpace => ItemSO.StackLimit - Quantity;

	// Representation of an item for use outside the inventory (mainly AbilitySystem for now) not sure if it should be stored here
	public ItemHandle ItemHandle { get => _itemHandle; set => _itemHandle = value; }
	private ItemHandle _itemHandle;

	

	private List<ContextOption> _options;
	private List<ContextOption> _equipedOptions;


	public InventoryItem(int itemID, int quantity)
		: this(ItemManager.Instance.GetItemSO(itemID))
	{
		Quantity = quantity;
	}


	public InventoryItem(ItemSO itemSO)
	{
		_itemSO = itemSO;
	}


	// Conversion Constructor for convenience
	public InventoryItem(ItemNetData itemNetData) : this(itemNetData.ItemID, itemNetData.Quantity) { }


	public static InventoryItem Empty()
	{
		return new InventoryItem(0, 0);
	}


	public ItemNetData GetNetData()
	{
		return new ItemNetData(ItemID, Quantity);
	}


	public List<ContextOption> GetOptions(int slotIndex)
	{
		if (_options == null)
		{
			_options = new List<ContextOption>();

			List<Options> optCodes = ItemSO.InventoryOptions;

			for (int i = 0; i < optCodes.Count; i++)
			{
				switch(optCodes[i])
				{
					case Options.Examine:
					{
						_options.Add(new ContextOption("Examine " + ItemSO.ItemName, slotIndex, Examine));
						break;
					}
					case Options.Use:
					{
						_options.Add(new ContextOption("Use " + ItemSO.ItemName, slotIndex, Use));
						break;
					}
					case Options.Equip:
					{
						_options.Add(new ContextOption("Equip " + ItemSO.ItemName, slotIndex, Equip));
						break;
					}
					case Options.Drop:
					{
						_options.Add(new ContextOption("Drop " + ItemSO.ItemName, slotIndex, Drop));
						break;
					}
					default:
					{
						break;
					}
				}
			}
		}

		return _options;
	}

	public List<ContextOption> GetEquipedOptions(int slotIndex)
	{
		if (_equipedOptions == null)
		{
			_equipedOptions = new List<ContextOption>();

			List<Options> optCodes = ItemSO.EquipedOptions;

			for (int i = 0; i < optCodes.Count; i++)
			{
				switch (optCodes[i])
				{
					case Options.Unequip:
					{
						_equipedOptions.Add(new ContextOption("Unequip " + ItemSO.ItemName, slotIndex, Unequip));
						break;
					}
					case Options.Teleport:
					{
						_equipedOptions.Add(new ContextOption("Teleport " + ItemSO.ItemName, slotIndex, Teleport));
						break;
					}
					default:
					{
						break;
					}
				}
			}
		}

		return _equipedOptions;
	}

	// TODO Eliminate use of Inventory.ClientInstance there are safer ways to access the players inventory already
	// but while reworking item abilities might add a better way

	public void Examine(int index)
	{
		Debug.Log("Player Examined Item: " + ItemSO.ItemName);

		ChatUI.SendChat(ItemSO.ExamineText);
	}

	public void Use(int index)
	{
		Debug.Log("Player Used Item: " + ItemSO.ItemName);
	}


	public void Equip(int index)
	{
		Debug.Log("Player Equiped Item: " + ItemSO.ItemName);

		Inventory.ClientInstance.EquipItemSRPC(index);
	}


	public void Drop(int index)
	{
		Debug.Log("Player Dropped Item: " + ItemSO.ItemName);

		Inventory.ClientInstance.DropItemSRPC(index);
	}


	public void Unequip(int index)
	{
		Debug.Log("Player Unequiped Item: " + ItemSO.ItemName);

		Inventory.ClientInstance.UnequipItemSRPC(index);
	}


	public void Teleport(int index)
	{
		Debug.Log("Player Teleported with Item: " + ItemSO.ItemName);
	}
}