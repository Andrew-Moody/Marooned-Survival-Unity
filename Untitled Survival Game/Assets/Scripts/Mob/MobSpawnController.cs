using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawnController : MonoBehaviour
{
	[SerializeField]
	private int _maxSpawns;

	private float _delay = 5f;

	void Awake()
	{
		if (!InstanceFinder.IsServer)
		{
			enabled = false;
		}
	}


	void Update()
	{
		if (_delay > 0f)
		{
			_delay -= Time.deltaTime;
		}
		else
		{
			if (MobManager.MobCount < _maxSpawns)
			MobManager.SpawnMob("Man", transform.position, transform.rotation);
		}
		
	}


	private void SpawnMob(string name)
	{

	}
}
