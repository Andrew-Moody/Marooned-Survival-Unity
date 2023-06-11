using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : NetworkBehaviour
{

	[SerializeField]
	private int _inventorySize;

	[SerializeField]
	private int _hotbarSize;

	[SerializeField]
	private int _equipmentSize;

	[SerializeField]
	private EquipSlot[] _equipSlots;

	[SerializeField]
	private List<InventoryItem> _items;

	public event EventHandler<SlotUpdateEventArgs> SlotUpdated;

	public event Action OnSyncContents;

	public event Action<int> OnHotbarSelect;

	// Points to the inventory only for the player owned by this client
	public static Inventory ClientInstance;

	private EquipmentController _equipmentController;

	private int _hotbarSelection;


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		if (Owner.IsLocalClient)
		{
			ClientInstance = this;
		}

		if (IsServer || Owner.IsLocalClient)
		{
			Initialize();

			//Debug.LogError("Initialized inventory for Client: " + OwnerId + " AsServer: " + IsServer);
		}
		else
		{
			//Debug.LogError("Non Owning client would  have initialized Inventory " + OwnerId);
		}
	}


	private void Initialize()
	{
		if (_inventorySize == 0)
		{
			Debug.LogWarning("Inventory Size set to zero");
		}

		_items = new List<InventoryItem>(_inventorySize + _equipmentSize);

		for (int i = 0; i < _inventorySize + _equipmentSize; i++)
		{
			_items.Add(InventoryItem.Empty());
		}

		_equipmentController = GetComponent<EquipmentController>();
	}


	private void ClearSlot(int index)
	{
		// Need consitent definition of empty slot
		_items[index] = InventoryItem.Empty();
		
		UpdateSlot(index);
	}


	private void SetItemAtSlot(int index, InventoryItem inventoryItem)
	{
		_items[index] = inventoryItem;

		UpdateSlot(index);
	}

	
	private void UpdateSlot(int index)
	{
		// Check if item needs to be equiped
		if (index >= _inventorySize)
		{
			_equipmentController.EquipItem(_items[index], IndexToSlot(index));
		}
		else if (index == _hotbarSelection)
		{
			_equipmentController.EquipItem(_items[index], EquipSlot.MainHand);
		}


		// Update UI for Owning Client (this is called by OnStartNetwork so IsOwner is always false)
		if (Owner.IsLocalClient)
		{
			SlotUpdateEventArgs args = new SlotUpdateEventArgs(index, _items[index].Quantity, _items[index].Sprite);

			SlotUpdated?.Invoke(this, args);

			if (index == _hotbarSelection)
			{
				OnHotbarSelect?.Invoke(_items[_hotbarSelection].ItemID);
			}
		}
		else

		if (!Owner.IsLocalClient && !IsServer)
		{
			Debug.LogError("Calling UpdateSlot is not needed on Observing Clients");
		}
	}


	public void UpdateSlots()
	{
		//Debug.LogError("Updating all slots");

		for (int i = 0; i < _inventorySize + _equipmentSize; i++)
		{
			UpdateSlot(i);
		}
	}

  
	// Inventory Operations
	#region Operations


	[ServerRpc(RunLocally = true)]
	public void EquipItemSRPC(int currentSlot)
	{
		EquipSlot equipSlot = _items[currentSlot].EquipSlot;

		if (equipSlot == EquipSlot.None)
		{
			Debug.LogError("Item has an Equip option but the EquipSlot was set to None");
			return;
		}

		int toSlot = -1;

		for (int i = 0; i < _equipmentSize; i++)
		{
			if (equipSlot == _equipSlots[i])
			{
				if (_items[i + _inventorySize].ItemID == 0)
				{
					// Found Empty slot with matching equipment type
					toSlot = i + _inventorySize;
					break;
				}

				if (toSlot == -1)
				{
					// Found first slot with matching equipment type (not empty)
					toSlot = i + _inventorySize;
				}
			}
		}

		if (toSlot != -1)
		{
			SwapSlots(currentSlot, toSlot);
		}
	}


	[ServerRpc(RunLocally = true)]
	public void UnequipItemSRPC(int currentSlot)
	{
		int emptySlot = FirstEmptySlot();

		if (emptySlot != -1)
		{
			SwapSlots(currentSlot, emptySlot);
		}
	}


	[ServerRpc(RunLocally = true)]
	public void UseItemSRPC(int currentSlot)
	{
		Debug.Log($"Player used {_items[currentSlot].ItemSO.ItemName} in slot: {currentSlot}");
	}


	[ServerRpc(RunLocally = true)]
	public void DropItemSRPC(int currentSlot)
	{
		ClearSlot(currentSlot);
	}


	[ServerRpc(RunLocally = true)]
	public void SetHotbarSelectionSRPC(int selection)
	{
		_hotbarSelection = selection;

		UpdateSlot(_hotbarSelection);
	}


	[ServerRpc(RunLocally = true)]
	public void SwapSlotsSRPC(int currentSlot, int prevSlot)
	{
		SwapSlots(currentSlot, prevSlot);
	}


	#endregion


	[Server]
	public bool TryTakeItem(ref ItemNetData itemNetData)
	{
		Debug.LogWarning("TryTakeItem for Client: " + OwnerId);

		int firstEmpty = -1;

		for (int i = 0; i < _inventorySize; i++)
		{
			// Find first empty slot
			if (firstEmpty == -1 && _items[i].ItemID == 0)
			{
				firstEmpty = i;
			}

			
			// Try to add some or all to an existing stack
			if (itemNetData.ItemID == _items[i].ItemID && _items[i].StackSpace > 0)
			{
				int transferAmount = itemNetData.Quantity;

				if (transferAmount > _items[i].StackSpace)
				{
					transferAmount = _items[i].StackSpace;
				}

				_items[i].Quantity += transferAmount;
				itemNetData.Quantity -= transferAmount;

				UpdateSlot(i);
				TargetSyncSlot(Owner, _items[i].GetNetData(), i);

				// Item was consumed by existing stack
				if (itemNetData.Quantity == 0)
				{
					return true;
				}
			}
		}

		// If the item wasnt consumed by existing stacks add it to the first empty slot
		if (firstEmpty != -1)
		{
			_items[firstEmpty] = new InventoryItem(itemNetData);
			itemNetData.Quantity = 0;

			UpdateSlot(firstEmpty);
			TargetSyncSlot(Owner, _items[firstEmpty].GetNetData(), firstEmpty);
			return true;
		}


		return false;
	}


	[Server]
	public void UseItem(AbilityActor user, AbilityItem abilityItem)
	{
		if (_items[_hotbarSelection].ItemID != abilityItem.ItemID)
		{
			Debug.LogError("Attempted to use AbilityActor item not selected by hotbar");
			return;
		}

		InventoryItem item = _items[_hotbarSelection];

		Debug.LogError("Using Item");

		if (item.ItemSO.PlacedItemID != 0)
		{
			DestructibleManager.Instance.PlaceItem(user, item.ItemSO.PlacedItemID);
		}

		if (item.ConsumeOnUse)
		{
			item.Quantity -= 1;

			if (item.Quantity == 0)
			{
				_items[_hotbarSelection] = InventoryItem.Empty();
			}

			UpdateSlot(_hotbarSelection);
			TargetSyncSlot(Owner, _items[_hotbarSelection].GetNetData(), _hotbarSelection);
		}
	}


	public List<ItemNetData> GetContents()
	{
		List<ItemNetData> contents = new List<ItemNetData>();

		for (int i = 0; i < _items.Count; i++)
		{
			contents.Add(_items[i].GetNetData());
		}

		return contents;
	}


	public void SetContents(List<ItemNetData> items)
	{
		for (int i = 0; i < _items.Count; i++)
		{
			_items[i] = new InventoryItem(items[i]);
			UpdateSlot(i);
		}
	}


	public List<ContextOption> GetItemOptions(int index)
	{
		if (_items[index].ItemID != 0)
		{
			if (index < _inventorySize)
			{
				return _items[index].GetOptions(index);
			}
			else
			{
				return _items[index].GetEquipedOptions(index);
			}
		}

		return null;
	}


	[TargetRpc]
	private void TargetSyncContents(NetworkConnection connection, List<ItemNetData> items)
	{
		SetContents(items);
		OnSyncContents?.Invoke();
	}


	[TargetRpc]
	private void TargetSyncSlot(NetworkConnection connection, ItemNetData item, int index)
	{
		_items[index] = new InventoryItem(item);
		UpdateSlot(index);
	}

	// Utility Functions
	#region Utility


	public bool HasItem(out int slot, int itemID, int quantity = 1)
	{
		for (int i = 0; i < _items.Count; i++)
		{
			if (_items[i].ItemID == itemID && _items[i].Quantity >= quantity)
			{
				slot = i;
				return true;
			}
		}

		slot = -1;
		return false;
	}


	private int FirstEmptySlot()
	{
		int firstEmptySlot = -1;

		for (int i = 0; i < _inventorySize; i++)
		{
			if (_items[i].ItemID == 0)
			{
				firstEmptySlot = i;
				break;
			}
		}

		return firstEmptySlot;
	}


	private bool IsValidMove(int fromSlot, int toSlot)
	{
		EquipSlot equipSlot = IndexToSlot(toSlot);

		return (equipSlot == EquipSlot.None || _items[fromSlot].ItemID == 0 || equipSlot == _items[fromSlot].EquipSlot);
	}


	private EquipSlot IndexToSlot(int index)
	{
		if (index < _inventorySize)
		{
			return EquipSlot.None;
		}

		return _equipSlots[index - _inventorySize];
	}


	private void SwapSlots(int currentSlot, int prevSlot)
	{
		if (!IsValidMove(currentSlot, prevSlot) || !IsValidMove(prevSlot, currentSlot))
		{
			return;
		}

		Debug.LogError($"Swapping {_items[currentSlot].ItemID} in Slot {currentSlot} with {_items[prevSlot].ItemID} in slot {prevSlot}");

		InventoryItem tempItem = _items[currentSlot];

		_items[currentSlot] = _items[prevSlot];

		_items[prevSlot] = tempItem;

		UpdateSlot(currentSlot);
		UpdateSlot(prevSlot);
	}


	[Server]
	public bool RemoveItemsAtSlot(int slot, int itemID, int quantity)
	{
		if (_items[slot].ItemID != itemID)
		{
			Debug.LogError($"Failed to remove item at slot {slot}, desired itemID {itemID} did not match existing ID {_items[slot].ItemID}");
			return false;
		}

		if (quantity > _items[slot].Quantity)
		{
			Debug.LogError($"Failed to remove item at slot {slot}, desired quantity {quantity} was greater than existing quantity {_items[slot].Quantity}");
			return false;
		}

		if (_items[slot].Quantity == quantity)
		{
			_items[slot] = InventoryItem.Empty();
		}
		else
		{
			_items[slot].Quantity -= quantity;
		}

		UpdateSlot(slot);
		TargetSyncSlot(Owner, _items[slot].GetNetData(), slot);
		return true;
	}


	#endregion
}

public class SlotUpdateEventArgs : EventArgs
{
	public int Index;
	public int Count;
	public Sprite Sprite;

	public SlotUpdateEventArgs(int index, int count, Sprite sprite)
	{
		Index = index;
		Count = count;
		Sprite = sprite;
	}
}
