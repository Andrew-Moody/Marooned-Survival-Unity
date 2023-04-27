using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
	[SerializeField]
	private StatType _statType;

	public StatType StatType { get { return _statType; } private set { _statType = value; } }

	[SerializeField]
	private Image _statFill;

	[SerializeField]
	private Image _statBackground;


	public void StatChangeHandler(StatData statData, bool immediate)
	{
		float fillWidth = _statBackground.rectTransform.rect.width * statData.CurrentValue / statData.MaxValue;

		_statFill.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fillWidth);

		//Debug.Log("Stat Changed on: " + gameObject.name);
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
