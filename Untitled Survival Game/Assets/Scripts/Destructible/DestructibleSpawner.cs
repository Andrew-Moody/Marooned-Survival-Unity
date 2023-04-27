using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleSpawner : NetworkBehaviour
{
	[SerializeField]
	private int _destructibleID;

	private float _delay = 0.5f;
	private bool _hasSpawned = false;

	public override void OnStartServer()
	{
		base.OnStartServer();
	}

	public override void OnStartClient()
	{
		base.OnStartClient();

		if (!IsServer)
		{
			this.enabled = false;
		}
	}


	private void Update()
	{
		if (!_hasSpawned)
		{
			_delay -= Time.deltaTime;

			if (_delay <= 0f)
			{
				SpawnDestructible(_destructibleID);
				_hasSpawned = true;
			}
		}
	}


	private void SpawnDestructible(int destructibleID)
	{
		Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 30f);

		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);

		DestructibleManager.Instance.SpawnDestructable(destructibleID, hitInfo.point, rotation, transform);
	}
}
