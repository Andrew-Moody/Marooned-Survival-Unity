using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : NetworkBehaviour
{
	[SerializeField]
	private int _mobID;


	private float _delay = 0.5f;
	private bool hasSpawned = false;

	public override void OnStartClient()
	{
		base.OnStartClient();
		if (!IsServer)
		{
			this.enabled = false;
		}
		
	}


	public override void OnStartServer()
	{
		base.OnStartServer();		
	}


	private void Update()
	{
		if (!hasSpawned)
		{
			_delay -= Time.deltaTime;

			if (_delay < 0f)
			{
				hasSpawned = true;

				SpawnMob(_mobID);
			}
		}
	}


	[Server]
	private void SpawnMob(int mobID)
	{
		MobManager.Instance.SpawnMob(mobID, transform);
	}
}
