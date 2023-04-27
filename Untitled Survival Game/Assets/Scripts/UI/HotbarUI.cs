using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
	[SerializeField]
	private float _slotWidth;

	[SerializeField]
	private int _size;

	[SerializeField]
	private Image _selectionBox;

	private int _selection;

	private Inventory _inventory;
    

	public void Initialize(GameObject player)
	{
		Debug.LogError("Hotbar Initialize");

		_inventory = player.GetComponent<Inventory>();

		gameObject.SetActive(true);
	}


	private void SetSelection(int selection)
	{
		Vector3 position = _selectionBox.rectTransform.anchoredPosition;

		position.x = selection * _slotWidth;

		//Debug.Log($"Slot: {_selection}, Start: {_selectionBox.rectTransform.anchoredPosition.x}, End: {position.x}");

		_selectionBox.rectTransform.anchoredPosition = position;

		_inventory.SetHotbarSelectionSRPC(selection);
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
}
