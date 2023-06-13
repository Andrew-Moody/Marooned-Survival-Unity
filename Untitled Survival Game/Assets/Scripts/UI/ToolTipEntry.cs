using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolTipEntry : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _text;

	public void SetEntry(string entry)
	{
		_text.text = entry;
	}
}
