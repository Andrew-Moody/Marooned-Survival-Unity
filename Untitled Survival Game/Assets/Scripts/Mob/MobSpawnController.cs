using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawnController : MonoBehaviour
{
	[SerializeField]
	private int _maxSpawns;

	[SerializeField]
	private string[] _mobs;

	[SerializeField]
	private bool _spawnInDaytime;

	[SerializeField]
	private float _delay = 5f;

	[SerializeField]
	private float _minDistance = 5f;

	[SerializeField]
	private float _maxDistance = 10f;

	private float _timeToSpawn;

	void Awake()
	{
		enabled = false;

		_timeToSpawn = _delay;
	}


	void Update()
	{
		if (_timeToSpawn > 0f)
		{
			_timeToSpawn -= Time.deltaTime;
		}
		else
		{
			if (MobManager.MobCount < _maxSpawns && (_spawnInDaytime || GamePlay.Instance.CurrentHour > 11))
			{
				string mob = _mobs[Random.Range(0, _mobs.Length)];

				SpawnMobAtRandomPlayer(mob);
			}

			_timeToSpawn = _delay;
		}
		
	}


	private void SpawnMobAtRandomPlayer(string name)
	{
		Vector3 playerPos = GamePlay.GetRandomPlayer().transform.position;

		Vector3 spawnPos = Vector3.zero;
		spawnPos.x = Random.Range(-1f, 1f);
		spawnPos.z = Random.Range(-1f, 1f);

		spawnPos.Normalize();

		spawnPos.x = spawnPos.x * (_maxDistance - _minDistance) + Mathf.Sign(spawnPos.x) * _minDistance;
		spawnPos.z = spawnPos.z * (_maxDistance - _minDistance) + Mathf.Sign(spawnPos.z) * _minDistance;
		
		spawnPos += playerPos;

		if (CheckSpawnLocation(ref spawnPos))
		{
			MobManager.SpawnMob(name, spawnPos, Quaternion.identity);
		}


		Debug.LogError($"Player {playerPos} Spawn {spawnPos}");
	}


	private bool CheckSpawnLocation(ref Vector3 location)
	{
		location.y = 10f;

		if (Physics.Raycast(location, Vector3.down, out RaycastHit hitInfo, 20f, LayerMask.GetMask("Ground")))
		{
			location = hitInfo.point;
			return true;
		}

		return false;
	}
}
