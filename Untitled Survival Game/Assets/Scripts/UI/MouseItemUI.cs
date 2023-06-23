using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseItemUI : MonoBehaviour
{
	[SerializeField]
	private Image _itemIcon;

	[SerializeField]
	private TextMeshProUGUI _itemCount;


	private void Awake()
	{
		Hide();
	}


	public void Show(Sprite icon, int count)
	{
		_itemIcon.sprite = icon;
		gameObject.SetActive(true);


		if (count < 2)
		{
			_itemCount.text = "";
		}
		else if (count < 100000)
		{
			_itemCount.text = count.ToString("N0");
			_itemCount.color = Color.yellow;
		}
		else if (count < 10000000)
		{
			_itemCount.text = (count / 1000).ToString("N0") + "K";
			_itemCount.color = Color.yellow;
		}
		else
		{
			_itemCount.text = (count / 1000000).ToString("N0") + "M";
			_itemCount.color = Color.green;
		}
	}


	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
