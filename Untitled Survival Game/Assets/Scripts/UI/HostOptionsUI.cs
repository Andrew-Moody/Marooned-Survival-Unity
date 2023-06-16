using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HostOptionsUI : UIPanel
{
	[SerializeField]
	TMP_InputField _input;


	public void OnStartGamePressed()
	{
		// If no seed is set use the default set by the gamemanager
		if (_input.text != "")
		{
			if (!int.TryParse(_input.text, out int seed))
			{
				seed = _input.text.GetHashCode();
			}

			GameManager.Instance.Seed = seed;
		}
		
		GameManager.Instance.OnStartGamePressed();
	}
}
