using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientAuthTest : NetworkBehaviour
{
	public float _walkSpeed;
	public float _sprintSpeed;
	public float _jumpSpeed;
	public LayerMask _groundMask;

	private bool _jumpQueued;

	[SerializeField]
	private float _jumpForce;


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


	public override void OnStartClient()
	{
		base.OnStartClient();

		if (IsOwner)
		{
			TimeManager.OnTick += TimeManager_OnTick;
		}
	}


	public override void OnStopClient()
	{
		base.OnStopClient();

		if (TimeManager != null)
		{
			TimeManager.OnTick -= TimeManager_OnTick;
		}
	}

	void Update()
	{
		if (!IsOwner) return;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			_jumpQueued = true;
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

	private void TimeManager_OnTick()
	{
		GetInput(out InputData data);

		Move(data);
	}




	public void Move(InputData data)
	{
		float jumpForce = 0f;
		if (data.Jump)
		{
			Vector3 origin = transform.position + new Vector3(0f, 0.5f, 0f);
			float radius = 0.5f;

			bool isGrounded = Physics.CheckSphere(origin, radius, _groundMask);

			if (isGrounded)
			{
				jumpForce = _jumpForce;
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
}
