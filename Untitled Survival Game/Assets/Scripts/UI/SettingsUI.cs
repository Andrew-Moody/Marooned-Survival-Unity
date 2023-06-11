using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsUI : UIPanel
{
	public void OnBackPressed()
	{
		UIManager.HideStackTop(true);
	}
}
