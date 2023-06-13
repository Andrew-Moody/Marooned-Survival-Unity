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
		int seed = _input.text.GetHashCode();

		if (!int.TryParse(_input.text, out seed))
		{
			seed = _input.text.GetHashCode();
		}

		GameManager.Instance.Seed = seed;

		GameManager.Instance.OnStartGamePressed();
	}
}
