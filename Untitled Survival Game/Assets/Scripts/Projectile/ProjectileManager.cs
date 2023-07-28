using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IActor = Actors.IActor;

public class ProjectileManager : NetworkBehaviour
{
	public static ProjectileManager Instance => _instance;

	private static ProjectileManager _instance;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}


	[Server]
	public static ProjectileBase SpawnProjectile(ProjectileBase prefab, Vector3 position, Quaternion rotation, GameObject actorObject = null)
	{
		ProjectileBase proj = Instantiate(prefab, position, rotation, Instance.transform);
		Instance.Spawn(proj.gameObject);

		if (actorObject != null)
		{
			proj.SetOwningActorORPC(actorObject);
		}
		
		return proj;
	}
}
