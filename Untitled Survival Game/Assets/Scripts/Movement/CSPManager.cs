using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSPManager : NetworkBehaviour
{
	public static CSPManager Instance;

	[SerializeField]
	private bool _cacheRecArray;

	private List<CSPObject> _predictedObjects = new List<CSPObject>();

	private CSPObject _controlledObject;

	private ReconcileDataPack _recDataPack;


	// Move to Input Handler
	private bool _jumpQueued;


	public void RegisterCSPObject(CSPObject nob)
	{
		_predictedObjects.Add(nob);

		// Attempt to reduce garbage created every frame
		_recDataPack = new ReconcileDataPack();
		_recDataPack.ReconcileDataArray = new ReconcileData[_predictedObjects.Count];
	}

	public void SetControlledObject(CSPObject nob)
	{
		_controlledObject = nob;
	}

	private void Awake()
	{
		//if (Instance == null)
		//{
		//	Instance = this;
		//}
	}


	private void Start()
	{
		if (IsServer || Owner.IsLocalClient)
		{
			TimeManager.OnTick += TimeManager_OnTick;
			TimeManager.OnPostTick += TimeManager_OnPostTick;
		}
	}

	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		// My notes say this is too early to subscribe to timemanager as it interferes with spawning?
		// Perhaps OnSpawnServer would work

		// Yes subscribing here causes the player to spawn at origin rather than networkmanager spawn point

		if (IsServer || Owner.IsLocalClient)
		{
			if (Instance == null)
			{
				Instance = this;
			}
			//TimeManager.OnTick += TimeManager_OnTick;
			//TimeManager.OnPostTick += TimeManager_OnPostTick;
		}

		// Doesnt work with Awake, OnStartServer or OnStartClient either
		// Start seems to work
	}


	public override void OnStartServer()
	{
		base.OnStartServer();

		// May not work but worth a try
		//TimeManager.OnTick += TimeManager_OnTick;
		//TimeManager.OnPostTick += TimeManager_OnPostTick;
	}


	public override void OnStartClient()
	{
		base.OnStartClient();
		if (!IsServer)
		{
			//TimeManager.OnTick += TimeManager_OnTick;
			//TimeManager.OnPostTick += TimeManager_OnPostTick;
		}
	}

	public override void OnStopNetwork()
	{
		base.OnStopNetwork();

		if (TimeManager != null)
		{
			TimeManager.OnTick -= TimeManager_OnTick;
			TimeManager.OnPostTick -= TimeManager_OnPostTick;
		}
	}


	private void TimeManager_OnTick()
	{
		if (IsOwner)
		{
			Reconcile(default, false);
			GetInput(out InputData data);
			Replicate(data, false);
		}

		if (IsServer)
		{
			Replicate(default, true);
		}
	}


	private void TimeManager_OnPostTick()
	{
		if (IsServer)
		{
			GetReconcileData(out ReconcileDataPack data);

			Reconcile(data, true);
		}
	}

	private void GetInput(out InputData data)
	{
		data = default;

		// probably best to have a seperate input handler
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		bool sprint = Input.GetKey(KeyCode.LeftShift);

		bool jump = _jumpQueued;
		_jumpQueued = false;

		if (horizontal == 0f && vertical == 0f && !jump)
		{
			return;
		}

		data = new InputData(horizontal, vertical, sprint, jump);
	}

	private void GetReconcileData(out ReconcileDataPack data)
	{
		if (_cacheRecArray)
		{
			data = _recDataPack;
		}
		else
		{
			data = new ReconcileDataPack();
			data.ReconcileDataArray = new ReconcileData[_predictedObjects.Count];
		}
		

		for (int i = 0; i < _predictedObjects.Count; i++)
		{
			data.ReconcileDataArray[i] = _predictedObjects[i].GetReconcileData();
		}
	}

	[Replicate]
	private void Replicate(InputData data, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
	{
		//Debug.Log("Manager Replicate, as Server: " + asServer + " " + _controlledObject);
		if (_controlledObject != null)
		{
			_controlledObject.Replicate(data, asServer);
		}
		
	}


	[Reconcile]
	private void Reconcile(ReconcileDataPack data, bool asServer, Channel channel = Channel.Unreliable)
	{
		if (_predictedObjects.Count == data.ReconcileDataArray.Length)
		{
			for (int i = 0; i < _predictedObjects.Count; i++)
			{
				_predictedObjects[i].Reconcile(data.ReconcileDataArray[i]);
			}
		}
		else
		{
			// Didn't allocate datapack correctly
			Debug.LogWarning("Number of predicted objects doesn't match the length of Reconcile data array");
		}
		
	}



	// Temporary input handling, move to input manager
	void Update()
	{
		if (!IsOwner) return;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			_jumpQueued = true;
		}
	}

}

// Data type used to send players movement input to the server
public struct InputData : IReplicateData
{
	public float Horizontal;
	public float Vertical;
	public bool Sprint;
	public bool Jump;

	public InputData(float horizontal, float vertical, bool sprint, bool jump)
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
public struct ReconcileData : IReconcileData
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


public struct ReconcileDataPack : IReconcileData
{
	public ReconcileData[] ReconcileDataArray;

	private uint _tick;
	public void Dispose() { }
	public uint GetTick() => _tick;
	public void SetTick(uint value) => _tick = value;
}