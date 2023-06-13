using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawnController : MonoBehaviour
{
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
			MobManager.SpawnMob("Man", transform.position, transform.rotation);
		}
		
	}


	private void SpawnMob(string name)
	{

	}
}
