using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;
using Actors;


public class Projectile : ProjectileBase
{
	[SerializeField]
	private ProjectileMotion _projectileMotion;

	[SerializeField]
	private Transform _graphic;

	[SerializeField]
	private GameObject _startParticle;

	[SerializeField]
	private GameObject _endParticle;

	[SerializeField]
	private ProjectileCollider _collider;

	[SerializeField]
	private float _lifeTime;

	[SerializeField]
	private float _disposeDelay;

	[SerializeField]
	private Ability _ability;

	private bool _active = false;

	private bool _dying = false;



	private void Start()
	{
		enabled = IsServer;
	}


	public override void SetFollowTarget(Transform target)
	{
		if (target == null)
		{
			Debug.LogError("Followtarget is null");
		}
		else
		{
			Debug.LogError($"FollowTarget {target.gameObject.name}");
		}

		_followTarget = target;

		_projectileMotion.SetFollowTarget(target);
	}


	public override void Spawn(Vector3 position, Quaternion rotation)
	{
		if (IsServer)
		{
			SpawnORPC(position, rotation);

			Debug.Log("Setting Offset");
			_projectileMotion.SetOffset(_spawnPos, _spawnRot);
			_projectileMotion.enabled = IsServer;
		}
	}


	public override void Launch(Vector3 velocity, bool align = false)
	{
		if (align)
		{
			_projectileMotion.transform.rotation = Quaternion.LookRotation(velocity);
		}

		_projectileMotion.Launch(velocity);

		_active = true;
	}


	public override void Dispose()
	{
		HideGraphicORPC();

		_active = false;
		_dying = true;
	}


	private void Update()
	{
		if (!IsSpawned)
		{
			return;
		}

		if (_active)
		{
			if (_lifeTime < 0f)
			{
				_active = false;
				_dying = true;
			}

			_lifeTime -= Time.deltaTime;

			HandleCollision();
		}
		else if (_dying)
		{
			if (_disposeDelay < 0f)
			{
				Despawn();
			}

			_disposeDelay -= Time.deltaTime;
		}
	}


	private void HandleCollision()
	{
		Vector3 prevPos = _projectileMotion.PrevPos;
		Vector3 currPos = _projectileMotion.transform.position;

		if (_collider.CheckCollision(prevPos, currPos, out RaycastHit hitInfo))
		{
			_projectileMotion.Resolve(hitInfo);

			//_projectileMotion.Stick(hitInfo.transform);

			_projectileMotion.Halt();

			if (hitInfo.collider.gameObject.TryGetComponent(out AbilityActor target))
			{
				ActivateAbility(target);
			}

			Dispose();
		}
	}


	[ObserversRpc(RunLocally = true)]
	private void HideGraphicORPC()
	{
		_graphic.gameObject.SetActive(false);

		_endParticle.GetComponent<ParticleSystem>().Play();
	}


	[ObserversRpc(RunLocally = true)]
	private void ActivateAbility(AbilityActor target)
	{
		AbilityActor user = null;
		
		if (OwningActor is Actor actor)
		{
			actor.gameObject.TryGetComponent(out user);
		}

		AbilityHandle handle = new AbilityHandle(_ability, user);

		AbilityEventData data = new AbilityEventData()
		{
			Target = target,
		};

		handle.AbilityData.AbilityEventData = data;

		handle.Activate();
	}
}
