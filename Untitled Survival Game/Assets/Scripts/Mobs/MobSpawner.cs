using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : NetworkBehaviour
{
	[SerializeField]
	private string _mobName;


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

				SpawnMob(_mobName);
			}
		}
		else
		{
			if (IsServer && Input.GetKeyDown(KeyCode.P))
			{
				SpawnMob(_mobName);
			}
		}
	}


	[Server]
	private void SpawnMob(string mobName)
	{
		MobManager.Instance.SpawnMob(mobName, transform);
	}
}
