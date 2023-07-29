using Actors;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePattern : ProjectileBase
{
	[SerializeField]
	private ProjectileEmitter[] _emitters;

	[SerializeField]
	private bool _keepVertical;

	

	[SerializeField]
	private bool _disposeChildren;

	private List<ProjectileBase> _projectiles;


	public override void Spawn()
	{
		base.Spawn();

		_projectiles = new List<ProjectileBase>();

		foreach (ProjectileEmitter emitter in _emitters)
		{
			ProjectileBase projectile = emitter.Spawn(OwningActor);

			projectile.SetFollowTarget(_networkTransform.transform);

			_projectiles.Add(projectile);
		}
	}


	public override void Launch(Vector3 velocity)
	{
		base.Launch(velocity);

		for (int i = 0; i < _projectiles.Count; i++)
		{
			velocity  = _emitters[i].transform.localRotation * velocity;

			_projectiles[i].Launch(velocity);
		}
	}


	public override void SetFollowTarget(Transform target)
	{
		base.SetFollowTarget(target);

		foreach (ProjectileBase projectile in _projectiles)
		{
			projectile.SetFollowTarget(target);
		}
	}


	public override void SetOwningActor(Actor actor)
	{
		base.SetOwningActor(actor);

		foreach (ProjectileBase projectile in _projectiles)
		{
			projectile.SetOwningActor(actor);
		}
	}

	public override void Dispose()
	{
		base.Dispose();

		if (_disposeChildren)
		{
			foreach (ProjectileBase projectile in _projectiles)
			{
				projectile.Dispose();
			}
		}
	}
}
