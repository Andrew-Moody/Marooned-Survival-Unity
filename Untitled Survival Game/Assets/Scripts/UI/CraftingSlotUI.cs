using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public int index;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private Color _greyoutColor;

	public Sprite CurrentIcon => _icon.sprite;

	private int _itemID;

	public event System.Action<PointerEventData> OnPointerClickEvent;

	public event System.Action<PointerEventData> OnPointerEnterEvent;

	public event System.Action<PointerEventData> OnPointerExitEvent;


	public void SetRecipe(CraftingRecipe recipe)
	{
		Debug.Log($"Output: {recipe.OutputID}, currentID: {_itemID}");

		if (recipe.OutputID != _itemID)
		{
			_itemID = recipe.OutputID;

			ItemSO itemSO = ItemManager.Instance.GetItemSO(_itemID);

			if (itemSO != null)
			{
				Debug.Log(itemSO.Sprite.name);
				_icon.sprite = itemSO.Sprite;
			}
			else
			{
				Debug.Log("ItemSO not found for ID: " + _itemID);
				_icon.sprite = null;
			}
		}
	}


	public void SetGreyout(bool greyout)
	{
		if (greyout)
		{
			_icon.color = _greyoutColor;
		}
		else
		{
			_icon.color = Color.white;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnPointerClickEvent?.Invoke(eventData);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		OnPointerEnterEvent?.Invoke(eventData);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnPointerExitEvent?.Invoke(eventData);
	}
}
