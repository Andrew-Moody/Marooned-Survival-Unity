using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

public class ProjectileBase : NetworkBehaviour
{
	public Transform NetworkTransform => _networkTransform.transform;
	[SerializeField] protected NetworkBehaviour _networkTransform;


	public Transform FollowTarget => _followTarget;
	protected Transform _followTarget;

	protected Vector3 _spawnPos;
	protected Quaternion _spawnRot;

	public IActor OwningActor => _owningActor;
	private IActor _owningActor;


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		// Preserve the spawn position and rotation
		_spawnPos = transform.position;
		_spawnRot = transform.rotation;

		// Reset Projectile root object transform
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		// Move the network transform to the starting position
		_networkTransform.transform.position = _spawnPos;
		_networkTransform.transform.rotation = _spawnRot;
	}


	[Server]
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
	public void SetOwningActorORPC(GameObject actorObject)
	{
		if (actorObject != null)
		{
			Debug.LogWarning("OwningActor set to null");
			return;
		}

		if (!actorObject.TryGetComponent(out IActor actor))
		{
			Debug.LogWarning("ActorObject missing an IActor component");
		}

		_owningActor = actor;
	}


	[ObserversRpc(RunLocally = true, BufferLast = true)]
	protected void SpawnORPC(Vector3 position, Quaternion rotation)
	{
		Debug.LogError("Projectile SpawnORPC");

		// Preserve the starting position of the prefab
		_spawnPos = transform.position;
		_spawnRot = transform.rotation;

		// Reparent and reset Projectile root object
		transform.SetParent(ProjectileManager.Instance.transform, false);
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		// Move the network transform to the starting position
		_networkTransform.transform.position = _spawnPos + position;
		_networkTransform.transform.rotation = _spawnRot * rotation;
	}
}
