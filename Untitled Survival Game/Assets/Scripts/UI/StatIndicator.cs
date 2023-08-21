using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatIndicator : MonoBehaviour
{
	public string StatName => _statName;
	[SerializeField] private string _statName;


	public virtual void StatChangeHandler(UIEventData data) { }


	public virtual void SetAlpha(float alpha) { }
}

public class UIFloatChangeEventData : UIEventData
{
	public float Value { get; set; }

	public float MinValue { get; set; }

	public float MaxValue { get; set; }
}