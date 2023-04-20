using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshRenderable : IRenderable
{
	private SkinnedMeshRenderer _skinnedMeshRenderer;


	public SkinnedMeshRenderable(SkinnedMeshRenderer parentRig, GameObject gameObject)
	{
		_skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();

		if (_skinnedMeshRenderer == null)
		{
			Debug.LogError($"Equipment Slot {gameObject.name} is missing a SkinnedMeshRenderer");
			return;
		}

		_skinnedMeshRenderer.bones = parentRig.bones;
		_skinnedMeshRenderer.rootBone = parentRig.rootBone;
	}

	public void SetMaterial(Material material)
	{
		_skinnedMeshRenderer.sharedMaterial = material;
	}

	public void SetMesh(Mesh mesh)
	{
		_skinnedMeshRenderer.sharedMesh = mesh;
	}
}
