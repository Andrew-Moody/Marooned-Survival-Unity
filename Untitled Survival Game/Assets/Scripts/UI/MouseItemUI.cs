using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseItemUI : MonoBehaviour
{
	[SerializeField]
	private Image _itemIcon;


	private void Awake()
	{
		Hide();
	}


	public void Show(Sprite icon)
	{
		_itemIcon.sprite = icon;
		gameObject.SetActive(true);
	}


	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
