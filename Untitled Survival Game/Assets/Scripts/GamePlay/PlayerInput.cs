using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	// Loosely based on tutorial by "Dave / GameDevelopment" (added y inversion, ability to toggle off, and integration with fishnetworking)

	private static PlayerInput _instance;

	[SerializeField]
	private float _senX;

	[SerializeField]
	private float _senY;

	[SerializeField]
	private bool _invertY;

	private bool _fpsMode;
	public static bool FPSMode => _instance != null && _instance._fpsMode;

	private float _xRotation;
	public static float XRotation => _instance._xRotation;

	private float _yRotation;
	public static float YRotation => _instance._yRotation;

	private bool _jumpQueued;

	public static bool CheckJump()
	{
		bool jump = false;

		if (_instance != null)
		{
			jump = _instance._jumpQueued;
			_instance._jumpQueued = false;
		}

		return jump;
	}

	public static void SetFPSMode(bool fpsMode)
	{
		if (_instance == null)
		{
			return;
		}

		_instance._fpsMode = fpsMode;

		if (fpsMode)
		{
			MouseUI.SetCursorMode(CursorMode.Crosshair);
		}
		else
		{
			MouseUI.SetCursorMode(CursorMode.Cursor);
		}
	}


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


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			CameraController.Instance.IncrementCameraMode();
		}

		if (!_fpsMode)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			_jumpQueued = true;
		}

		float mouseX = Input.GetAxisRaw("Mouse X") * _senX;
		float mouseY = Input.GetAxisRaw("Mouse Y") * _senY;

		// Thats it for horizontal rotation
		_yRotation += mouseX; // moving mouse in x rotates about y

		// Vertical is a bit more complex
		if (_invertY)
		{
			mouseY = -mouseY;
		}

		_xRotation -= mouseY;

		_xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
	}
}
