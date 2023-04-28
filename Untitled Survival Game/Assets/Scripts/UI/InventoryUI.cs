using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField]
	private ItemSlotUI[] _slots;

	[SerializeField]
	private MouseUI _mouseUI;

	[SerializeField]
	private ContextUI _contextUI;

	private Inventory _inventory;

	private int _ptrDownIndex;

	private int _rows;

	private int _cols;


	
	void Start()
	{
	
	}


	private void OnDestroy()
	{
		if (_inventory != null)
		{
			_inventory.SlotUpdated -= OnUpdateSlot;
		}
	}

	public void Initialize(GameObject player)
	{
		_inventory = player.GetComponent<Inventory>();

		if (_inventory == null)
		{
			Debug.Log("Failed to register player inventory to the inventoryUI");
		}

		_inventory.SlotUpdated += OnUpdateSlot;

		for (int i = 0; i < _slots.Length; ++i)
		{
			_slots[i].Initialize(i);
		}

		_mouseUI.ClearMouseItem();

		Debug.LogError("InventoryUI Initialized");

		_inventory.UpdateSlots();
	}


	// OnPointerDown and OnBeginDrag both are called on the object that is actually clicked*
	// OnPointerUp, OnDrag, and OnEndDrag are called on the initial object regardless of where the mouse ends up*
	// The object that the mouse raycast has hit can be accessed by the eventdata*
	// Also OnPointerUp is called before OnEndDrag

	// * In this case all of the events are called on the parent UI object
	// The current object under the mouse can be accessed through eventdata
	// The object under the mouse can be cached in OnPointerDown OnBeginDrag etc or with eventdata.pointerPress, eventdata.pointerDrag

	public void OnPointerDown(PointerEventData eventData)
	{
		
		ItemSlotUI slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlotUI>();
		if (slot != null)
		{
			if (slot.Sprite != null)
			{
				_ptrDownIndex = slot.Index;
			}
			else
			{
				_ptrDownIndex = -1;
			}

			Debug.Log("OnPointerDown on Slot: " + slot.Index);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		ItemSlotUI slot = null;

		// It is common for PointerUp to occur offscreen or in the game window causing pointerCurrentRaycast to be null (due to dragging outside UI bounds)
		if (eventData.pointerCurrentRaycast.gameObject != null)
		{
			slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlotUI>();
		}

		if (slot == null)
		{
			Debug.Log("OnPointerUp Outside Inventory");
			return;
		}

		Debug.Log("OnPointerUp on Slot: " + slot.Index);

		// OnPointerUp with no drag represents a "click"
		// Could handle double+ click but I havent really seen many examples in games i've played
		if (eventData.dragging)
		{
			// Handled by OnEndDrag
			return;
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

			if (slot.IsEquipmentSlot)
			{
				List<ContextOption> options = _inventory.GetItemOptions(slot.Index);
				if (options != null)
				{
					_contextUI.PopulateOptions(slot.Index, options);
				}
			}
			else
			{
				List<ContextOption> options = _inventory.GetItemOptions(slot.Index);
				if (options != null)
				{
					_contextUI.PopulateOptions(slot.Index, options);
				}
			}
			
		}

		
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		ItemSlotUI slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlotUI>();
		if (slot != null)
		{
			Debug.LogError("OnBeginDrag on Slot: " + slot.Index);

			if (slot.Sprite != null)
			{
				_mouseUI.SetMouseItem(slot.Sprite);
				slot.HideSlot();
			}
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		// Must implement OnDrag or else OnBeginDrag and OnEndDrag will not be called
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		// First check that drag didnt start on empty slot
		if ( _ptrDownIndex == -1)
		{
			return;
		}

		ItemSlotUI slot = null;

		// It is common for a drag to end offscreen or in the gamewindow causing pointerCurrentRaycast to be null
		if (eventData.pointerCurrentRaycast.gameObject != null)
		{
			slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlotUI>();
		}

		if (slot != null)
		{
			// Drag ended inside the inventory UI (Minecraft, Runescape, Muck, and others swap items)
			Debug.LogError("OnEndDrag on Slot: " + slot.Index);

			_inventory.SwapSlotsSRPC(_ptrDownIndex, slot.Index);

			_slots[_ptrDownIndex].ShowSlot();
			_mouseUI.ClearMouseItem();
		}
		else
		{
			// Drag ended outside the inventory UI (Minecraft, Runescape, Muck, and others choose to drop the item in this case)
			Debug.LogError("OnEndDrag Outside Inventory");

			_inventory.DropItemSRPC(_ptrDownIndex);

			_slots[_ptrDownIndex].ShowSlot();
			_mouseUI.ClearMouseItem();
		}
	}





	private void OnUpdateSlot(object sender, SlotUpdateEventArgs eventArgs)
	{
		_slots[eventArgs.Index].UpdateSlot(eventArgs.Sprite, eventArgs.Count);
	}
}
