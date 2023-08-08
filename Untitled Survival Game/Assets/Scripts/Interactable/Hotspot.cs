using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Hotspot : Interactable
{
	[SerializeField]
	private GameObject _inactiveObject;

	[SerializeField]
	private GameObject _activeObject;

	[SerializeField]
	private ItemRequirement[] _requiredItems;


	public System.Action<NetworkConnection, Hotspot> HotspotActivatedEvent;


	private bool _isActivated;


	public override void OnStartClient()
	{
		base.OnStartClient();

		_inactiveObject.SetActive(true);
		_activeObject.SetActive(false);


		StringBuilder stringBuilder = new StringBuilder();

		for (int i = 0; i < _requiredItems.Length; i++)
		{
			ItemRequirement item = _requiredItems[i];

			if (!_requiredItems[i].Fullfilled)
			{
				string itemName = ItemManager.Instance.GetItemSO(_requiredItems[i].ItemID).ItemName;

				stringBuilder.AppendLine($"{itemName} x{_requiredItems[i].Quantity}");
			}
		}

		_interactPrompt = stringBuilder.ToString();
	}


	public override void Interact(NetworkConnection user)
	{
		base.Interact(user);

		if (_isActivated)
		{
			return;
		}

		Inventory inventory = GameManager.Instance.GetPlayer(user).Inventory;

		StringBuilder stringBuilder = new StringBuilder();

		bool activate = true;

		for (int i = 0; i < _requiredItems.Length; i++)
		{
			ItemRequirement item = _requiredItems[i];

			if (!item.Fullfilled)
			{
				if (inventory.HasItem(out int slot, item.ItemID, item.Quantity))
				{
					item.Fullfilled = true;

					inventory.RemoveItemsAtSlot(slot, item.ItemID, item.Quantity);
				}
			}

			if (!item.Fullfilled)
			{
				activate = false;

				string itemName = ItemManager.Instance.GetItemSO(item.ItemID).ItemName;

				stringBuilder.AppendLine($"{itemName} x{item.Quantity}");
			}

			_requiredItems[i].Fullfilled = item.Fullfilled;
		}

		_interactPrompt = stringBuilder.ToString();

		if (activate)
		{
			ActivateORPC(user);

			
		}
	}


	[ObserversRpc(RunLocally = true, BufferLast = true)]
	private void ActivateORPC(NetworkConnection user)
	{
		_isActivated = true;

		_inactiveObject.SetActive(false);
		_activeObject.SetActive(true);

		GetComponent<Collider>().enabled = false;

		HotspotActivatedEvent?.Invoke(user, this);
	}




	[System.Serializable]
	private struct ItemRequirement
	{
		public int ItemID;

		public int Quantity;

		[HideInInspector]
		public bool Fullfilled;
	}
}

