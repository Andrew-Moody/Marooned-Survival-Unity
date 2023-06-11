using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile : ProjectileBase
{
	[SerializeField]
	private ProjectileMotion _projectileMotion;

	[SerializeField]
	private Transform _graphic;

	[SerializeField]
	private ProjectileCollider _collider;

	[SerializeField]
	private AbilityActor _abilityActor;

	[SerializeField]
	private float _lifeTime;

	[SerializeField]
	private float _disposeDelay;

	[SerializeField]
	private AbilitySO _abilitySO;

	private Ability _ability;

	private bool _active = false;

	private bool _dying = false;

	


	private void Awake()
	{
		_ability = _abilitySO.GetRuntimeAbility();
	}


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
			_projectileMotion.SetOffset(_offsetPos, _offsetRot);
			_projectileMotion.enabled = IsServer;

			//UseAbilityORPC(null, EffectTiming.OnStart);
		}
	}


	public override void Launch(Vector3 velocity, bool align = false)
	{
		if (align)
		{
			_projectileMotion.transform.rotation = Quaternion.LookRotation(velocity);
		}


		_projectileMotion.Launch(velocity);

		UseAbilityORPC(null, EffectTiming.OnStart);

		_active = true;
	}


	public override void Dispose()
	{
		UseAbilityORPC(null, EffectTiming.OnEnd);

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

			AbilityActor target = hitInfo.transform.gameObject.GetComponent<AbilityActor>();

			UseAbilityORPC(target, EffectTiming.OnAnimEvent);

			Dispose();
		}
	}


	[ObserversRpc(RunLocally = true)]
	private void UseAbilityORPC(AbilityActor target, EffectTiming timing)
	{
		_ability.UseAbility(_abilityActor, null, target, timing, IsServer);
	}


	[ObserversRpc(RunLocally = true)]
	private void HideGraphicORPC()
	{
		_graphic.gameObject.SetActive(false);
	}
}
