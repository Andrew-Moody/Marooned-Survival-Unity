using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		MeshGenerator meshGenerator = (MeshGenerator)target;

		if (GUILayout.Button("Resize Mesh", GUILayout.Height(40)))
		{
			meshGenerator.CreateMesh();
		}
	}
}
