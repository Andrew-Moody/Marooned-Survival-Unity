using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : NetworkBehaviour
{
	public static ItemManager Instance;

	[SerializeField]
	private GameObject WorldItemPrefab;

	[SerializeField]
	private List<ItemSO> itemSOs;

	private Dictionary<int, ItemSO> itemSODict;


	[SerializeField]
	private DestructibleFactory _destructibleFactory;

	// Start is called before the first frame update
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		itemSODict = new Dictionary<int, ItemSO>();

		for (int i = 0; i < itemSOs.Count; i++)
		{
			ItemSO item = itemSOs[i];

			if (itemSODict.ContainsKey(item.ItemID))
			{
				Debug.Log($"itemSO: {item.ItemName} Attempting to use same ID({item.ItemID}) as: {itemSODict[item.ItemID].ItemName}");
			}
			else
			{
				itemSODict.Add(item.ItemID, item);
			}
		}
	}


	private void Start()
	{
		//itemSODict = new Dictionary<int, ItemSO>();

		//for (int i = 0; i < itemSOs.Count; i++)
		//{
		//	ItemSO item = itemSOs[i];

		//	if (itemSODict.ContainsKey(item.ItemID))
		//	{
		//		Debug.Log($"itemSO: {item.ItemName} Attempting to use same ID({item.ItemID}) as: {itemSODict[item.ItemID].ItemName}");
		//	}
		//	else
		//	{
		//		itemSODict.Add(item.ItemID, item);
		//	}
		//}
	}


	public ItemSO GetItemSO(int id)
	{
		if (!itemSODict.TryGetValue(id, out ItemSO itemSO))
		{
			Debug.Log($"No ItemSO exists with ID: {id}");
		}

		return itemSO;
	}


	// Need to test but I think server attribute means only a server can call it?
	// Non servers will throw a warning if they attempt to call this so still good to check IsServer before calling
	[Server]
	public WorldItem SpawnWorldItem(ItemNetData itemNetData, Vector3 location)
	{
		if (itemNetData.ItemID == 0)
		{
			Debug.LogError("ItemManager attempted to spawn empty item");
			return null;
		}

		GameObject worldItemGO = Instantiate(WorldItemPrefab, location, Quaternion.identity, transform);

		WorldItem worldItem = worldItemGO.GetComponent<WorldItem>();
		if (worldItem == null)
		{
			Debug.LogError("ItemManager WorldItemPrefab must be of type WorldItem");
			return null;
		}

		// Spawn the a world item and setup on server
		InstanceFinder.ServerManager.Spawn(worldItemGO);
		worldItem.SetItem(itemNetData);

		// Setup world item on clients
		worldItem.ObserversSetupWorldItem(itemNetData);

		//Debug.LogError("SpawnedWorldItem");

		return worldItem;
	}


	[Server]
	public WorldItem SpawnWorldItem(int itemID, Vector3 location)
	{
		ItemNetData itemNetData = new ItemNetData(itemID, 1);

		return SpawnWorldItem(itemNetData, location);
	}


	public DestructibleSO GetPlacedItemSO(int itemID)
	{
		ItemSO itemSO = GetItemSO(itemID);

		return _destructibleFactory.GetDestructible(itemSO.PlacedItemID);
	}
}
