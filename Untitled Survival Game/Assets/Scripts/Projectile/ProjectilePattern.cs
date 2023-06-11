using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePattern : ProjectileBase
{
	[SerializeField]
	private ProjectileBase[] _projectiles;

	[SerializeField]
	private bool _keepVertical;

	[SerializeField]
	private float _lifeTime;

	private bool _isLaunched;

	public override void Spawn(Vector3 position, Quaternion rotation)
	{
		_networkTransform.transform.position = position;

		_networkTransform.transform.rotation = rotation;

		if (_keepVertical)
		{
			Vector3 forward = _networkTransform.transform.forward;
			_networkTransform.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
		}
		

		for (int i = 0; i < _projectiles.Length; i++)
		{
			//_projectiles[i].Spawn(_projectiles[i].transform.localPosition, _projectiles[i].transform.localRotation);

			_projectiles[i].Spawn(_networkTransform.transform.position, _networkTransform.transform.rotation);

			_projectiles[i].SetFollowTarget(_networkTransform.transform);
		}

		//for (int i = 0; i < _projectiles.Length; i++)
		//{
		//	_projectiles[i].transform.SetParent(ProjectileManager.Instance.transform);
		//	_projectiles[i].transform.localPosition = Vector3.zero;
		//	_projectiles[i].transform.localRotation = Quaternion.identity;
		//}

		//ResetParentsORPC();
	}


	public override void Launch(Vector3 velocity, bool align = false)
	{
		_isLaunched = true;

		for (int i = 0; i < _projectiles.Length; i++)
		{
			// Get the relative rotation from group to individual
			Quaternion rot = _projectiles[i].NetworkTransform.rotation * Quaternion.Inverse(_networkTransform.transform.rotation);

			Vector3 vel = rot * velocity;

			_projectiles[i].Launch(vel, align);

			_projectiles[i].SetFollowTarget(null);
		}
	}


	public override void Dispose()
	{

	}


	private void Update()
	{
		if (_followTarget != null)
		{
			_networkTransform.transform.position = _followTarget.position;

			_networkTransform.transform.rotation = _followTarget.rotation;

			if (_keepVertical)
			{
				Vector3 forward = _networkTransform.transform.forward;
				_networkTransform.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
			}
		}

		if (_isLaunched && IsSpawned)
		{
			if (_lifeTime < 0f)
			{
				Despawn();
			}

			_lifeTime -= Time.deltaTime;
		}
	}


	[ObserversRpc(BufferLast = true)]
	private void ResetParentsORPC()
	{
		for (int i = 0; i < _projectiles.Length; i++)
		{
			_projectiles[i].transform.SetParent(ProjectileManager.Instance.transform);
			_projectiles[i].transform.localPosition = Vector3.zero;
			_projectiles[i].transform.localRotation = Quaternion.identity;
		}
		
	}
}
