using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Runtime Representation of an Item
/// </summary>
public class InventoryItem
{
	public int ItemID;

	public int Quantity;

	public EquipSlot EquipSlot;

	public bool ConsumeOnUse = true;

	public Sprite Sprite;

	public ItemSO ItemSO;

	public AbilityItem AbilityItem;


	public int StackSpace { get { return (ItemSO.StackLimit - Quantity); } }

	

	private List<ContextOption> _options;
	private List<ContextOption> _equipedOptions;


	public InventoryItem(int itemID, int quantity)
		: this(ItemManager.Instance.GetItemSO(itemID))
	{
		ItemID = itemID;
		Quantity = quantity;
	}


	public InventoryItem(ItemSO itemSO)
	{
		ItemSO = itemSO;

		if (ItemSO == null)
		{
			return;
		}

		EquipSlot = itemSO.equipSlot;
		Sprite = itemSO.Sprite;

		if (itemSO.AbilityItemSO != null)
		{
			AbilityItem = new AbilityItem(itemSO.AbilityItemSO);
		}
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



	public void Examine(int index)
	{
		Debug.Log("Player Examined Item: " + ItemSO.ItemName);

		ChatBox.Instance.SendChat(ItemSO.ExamineText);
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