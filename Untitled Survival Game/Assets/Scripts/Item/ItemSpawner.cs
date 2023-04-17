using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
	[SerializeField]
	private int _itemID;

	public int SpawnRate;

	private WorldItem WorldItem;

	private float _timeTillSpawn;

	private void Start()
	{
		_timeTillSpawn = SpawnRate;
	}


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		// Cant rely on the the order of network vs unity callbacks
		if (!IsServer)
		{
			gameObject.SetActive(false);
			return;
		}
	}

	[Server]
	public void SpawnItem()
	{
		WorldItem = ItemManager.Instance.SpawnWorldItem(_itemID, transform.position);
	}


	private void Update()
	{
		if (!IsServer) return;

		if (WorldItem == null)
		{
			_timeTillSpawn -= Time.deltaTime;

			if (_timeTillSpawn <= 0f)
			{
				SpawnItem();
				_timeTillSpawn += SpawnRate;
			}
		}
	}
}
