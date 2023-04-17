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

	private Inventory _inventory;

	public Sprite Sprite { get { return _icon.sprite; } set { _icon.sprite = value; } }

    public void Initialize(Inventory inventory, int index)
	{
		_inventory = inventory;
		Index = index;
	}


	public void UpdateSlot(Sprite icon, int count)
	{
		if (icon == null)
		{
			ClearSlot();
			return;
		}

		if (count < 2)
		{
			_countTMP.text = "";
		}
		else if (count < 100000)
		{
			_countTMP.text = count.ToString("N0");
			_countTMP.color = Color.yellow;
		}
		else if (count < 10000000)
		{
			_countTMP.text = (count / 1000).ToString("N0") + "K";
			_countTMP.color = Color.yellow;
		}
		else
		{
			_countTMP.text = (count / 1000000).ToString("N0") + "M";
			_countTMP.color = Color.green;
		}

		_icon.sprite = icon;
		_icon.enabled = true;
		_countTMP.enabled = true;
	}


	public void ClearSlot()
	{
		_icon.sprite = null; // This will leave a white square, need to either disable or set alpha to 0 (but then need to undo later)
		_icon.enabled = false;
		_countTMP.text = "";
		_countTMP.enabled = false;
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
