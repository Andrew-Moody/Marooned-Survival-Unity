using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlotWindow : EditorWindow
{
	private List<FunctionPlotter> _playerPlotters;

	private List<FunctionPlotter> _mobPlotters;

	[MenuItem("Window/Plot Viewer")]
	private static void OpenWindow()
	{
		PlotWindow window = GetWindow<PlotWindow>() as PlotWindow;
		window.Show();

	}

	private void OnGUI()
	{
		if (GUILayout.Button("Find Objects"))
		{
			FindObjects();
		}

		//if (_plotters != null)
		//{
		//	for (int i = 0; i < _plotters.Length; i++)
		//	{
		//		EditorGUILayout.CurveField(_plotters[i].Label, _plotters[i].Curve, GUILayout.Height(150f));
		//	}
		//}


		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.BeginVertical();

		if (_playerPlotters != null)
		{
			for (int i = 0; i < _playerPlotters.Count; i++)
			{
				EditorGUILayout.CurveField(_playerPlotters[i].Label, _playerPlotters[i].Curve, GUILayout.Height(150f));
			}
		}

		EditorGUILayout.EndVertical();


		EditorGUILayout.BeginVertical();

		if (_mobPlotters != null)
		{
			for (int i = 0; i < _mobPlotters.Count; i++)
			{
				EditorGUILayout.CurveField(_mobPlotters[i].Label, _mobPlotters[i].Curve, GUILayout.Height(150f));
			}
		}

		EditorGUILayout.EndVertical();

		EditorGUILayout.EndHorizontal();
	}


	private void FindObjects()
	{
		FunctionPlotter[] plotters = FindObjectsOfType<FunctionPlotter>();

		_playerPlotters = new List<FunctionPlotter>();
		_mobPlotters = new List<FunctionPlotter>();

		for (int i = plotters.Length - 1; i >= 0; i--)
		{
			if (plotters[i].gameObject.CompareTag("Player"))
			{
				_playerPlotters.Add(plotters[i]);
			}
			else if (plotters[i].gameObject.CompareTag("Mob"))
			{
				_mobPlotters.Add(plotters[i]);
			}
		}
	}
}
