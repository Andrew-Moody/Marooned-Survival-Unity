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
		if (destructibleID != 303)
		{
			return null;
		}

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



	[Server]
	public DestructibleObject SpawnDestructibleFromSO(int destructibleID, Vector3 position, Quaternion rotation, Transform parent)
	{
		DestructibleSO destructibleSO = _destructibleFactory.GetDestructible(destructibleID);

		if (destructibleSO == null)
		{
			Debug.Log("Failed to Spawn DestructibleObject with ID: " + destructibleID);
			return null;
		}
		// Update 7/22/23 parents are set on spawn only if the parent is a networkObject (not only a child of one)
		// Changes to the parent after spawning are still only synced in pro version I believe

		// Local position is changed (even with no network transform) but the parent is not set on clients
		// NetworkTransform does not let you automatically sync parent unless you have pro version
		// As a result you will always need an RPC to set the parent on an Instantiated Prefab
		// Note that a transform or gameobject sent in an RPC will always be null if not attached to a NetworkObject

		DestructibleObject destructible = Instantiate(destructibleSO.BasePrefab, position, rotation);

		Spawn(destructible.gameObject);

		destructible.InitializeORPC(destructibleSO.ID);

		return destructible;
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


	public DestructibleSO GetDestructibleSO(int id)
	{
		return _destructibleFactory.GetDestructible(id);
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
