using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LessCondition : BaseCondition
{
	public string ValueName;

	public float Value;

	public override bool Evaluate(Agent agent)
	{
		float value = agent.GetBlackboardValue(ValueName);

		if (value < Value)
		{
			Debug.Log($"{ValueName}: {value} was less than {Value}");
			return true;
		}

		//return agent.GetBlackboardValue(ValueName) < Value;

		return false;
	}


	public static BaseCondition Create()
	{
		return new LessCondition();
	}


	public override BaseCondition DeepCopy()
	{
		return new LessCondition(this);
	}


	public LessCondition()
	{

	}


	public LessCondition(LessCondition condition)
	{
		NextState = condition.NextState;

		ValueName = condition.ValueName;

		Value = condition.Value;
	}
}
