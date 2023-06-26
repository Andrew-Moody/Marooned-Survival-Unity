using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseUI : UIPanel
{
	private static MouseUI _instance;

	[SerializeField]
	private Texture2D _cursorImage;

	[SerializeField]
	private Transform _mouseFollower;

	[SerializeField]
	private Image _crosshair;

	[SerializeField]
	private Image _cursor;

	[SerializeField]
	private MouseItemUI _mouseItemUI;

	[SerializeField]
	private ToolTip _toolTip;

	[SerializeField]
	private ContextUI _contextUI;

	private Canvas _canvas;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		
	}

	public override void Initialize()
	{
		base.Initialize();

		Cursor.SetCursor(_cursorImage, new Vector2(0f, 0f), UnityEngine.CursorMode.Auto);
	}


	public override void Show(UIPanelData data)
	{
		gameObject.SetActive(true);

		if (_canvas == null)
		{
			_canvas = UIManager.Instance.UICanvas;
		}

		FollowMouseClamped(); // Might flicker if set visible for a frame before update moves to correct position

		HideTooltip();
		HideContext();

		SetCursorMode(CursorMode.Cursor);
	}



	public static void SetCursorMode(CursorMode mode)
	{
		if (_instance == null)
		{
			Debug.LogError("MouseUI Instance not yet set");
			return;
		}

		_instance._cursor.enabled = mode == CursorMode.Cursor;
		_instance._crosshair.enabled = mode == CursorMode.Crosshair;

		Cursor.visible = mode == CursorMode.Cursor;
		//Cursor.visible = false;

		if (mode == CursorMode.Cursor)
		{
			Cursor.lockState = CursorLockMode.None;
		}
		else if (mode == CursorMode.Crosshair)
		{
			Cursor.lockState = CursorLockMode.Locked;

			HideMouseItem();
		}
	}


	public static void ShowTooltip(Sprite icon, string itemName, string[] entries)
	{
		if (_instance != null)
		{
			_instance._toolTip.Show(icon, itemName, entries);
		}
	}

	public static void HideTooltip()
	{
		if (_instance != null)
		{
			_instance._toolTip.Hide();
		}
	}

	public static void ShowContext(List<ContextOption> contextOptions)
	{
		if (_instance != null)
		{
			_instance._contextUI.Show(contextOptions);
		}
	}

	public static void HideContext()
	{
		if (_instance != null)
		{
			_instance._contextUI.Hide();
		}
	}


	public static void ShowMouseItem(Sprite icon, int count)
	{
		if (_instance != null)
		{
			_instance._mouseItemUI.Show(icon, count);
		}
	}


	public static void HideMouseItem()
	{
		if (_instance != null)
		{
			_instance._mouseItemUI.Hide();
		}
	}


	private void Update()
	{
		FollowMouseClamped();
	}


	private void FollowMouseClamped()
	{
		Rect pixelRect = _canvas.pixelRect;

		Vector3 position = Input.mousePosition;

		position.x = Mathf.Clamp(position.x, pixelRect.xMin, pixelRect.xMax);
		position.y = Mathf.Clamp(position.y, pixelRect.yMin, pixelRect.yMax);

		_mouseFollower.position = position;
	}
}


public enum CursorMode
{
	None,
	Cursor,
	Crosshair
}
