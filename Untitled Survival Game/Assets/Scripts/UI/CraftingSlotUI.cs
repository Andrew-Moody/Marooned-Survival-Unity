using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingSlotUI : MonoBehaviour
{
	public int index;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private Color _greyoutColor;

	public Sprite ItemIcon => _icon.sprite;

	public string ItemName { get; private set; }

	private int _itemID;


	public void SetRecipe(CraftingRecipe recipe)
	{
		if (recipe.OutputID != _itemID)
		{
			_itemID = recipe.OutputID;

			ItemSO itemSO = ItemManager.Instance.GetItemSO(_itemID);

			if (itemSO != null)
			{
				_icon.sprite = itemSO.Sprite;
				ItemName = itemSO.ItemName;
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
}
