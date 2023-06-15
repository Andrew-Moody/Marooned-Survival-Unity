using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyController : MonoBehaviour
{
	[SerializeField]
	private Transform _transform;

	private float _speed;

	private float _targetAngle;

	void Start()
	{
		GamePlay.Instance.TimeChangedEvent += OnTimeChanged;
		_transform.rotation = Quaternion.identity;
	}




	void Update()
	{
		if (_transform.rotation.z < _targetAngle)
		{
			float angle = _speed * Time.deltaTime;

			_transform.Rotate(Vector3.forward, angle);
		}
	}


	public void OnTimeChanged()
	{
		_targetAngle = GamePlay.Instance.CurrentHour * 15f;

		_speed = 15f / GamePlay.Instance.HourLength;
	}
}
