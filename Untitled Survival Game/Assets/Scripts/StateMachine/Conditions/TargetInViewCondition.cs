using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInViewCondition : BaseCondition
{
	public override bool Evaluate(Agent agent)
	{
		return Physics.CheckSphere(agent.transform.position, agent.ViewRange, agent.ViewMask);
	}


	public static BaseCondition Create()
	{
		return new TargetInViewCondition();
	}

	public override BaseCondition DeepCopy()
	{
		return new TargetInViewCondition(this);
	}


	public TargetInViewCondition() { }


	public TargetInViewCondition(TargetInViewCondition targetInViewCondition)
	{
		NextState = targetInViewCondition.NextState;
	}
}
