using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreaterCondition : BaseCondition
{
	public string ValueName;

	public float Value;

	public override bool Evaluate(Agent agent)
	{
		float value = agent.GetBlackboardValue(ValueName);

		if (value > Value)
		{
			Debug.Log($"{ValueName}: {value} was greater than {Value}");
			return true;
		}

		//return agent.GetBlackboardValue(ValueName) > Value;

		return false;
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
