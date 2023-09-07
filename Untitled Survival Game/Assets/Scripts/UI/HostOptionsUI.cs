using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HostOptionsUI : UIPanel
{
	[SerializeField]
	TMP_InputField _seedInput;


	public void OnStartGamePressed()
	{
		// If no seed is set use the default set by the gamemanager
		if (_seedInput.text != "")
		{
			if (!int.TryParse(_seedInput.text, out int seed))
			{
				seed = _seedInput.text.GetHashCode();
			}

			GameManager.Instance.Seed = seed;
		}
		
		GameManager.Instance.OnStartGamePressed();
	}


	public void OnBackPressed()
	{
		//UIManager.HideStackTop(true);

		//UIManager.ShowPanel("MainUI", pushToStack: true);

		GameManager.Instance.OnMainMenuPressed();
	}
}
