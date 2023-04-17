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
		DestructibleObject destructiblePrefab = _destructibleFactory.GetPrefab(destructibleID);

		if (destructiblePrefab == null)
		{
			Debug.Log("Failed to Spawn DestructibleObject with ID: " + destructibleID);
			return null;
		}

		// Have to check that this does change position etc on clients (probably doesn't)
		DestructibleObject destructible = Instantiate(destructiblePrefab, position, rotation, parent);
		// DestructibleObject destructible = Instantiate(destructiblePrefab, parent, false);

		Spawn(destructible.gameObject);

		return destructible;
	}
}
