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
		UIManager.ShowPanel("SettingsUI", pushToStack: true);
	}
}
