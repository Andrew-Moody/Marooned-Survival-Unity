using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatNumeric : StatIndicator
{
	[SerializeField]
	private TextMeshProUGUI _text;

	public override void StatChangeHandler(UIEventData data)
	{
		if (data is UIFloatChangeEventData floatData)
			_text.text = StatName + " " + floatData.Value;
	}


	public override void SetAlpha(float alpha)
	{
		gameObject.SetActive(alpha != 0f);
	}
}
