using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRequirementKey : Interactable
{
	[SerializeField]
	private int _requiredItemID;

	[SerializeField]
	private int _requiredQuantity;

	[SerializeField]
	private bool _showOutline;

	[SerializeField]
	private bool _showPlacedItem;

	[SerializeField]
	private Transform _itemHolder;

	public event System.Action<NetworkConnection, ItemRequirementKey> ActivatedEvent;

	private bool _isActivated;
	public bool IsActivated => _isActivated;

	private ItemSO _itemSO;

	private Material _inactiveMat;

	public override void OnStartClient()
	{
		base.OnStartClient();

		_inactiveMat = _itemHolder.gameObject.GetComponent<MeshRenderer>().material;

		_itemSO = ItemManager.Instance.GetItemSO(_requiredItemID);

		_interactPrompt = $"{_itemSO.ItemName} x{_requiredQuantity}";

		_itemHolder.gameObject.GetComponent<MeshFilter>().mesh = _itemSO.Mesh;

		_itemHolder.gameObject.SetActive(_showOutline);
	}


	public override void Interact(NetworkConnection user)
	{
		base.Interact(user);

		if (_isActivated)
		{
			return;
		}

		
		Inventory inventory = GameManager.Instance.GetPlayer(user).Inventory;

		if (inventory != null && inventory.HasItem(out int slot, _requiredItemID, _requiredQuantity))
		{
			if (!inventory.RemoveItemsAtSlot(slot, _requiredItemID, _requiredQuantity))
			{
				Debug.LogError("HasItem passed but RemoveItemsAtSlot failed");
				return;
			}

			SetActivationStateORPC(true);

			ActivatedEvent?.Invoke(user, this);
		}
	}


	[Server]
	public void ResetActivation()
	{
		SetActivationStateORPC(false);
	}


	[ObserversRpc(RunLocally = true, BufferLast = true)]
	private void SetActivationStateORPC(bool activated)
	{
		_isActivated = activated;

		if (activated)
		{
			_interactPrompt = "";
			_itemHolder.gameObject.GetComponent<MeshRenderer>().material = _itemSO.Material;
			_itemHolder.gameObject.SetActive(_showPlacedItem);
		}
		else
		{
			_interactPrompt = $"{_itemSO.ItemName} x{_requiredQuantity}";
			_itemHolder.gameObject.GetComponent<MeshRenderer>().material = _inactiveMat;
			_itemHolder.gameObject.SetActive(_showOutline);
		}
	}
}
