using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ComponentFinder))]
public class ComponentFinderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("SearchComponents", GUILayout.Height(40)))
		{
			Undo.RecordObject(target, "Search Components");

			((ComponentFinder)target).SearchComponents();

			PrefabUtility.RecordPrefabInstancePropertyModifications(target);
		}
	}
}
