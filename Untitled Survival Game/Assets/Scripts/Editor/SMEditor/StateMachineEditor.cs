using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StateMachineSO))]
public class StateMachineEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		StateMachineSO fsmSO = (StateMachineSO)target;

		if (GUILayout.Button("Reload States", GUILayout.Height(40)))
		{
			FSMFactorySO.ReloadFSMFactory();
		}
	}
}
