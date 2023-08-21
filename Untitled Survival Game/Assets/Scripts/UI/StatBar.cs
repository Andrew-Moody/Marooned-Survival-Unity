using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : StatIndicator
{
	[SerializeField]
	private Image _statFill;

	[SerializeField]
	private Image _statBackground;


	public override void StatChangeHandler(UIEventData data)
	{
		if (data is UIFloatChangeEventData floatData)
		{
			float fillWidth = _statBackground.rectTransform.rect.width * floatData.Value / floatData.MaxValue;

			_statFill.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fillWidth);
		}
	}


	public override void SetAlpha(float alpha)
	{
		Color fillColor = _statFill.color;
		fillColor.a = alpha;
		_statFill.color = fillColor;

		Color backColor = _statBackground.color;
		backColor.a = alpha;
		_statBackground.color = backColor;
	}
}
