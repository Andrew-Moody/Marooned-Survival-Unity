using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenManager : NetworkBehaviour
{
	private static WorldGenManager _instance;
	public static WorldGenManager Instance => _instance;


	[SerializeField]
	private ResourceGenerator _resourceGenerator;

	public event System.Action FinishedWorldGenEvent;

	private bool _worldGenerated = false;
	private float _worldGenDelay = 0.5f;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	private void Update()
	{
		if (!_worldGenerated)
		{
			_worldGenDelay -= Time.deltaTime;

			if (_worldGenDelay <= 0f)
			{
				_worldGenerated = true;
				
				if (IsServer)
				{
					Debug.Log("Generating Resources");

					_resourceGenerator.SpawnResources();

					OnFinishedWorldGenORPC();
				}
			}
		}
	}


	[ObserversRpc(RunLocally = true)]
	private void OnFinishedWorldGenORPC()
	{
		FinishedWorldGenEvent?.Invoke();
	}
}
