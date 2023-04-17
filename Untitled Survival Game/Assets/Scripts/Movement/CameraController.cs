using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Managing;

public class CameraController : NetworkBehaviour
{
    // Loosely based on tutorial by "Dave / GameDevelopment" (added y inversion, ability to toggle off, and integration with fishnetworking)

    public float SenX;
    public float SenY;
    public bool InvertY;

    [SerializeField]
    private float _smoothTime;

    [SerializeField]
    private List<CameraAnchor> _cameraAnchors;

    private Camera _camera;

    private int _currentAnchorIndex;

    private CameraAnchor _currentAnchor;

    public Transform _graphicTarget;

    public Transform _networkTarget;

    private float _xRotation;
    private float _yRotation;
    private bool _fpsMode;

    private Vector3 _velocity;

    public static CameraController Instance { get; private set; }


    public bool GetFPSMode()
	{
        return _fpsMode;
	}

    public float GetYRotation()
	{
        return _yRotation;
	}


    public float GetXRotation()
	{
        return _xRotation;
	}

	private void Awake()
	{
        Instance = this;
	}

	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

        _camera = Camera.main;

        SetCameraMode(_currentAnchorIndex);
    }

	public override void OnStartClient()
	{
		base.OnStartClient();
        
        // The transform of the graphical Object;
        _graphicTarget = PlayerLocator.Player.transform.parent.GetComponentInChildren<SmoothFollower>().transform;

        // The transform of the actual NetworkObject
        _networkTarget = PlayerLocator.Player.transform;
    }


	void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
		{
            _currentAnchorIndex++;

            if (_currentAnchorIndex >= _cameraAnchors.Count)
			{
                _currentAnchorIndex = 0;
			}

            SetCameraMode(_currentAnchorIndex);
		}


        if (Input.GetKeyDown(KeyCode.L))
        {
            SetFPSMode(!_fpsMode);
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
		{
            SetFPSMode(false);
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
    }


    private void SetFPSMode(bool fpsMode)
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


    private void SetCameraMode(int anchorIndex)
    {
        _currentAnchor = _cameraAnchors[anchorIndex];

        _camera.transform.SetParent(_currentAnchor.PositionAnchor, false);

        if (!_currentAnchor.FollowTarget)
		{
            transform.position = Vector3.zero;
		}

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


        // This tutorial uses two separate transforms
        // The camera holder has a script that updates the position to the player position in update
        // rather than parent the camera under the player because of rigidbody wierdness

        // I actually decided to change my setup to be closer to the tutorial
        // He used Update but LateUpdate would be better normally
        // Unfortunatly for me, fishnet handles predicted movement with its own timemanager
        // Ill have to test the best place to move this to (Input will still need to be cached in regular Update)
        // Probably TimeManager.OnPostTick would be the best place

        

        if (_currentAnchor.FollowTarget && _graphicTarget != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, _graphicTarget.position, ref _velocity, _smoothTime);
        }

        if (!_fpsMode)
            return;

        // handle rotation for the camera (not a child of player)
        //transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);

        _currentAnchor.RotationAnchor.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);

        // Allow prediction to get the resultant Y Rotation from here (move to Input Manager)
        // Allow prediction to drive the network object transform taking Y Rotation into account
        // Let the graphical object follow along smoothly in SmoothFollower
        // Possibly add smoothing to camera rotation as well but thats purely client side


        // Rotate the Graphic object (probably dont. let smooth follower handle this instead)
        //_graphicTarget.rotation = Quaternion.Euler(0, _yRotation, 0); // Only handles horizontal rotation

        // Rotate the NetworkObject transform (this is the only "safe" client authoritive movement)
        // Better yet have a dummy transform that accumulates changes due to input in update (or just the yRotation)
        // Then use that to get lookDirection in the kinematic prediction script
        // _networkTarget.rotation = Quaternion.Euler(0f, _yRotation, 0f);
    }
}


[System.Serializable]
public struct CameraAnchor
{
    public Transform PositionAnchor;

    public Transform RotationAnchor;

    public bool FollowTarget;
}