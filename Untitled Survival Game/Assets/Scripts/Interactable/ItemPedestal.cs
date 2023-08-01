using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPedestal : Interactable
{
	[SerializeField]
	private Transform _itemHolder;

	public event System.Action<NetworkConnection, ItemPedestal> ItemPlacedEvent;

	private InventoryItem _item;
	public InventoryItem Item => _item;


	public override void OnStartClient()
	{
		base.OnStartClient();

		Initialize();
	}


	private void Initialize()
	{
		_interactPrompt = "Place Item";

		if (_item == null)
		{
			_item = InventoryItem.Empty();
		}
	}


	public override void Interact(NetworkConnection user)
	{
		base.Interact(user);

		GameObject playerObject = GameManager.Instance.GetPlayer(user);

		Inventory inventory = playerObject.GetComponentInChildren<Inventory>();

		//Debug.LogError($"User first object {user.FirstObject.name}"); Not reliable
		//Inventory inventory = user.FirstObject.GetComponentInChildren<Inventory>();

		if (inventory != null)
		{
			inventory.SwapWithItemAtSlot(inventory.HotbarSelection, ref _item);

			ItemPlacedEvent?.Invoke(user, this);

			SetPedestalItem(_item.ItemID);
		}
	}


	[ObserversRpc(RunLocally = true, BufferLast = true)]
	private void SetPedestalItem(int itemID)
	{
		if (itemID == 0)
		{
			_interactPrompt = "Place Item";

			_itemHolder.GetComponent<MeshFilter>().mesh = null;
			_itemHolder.gameObject.SetActive(false);
		}
		else
		{
			_interactPrompt = "Swap Item";

			ItemSO itemSO = ItemManager.Instance.GetItemSO(itemID);

			_itemHolder.GetComponent<MeshFilter>().mesh = itemSO.Mesh;
			_itemHolder.GetComponent<MeshRenderer>().material = itemSO.Material;
			_itemHolder.gameObject.SetActive(true);
		}
	}
}


public enum PedestalMode
{
	None,
	Pedestal,
	RequireItem,
	DispenseItem
}
