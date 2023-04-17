using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTest : MonoBehaviour
{
	public float _walkSpeed;
	public float _sprintSpeed;
	public float _jumpSpeed;
	public LayerMask _groundMask;

	[SerializeField]
	private CharacterController _controller;

	private Vector3 _velocity;
	private bool _jumping;
	private bool _sprint;


	private void Update()
	{
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");

		bool jump = Input.GetKeyDown(KeyCode.Space);
		bool sprint = Input.GetKeyDown(KeyCode.LeftShift);


		//Debug.Log("Replicate, as Server: " + asServer);
		Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;

		if (sprint)
		{
			movement *= _sprintSpeed;
		}
		else
		{
			movement *= _walkSpeed;
		}

		_velocity.x = movement.x;
		_velocity.z = movement.z;

		float delta = Time.deltaTime;

		Vector3 origin = transform.position + new Vector3(0f, 0.4f, 0f);
		float radius = 0.5f;
		bool isGrounded = Physics.CheckSphere(origin, radius, _groundMask);


		// This is begging for a statemachine
		if (jump && isGrounded) // && !_jumping?
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
}
