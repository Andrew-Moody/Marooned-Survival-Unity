using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextUI : MonoBehaviour
{
	[SerializeField]
	private Image _window;

	[SerializeField]
	private OptionUI _optionPF;

	private List<OptionUI> _options;

	private int _slotIndex;


	private void Start()
	{
		_options = new List<OptionUI>();
		Hide();
	}

	public void PopulateOptions(int slotIndex, List<ContextOption> options)
	{
		if (options == null)
		{
			Debug.LogError("Tried to open a context window with null options");
			Hide();
			return;
		}

		transform.parent.position = Input.mousePosition;

		_slotIndex = slotIndex;
		Show();

		Debug.Log("Populating options " + options.Count);

		if (options.Count > _options.Count)
		{
			for (int i = _options.Count; i < options.Count; i++)
			{
				OptionUI option = Instantiate(_optionPF, transform, false);
				_options.Add(option);
			}
		}


		for (int i = 0; i < _options.Count; i++)
		{
			if (i < options.Count)
			{
				_options[i].gameObject.SetActive(true);
				_options[i].SetOption(options[i]);
			}
			else
			{
				_options[i].gameObject.SetActive(false);
			}
			
		}
	}


	public void Hide()
	{
		gameObject.SetActive(false);
	}


	public void Show()
	{
		gameObject.SetActive(true);
	}
}
