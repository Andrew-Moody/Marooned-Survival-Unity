using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
	public int Index { get; private set; }

	public bool IsEquipmentSlot;

	[SerializeField]
	private TextMeshProUGUI _countTMP;

	[SerializeField]
	private Image _icon;

	public Sprite ItemIcon => _icon.sprite;

	public string ItemName { get; private set; }

	public string ItemDescription { get; private set; }

	public int ItemCount { get; private set; }


	public void Initialize(int index)
	{
		Index = index;
		ShowSlot();
	}


	public void UpdateSlot(Sprite itemIcon, string itemName, string itemDescription, int itemCount)
	{
		_icon.sprite = itemIcon;
		ItemName = itemName;
		ItemDescription = itemDescription;
		ItemCount = itemCount;

		// Format the count text depending on the amount
		if (itemCount < 2)
		{
			_countTMP.text = "";
		}
		else if (itemCount < 100000)
		{
			_countTMP.text = itemCount.ToString("N0");
			_countTMP.color = Color.yellow;
		}
		else if (itemCount < 10000000)
		{
			_countTMP.text = (itemCount / 1000).ToString("N0") + "K";
			_countTMP.color = Color.yellow;
		}
		else
		{
			_countTMP.text = (itemCount / 1000000).ToString("N0") + "M";
			_countTMP.color = Color.green;
		}
	}


	public void HideSlot()
	{
		_icon.enabled = false;
		_countTMP.enabled = false;
	}

	public void ShowSlot()
	{
		_icon.enabled = true;
		_countTMP.enabled = true;
	}
}
