using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Managing;
using System;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private float _smoothTime;

	[SerializeField]
	private List<CameraAnchor> _cameraAnchors;

	[SerializeField]
	private Transform _playerFollower;

	[SerializeField]
	private EquipmentCamera _equipmentCamera;
	public EquipmentCamera EquipmentCamera => _equipmentCamera;

	private Camera _camera;

	private int _currentAnchorIndex = 1;

	private CameraAnchor _currentAnchor;

	public Transform _graphicTarget;

	public Transform _networkTarget;

	private Vector3 _velocity;

	public static CameraController Instance { get; private set; }


	private void Awake()
	{
		Instance = this;
	}


	private void Start()
	{
		_camera = Camera.main;

		SetCameraMode(_currentAnchorIndex);
	}


	public void SetPlayer(GameObject player)
	{
		if (player != null)
		{
			// The transform of the graphical Object;
			_graphicTarget = PlayerLocator.Player.transform.parent.GetComponentInChildren<SmoothFollower>().transform;

			// The transform of the actual NetworkObject
			_networkTarget = PlayerLocator.Player.transform;
		}
	}


	public void IncrementCameraMode()
	{
		_currentAnchorIndex++;

		if (_currentAnchorIndex >= _cameraAnchors.Count)
		{
			_currentAnchorIndex = 0;
		}

		SetCameraMode(_currentAnchorIndex);
	}


	private void SetCameraMode(int anchorIndex)
	{
		_currentAnchor = _cameraAnchors[anchorIndex];

		_camera.transform.SetParent(_currentAnchor.PositionAnchor, false);
	}


	private void LateUpdate()
	{
		Tick(Time.deltaTime);
	}
	private void Tick(float deltaTime)
	{
		// I Have no explaination why, but updating the camera in PostTick does not work
		// Its okay with no interpolation, but jitter occurs with interpolation and gets worse the more frames you interpolate over
		// For now it seems that lateUpdate does work just fine despite the two different time systems
		// I dont really understand how this works because the actual movement is being done in OnTick (fishnet timemanager)
		// I suspect the predictedObject component handles interpolation in unity update rather than OnTick
		// Perhaps the graphical object can be freely moved in update to follow the root because the interpolation is client side only


		// The tutorial uses two separate transforms
		// The camera holder has a script that updates the position to the player position in update
		// rather than parent the camera under the player because of rigidbody wierdness

		// I actually decided to change my setup to be closer to the tutorial
		// He used Update but LateUpdate would be better normally
		// Unfortunatly for me, fishnet handles predicted movement with its own timemanager
		// Ill have to test the best place to move this to (Input will still need to be cached in regular Update)
		// Probably TimeManager.OnPostTick would be the best place

		// Update 6/21/23
		// For now I am following the graphic object not the network transform directly
		// In this case LateUpdate ensures the graphic object has applied the most current result
		// smoothing is not strictly needed here as the graphic object already handles smoothing
		// but smoothing can still be used here if you want a damping effect to the camera follow
		// This also means there should be no problem using cinemachine as no networked objects are involved
		
		// At the moment only the y rotation need be applied to the player follower as the x rotation is really the players head
		// not the x rotation of the body. Instead the x rotation is applied to the camera anchors

		if (_graphicTarget != null)
		{
			_playerFollower.position = Vector3.SmoothDamp(_playerFollower.position, _graphicTarget.position, ref _velocity, _smoothTime);
			_playerFollower.rotation = Quaternion.Euler(0f, PlayerInput.YRotation, 0f);

			// handle x rotation for the player view (not of the players body)
			_currentAnchor.RotationAnchor.localRotation = Quaternion.Euler(PlayerInput.XRotation, 0f, 0f);
		}
	}
}


[System.Serializable]
public struct CameraAnchor
{
	public Transform PositionAnchor;

	public Transform RotationAnchor;

	public bool FollowTarget;
}