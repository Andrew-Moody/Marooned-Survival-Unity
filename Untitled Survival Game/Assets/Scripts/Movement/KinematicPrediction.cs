using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicPrediction : NetworkBehaviour
{
	[SerializeField]
	private Transform _headTransform;

	[SerializeField]
	private float _walkSpeed;

	[SerializeField]
	private float _sprintSpeed;

	[SerializeField]
	private float _jumpSpeed;

	[SerializeField]
	private LayerMask _groundMask;

    private CharacterController _controller;

	private Animator _animator;

    private bool _jumpQueued;

	private bool _jumping;


	private float _prefXRotation;
	private float _prefYRotation;



	[SerializeField]
	private Vector3 _velocity;

	private struct MoveData : IReplicateData
	{
		public float Horizontal;
		public float Vertical;
		public float YRotation;
		public float XRotation;
		public bool RotationChanged;
		public bool Sprint;
		public bool Jump;

		public MoveData(float horizontal, float vertical, float yRotation, float xRotation, bool rotationChanged, bool sprint, bool jump)
		{
			Horizontal = horizontal;
			Vertical = vertical;
			YRotation = yRotation;
			XRotation = xRotation;
			RotationChanged = rotationChanged;
			Sprint = sprint;
			Jump = jump;
			_tick = 0;
		}

		private uint _tick;
		public void Dispose() {}
		public uint GetTick() => _tick; 
		public void SetTick(uint value) => _tick = value;
	}


	private struct ReconcileData : IReconcileData
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public Vector3 Velocity;

		public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity)
		{
			Position = position;
			Rotation = rotation;
			Velocity = velocity;
			_tick = 0;
		}

		private uint _tick;
		public void Dispose() { }
		public uint GetTick() => _tick;
		public void SetTick(uint value) => _tick = value;
	}

	void Awake()
    {
        _controller = GetComponent<CharacterController>();
		_animator = transform.parent.GetComponentInChildren<Animator>();
	}


	private void Start()
	{
		// OnStartNetwork is too soon but this seems to work
		TimeManager.OnTick += TimeManager_OnTick;
	}


	void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Space))
		{
            _jumpQueued = true;
		}
    }


	private void GetInput(out MoveData data)
	{
		data = default;

		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		bool sprint = Input.GetKey(KeyCode.LeftShift);

		bool jump = _jumpQueued;
		_jumpQueued = false;

		// Get the players look direction (Currently controlled by CameraController, but should move to Input Manager)
		// Its acceptable for this to be client authoritive as generally speaking
		// The input that drives this can be easily faked, so theres no point in preventing the value itself from being faked

		float yRotation = CameraController.Instance.GetYRotation();
		float xRotation = CameraController.Instance.GetXRotation();

		bool rotationChanged = yRotation != _prefYRotation || xRotation != _prefXRotation;

		if (rotationChanged)
		{
			_prefXRotation = xRotation;
			_prefYRotation = yRotation;
		}


		if (horizontal == 0f && vertical == 0f && !jump && !rotationChanged)
		{
			return;
		}

		data = new MoveData(horizontal, vertical, yRotation, xRotation, rotationChanged, sprint, jump);
	}


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();
		Debug.Log("StartNetwork");
		// This is useless here because its too early to check if isowner and as is this will always be true
		if (IsServer || IsClient)
		{
			// Seems this is too early because move interferes with spawning. only fix so far was subscribe in Start()
            //TimeManager.OnTick += TimeManager_OnTick;
		}
	}


	public override void OnStopNetwork()
	{
		base.OnStopNetwork();
		if (TimeManager != null)
		{
			TimeManager.OnTick -= TimeManager_OnTick;
		}
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
		Debug.Log("StartClient");
		// Disable CharacterController if not owner
		_controller.enabled = (IsOwner || IsServer);
	}


	private void TimeManager_OnTick()
	{
        if (IsOwner)
		{
			Reconcile(default, false);

			GetInput(out MoveData moveData);

			Move(moveData, false);
		}

        if (IsServer)
		{
			Move(default, true);

			ReconcileData recData = new ReconcileData(transform.position, transform.rotation, _velocity);

			Reconcile(recData, true);
		}

		// I was setting the animator speed here but using the smooth follower script works so much better
	}


    [Replicate]
    private void Move(MoveData data, bool isServer, Channel channel = Channel.Unreliable, bool isReplaying = false)
	{
		if (data.RotationChanged)
		{
			// Set Rotation based on MoveData
			transform.rotation = Quaternion.Euler(0f, data.YRotation, 0f);

			// Head transform is seperate
			_headTransform.localRotation = Quaternion.Euler(data.XRotation, 0f, 0f);
		}


		Vector3 movement = new Vector3(data.Horizontal, 0f, data.Vertical).normalized;

		// Need to align this movement with the look direction

		movement = transform.TransformDirection(movement);

		//data.LookDirection;

		if (data.Sprint)
		{
			movement *= _sprintSpeed;
		}
		else
		{
			movement *= _walkSpeed;
		}

		_velocity.x = movement.x;
		_velocity.z = movement.z;

		float delta = (float)TimeManager.TickDelta;

		Vector3 origin = transform.position + new Vector3(0f, 0.4f, 0f);
		float radius = 0.5f;
		bool isGrounded = Physics.CheckSphere(origin, radius, _groundMask);


		if (data.Jump)
		{
			Debug.Log("Jump Requested" + isServer);
		}

		// This is begging for a statemachine
		if (data.Jump && isGrounded) // && !_jumping?
		{
			// Execute jump
			_jumping = true;
			_velocity.y = _jumpSpeed;

			Debug.Log("Jump");
		}

		if (_jumping && _velocity.y < 0f)
		{
			// Transition from jumping to falling
			_jumping = false;
		}

		if (isGrounded && !_jumping)
		{
			// Fall has ended (cant check while jumping as may be grounded a few frames at the start of the jump)
			_velocity.y = 0f;
		}
		
		if (!isGrounded && _velocity.y > -4f)
		{
			// Gravity applied in the air
			_velocity.y += (Physics.gravity.y * delta);
		}

		// Debug.Log("Jump: " + data.Jump + " Replaying: " + isReplaying + " VelY: " + _velocity.y);

		_controller.Move(_velocity * delta);
	}


	[Reconcile]
	private void Reconcile(ReconcileData data, bool isServer, Channel channel = Channel.Unreliable)
	{
		transform.position = data.Position;
		transform.rotation = data.Rotation;
		_velocity = data.Velocity;
	}
}
