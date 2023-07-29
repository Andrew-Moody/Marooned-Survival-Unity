using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor = Actors.Actor;

public class ProjectileEmitter : MonoBehaviour
{
	public GameObject ProjectileBase => _projectilePrefab;
	[SerializeField] private GameObject _projectilePrefab;


	public ProjectileBase Spawn(Actor actor)
	{
		ProjectileBase prefab = _projectilePrefab.GetComponent<ProjectileBase>();
		return ProjectileManager.SpawnProjectile(prefab, transform.position, transform.rotation, actor);
	}

	private void OnValidate()
	{
		if (_projectilePrefab != null && _projectilePrefab.GetComponent<ProjectileBase>() == null)
		{
			_projectilePrefab = null;
			Debug.LogWarning("Prefab must have a Projectile component");
		}
	}
}
