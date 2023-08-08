using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

public class ProjectileBase : NetworkBehaviour
{
	public Transform NetworkTransform => _networkTransform.transform;
	[SerializeField] protected NetworkBehaviour _networkTransform;

	[SerializeField] private float _lifeTime;

	[SerializeField] private float _deathTime;

	public Transform FollowTarget => _followTarget;
	protected Transform _followTarget;

	protected Vector3 _spawnPos;
	protected Quaternion _spawnRot;

	public Actor OwningActor => _owningActor;
	private Actor _owningActor;

	public ProjectileState State => _state;
	protected ProjectileState _state;

	protected int _layerMask;
	
	private float _timeRemaining;


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		SetSpawnTransform();
	}


	public override void OnStartServer()
	{
		base.OnStartServer();

		Spawn();

		TimeManager.OnTick += TimeManager_OnTick;
	}


	private void OnDestroy()
	{
		TimeManager.OnTick -= TimeManager_OnTick;
	}


	private void TimeManager_OnTick()
	{
		Tick((float)TimeManager.TickDelta);
	}

	[Server]
	public virtual void Spawn()
	{
		_state = ProjectileState.Spawned;
	}


	[Server]
	public virtual void Launch(Vector3 velocity)
	{
		_state = ProjectileState.Launched;
		_timeRemaining = _lifeTime;
	}


	[Server]
	public virtual void Dispose()
	{
		_timeRemaining = _deathTime;
		_state = ProjectileState.Disposing;
	}


	[Server]
	public virtual void SetFollowTarget(Transform target)
	{
		_followTarget = target;
	}


	[Server]
	public virtual void SetOwningActor(Actor actor)
	{
		_owningActor = actor;

		if (actor != null)
		{
			_layerMask = actor.HostilityMask.value;
		}
	}


	protected virtual void Tick(float deltaTime)
	{
		if (_timeRemaining < 0f)
		{
			if (_state == ProjectileState.Launched && IsSpawned)
			{
				Dispose();
			}
			else if (_state == ProjectileState.Disposing)
			{
				_state = ProjectileState.Dead;
				Despawn();
			}
		}

		_timeRemaining -= deltaTime;
	}


	private void SetSpawnTransform()
	{
		// Preserve the spawn position and rotation
		_spawnPos = transform.position;
		_spawnRot = transform.rotation;

		// Reset Projectile root object transform
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		if (IsServer)
		{
			// Move the network transform to the starting position
			_networkTransform.transform.position = _spawnPos;
			_networkTransform.transform.rotation = _spawnRot;
		}
	}


	public enum ProjectileState
	{
		Spawned,
		Launched,
		Disposing,
		Dead
	}

}
