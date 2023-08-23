using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Actors;

public class InventoryUI : UIPanel
{
	[SerializeField][Tooltip("Allow empty slots to be dragged as items")]
	private bool _dragEmpty;

	[SerializeField]
	private ItemSlotUI[] _slots;

	[SerializeField]
	private Transform _inventoryWindow;

	[SerializeField]
	private PointerEventDetector _outsideDetector;

	[SerializeField]
	private CraftingUI _handCraftingUI;

	[SerializeField]
	private CraftingStationSO _handCraftingSO;

	private Inventory _inventory;


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

			if (_slots[i].TryGetComponent(out PointerEventDetector detector))
			{
				detector.OnPointerClickEvent += OnPointerClickEvent;
				detector.OnBeginDragEvent += OnBeginDragEvent;
				detector.OnEndDragEvent += OnEndDragEvent;
				detector.OnPointerEnterEvent += OnPointerEnterEvent;
				detector.OnPointerExitEvent += OnPointerExitEvent;
			}
		}

		_outsideDetector.OnPointerClickEvent += OnPointerClickEvent;
	}


	public override void SetPlayer(Actor player)
	{
		_player = player;

		if (_player != null)
		{
			_inventory = player.Inventory;

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

		if (data is CraftingUIPanelData craftingData)
		{
			_handCraftingUI.Show(craftingData);
		}
		else
		{
			_handCraftingUI.Show(new CraftingUIPanelData(_handCraftingSO.Recipes));
		}
	}


	public override void Hide()
	{
		base.Hide();

		if (CameraController.Instance != null && CameraController.Instance.EquipmentCamera != null)
		{
			CameraController.Instance.EquipmentCamera.gameObject.SetActive(false);
		}

		MouseUI.HideTooltip();
	}


	// Update 6/22/23
	// I have found some quirks with the way pointer events work when handled by parent objects
	// Things like OnPointerExit not firing as long as the mouse is over a child at each event
	// (moving the mouse from one item to the next even over a gap if moved quickly enough)
	// And now pointerPress and pointerDrag yield the InventoryUI gameObject (the object that handled OnPointerDown is my guess why)
	// not the gameObject that was hit by the raycast even though pointerPressRaycast still gives the actual object hit

	// The slot that recieved OnPointerDown can be retrieved through pointerCurrentRaycast and cached or
	// by pointerPressRaycast in future callbacks (up, drag etc.) note that pointerPress is null currently
	// and will refer to InventoryUI, not the slot in future events (because this object actually handles the event)?
	// if it becomes an issue it may be best to move the pointer event handlers to the slots and give them callbacks to actually handle them
	
	// Update 6/23/23
	// Adding tooltips to items caused the same issue with pointer exit events
	// Decided to move pointer event handlers to the slots

	private void OnPointerClickEvent(PointerEventData eventData)
	{
		string pointerPress = eventData.pointerPress != null ? eventData.pointerPress.name : "null";

		string pointerClick = eventData.pointerClick != null ? eventData.pointerClick.name : "null";

		string pressRay = eventData.pointerPressRaycast.gameObject != null ? eventData.pointerPressRaycast.gameObject.name : "null";

		string currentRay = eventData.pointerCurrentRaycast.gameObject != null ? eventData.pointerCurrentRaycast.gameObject.name : "null";

		Debug.LogError($"Click pointerClick {pointerClick}, pointerPress {pointerPress}, pressRay {pressRay}, currentRay {currentRay}");


		// Handle in OnEndDrag (PointerClick will still fire for a drag if the drag starts and ends on the same object)
		if (eventData.dragging)
		{
			Debug.LogError("OnPointerClick recieved while dragging");
			return;
		}

		// This shouldn't be true but wanted to check
		if (eventData.pointerCurrentRaycast.gameObject == null || eventData.pointerClick == null)
		{
			Debug.LogError("OnPointerClick recieved on null gameObject");
			return;
		}


		// Clicking outside inventory with an item in the mouse slot should drop the item
		if (eventData.pointerClick == _outsideDetector.gameObject)
		{
			Debug.LogError("OnPointerClick outside inventory");

			if (_inventory.HasMouseItem())
			{
				_inventory.DropItemSRPC(_inventory.MouseIndex);
			}

			return;
		}


		if (!eventData.pointerClick.TryGetComponent(out ItemSlotUI slot))
		{
			Debug.LogError($"Pointer up on {eventData.pointerClick.gameObject}, failed to find slot");
			return;
		}

		// If double+ click is needed
		if (eventData.clickCount > 1)
		{
			Debug.LogError($"Multi click on slot {slot.gameObject.name}, clicks: {eventData.clickCount}");
		}


		// Handle LeftClick
		// (Use or consume in Runescape, Pickup by mouse in Minecraft or Muck)
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				// Shift Click if applicable
				Debug.LogError($"Shift Left click on slot {slot.gameObject.name}");
			}
			else
			{
				//Debug.LogError($"Left click on slot {slot.gameObject.name}");
				_inventory.SwapWithMouseSRPC(slot.Index);
			}
		}

		// Handle RightClick
		// Open Options context menu in Runescape, Pickup half the stack in Minecraft (and Muck?)
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			Debug.LogError("Right Clicked Slot: " + slot.Index);

			// need to check mouse is not holding an item

			if (!_inventory.IsSlotEmpty(slot.Index))
			{
				List<ContextOption> options = _inventory.GetItemOptions(slot.Index);

				MouseUI.ShowContext(options);
			}
		}
	}


	private void OnBeginDragEvent(PointerEventData eventData)
	{
		string pointerPress = eventData.pointerPress != null ? eventData.pointerPress.name : "null";

		string pressRay = eventData.pointerPressRaycast.gameObject != null ? eventData.pointerPressRaycast.gameObject.name : "null";

		string currentRay = eventData.pointerCurrentRaycast.gameObject != null ? eventData.pointerCurrentRaycast.gameObject.name : "null";

		Debug.LogError($"BeginDrag pointerPress {pointerPress}, pressRay {pressRay}, currentRay {currentRay}");


		GameObject slotObj = eventData.pointerCurrentRaycast.gameObject;

		if (slotObj != null && slotObj.TryGetComponent(out ItemSlotUI slot))
		{
			if (_dragEmpty || !_inventory.IsSlotEmpty(slot.Index))
			{
				// Hide the item in the starting slot and set the mouse sprite
				slot.HideSlot();
				MouseUI.ShowMouseItem(slot.ItemIcon, slot.ItemCount);
			}
		}
	}


	private void OnEndDragEvent(PointerEventData eventData)
	{
		string lastPress = eventData.lastPress != null ? eventData.lastPress.name : "null";

		string pointerDrag = eventData.pointerDrag != null ? eventData.pointerDrag.name : "null";

		string pointerPress = eventData.pointerPress != null ? eventData.pointerPress.name : "null";

		string pressRay = eventData.pointerPressRaycast.gameObject != null ? eventData.pointerPressRaycast.gameObject.name : "null";

		string currentRay = eventData.pointerCurrentRaycast.gameObject != null ? eventData.pointerCurrentRaycast.gameObject.name : "null";

		Debug.LogError($"EndDrag pointerDrag {pointerDrag}, lastPress {lastPress}, pointerPress {pointerPress}, pressRay {pressRay}, currentRay {currentRay}");


		GameObject startObj = eventData.pointerPressRaycast.gameObject;
		GameObject endObj = eventData.pointerCurrentRaycast.gameObject;

		if (startObj == null || endObj == null)
		{
			Debug.LogError($"StartObj was null: {startObj == null}, EndObj was null: {endObj == null}");
		}

		// Check if the drag start was not on an item
		if (startObj == null || !startObj.TryGetComponent(out ItemSlotUI startSlot))
		{
			Debug.LogError($"DragStart was not on a UI object");
			return;
		}

		// Do nothing if the starting slot is empty and dragEmpty is false
		if (!_dragEmpty && _inventory.IsSlotEmpty(startSlot.Index))
		{
			Debug.LogError($"DragStart was on an empty slot");
			return;
		}

		// Unhide the starting slot and clear the mouse item regardless of where the drag ends
		_slots[startSlot.Index].ShowSlot();
		MouseUI.HideMouseItem();


		if (endObj != null && endObj.TryGetComponent(out ItemSlotUI endSlot))
		{
			// Drag between two items (Minecraft, Runescape, Muck, and others swap items)
			_inventory.SwapSlotsSRPC(startSlot.Index, endSlot.Index);
		}
		else if (endObj.transform.IsChildOf(_inventoryWindow))
		{
			// Drag ended inside the inventory window but not on another slot (return item to its original place)
			Debug.LogError($"DragEnd was inside the inventory but not on a slot: {endObj.name}");
			return;
		}
		else
		{
			// Drag ended outside the inventory UI (Minecraft, Runescape, Muck, and others choose to drop the item in this case)
			_inventory.DropItemSRPC(startSlot.Index);
		}	
	}


	private void OnPointerEnterEvent(PointerEventData eventData)
	{
		if (eventData.pointerEnter.TryGetComponent(out ItemSlotUI slot))
		{
			if (!_inventory.IsSlotEmpty(slot.Index))
			{
				MouseUI.ShowTooltip(slot.ItemIcon, slot.ItemName, new string[] { slot.ItemDescription });
			}
		}
	}


	private void OnPointerExitEvent(PointerEventData eventData)
	{
		MouseUI.HideTooltip();
	}


	private void OnUpdateSlot(object sender, SlotUpdateEventArgs eventArgs)
	{
		if (eventArgs.Index == _inventory.MouseIndex)
		{
			MouseUI.ShowMouseItem(eventArgs.ItemIcon, eventArgs.Count);
		}
		else
		{
			_slots[eventArgs.Index].UpdateSlot(eventArgs.ItemIcon, eventArgs.ItemName, eventArgs.ItemDescription, eventArgs.Count);
		}
	}
}
