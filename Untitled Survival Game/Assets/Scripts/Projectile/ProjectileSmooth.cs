using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSmooth : MonoBehaviour
{
	[SerializeField]
	private Transform _target;

	private Vector3 _prevPos;
	private Quaternion _prevRot;



	void Start()
	{
		transform.position = _target.position;
		transform.rotation = _target.rotation;

		_prevPos = _target.position;
		_prevRot = _target.rotation;
	}


	void Update()
	{
		transform.position = Vector3.Lerp(_prevPos, _target.position, 0.5f);

		transform.rotation = Quaternion.Slerp(_prevRot, _target.rotation, 0.5f);

		_prevPos = _target.position;
		_prevRot = _target.rotation;
	}
}
