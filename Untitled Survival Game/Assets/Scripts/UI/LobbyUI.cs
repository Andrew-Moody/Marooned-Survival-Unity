using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUI : UIPanel
{
	public void OnBackPressed()
	{
		GameManager.Instance.OnMainMenuPressed();
	}
}
