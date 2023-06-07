using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenManager : NetworkBehaviour
{
	[SerializeField]
	private ResourceGenerator _resourceGenerator;

	private bool _worldGenerated = false;
	private float _worldGenDelay = 0.5f;

	

	private void Update()
	{
		if (!_worldGenerated)
		{
			_worldGenDelay -= Time.deltaTime;

			if (_worldGenDelay <= 0f)
			{
				_worldGenerated = true;
				Debug.Log("Generating Resources");

				if (IsServer)
				{
					_resourceGenerator.SpawnResources();
				}
			}
		}
	}
}
