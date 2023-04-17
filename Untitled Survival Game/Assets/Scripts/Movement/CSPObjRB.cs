using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSPObjRB : CSPObject
{
	[SerializeField]
	private float _walkSpeed;
	
	[SerializeField]
	private float _sprintSpeed;

	[SerializeField]
	private float _jumpSpeed;

	[SerializeField]
	public LayerMask _groundMask;

	[SerializeField]
	private float _damping;

	private Rigidbody _rigidbody;


	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();

		if (_rigidbody == null)
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
		return new ReconcileData(transform.position, transform.rotation.eulerAngles, _rigidbody.velocity, _rigidbody.angularVelocity);
	}

	public override void Replicate(InputData data, bool asServer)
	{
		float jumpForce = 0f;
		if (data.Jump)
		{
			Vector3 origin = transform.position + new Vector3(0f, 0.5f, 0f);
			float radius = 0.5f;

			bool isGrounded = Physics.CheckSphere(origin, radius, _groundMask);

			if (isGrounded)
			{
				jumpForce = _jumpSpeed;
			}
		}

		float targetSpeed = _walkSpeed;
		if (data.Sprint)
		{
			targetSpeed = _sprintSpeed;
		}

		Vector3 targetVelocity = new Vector3(data.Horizontal, 0f, data.Vertical).normalized * targetSpeed;

		Vector3 deltaVelocity = targetVelocity - _rigidbody.velocity;
		Vector3 newVelocity = deltaVelocity * (1 - _damping);
		newVelocity.y = 0f;

		_rigidbody.AddForce(newVelocity, ForceMode.VelocityChange);

		_rigidbody.AddForce(new Vector3(0f, jumpForce, 0f));

		//Vector3 force = new Vector3(moveData.Horizontal, jumpForce, moveData.Vertical) * _moveRate;
		//_rigidbody.AddForce(force);
	}


	public override void Reconcile(ReconcileData data)
	{
		transform.position = data.Position;
		transform.rotation = Quaternion.Euler(data.Rotation);
		_rigidbody.velocity = data.Velocity;
		_rigidbody.angularVelocity = data.AngularVelocity;
	}


	private void Update()
	{
		if (!IsServer && !IsOwner)
		{
			_rigidbody.isKinematic = true;

			Debug.Log(_rigidbody.velocity);
		}
	}
}
