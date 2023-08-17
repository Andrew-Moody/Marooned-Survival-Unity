using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Actors;

public class HotbarUI : UIPanel
{
	[SerializeField]
	private float _slotWidth;

	[SerializeField]
	private int _size;

	[SerializeField]
	private Image _selectionBox;

	[SerializeField]
	private ItemSlotUI[] _slots;

	private int _selection;

	private Inventory _inventory;


	public override void Initialize()
	{
		base.Initialize();

		for (int i = 0; i < _slots.Length; i++)
		{
			_slots[i].Initialize(i);
		}
	}

	public override void SetPlayer(Actor player)
	{
		base.SetPlayer(player);

		if (_player != null)
		{
			_inventory = player.Inventory;

			if (_inventory == null)
			{
				Debug.Log("Failed to register player inventory to HotbarUI");
			}

			_inventory.SlotUpdated += OnUpdateSlot;
		}
	}


	public override void Show(UIPanelData data)
	{
		base.Show(data);

		if (_inventory != null)
		{
			_inventory.UpdateSlots();
		}
	}


	private void SetSelection(int selection)
	{
		Vector3 position = _selectionBox.rectTransform.anchoredPosition;

		position.x = selection * _slotWidth;

		Debug.Log($"Slot: {_selection}, Start: {_selectionBox.rectTransform.anchoredPosition.x}, End: {position.x}");

		_selectionBox.rectTransform.anchoredPosition = position;

		if (_inventory != null)
		{
			_inventory.SetHotbarSelectionSRPC(selection);
		}
	}


	private void Update()
	{
		if (Input.mouseScrollDelta.y != 0f)
		{
			float scroll = Input.mouseScrollDelta.y;
			if (scroll > 0f)
			{
				_selection -= 1;
			}
			else
			{
				_selection += 1;
			}


			if (_selection < 0)
			{
				_selection = _size - 1;
			}
			else if (_selection > _size - 1)
			{
				_selection = 0;
			}

			SetSelection(_selection);
		}
	}

	private void OnUpdateSlot(object sender, SlotUpdateEventArgs eventArgs)
	{
		//Debug.LogError("Hotbar OnUpdateSlot");
		
		if (eventArgs.Index < _slots.Length)
		{
			_slots[eventArgs.Index].UpdateSlot(eventArgs.ItemIcon, eventArgs.ItemName, eventArgs.ItemDescription, eventArgs.Count);
		}
	}


	private void OnDestroy()
	{
		if (_inventory != null)
		{
			_inventory.SlotUpdated -= OnUpdateSlot;
		}
	}
}
