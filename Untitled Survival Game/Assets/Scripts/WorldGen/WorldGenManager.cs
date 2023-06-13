using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenManager : NetworkBehaviour
{
	private static WorldGenManager _instance;
	public static WorldGenManager Instance => _instance;

	[SerializeField]
	private MeshGenerator _meshGenerator;

	[SerializeField]
	private NavMeshGenerator _navMeshGenerator;

	[SerializeField]
	private ResourceGenerator _resourceGenerator;

	
	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}


	public void GenerateWorld(int seed, bool asServer)
	{
		_meshGenerator.GenerateTerrain(seed);

		_navMeshGenerator.BuildNavMesh();

		if (IsServer)
		{
			Debug.Log("Generating Resources");

			_resourceGenerator.SpawnResources(seed);

			Debug.Log("Finished Generating Resources");
		}
	}
}
