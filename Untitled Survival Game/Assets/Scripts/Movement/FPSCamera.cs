using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
	// Tutorial by "Dave / GameDevelopment" (I added y inversion and ability to toggle off)
	// Outdated, see CameraController
	public float SenX;
	public float SenY;
	public bool InvertY;

	private Transform _player;

	private float _xRotation;
	private float _yRotation;
	private bool _fpsMode;

	// Start is called before the first frame update
	void Start()
	{
		_player = PlayerLocator.Player.transform;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			SetFPSMode(!_fpsMode);
		}

		if (!_fpsMode)
		{
			return;
		}

		float mouseX = Input.GetAxisRaw("Mouse X") * SenX;
		float mouseY = Input.GetAxisRaw("Mouse Y") * SenY;

		// Thats it for horizontal rotation
		_yRotation += mouseX; // moving mouse in x rotates about y

		// Vertical is a bit more complex
		if (InvertY)
		{
			mouseY = -mouseY;
		}

		_xRotation -= mouseY;

		_xRotation = Mathf.Clamp(_xRotation, -90f, 90f);


		// This tutorial uses two seperate transforms
		// The camera holder has a script that updates the position to the player position in update
		// rather than parent the camera under the player because of rigidbody wierdness

		// I actually decided to change my setup to be closer to the tutorial
		// He used Update but LateUpdate would be better normally
		// Unfortunatly for me, fishnet handles predicted movement with its own timemanager
		// Ill have to test the best place to move this too (Input will still need to be cached in regular Update)
		// Probably TimeManager.OnPostTick would be the best place

		_player.rotation = Quaternion.Euler(0, _yRotation, 0); // Only handles horizontal rotation
		transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0); // has to handle both since not a child of player
	}


	public void SetFPSMode(bool fpsMode)
	{
		_fpsMode = fpsMode;

		if (_fpsMode)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
