using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : NetworkBehaviour
{
	private Stats _stats;

	private AbilityActor _abilityActor;

	[SerializeField]
	private float _deathTime;

	[SerializeField]
	private int _itemToSpawn;

	[SerializeField]
	private GameObject _graphicObject;

	[SerializeField]
	private WorldStatDisplay _statDisplay;


	private bool _isAlive;

	private bool _isDying;

	private float _deathTimeLeft;


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		_abilityActor = GetComponent<AbilityActor>();

		_stats = GetComponent<Stats>();

		_stats.OnStatEmpty += OnStatEmpty;

		if (IsServer)
		{
			TimeManager.OnPostTick += OnTick;
		}
		else
		{
			enabled = false;
		}

		_isAlive = true;

		_isDying = false;

		_deathTimeLeft = 0f;
	}


	private void OnDestroy()
	{
		if (_stats != null)
		{
			_stats.OnStatEmpty -= OnStatEmpty;
		}
	}

	private void OnStatEmpty(StatType statType)
	{
		if (statType == StatType.Health)
		{
			OnDeathStart();
		}
	}


	private void OnDeathStart()
	{
		Debug.Log("OnDeathStart " + TimeManager.Tick);

		// This is a little to soon to be despawning since this will get triggered by the stat  change on the server
		// before the OnChange has been executed on the client
		//Despawn(gameObject);

		// Need some way for the server to know Death effects are finished even though they dont play on the server
		_isDying = true;

		_deathTimeLeft = _deathTime;

		// I would really like either a way to know the syncvar send rate or better a callback when syncvars are actually sent
		// The default sync rate of 0.1 is probably enough
		if (_deathTimeLeft < 0.1f)
		{
			_deathTimeLeft = 0.1f;
		}

		_abilityActor.IsAlive = false;

		_abilityActor.PlayParticles("Death");

		_graphicObject.SetActive(false);

		_statDisplay.Show(false);

		GetComponent<Collider>().enabled = false;

		if (IsServer)
		{
			Vector3 spawnPosition = new Vector3(0f, 0.5f, 0f) + transform.position;

			ItemManager.Instance.SpawnWorldItem(_itemToSpawn, spawnPosition);
		}
	}


	[Server]
	private void OnDeathFinish()
	{
		Debug.Log("OnDeathFinish " + TimeManager.Tick);

		Despawn(gameObject);
	}

	private void OnTick()
	{
		// Maybe better to move to Timemanager
		// I hoped waiting one frame would be enough time to prevent
		// OnDeathFinish from occuring before the OnChange event was sent to clients

		// Check this before to force a one tick delay between condition changing and responding
		// Did not work on client host

		// So Since syncvars update on a time based interval and may take several ticks before sync is sent to client
		// There is no way to know for sure that the OnChanged callback will be called on the client before
		// Executing something initiated by OnChanged callback called on the server

		// The only way I can really think is to do some form of handshake (potentially unsafe)
		// Or wait at least a minimum amount of time (ie. At least as long as the sync rate
		// And there no method to get sync rate from server :} 0.1 seconds is the default and seems enough
		// May need to double it if packets are being recieved out of order not sure how that works

		if (IsSpawned && !_isAlive)
		{
			OnDeathFinish();
		}


		if (_isAlive && _isDying)
		{
			_deathTimeLeft -= (float)TimeManager.TickDelta;

			if (_deathTimeLeft <= 0f)
			{
				_isDying = false;
				_isAlive = false;
			}
		}
	}

}
