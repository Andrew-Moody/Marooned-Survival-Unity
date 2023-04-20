using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : NetworkBehaviour
{

    [SerializeField]
    private int Size;

    [SerializeField]
    private List<InventoryItem> _items;

	public event EventHandler<SlotUpdateEventArgs> SlotUpdated;

    // Points to the inventory only for the player owned by this client
    public static Inventory ClientInstance;


	private void Awake()
	{
        // This really doesnt work well given there are multiple instances on the server (See OnStartNetwork)
        // Instance = this;
	}


	void Start()
    {
        Debug.Log("Inventory start called");
        
    }


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();
        if (IsServer || Owner.IsLocalClient)
        {
            ClientInstance = this;

            Initialize();

            Debug.Log("Initialized inventory for Client: " + OwnerId + " AsServer: " + IsServer);
        }
		else
		{
            Debug.Log("Non Owning client would  have initialized Inventory " + OwnerId);
		}
    }


	private void Initialize()
	{
        if (Size == 0)
		{
            Debug.LogWarning("Inventory Size set to zero");
		}
        _items = new List<InventoryItem>(Size + 4);

        for (int i = 0; i < Size + 5; i++)
		{
            _items.Add(InventoryItem.Empty());
		}
    }


    public void UseItem(int index)
	{
        Debug.Log("Player used item in slot: " + index);
	}


    public void ClearSlot(int index)
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

    // Raise an event on update to slot contents
    private void UpdateSlot(int index)
	{
        SlotUpdateEventArgs e = new SlotUpdateEventArgs();
        e.Index = index;

		if (_items[index].ItemSO != null)
		{
			e.sprite = _items[index].ItemSO.Sprite;
		}
		else
		{
			e.sprite = null;
		}

		e.count = _items[index].Quantity;

		SlotUpdated?.Invoke(this, e);
    }


    [ServerRpc]
    public void ServerSwapSlots(int indexA, int indexB)
	{
        if (indexA >= Size && indexB >= Size)
		{
            // Cant swap between equipment slots unless slots are made non unique (ie. Terraria)
            // Still have to sync to avoid unhiding early
            TargetSyncContents(Owner, GetContents());
            return;
        }


        if (indexA >= Size || indexB >= Size)
		{
            int currEquip;
            int toEquip;

            if (indexA >= Size )
			{
                currEquip = indexA;
                toEquip = indexB;
			}
			else
			{
                currEquip = indexB;
                toEquip = indexA;
            }

            if (_items[toEquip].ItemID == 0)
            {
                gameObject.GetComponent<EquipmentController>().ServerUnequipItem(currEquip - Size);
            }
            else if (_items[toEquip].ItemSO.equipSlot == (currEquip - Size))
			{
                gameObject.GetComponent<EquipmentController>().ServerEquipItem(_items[toEquip].GetNetData());
            }
			else
			{
                // Cant swap an equiped item for an item that cant be equiped in the same slot
                // Still have to sync to avoid unhiding early
                TargetSyncContents(Owner, GetContents());
                return;
			}
        }

        InventoryItem tempItem = _items[indexA];

        _items[indexA] = _items[indexB];

        _items[indexB] = tempItem;

        TargetSyncContents(Owner, GetContents());
	}



    public void EquipItem(int index)
	{
        int equipSlot = _items[index].ItemSO.equipSlot + Size;

        if (equipSlot < Size)
		{
            return;
		}

        // Equip Item
        //gameObject.GetComponent<EquipmentController>().EquipItem(_items[index].GetNetData());

        ServerSwapSlots(index, equipSlot);
	}


    public void UnequipItem(int index)
	{
        int firstFreeSlot = -1;

        for (int i = 0; i < Size; i++)
		{
            if (_items[i].ItemID == 0)
			{
                firstFreeSlot = i;
                break;
			}
		}

        if (firstFreeSlot != -1)
		{
            gameObject.GetComponent<EquipmentController>().ServerUnequipItem(index - Size);
            ServerSwapSlots(index, firstFreeSlot);
        }
	}


    public void DropItem(int index)
	{
        ServerRemoveItem(index);
	}


    [ServerRpc]
    private void ServerRemoveItem(int index)
	{
        _items[index] = InventoryItem.Empty();

        TargetSyncContents(Owner, GetContents());
	}


    


    [Server]
    public bool TryTakeItem(ref ItemNetData itemNetData)
	{
        Debug.LogError("TryTakeItem for Client: " + OwnerId);

        bool itemTaken = false;

        int firstEmpty = -1;

        for (int i = 0; i < Size; i++)
        {
            // Find first empty slot
            if (firstEmpty == -1 && _items[i].ItemID == 0)
			{
                firstEmpty = i;
			}

            // Try to add item to an existing stack
            if (itemNetData.ItemID == _items[i].ItemID)
			{
                InventoryItem existingItem = _items[i];

				int space = existingItem.ItemSO.StackLimit - existingItem.Quantity;

				if (space > 0)
				{
					// Transfer some or all of the new stack to the existing stack
					if (itemNetData.Quantity <= space)
					{
						// Transfer all
						existingItem.Quantity += itemNetData.Quantity;
						itemNetData.Quantity = 0;

						_items[i] = existingItem;
						itemTaken = true;
                        //UpdateSlot(i);
						return itemTaken;
					}
					else
					{
						// Transfer some
						existingItem.Quantity += space;
						itemNetData.Quantity -= space;

						_items[i] = existingItem;
                        //UpdateSlot(i);
                    }
				}
			}
		}

        // If the item wasnt consumed by existing stacks add it to the first empty slot
        if (!itemTaken && firstEmpty != -1)
		{
            _items[firstEmpty] = new InventoryItem(itemNetData);
            itemNetData.Quantity = 0;
            itemTaken = true;
            //UpdateSlot(firstEmpty);
        }

        return itemTaken;
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
            if (index < Size)
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
    public void TargetSyncContents(NetworkConnection connection, List<ItemNetData> items)
	{
        SetContents(items);
	}


    public ItemSO GetItemSO(int slot)
	{
        return _items[slot].ItemSO;
	}


 //   [System.Serializable]
 //   private struct InventoryItem
	//{
 //       public int ItemID;
 //       public int Quantity;
 //       public ItemSO ItemSO;

 //       public static InventoryItem Empty;

 //       public InventoryItem(int itemID, int quantity)
 //       {
 //           ItemID = itemID;
 //           Quantity = quantity;

 //           if (ItemID != 0)
 //           {
 //               ItemSO = ItemManager.Instance.GetItemSO(ItemID);
 //           }
 //           else
 //           {
 //               ItemSO = null;
 //           }
 //       }


 //       public InventoryItem(ItemNetData itemNetData)
	//	{
 //           ItemID = itemNetData.ItemID;
 //           Quantity = itemNetData.Quantity;

 //           if (ItemID != 0)
 //           {
 //               ItemSO = ItemManager.Instance.GetItemSO(ItemID);
 //           }
 //           else
 //           {
 //               ItemSO = null;
 //           }
 //       }


 //       public ItemNetData GetNetData()
	//	{
 //           return new ItemNetData(ItemID, Quantity);
	//	}


 //       static InventoryItem()
	//	{
 //           Empty = new InventoryItem(0, 0);
	//	}
 //   }
}

public class SlotUpdateEventArgs : EventArgs
{
    public int Index;
    public int count;
    public Sprite sprite;
}