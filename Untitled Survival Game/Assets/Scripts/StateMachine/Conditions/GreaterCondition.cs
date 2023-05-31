using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreaterCondition : BaseCondition
{
	public string ValueName;

	public float Value;

	public override bool Evaluate(Agent agent)
	{
		//Debug.Log(agent.GetBlackboardValue(ValueName));

		return agent.GetBlackboardValue(ValueName) > Value;
	}


	public static BaseCondition Create()
	{
		return new GreaterCondition();
	}


	public override BaseCondition DeepCopy()
	{
		return new GreaterCondition(this);
	}


	public GreaterCondition()
	{

	}


	public GreaterCondition(GreaterCondition condition)
	{
		NextState = condition.NextState;

		ValueName = condition.ValueName;

		Value = condition.Value;
	}
}
