using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : NetworkBehaviour
{
	[SerializeField]
	private string _mobName;


	private float _delay = 5f;
	private bool _hasSpawned = false;
	private bool _running = false;

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

		_running = true;
	}


	private void Update()
	{
		if (_running && !_hasSpawned)
		{
			_delay -= Time.deltaTime;

			if (_delay < 0f)
			{
				_hasSpawned = true;

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
		MobManager.SpawnMob(mobName, transform.position, transform.rotation);
	}
}
