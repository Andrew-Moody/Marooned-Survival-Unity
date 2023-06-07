using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{
	[SerializeField]
	private Transform _networkTransform;

	[SerializeField]
	private ProjectileCollider _collider;

	[SerializeField]
	private float _speed;

	private bool _isBouncing;

	private Vector3 _preHitVelocity;

	private Vector3 _postHitVelocity;

	private Vector3 _prevPosition;
	public Vector3 PrevPos => _prevPosition;

	private Quaternion _prevRotation;

	private Vector3 _velocity;
	private Vector3 _angularVelocity;

	private Vector3 _acceleration;
	private Vector3 _angularAcceleration;

	private bool _stuck;
	private Transform _stuckTarget;

	//private bool _isLaunched = false;

	private Transform _followTarget;

	private Vector3 _offsetPos;
	private Quaternion _offsetRot;


	public void SetOffset(Vector3 position, Quaternion rotation)
	{
		_offsetPos = position;
		_offsetRot = rotation;
	}


	public void SetFollowTarget(Transform target)
	{
		_followTarget = target;

		if (_followTarget != null)
		{
			transform.position = _followTarget.TransformPoint(_offsetPos);
			transform.rotation = _offsetRot * _followTarget.rotation;
		}
	}


	public void Launch(Vector3 velocity)
	{
		//_isLaunched = true;

		_prevPosition = transform.position;
		_prevRotation = transform.rotation;

		_velocity = velocity;
	}


	public void Resolve(RaycastHit hitInfo)
	{
		Vector3 resolve = _prevPosition + hitInfo.distance * (transform.position - _prevPosition).normalized;

		transform.position = resolve;

		Debug.Log($"Resolve: {resolve.x}, {resolve.y}, {resolve.z}");
	}


	public void Halt()
	{
		_velocity = Vector3.zero;
		_acceleration = Vector3.zero;

		_angularVelocity = Vector3.zero;
		_angularAcceleration = Vector3.zero;
	}


	public void Stick(Transform stickTo)
	{
		_velocity = Vector3.zero;
		_acceleration = Vector3.zero;

		_angularVelocity = Vector3.zero;
		_angularAcceleration = Vector3.zero;

		_stuck = true;

		_stuckTarget = stickTo;
	}


	public void BounceStart(Collision collision)
	{
		_isBouncing = true;

		_postHitVelocity = Vector3.Reflect(_preHitVelocity, collision.GetContact(0).normal);
	}


	public void BounceEnd()
	{
		_isBouncing = false;
	}

	void Awake()
	{
		enabled = false;

		_isBouncing = false;

		_preHitVelocity = Vector3.zero;

		_postHitVelocity = Vector3.zero;
	}

	
	// Unclear if FixedUpdate would be better or worse but will most likely move to OnTick regardless
	void Update()
	{
		//if (_stuck)
		//{
		//	if (_stuckTarget == null)
		//	{
		//		Debug.LogError("StuckTarget null");
		//	}

		//	transform.position = _stuckTarget.position;
		//	transform.rotation = _stuckTarget.rotation;

		//	return;
		//}

		_prevPosition = transform.position;
		_prevRotation = transform.rotation;


		if (_followTarget != null)
		{
			transform.position = _followTarget.TransformPoint(_offsetPos);
			transform.rotation = _offsetRot * _followTarget.rotation;
		}
		else
		{
			_velocity += Time.deltaTime * _acceleration;

			_angularVelocity += Time.deltaTime * _angularAcceleration;

			transform.position += Time.deltaTime * _velocity;

			transform.rotation *= Quaternion.Euler(Time.deltaTime * _angularAcceleration);
		}


		
	}
}
