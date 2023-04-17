using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using System;
using UnityEngine;


public struct DisplayData
{
	public double Time;

	public int Ticks;

	public int Reconciles;

	public int Desyncs;

	// Will always be zero on client
	public int ServerMoves;

	public int Moves;

	public int Replays;

	public float CumulativeDesync;

	public float MaxDesync;
}


public class PredictedMovementDebug : NetworkBehaviour
{
	private Rigidbody _rigidbody;

	private Animator _animator;

	[SerializeField]
	private Transform _graphicObject;

	[SerializeField]
	private float _moveRate;

	[SerializeField]
	private float _jumpForce;

	[SerializeField]
	private int RecRate;

	private float _xInput;

	private bool _isWalking;

	private DisplayData _displayData;

	private float _forceApplied;


	// Data type used to send players movement input to the server
	private struct MoveData : IReplicateData
	{
		public float Horizontal;
		public float Vertical;
		public bool Jump;

		// Constructor balks at _tick not being set before return not sure what it should be set too;
		//public MoveData(float horizontal, float vertical, bool jump)
		//{
		//	Horizontal = horizontal;
		//	Vertical = vertical;
		//	Jump = jump;
		//}

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
		public Quaternion Rotation;
		public Vector3 Velocity;
		public Vector3 AngularVelocity;

		//public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
		//{
		//	Position = position;
		//	Rotation = rotation;
		//	Velocity = velocity;
		//	AngularVelocity = angularVelocity;
		//}

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

		// Get control of the camera when client starts
		//CameraController.Instance.CaptureCamera(_graphicObject);
	}


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			_xInput += 1f;
		}

		if (Input.GetKeyDown(KeyCode.D))
		{
			_xInput -= 1f;
		}
	}


	// Equivalent to Unity's FixedUpdate. Called right before the server physics simulation
	private void TimeManager_OnTick()
	{
		UpdateDisplay();

		// Reconcile previous movement, gather new input, then perform new movement on the client
		if (base.IsOwner)
		{
			// Reconciliation to the previous correct position must be done before processing the next movement
			// When using reconcile on the client, pass default as the data and asServer = false
			// the data for reconciliation is cached by the server so default is used here
			Vector3 cachedPosition = transform.position;

			Reconcile(default, false);

			Vector3 deltaPos = cachedPosition - transform.position;

			//Debug.Log(deltaPos.x);

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
				ReconcileData data = new ReconcileData();
				data.Position = transform.position;
				data.Rotation = transform.rotation;
				data.Velocity = _rigidbody.velocity;
				data.AngularVelocity = _rigidbody.angularVelocity;

				// Reconciliation data can be sent every tick. Fish-Networking automatically detects when the data is unchanged to conserve bandwidth
				Reconcile(data, true);
			}
			
		}


		// Dont know if this is the best place for this
		//if (base.IsOwner)
		//{
		//	bool isWalking = false;

		//	if (Math.Abs(_rigidbody.velocity.x) > 0.01f || Math.Abs(_rigidbody.velocity.z) > 0.01f)
		//	{
		//		// Debug.Log(_rigidbody.velocity.x.ToString() + " " + _rigidbody.velocity.z.ToString());
		//		isWalking = true;
		//	}

		//	if (isWalking != _isWalking)
		//	{
		//		_isWalking = isWalking;
		//		// Debug.Log(gameObject.GetInstanceID().ToString() + " isWalking = " + _isWalking.ToString());
		//		_animator.SetBool("IsWalking", _isWalking);
		//	}
		//}
	}


	private void GetInput(out MoveData moveData)
	{
		moveData = default;

		// Example for now. Will want to get input from an input manager later on
		// Note that this isnt called during Unity Update so input will need to be cached
		//float horizontal = Input.GetAxisRaw("Horizontal");
		float horizontal = _xInput;
		_xInput = 0f;
		float vertical = Input.GetAxisRaw("Vertical");
		bool jump = Input.GetKeyDown(KeyCode.Space);

		// data is set to default if no input is recieved
		// This works behind the scenes to reduce redundancy when sending information to the server
		if (horizontal == 0f && vertical == 0f && !jump)
		{
			return;
		}
		else
		{
			Debug.Log("Input Recieved on Client Tick: " + base.TimeManager.LocalTick);
		}

		moveData.Horizontal = horizontal;
		moveData.Vertical = vertical;
		moveData.Jump = jump;
	}


	
	[Replicate]
	private void Move(MoveData moveData, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
	{
		// When manually called replaying will be false, but during reconciliation the inputs may be replayed
		// This may cause problems if audio/vfx are triggered multiple times, so a check if replaying is true can prevent that

		float jumpForce = 0f;
		if (moveData.Jump)
		{
			jumpForce = _jumpForce;
		}

		if (!replaying)
		{
			// Didnt help to check replaying (force was the same, number of physics simulations was not)

			// Example movement code
			//Vector3 force = new Vector3(moveData.Horizontal, jumpForce, moveData.Vertical) * _moveRate;
			//_rigidbody.AddForce(force);
		}

		Vector3 force = new Vector3(moveData.Horizontal, jumpForce, moveData.Vertical) * _moveRate;
		_rigidbody.AddForce(force);
		_forceApplied += force.x;

		if (asServer)
		{
			_displayData.ServerMoves++;
		}
		else
		{
			_displayData.Moves++;

			if (replaying)
			{
				_displayData.Replays++;

				Debug.Log("Replaying on Client Tick: " + base.TimeManager.LocalTick + " VelX: " + _rigidbody.velocity.x + " Force: " + _forceApplied);
			}
			else
			{
				Debug.Log("Moving on Client Tick: " + base.TimeManager.LocalTick + " VelX: " + _rigidbody.velocity.x + " Force: " + _forceApplied);
			}
		}

	}


	[Reconcile]
	private void Reconcile(ReconcileData data, bool asServer, Channel channel = Channel.Unreliable)
	{
		//Debug.Log("Client Tick: " + base.TimeManager.Tick + " Data Tick: " + data.GetTick());

		bool desync = false;
		Vector3 deltaPos = transform.position - data.Position;
		Vector3 deltaRot = transform.rotation.eulerAngles - data.Rotation.eulerAngles;
		Vector3 deltaVel = _rigidbody.velocity - data.Velocity;
		Vector3 deltaAVel = _rigidbody.angularVelocity - data.AngularVelocity;

		Debug.Log("Reconciling on Client Tick: " + base.TimeManager.LocalTick + " Data Tick: " + data.GetTick() + " VelX: " + _rigidbody.velocity.x + " DataX: " + data.Velocity.x);

		if (deltaPos.sqrMagnitude > 0f)
		{
			//Debug.Log("ClientPos: " + transform.position.x + ", " + transform.position.y + ", " + transform.position.z);
			//Debug.Log("ServerPos: " + data.Position.x + ", " + data.Position.y + ", " + data.Position.z);
			//Debug.Log("DeltaPos: " + deltaPos.x +", "+ deltaPos.y +", "+ deltaPos.z);
			desync = true;
		}

		if (deltaRot.sqrMagnitude > 0f)
		{
			//Debug.Log("DeltaRot: " + deltaRot.ToString());
			desync = true;
		}

		if (deltaVel.sqrMagnitude > 0f)
		{
			//Debug.Log("Client Tick: " + base.TimeManager.LocalTick + " Data Tick: " + data.GetTick() + " " + "DeltaVel: + " + deltaVel.x + ", " + deltaVel.y + ", " + deltaVel.z);
			//Debug.Log("DeltaVel: " + deltaVel.x + ", " + deltaVel.y + ", " + deltaVel.z);
			desync = true;
		}

		if (deltaAVel.sqrMagnitude > 0f)
		{
			//Debug.Log("DeltaAVel: " + deltaAVel.ToString());
			desync = true;
		}

		_displayData.Reconciles++;

		if (desync)
		{
			_displayData.Desyncs++;

			float absDesyncX = Math.Abs(deltaPos.x);

			_displayData.CumulativeDesync += absDesyncX;

			if (absDesyncX > _displayData.MaxDesync)
			{
				_displayData.MaxDesync = absDesyncX;
			}
		}


		// Here all the relevant properties are set back to what the server determines
		transform.position = data.Position;
		transform.rotation = data.Rotation;
		_rigidbody.velocity = data.Velocity;
		_rigidbody.angularVelocity = data.AngularVelocity;
	}


	private void UpdateDisplay()
	{
		if (base.IsClient && base.IsOwner)
		{
			_displayData.Time += base.TimeManager.TickDelta;

			if (_displayData.Time >= 1f)
			{
				HUD.Instance.UpdateStats(_displayData);

				_displayData.Time = 0f;
				_displayData.Ticks = 0;
				_displayData.Reconciles = 0;
				_displayData.Desyncs = 0;
				_displayData.ServerMoves = 0;
				_displayData.Moves = 0;
				_displayData.Replays = 0;
				_displayData.MaxDesync = 0f;
				_displayData.CumulativeDesync = 0f;
			}
		}

		_displayData.Ticks++;
	}
}
