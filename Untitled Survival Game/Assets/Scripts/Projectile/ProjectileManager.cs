using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : NetworkBehaviour
{
	public static ProjectileManager Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}


	[Server]
	public static ProjectileBase SpawnProjectile(ProjectileBase prefab)
	{
		ProjectileBase proj = Instantiate(prefab);
		Instance.Spawn(proj.gameObject);

		proj.Spawn(Vector3.zero, Quaternion.identity);

		return proj;
	}


	[Server]
	public static ProjectileBase SpawnProjectile(ProjectileBase prefab, Vector3 position, Quaternion rotation)
	{
		ProjectileBase proj = Instantiate(prefab);
		Instance.Spawn(proj.gameObject);

		proj.Spawn(position, rotation);

		return proj;
	}
}
