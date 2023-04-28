using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollower : MonoBehaviour
{
	[SerializeField]
	private Transform _target;

	[SerializeField]
	private float _smoothTime;

	[SerializeField]
	private float _sprintSpeed;

	private Animator _animator;

	private Vector3 _velocity = Vector3.zero;

	private Vector3 _localVelocity;


	private void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
	}


	// Update is called once per frame
	void Update()
	{
		if (_target != null)
		{
			transform.position = Vector3.SmoothDamp(transform.position, _target.position, ref _velocity, _smoothTime);

			transform.rotation = Quaternion.Slerp(transform.rotation, _target.rotation, Time.deltaTime / _smoothTime);

			// Target is currently not rotating so something else is actually setting the rotation
			// CameraController is setting rotation in LateUpdate
			// Maybe have CameraController directly control the network object rotation
			// And have this follow that
			//Debug.Log(_target.rotation);
			//Debug.Log(transform.rotation);

			_localVelocity = transform.InverseTransformDirection(_velocity);


			if (_animator != null)
			{
				_animator.SetFloat("ForwardSpeed", _localVelocity.z / _sprintSpeed);
			}
		}
	}


	private void OnDrawGizmos()
	{

		//Gizmos.color = Color.blue;
		//Gizmos.DrawLine(transform.position, transform.position + _velocity);


		//Gizmos.color = Color.red;
		//Gizmos.DrawLine(transform.position, transform.position + _localVelocity);
	}
}
