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
	private ParticleSystem _startParticle;

	[SerializeField]
	private ParticleSystem _endParticle;

	[SerializeField]
	private Ability _ability;


	public override void OnStartClient()
	{
		base.OnStartClient();

		this.enabled = IsServer;

		_projectileMotion.enabled = IsServer;

		_startParticle.Play();
	}


	public override void SetFollowTarget(Transform target)
	{
		_followTarget = target;

		_projectileMotion.SetFollowTarget(target);
	}


	public override void Launch(Vector3 velocity)
	{
		base.Launch(velocity);

		_projectileMotion.Launch(velocity);
	}


	public override void Dispose()
	{
		base.Dispose();
		HideGraphicORPC();
	}


	protected override void Tick(float deltaTime)
	{
		base.Tick(deltaTime);

		_projectileMotion.Tick(deltaTime);

		if (State == ProjectileState.Launched)
		{
			HandleCollision();
		}
	}


	private void HandleCollision()
	{
		if (_projectileMotion.CheckCollision(out RaycastHit hitInfo, _layerMask))
		{
			if (hitInfo.collider.gameObject.TryGetComponent(out ActorFinder actorFinder))
			{
				ActivateAbility(actorFinder.Actor.AbilityActor);
			}

			Dispose();
		}
	}


	[ObserversRpc(RunLocally = true)]
	private void HideGraphicORPC()
	{
		_graphic.gameObject.SetActive(false);

		_endParticle.Play();
	}


	[ObserversRpc(RunLocally = true)]
	private void ActivateAbility(AbilityActor target)
	{
		AbilityActor user = null;
		if (OwningActor != null)
		{
			OwningActor.gameObject.TryGetComponent(out user);
		}
		
		AbilityEventData data = new ProjectileActivateEventData() { Target = target };

		AbilityHandle handle = new AbilityHandle(_ability, user, AbilityInput.Primary, data);

		handle.Activate();
	}
}
