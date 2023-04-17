using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSPObjCC : CSPObject
{
	public float _walkSpeed;
	public float _sprintSpeed;
	public float _jumpSpeed;
	public LayerMask _groundMask;

	private CharacterController _controller;
	private bool _jumping;


	private void Awake()
	{
		_controller = GetComponent<CharacterController>();

		if (_controller == null)
		{
			Debug.LogWarning($"CSPObjCC ({gameObject}) requires a CharacterController component");
		}
	}


	public override void OnStartServer()
	{
		base.OnStartServer();


		CSPManager cspManager = GetComponent<CSPManager>();

		if (cspManager != null)
		{
			cspManager.RegisterCSPObject(this);
			cspManager.SetControlledObject(this);
		}
		else
		{
			cspManager = PlayerLocator.Player.GetComponent<CSPManager>();
			cspManager.RegisterCSPObject(this);
		}


	}


	public override void OnStartClient()
	{
		base.OnStartClient();

		if (!IsServer)
		{
			CSPManager cspManager = GetComponent<CSPManager>();

			if (cspManager != null)
			{
				cspManager.RegisterCSPObject(this);
				cspManager.SetControlledObject(this);
			}
			else
			{
				cspManager = PlayerLocator.Player.GetComponent<CSPManager>();
				cspManager.RegisterCSPObject(this);
			}
		}
	}


	public override ReconcileData GetReconcileData()
	{
		return new ReconcileData(transform.position, transform.rotation.eulerAngles, _velocity, _angularVelocity);
	}

	public override void Replicate(InputData data, bool asServer)
	{
		//Debug.Log("Replicate, as Server: " + asServer);
		Vector3 movement = new Vector3(data.Horizontal, 0f, data.Vertical).normalized;

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
			Debug.Log("Jump Requested" + asServer);
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

	public override void Reconcile(ReconcileData data)
	{
		transform.position = data.Position;
		transform.rotation = Quaternion.Euler(data.Rotation);
		_velocity = data.Velocity;
		_angularVelocity = data.AngularVelocity;
	}
}
