using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : UIPanel
{
	public void OnHostPressed()
	{
		GameManager.Instance.OnHostPressed();
	}


	public void OnJoinPressed()
	{
		GameManager.Instance.OnJoinPressed();
	}


	public void OnSettingsPressed()
	{
		UIManager.ShowPanel("SettingsUI", PlayerOptions.GetSettingsData(), pushToStack: true);
	}


	public void OnQuitPressed()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
