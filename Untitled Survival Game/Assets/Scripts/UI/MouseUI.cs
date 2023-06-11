using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseUI : UIPanel
{
	[SerializeField]
	private Image _crosshair;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private Canvas _canvas;


	public override void Initialize()
	{
		_icon.sprite = null;
		_icon.enabled = false;
		_crosshair.enabled = false;
	}


	public override void Show(UIPanelData mouseData)
	{
		gameObject.SetActive(true);

		MouseUIPanelData data = mouseData as MouseUIPanelData;

		if (data != null)
		{
			Debug.Log($"{data.Sprite} {data.Mode}");

			_icon.sprite = data.Sprite;
			_icon.enabled = data.Sprite != null;

			if (data.Mode == MouseUIMode.Crosshair)
			{
				_icon.enabled = false;
				_crosshair.enabled = true;
			}
			else
			{
				_crosshair.enabled = false;
			}
		}
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

		Vector3 position = Input.mousePosition;

		position.x = Mathf.Clamp(position.x, pixelRect.xMin, pixelRect.xMax);
		position.y = Mathf.Clamp(position.y, pixelRect.yMin, pixelRect.yMax);

		_icon.transform.position = position;
	}
}


public class MouseUIPanelData : UIPanelData
{
	private Sprite _sprite;
	public Sprite Sprite => _sprite;

	private MouseUIMode _mode;
	public MouseUIMode Mode => _mode;

	public MouseUIPanelData(Sprite sprite, MouseUIMode mode)
	{
		_sprite = sprite;

		_mode = mode;
	}


	public MouseUIPanelData(MouseUIMode mode)
	{
		_sprite = null;

		_mode = mode;
	}
}


public enum MouseUIMode
{
	None,
	Cursor,
	Crosshair
}
