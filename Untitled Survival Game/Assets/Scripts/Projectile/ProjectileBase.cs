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


	protected Vector3 _offsetPos;
	protected Quaternion _offsetRot;


	public virtual void SetFollowTarget(Transform target)
	{
		_followTarget = target;
	}


	public virtual void Spawn(Vector3 position, Quaternion rotation)
	{
		if (IsServer)
		{
			SpawnORPC(position, rotation);
		}
	}


	public virtual void Launch(Vector3 velocity, bool align = false)
	{

	}


	public virtual void Dispose()
	{

	}


	[ObserversRpc(RunLocally = true, BufferLast = true)]
	protected void SpawnORPC(Vector3 position, Quaternion rotation)
	{
		Debug.LogError("Projectile SpawnORPC");

		// Preserve the starting position of the prefab
		_offsetPos = transform.position;
		_offsetRot = transform.rotation;

		// Reparent and reset Projectile root object
		transform.SetParent(ProjectileManager.Instance.transform, false);
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		// Move the network transform to the starting position
		_networkTransform.transform.position = _offsetPos + position;
		_networkTransform.transform.rotation = _offsetRot * rotation;
	}
}
