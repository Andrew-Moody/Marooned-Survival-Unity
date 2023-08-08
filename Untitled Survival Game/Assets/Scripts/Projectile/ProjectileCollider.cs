using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProjectileCollider : MonoBehaviour
{
	[SerializeField]
	private CollisionType _collisionType;

	[SerializeField]
	private Vector3 _center = Vector3.zero;

	[SerializeField]
	private Vector3 _boundingBox = Vector3.one;

	[SerializeField]
	private float _radius = 1f;

	[SerializeField]
	private float _length = 1f;

	[SerializeField]
	private Vector3 _direction = Vector3.up;

	private Vector3 _up;

	private Vector3 _right;

	private Vector3 _forward;

	private Quaternion _orientation;

	private Matrix4x4 _mat;

	private Vector3 _halfExtents;


	void Awake()
	{
		CalculateBounds();	
	}


	private void CalculateBounds()
	{
		_up = _direction.normalized;

		if (_up != Vector3.forward)
		{
			_right = Vector3.Cross(_up, Vector3.forward).normalized;
		}
		else
		{
			_right = Vector3.Cross(_up, Vector3.down).normalized;
		}

		_forward = Vector3.Cross(_right, _up).normalized;

		_orientation = Quaternion.LookRotation(_forward, _up);

		_mat = new Matrix4x4(_right, _up, _forward, _center);

		if (_collisionType == CollisionType.Box)
		{
			
		}
		else if (_collisionType == CollisionType.Sphere)
		{
			_boundingBox = 2f * _radius * Vector3.one;
		}
		else if (_collisionType == CollisionType.Capsule)
		{
			_boundingBox.x = 2f * _radius;
			_boundingBox.z = 2f * _radius;
			_boundingBox.y = 2f * _radius + _length;
		}
		//else if (_collisionType == CollisionType.Cylinder)
		//{
		//	_boundingBox.x = 2f * _radius;
		//	_boundingBox.z = 2f * _radius;
		//	_boundingBox.y = _length;
		//}


		_halfExtents = 0.5f * _boundingBox;
	}


	public bool CheckCollision(Vector3 prevPos, Vector3 currPos, out RaycastHit hitInfo, int layerMask)
	{
		Vector3 center = prevPos + _center;
		Vector3 direction = currPos - prevPos;
		Quaternion orientation = transform.rotation * _orientation;
		float maxDistance = direction.magnitude;

		if (_collisionType == CollisionType.Sphere)
		{
			bool hit = Physics.SphereCast(center, _radius, direction, out hitInfo, maxDistance, layerMask);

			if (!hit && Physics.CheckSphere(center, _radius, layerMask))
			{
				if (Physics.SphereCast(center - (10f * direction), _radius, direction, out hitInfo, 11f * maxDistance, layerMask))
				{
					//Debug.LogWarning("SphereCast success starting from center minus direction");

					return true;
				}
				else
				{
					if (Physics.CheckSphere(center - (10f * direction), _radius, layerMask))
					{
						Debug.LogWarning("CheckSphere true at center - 10 * direction");
					}
					else
					{
						Debug.LogWarning("SphereCast failed but CheckSphere did not");
					}
					
				}
			}


			return hit;
		}

		return Physics.BoxCast(center, _halfExtents, direction, out hitInfo, orientation, maxDistance, layerMask);
	}


#if UNITY_EDITOR


	private void OnValidate()
	{
		CalculateBounds();
	}


	private void OnDrawGizmosSelected()
	{
		Vector3[] boundLines = BuildLines();

		Handles.color = Color.green;
		Handles.DrawLines(boundLines);

		Handles.color = Color.green;
		Handles.DrawLine(Vector3.zero, _up);

		Handles.color = Color.red;
		Handles.DrawLine(Vector3.zero, _right);

		Handles.color = Color.blue;
		Handles.DrawLine(Vector3.zero, _forward);

		Vector3 bottom = -_length * 0.5f * _up + _center;
		Vector3 top = _length * 0.5f * _up + _center;

		Handles.color = Color.red;

		if (_collisionType == CollisionType.Box)
		{
			Handles.DrawLines(boundLines);
		}
		else if (_collisionType == CollisionType.Sphere)
		{
			Handles.DrawWireDisc(_center, _up, _radius);
			Handles.DrawWireDisc(_center, _right, _radius);
			Handles.DrawWireDisc(_center, _forward, _radius);
		}
		//else if (_collisionType == CollisionType.Cylinder)
		//{
		//	Handles.DrawWireDisc(bottom, _up, _radius);
		//	Handles.DrawWireDisc(top, _up, _radius);

		//	Handles.DrawLine(bottom + _radius * _right, top + _radius * _right);
		//	Handles.DrawLine(bottom + _radius * -_right, top + _radius * -_right);
		//	Handles.DrawLine(bottom + _radius * _forward, top + _radius * _forward);
		//	Handles.DrawLine(bottom + _radius * -_forward, top + _radius * -_forward);
		//}
		else if (_collisionType == CollisionType.Capsule)
		{
			Handles.DrawWireDisc(bottom, _up, _radius);
			Handles.DrawWireDisc(top, _up, _radius);

			Handles.DrawLine(bottom + _radius * _right, top + _radius * _right);
			Handles.DrawLine(bottom + _radius * -_right, top + _radius * -_right);
			Handles.DrawLine(bottom + _radius * _forward, top + _radius * _forward);
			Handles.DrawLine(bottom + _radius * -_forward, top + _radius * -_forward);

			Handles.DrawWireArc(top, _forward, _right, 180f, _radius);
			Handles.DrawWireArc(top, _right, _forward, -180f, _radius);

			Handles.DrawWireArc(bottom, _forward, _right, -180f, _radius);
			Handles.DrawWireArc(bottom, _right, _forward, 180f, _radius);
		}
	}


	private Vector3[] BuildLines()
	{
		_halfExtents = 0.5f * _boundingBox;

		Vector3[] boundLines = new Vector3[24];

		Vector3[] points =
		{
			new Vector3(1, 1, 1),
			new Vector3(1, -1, 1),
			new Vector3(1, 1, -1),
			new Vector3(1, -1, -1),

			new Vector3(1, 1, 1),
			new Vector3(-1, 1, 1),
			new Vector3(1, 1, -1),
			new Vector3(-1, 1,-1),

			new Vector3(1, 1, 1),
			new Vector3(1, -1, 1),
			new Vector3(-1, 1, 1),
			new Vector3(-1, -1, 1),
		};

		for (int i = 0; i < points.Length; i++)
		{
			points[i] = Vector3.Scale(points[i], _halfExtents);
		}

		for (int axis = 0; axis < 3; axis++)
		{
			for (int line = 0; line < 4; line++)
			{
				int start = axis * 8 + line * 2;
				int ptidx = axis * 4 + line;

				Vector3 point = points[ptidx];
				boundLines[start] = transform.TransformPoint((Vector3)(_mat * point) + _center);
				point[axis] *= -1;
				boundLines[start + 1] = transform.TransformPoint((Vector3)(_mat * point) + _center);
			}
		}

		return boundLines;
	}


#endif
}

public enum CollisionType
{
	None,
	Box,
	Sphere,
	Capsule,
	//Cylinder,
}