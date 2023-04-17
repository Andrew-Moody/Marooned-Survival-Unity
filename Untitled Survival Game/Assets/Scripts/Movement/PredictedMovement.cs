using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using System;
using UnityEngine;


public class PredictedMovement : NetworkBehaviour
{
	[SerializeField]
	private Transform _graphicObject;

	[SerializeField]
	private float _moveRate;

	[SerializeField]
	private float _maxSpeed;

	[SerializeField]
	private float _walkSpeed;

	[SerializeField]
	private float _sprintSpeed;

	[SerializeField]
	private float _damping;

	[SerializeField]
	private float _jumpForce;

	[SerializeField]
	private LayerMask groundMask;

	[SerializeField]
	private int RecRate;

	private float _xInput;

	private bool _jumpQueued;

	private Rigidbody _rigidbody;

	private Animator _animator;


	// Data type used to send players movement input to the server
	private struct MoveData : IReplicateData
	{
		public float Horizontal;
		public float Vertical;
		public bool Sprint;
		public bool Jump;

		public MoveData(float horizontal, float vertical, bool sprint, bool jump)
		{
			Horizontal = horizontal;
			Vertical = vertical;
			Sprint = sprint;
			Jump = jump;
			_tick = 0;
		}

		// Required to implement Interface
		private uint _tick;
		public void Dispose() { }
		public uint GetTick() => _tick;
		public void SetTick(uint value) => _tick = value;
	}


	// Data type used by the server to correct or reconcile the clients movement properties
	// Any variables that might affect the clients position or velocity etc should be corrected
	private struct ReconcileData : IReconcileData
	{
		public Vector3 Position;
		public Vector3 Rotation;
		public Vector3 Velocity;
		public Vector3 AngularVelocity;

		public ReconcileData(Vector3 position, Vector3 rotation, Vector3 velocity, Vector3 angularVelocity)
		{
			Position = position;
			Rotation = rotation;
			Velocity = velocity;
			AngularVelocity = angularVelocity;
			_tick = 0;
		}

		private uint _tick;
		public void Dispose() { }
		public uint GetTick() => _tick;
		public void SetTick(uint value) => _tick = value;
	}


	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();

		_animator = GetComponentInChildren<Animator>();

		InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
		InstanceFinder.TimeManager.OnPostTick += TimeManager_OnPostTick;
	}

	private void OnDestroy()
	{
		if (InstanceFinder.TimeManager != null)
		{
			InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
			InstanceFinder.TimeManager.OnPostTick -= TimeManager_OnPostTick;
		}
	}

	public override void OnStartClient()
	{
		base.OnStartClient();

		if (IsOwner)
		{
			// Get control of the camera when client starts
			//CameraController.Instance.CaptureCamera(_graphicObject);
		}
		
		if (!IsOwner && !IsServer)
		{
			// Hmm Kinematic gets turned back off by itself
			//_rigidbody.isKinematic = true;

			// Doesnt work cause its accessed by the smoother
			//Destroy(_rigidbody);

			_rigidbody.useGravity = false;
		}

	}


	private void Update()
	{
		if (!IsOwner) return;

		if (Input.GetKeyDown(KeyCode.A))
		{
			_xInput += 1f;
		}

		if (Input.GetKeyDown(KeyCode.D))
		{
			_xInput -= 1f;
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			_jumpQueued = true;
		}
	}


	// Equivalent to Unity's FixedUpdate. Called right before the server physics simulation
	private void TimeManager_OnTick()
	{
		// Reconcile previous movement, gather new input, then perform new movement on the client
		if (base.IsOwner)
		{
			// Reconciliation to the previous correct position must be done before processing the next movement
			// When using reconcile on the client, pass default as the data and asServer = false
			// the data for reconciliation is cached by the server so default is used here
			Reconcile(default, false);

			// Get the input data used to process movement for this frame
			GetInput(out MoveData moveData);

			// Perform movement on the client directly using the gathered input with asServer = false
			// Replaying value is not needed here
			Move(moveData, false);
		}


		// perform movement on the server
		if (base.IsServer)
		{
			// default is passed as the data with asServer = true
			// the input data is cached and automatically uses the correct input on the server
			// Replaying value is not needed here
			Move(default, true);

			// Reconcile data is calculated in OnPostTick after physics is simulated for physics based movement
			// Reconcile data can be calculated here for non-physics based controllers
		}
		
	}


	// Called right after server physics simulation (not needed if not using a physics based controller)
	private void TimeManager_OnPostTick()
	{
		//Debug.Log("OnPostTick: " + GetInstanceID());
		// Here the server builds the reconcile data based on the servers knowledge of the clients status
		// Anything that might effect movement must included in this data including positions, rotations, and velocities not only of the players rigibody
		// but also anything else used for movement like wheel coliders on a car for example
		// Another example would be things like stamina that effect whether a player is allowed to run

		// The video didnt check if this was server which was causing errors on the second client
		// The documentation example does but data was built in OnTick not OnPostTick (Nonphysics controller)
		if (base.IsServer)
		{
			if (base.TimeManager.Tick % RecRate == 0)
			{
				// Reconciliation data can be sent every tick. Fish-Networking automatically detects when the data is unchanged to conserve bandwidth
				ReconcileData data = new ReconcileData(transform.position, transform.rotation.eulerAngles, _rigidbody.velocity, _rigidbody.angularVelocity);
				Reconcile(data, true);
			}
			
		}


		// Dont know if this is the best place for this
		if (base.IsOwner)
		{
			float speed = _rigidbody.velocity.z / _maxSpeed;
			_animator.SetFloat("Speed", speed);
		}
	}


	private void GetInput(out MoveData moveData)
	{
		moveData = default;

		// Example for now. Will want to get input from an input manager later on
		// Note that this isnt called during Unity Update so some input will need to be cached
		//float horizontal = _xInput;
		//_xInput = 0f;
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		bool sprint = Input.GetKey(KeyCode.LeftShift);

		bool jump = _jumpQueued;
		_jumpQueued = false;

		// data is set to default if no input is recieved
		// This works behind the scenes to reduce redundancy when sending information to the server
		if (horizontal == 0f && vertical == 0f && !jump)
		{
			return;
		}

		moveData = new MoveData(horizontal, vertical, sprint, jump);
	}


	
	[Replicate]
	private void Move(MoveData moveData, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
	{
		// When manually called replaying will be false, but during reconciliation the inputs may be replayed
		// This may cause problems if audio/vfx are triggered multiple times, so a check if replaying is true can prevent that

		float jumpForce = 0f;
		if (moveData.Jump)
		{
			Vector3 origin = transform.position + new Vector3(0f, 0.5f, 0f);
			float radius = 0.5f;

			bool isGrounded = Physics.CheckSphere(origin, radius, groundMask);

			if (isGrounded)
			{
				jumpForce = _jumpForce;
			}		
		}

		float targetSpeed = _walkSpeed;
		if (moveData.Sprint)
		{
			targetSpeed = _sprintSpeed;
		}

		Vector3 targetVelocity = new Vector3(moveData.Horizontal, 0f, moveData.Vertical).normalized * targetSpeed;

		Vector3 deltaVelocity = targetVelocity - _rigidbody.velocity;
		Vector3 newVelocity = deltaVelocity * (1 - _damping);
		newVelocity.y = 0f;

		_rigidbody.AddForce(newVelocity, ForceMode.VelocityChange);

		_rigidbody.AddForce(new Vector3(0f, jumpForce, 0f));

		//Vector3 force = new Vector3(moveData.Horizontal, jumpForce, moveData.Vertical) * _moveRate;
		//_rigidbody.AddForce(force);
	}


	[Reconcile]
	private void Reconcile(ReconcileData data, bool asServer, Channel channel = Channel.Unreliable)
	{
		// Here all the relevant properties are set back to what the server determines
		transform.position = data.Position;
		transform.rotation = Quaternion.Euler(data.Rotation);
		_rigidbody.velocity = data.Velocity;
		_rigidbody.angularVelocity = data.AngularVelocity;
	}
}
