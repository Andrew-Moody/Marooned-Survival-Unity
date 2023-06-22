using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : UIPanel, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField][Tooltip("Allow empty slots to be dragged as items")]
	private bool _dragEmpty;

	[SerializeField]
	private ItemSlotUI[] _slots;

	[SerializeField]
	private GameObject _inventoryWindow;

	[SerializeField]
	private CraftingUI _handCraftingUI;

	[SerializeField]
	private CraftingStationSO _handCraftingSO;

	private Inventory _inventory;

	private int _ptrDownIndex;


	private void OnDestroy()
	{
		if (_inventory != null)
		{
			_inventory.SlotUpdated -= OnUpdateSlot;
		}
	}

	public override void Initialize()
	{
		for (int i = 0; i < _slots.Length; ++i)
		{
			_slots[i].Initialize(i);
		}

		Debug.LogError("InventoryUI Initialized");
	}


	public override void SetPlayer(GameObject player)
	{
		_player = player;

		if (_player != null)
		{
			_inventory = player.GetComponent<Inventory>();

			if (_inventory == null)
			{
				Debug.Log("Failed to register player inventory to InventoryUI");
			}

			_inventory.SlotUpdated += OnUpdateSlot;

			_handCraftingUI.SetPlayer(_player);
		}
	}


	public override void Show(UIPanelData data)
	{
		gameObject.SetActive(true);

		_inventory.UpdateSlots();

		if (CameraController.Instance != null && CameraController.Instance.EquipmentCamera != null)
		{
			CameraController.Instance.EquipmentCamera.gameObject.SetActive(true);
		}

		_handCraftingUI.Show(new CraftingUIPanelData(_handCraftingSO.Recipes));
	}


	public override void Hide()
	{
		base.Hide();

		if (CameraController.Instance != null && CameraController.Instance.EquipmentCamera != null)
		{
			CameraController.Instance.EquipmentCamera.gameObject.SetActive(false);
		}
	}


	// OnPointerDown and OnBeginDrag both are called on the object that is actually clicked*
	// OnPointerUp, OnDrag, and OnEndDrag are called on the initial object regardless of where the mouse ends up*
	// The object that the mouse raycast has hit can be accessed by the eventdata*
	// Also OnPointerUp is called before OnEndDrag

	// * In this case all of the events are called on the parent UI object
	// The current object under the mouse can be accessed through eventdata
	// The object under the mouse can be cached in OnPointerDown OnBeginDrag etc or with eventdata.pointerPress, eventdata.pointerDrag

	// Update 6/22/23
	// I have found some quirks with the way pointer events work when handled by parent objects
	// Things like OnPointerExit not firing as long as the mouse is over a child at each event
	// (moving the mouse from one item to the next even over a gap if moved quickly enough)
	// And now pointerPress and pointerDrag yield the InventoryUI gameObject (the object that handled OnPointerDown is my guess why)
	// not the gameObject that was hit by the raycast even though pointerPressRaycast still gives the actual object hit

	public void OnPointerDown(PointerEventData eventData)
	{
		// The slot that recieved OnPointerDown can be retrieved through pointerCurrentRaycast and cached or
		// by pointerPressRaycast in future callbacks (up, drag etc.) note that pointerPress is null currently
		// and will refer to InventoryUI, not the slot in future events (because this object actually handles the event)?
		// if it becomes an issue it may be best to move the pointer event handlers to the slots and give them callbacks to actually handle them

		ItemSlotUI slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlotUI>();
		if (slot != null)
		{
			_ptrDownIndex = slot.Index;

			//if (!_inventory.IsSlotEmpty(slot.Index))
			//{
			//	_ptrDownIndex = slot.Index;
			//}
			//else
			//{
			//	_ptrDownIndex = -1;
			//}

			Debug.Log("OnPointerDown on Slot: " + slot.Index);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		// Handled by OnEndDrag
		if (eventData.dragging || eventData.pointerCurrentRaycast.gameObject == null)
		{
			return;
		}

		if (!eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out ItemSlotUI slot))
		{
			Debug.LogError($"Pointer up on {eventData.pointerCurrentRaycast.gameObject}, failed to find slot");
			return;
		}

		// If double+ click is needed
		if (eventData.clickCount > 1)
		{
			Debug.LogError($"Double click on slot {slot.gameObject.name}");
		}

		// Handle LeftClick
		// (Use or consume in Runescape, Pickup by mouse in Minecraft or Muck)
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				// Shift Click if applicable
			}
		}
		
		// Handle RightClick
		// Open Options context menu in Runescape, Pickup half the stack in Minecraft (and Muck?)
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			Debug.LogError("Right Clicked Slot: " + slot.Index);

			if (!_inventory.IsSlotEmpty(slot.Index))
			{
				List<ContextOption> options = _inventory.GetItemOptions(slot.Index);

				MouseUI.ShowContext(options);
			}
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		GameObject slotObj = eventData.pointerCurrentRaycast.gameObject;

		if (slotObj != null && slotObj.TryGetComponent(out ItemSlotUI slot))
		{
			if (_dragEmpty || !_inventory.IsSlotEmpty(slot.Index))
			{
				// Hide the item in the starting slot and set the mouse sprite
				slot.HideSlot();
				MouseUI.ShowMouseItem(slot.Sprite);
			}
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		// Must implement OnDrag or else OnBeginDrag and OnEndDrag will not be called
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		GameObject startObj = eventData.pointerPressRaycast.gameObject;
		GameObject endObj = eventData.pointerCurrentRaycast.gameObject;

		// Check if the drag start was not on an item
		if (startObj == null || !startObj.TryGetComponent(out ItemSlotUI startSlot))
		{
			Debug.LogError($"DragStart was not on a UI object");
			return;
		}

		// Do nothing if the starting slot is empty and dragEmpty is false
		if (!_dragEmpty && _inventory.IsSlotEmpty(startSlot.Index))
		{
			return;
		}

		// Unhide the starting slot and clear the mouse item regardless of where the drag ends
		_slots[startSlot.Index].ShowSlot();
		MouseUI.HideMouseItem();

		// Check if the Drag ended inside the inventory window but not on another slot
		if (endObj == _inventoryWindow)
		{
			return;
		}


		if (endObj != null && endObj.TryGetComponent(out ItemSlotUI endSlot))
		{
			// Drag between two items (Minecraft, Runescape, Muck, and others swap items)
			_inventory.SwapSlotsSRPC(startSlot.Index, endSlot.Index);
		}
		else
		{
			// Drag ended outside the inventory UI (Minecraft, Runescape, Muck, and others choose to drop the item in this case)
			_inventory.DropItemSRPC(_ptrDownIndex);
		}	
	}





	private void OnUpdateSlot(object sender, SlotUpdateEventArgs eventArgs)
	{
		_slots[eventArgs.Index].UpdateSlot(eventArgs.Sprite, eventArgs.Count);
	}
}
