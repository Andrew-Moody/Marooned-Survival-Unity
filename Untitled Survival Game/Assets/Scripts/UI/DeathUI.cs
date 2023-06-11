using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathUI : UIPanel
{
	public void OnMainMenuPressed()
	{
		GameManager.Instance.OnMainMenuPressed();
	}
}
