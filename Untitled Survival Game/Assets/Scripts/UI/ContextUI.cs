using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextUI : MonoBehaviour
{
	[SerializeField]
	private Transform _optionHolder;

	[SerializeField]
	private OptionUI _optionPF;

	private List<OptionUI> _options = new List<OptionUI>();


	public void Show(List<ContextOption> options)
	{
		if (options == null)
		{
			Debug.LogError("Tried to open a context window with null options");
			Hide();
			return;
		}

		// Instantiate more options if there aren't enough
		if (options.Count > _options.Count)
		{
			for (int i = _options.Count; i < options.Count; i++)
			{
				OptionUI option = Instantiate(_optionPF, _optionHolder, false);
				_options.Add(option);
			}
		}


		// Set and show options and hide the unused extras
		for (int i = 0; i < _options.Count; i++)
		{
			if (i < options.Count)
			{
				_options[i].SetOption(options[i]);
				_options[i].gameObject.SetActive(true);
			}
			else
			{
				_options[i].gameObject.SetActive(false);
			}
		}

		transform.parent.position = Input.mousePosition;

		gameObject.SetActive(true);
	}


	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
