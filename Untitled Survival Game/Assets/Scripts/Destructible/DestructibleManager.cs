using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityActor = LegacyAbility.AbilityActor;

public class DestructibleManager : NetworkBehaviour
{
	public static DestructibleManager Instance;

	[SerializeField]
	private DestructibleObject _prefab;

	[SerializeField]
	private DestructibleFactory _destructibleFactory;

	[SerializeField]
	private Transform _destructibleHolder;
	public Transform DestructibleHolder => _destructibleHolder;


	[SerializeField]
	private PlaceableItem[] _placeableItems;

	private Dictionary<int, int> _placeableItemDict;


	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		BuildPlaceableMap();
	}


	[Server]
	public DestructibleObject SpawnDestructible(int destructibleID, Vector3 position, Quaternion rotation)
	{
		DestructibleObject prefab = _destructibleFactory.GetPrefab(destructibleID);

		if (prefab == null)
		{
			Debug.Log("Failed to Spawn DestructibleObject with ID: " + destructibleID);
			return null;
		}

		// will this work if _destructibleHolder is a networkObject?
		// Aha that seems to be why parent wasn't set on spawn
		// When instantiating a prefab intended to be spawned on server the parent transform
		// needs to be attached to a gameObject that has a networkObject component (networkObject on root isn't enough)
		// (References don't serialize over the net if they aren't networkObjects)
		DestructibleObject dObject = Instantiate(prefab, position, rotation, _destructibleHolder);

		Spawn(dObject.gameObject);

		return dObject;
	}


	public DestructibleObject PlaceItem(int itemID, Vector3 position, Quaternion rotation)
	{
		if (!_placeableItemDict.TryGetValue(itemID, out int destructibleID))
		{
			Debug.LogError($"Attempted to place item: {itemID} with no corresponding destructibleID");
			return null;
		}

		return SpawnDestructible(destructibleID, position, rotation);
	}

	[Server]
	public DestructibleObject PlaceItem(AbilityActor user, int itemID)
	{
		if (!_placeableItemDict.TryGetValue(itemID, out int destructibleID))
		{
			Debug.LogError($"Attempted to place item: {itemID} with no corresponding destructibleID");
			return null;
		}


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

			return SpawnDestructible(destructibleID, hitInfo.point, rotation);
		}


		return null;
	}


	public DestructibleSO GetPlacedItemSO(int itemID)
	{
		if (_placeableItemDict.TryGetValue(itemID, out int destructibleID))
		{
			return _destructibleFactory.GetDestructible(destructibleID);
		}

		return null;
	}


	private void BuildPlaceableMap()
	{
		_placeableItemDict = new Dictionary<int, int>();

		for (int i = 0; i < _placeableItems.Length; i++)
		{
			_placeableItemDict[_placeableItems[i].ItemID] = _placeableItems[i].DestructibleID;
		}
	}


	[System.Serializable]
	private struct PlaceableItem
	{
		public int ItemID;

		public int DestructibleID;
	}
}
