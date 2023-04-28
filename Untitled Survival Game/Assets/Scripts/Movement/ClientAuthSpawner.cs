using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientAuthSpawner : NetworkBehaviour
{

	public GameObject prefab;



	public override void OnStartClient()
	{
		base.OnStartClient();

		ServerSpawnPrefab(LocalConnection, PlayerLocator.Player.transform.position);
	}


	[ServerRpc(RequireOwnership = false)]
	private void ServerSpawnPrefab(NetworkConnection connection, Vector3 position)
	{
		position.y = 3f;

		GameObject obj = Instantiate(prefab, position, Quaternion.identity);

		Spawn(obj, connection);
	}
}
