using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
	[SerializeField]
	private string _statName;

	public string StatName { get { return _statName; } private set { _statName = value; } }

	[SerializeField]
	private Image _statFill;

	[SerializeField]
	private Image _statBackground;


	public void StatChangeHandler(UIEventData data)
	{
		if (data is UIFloatChangeEventData floatData)
		{
			float fillWidth = _statBackground.rectTransform.rect.width * floatData.Value / floatData.MaxValue;

			_statFill.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fillWidth);
		}
	}

	public void SetAlpha(float alpha)
	{
		Color fillColor = _statFill.color;
		fillColor.a = alpha;
		_statFill.color = fillColor;

		Color backColor = _statBackground.color;
		backColor.a = alpha;
		_statBackground.color = backColor;
	}
}

public class UIFloatChangeEventData : UIEventData
{
	public float Value { get; set; }

	public float MinValue { get; set; }

	public float MaxValue { get; set; }
}