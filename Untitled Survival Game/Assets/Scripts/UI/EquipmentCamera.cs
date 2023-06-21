using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCamera : MonoBehaviour
{
	[SerializeField]
	private float _maxAngleX;

	[SerializeField]
	private float _maxAngleY;

	private Canvas _canvas;


	private void OnEnable()
	{
		if (_canvas == null)
		{
			_canvas = UIManager.Instance.UICanvas;
		}

		Rect bounds = _canvas.pixelRect;


	}

	private void Update()
	{
		Vector3 mousePos = Input.mousePosition;

		Rect bounds = _canvas.pixelRect;

		// Normalize position to -0.5 to 0.5
		mousePos.x = -0.5f + Mathf.Clamp(mousePos.x, bounds.xMin, bounds.xMax) / bounds.xMax;
		mousePos.y = -0.5f + Mathf.Clamp(mousePos.y, bounds.yMin, bounds.yMax) / bounds.yMax;

		transform.localRotation = Quaternion.Euler(new Vector3(_maxAngleX * mousePos.y, _maxAngleY * mousePos.x, 0f));

		// Use this instead to make a fun clip for a fail montage 
		//transform.Rotate(45 * mousePos);

	}
}
