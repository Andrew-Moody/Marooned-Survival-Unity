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
	private bool _keepOffset;

	[SerializeField]
	private bool _keepVertical;

	[SerializeField]
	private bool _alignWithVelocity;


	private Vector3 _preHitVelocity;

	private Vector3 _postHitVelocity;

	private Vector3 _prevPosition;
	public Vector3 PrevPos => _prevPosition;

	private Quaternion _prevRotation;

	private Vector3 _velocity;
	private Vector3 _angularVelocity;

	private Vector3 _acceleration;
	private Vector3 _angularAcceleration;

	private Transform _followTarget;

	private Vector3 _offsetPos;
	private Quaternion _offsetRot;


	public void SetOffset(Vector3 position, Quaternion rotation)
	{
		_offsetPos = position;
		_offsetRot = rotation;
	}


	public virtual void SetFollowTarget(Transform target)
	{
		_followTarget = target;

		_offsetPos = Vector3.zero;
		_offsetRot = Quaternion.identity;

		if (target != null)
		{
			if (_keepOffset)
			{
				// find the offset from current world position relative to target transform
				_offsetPos = _followTarget.InverseTransformPoint(transform.position);

				// inverse of transform.rotation = _followTarget.rotation * _offsetRot;
				_offsetRot = Quaternion.Inverse(_followTarget.rotation) * transform.rotation;
			}
			else
			{
				// Snap to new position
				transform.position = _followTarget.TransformPoint(_offsetPos);
				transform.rotation = _followTarget.rotation * _offsetRot;
			}
		}
	}


	public void Launch(Vector3 velocity)
	{
		_followTarget = null;

		if (_alignWithVelocity)
		{
			transform.rotation = Quaternion.LookRotation(velocity);
		}

		_prevPosition = transform.position;
		_prevRotation = transform.rotation;

		_velocity = velocity;
	}


	public bool CheckCollision(out RaycastHit hitInfo)
	{
		if (_collider.CheckCollision(_prevPosition, transform.position, out hitInfo))
		{
			Resolve(hitInfo);

			//_projectileMotion.Stick(hitInfo.transform);

			Halt();

			return true;
		}

		return false;
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

		//_stuck = true;

		//_stuckTarget = stickTo;
	}


	public void BounceStart(Collision collision)
	{
		//_isBouncing = true;

		_postHitVelocity = Vector3.Reflect(_preHitVelocity, collision.GetContact(0).normal);
	}


	public void BounceEnd()
	{
		//_isBouncing = false;
	}

	void Awake()
	{
		enabled = false;

		//_isBouncing = false;

		_preHitVelocity = Vector3.zero;

		_postHitVelocity = Vector3.zero;
	}

	
	public void Tick(float deltaTime)
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
			// Still need to add keep vertical

			transform.position = _followTarget.TransformPoint(_offsetPos);
			transform.rotation = _followTarget.rotation * _offsetRot;
			// convention seems to be Qworld = Qparent * Qchild
			// old transform.rotation = _offsetRot * _followTarget.rotation;
		}
		else
		{
			_velocity += deltaTime * _acceleration;

			_angularVelocity += deltaTime * _angularAcceleration;

			transform.position += deltaTime * _velocity;

			transform.rotation *= Quaternion.Euler(deltaTime * _angularAcceleration);
		}
	}
}
