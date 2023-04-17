using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseUI : MonoBehaviour
{
	public static MouseUI Instance;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private Canvas _canvas;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		_icon = GetComponent<Image>();

		ClearMouseItem();
	}


	public void SetMouseItem(Sprite sprite)
	{
		_icon.sprite = sprite;
		_icon.enabled = true;
	}


	public void ClearMouseItem()
	{
		_icon.sprite = null;
		_icon.enabled = false;
	}


	private void Update()
	{
		// Had issues with screenspace camera 

		// Position of the mouse in Canvas coordinates
		//Vector3 position = Vector3.zero;

		//if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
		//{
		//	RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_canvas.transform, Input.mousePosition, _canvas.worldCamera, out position);
		//}
		//else if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
		//{
		//	// Might have to divide by scale factor if not 1?
		//	position = Input.mousePosition;
		//}

		Rect pixelRect = _canvas.pixelRect;

		//Debug.Log(pixelRect);

		Vector3 position = Input.mousePosition;

		if (position.x < pixelRect.xMin)
		{
			position.x = pixelRect.xMin;
		}
		else if (position.x > pixelRect.xMax)
		{
			position.x = pixelRect.xMax;
		}


		if (position.y < pixelRect.yMin)
		{
			position.y = pixelRect.yMin;
		}
		else if (position.y > pixelRect.yMax)
		{
			position.y = pixelRect.yMax;
		}

		//Debug.Log(position);

		transform.position = position;

	}
}
