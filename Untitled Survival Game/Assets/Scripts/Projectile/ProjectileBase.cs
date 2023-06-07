using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : NetworkBehaviour
{
	[SerializeField]
	protected NetworkBehaviour _networkTransform;
	public Transform NetworkTransform => _networkTransform.transform;

	protected Transform _followTarget;
	public Transform FollowTarget => _followTarget;


	public virtual void SetFollowTarget(Transform target)
	{
		_followTarget = target;
	}


	public virtual void Spawn(Vector3 position, Quaternion rotation)
	{
		if (IsServer)
		{

		}
	}


	public virtual void Launch(Vector3 velocity, bool align = false)
	{

	}


	public virtual void Dispose()
	{

	}
}
