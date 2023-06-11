using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostOptionsUI : UIPanel
{
	public void OnStartGamePressed()
	{
		GameManager.Instance.OnStartGamePressed();
	}
}
