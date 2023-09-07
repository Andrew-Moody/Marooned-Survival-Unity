using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
	[SerializeField] private MeshGenerator _meshGenerator;


	void Start()
	{
		_meshGenerator.GenerateTerrain(_meshGenerator.Seed);
	}
}
