using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleManager : NetworkBehaviour
{
	public static DestructibleManager Instance;

    [SerializeField]
    private DestructibleFactory _destructibleFactory;


	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}


	[Server]
	public DestructibleObject SpawnDestructable(int destructibleID, Vector3 position, Quaternion rotation, Transform parent)
	{
		DestructibleSO destructibleSO = _destructibleFactory.GetDestructible(destructibleID);

		if (destructibleSO == null)
		{
			Debug.Log("Failed to Spawn DestructibleObject with ID: " + destructibleID);
			return null;
		}

		// Have to check that this does change position etc on clients (probably doesn't) Seems it does work
		DestructibleObject destructible = Instantiate(destructibleSO.Prefab, position, rotation, parent);
		// DestructibleObject destructible = Instantiate(destructiblePrefab, parent, false);

		Spawn(destructible.gameObject);

		return destructible;
	}


	[Server]
	public DestructibleObject PlaceItem(AbilityActor user, int destructibleID)
	{
		Transform view = user.ViewTransform;

		if (view == null)
		{
			return null;
		}


		Physics.Raycast(view.position, view.forward, out RaycastHit hitInfo, user.ViewRange, user.ViewMask);

		if (hitInfo.collider != null)
		{
			Vector3 right = Vector3.Cross(view.forward, hitInfo.normal).normalized;
			Vector3 forward = Vector3.Cross(right, hitInfo.normal).normalized;

			Quaternion rotation = Quaternion.LookRotation(forward, hitInfo.normal);

			return SpawnDestructable(destructibleID, hitInfo.point, rotation, transform);
		}


		return null;

		
	}
}
