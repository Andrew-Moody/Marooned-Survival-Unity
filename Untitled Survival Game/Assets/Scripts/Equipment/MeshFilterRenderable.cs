using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFilterRenderable : IRenderable
{
	private MeshFilter _meshFilter;
	private MeshRenderer _meshRenderer;

	public MeshFilterRenderable(GameObject gameObject)
	{
		_meshFilter = gameObject.GetComponent<MeshFilter>();
		_meshRenderer = gameObject.GetComponent<MeshRenderer>();

		if (_meshFilter == null)
		{
			Debug.LogError($"Equipment Slot {gameObject.name} is missing a MeshFilter");
		}

		if (_meshRenderer == null)
		{
			Debug.LogError($"Equipment Slot {gameObject.name} is missing a MeshRenderer");
		}
	}

	public void SetMaterial(Material material)
	{
		_meshRenderer.sharedMaterial = material;
	}

	public void SetMesh(Mesh mesh)
	{
		_meshFilter.sharedMesh = mesh;
	}
}
